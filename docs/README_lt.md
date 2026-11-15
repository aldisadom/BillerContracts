# BillerContracts

[English](../README.md)

Bendri Biller API tipai: užklausų ir atsakymų įrašai, išvardijimai (enum) ir tipizuoti HTTP
klientai. Jie skelbiami nuget.org kaip du paketai, kurių versija visada vienoda.

Šis failas yra [README.md](../README.md) vertimas. Komandos, keliai ir pavadinimai palikti tokie,
kokie yra angliškame faile.

| Paketas | Projektas | Kas jame |
|---|---|---|
| `BillerContracts` | `Contracts` | Įrašai aplankuose `Requests/<Resource>/` ir `Responses/<Resource>/`, išvardijimai aplanke `Enums/`, `ErrorModel`, `Result<T>` |
| `BillerContracts.Clients` | `Clients` | Po vieną klientą kiekvienam ištekliui (`IUserClient`, `ISellerClient`, `ISeriesClient`, `ICustomerClient`, `IItemClient`, `IInvoiceClient`), `ClientsOptions` su jų adresais, `ClientAPIException` |

## Keturios repozitorijos

| Repozitorija | Kas tai |
|---|---|
| [Biller](https://github.com/aldisadom/Biller) | API, duomenų bazės migracijos ir PDF dokumentai |
| [BillerContracts](https://github.com/aldisadom/BillerContracts) | Užklausų ir atsakymų įrašai, išvardijimai ir tipizuoti HTTP klientai, skelbiami kaip NuGet paketai `BillerContracts` ir `BillerContracts.Clients` |
| [BillerFrontEnd](https://github.com/aldisadom/BillerFrontEnd) | Blazor naudotojo sąsaja. API ji kviečia per `BillerContracts.Clients` |
| [BillioIntegrationTest](https://github.com/aldisadom/BillioIntegrationTest) | Testai, kurie kviečia veikiančią API per HTTP |

Kas ką naudoja:

- Biller: `BillerContracts`.
- BillerFrontEnd ir BillioIntegrationTest: `BillerContracts.Clients`, kuris kartu atsineša ir `BillerContracts`.

Pakeitimas čia jas pasiekia tik paskelbus paketus ir atnaujinus versiją kiekvienoje repozitorijoje.

## Ko reikia

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Kompiliavimas ir testai

```sh
dotnet build -c Release
dotnet test -c Release
```

`Clients.Tests` tikrina, kokį HTTP metodą, adresą ir turinį siunčia kiekvienas klientas. CI
(`.github/workflows/CI.yaml`) daro tą patį ir nutrūksta, jei padengimas nukrenta žemiau nustatytų
minimumų.

## Klientų naudojimas

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

Kai API atsako klaidos būsena, kvietimas meta `ClientAPIException`. Jos `Error` turi būsenos kodą,
pranešimą ir išplėstinį pranešimą, kai turinys yra Biller klaidos atsakymas, o kitu atveju tik
būsenos kodą (pavyzdžiui, tarpinio serverio HTML puslapis).

`Get`, `Add` ir `Login` sėkmingą atsakymą skaito kaip JSON. Kai jo turinys tuščias arba `null`, jie
meta `ClientAPIException`, kurios `Error` yra null; kai tai nėra teisingas JSON atsakymo tipui
(masyvas vietoj objekto, bloga Guid reikšmė), jie meta `Newtonsoft.Json.JsonException`. Savybės,
kurių tipas neturi, ignoruojamos, todėl nesusijęs JSON objektas duoda atsakymą su numatytosiomis
reikšmėmis. `GeneratePDF` sėkmingo atsakymo turinį grąžina kaip srautą, jo neskaitydamas. `Update`,
`UpdateStatus` ir `Delete` jį ignoruoja.

Kvietimas, kuris negauna atsakymo, meta `HttpRequestException`, o pasibaigus laukimo laikui –
`TaskCanceledException`.

## Skelbimas

Savininkas GitHub Actions paleidžia `Publish` darbo eigą su `package_version` formatu
`MAJOR.MINOR.PATCH`. Ji pirmiausia paleidžia CI.

- Iš `master` paskelbiama ta abiejų paketų versija.
- Iš kitos šakos paskelbiama `<version>-preview.<run>`, kad kitos repozitorijos galėtų išbandyti
  pakeitimą prieš jį sujungiant.

Savybės, išvardijimo nario ar vardų srities pervadinimas arba pašalinimas sugadina paketus
naudojančias repozitorijas, todėl tam reikia naujos pagrindinės (major) versijos.

## Kur kas yra

- `Contracts`: įrašai ir išvardijimai.
- `Clients`: klientai, jų sąsajos ir registravimas.
- `Clients.Tests`: klientų testai.

Susitarimai, paketų apribojimai ir peržiūros taisyklės yra [CLAUDE.md](../CLAUDE.md) ir
[REVIEW.md](../REVIEW.md) failuose.

## Licencija

MIT, žr. [LICENSE.txt](../LICENSE.txt).
