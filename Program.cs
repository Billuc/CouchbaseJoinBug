using System;
using Couchbase.Lite.Query;

namespace CouchbaseJoinBug
{
    class Program
    {
        static void Main(string[] args)
        {
            var mydb = new MyDatabase();
            var db = mydb.Database;

            var airlines = new Airlines(db);
            var routes = new Routes(db);

            // Initializing airlines

            airlines.Add(new AirlineDTO()
            {
                AirlineId = "airline1",
                Name = "My First Airline",
                CallSign = "CYF"
            });

            airlines.Add(new AirlineDTO()
            {
                AirlineId = "airline2",
                Name = "My Second Airline",
                CallSign = "BGB"
            });

            // Initializing routes

            routes.Add(new RouteDTO()
            {
                RouteId = "route1",
                Airline = "airline1",
                SourceAirport = "LYS",
                DestinationAirport = "LAX"
            });

            routes.Add(new RouteDTO()
            {
                RouteId = "route1",
                Airline = "airline1",
                SourceAirport = "RIX",
                DestinationAirport = "LAX"
            });

            routes.Add(new RouteDTO()
            {
                RouteId = "route1",
                Airline = "airline1",
                SourceAirport = "RIX",
                DestinationAirport = "LYS"
            });

            // Execute Query with join

            using (var query = QueryBuilder.Select(
                    SelectResult.Expression(Expression.Property(nameof(AirlineDTO.Name)).From(Airlines.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(AirlineDTO.CallSign)).From(Airlines.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(RouteDTO.SourceAirport)).From(Routes.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(RouteDTO.DestinationAirport)).From(Routes.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(RouteDTO.Airline)).From(Routes.ALIAS))
                )
                .From(DataSource.Database(db).As(Airlines.ALIAS))
                .Join(
                    Join.LeftJoin(DataSource.Database(db).As(Routes.ALIAS))
                        .On(Expression.Property(nameof(AirlineDTO.AirlineId)).From(Airlines.ALIAS)
                            .EqualTo(Expression.Property(nameof(RouteDTO.Airline)).From(Routes.ALIAS))
                        )
                )
                .Where(
                    Expression.Property(nameof(RouteDTO.Type)).From(Routes.ALIAS).EqualTo(Expression.String(RouteDTO.TYPE))
                        .And(Expression.Property(nameof(AirlineDTO.Type)).From(Airlines.ALIAS).EqualTo(Expression.String(AirlineDTO.TYPE)))
                        .And(Expression.Property(nameof(RouteDTO.SourceAirport)).From(Routes.ALIAS).EqualTo(Expression.String("RIX")))
                )
            )
            {
                foreach (var result in query.Execute())
                {
                    Console.WriteLine($"===============================================");

                    foreach (var r in result.Keys)
                    {
                        Console.WriteLine(r + " : " + result[r]);
                    }
                }

                Console.WriteLine($"===============================================");
            }

            db.Delete();
        }
    }
}
