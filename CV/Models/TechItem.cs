﻿using System.Collections.Generic;

namespace CV.Models
{
    public class TechItem
    {
        public string Id { get; set; }
        /// <summary>
        /// Name of the technology
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Number of years of experience 
        /// </summary>
        public int Years { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// The experience level, 1 being lowest, 10 being highest
        /// </summary>
        public int ExperienceLevel { get; set; }

        public string Versions { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<string> BulletPoints { get; set; } = new List<string>();

        public List<string> Xref { get; set; } = new List<string>();

        public List<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
        public List<ProjectItem> ProjectItems { get; set; } = new List<ProjectItem>();

        public int GetFullStars()
        {
            return (ExperienceLevel - GetHalfStars()) / 2;
        }

        public int GetHalfStars()
        {
            return (ExperienceLevel % 2);
        }

        public int GetEmptyStars()
        {
            return (10 - ExperienceLevel - GetHalfStars()) / 2;
        }
    }
}
