using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_IO.Models
{
    public class BankCard
    {
        [Key]
        [Required]
        public string CardNumber { get; set; } 

        [Required]
        public string CVV2 { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [Column(TypeName = "REAL")]
        public decimal Balance { get; set; }
    }
}
