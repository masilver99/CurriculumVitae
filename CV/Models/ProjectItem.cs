using System.Collections.Generic;

namespace CV.Models
{
    public class ProjectItem
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Url { get; set; }
        public string LastWorkedOnDate { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
        public bool OpenSource { get; set; }
        public string Image { get; set; }
        public string Screenshots { get; set; }
        public string Status { get; set; }
        public bool CodeAvailable { get; set; }

        public List<string> ProjectType { get; set; }
        public List<string> TechXref { get; set; } = new List<string>();
        public List<TechItem> TechItems { get; set; } = new List<TechItem>();
        public List<string> TechnologyUsed { get; set; } = new List<string>();
        //Lookup Tech XRef for tech used
    }

    public enum ProjectType
    {
        Hardware,   //<i class="fas fa-server"></i>
        Application,  //<i class="far fa-window-restore"></i>
        Website,  //<i class="fas fa-globe-americas"></i>
        Firmware  //<i class="fas fa-microchip"></i>
    }
}
