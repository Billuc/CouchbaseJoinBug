namespace CouchbaseJoinBug
{
    public class RouteDTO
    {
        public const string TYPE = "route";

        public string Type => TYPE;
        public string Id { get; set; }
        public string SourceAirport { get; set; }
        public string DestinationAirport { get; set; }
        public string AirlineId { get; set; }
    }
}