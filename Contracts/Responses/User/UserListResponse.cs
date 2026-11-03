namespace BillerContracts.Responses.User;

public record UserListResponse : PagedResponse
{
    public List<UserResponse> Users { get; set; } = [];
}
