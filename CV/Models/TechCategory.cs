using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class TechCategory
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public List<TechItem> Items { get; set; }
    }
}
