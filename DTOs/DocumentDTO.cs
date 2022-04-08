
using System;

namespace Monitoring.Context.Core.DTOs
{
    public abstract class DocumentDTO
    {
        public string Id { get; set; }
        public bool Disabled { get; set; }
        public abstract string Type { get; }
    }
}