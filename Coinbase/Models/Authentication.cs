using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coinbase.Models
{
    [Table("authentication")]
    public class Authentication
    {
        [Key]
        public string ApiKey;
        public string UserName;
    }
}