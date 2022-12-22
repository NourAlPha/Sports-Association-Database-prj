using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportAssociation.Models
{
    public class Manager
    {


        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public int stadium_id { get; set; }

        public Manager() 
        { 

        }
    }
}
