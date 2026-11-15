# BillerContracts

[Lietuviškai](docs/README_lt.md)

The shared types of the Biller API: request and response records, enums, and typed HTTP clients.
They are published to nuget.org as two packages that always have the same version.

| Package | Project | What is in it |
|---|---|---|
| `BillerContracts` | `Contracts` | Records in `Requests/<Resource>/` and `Responses/<Resource>/`, enums in `Enums/`, `ErrorModel`, `Result<T>` |
| `BillerContracts.Clients` | `Clients` | One client per resource (`IUserClient`, `ISellerClient`, `ISeriesClient`, `ICustomerClient`, `IItemClient`, `IInvoiceClient`), `ClientsOptions` with their URLs, `ClientAPIException` |

## The four repos

| Repo | What it is |
|---|---|
| [Biller](https://github.com/aldisadom/Biller) | The API, the database migrations and the PDFs |
| [BillerContracts](https://github.com/aldisadom/BillerContracts) | Request and response records, enums and typed HTTP clients, published as the `BillerContracts` and `BillerContracts.Clients` NuGet packages |
| [BillerFrontEnd](https://github.com/aldisadom/BillerFrontEnd) | The Blazor UI. It calls the API through `BillerContracts.Clients` |
| [BillioIntegrationTest](https://github.com/aldisadom/BillioIntegrationTest) | Tests that call a running API over HTTP |

Who uses what:

- Biller: `BillerContracts`.
- BillerFrontEnd and BillioIntegrationTest: `BillerContracts.Clients`, which brings `BillerContracts` with it.

A change here reaches them only after a publish and a version bump in each repo.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Build and test

```sh
dotnet build -c Release
dotnet test -c Release
```

`Clients.Tests` checks the HTTP method, URL and body each client sends. CI
(`.github/workflows/CI.yaml`) runs the same and fails when coverage drops below its minimums.

## Use the clients

```csharp
services.AddBillerClients(configuration.GetSection("ClientsOptions"));
```

```json
"ClientsOptions": {
  "UserUrl": "http://localhost:8090/users",
  "SellerUrl": "http://localhost:8090/sellers",
  "SeriesUrl": "http://localhost:8090/series",
  "CustomerUrl": "http://localhost:8090/customers",
  "ItemUrl": "http://localhost:8090/items",
  "InvoiceUrl": "http://localhost:8090/invoices"
}
```

When the API answers with an error status, the call throws `ClientAPIException`. Its `Error` holds
the status code, the message and the extended message when the body is Biller's error response, and
only the status code otherwise (a proxy's HTML page, say).

`Get`, `Add` and `Login` read a successful response as JSON. When its body is empty or `null` they
throw `ClientAPIException` with `Error` null; when it isn't valid JSON for the response type (an
array for an object, a bad Guid) they throw `Newtonsoft.Json.JsonException`. Properties the type
doesn't have are ignored, so an unrelated JSON object gives a response with default values.
`GeneratePDF` returns the successful body as a stream, unread. `Update`, `UpdateStatus` and
`Delete` ignore it.

A call that gets no response throws `HttpRequestException`, or `TaskCanceledException` on a timeout.

## Publish

The owner runs the `Publish` workflow in GitHub Actions with `package_version` as
`MAJOR.MINOR.PATCH`. It runs CI first.

- From `master` it publishes that version of both packages.
- From another branch it publishes `<version>-preview.<run>`, so the other repos can try a change
  before it is merged.

Renaming or removing a property, enum member or namespace breaks the repos that use the packages,
so it needs a new major version.

## Where things go

- `Contracts`: the records and enums.
- `Clients`: the clients, their interfaces and registration.
- `Clients.Tests`: the client tests.

The conventions, package limits and review rules are in [CLAUDE.md](CLAUDE.md) and [REVIEW.md](REVIEW.md).

## Licence

MIT, see [LICENSE.txt](LICENSE.txt).
