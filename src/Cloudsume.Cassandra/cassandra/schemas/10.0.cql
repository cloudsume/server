CREATE TABLE resume_websites (
    user_id UUID,
    resume_id UUID,
    language ASCII,
    value frozen<textprop>,
    updated_at TIMESTAMP,
    PRIMARY KEY (user_id, resume_id, language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE configurations (
    name ASCII,
    value BLOB,
    PRIMARY KEY (name)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};
