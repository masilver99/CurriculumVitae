using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class TechItem
    {
        /// <summary>
        /// Name of the technology
        /// </summary>
        public string Name { get; set; }

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

        public List<string> BulletPoints { get; set; } = new List<string>();

        public List<string> Xref { get; set; } = new List<string>();

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
