using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CV.data;
using CV.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CV.Pages.Admin
{
    public class ValidateTechRefsModel : PageModel
    {
        private readonly Repository _repository;

        public ValidateTechRefsModel(Repository repository)
        {
            _repository = repository;
        }

        public List<string> MissingWorkRefs { get; set; } = new List<string>();
        public List<string> MissingProjectRefs { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            var workItems = await _repository.GetWorkItems();
            foreach (var work in workItems)
            {
                foreach (var tech in work.TechItems)
                {
                    if (tech.CategoryId == 0)
                    {
                        MissingWorkRefs.Add($"Work: {work.CompanyName} - Tech: {tech.DisplayName}");
                    }
                }
            }

            var projectItems = await _repository.GetProjectItems();
            foreach (var project in projectItems)
            {
                foreach (var tech in project.TechItems)
                {
                    if (tech.CategoryId == 0)
                    {
                        MissingProjectRefs.Add($"Project: {project.ProjectName} - Tech: {tech.DisplayName}");
                    }
                }
            }

            MissingWorkRefs.Sort();
            MissingProjectRefs.Sort();
        }
    }
}
