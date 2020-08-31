using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using Utf8Json;

namespace CV.Pages
{
    public class ProjectModel : PageModel
    {
        private readonly ILogger<ProjectModel> _logger;

        public List<ProjectItem> ProjectItems { get; }

        public ProjectModel(ILogger<ProjectModel> logger, List<ProjectItem> projectItems, TechCategories techCats)
        {
            _logger = logger;
            this.ProjectItems = projectItems;
            foreach (var item in projectItems)
            {
                foreach (var xref in item.TechXref)
                {
                    // Linq would be easier...but slower
                    if (techCats.GetTechItemByName(xref) == null)
                    {
                        _logger.LogWarning($"Cross-Ref not found: {xref}");
                    }
                }
            }
        }

        public void OnGet()
        {

        }
    }
}