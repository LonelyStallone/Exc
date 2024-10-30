namespace Exc.Banking;

public sealed record Transaction(Guid Id, Guid UserId, Guid BankId);