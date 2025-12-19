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

        /// <summary>
        /// Calculates and returns the duration of employment in a human-readable format
        /// </summary>
        public string GetDuration()
        {
            var endDate = EndDate ?? DateTime.Now;
            var duration = endDate - StartDate;
            var years = (int)(duration.Days / 365.25);
            var months = (int)((duration.Days % 365.25) / 30.44);

            if (years > 0 && months > 0)
            {
                return $"{years} year{(years > 1 ? "s" : "")} {months} month{(months > 1 ? "s" : "")}";
            }
            else if (years > 0)
            {
                return $"{years} year{(years > 1 ? "s" : "")}";
            }
            else if (months > 0)
            {
                return $"{months} month{(months > 1 ? "s" : "")}";
            }
            else
            {
                return "Less than a month";
            }
        }
    }
}
