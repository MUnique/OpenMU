#!/usr/bin/env python3
"""
Translate missing docs/Packets/*.md files into docs/vietnamese/packets/.
Uses structural replacements + Google Translate (deep-translator) with caching.

Run from repo root:
  . .venv_pkt/bin/activate && python scripts/translate_missing_packet_docs.py
"""

from __future__ import annotations

import os
import re
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SRC_DIR = ROOT / "docs" / "Packets"
DST_DIR = ROOT / "docs" / "vietnamese" / "packets"


def load_cache() -> dict[str, str]:
    return {}


SECTION_MAP = {
    "## Is sent when": "## Được gửi khi nào",
    "## Causes the following actions on the server side": "## Hành động phía server",
    "## Causes the following actions on the client side": "## Hành động phía client",
    "## Structure": "## Cấu trúc",
}


def fix_title(line: str) -> str:
    if not line.startswith("#"):
        return line
    line = line.replace(" (by client)", " (client gửi)")
    line = line.replace(" (by server)", " (server gửi)")
    return line


def fix_md_links(text: str) -> str:
    text = text.replace("](PacketTypes.md)", "](../../Packets/PacketTypes.md)")
    text = text.replace("](Appearance.md)", "](../../Packets/Appearance.md)")
    text = text.replace("](ServerToClient.md)", "](../../Packets/ServerToClient.md)")
    text = text.replace("](ClientToServer.md)", "](../../Packets/ClientToServer.md)")
    text = text.replace("](ChatServer.md)", "](../../Packets/ChatServer.md)")
    return text


def fix_subsection_headers(text: str) -> str:
    text = re.sub(
        r"^###\s+([A-Za-z0-9_]+)\s+Structure\s*$",
        r"### Cấu trúc \1",
        text,
        flags=re.MULTILINE,
    )
    text = re.sub(
        r"^###\s+([A-Za-z0-9_]+)\s+Enum\s*$",
        r"### Enum \1",
        text,
        flags=re.MULTILINE,
    )
    text = re.sub(
        r"^\*\*Length:\*\*\s*(\d+)\s*Bytes\s*$",
        r"**Độ dài:** \1 byte",
        text,
        flags=re.MULTILINE | re.IGNORECASE,
    )
    text = re.sub(
        r"^\*\*Contains the data of (.+)\.\*\*\s*$",
        r"**Chứa dữ liệu của \1.**",
        text,
        flags=re.MULTILINE,
    )
    return text


def looks_english_prose(s: str) -> bool:
    s = s.strip()
    if not s or len(s) < 3:
        return False
    if re.match(r"^[\d\s|.\-:/\\xX+\[\]\(\)]+$", s):
        return False
    letters = sum(c.isalpha() for c in s)
    return letters >= 3 and any("a" <= c.lower() <= "z" for c in s if c.isalpha())


def split_table_row(line: str) -> list[str]:
    if not line.strip().startswith("|"):
        return []
    parts = line.split("|")
    inner = parts[1:-1] if len(parts) > 2 else parts[1:]
    return [p.strip() for p in inner]


def is_separator_row(line: str) -> bool:
    inner = "|".join(split_table_row(line)) if line.startswith("|") else ""
    return bool(re.match(r"^[\s\-:|]+$", inner))


def translate_table_lines(lines: list[str], tr, cache: dict[str, str]) -> list[str]:
    if not lines:
        return lines
    header = lines[0]
    cols_h = split_table_row(header)
    mode = None
    if len(cols_h) >= 5 and cols_h[0].strip() == "Index":
        mode = "five"
    elif len(cols_h) >= 3 and cols_h[0].strip() == "Value":
        mode = "three_enum"
    elif len(cols_h) >= 3 and cols_h[0].strip() == "First byte":
        mode = "packet_types"

    out: list[str] = []
    for line in lines:
        if not line.startswith("|"):
            out.append(line)
            continue
        if is_separator_row(line):
            out.append(line)
            continue
        cols = split_table_row(line)
        if mode == "five" and len(cols) >= 5:
            desc = cols[-1]
            if looks_english_prose(desc):
                cols[-1] = translate_cached(desc, tr, cache)
            out.append("| " + " | ".join(cols) + " |")
        elif mode == "three_enum" and len(cols) >= 3:
            desc = cols[-1]
            if looks_english_prose(desc):
                cols[-1] = translate_cached(desc, tr, cache)
            out.append("| " + " | ".join(cols) + " |")
        elif mode == "packet_types" and len(cols) >= 4:
            for i in (1, 2, 3):
                if len(cols) > i and looks_english_prose(cols[i]):
                    cols[i] = translate_cached(cols[i], tr, cache)
            out.append("| " + " | ".join(cols) + " |")
        else:
            if len(cols) >= 3:
                desc = cols[-1]
                if looks_english_prose(desc):
                    cols[-1] = translate_cached(desc, tr, cache)
                out.append("| " + " | ".join(cols) + " |")
            else:
                out.append(line)
    return out


SKIP_EXACT = frozenset(
    {
        "Description",
        "Data Type",
        "Index",
        "Length",
        "Value",
        "Name",
        "Type",
        "First byte",
        "Encrypted Server -> Client",
        "Encrypted Client -> Server",
    }
)


def translate_cached(text: str, tr, cache: dict[str, str]) -> str:
    key = text.strip()
    if key in SKIP_EXACT:
        return key
    if key in cache:
        return cache[key]
    try:
        vi = tr.translate(key)
    except Exception:
        vi = key
        print(f"[warn] translate failed for: {key[:80]}...", file=sys.stderr)
    cache[key] = vi
    time.sleep(0.03)
    return vi


def translate_prose_line(line: str, tr, cache: dict[str, str]) -> str:
    if line.startswith("#") or line.startswith("|"):
        return line
    lstripped = line.lstrip()
    indent = line[: len(line) - len(lstripped)]
    if lstripped.startswith("- ") or lstripped.startswith("* "):
        marker = lstripped[:2]
        rest = lstripped[2:]
        if rest.startswith("["):
            return line
        if looks_english_prose(rest):
            trd = translate_cached(rest.strip(), tr, cache)
            return indent + marker + trd
        return line
    if line.startswith("["):
        return line
    stripped = line.strip()
    if not stripped:
        return line
    if looks_english_prose(stripped):
        out = translate_cached(stripped, tr, cache)
        if line.startswith(" "):
            return line.replace(stripped, out, 1)
        return out
    return line


def normalize_doc_links(text: str) -> str:
    """Keep stable English anchor text for technical cross-links."""
    text = re.sub(
        r"\[[^\]]*\]\((\.\./\.\./Packets/PacketTypes\.md)\)",
        r"[Packet type](\1)",
        text,
    )
    text = re.sub(
        r"\[[^\]]*\]\((\.\./\.\./Packets/Appearance\.md)\)",
        r"[Appearance](\1)",
        text,
    )
    return text


def post_process_vi(text: str) -> str:
    fixes = [
        ("Thiệt hại", "Sát thương"),
        ("thiệt hại", "sát thương"),
        ("Người chơi PK", "PK"),
    ]
    for a, b in fixes:
        text = text.replace(a, b)
    text = normalize_doc_links(text)
    return text


def translate_document(raw: str, tr, cache: dict[str, str]) -> str:
    text = raw.replace("\r\n", "\n")
    for en, vi in SECTION_MAP.items():
        text = text.replace(en + "\n", vi + "\n")

    lines = text.split("\n")
    # Title
    if lines:
        lines[0] = fix_title(lines[0])

    out_lines: list[str] = []
    i = 0
    while i < len(lines):
        line = lines[i]
        if line.startswith("|"):
            block = []
            while i < len(lines) and lines[i].startswith("|"):
                block.append(lines[i])
                i += 1
            out_lines.extend(translate_table_lines(block, tr, cache))
            continue
        out_lines.append(translate_prose_line(line, tr, cache))
        i += 1

    text = "\n".join(out_lines)
    text = fix_md_links(text)
    text = fix_subsection_headers(text)
    text = post_process_vi(text)
    return text


def main() -> None:
    try:
        from deep_translator import GoogleTranslator
    except ImportError:
        print("Install: . .venv_pkt/bin/activate && pip install deep-translator", file=sys.stderr)
        sys.exit(1)

    tr = GoogleTranslator(source="en", target="vi")
    cache: dict[str, str] = {}

    en_files = sorted(p.name for p in SRC_DIR.glob("*.md"))
    vi_files = set(p.name for p in DST_DIR.glob("*.md"))
    missing = [f for f in en_files if f not in vi_files]

    print(f"Total EN: {len(en_files)}, VI: {len(vi_files)}, Missing: {len(missing)}")
    DST_DIR.mkdir(parents=True, exist_ok=True)

    for idx, name in enumerate(missing, 1):
        src = SRC_DIR / name
        dst = DST_DIR / name
        raw = src.read_text(encoding="utf-8")
        vi = translate_document(raw, tr, cache)
        dst.write_text(vi, encoding="utf-8")
        if idx % 25 == 0:
            print(f"[{idx}/{len(missing)}] wrote {name} (cache size {len(cache)})")

    print(f"Done. Wrote {len(missing)} files to {DST_DIR.relative_to(ROOT)}")
    print(f"Translation cache entries: {len(cache)}")


if __name__ == "__main__":
    main()
