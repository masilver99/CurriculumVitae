using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Division { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public List<string> BulletPoints { get; set; } = new List<string>();
        public List<TechItem> TechItems { get; set; } = new List<TechItem>();
    }
}
