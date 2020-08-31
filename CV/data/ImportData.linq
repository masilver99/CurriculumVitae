<Query Kind="Program">
  <Connection>
    <ID>a8f5e66d-4c69-4697-9c82-058c0dd9182c</ID>
    <Persist>true</Persist>
    <Driver Assembly="IQDriver" PublicKeyToken="5b59726538a49684">IQDriver.IQDriver</Driver>
    <Provider>System.Data.SQLite</Provider>
    <CustomCxString>Data Source=C:\dev\CurriculumVitae\CV\data\cv.db;FailIfMissing=True</CustomCxString>
    <AttachFileName>C:\dev\CurriculumVitae\CV\data\cv.db</AttachFileName>
    <NoCapitalization>true</NoCapitalization>
    <NoPluralization>true</NoPluralization>
    <DisplayName>cv.db</DisplayName>
    <DriverData>
      <StripUnderscores>false</StripUnderscores>
      <QuietenAllCaps>false</QuietenAllCaps>
    </DriverData>
  </Connection>
  <NuGetReference>Dapper</NuGetReference>
  <NuGetReference>Microsoft.Data.Sqlite</NuGetReference>
  <NuGetReference>Microsoft.Data.Sqlite.Core</NuGetReference>
  <NuGetReference>SQLitePCLRaw.bundle_e_sqlite3</NuGetReference>
  <NuGetReference>System.Text.Json</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>Dapper</Namespace>
</Query>

void Main()
{
	var options = new JsonSerializerOptions();
	options.ReadCommentHandling = JsonCommentHandling.Skip;
	TechCategories techCats = new TechCategories(JsonSerializer.Deserialize<List<TechCategory>>(System.IO.File.ReadAllText(@"C:\dev\CurriculumVitae\CV\data\tech.json"), options));
	var workItems = JsonSerializer.Deserialize<List<WorkItem>>(System.IO.File.ReadAllText(@"C:\dev\CurriculumVitae\CV\data/work.json"), options);
	var projectItems = JsonSerializer.Deserialize<List<ProjectItem>>(System.IO.File.ReadAllText(@"C:\dev\CurriculumVitae\CV\data/projects.json"), options);
	var edItems = JsonSerializer.Deserialize<List<EdItem>>(System.IO.File.ReadAllText(@"C:\dev\CurriculumVitae\CV\data/ed.json"), options);
	// Write code to test your extensions here. Press F5 to compile and run.

    var connection = this.Connection;
	connection.Open();
	connection.Execute("DELETE FROM work_search_terms");
	connection.Execute("DELETE FROM work_tech");
	connection.Execute("DELETE FROM work");

	connection.Execute("DELETE FROM tech_search_terms");
	connection.Execute("DELETE FROM tech");

	connection.Execute("DELETE FROM category_search_terms");
	connection.Execute("DELETE FROM category");
	/*
		INSERT INTO work(id, company_name, division, company_note, start_date, end_date, position, location, image) VALUES
(1,
 "Wolters Kluwer",
 "Tax and Accounting",
 "Mostly Remote",
 "2018-05-01",
 NULL,
 "Senior Product Software Engineer",
 "Kennesaw, GA",
 "wk.png");

	INSERT INTO work_search_terms(work_id, search_ter) VALUES
*/
	int techId = 0;
	int catId = 0;
	foreach (var techCat in techCats)
	{
		connection.Execute(
		  "INSERT INTO category (id, name, singular_name) VALUES (@id, @name, @singular_name)",
		  new { id = ++catId, name = techCat.Category, singular_name = techCat.Title });
		foreach (var xref in techCat.Xref)
		{
			connection.Execute(
			  "INSERT INTO category_search_terms (category_id, search_term) VALUES (@category_id, @search_term)",
			  new { category_id = catId, search_term = xref });
		}
		foreach (var tech in techCat.Items)
		{
			var bulletPoints = String.Join("|", tech.BulletPoints);
			connection.Execute(
				@"INSERT INTO tech (id, name, years, experience_level, image, versions, bullet_points, category_id) VALUES 
				   (@id, @name, @years, @experience_level, @image, @versions, @bullet_points, @category_id)",
			    new { id = ++techId, name = tech.Name, years = tech.Years, experience_level = tech.ExperienceLevel, image = tech.Image, versions = tech.Versions, bullet_points = bulletPoints, category_id = catId});

			foreach (var xref in tech.Xref)
			{
				connection.Execute(
				  "INSERT INTO tech_search_terms (tech_id, search_term) VALUES (@tech_id, @search_term)",
				  new { tech_id = techId, search_term = xref });
			}
		}
	}

	int edId = 0;
	foreach (var edItem in edItems)
	{
		var bulletPoints = String.Join("|", edItem.BulletPoints);
		connection.Execute(
    		@"INSERT INTO ed (id, school_name, degree, graduation_date, image, bullet_points) VALUES 
			(@id, @school_name, @degree, @graduation_date, @image, @bullet_points)",
		  	new { id = ++edId, school_name = edItem.SchoolName, degree = edItem.Degree, 
			  graduation_date = edItem.GraduationDate, image = edItem.Image, bullet_points = bulletPoints });

		foreach (var xref in edItem.Xref)
		{
			connection.Execute(
				@"INSERT INTO ed_search_terms (ed_id, search_term) VALUES 
			(@ed_id, @search_term)",
				  new { ed_id = edId, search_term = xref });
		}
	}


	CREATE TABLE work_search_terms(
		work_id INTEGER NOT NULL,
		search_term TEXT NOT NULL,
		FOREIGN KEY(work_id) REFERENCES work(id),
		PRIMARY KEY(work_id, search_term)
	);

	CREATE TABLE work_tech(
		work_id INTEGER NOT NULL,
		tech_id INTEGER NOT NULL,
		FOREIGN KEY(work_id) REFERENCES work(id),
		FOREIGN KEY(tech_id) REFERENCES tech(id),
		PRIMARY KEY(work_id, tech_id)
	);
	int workId = 0;
	foreach (var workItem in workItems)
	{
		var bulletPoints = String.Join("|", workItem.BulletPoints);
		connection.Execute(
			@"INSERT INTO work (
				id, company_name, division, company_note, start_date, end_date, position, location, image, bullet_points) VALUES
				
			(@id, @company_name, @division, @company_note, @start_date, @end_date, @position, @location, @image, @bullet_points)",
		  	new
			  {
			      id = ++workId,
				  company_name = workItem.CompanyName,
				  division = workItem.Division,
				  company_note = workItem.CompanyNote,
				  start_date = , 
				  end_date = , 
				  position = workItem.Position,
				  location = workItem.Location,
				  image = workItem.Image, 
				  bullet_points = bulletPoints
			  });

		foreach (var xref in edItem.Xref)
		{
			connection.Execute(
				@"INSERT INTO ed_search_terms (ed_id, search_term) VALUES 
			(@ed_id, @search_term)",
				  new { ed_id = edId, search_term = xref });
		}
	}
}


public class EdItem
{
	public string SchoolName { get; set; }
	public string Degree { get; set; }
	public string GraduationDate { get; set; }
	public string Image { get; set; }
	public List<string> Xref { get; set; } = new List<string>();
	public List<string> BulletPoints { get; set; } = new List<string>();
}

public class TechItem
{
	/// <summary>
	/// Name of the technology
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Number of years of experience 
	/// </summary>
	public int Years { get; set; }

	public string Image { get; set; }

	/// <summary>
	/// The experience level, 1 being lowest, 10 being highest
	/// </summary>
	public int ExperienceLevel { get; set; }

	public string Versions { get; set; }

	public List<string> BulletPoints { get; set; } = new List<string>();

	public List<string> Xref { get; set; } = new List<string>();

	public int GetFullStars()
	{
		return (ExperienceLevel - GetHalfStars()) / 2;
	}

	public int GetHalfStars()
	{
		return (ExperienceLevel % 2);
	}

	public int GetEmptyStars()
	{
		return (10 - ExperienceLevel - GetHalfStars()) / 2;
	}
}


public class ProjectItem
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
	public List<string> TechXref { get; set; } = new List<string>();
	public List<TechItem> TechItems { get; set; } = new List<TechItem>();
	public List<string> TechnologyUsed { get; set; } = new List<string>();
	//Lookup Tech XRef for tech used
}

public enum ProjectType
{
	Hardware,   //<i class="fas fa-server"></i>
	Application,  //<i class="far fa-window-restore"></i>
	Website,  //<i class="fas fa-globe-americas"></i>
	Firmware  //<i class="fas fa-microchip"></i>
}

public class TechCategory
{
	public string Category { get; set; }
	public string Title { get; set; }
	public List<string> Xref { get; set; } = new List<string>();
	public List<TechItem> Items { get; set; }
	public TechCategory ItemlessCopy(List<TechItem> newTechItems)
	{
		return new TechCategory
		{
			Category = this.Category,
			Title = this.Title,
			Xref = this.Xref,
			Items = newTechItems
		};
	}
}

public class TechCategories : List<TechCategory>
{
	public TechCategories()
	{
	}

	public TechCategories(List<TechCategory> techCategories)
	{
		this.AddRange(techCategories);
	}


	public void SafeAddTechCat(TechCategory techCategory)
	{
		if (this.Where(t => t.Category == techCategory.Category).Count() == 0)
		{
			this.Add(techCategory);
		}
	}
}

public class WorkItem
{
	public string CompanyName { get; set; }
	public string CompanyNote {get; set; }
	public string Division { get; set; }
	public string Dates { get; set; }
	public string Position { get; set; }
	public string Location { get; set; }
	public string Image { get; set; }
	public List<string> Xref { get; set; } = new List<string>();
	public List<string> TechXref { get; set; } = new List<string>();
	public TechCategories TechCats { get; set; } = new TechCategories();
	public List<string> BulletPoints { get; set; } = new List<string>();
}


// You can also define non-static classes, enums, etc.