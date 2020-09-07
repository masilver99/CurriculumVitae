using System.Collections.Generic;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CV.data;

namespace CV.Pages
{
    public class ProjectModel : PageModel
    {
        private readonly ILogger<ProjectModel> _logger;
        private Repository _repository;

        public IEnumerable<ProjectItem> ProjectItems { get; private set;  }

       
        public ProjectModel(ILogger<ProjectModel> logger, Repository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnGet()
        {
            ProjectItems = await _repository.GetProjectItems();
        }
    }
}