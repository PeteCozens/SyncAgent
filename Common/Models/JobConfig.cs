using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class JobConfig
    {
        public string Url { get; set; } = string.Empty;
        public string Verb { get; set; } = "POST";
        public string Connection { get; set; } = "Default";
        public string Sql { get; set; } = string.Empty;
        public string[] OrderBy { get; set; } = [];
        public string[] Progress { get; set; } = [];
        public bool Enabled { get; set; } = true;
    }
}
