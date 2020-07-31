using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace CV.Pages
{
    public class ProjectModel : PageModel
    {
        private readonly ILogger<ProjectModel> _logger;

        public IList<ProjectItem> ProjectItems { get; }

        public ProjectModel(ILogger<ProjectModel> logger)
        {
            _logger = logger;
            this.ProjectItems = JsonSerializer.Deserialize<List<ProjectItem>>(System.IO.File.ReadAllText("data/projects.json"));
        }

        public void OnGet()
        {

        }
    }
}