using CV.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.data
{
    public class Repository
    {
        public Repository()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new StringArrayHandler());
        }

        private async Task<SqliteConnection> GetConnection()
        {
            var connection = new SqliteConnection(@"Data Source=data\cv.db;");
            await connection.OpenAsync();
            return connection;
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItems(List<string> searchTerms = null)
        {
            string searchSql = null;
            if (searchTerms != null && searchTerms.Count > 0)
            {
                searchSql = @" LEFT OUTER JOIN work_search_terms wst ON w.id = wst.work_id 
LEFT OUTER JOIN tech_search_terms tst ON t.id = tst.tech_id 
WHERE wst.search_term in @searchTerms OR tst.search_term in @searchTerms";
            }

            using var connection = await GetConnection();
            var sql = @$"SELECT DISTINCT w.*, t.* 
FROM work w 
LEFT OUTER JOIN work_tech wt on w.id = wt.work_id
LEFT OUTER JOIN tech t on wt.tech_id = t.id{searchSql};";
            var lookup = new Dictionary<int, WorkItem>();
            await connection.QueryAsync<WorkItem, TechItem, WorkItem>(sql,
				(w, t) =>
                {
                    WorkItem workItem;
                    if (!lookup.TryGetValue(w.Id, out workItem))
                    {
                        lookup.Add(w.Id, workItem = w);
                    }
                    workItem.TechItems.Add(t);
                    return workItem;
                },
                new { searchTerms = searchTerms?.ToArray() }
                );
            return lookup.Values;
        }

        public async Task<IEnumerable<ProjectItem>> GetProjectItems(List<string> searchTerms = null)
        {

            string searchSql = null;
            if (searchTerms != null && searchTerms.Count > 0)
            {
                searchSql = @" LEFT OUTER JOIN tech_search_terms tst ON t.id = tst.tech_id 
WHERE tst.search_term in @searchTerms ";
            }

            using var connection = await GetConnection();
            var sql = @$"SELECT p.*, t.* 
FROM project p 
LEFT OUTER JOIN project_tech pt on p.id = pt.project_id
LEFT OUTER JOIN tech t on pt.tech_id = t.id{searchSql} ORDER BY p.list_order;";
            var lookup = new Dictionary<int, ProjectItem>();
            await connection.QueryAsync<ProjectItem, TechItem, ProjectItem>(sql,
                (p, t) =>
                {
                    ProjectItem projectItem;
                    if (!lookup.TryGetValue(p.Id, out projectItem))
                    {
                        lookup.Add(p.Id, projectItem = p);
                    }
                    if (t != null) projectItem.TechItems.Add(t);

                    return projectItem;
                },
                new { searchTerms = searchTerms?.ToArray() }
                );
            return lookup.Values;
        }

        public async Task<IEnumerable<TechCategory>> GetTechCategories(List<string> searchTerms = null)
        {
            string searchSql = null;
            if (searchTerms != null && searchTerms.Count > 0)
            {
                searchSql = @" LEFT OUTER JOIN category_search_terms cst ON p.id = cst.category_id 
LEFT OUTER JOIN tech_search_terms tst ON t.id = tst.tech_id 
WHERE cst.search_term in @searchTerms OR tst.search_term in @searchTerms";
            }

            using var connection = await GetConnection();
            var sql = @$"SELECT DISTINCT p.*, t.* 
FROM category p 
LEFT OUTER JOIN tech t on p.id = t.category_id{searchSql};";
            var lookup = new Dictionary<int, TechCategory>();
            await connection.QueryAsync<TechCategory, TechItem, TechCategory>(sql,
                (c, t) =>
                {
                    TechCategory techCategory;
                    if (!lookup.TryGetValue(c.Id, out techCategory))
                    {
                        lookup.Add(c.Id, techCategory = c);
                    }
                    techCategory.TechItems.Add(t);
                    return techCategory;
                }, 
                new { searchTerms = searchTerms?.ToArray() }
                );
            return lookup.Values;
        }

        public async Task<IEnumerable<EdItem>> GetEdItems(List<string> searchTerms = null)
        {
            string searchSql = null;
            if (searchTerms != null && searchTerms.Count > 0)
            {
                searchSql = " JOIN ed_search_terms est ON e.id = est.ed_id WHERE est.search_term in @searchTerms";
            }
            using var connection = await GetConnection();
            var sql = @$"SELECT e.* FROM ed e{searchSql};";
            return await connection.QueryAsync<EdItem>(sql, new { searchTerms = searchTerms?.ToArray() });
        }
    }
}
