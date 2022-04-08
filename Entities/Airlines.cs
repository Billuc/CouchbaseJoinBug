namespace CouchbaseJoinBug
{
    public class Airlines
    {
        public const string ALIAS = "airlines";

        Couchbase.Lite.Database _db;

        public Airlines(Couchbase.Lite.Database db) {
            this._db = db;
        }

        public string Add(AirlineDTO dto) {
            // using (var doc = new Couchbase.Lite.MutableDocument()) {
            //     doc.SetString(nameof(AirlineDTO.Id), dto.Id)
            //         .SetString(nameof(AirlineDTO.Type), dto.Type)
            //         .SetString(nameof(AirlineDTO.Name), dto.Name)
            //         .SetString(nameof(AirlineDTO.CallSign), dto.CallSign);

            //     _db.Save(doc);
            //     return doc.Id;
            // }

            using (var doc = dto.ToMutableDocument()) {
                _db.Save(doc);
            }

            return dto.Id;
        }
    }
}