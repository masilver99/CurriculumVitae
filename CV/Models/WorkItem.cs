using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class WorkItem
    {
        public string CompanyName { get; set; }
        public string Division { get; set; }
        public string Dates { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public List<string> BulletPoints { get; set; }
    }
}
