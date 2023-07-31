CREATE TYPE asset (
    name TEXT,
    size INT,
    last_modified TIMESTAMP
);

CREATE TABLE jobs (
    id UUID,
    names MAP<ASCII, TEXT>,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE workspaces (
    registration_id UUID,
    applicable_data FROZEN<LIST<ASCII>>,
    experience_options FROZEN<expopts>,
    education_options FROZEN<eduopts>,
    skill_options FROZEN<skillopts>,
    assets MAP<TEXT, FROZEN<asset>>,
    PRIMARY KEY (registration_id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

ALTER TABLE resume_languages ADD toefl_score TINYINT;
ALTER TABLE template_registrations ADD category TINYINT;
ALTER TABLE template_registrations ADD updated_at TIMESTAMP;

CREATE MATERIALIZED VIEW template_registrations_by_owner_id AS
    SELECT * FROM template_registrations WHERE owner_id IS NOT NULL AND id IS NOT NULL
    PRIMARY KEY (owner_id, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};
