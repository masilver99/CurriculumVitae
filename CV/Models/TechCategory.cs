using System.Collections.Generic;

namespace CV.Models
{
    public class TechCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SingularName { get; set; }
        public List<string> Xref { get; set; } = new List<string>();
        public List<TechItem> TechItems { get; set; } = new List<TechItem>();
    }
}
