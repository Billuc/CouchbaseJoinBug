namespace CouchbaseJoinBug
{
    public class AirlineDTO
    {
        public const string TYPE = "airline";
        
        public string Type => TYPE;
        public string AirlineId { get; set; }
        public string Name { get; set; }
        public string CallSign { get; set; }
    }
}