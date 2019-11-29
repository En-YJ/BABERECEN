using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BABERECEN.Model
{
    public class Meal
    {
        public int id { get; set; }
        public DateTime time { get; set; }
        public int type { get; set; }
        public int intake { get; set; }
        public string special { get; set; }
    }
}
