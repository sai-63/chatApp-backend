using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ObjId
    {
        public int timestamp { get; set; }
        public int machine { get; set; }
        public int pid { get; set; }
        public int increment { get; set; }
        public DateTime creationTime { get; set; }
    }
}
