-- types and tables

CREATE TYPE resume_certificate (
    name FROZEN<textprop>,
    obtained FROZEN<monthprop>,
    updated TIMESTAMP
);

CREATE TABLE resume_certificates (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    position TINYINT,
    id UUID,
    base_id UUID,
    data FROZEN<resume_certificate>,
    PRIMARY KEY (user_id, resume_id, language, position, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_certificates (
    owner UUID,
    job UUID,
    culture ASCII,
    id UUID,
    position TINYINT,
    parent UUID,
    data FROZEN<resume_certificate>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

-- views

DROP MATERIALIZED VIEW feedbacks_by_score;

CREATE MATERIALIZED VIEW feedbacks_by_score AS
    SELECT * FROM feedbacks WHERE id IS NOT NULL AND score IS NOT NULL
    PRIMARY KEY (score, id)
    WITH CLUSTERING ORDER BY (id DESC);

-- sample data

INSERT INTO resume_sample_certificates (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    '',
    7214b651-5d2d-427c-9815-3ee8c7375bc5,
    0,
    {
        name: { flags: 0, value: 'CompTIA A+' },
        obtained: { flags: 0, value: { year: 2022, month: 3 } },
        updated: currentTimestamp()
    }
);
