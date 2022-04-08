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

            var airline1ID = airlines.Add(new AirlineDTO()
            {
                Id = "airline1",
                Name = "My First Airline",
                CallSign = "CYF"
            });

            var airline2ID = airlines.Add(new AirlineDTO()
            {
                Id = "airline2",
                Name = "My Second Airline",
                CallSign = "BGB"
            });

            // Initializing routes

            routes.Add(new RouteDTO()
            {
                Id = "route1",
                AirlineId = airline2ID,
                SourceAirport = "LYS",
                DestinationAirport = "LAX"
            });

            routes.Add(new RouteDTO()
            {
                Id = "route2",
                AirlineId = airline1ID,
                SourceAirport = "RIX",
                DestinationAirport = "LAX"
            });

            routes.Add(new RouteDTO()
            {
                Id = "route3",
                AirlineId = airline2ID,
                SourceAirport = "RIX",
                DestinationAirport = "LYS"
            });

            // Execute Query with join
            // Inspired by https://docs.couchbase.com/couchbase-lite/current/csharp/querybuilder.html#lbl-join

            using (var query = QueryBuilder.Select(
                    SelectResult.Expression(Expression.Property(nameof(AirlineDTO.Name)).From(Airlines.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(AirlineDTO.CallSign)).From(Airlines.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(RouteDTO.SourceAirport)).From(Routes.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(RouteDTO.DestinationAirport)).From(Routes.ALIAS)),
                    SelectResult.Expression(Expression.Property(nameof(RouteDTO.AirlineId)).From(Routes.ALIAS))
                )
                .From(DataSource.Database(db).As(Airlines.ALIAS))
                .Join(
                    Join.LeftJoin(DataSource.Database(db).As(Routes.ALIAS))
                        .On(Expression.Property(nameof(AirlineDTO.Id)).From(Airlines.ALIAS)
                            .EqualTo(Expression.Property(nameof(RouteDTO.AirlineId)).From(Routes.ALIAS))
                        )
                )
            )
            {
                var where = Expression.Property(nameof(RouteDTO.Type)).From(Routes.ALIAS).EqualTo(Expression.String(RouteDTO.TYPE))
                    .And(Expression.Property(nameof(AirlineDTO.Type)).From(Airlines.ALIAS).EqualTo(Expression.String(AirlineDTO.TYPE)))
                    .And(Expression.Property(nameof(RouteDTO.SourceAirport)).From(Routes.ALIAS).EqualTo(Expression.String("RIX")));

                Console.WriteLine(query.Where(where).Explain());
                var results = query.Where(where).Execute().AllResults();

                foreach (var result in results)
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
