# Protocol Replay Test Scaffold

This directory is the initial scaffold for protocol replay testing, based on
`docs/adr/ADR-Protocol-Replay-Testing-Strategy.md`.

## Structure

- `golden/`: captured packet streams considered baseline truth.
- `fixtures/`: canonical semantic messages used for encode tests.
- `whitelist/`: volatile fields allowed to differ in comparisons.

## First flow: login

- Golden metadata: `golden/login/metadata.json`
- Golden stream sample: `golden/login/stream.hex.txt`
- Canonical fixture: `fixtures/login/login-request.json`

## Next implementation steps

1. Add replay runner in a dedicated test project.
2. Decode `stream.hex.txt` and assert packet sequence.
3. Encode from `login-request.json` and diff against expected bytes.
4. Honor `whitelist/default.json` for volatile fields.
