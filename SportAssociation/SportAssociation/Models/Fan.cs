using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportAssociation.Models
{
    public class Fan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String national_id { get; set; }
        public String Name { get; set; }
        public DateTime birth_date { get; set; }
        public String Address { get; set; }
        public String phone_number { get; set; }
        public bool Status { get; set; }
        public String Username { get; set; }

        public Fan()
        {

        }

    }
}
