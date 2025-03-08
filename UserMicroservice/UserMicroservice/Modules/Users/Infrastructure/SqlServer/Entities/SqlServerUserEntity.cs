using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserMicroservice.Modules.Users.Infrastructure.SqlServer.Entities
{
    [Table("Users")]
    public class SqlServerUserEntity
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Constructor sin par�metros para EF Core.
        public SqlServerUserEntity() { }
    }
}
