namespace CouchbaseJoinBug
{
    public class RouteDTO
    {
        public const string TYPE = "route";

        public string Type => TYPE;
        public string RouteId { get; set; }
        public string SourceAirport { get; set; }
        public string DestinationAirport { get; set; }
        public string Airline { get; set; }
    }
}