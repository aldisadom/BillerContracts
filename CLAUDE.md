# BillerContracts

Request/response records, enums and typed HTTP clients for the Biller API. Published to NuGet as
two packages that always share one version.

## Projects
- `Contracts` → package `BillerContracts`: records in `Requests/<Resource>/` and `Responses/<Resource>/`, enums in `Enums/`.
- `Clients` → package `BillerContracts.Clients`: one client per resource (`Clients/<X>Client.cs` and
  `Interfaces/I<X>Client.cs`) on top of `BaseHttpClient`, registered in `DependencyInjection.cs`,
  with URLs from `ClientsOptions`.
- `Clients.Tests`: xUnit v2 and FluentAssertions. `FakeHttpMessageHandler` checks the method, URL and body each client sends.

## Who uses it
- Biller: `BillerContracts` in `src/Domain/Domain.csproj`.
- BillioIntegrationTest: `BillerContracts` in `src/IntegrationTests/IntegrationTests.csproj`.
- BillerFrontEnd: `BillerContracts.Clients` in `src/FrontEnd/FrontEnd.csproj`.

A change reaches them only after a publish and a version bump in each repo.

## Conventions
- The namespace is `BillerContracts.` plus the folder path (`Requests/User/` → `BillerContracts.Requests.User`).
- Records use `{ get; set; }` with `= string.Empty` / `= []` defaults. List responses carry the list plus `TotalCount`, `Page` and `PageSize`.
- Biller stores enums as numbers, and clients send them in query strings by name (`PageSize=p25`).
  Add new values at the end. Never change a number or rename a member.
- A new Biller endpoint needs a method on its client and interface, and a client test.
- Renaming or removing a property, enum member or namespace breaks consumers: bump the major version.
- No AutoMapper or any other mapping library, in any Biller repo. Map with hand-written extension methods.
- Plans for work only in this repo go in `docs/plans/BIL-<n>-<slug>.md` here. Plans that change
  several repos live in Biller's `docs/plans/`.
- Before reading, comparing or using another Biller repo, put it on the branch with this branch's
  name, else on up-to-date master:
  `bash ../Biller/.claude/skills/_sync-repos/scripts/sync-repos.sh --branch "$(git branch --show-current)" Biller BillerFrontEnd BillioIntegrationTest`
  (add `--sync` to switch and fast-forward).

## Publishing (the owner's action)
Run the `Publish` workflow (GitHub Actions, input `package_version` as `MAJOR.MINOR.PATCH`). It runs
CI first. From master it publishes that version. From another branch it publishes
`<version>-preview.<run>`, so consumers can try a change before it is merged.

## Before a PR
1. `dotnet build -c Release` and `dotnet test -c Release`. Paste the `Passed!/Failed!` line.
2. Run `/code-review` on the branch and tell it to follow `REVIEW.md`. Fix Important findings,
   answer the rest, at most 3 rounds.
3. Fill `.github/pull_request_template.md`, including whether the change is breaking.
