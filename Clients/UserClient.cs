using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.User;
using BillerContracts.Responses;
using BillerContracts.Responses.User;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients;

public class UserClient : IUserClient
{
    private readonly BaseHttpClient _httpClient;
    private readonly string _controller = "users";

    public UserClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string billioUrl = clientOptions.Value.BillioUrl;
        if (string.IsNullOrWhiteSpace(billioUrl))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.BillioUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, billioUrl);
    }

    public async Task<UserLoginResponse> Login(UserLoginRequest user)
    {
        return await _httpClient.PostAsync<UserLoginRequest, UserLoginResponse>($"{_controller}/Login", user);
    }

    public async Task<UserListResponse> Get()
    {
        return await _httpClient.GetAsync<UserListResponse>($"{_controller}");
    }

    public async Task<UserResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<UserResponse>($"{_controller}/{id}");
    }

    public async Task<AddResponse> Add(UserAddRequest user)
    {
        return await _httpClient.PostAsync<UserAddRequest, AddResponse>($"{_controller}", user);
    }

    public async Task Update(UserUpdateRequest user)
    {
        await _httpClient.PutAsync($"{_controller}", user);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{_controller}/{id}");
    }
}
