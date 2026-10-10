using BillerContracts.Requests.User;
using BillerContracts.Responses;
using BillerContracts.Responses.User;

namespace BillerContracts.Clients.Interfaces;

public interface IUserClient
{
    Task<UserLoginResponse> Login(UserLoginRequest user);
    Task<UserListResponse> Get();
    Task<UserResponse> Get(Guid id);
    Task<AddResponse> Add(UserAddRequest user);
    Task Update(UserUpdateRequest user);
    Task Delete(Guid id);
}
