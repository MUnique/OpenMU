# MUnique.OpenMU.Measurements

A small console tool which measures the startup and loading costs of OpenMU:
plugin discovery and proxy compilation, EF Core model building, and loading the
game configuration and accounts. It provides the baseline for
[#1109](https://github.com/MUnique/OpenMU/issues/1109).

Each measurement runs in a fresh process, so the first ("cold") numbers include
JIT compilation and first-time initialization, like at server start.

## Usage

The database connection is taken from `ConnectionSettings.xml` of the EntityFramework project.
The generated persistence code is checked in, so build with `-p:ci=true`:

```
dotnet build -c Release -p:ci=true
cd bin/Release
./MUnique.OpenMU.Measurements init       # drops and re-creates the database with season 6 data and test accounts
./MUnique.OpenMU.Measurements all        # runs all measurements, each in its own process
./MUnique.OpenMU.Measurements snapshot <folder>   # canonical JSON snapshots for comparison tests
```

Single modes: `plugins`, `proxies`, `efmodel`, `config`, `config-cold`, `account`.
Options: `--iterations <n>` (warm iterations, default 5), `--accounts <a,b,...>` (default `test0,test400,testgm`).

The snapshots are canonicalized (array items sorted by id), so two snapshots of the same data are byte-wise
equal. This allows comparing the output of a new loader or converter with the current one.

## Baseline (2026-10-06)

Environment: 4 logical cores, Linux, .NET 10.0.12, PostgreSQL 16.15 with default settings (JIT enabled),
database on the same machine, season 6 data.

### Plugin discovery and registration

| Measurement | Time (ms) | Allocated (MB) | Note |
|---|---:|---:|---|
| 1st PlugInManager: DiscoverAndRegisterPlugIns (cold) | 2246.5 | 26.4 | 740 plugins, 24 compiled proxies |
| 2nd PlugInManager: DiscoverAndRegisterPlugIns | 492.0 | 18.5 | proxies are compiled again |
| Assembly scan for [PlugIn] types only | 307.1 | 3.4 | |
| PlugInConfiguration.Name (one access) | 13.8 | 0.1 | scans all types of all assemblies on every access |

### Plugin proxy compilation (Roslyn at runtime)

| Measurement | Time (ms) | Allocated (MB) | Note |
|---|---:|---:|---|
| First proxy (cold) | 1368.3 | 8.7 | includes loading and warming up Roslyn |
| Following proxies (median of 23) | 19.2 | 0.6 | |
| All 24 proxies | 2287.6 | 22.5 | |
| All 24 proxies again (warm) | 420.3 | 13.2 | |

### EF Core model building

| Measurement | Time (ms) | Allocated (MB) | Note |
|---|---:|---:|---|
| EntityDataContext (cold, first model) | 2111.7 | 88.3 | includes EF Core startup and JIT |
| ConfigurationContext | 965.5 | 81.6 | |
| AccountContext | 738.4 | 60.0 | |
| TradeContext | 807.3 | 59.7 | |
| GuildContext | 12.5 | 1.0 | |
| FriendContext | 4.5 | 0.3 | |
| TypedContext for the 10 edit types used by the servers | 5435.8 | 547.4 | one model per edit type |
| TypedContext for the other 105 entity types | 20159.8 | 5793.9 | worst case, e.g. admin panel |

### Game configuration loading

| Measurement | JIT on (ms) | JIT off (ms) | Note |
|---|---:|---:|---|
| End-to-end: GameConfigurationContext.GetByIdAsync (cold) | 14584.5 | 10269.3 | includes EF model building and .NET JIT |
| End-to-end: GameConfigurationContext.GetByIdAsync (warm, median of 5) | 7764.0 | 1324.2 | |
| Database server: planning | 41.6 | 42.5 | query has 46,105 characters |
| Database server: execution (EXPLAIN ANALYZE) | 7020.8 | 1705.6 | EXPLAIN adds timing overhead |
| Query execution + reading JSON (warm, median of 5) | 7470.8 | 1159.1 | 21.6 MB of JSON |
| Deserialization from memory (warm, median of 5) | 549.6 | 503.1 | allocates ~45 MB |

"JIT" here is the PostgreSQL JIT compiler (LLVM), see the findings below.

### Game configuration loading, each step cold (`config-cold`)

PostgreSQL was restarted before each run, so its shared buffers were empty.

| Step | JIT on (ms) | JIT off (ms) | Note |
|---|---:|---:|---|
| EF model of the EntityDataContext | 2806.0 | 2016.0 | required to build the query |
| Building the JSON query | 14.1 | 12.0 | |
| Opening the database connection | 390.2 | 283.8 | |
| Query execution + reading JSON | 8450.0 | 612.6 | 21.6 MB of JSON |
| Deserialization from memory | 1622.9 | 1535.4 | includes creating the converters; ~0.5 s when warm |
| End-to-end after the steps above | 9728.8 | 3751.6 | remaining cold costs, e.g. the model of the ConfigurationContext |

### Account loading (warm, median of 5)

| Account | End-to-end (ms) | DB execution (ms) | Query + reading JSON (ms) | Deserialization (ms) | JSON size |
|---|---:|---:|---:|---:|---:|
| test0 | 25.5 | 3.6 | 5.4 | 8.5 | 0.11 MB |
| test400 | 49.1 | 7.3 | 7.7 | 14.5 | 0.19 MB |
| testgm | 41.6 | 4.3 | 7.7 | 19.2 | 0.20 MB |

The first (cold) account load takes about 1.1 s, mostly EF model building of the `AccountContext` and .NET JIT.
End-to-end includes the login name lookup and attaching the graph to the EF context.

## Findings

1. **PostgreSQL JIT dominates the configuration loading.** The configuration JSON query has a huge cost estimate
   because of its many correlated subqueries, so PostgreSQL compiles it with LLVM: 1038 functions,
   about 7.1 s of the 7.5 s execution time ("Optimization 3818 ms, Emission 3135 ms").
   With `jit = off`, the same query executes in about 0.5 s (measured with psql), and the warm end-to-end
   loading drops from 7.8 s to 1.3 s. The account queries are below the JIT cost threshold and not affected.
   This can be fixed without any refactoring, e.g. by `Options=-c jit=off` in the connection string,
   or by disabling JIT for the JSON queries only. Things which don't help (measured with psql):
   - **Prepared statements:** PostgreSQL doesn't cache JIT-compiled code. Every `EXECUTE` of a prepared
     statement compiles again (~8 s each), also with a cached generic plan (`plan_cache_mode = force_generic_plan`).
   - **Table statistics:** after data initialization, 58 of 111 tables have never been analyzed, because
     autovacuum only analyzes a table after 50 changed rows. `ANALYZE` lowers the cost estimate from 1.3 billion
     to 2.9 million, but that is still far above `jit_optimize_above_cost` (500,000), so the query is still
     compiled with full optimization (928 functions, ~7.7 s).
   - **JIT without optimization and inlining** (`jit_optimize_above_cost = -1`, `jit_inline_above_cost = -1`):
     1.1–1.7 s, still slower than without JIT (0.5–0.9 s).
   The query consists of hundreds of small correlated subqueries with little data each, so the compilation
   can't pay off at execution time.
2. **Runtime proxy compilation costs about 2.3 s at startup**, and it's repeated for every `PlugInManager` instance
   (another ~0.4–0.5 s each). Scanning the assemblies for plugins costs another ~0.3 s.
3. **EF Core model building costs several seconds at startup** (about 4.6 s for the main contexts),
   plus about 0.5 s for each typed context on first use (5.4 s for the 10 edit types used by the servers).
4. **Deserialization of the configuration costs about 0.5 s and 45 MB** of allocations when warm, and about
   1.5 s cold (creating the converters with compiled expressions, and .NET JIT). For accounts it's
   8–19 ms, which is about a third of the end-to-end login loading time.
5. **Without PostgreSQL JIT, the query is also fast when cold:** 0.6 s after a restart of PostgreSQL.
   The cold loading time is then dominated by the .NET side: EF model building, creating the converters
   and .NET JIT.
