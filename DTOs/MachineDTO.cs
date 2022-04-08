using System.Collections.Generic;

namespace Monitoring.Context.Core.DTOs
{
    public class MachineDTO : DocumentDTO
    {
        public const string TYPE = "machine";
        public string Label { get; set; }
        public string Description { get; set; }      
        public string ModelId { get; set; }      
        public override string Type => TYPE;
    }
}