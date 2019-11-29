using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BABERECEN.Model
{
    public class Diaper
    {
        public int id { get; set; }
        public int state { get; set; }
        public DateTime time { get; set; }
        public int color { get; set; }
        public string special { get; set; }
    }
}
