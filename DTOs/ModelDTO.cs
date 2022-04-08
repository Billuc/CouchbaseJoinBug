using System.Collections.Generic;

namespace Monitoring.Context.Core.DTOs
{
    public class ModelDTO : DocumentDTO
    {
        public const string TYPE = "models";
        public string Label { get; set; }
        public override string Type => TYPE;
    }
}