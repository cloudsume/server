-- types

CREATE TYPE asciiprop (
    flags TINYINT,
    value ASCII
);

CREATE TYPE dateprop (
    flags TINYINT,
    value DATE
);

CREATE TYPE eduopts (
    desc_paragraph ASCII,
    desc_list_options ASCII
);

CREATE TYPE expopts (
    desc_paragraph ASCII,
    desc_list_options ASCII
);

CREATE TYPE ielts (
    type TINYINT,
    band DECIMAL
);

CREATE TYPE ieltsprop (
    flags TINYINT,
    value frozen<ielts>
);

CREATE TYPE month (
    year SMALLINT,
    month TINYINT
);

CREATE TYPE monthprop (
    flags TINYINT,
    value frozen<month>
);

CREATE TYPE photo (
    format TINYINT,
    size INT
);

CREATE TYPE photoprop (
    flags TINYINT,
    value frozen<photo>
);

CREATE TYPE skillopts (
    grouping TINYINT
);

CREATE TYPE smallintprop (
    flags TINYINT,
    value SMALLINT
);

CREATE TYPE telephone (
    country ASCII,
    number ASCII
);

CREATE TYPE telprop (
    flags TINYINT,
    value frozen<telephone>
);

CREATE TYPE textprop (
    flags TINYINT,
    value TEXT
);

CREATE TYPE tinyintprop (
    flags TINYINT,
    value TINYINT
);

CREATE TYPE uuidprop (
    flags TINYINT,
    value UUID
);

-- tables

CREATE TABLE resumes (
    user_id UUID,
    id UUID,
    name TEXT,
    template_id BLOB,
    created_at TIMESTAMP,
    PRIMARY KEY (user_id, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_addresses (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    region frozen<asciiprop>,
    street frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_educations (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    position TINYINT,
    id UUID,
    base_id UUID,
    start frozen<monthprop>,
    end frozen<monthprop>,
    campus_id frozen<uuidprop>,
    degree_id frozen<uuidprop>,
    grade frozen<textprop>,
    description frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language, position, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_emails (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_experiences (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    position TINYINT,
    id UUID,
    base_id UUID,
    start frozen<dateprop>,
    end frozen<dateprop>,
    title frozen<textprop>,
    company frozen<textprop>,
    region frozen<asciiprop>,
    street frozen<textprop>,
    description frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language, position, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_githubs (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_headlines (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_languages (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    position TINYINT,
    id UUID,
    base_id UUID,
    language_id frozen<tinyintprop>,
    comment frozen<textprop>,
    proficiency_flags TINYINT,
    ilr_scale TINYINT,
    toeic_score SMALLINT,
    ielts_score frozen<ielts>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language, position, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_linkedins (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_mobiles (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<telprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_names (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    first_name frozen<textprop>,
    middle_name frozen<textprop>,
    last_name frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_photos (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    info frozen<photoprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_skills (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    position TINYINT,
    id UUID,
    base_id UUID,
    skill_id frozen<uuidprop>,
    skill_level frozen<tinyintprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language, position, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_summaries (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE templates (
    id BLOB,
    registration_id UUID,
    applicable_data frozen<LIST<ASCII>>,
    experience_options frozen<expopts>,
    education_options frozen<eduopts>,
    skill_options frozen<skillopts>,
    PRIMARY KEY (id)
);

CREATE TABLE template_registrations (
    id UUID,
    owner_id UUID,
    name TEXT,
    description TEXT,
    language ASCII,
    applicable_jobs frozen<SET<UUID>>,
    created_at TIMESTAMP,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

-- views

CREATE MATERIALIZED VIEW templates_by_registration AS
    SELECT * FROM templates WHERE registration_id IS NOT NULL AND id IS NOT NULL
    PRIMARY KEY (registration_id, id)
    WITH CLUSTERING ORDER BY (id DESC);
