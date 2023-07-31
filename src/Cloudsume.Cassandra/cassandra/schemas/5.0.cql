CREATE TABLE template_licenses (
    registration_id UUID,
    id BLOB,
    user_id UUID,
    stripe_payment_intent TEXT,
    status TINYINT,
    updated_at TIMESTAMP,
    PRIMARY KEY (registration_id, id)
) WITH CLUSTERING ORDER BY (id DESC) AND compaction = {'class': 'LeveledCompactionStrategy'};

CREATE MATERIALIZED VIEW template_licenses_by_user_id AS
    SELECT * FROM template_licenses WHERE registration_id IS NOT NULL AND id IS NOT NULL AND user_id IS NOT NULL
    PRIMARY KEY (user_id, registration_id, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};
