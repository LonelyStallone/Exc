namespace Exc.Banking;

internal class BankRepository : IBankRepository
{
    private readonly Bank _alfa;
    private readonly Bank _sber;

    private readonly List<Bank> _banks = new();



    public BankRepository()
    {
        _alfa = new Bank(Guid.Parse("33333333-3333-4e75-bf09-29ef0683f00e"), "Alfa");
        _sber = new Bank(Guid.Parse("44444444-4444-4841-b2f1-2f30ef3e62f9"), "Sber");
        _banks.Add(_alfa);
        _banks.Add(_sber);
    }

    public Task<IReadOnlyCollection<Bank>> GetBanksAsync(CancellationToken _)
    {
        return Task.FromResult((IReadOnlyCollection<Bank>)_banks);
    }

    public Task<Bank?> GetBankAsync(Guid bankId, CancellationToken _)
    {
        return Task.FromResult(_banks.FirstOrDefault(bank => bank.Id == bankId));
    }

    public Bank AlfaBank => _alfa;

    public Bank SberBank => _sber;
}
