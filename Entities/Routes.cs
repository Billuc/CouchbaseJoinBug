namespace CouchbaseJoinBug
{
    public class Routes
    {
        public const string ALIAS = "routes";

        Couchbase.Lite.Database _db;

        public Routes(Couchbase.Lite.Database db) {
            this._db = db;
        }

        public string Add(RouteDTO dto) {
            using (var doc = new Couchbase.Lite.MutableDocument()) {
                doc.SetString(nameof(RouteDTO.RouteId), dto.RouteId)
                    .SetString(nameof(RouteDTO.Type), dto.Type)
                    .SetString(nameof(RouteDTO.SourceAirport), dto.SourceAirport)
                    .SetString(nameof(RouteDTO.DestinationAirport), dto.DestinationAirport)
                    .SetString(nameof(RouteDTO.Airline), dto.Airline);

                _db.Save(doc);
                return doc.Id;
            }
        }
    }
}