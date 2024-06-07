using CommonClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DbEntities
{
    public class BankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ulong Balance { get; set; }
        public ulong UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public BankAccount() { }

        public BankAccount(CreateAccountJson  json)
        {
            Name = json.Name;
            Balance = 0;
            UserId = json.UserId;
        }

        public BankAccountJson CreateJson()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Balance = Balance,
                UserId = UserId
            };
        }
    }
}
