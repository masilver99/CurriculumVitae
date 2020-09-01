using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CV.Models;
using System.Text.RegularExpressions;

namespace CV.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet = true)]
        public string searchTerms { get; set; }

        public TechCategories SearchedTechCategories { get; set; } = new TechCategories();
        public List<EdItem> SearchedEdItems { get; set; } = new List<EdItem>();
        public List<WorkItem> SearchedWorkItems { get; set; } = new List<WorkItem>();
        public List<ProjectItem> SearchedProjectItems { get; set; } = new List<ProjectItem>();

        private List<TechCategory> _techCategories { get; }
        private List<EdItem> _edItems { get; }
        private List<WorkItem> _workItems { get; }
        private List<ProjectItem> _projectItems { get; }

        public IndexModel(
            ILogger<IndexModel> logger,
            TechCategories techCategories,
            List<EdItem> edItems,
            List<ProjectItem> projectItems)

        {
            _logger = logger;
            _techCategories = techCategories;
            _edItems = edItems;
            _projectItems = projectItems;
        }

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(searchTerms))
            {
                var terms = ScrubUserInput(searchTerms);
                /*
                // Search data
                foreach (var term in terms)
                {
                    // Search tech catergories
                    SearchTechCategories(_techCategories, term);
                    SearchTechItems(_techCategories, term);

                    //Search Jobs
                    SearchWorkItems(_workItems, term);

                    //Search Projects
                    SearchProjectItems(_projectItems, term);

                    //Search Education
                    SearchEdItems(_edItems, term);
                }
                */
            }
        }

        public void OnPost(string emailAddress)
        {

        }
        /*
        private void SearchTechCategories(List<TechCategory> techCategories, string term)
        {
            // Linq is avoided here to increase speed
            foreach(var techCategory in techCategories)
            {
                if (techCategory.Xref.Contains(term))
                {
                    SafeAddTechCat(techCategory);
                }
            }
        }

        private void SearchTechItems(List<TechCategory> techCategories, string term)
        {
            // linq is avoided here to increase speed
            foreach (var techCategory in techCategories)
            {
                foreach (var techItem in techCategory.Items)
                {
                    if (techItem.Xref.Contains(term))
                    {
                        SafeAddTechItem(techCategory, techItem);
                    }
                }
            }
        }

        private void SearchEdItems(List<EdItem> edItems, string term)
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

            // Check if looking for all items, if not add only ones matching
            if (globalSearchTerms.Contains(term))
            {
                SearchedEdItems = edItems;
            }
            else
            {
                // linq is avoided here to increase speed
                foreach (var edItem in edItems)
                {
                    if (edItem.Xref.Contains(term))
                    {
                        SafeAddItem(SearchedEdItems, edItem);
                    }
                }
            }
        }

        private void SearchProjectItems(List<ProjectItem> projectItems, string term)
        {
            var globalSearchTerms = new string[]
            {
                "PROJECT"
            };

            // Check if looking for all items, if not add only ones matching
            if (globalSearchTerms.Contains(term))
            {
                SearchedProjectItems = projectItems;
            }
            else
            {
                // linq is avoided here to increase speed
                foreach (var projectItem in projectItems)
                {
                    foreach (var xref in projectItem.TechXref)
                    {
                        // Linq would be easier...but slower
                        if (GetTechItemByName(xref, term, this._techCategories) != "")
                        {
                            SafeAddItem(SearchedProjectItems, projectItem);
                        }
                    }
                }
            }
        }
        */
        private string GetTechItemByName(string name, string searchTerm, List<TechCategory> techCats)
        {
            foreach (var techCat in techCats)
            {
                foreach (var techitem in techCat.Items)
                {
                    if (techitem.Name == name && techitem.Xref.Contains(searchTerm))
                    {
                        return techitem.Name;
                    }
                }
            }
            return "";
        }

        /*
        private void SearchWorkItems(List<WorkItem> workItems, string term)
        {
            var globalSearchTerms = new string[]
            {
                "WORK",
                "JOB"
            };

            // Check if looking for all items, if not add only ones matching
            if (globalSearchTerms.Contains(term))
            {
                SearchedWorkItems = workItems;
            }
            else
            {
                foreach (var workItem in workItems)
                {
                    foreach (var xref in workItem.TechXref)
                    {
                        // Linq would be easier...but slower
                        if (GetTechItemByName(xref, term, this._techCategories) != "")
                        {
                            SafeAddItem(SearchedWorkItems, workItem);
                        }
                    }
                }
            }
        }
        */
        private void SafeAddItem<T>(List<T> searchedItems, T newItem)
        {
            foreach (var item in searchedItems)
            {
                if (item.Equals(newItem)) return;
            }
            searchedItems.Add(newItem);
        }

        public void SafeAddTechItem(TechCategory techCategory, TechItem techItem)
        {
            var matchedCategories = SearchedTechCategories.Where(c => c.Category == techCategory.Category).SingleOrDefault();
            if (matchedCategories != null)
            {
                //Check if Item doesn't exist and add
                if (matchedCategories.Items.Where(t => t.Name == techItem.Name).Count() == 0)
                {
                    matchedCategories.Items.Add(techItem);
                }
            }
            else
            {
                var newTechCategory = techCategory.ItemlessCopy(new List<TechItem>() { techItem });
                SearchedTechCategories.Add(newTechCategory);
            }
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
