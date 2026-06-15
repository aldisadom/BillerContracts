using BillerContracts.Enums;

namespace BillerContracts.Responses.User;

public class UserListResponse
{
    public List<UserResponse> Users { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
