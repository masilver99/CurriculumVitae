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
        public List<string> Xref { get; set; } = new List<string>();
        public List<TechItem> Items { get; set; }
        public TechCategory ItemlessCopy(List<TechItem> newTechItems)
        {
            return new TechCategory
            {
                Category = this.Category,
                Title = this.Title,
                Xref = this.Xref,
                Items = newTechItems
            };
        }
    }
}
