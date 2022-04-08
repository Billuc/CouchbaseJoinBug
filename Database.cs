namespace CouchbaseJoinBug
{
    public class MyDatabase
    {
        private const string DBNAME = "mydb";

        public Couchbase.Lite.Database Database { get; set; }

        public MyDatabase()
        {
            Database = new Couchbase.Lite.Database(DBNAME, new Couchbase.Lite.DatabaseConfiguration()
            {
                Directory = System.AppDomain.CurrentDomain.BaseDirectory
            });
        }
    }
}