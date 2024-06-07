using CommonClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using WebAPI.DbContexts;

namespace WebAPI.Controllers
{
    [Route("api")]
    public class UserController : ControllerBase
    {
        private IDbContextFactory<BankDbContext> dbFactory;
        public UserController(IDbContextFactory<BankDbContext> factory)
        {
            dbFactory = factory;
        }

        [HttpPost("user")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserJson json)
        {
            if (json == null)
                return new BadRequestResult();

            using var db = await dbFactory.CreateDbContextAsync();

            if (await db.Users.AnyAsync(x => x.Id == json.Id))
                return new BadRequestObjectResult("Пользователь с данным Id уже существует");

            db.Users.Add(new(json));

            await db.SaveChangesAsync();

            return new OkResult();
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult> GetUser(ulong id)
        {
            using var db = await dbFactory.CreateDbContextAsync();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return new NotFoundResult();

            await db.Entry(user).Collection(x => x.Accounts).LoadAsync();

            return new OkObjectResult(user.CreateJson());
        }
    }
}
