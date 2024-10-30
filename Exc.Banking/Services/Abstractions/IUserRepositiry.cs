namespace Exc.Banking;

public interface IUserRepositiry
{
    Task<IReadOnlyCollection<User>> GetUsersAsync();

    Task<User?> GetUserAsync(Guid userId);
}
