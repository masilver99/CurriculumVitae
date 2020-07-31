using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class OpenItem
    {
        public string ProjectName { get; set; }
        public string Division { get; set; }
        public string Dates { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public List<string> BulletPoints { get; set; }
    }
}
