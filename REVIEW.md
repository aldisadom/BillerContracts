# Review rules for BillerContracts

Failure here looks like:
- A consumer breaks after upgrading: a property, enum member or namespace was renamed or removed, or an enum number changed, without a major version bump.
- A type is in the wrong namespace for its folder (`UserGetRequest` was in `BillerContracts.Requests.Seller`).
- A client calls the wrong URL or HTTP method, or sends a body where Biller reads `[FromQuery]` (`POST invoices/generate`).
- A Biller endpoint has no client method, or a new client isn't registered in `DependencyInjection.cs`.
- A default value changes behaviour for callers that don't set it (`InvoiceGenerateRequest.LanguageCode` is `LT`, `InvoiceStatus` 0 is `Payed`).
- A response gets a field that must never leave Biller (password, hash, salt).
- A mapping library (AutoMapper or similar) is added.
- A client test passes whatever the client sends (no `ShouldHaveSent`, or the expected URL built from the client's own code).

Skip: `bin/`, `obj/`.

Tag each finding **Important**, **Nit**, or **Pre-existing** (the bug was there before this change). At most 2 Nits. No naming or formatting comments.

Every finding cites `file:line` and a concrete input that fails. No guesses from names.

Start with the number of Important findings. If there are none, say "No issues found."

When the same kind of bug appears in several places, report it once and list every place.

When a finding repeats across reviews, propose a rule for `CLAUDE.md` or a test instead of repeating it.
