using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raebot
{
    class SearchResult
    {
        public int approx { get; set; }
        public IList<Dictionary<string, string>> res { get; set; }
    }
}
