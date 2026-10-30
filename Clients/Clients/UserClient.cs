using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.User;
using BillerContracts.Responses;
using BillerContracts.Responses.User;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients.Clients;

public class UserClient : IUserClient
{
    private readonly BaseHttpClient _httpClient;

    public UserClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string url = clientOptions.Value.UserUrl;
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.UserUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, url);
    }

    public async Task<UserLoginResponse> Login(UserLoginRequest user)
    {
        return await _httpClient.PostAsync<UserLoginRequest, UserLoginResponse>("Login", user);
    }

    public async Task<UserListResponse> Get()
    {
        return await _httpClient.GetAsync<UserListResponse>(string.Empty);
    }

    public async Task<UserListResponse> Get(UserGetRequest request)
    {
        return await _httpClient.GetAsync<UserListResponse>(string.Empty, BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<UserResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<UserResponse>($"{id}");
    }

    public async Task<AddResponse> Add(UserAddRequest user)
    {
        return await _httpClient.PostAsync<UserAddRequest, AddResponse>(string.Empty, user);
    }

    public async Task Update(UserUpdateRequest user)
    {
        await _httpClient.PutAsync(string.Empty, user);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{id}");
    }
}
