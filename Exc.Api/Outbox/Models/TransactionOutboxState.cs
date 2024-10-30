namespace Exc.Api.Outbox;

public enum TransactionOutboxState
{
    Registered, // Добавлена
    Produced    // Отправлена в шину
}
