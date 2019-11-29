using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BABERECEN.Model
{
    public class Sleep
    {
        public int id { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int sleep_time { get; set; } // 분 단위
    }
}
