
using Couchbase.Lite;
using Couchbase.Lite.Query;

using Monitoring.Context.Core.DTOs;
namespace Monitoring.Context.Core
{
    public class ApplicationContext
    {
        public const string LABEL_FT_INDEX = "LABEL_FT_INDEX";
        public Database Database { get; }

        public ApplicationContext(
            Database database)
        {
            Database = database;
        }

        public static void OnModelCreating(Database db)
        {            
            var indexes = db.GetIndexes();

            if (!indexes.Contains(nameof(DocumentDTO.Type)))
            {
                var index = IndexBuilder.ValueIndex(
                    ValueIndexItem.Expression(Expression.Property(nameof(DocumentDTO.Type)))
                );
                db.CreateIndex(nameof(DocumentDTO.Type), index);
            }

            // if (!indexes.Contains(nameof(DeviceStateDTO.DeviceId)))
            // {
            //     var index = IndexBuilder.ValueIndex(
            //         ValueIndexItem.Expression(Expression.Property(nameof(DocumentDTO.Type))),
            //         ValueIndexItem.Expression(Expression.Property(nameof(DeviceStateDTO.DeviceId)))
            //     );
            //     db.CreateIndex(nameof(DeviceStateDTO.DeviceId), index);
            // }

            // if (!indexes.Contains(nameof(DeviceStateDTO)))
            // {
            //     var index = IndexBuilder.ValueIndex(
            //         ValueIndexItem.Expression(Expression.Property(nameof(DocumentDTO.Type))),
            //         ValueIndexItem.Expression(Expression.Property(nameof(DeviceStateDTO.DataId))),
            //         ValueIndexItem.Expression(Expression.Property(nameof(DeviceStateDTO.DeviceId))),
            //         ValueIndexItem.Expression(Expression.Property(nameof(DeviceStateDTO.Label)))
            //     );
            //     db.CreateIndex(nameof(DeviceStateDTO), index);
            // }

            // if (!indexes.Contains(LABEL_FT_INDEX))
            // {
            //     var index = IndexBuilder.FullTextIndex(
            //         FullTextIndexItem.Property(nameof(DeviceDTO.Label))
            //     ).IgnoreAccents(true);
            //     db.CreateIndex(LABEL_FT_INDEX, index);
            // }
        }
    }
}