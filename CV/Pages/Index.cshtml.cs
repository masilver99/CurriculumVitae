using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CV.Models;
using System.Text.RegularExpressions;
using CV.data;
using System.Threading.Tasks;

namespace CV.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private Repository _repository;

        [BindProperty(SupportsGet = true)]
        public string searchTerms { get; set; }

        public IEnumerable<TechCategory> SearchedTechCategories { get; set; } = new TechCategories();
        public IEnumerable<EdItem> SearchedEdItems { get; set; } = new List<EdItem>();
        public IEnumerable<WorkItem> SearchedWorkItems { get; set; } = new List<WorkItem>();
        public IEnumerable<ProjectItem> SearchedProjectItems { get; set; } = new List<ProjectItem>();

        public IndexModel(
            ILogger<IndexModel> logger, Repository repostory)
        {
            _logger = logger;
            _repository = repostory;
        }

        private string GetIpAddress()
        {
            var headerValue = GetFirstHeaderValue("X-Forwarded-For", "X-Client-IP", "HTTP_X_FORWARDED_FOR", "HTTP_CLIENT_IP");

            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return headerValue;
        }

        private string GetFirstHeaderValue(params string[] headers)
        {
            foreach(var header in headers)
            {
                var headerValues = HttpContext.Request.Headers[header];

                if (headerValues.Count > 0)
                {
                    return headerValues.FirstOrDefault();
                }
            }

            return null;
        }

        public async Task OnGet()
        {
            if (!string.IsNullOrWhiteSpace(searchTerms))
            {
                _logger.LogInformation("Search: {searchTerms}, from {IpAddress}", searchTerms, GetIpAddress());
                var terms = ScrubUserInput(searchTerms);

                //Search Education
                await SearchEdItems(terms);
                await SearchTechCategories(terms);
                await SearchProjectItems(terms);
                await SearchWorkItems(terms);
            }
        }

        public void OnPost(string emailAddress)
        {

        }
        
        private async Task SearchTechCategories(List<string> terms)
        {
            SearchedTechCategories = await _repository.GetTechCategories(terms);
        }

        private async Task SearchEdItems(List<string> terms)
        {
            var globalSearchTerms = new string[]
            {
                "ED",
                "EDUCATION",
                "SCHOOL",
                "DEGREE",
                "SCHOOLING",
                "COLLEGE"
            };

            foreach (var global in globalSearchTerms)
            {
                if (terms.Contains(global))
                {
                    SearchedEdItems = await _repository.GetEdItems();
                    return;
                }
            }
                
            SearchedEdItems = await _repository.GetEdItems(terms);
        }
        private async Task SearchWorkItems(List<string> searchTerms)
        {
            var globalSearchTerms = new string[]
            {
                "WORK",
                "WORKEX",
                "WORKXPERIENCE",
                "JOBS"
            };

            // Check if looking for all items, if not add only ones matching
            foreach (var global in globalSearchTerms)
            {
                if (searchTerms.Contains(global))
                {
                    SearchedWorkItems = await _repository.GetWorkItems();
                    return;
                }
            }

            SearchedWorkItems = await _repository.GetWorkItems(searchTerms);
        }

        private async Task SearchProjectItems(List<string> searchTerms)
        {
            var globalSearchTerms = new string[]
            {
                "PROJECT"
            };

            // Check if looking for all items, if not add only ones matching
            foreach (var global in globalSearchTerms)
            {
                if (searchTerms.Contains(global))
                {
                    SearchedProjectItems = await _repository.GetProjectItems();
                    return;
                }
            }

            SearchedProjectItems = await _repository.GetProjectItems(searchTerms);
        }

        private List<string> ScrubUserInput(string searchTerms)
        {
            
            // Removed numbers and most chars from search term
            // Also remove all trailing 'S'es
            // And capitalize
            // This will speed up searches.
            Regex regex = new Regex(@"[^A-Z#+]|[S]*$");
            List<string> scrubedTerms = new List<string>();
            foreach (var term in searchTerms.Split(',', ';', StringSplitOptions.RemoveEmptyEntries))
            {
                scrubedTerms.Add(regex.Replace(term.ToUpper(), ""));
            }
            return scrubedTerms;

        }
    }
}
