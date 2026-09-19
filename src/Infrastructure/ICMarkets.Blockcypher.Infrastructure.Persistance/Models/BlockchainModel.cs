using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Models
{
    public class BlockchainModel
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Coin { get; set; }

        [Required]
        public string Chain { get; set; }

        public string BlockchainData { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public DateTime UtcCreatedAt { get; set; }
    }
}
