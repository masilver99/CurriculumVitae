using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class ProjectItem
    {
        public string SiteName { get; set; }
        public string Url { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<string> TechnologyUsed { get; set; }
    }
}
