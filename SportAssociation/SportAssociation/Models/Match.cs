namespace SportAssociation.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime starting_time { get; set; }
        public DateTime ending_time { get; set; }
        public int host_club { get; set; }
        public int guest_club { get; set; }
        public int? stadium_id { get; set; }

        public Match()
        {

        }
    }
}
