using System.Collections.Generic;

namespace CV.Models
{
    public class CertificationItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IssuedBy { get; set; }
        public string DateIssued { get; set; }
        public string ExpirationDate { get; set; }
        public string VerificationUrl { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadUrl { get; set; }

        public List<string> TechXref { get; set; } = new List<string>();
        public List<TechItem> TechItems { get; set; } = new List<TechItem>();
    }
}
