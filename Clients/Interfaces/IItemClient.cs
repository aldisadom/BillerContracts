using BillerContracts.Requests.Item;
using BillerContracts.Responses;
using BillerContracts.Responses.Item;

namespace BillerContracts.Clients.Interfaces;

public interface IItemClient
{
    Task<AddResponse> Add(ItemAddRequest item);
    Task Delete(Guid id);
    Task<ItemListResponse> Get();
    Task<ItemListResponse> Get(ItemGetRequest request);
    Task<ItemResponse> Get(Guid id);
    Task Update(ItemUpdateRequest item);
}
