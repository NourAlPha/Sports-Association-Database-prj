namespace SportAssociation.Models
{
    public class Host_Request
    {
        public int Id { get; set; }
        public int representative_id { get; set; }
        public int manager_id { get; set; }
        public int match_id { get; set; }
        public bool? status { get; set; }

        public Host_Request()
        {

        }
    }
}
