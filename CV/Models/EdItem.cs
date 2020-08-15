using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class EdItem
    {
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string GraduationDate { get; set; }
        public string Image { get; set; }
        public List<string> Xref { get; set; } = new List<string>();
        public List<string> BulletPoints { get; set; } = new List<string>();
    }
}
