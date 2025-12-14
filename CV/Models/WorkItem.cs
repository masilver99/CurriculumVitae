﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace CV.Models
{
    public class WorkItem
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Division { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public List<string> BulletPoints { get; set; } = new List<string>();
        public List<TechItem> TechItems { get; set; } = new List<TechItem>();
    }
}
