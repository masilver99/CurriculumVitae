using CV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CV.data
{
    public class Repository
    {
        // In-memory stores loaded at startup
        private readonly List<EdJson> _ed = new List<EdJson>();
        private readonly List<ProjectJson> _projects = new List<ProjectJson>();
        private readonly List<TechJsonCategory> _techCategories = new List<TechJsonCategory>();
        private readonly List<WorkJson> _work = new List<WorkJson>();

        private readonly Dictionary<string, TechItem> _techByName = new Dictionary<string, TechItem>();

        public Repository()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            // Load JSON files into memory
            var basePath = Path.Combine("data");
            var edPath = Path.Combine(basePath, "ed.json");
            var projectsPath = Path.Combine(basePath, "projects.json");
            var techPath = Path.Combine(basePath, "tech.json");
            var workPath = Path.Combine(basePath, "work.json");

            if (File.Exists(edPath))
            {
                var edJson = File.ReadAllText(edPath);
                var edItems = JsonSerializer.Deserialize<List<EdJson>>(edJson, options);
                if (edItems != null)
                {
                    _ed.AddRange(edItems);
                }
            }

            if (File.Exists(projectsPath))
            {
                var projectsJson = File.ReadAllText(projectsPath);
                var projectItems = JsonSerializer.Deserialize<List<ProjectJson>>(projectsJson, options);
                if (projectItems != null)
                {
                    _projects.AddRange(projectItems);
                }
            }

            if (File.Exists(techPath))
            {
                var techJson = File.ReadAllText(techPath);
                var techCats = JsonSerializer.Deserialize<List<TechJsonCategory>>(techJson, options);
                if (techCats != null)
                {
                    _techCategories.AddRange(techCats);
                    // Flatten tech items for quick lookup by name
                    var catId = 1;
                    foreach (var cat in _techCategories)
                    {
                        if (cat.Items != null)
                        {
                            foreach (var item in cat.Items)
                            {
                                var techItem = MapTechItem(cat, item, catId);
                                if (!string.IsNullOrWhiteSpace(techItem.Name))
                                {
                                    var key = techItem.Name.Trim().ToUpperInvariant();
                                    if (!_techByName.ContainsKey(key))
                                    {
                                        _techByName.Add(key, techItem);
                                    }
                                }
                            }
                        }
                        catId++;
                    }
                }
            }

            if (File.Exists(workPath))
            {
                var workJson = File.ReadAllText(workPath);
                var workItems = JsonSerializer.Deserialize<List<WorkJson>>(workJson, options);
                if (workItems != null)
                {
                    _work.AddRange(workItems);
                }
            }
        }

        // Public API preserved
        public async Task<IEnumerable<WorkItem>> GetWorkItems(List<string> searchTerms = null)
        {
            return await Task.Run(() =>
            {
                var items = _work.Select(MapWorkItem).ToList();
                if (searchTerms != null && searchTerms.Count > 0)
                {
                    var terms = searchTerms.Select(t => t.Trim().ToUpperInvariant()).ToHashSet();
                    items = items.Where(w =>
                        MatchesTerms(terms, w.CompanyName) ||
                        MatchesTerms(terms, w.Division) ||
                        MatchesTerms(terms, w.Location) ||
                        w.TechItems.Any(t => MatchesTerms(terms, t.Name) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant()))))
                    ).ToList();
                }
                return items;
            });
        }

        public async Task<IEnumerable<ProjectItem>> GetProjectItems(List<string> searchTerms = null)
        {
            return await Task.Run(() =>
            {
                var items = _projects.Select(MapProjectItem).ToList();
                if (searchTerms != null && searchTerms.Count > 0)
                {
                    var terms = searchTerms.Select(t => t.Trim().ToUpperInvariant()).ToHashSet();
                    items = items.Where(p =>
                        MatchesTerms(terms, p.ProjectName) ||
                        MatchesTerms(terms, p.Description) ||
                        (p.TechItems != null && p.TechItems.Any(t => MatchesTerms(terms, t.Name) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant())))))
                    ).ToList();
                }
                // Keep original ordering intent if any (projects.json has no explicit list_order; return as-is)
                return items;
            });
        }

        public async Task<IEnumerable<TechCategory>> GetTechCategories(List<string> searchTerms = null)
        {
            return await Task.Run(() =>
            {
                var cats = _techCategories.Select((c, index) => MapTechCategory(c, index + 1)).ToList();
                if (searchTerms != null && searchTerms.Count > 0)
                {
                    var terms = searchTerms.Select(t => t.Trim().ToUpperInvariant()).ToHashSet();
                    cats = cats.Where(c =>
                        MatchesTerms(terms, c.Name) ||
                        (c.TechItems != null && c.TechItems.Any(t => MatchesTerms(terms, t.Name) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant())))))
                    ).ToList();
                }
                return cats;
            });
        }

        public async Task<IEnumerable<EdItem>> GetEdItems(List<string> searchTerms = null)
        {
            return await Task.Run(() =>
            {
                var items = _ed.Select(MapEdItem).ToList();
                if (searchTerms != null && searchTerms.Count > 0)
                {
                    var terms = searchTerms.Select(t => t.Trim().ToUpperInvariant()).ToHashSet();
                    items = items.Where(e =>
                        MatchesTerms(terms, e.SchoolName) ||
                        MatchesTerms(terms, e.Degree) ||
                        (e.Xref != null && e.Xref.Any(x => terms.Contains(x.ToUpperInvariant())))
                    ).ToList();
                }
                return items;
            });
        }

        // Mapping helpers
        private WorkItem MapWorkItem(WorkJson w)
        {
            var item = new WorkItem
            {
                CompanyName = w.CompanyName,
                Division = w.Division,
                Position = w.Position,
                Location = w.Location,
                Image = w.Image,
                BulletPoints = w.BulletPoints ?? new List<string>()
            };

            // Parse StartDate
            if (!string.IsNullOrWhiteSpace(w.StartDate) && DateTime.TryParse(w.StartDate, out var startDate))
            {
                item.StartDate = startDate;
            }

            // Parse EndDate (can be empty)
            if (!string.IsNullOrWhiteSpace(w.EndDate) && DateTime.TryParse(w.EndDate, out var endDate))
            {
                item.EndDate = endDate;
            }

            // Map tech items from TechXref names to full tech detail if available
            if (w.TechXref != null && w.TechXref.Count > 0)
            {
                foreach (var name in w.TechXref)
                {
                    var key = (name ?? string.Empty).Trim().ToUpperInvariant();
                    if (!string.IsNullOrWhiteSpace(key) && _techByName.TryGetValue(key, out var tech))
                    {
                        item.TechItems.Add(CloneTechItem(tech));
                    }
                    else
                    {
                        // Fallback minimal tech item
                        item.TechItems.Add(new TechItem { Name = name ?? string.Empty });
                    }
                }
            }
            return item;
        }

        private ProjectItem MapProjectItem(ProjectJson p)
        {
            var item = new ProjectItem
            {
                ProjectName = p.SiteName,
                Url = p.Url,
                LastWorkedOnDate = p.Date,
                Description = p.Description,
                Purpose = p.Purpose,
                OpenSource = p.OpenSource,
                Image = p.Image,
                Screenshots = p.Screenshot,
                Status = p.Status,
                CodeAvailable = p.CodeAvailable,
                ProjectType = p.Types ?? new List<string>(),
                TechnologyUsed = p.TechnologyUsed ?? new List<string>()
            };

            if (p.TechXref != null && p.TechXref.Count > 0)
            {
                foreach (var name in p.TechXref)
                {
                    var key = (name ?? string.Empty).Trim().ToUpperInvariant();
                    if (!string.IsNullOrWhiteSpace(key) && _techByName.TryGetValue(key, out var tech))
                    {
                        item.TechItems.Add(CloneTechItem(tech));
                    }
                    else
                    {
                        item.TechItems.Add(new TechItem { Name = name ?? string.Empty });
                    }
                }
            }
            return item;
        }

        private TechCategory MapTechCategory(TechJsonCategory c, int id)
        {
            var cat = new TechCategory
            {
                Id = id,
                Name = c.Category,
                SingularName = c.Title,
                Xref = c.Xref ?? new List<string>(),
                TechItems = new List<TechItem>()
            };
            if (c.Items != null)
            {
                foreach (var item in c.Items)
                {
                    cat.TechItems.Add(MapTechItem(c, item, id, true));
                }
            }
            return cat;
        }

        private TechItem MapTechItem(TechJsonCategory cat, TechJsonItem item, int categoryId, bool populateRelations = false)
        {
            var techItem = new TechItem
            {
                Name = item.Name,
                Years = item.Years,
                ExperienceLevel = item.ExperienceLevel,
                Image = item.Image,
                Versions = item.Versions,
                BulletPoints = item.BulletPoints ?? new List<string>(),
                Xref = item.Xref ?? new List<string>(),
                CategoryId = categoryId,
                CategoryName = cat.Category
            };

            if (populateRelations)
            {
                var techName = item.Name.Trim().ToUpperInvariant();

                if (_work != null)
                {
                    var matchingWork = _work.Where(w => w.TechXref != null && w.TechXref.Any(x => x.Trim().ToUpperInvariant() == techName));
                    techItem.WorkItems = matchingWork.Select(MapWorkItem).ToList();
                }

                if (_projects != null)
                {
                    var matchingProjects = _projects.Where(p => p.TechXref != null && p.TechXref.Any(x => x.Trim().ToUpperInvariant() == techName));
                    techItem.ProjectItems = matchingProjects.Select(MapProjectItem).ToList();
                }
            }

            return techItem;
        }

        private EdItem MapEdItem(EdJson e)
        {
            return new EdItem
            {
                SchoolName = e.SchoolName,
                Degree = e.Degree,
                GraduationDate = e.GraduationDate,
                Image = e.Image,
                Xref = e.Xref ?? new List<string>(),
                BulletPoints = e.BulletPoints ?? new List<string>()
            };
        }

        private TechItem CloneTechItem(TechItem t)
        {
            return new TechItem
            {
                Name = t.Name,
                Years = t.Years,
                ExperienceLevel = t.ExperienceLevel,
                Image = t.Image,
                Versions = t.Versions,
                BulletPoints = t.BulletPoints != null ? new List<string>(t.BulletPoints) : new List<string>(),
                Xref = t.Xref != null ? new List<string>(t.Xref) : new List<string>(),
                WorkItems = t.WorkItems != null ? new List<WorkItem>(t.WorkItems) : new List<WorkItem>(),
                ProjectItems = t.ProjectItems != null ? new List<ProjectItem>(t.ProjectItems) : new List<ProjectItem>(),
                CategoryId = t.CategoryId,
                CategoryName = t.CategoryName
            };
        }

        private bool MatchesTerms(HashSet<string> terms, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            var v = value.ToUpperInvariant();
            foreach (var t in terms)
            {
                if (v.Contains(t))
                {
                    return true;
                }
            }
            return false;
        }
    }

    // JSON DTOs used for deserialization
    internal class EdJson
    {
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string GraduationDate { get; set; }
        public string Image { get; set; }
        public List<string> Xref { get; set; }
        public List<string> BulletPoints { get; set; }
    }

    internal class ProjectJson
    {
        public string SiteName { get; set; }
        public string Url { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
        public bool OpenSource { get; set; }
        public string Image { get; set; }
        public string Screenshot { get; set; }
        public string Status { get; set; }
        public bool CodeAvailable { get; set; }
        public List<string> Types { get; set; }
        public List<string> TechnologyUsed { get; set; }
        public List<string> TechXref { get; set; }
    }

    internal class TechJsonCategory
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public List<string> Xref { get; set; }
        public List<TechJsonItem> Items { get; set; }
    }

    internal class TechJsonItem
    {
        public string Name { get; set; }
        public int Years { get; set; }
        public int ExperienceLevel { get; set; }
        public string Image { get; set; }
        public string Versions { get; set; }
        public List<string> BulletPoints { get; set; }
        public List<string> Xref { get; set; }
    }

    internal class WorkJson
    {
        public string CompanyName { get; set; }
        public string Division { get; set; }
        public string CompanyNote { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public List<string> Xref { get; set; }
        public List<string> TechXref { get; set; }
        public List<string> BulletPoints { get; set; }
    }
}
