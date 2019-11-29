using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BABERECEN.Model
{
    public class BreastMilk
    {
        public int id { get; set; }
        public DateTime right_start_time { get; set; }
        public int right_end_time { get; set; }
        public int left_start_time { get; set; }
        public int left_end_time { get; set; }
        public string special { get; set; }
    }
}
