ALTER TABLE template_registrations ADD prices MAP<ASCII, DECIMAL>;

CREATE TABLE payment_receiving_methods (
    user UUID,
    id UUID,
    stripe_account TEXT,
    created_at TIMESTAMP,
    PRIMARY KEY (user, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};
