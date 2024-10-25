using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class SyncSet
    {
        public string Url { get; set; } = string.Empty;
        public string Verb { get; set; } = "POST";
        public string Connection { get; set; } = "Default";
        public string Sql { get; set; } = string.Empty;
        public string[] Progress { get; set; } = [];
        public bool Enabled { get; set; } = true;
        public AdditionalData[] AdditionalData { get; set; } = [];
        public IncludedFile[] Files { get; set; } = [];
        public Dictionary<string, string> Collections { get; set; } = [];
    }

    public class AdditionalData
    {
        public required string Connection { get; set; }
        public required string Sql { get; set; }
    }

    public class IncludedFile
    {
        public required string Path { get; set; }
        public string? Name { get; set; }
    }
}
