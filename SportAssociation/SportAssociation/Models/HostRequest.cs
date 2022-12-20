namespace SportAssociation.Models
{
    public class HostRequest
    {
        public int Id { get; set; }
        public int RepresentativeId { get; set; }
        public int ManagerId { get; set; }
        public int MatchId { get; set; }
        public bool Status { get; set; }

        public HostRequest()
        {

        }
    }
}
