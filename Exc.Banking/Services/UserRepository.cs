namespace Exc.Banking;

internal class UserRepository : IUserRepositiry
{
    private readonly List<User> _banks = new();

    public UserRepository()
    {
        _banks.Add(new User(Guid.Parse("11111111-1111-4e75-bf09-29ef0683f00e"), "Ivan"));
        _banks.Add(new User(Guid.Parse("22222222-2222-4841-b2f1-2f30ef3e62f9"), "Petr"));
    }

    public Task<IReadOnlyCollection<User>> GetUsersAsync()
    {
        return Task.FromResult((IReadOnlyCollection<User>)_banks);
    }

    public Task<User?> GetUserAsync(Guid bankId)
    {
        return Task.FromResult(_banks.FirstOrDefault(bank => bank.Id == bankId));
    }
}
