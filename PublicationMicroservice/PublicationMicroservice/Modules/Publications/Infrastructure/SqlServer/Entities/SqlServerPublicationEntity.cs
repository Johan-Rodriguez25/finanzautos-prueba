using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer.Entities
{
    [Table("Publications")]
    public class SqlServerPublicationEntity
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public SqlServerPublicationEntity() { }
    }
}
