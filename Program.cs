using System;
using Couchbase.Lite.Query;
using Monitoring.Context.Core;
using Monitoring.Context.Core.DTOs;

namespace CouchbaseJoinBug
{
    class Program
    {
        static void Main(string[] args)
        {
            var mydb = new ApplicationContext(
                new Couchbase.Lite.Database("Test", new Couchbase.Lite.DatabaseConfiguration())
            );
            var db = mydb.Database;

            var modelId = Guid.NewGuid().ToString();
            var machineId = Guid.NewGuid().ToString();

            db.Save(new ModelDTO()
            {
                Id = modelId,
                Label = "Model 1",
                Disabled = false
            }.ToMutableDocument(modelId));

            db.Save(new MachineDTO()
            {
                Id = machineId,
                Label = "Machine 1",
                ModelId = modelId,
                Disabled = false
            }.ToMutableDocument(machineId));

            // Execute Query with join
            // Inspired by https://docs.couchbase.com/couchbase-lite/current/csharp/querybuilder.html#lbl-join

            const string MACHINES = "machines";
            const string MODELS = "models";
            
            using (var query = QueryBuilder
                .Select(
                    SelectResult.Expression(Expression.Property(nameof(MachineDTO.Id)).From(MACHINES)),
                    SelectResult.Expression(Expression.Property(nameof(MachineDTO.Label)).From(MACHINES)).As("Label"), // removing alias fix the issue
                    SelectResult.Expression(Expression.Property(nameof(MachineDTO.ModelId)).From(MACHINES)),
                    SelectResult.Expression(Expression.Property(nameof(ModelDTO.Label)).From(MODELS)).As("ModelLabel")
                )
                .From(DataSource.Database(db).As(MACHINES))
                .Join(
                    Join.LeftJoin(DataSource.Database(db).As(MODELS))
                        .On(Expression.Property(nameof(ModelDTO.Id)).From(MODELS)
                            .EqualTo(Expression.Property(nameof(MachineDTO.ModelId)).From(MACHINES))
                        )
                )
            )
            {
                IExpression whereExpression = Expression.Property(nameof(MachineDTO.Type)).From(MACHINES)
                    .EqualTo(Expression.String(MachineDTO.TYPE));

                whereExpression = whereExpression.And(
                    Expression.Property(nameof(MachineDTO.Disabled))
                    .From(MACHINES)
                    .EqualTo(Expression.Boolean(false))
                    .Or(
                        Expression.Property(nameof(MachineDTO.Disabled))
                        .From(MACHINES)
                        .IsNotValued()
                    )
                );

                Console.WriteLine(query.Where(whereExpression).Explain());

                var results = query.Where(whereExpression).Execute().AllResults();

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
