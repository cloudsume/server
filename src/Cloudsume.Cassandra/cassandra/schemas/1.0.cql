CREATE TABLE degrees (
    id UUID,
    level TINYINT,
    names MAP<ASCII, TEXT>,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE skill_categories (
    id UUID,
    names MAP<ASCII, TEXT>,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE skills (
    id UUID,
    type TINYINT,
    category UUID,
    names MAP<ASCII, TEXT>,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE institutes (
    id UUID,
    names MAP<ASCII, TEXT>,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE campuses (
    id UUID,
    institute UUID,
    flags TINYINT,
    location ASCII,
    names MAP<ASCII, TEXT>,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_links (
    resume_id UUID,
    id BLOB,
    name TEXT,
    user_id UUID,
    created_at TIMESTAMP,
    PRIMARY KEY (resume_id, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_link_accesses (
    link_id BLOB,
    id BLOB,
    from_ip INET,
    PRIMARY KEY (link_id, id)
) WITH CLUSTERING ORDER BY (id DESC);

CREATE MATERIALIZED VIEW degrees_by_level AS
    SELECT * FROM degrees WHERE level IS NOT NULL AND id IS NOT NULL
    PRIMARY KEY (level, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE MATERIALIZED VIEW skills_by_type AS
    SELECT * FROM skills WHERE type IS NOT NULL AND id IS NOT NULL
    PRIMARY KEY (type, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE MATERIALIZED VIEW campuses_by_location AS
    SELECT * FROM campuses WHERE location IS NOT NULL AND id IS NOT NULL
    PRIMARY KEY (location, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE MATERIALIZED VIEW resume_links_by_id AS
    SELECT * FROM resume_links
    WHERE id IS NOT NULL AND resume_id IS NOT NULL
    PRIMARY KEY (id, resume_id);

ALTER TABLE template_registrations ADD website TEXT;
