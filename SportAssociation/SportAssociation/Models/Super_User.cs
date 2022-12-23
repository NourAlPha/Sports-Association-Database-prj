using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportAssociation.Models
{
    public class Super_User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String Username { get; set; }
        public String Password { get; set; }

    }
}
