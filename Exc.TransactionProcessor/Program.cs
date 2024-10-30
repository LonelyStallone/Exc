using Exc.TransactionProcessor;
using Exc.TransactionProcessor.EventBus;
using Exc.Banking;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBanking();
builder.Services.AddTransactionProcessor();

var app = builder.Build();

app.Run();
