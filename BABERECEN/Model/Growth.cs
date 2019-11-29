using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BABERECEN.Model
{
    public class Growth
    {
        public DateTime reference_date { get; set; }
        public int height { get; set; }
        public float weight { get; set; }
        public int head_size { get; set; }
        public float temperature { get; set; }
    }
}
