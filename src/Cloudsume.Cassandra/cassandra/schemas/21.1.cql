-- tables

CREATE TABLE template_packs (
    id UUID,
    owner UUID,
    name TEXT,
    prices MAP<ASCII, DECIMAL>,
    created TIMESTAMP,
    updated TIMESTAMP,
    PRIMARY KEY (id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE template_pack_members (
    pack UUID,
    template UUID,
    PRIMARY KEY (pack, template)
);

CREATE TABLE template_pack_licenses (
    pack UUID,
    id BLOB,
    user UUID,
    stripe_payment_intent TEXT,
    status TINYINT,
    updated TIMESTAMP,
    PRIMARY KEY (pack, id)
) WITH CLUSTERING ORDER BY (id DESC) AND compaction = {'class': 'LeveledCompactionStrategy'};

-- views

CREATE MATERIALIZED VIEW template_packs_by_owner AS
    SELECT * FROM template_packs WHERE id IS NOT NULL AND owner IS NOT NULL
    PRIMARY KEY (owner, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE MATERIALIZED VIEW template_pack_members_by_template AS
    SELECT * FROM template_pack_members WHERE pack IS NOT NULL AND template IS NOT NULL
    PRIMARY KEY (template, pack);

CREATE MATERIALIZED VIEW template_pack_licenses_by_user AS
    SELECT * FROM template_pack_licenses WHERE pack IS NOT NULL AND id IS NOT NULL AND user IS NOT NULL
    PRIMARY KEY (user, pack, id)
    WITH compaction = {'class': 'LeveledCompactionStrategy'};
