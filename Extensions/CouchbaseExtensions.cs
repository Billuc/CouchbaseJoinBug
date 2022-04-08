using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using Couchbase.Lite;
using Couchbase.Lite.Query;

namespace CouchbaseJoinBug
{
    public static class CouchbaseExtensions
    {
        public static MutableDocument ToMutableDocument(this object obj, string id)
        {
            var objectAsJson = JsonSerializer.Serialize(obj);

            var deserializedObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(objectAsJson);

            // var document = new MutableDocument(id, objectAsJson);

            var document = new MutableDocument(id);

            foreach (var pair in deserializedObject)
            {
                document.SetValue(pair.Key, pair.Value.ToPairOrValue());
            }

            return document;
        }

        public static Dictionary<string, object> ToObject(this object obj)
        {
            var objectAsJson = JsonSerializer.Serialize(obj);
            var deserializedObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(objectAsJson);

            var dico = new Dictionary<string, object>();

            foreach (var pair in deserializedObject)
            {
                dico.Add(pair.Key, pair.Value.ToPairOrValue());
            }

            return dico;
        }

        private static object ToPairOrValue(this JsonElement elt)
        {
            switch (elt.ValueKind)
            {
                case JsonValueKind.Array:
                    return elt.EnumerateArray()
                        .Select(e => e.ToPairOrValue()).ToArray();
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Number:
                    return elt.GetDouble();
                case JsonValueKind.Object:
                    return elt.EnumerateObject().ToDictionary(o => o.Name, o => o.Value.ToPairOrValue());
                case JsonValueKind.String:
                    return elt.GetString();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.Undefined:
                    return null;
                default:
                    throw new NotImplementedException();
            }
        }

        public static T ToObject<T>(this Document document)
        {
            var dico = new Dictionary<string, object>();

            foreach (var item in document)
            {
                dico.Add(item.Key, item.Value.ToPairOrValue());
            }

            var objectAsJson = JsonSerializer.Serialize(dico);

            var deserializedObject = JsonSerializer.Deserialize<T>(objectAsJson);

            return deserializedObject;
        }


        public static T ToObject<T>(this Result document)
        {
            var dico = new Dictionary<string, object>();

            Console.WriteLine("==================================================");

            foreach (var item in document as IEnumerable<KeyValuePair<string, object>>)
            {
                dico.Add(item.Key, item.Value.ToPairOrValue());
                Console.WriteLine("<" + item.Key + " : " + (item.Value == null ? "null" : item.Value.ToPairOrValue().ToString()) + ">");
            }

            var str = JsonSerializer.Serialize(dico);

            var obj2 = JsonSerializer.Deserialize<T>(str);

            return obj2;
        }

        private static object ToPairOrValue(this object obj)
        {
            switch (obj)
            {
                case DictionaryObject dico:
                    return dico.ToDictionary(pair => pair.Key, pair => pair.Value.ToPairOrValue());
                case ArrayObject array:
                    return array.Select(a => a.ToPairOrValue());
                default:
                    return obj;
            }
        }
    }
}