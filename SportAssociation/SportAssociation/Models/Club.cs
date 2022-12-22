using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportAssociation.Models
{

    public class Club
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string location { get; set; }

        public Club()
        {

        }

    }

    
}
