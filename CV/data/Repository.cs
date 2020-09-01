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

        public async Task<IEnumerable<WorkItem>> GetAllWorkItems()
        {
            using var connection = await GetConnection();
            var sql = @"SELECT w.*, w.id as techid, t.* 
FROM work w 
LEFT OUTER JOIN work_tech wt on w.id = wt.work_id
LEFT OUTER JOIN tech t on wt.tech_id = t.id;";
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
                }
                );
            return lookup.Values;
        }

        public async Task<IEnumerable<ProjectItem>> GetAllProjectItems()
        {
            using var connection = await GetConnection();
            var sql = @"SELECT p.*, t.* 
FROM project p 
LEFT OUTER JOIN project_tech pt on p.id = pt.project_id
LEFT OUTER JOIN tech t on pt.tech_id = t.id;";
            var lookup = new Dictionary<int, ProjectItem>();
            await connection.QueryAsync<ProjectItem, TechItem, ProjectItem>(sql,
                (p, t) =>
                {
                    ProjectItem projectItem;
                    if (!lookup.TryGetValue(p.Id, out projectItem))
                    {
                        lookup.Add(p.Id, projectItem = p);
                    }
                    projectItem.TechItems.Add(t);
                    return projectItem;
                }
                );
            return lookup.Values;
        }
    }
}
