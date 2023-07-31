ALTER TABLE resumes ADD recruitment_consent DATE;

CREATE TABLE recruitment_revokes (
    date DATE,
    user UUID,
    resume UUID,
    PRIMARY KEY (date, user, resume)
);

CREATE MATERIALIZED VIEW resumes_by_recruitment_consent AS
    SELECT * FROM resumes WHERE user_id IS NOT NULL AND id IS NOT NULL AND recruitment_consent IS NOT NULL
    PRIMARY KEY (recruitment_consent, user_id, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};
