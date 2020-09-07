using System;
using System.Collections.Generic;

namespace CV.Models
{
    public class EdItem
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string GraduationDate { get; set; }
        public string Image { get; set; }
        public List<string> Xref { get; set; } = new List<string>();
        public List<string> BulletPoints { get; set; } = new List<string>();
    }
}
