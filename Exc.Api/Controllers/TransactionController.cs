using Exc.Api.Outbox;
using Exc.Banking;
using Exc.Banking.Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Exc.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IOutboxTransactionRepository _repository;
        private readonly IBankRepository _bankRepository;

        public TransactionController(
            IOutboxTransactionRepository repository,
            IBankRepository bankRepository)
        {
            _repository = repository;
            _bankRepository = bankRepository;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(TransactionDto transaction)
        {
            var domainTransaction = transaction.ToDomain();
        
            await _repository.Add(domainTransaction);
        
            return new OkResult();
        }

        [HttpPost("AddRandomData")]
        public async Task<IActionResult> AddRandomData(int count)
        {
            var sberLongtermUser = Guid.NewGuid();
            var alfaLongtermUser = Guid.NewGuid();


            for (int i = 0; i < count; i++)
            {
                await _repository.Add(new Transaction(Guid.NewGuid(), Guid.NewGuid(), _bankRepository.AlfaBank.Id));
                await _repository.Add(new Transaction(Guid.NewGuid(), Guid.NewGuid(), _bankRepository.SberBank.Id));
                await _repository.Add(new Transaction(Guid.NewGuid(), alfaLongtermUser, _bankRepository.AlfaBank.Id));
                await _repository.Add(new Transaction(Guid.NewGuid(), sberLongtermUser, _bankRepository.SberBank.Id));
            }

            return new OkResult();
        }
    }
}