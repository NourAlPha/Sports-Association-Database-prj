using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportAssociation.Models
{
    public class Fan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String NationalId { get; set; }
        public String Name { get; set; }
        public DateTime BirthDate { get; set; }
        public String Address { get; set; }
        public String PhoneNumber { get; set; }
        public bool Status { get; set; }

        public Fan()
        {

        }

    }
}
