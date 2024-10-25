using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Progress
    {
        public string Name { get; set; } = string.Empty;
        public required object Value { get; set; }
        public byte[] SysRowVersion { get; set; } = [];
    }
}
