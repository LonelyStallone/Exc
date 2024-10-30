namespace Exc.Api.EventBus.Abstractions;

public interface ILongtermDetector
{
    Task<bool> IsLongterm(Guid userId, Guid bankId);
}
