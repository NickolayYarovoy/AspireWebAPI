using CommonClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using WebAPI.DbContexts;
using WebAPI.DbEntities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class BankAccountController : ControllerBase
    {
        private IDbContextFactory<BankDbContext> dbFactory;
        public BankAccountController(IDbContextFactory<BankDbContext> factory)
        {
            dbFactory = factory;
        }

        [HttpPost("account")]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountJson json)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            if (!await db.Users.AnyAsync(x => x.Id == json.UserId))
                return new NotFoundResult();

            BankAccount account = new(json);
            db.BankAccounts.Add(account);
            await db.SaveChangesAsync();

            return new OkObjectResult(account.Id);
        }

        [HttpGet("account/{id}")]
        public async Task<ActionResult> GetAccount(Guid id)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var account = await db.BankAccounts.FirstOrDefaultAsync(x => x.Id == id);

            if (account == null)
                return new NotFoundResult();

            return new OkObjectResult(account.CreateJson());
        }

        [HttpPut("top_up")]
        public async Task<ActionResult> TopUpAccount([FromBody] ChangeBalanceJson json)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var account = await db.BankAccounts.FirstOrDefaultAsync(x => x.Id == json.AccountId);

            if (account == null)
                return new NotFoundResult();

            account.Balance += json.Value;
            await db.SaveChangesAsync();

            return new OkResult();
        }

        [HttpPut("withdrawn")]
        public async Task<ActionResult> WithdrawnAccount([FromBody] ChangeBalanceJson json)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var account = await db.BankAccounts.FirstOrDefaultAsync(x => x.Id == json.AccountId);

            if (account == null)
                return new NotFoundResult();

            if (account.Balance < json.Value)
                return new BadRequestObjectResult("На счету недостаточно средств");

            account.Balance -= json.Value;
            await db.SaveChangesAsync();

            return new OkResult();
        }

        [HttpPut("remittance")]
        public async Task<ActionResult> RemittanceAccount([FromBody] RemittanceJson json)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var senderAccount = await db.BankAccounts.FirstOrDefaultAsync(x => x.Id == json.SenderAccount);
            var recipientAccount = await db.BankAccounts.FirstOrDefaultAsync(x => x.Id == json.RecipientAccount);

            if (senderAccount == null || recipientAccount == null)
                return new NotFoundResult();

            if (senderAccount.Balance < json.Value)
                return new BadRequestObjectResult("На счету недостаточно средств");

            senderAccount.Balance -= json.Value;
            recipientAccount.Balance += json.Value;
            await db.SaveChangesAsync();

            return new OkResult();
        }
    }
}
