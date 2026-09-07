# Releasing the Server

For whoever cuts the release. Nothing here is addressed to somebody installing
the product — that is [AlgoJudge-Ops](https://github.com/AlgoJudge/AlgoJudge-Ops)
and the documentation site.

Every dated figure below was measured on the date it names, on `release/0.1.0`
at `fa9bc29`. Anything undated is a rule rather than a reading.

## Where the version lives

**`Directory.Build.props`, one line.** Both projects inherit it, so a release
changes that file and nothing else. Without it MSBuild uses 1.0.0, which is what
this repository shipped as until 0.1.0 because nobody had said otherwise.

**`openapi.json` does not carry it.** Its `info.version` is `1.0` and stays
there: that is the version of the **API**, which is served at `/api/v1` and does
not move because the product released. Changing it would move the REST reference
on the documentation site, which pins this file by commit and checksum.

## What a tag does

`.github/workflows/release.yml` runs on a pushed tag matching `v*`, and **only
then** — nothing that lands on `main` reaches the registry on its own. It
refuses a tag that does not point at a commit on `main`, and a name that is not
`v<major>.<minor>.<patch>[-prerelease]`.

For `v0.1.0` it publishes one image, `ghcr.io/algojudge/algojudge-server`, under
four tags:

| | |
|---|---|
| `0.1.0` | the release |
| `0.1` | the moving minor |
| `0` | the moving major, and what an installation asks for by default |
| `latest` | |

**A prerelease publishes its own tag alone.** `v0.1.0-rc.1` gets `0.1.0-rc.1`
and nothing moving, because the point of a release candidate is that somebody
asked for it by name.

The workflow builds, checks the image carries the application and `aj-admin`,
and pushes. It does **not** re-run the test suite: a tag points at a commit, and
that commit's own CI run is the evidence. It also does **not** create a GitHub
Release and writes no release notes — it holds `contents: read`, and the only
thing it writes is the package.

## The migrations are squashed into one, named for the release

**Before each release, every migration added since the previous release becomes
one migration named `version_<major>_<minor>_<patch>`.** For 0.1.0 that is
`version_0_1_0`, and because nothing has ever been released every migration is
unreleased and every one of them goes into it.

**Only unreleased migrations are ever squashed, and that is what makes the rule
safe.** A database at 0.1.0 carries `…_version_0_1_0` in its history and reaches
0.1.1 by applying `…_version_0_1_1` on top of it; no released history row is
ever removed, so no released database is ever stranded. The only database that
cannot cross a squash is one migrated from unreleased code — a developer's own
stack, whose history names files that no longer exist. Those are disposable:
`docker compose … down -v`. For 0.1.0 there is not even that to weigh, because
nothing has been released at all: `ghcr.io/algojudge` is empty and no repository
carries a `v*` tag (checked 2026-09-07).

### Where they live

Two contexts, two chains, two history tables, and a command that names no
context gets the first.

| | migrations | model snapshot | history table |
|---|---|---|---|
| `ApplicationDbContext` | `AlgoJudge.Server/Database/Migrations` | `ApplicationDbContextModelSnapshot.cs` | `__EFMigrationsHistory` |
| `LtiDbContext` | `AlgoJudge.Server/Lti/Migrations` | `LtiDbContextModelSnapshot.cs` | `__EFMigrationsHistory_Lti` |

**Eight and one on 2026-09-07.** Both get a `version_0_1_0`; the two classes sit
in different namespaces, so the name does not collide. The LTI one is a rename
of a single migration rather than a merge of several, and is done the same way.

### What a regeneration silently drops

Three things, all of them measured on 2026-09-07 rather than remembered.

1. **The `FileContents` block**, at the end of `InitialCreate.Up`, with its
   `DROP TABLE IF EXISTS` at the start of `Down`. It is not an EF entity — the
   postgres blob store reads and writes those bytes with raw SQL — so
   `dotnet ef migrations add` does not produce it, and neither does the
   `ALTER TABLE … SET STORAGE EXTERNAL` that keeps a ranged read seekable.
   **Copy it out of the file before deleting anything.** `FileStorageSchemaTests`
   is what fails if it goes.

2. **Three column defaults the model does not declare**: `EvaluationJobs.Releases`
   and `EvaluationJobs.Refunds` (`DEFAULT 0`), and `Instance.ShowHero`
   (`DEFAULT true`). Every one came from an `AddColumn(defaultValue: …)` whose
   job was to backfill a table that already had rows, and a table created in one
   statement has no rows to backfill. **Let them go** — the same decision as the
   eleven dropped in the 2026-08-28 squash. Each is matched by a CLR initializer
   (`int` is 0; `Instance.ShowHero` is `= true`), and putting them in the model
   with `HasDefaultValue` would be worse than losing them, because EF omits a
   property whose value equals the CLR default. They are the **expected**
   difference in the schema comparison below.

3. **The comments.** The XML summary on `InitialCreate`, and the note above
   `ShowHero`'s `defaultValue: true` recording that the generator wrote `false`
   and that it was corrected by hand. That correction has no successor after the
   squash and needs none: on a database built from one migration the column
   arrives with the row.

**Nothing else in either chain is hand-written.** Every check constraint, every
filtered index and `RunnerTags`' `defaultValueSql` is declared in the model —
`ApplicationDbContextModelSnapshot.cs` carries nine `HasFilter`, four
`HasCheckConstraint` and one `HasDefaultValueSql`, so the differ reproduces all
of them.

### How it is done

Nothing here is a dry run. Do it on a branch, with the tree clean.

**0.** `dotnet tool restore` — `dotnet-ef` is pinned at 10.0.11 in
`.config/dotnet-tools.json`, with `rollForward: false`. Docker running.

**1. Record the schema the current chain builds**, from an empty database, into
a directory outside this repository:

```sh
compose="docker compose -f example-server-development-docker-compose.yaml"
$compose down -v
$compose up -d --build --wait
$compose exec -T postgres pg_dump --schema-only --no-owner --no-privileges \
    -U algojudge algojudge > ../before.sql
```

The stack applies migrations at start because it runs as Development. 2610 lines
on 2026-09-07.

**2. Copy the `FileContents` block out**, into that same directory. Not to a
stash and not to a branch: `git checkout` is how it gets lost.

**3. Delete the whole migrations directory for a context — snapshot included —
and regenerate:**

```sh
rm AlgoJudge.Server/Database/Migrations/*.cs
dotnet ef migrations add version_0_1_0 \
    --project AlgoJudge.Server --context ApplicationDbContext \
    --output-dir Database/Migrations

rm AlgoJudge.Server/Lti/Migrations/*.cs
dotnet ef migrations add version_0_1_0 \
    --project AlgoJudge.Server --context LtiDbContext \
    --output-dir Lti/Migrations
```

**The snapshot goes with them.** It is the differ's *before*: leave it in place
and the new migration comes out empty. **`--output-dir` is not optional** once
the directory is bare — EF follows the last migration's directory, and there is
no longer one to follow.

**4. Paste the `FileContents` block back**, with its comment: the `Sql` call at
the end of `Up`, the `DROP TABLE IF EXISTS` at the start of `Down`.

**5. Build and test.**

```sh
dotnet build AlgoJudge.sln -c Release -warnaserror
dotnet test AlgoJudge.sln -c Release --no-build
```

`MigrationsDescribeTheModelTests` proves the new snapshot still describes both
models; `FileStorageSchemaTests` proves `FileContents` came back and that its
`attstorage` is still `e`. A migration class named `version_0_1_0` compiles with
no warning under `-warnaserror` — checked 2026-09-07 against the same SDK and
the same project shape, so the lower-case name and the underscores cost nothing.

**6. Compare the two schemas.** This is the check that the squash is honest, and
reading the generated file is not a substitute for it.

```sh
$compose down -v
$compose up -d --build --wait
$compose exec -T postgres pg_dump --schema-only --no-owner --no-privileges \
    -U algojudge algojudge > ../after.sql
diff -u ../before.sql ../after.sql
```

**Read the diff; do not expect it to be empty.** One `CREATE TABLE` writes its
columns in model order where a chain appended them, so columns move. What is
allowed to differ, and nothing else:

- the three defaults above, gone;
- no other `DEFAULT` changed — `RunnerTags`' `'{}'::text[]` stays, because that
  one is in the model;
- `FileContents` still there, still `SET STORAGE EXTERNAL`.

**7. One history row per context.**

```sh
$compose exec -T postgres psql -U algojudge -d algojudge \
    -c 'SELECT * FROM "__EFMigrationsHistory"' \
    -c 'SELECT * FROM "__EFMigrationsHistory_Lti"'
```

Eight rows and one before, on 2026-09-07; one and one after.

**8.** `$compose down -v`, and regenerate `openapi.json` from a stack that is up
if anything about the API moved. The squash alone does not move it.

## Before the tag

- [ ] `Directory.Build.props` says the version being released. **`0.1.0` there
      on 2026-09-07.**
- [ ] `README.md` names that version where it shows a `docker pull` — line 245,
      `ghcr.io/algojudge/algojudge-server:0.1.0` on 2026-09-07.
- [ ] **The migrations are squashed into `version_0_1_0`**, both contexts, by
      the section above.
- [ ] **The commit is on `main`**, and **its** CI run is green — not a later
      one. `release/0.1.0` is not `main`, and the workflow refuses a tag that is
      not an ancestor of it: on 2026-09-07 this branch was one commit ahead of
      `origin/main` (`fa9bc29`) and none behind, so it has to land there first.
      `main` was green at `425d2c7`.
- [ ] `dotnet restore AlgoJudge.sln`, then
      `dotnet build AlgoJudge.sln -c Release --no-restore -warnaserror`. The
      release build treats **every warning as an error**; the count to aim at is
      zero, and it was zero on 2026-09-07 as it has been since 2026-08-29.
- [ ] `dotnet test AlgoJudge.sln -c Release --no-build`. Docker has to be
      running — the suite starts a real PostgreSQL 18 per run.
- [ ] The development stack comes up and answers: the `compose` job in
      `.github/workflows/ci.yml` is the list, and the one to run by hand if
      anything about configuration changed.
- [ ] **`openapi.json` matches what the container serves.** Regenerate it from
      the running **development** stack — the Swagger endpoint is mapped under
      `IsDevelopment()` in `Program.cs`, so the released image with a production
      environment does not serve it at all. `curl` its
      `/api/v1/swagger/v1/swagger.json`, never a test host, and commit any
      difference; `README.md` carries the three commands, `--build` included.
      CI compares the two textually. Identical on 2026-09-07,
      `sha256 79f61ee5…`.
- [ ] **Nothing is vulnerable, and what is behind is behind on purpose.**

      ```sh
      dotnet list AlgoJudge.sln package --vulnerable --include-transitive
      dotnet list AlgoJudge.sln package --outdated
      ```

      2026-09-07: **no vulnerable package in either project**, transitive
      included. Two are one patch behind — `AWSSDK.S3` 4.0.102.4 → 4.0.102.5 and
      `Testcontainers.PostgreSql` 4.14.0 → 4.15.0. Neither was taken here;
      whether to take them is the owner's call, and neither is a reason to hold
      a release.
- [ ] **The .NET version is the one this targets.** `net10.0` in both projects,
      `aspnet:10.0` and `sdk:10.0` in the Dockerfile, `10.0.x` on CI, and
      `10.0.400` locally on 2026-09-07. .NET 10 is the LTS; .NET 8 leaves
      support on 2026-11-10.
- [ ] **`.env.example`, checked in both directions.** Everything the development
      compose substitutes is listed, and nothing is listed that it does not
      substitute. Three on 2026-09-07 — `AJ_ADMIN_TOKEN`,
      `AJ_STORAGE_ACCESS_KEY`, `AJ_STORAGE_SECRET_KEY` — and the two sets match
      exactly. **Nothing checks this for you here**, so read the substitutions
      in the compose file against the keys in the file. **The Server's own
      configuration does not belong in it**: an installation is configured with
      `AJ_`-prefixed variables on the Server's environment, which `README.md`
      and the documentation site carry, and which no `.env` is involved in.
- [ ] **No `.env` in the repository, only `.env.example`.** Working tree *and*
      index, because `.gitignore` covering `.env` and `.env.*` does not
      un-track a file already added.

      ```sh
      find . -name '.env*' -not -path './.git/*'
      git ls-files | grep -i env
      ```

      Both named `.env.example` alone on 2026-09-07. If a real one turns up,
      report that it exists and do not open it.
- [ ] **The documentation describes the software as it is.** `README.md`,
      `AlgoJudge.Server/README.md`, `AUTHORS.md` and `AUTHORS.txt` — which
      `git shortlog -sne --all` still agrees with — `CLAUDE.md`, this file, and
      the comments in `example-server-development-docker-compose.yaml`,
      `AlgoJudge.Server/Dockerfile`, `AlgoJudge.Server/aj-admin` and the two
      workflows. `preconfig.example/pages/*.md` are an installation's own
      content, not documentation.

      On 2026-09-07 all of those held except two, neither of them this file's to
      fix:
      - `CLAUDE.md:407` — "**The schema is one migration per context**" was true
        on 2026-08-28 and is not now; seven followed. The squash above makes it
        true again, and the sentence should be re-read rather than assumed.
      - `.github/workflows/ci.yml:82` — "`--wait` only waits for a healthcheck,
        and the application service has none". It has had one since 2026-08-08
        (`fb6f122`); that comment is from 2026-08-03 (`e1b0700`). The polling
        under it is still right; its stated reason is not.

## After the tag

The image exists before an installation can pull it, so the order across
repositories is **Server and Client, then `AlgoJudge-Runner`, then
`AlgoJudge-External-Runner`, then `AlgoJudge-Ops`.**

The two Runners are not interchangeable in that order.
`AlgoJudge-External-Runner` pins `aj-protocol` at a Git revision of
`AlgoJudge-Runner` (`Cargo.toml:15`) and its own runbook says to move that pin
to the commit the Runner's tag points at — so it cannot be released until that
tag exists. `AlgoJudge-Ops` comes last because it pulls every image by tag,
`algojudge-server:${SERVER_TAG:-0}` among them (`compose.yaml:90`), and its own
CI says the full-stack check waits on the first release because
`algojudge-server:0` resolves to nothing (`.github/workflows/check.yml:97`).

### What this repository owes the others

- **The image, under all four tags.** Nothing downstream can be tested against a
  registry holding nothing.
- **The package has to be readable.** Nothing has ever been pushed, so its
  visibility on GitHub could not be checked here. `AlgoJudge-Ops` records that
  the first release includes a one-time flip of seven packages to public
  (`.github/workflows/check.yml:98`). Confirm it after the first push and before
  telling anybody to pull.
- **`openapi.json` and `events.json` at the released commit.**
  `AlgoJudge-Client` checks itself against both, in `scripts/check-api.mjs` and
  `scripts/check-events.mjs`. `AlgoJudge-Docs` fetches `openapi.json` by pinned
  ref and verifies a SHA-256 (`content-sources.json`), and says there that the
  pin becomes the tag once one exists — so hand over `v0.1.0` and the checksum
  of the file at it. On 2026-09-07 that pin was still a commit, `e01247c` of
  2026-09-04, five commits behind this branch's `openapi.json`.
- **Nothing else pins this repository's release number.** The Runners speak the
  wire contract, not the Server's version.

The documentation site cuts its `/server/` snapshot on release day, from
`AlgoJudge-Docs`.
