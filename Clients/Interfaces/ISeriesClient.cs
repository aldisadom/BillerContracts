using BillerContracts.Requests.Series;
using BillerContracts.Responses;
using BillerContracts.Responses.Series;

namespace BillerContracts.Clients.Interfaces;

public interface ISeriesClient
{
    Task<AddResponse> Add(SeriesAddRequest series);
    Task Delete(Guid id);
    Task<SeriesListResponse> Get();
    Task<SeriesListResponse> Get(SeriesGetRequest request);
    Task<SeriesResponse> Get(Guid id);
    Task Update(SeriesUpdateRequest series);
}
