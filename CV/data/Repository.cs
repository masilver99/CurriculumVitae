using CV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace CV.data
{
    public class Repository
    {
        // In-memory stores loaded at startup
        private readonly List<EdJson> _ed = new List<EdJson>();
        private readonly List<ProjectJson> _projects = new List<ProjectJson>();
        private readonly List<TechJsonCategory> _techCategories = new List<TechJsonCategory>();
        private readonly List<WorkJson> _work = new List<WorkJson>();
        private readonly List<CertificationJson> _certifications = new List<CertificationJson>();

        private readonly Dictionary<string, TechItem> _techByName = new Dictionary<string, TechItem>();
        private readonly Dictionary<string, WorkJson> _workById = new Dictionary<string, WorkJson>();

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
            var certificationsPath = Path.Combine(basePath, "certifications.json");

            if (File.Exists(edPath))
            {
                var edJson = File.ReadAllText(edPath);
                var edItems = JsonSerializer.Deserialize<List<EdJson>>(edJson, options);
                if (edItems != null)
                {
                    _ed.AddRange(edItems);
                }
            }
            else
            {
                Log.Warning("{EdPath} not found", edPath);
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
            else
            {
                Log.Warning("{projectsPath} not found", projectsPath);
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
                                if (!string.IsNullOrWhiteSpace(techItem.Id))
                                {
                                    var key = techItem.Id.Trim().ToUpperInvariant();
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
            else
            {
                Log.Warning("{techPath} not found", techPath);
            }

            if (File.Exists(workPath))
            {
                var workJson = File.ReadAllText(workPath);
                var workItems = JsonSerializer.Deserialize<List<WorkJson>>(workJson, options);
                if (workItems != null)
                {
                    _work.AddRange(workItems);
                    // Build work lookup by Id
                    foreach (var w in workItems)
                    {
                        if (!string.IsNullOrWhiteSpace(w.Id))
                        {
                            var key = w.Id.Trim().ToUpperInvariant();
                            if (!_workById.ContainsKey(key))
                            {
                                _workById.Add(key, w);
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Warning("{workPath} not found", workPath);
            }

            if (File.Exists(certificationsPath))
            {
                var certsJson = File.ReadAllText(certificationsPath);
                var certItems = JsonSerializer.Deserialize<List<CertificationJson>>(certsJson, options);
                if (certItems != null)
                {
                    _certifications.AddRange(certItems);
                }
            }
            else
            {
                Log.Warning("{certificationsPath} not found", certificationsPath);
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
                        w.TechItems.Any(t => MatchesTerms(terms, t.DisplayName) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant()))))
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
                        (p.Description != null && p.Description.Any(d => MatchesTerms(terms, d))) ||
                        (p.TechItems != null && p.TechItems.Any(t => MatchesTerms(terms, t.DisplayName) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant())))))
                    ).ToList();
                }
                // Keep original ordering intent if any (projects.json has no explicit list_order; return as-is)
                return items;
            });
        }

        public async Task<IEnumerable<CertificationItem>> GetCertificationItems(List<string> searchTerms = null)
        {
            return await Task.Run(() =>
            {
                var items = _certifications.Select(MapCertificationItem).ToList();
                if (searchTerms != null && searchTerms.Count > 0)
                {
                    var terms = searchTerms.Select(t => t.Trim().ToUpperInvariant()).ToHashSet();
                    items = items.Where(c =>
                        MatchesTerms(terms, c.Name) ||
                        MatchesTerms(terms, c.IssuedBy) ||
                        MatchesTerms(terms, c.DateIssued) ||
                        MatchesTerms(terms, c.ExpirationDate) ||
                        (c.TechItems != null && c.TechItems.Any(t => MatchesTerms(terms, t.DisplayName) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant())))))
                    ).ToList();
                }
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
                        (c.TechItems != null && c.TechItems.Any(t => MatchesTerms(terms, t.DisplayName) || (t.Xref != null && t.Xref.Any(x => terms.Contains(x.ToUpperInvariant())))))
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
                Id = w.Id,
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
                        item.TechItems.Add(new TechItem { Id = name ?? string.Empty, DisplayName = name ?? string.Empty });
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
                Status = p.Status,
                CodeAvailable = p.CodeAvailable,
                ProjectType = p.Types ?? new List<string>(),
                TechnologyUsed = p.TechnologyUsed ?? new List<string>(),
                WorkXref = p.WorkXref,
                Screenshots = p.Screenshots
            };

            // Resolve WorkItem if WorkXref is provided
            if (!string.IsNullOrWhiteSpace(p.WorkXref))
            {
                var key = p.WorkXref.Trim().ToUpperInvariant();
                if (_workById.TryGetValue(key, out var workJson))
                {
                    item.WorkItem = MapWorkItem(workJson);
                }
            }

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
                        item.TechItems.Add(new TechItem { Id = name ?? string.Empty, DisplayName = name ?? string.Empty });
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
                Id = item.Id,
                DisplayName = item.DisplayName,
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
                var techName = item.Id.Trim().ToUpperInvariant();

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
                Id = t.Id,
                DisplayName = t.DisplayName,
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

        private CertificationItem MapCertificationItem(CertificationJson c)
        {
            var item = new CertificationItem
            {
                Name = c.Name,
                IssuedBy = c.IssuedBy,
                DateIssued = c.DateIssued,
                ExpirationDate = c.ExpirationDate,
                VerificationUrl = c.VerificationUrl,
                ImageUrl = c.ImageUrl,
                DownloadUrl = c.DownloadUrl
            };

            if (c.TechXref != null && c.TechXref.Count > 0)
            {
                foreach (var name in c.TechXref)
                {
                    var key = (name ?? string.Empty).Trim().ToUpperInvariant();
                    if (!string.IsNullOrWhiteSpace(key) && _techByName.TryGetValue(key, out var tech))
                    {
                        item.TechItems.Add(CloneTechItem(tech));
                    }
                    else
                    {
                        item.TechItems.Add(new TechItem { Id = name ?? string.Empty, DisplayName = name ?? string.Empty });
                    }
                }
            }

            return item;
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

        /// <summary>
        /// Generates a markdown version of the curriculum vitae that is friendly to LLMs.
        /// </summary>
        /// <returns>A markdown formatted string containing all CV information</returns>
        public async Task<string> GenerateLlmFriendlyMarkdown()
        {
            return await GenerateLlmFriendlyMarkdown(false);
        }

        /// <summary>
        /// Generates a markdown version of the curriculum vitae with optional compact output.
        /// </summary>
        /// <param name="compact">If true, omits verbose fields for a shorter output.</param>
        public async Task<string> GenerateLlmFriendlyMarkdown(bool compact)
        {
            return await Task.Run(async () =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine("# Curriculum Vitae");
                sb.AppendLine();

                // Work Experience
                sb.AppendLine("## Work Experience");
                sb.AppendLine();
                var workItems = await GetWorkItems();
                foreach (var work in workItems)
                {
                    sb.AppendLine($"### {work.Position} at {work.CompanyName}");
                    if (!compact && !string.IsNullOrWhiteSpace(work.Division))
                    {
                        sb.AppendLine($"**Division:** {work.Division}");
                    }
                    if (!string.IsNullOrWhiteSpace(work.Location))
                    {
                        sb.AppendLine($"**Location:** {work.Location}");
                    }
                    sb.Append($"**Duration:** {work.StartDate:MMMM yyyy}");
                    if (work.EndDate.HasValue)
                    {
                        sb.Append($" - {work.EndDate:MMMM yyyy}");
                    }
                    else
                    {
                        sb.Append(" - Present");
                    }
                    sb.AppendLine();

                    if (!compact && work.BulletPoints != null && work.BulletPoints.Count > 0)
                    {
                        sb.AppendLine();
                        foreach (var bullet in work.BulletPoints)
                        {
                            sb.AppendLine($"- {bullet}");
                        }
                    }

                    if (!compact && work.TechItems != null && work.TechItems.Count > 0)
                    {
                        sb.AppendLine();
                        sb.Append("**Technologies:** ");
                        sb.AppendLine(string.Join(", ", work.TechItems.Select(t => t.DisplayName)));
                    }
                    sb.AppendLine();
                }

                // Education
                sb.AppendLine("## Education");
                sb.AppendLine();
                var edItems = await GetEdItems();
                foreach (var ed in edItems)
                {
                    sb.AppendLine($"### {ed.Degree}");
                    sb.AppendLine($"**Institution:** {ed.SchoolName}");
                    if (!string.IsNullOrWhiteSpace(ed.GraduationDate))
                    {
                        sb.AppendLine($"**Graduation Date:** {ed.GraduationDate}");
                    }

                    if (!compact && ed.BulletPoints != null && ed.BulletPoints.Count > 0)
                    {
                        sb.AppendLine();
                        foreach (var bullet in ed.BulletPoints)
                        {
                            sb.AppendLine($"- {bullet}");
                        }
                    }
                    sb.AppendLine();
                }

                // Certifications
                sb.AppendLine("## Certifications");
                sb.AppendLine();
                var certItems = await GetCertificationItems();
                foreach (var cert in certItems)
                {
                    sb.AppendLine($"### {cert.Name}");
                    if (!string.IsNullOrWhiteSpace(cert.IssuedBy))
                    {
                        sb.AppendLine($"**Issued By:** {cert.IssuedBy}");
                    }
                    if (!string.IsNullOrWhiteSpace(cert.DateIssued))
                    {
                        sb.AppendLine($"**Date Issued:** {cert.DateIssued}");
                    }
                    if (!compact && !string.IsNullOrWhiteSpace(cert.ExpirationDate))
                    {
                        sb.AppendLine($"**Expires:** {cert.ExpirationDate}");
                    }
                    if (!string.IsNullOrWhiteSpace(cert.VerificationUrl))
                    {
                        sb.AppendLine($"**Verification:** {cert.VerificationUrl}");
                    }

                    if (!compact && cert.TechItems != null && cert.TechItems.Count > 0)
                    {
                        sb.AppendLine();
                        sb.Append("**Related Technologies:** ");
                        sb.AppendLine(string.Join(", ", cert.TechItems.Select(t => t.DisplayName)));
                    }
                    sb.AppendLine();
                }

                // Technical Skills
                sb.AppendLine("## Technical Skills");
                sb.AppendLine();
                var techCategories = await GetTechCategories();
                foreach (var category in techCategories)
                {
                    sb.AppendLine($"### {category.SingularName}");
                    sb.AppendLine();

                    if (category.TechItems != null && category.TechItems.Count > 0)
                    {
                        var items = category.TechItems;
                        if (compact)
                        {
                            // In compact mode, show only top 5 by ExperienceLevel
                            items = items.OrderByDescending(t => t.ExperienceLevel).Take(5).ToList();
                        }
                        else
                        {
                            items = items.OrderByDescending(t => t.ExperienceLevel).ToList();
                        }
                        foreach (var tech in items)
                        {
                            sb.Append($"- **{tech.DisplayName}**");
                            if (tech.Years > 0)
                            {
                                sb.Append($" ({tech.Years} years)");
                            }
                            if (!compact && tech.ExperienceLevel > 0)
                            {
                                sb.Append($" - Level: {tech.ExperienceLevel}/10");
                            }
                            sb.AppendLine();

                            if (!compact && !string.IsNullOrWhiteSpace(tech.Versions))
                            {
                                sb.AppendLine($"  Versions: {tech.Versions}");
                            }

                            if (!compact && tech.BulletPoints != null && tech.BulletPoints.Count > 0)
                            {
                                foreach (var bullet in tech.BulletPoints)
                                {
                                    sb.AppendLine($"  - {bullet}");
                                }
                            }
                        }
                    }
                    sb.AppendLine();
                }

                // Projects
                sb.AppendLine("## Projects");
                sb.AppendLine();
                var projectItems = await GetProjectItems();
                foreach (var project in projectItems)
                {
                    sb.AppendLine($"### {project.ProjectName}");
                    if (!string.IsNullOrWhiteSpace(project.Url))
                    {
                        sb.AppendLine($"**URL:** {project.Url}");
                    }
                    if (!string.IsNullOrWhiteSpace(project.Status))
                    {
                        sb.AppendLine($"**Status:** {project.Status}");
                    }
                    if (!string.IsNullOrWhiteSpace(project.LastWorkedOnDate))
                    {
                        sb.AppendLine($"**Last Updated:** {project.LastWorkedOnDate}");
                    }

                    if (!compact && !string.IsNullOrWhiteSpace(project.Purpose))
                    {
                        sb.AppendLine();
                        sb.AppendLine($"**Purpose:** {project.Purpose}");
                    }

                    if (!compact && project.Description != null && project.Description.Count > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine("**Description:**");
                        foreach (var desc in project.Description)
                        {
                            sb.AppendLine($"- {desc}");
                        }
                    }

                    if (project.ProjectType != null && project.ProjectType.Count > 0)
                    {
                        sb.AppendLine();
                        sb.Append("**Project Type:** ");
                        sb.AppendLine(string.Join(", ", project.ProjectType));
                    }

                    if (project.TechItems != null && project.TechItems.Count > 0)
                    {
                        sb.AppendLine();
                        sb.Append("**Technologies Used:** ");
                        if (compact)
                        {
                            sb.AppendLine(string.Join(", ", project.TechItems.Select(t => t.DisplayName).Take(5)));
                        }
                        else
                        {
                            sb.AppendLine(string.Join(", ", project.TechItems.Select(t => t.DisplayName)));
                        }
                    }

                    if (!compact)
                    {
                        sb.AppendLine($"**Open Source:** {(project.OpenSource ? "Yes" : "No")}");
                        sb.AppendLine($"**Code Available:** {(project.CodeAvailable ? "Yes" : "No")}");
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            });
        }

        // ...existing code...
    }

    // JSON DTOs used for deserialization
    internal class CertificationJson
    {
        public string Name { get; set; }
        public string IssuedBy { get; set; }
        public string DateIssued { get; set; }
        public string ExpirationDate { get; set; }
        public List<string> TechXref { get; set; }
        public string VerificationUrl { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadUrl { get; set; }
    }
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
        public List<string> Description { get; set; }
        public string Purpose { get; set; }
        public bool OpenSource { get; set; }
        public string Image { get; set; }
        public string Screenshot { get; set; }
        public List<string> Screenshots { get; set; }
        public string Status { get; set; }
        public bool CodeAvailable { get; set; }
        public List<string> Types { get; set; }
        public List<string> TechnologyUsed { get; set; }
        public List<string> TechXref { get; set; }
        public string WorkXref { get; set; }
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
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public int Years { get; set; }
        public int ExperienceLevel { get; set; }
        public string Image { get; set; }
        public string Versions { get; set; }
        public List<string> BulletPoints { get; set; }
        public List<string> Xref { get; set; }
    }

    internal class WorkJson
    {
        public string Id { get; set; }
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
