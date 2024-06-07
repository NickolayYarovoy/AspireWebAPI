using CommonClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DbEntities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public string Name { get; set; }

        public List<BankAccount> Accounts { get; set; }

        public User() { }

        public User(CreateUserJson user)
        {
            Id = user.Id;
            Name = user.Name;
            Accounts = new();
        }

        public UserJson CreateJson()
        {
            return new UserJson
            {
                Id = Id,
                Name = Name,
                AccountsIds = Accounts.Select(x => new GuidNameClass() { Id = x.Id, Name = x.Name }).ToList()
            };
        }
    }
}
