namespace Exc.Banking;

public interface IBankRepository
{
    Task<IReadOnlyCollection<Bank>> GetBanksAsync(CancellationToken token);

    Task<Bank?> GetBankAsync(Guid bankId, CancellationToken token);

    Bank SberBank { get; }

    Bank AlfaBank { get; }
}
