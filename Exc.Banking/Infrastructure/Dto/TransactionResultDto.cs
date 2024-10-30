namespace Exc.Banking.Infrastructure.Dto;

public sealed record TransactionResultDto
{
    public required Guid Id { get; init; }

    public required int State { get; init; }
}
