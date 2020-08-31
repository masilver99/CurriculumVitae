DROP TABLE IF EXISTS tech_search_terms;
DROP TABLE IF EXISTS tech;

DROP TABLE IF EXISTS category_search_terms;
DROP TABLE IF EXISTS category;

DROP TABLE IF EXISTS ed_search_terms;
DROP TABLE IF EXISTS ed;

DROP TABLE IF EXISTS project_tech;
DROP TABLE IF EXISTS project_search_terms;
DROP TABLE IF EXISTS project;


DROP TABLE IF EXISTS work_tech;
DROP TABLE IF EXISTS work_search_terms;
DROP TABLE IF EXISTS work;


CREATE TABLE category (
	id INTEGER PRIMARY KEY,
   	name TEXT NOT NULL,
	singular_name TEXT NOT NULL
);

CREATE TABLE category_search_terms (
    category_id INTEGER NOT NULL,
    search_term TEXT NOT NULL,
	FOREIGN KEY(category_id) REFERENCES category(id),
	PRIMARY KEY(category_id, search_term)
);

CREATE TABLE tech (
	id INTEGER PRIMARY KEY,
   	name TEXT NOT NULL,
	years INTEGER NOT NULL,
    experience_level INTEGER NOT NULL,
	image TEXT,
	versions TEXT,
    bullet_points TEXT,
    category_id INTEGER NOT NULL,
    FOREIGN KEY(category_id) REFERENCES category(id)
);

CREATE TABLE tech_search_terms (
    tech_id INTEGER NOT NULL,
    search_term TEXT NOT NULL,
    FOREIGN KEY(tech_id) REFERENCES tech(id),
	PRIMARY KEY(tech_id, search_term)
);

CREATE TABLE ed (
	id INTEGER PRIMARY KEY,
    school_name TEXT NOT NULL,
    degree TEXT NOT NULL, 
    graduation_date INTEGER NOT NULL,
    image TEXT,
    bullet_points
);

CREATE TABLE ed_search_terms (
    ed_id INTEGER NOT NULL,
    search_term TEXT NOT NULL,
    FOREIGN KEY(ed_id) REFERENCES ed(id),
	PRIMARY KEY(ed_id, search_term)
);

CREATE TABLE project (
	id INTEGER PRIMARY KEY,
	project_name TEXT NOT NULL,
	url TEXT,
	last_worked_on_date DATE,
	description TEXT NOT NULL,
	purpose TEXT NOT NULL,
	opensource BOOLEAN NOT NULL,
	image TEXT NOT NULL,
	screenshots TEXT,
    status TEXT,
    code_available BOOLEAN NOT NULL,
    project_type TEXT NOT NULL
);

CREATE TABLE project_search_terms (
    project_id INTEGER NOT NULL,
    search_term TEXT NOT NULL,
    FOREIGN KEY(project_id) REFERENCES project(id),
	PRIMARY KEY(project_id, search_term)
);

CREATE TABLE project_tech (
    project_id INTEGER NOT NULL,
    tech_id INTEGER NOT NULL,
    FOREIGN KEY(project_id) REFERENCES project(id),
    FOREIGN KEY(tech_id) REFERENCES tech(id),
	PRIMARY KEY(project_id, tech_id)
);

CREATE TABLE work (
	id INTEGER PRIMARY KEY,
    company_name TEXT NOT NULL,
    division TEXT,
    company_note TEXT,
    start_date DATE NOT NULL,
    end_date DATE,
    position TEXT NOT NULL,
    location TEXT NOT NULL,
    image TEXT NOT NULL,
    bullet_points TEXT NOT NULL

);

CREATE TABLE work_search_terms (
    work_id INTEGER NOT NULL,
    search_term TEXT NOT NULL,
    FOREIGN KEY(work_id) REFERENCES work(id),
	PRIMARY KEY(work_id, search_term)
);

CREATE TABLE work_tech (
    work_id INTEGER NOT NULL,
    tech_id INTEGER NOT NULL,
    FOREIGN KEY(work_id) REFERENCES work(id),
    FOREIGN KEY(tech_id) REFERENCES tech(id),
	PRIMARY KEY(work_id, tech_id)
);
