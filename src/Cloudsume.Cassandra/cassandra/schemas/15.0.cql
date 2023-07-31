DROP MATERIALIZED VIEW campuses_by_location;
DROP MATERIALIZED VIEW degrees_by_level;

DROP TABLE campuses;
DROP TABLE degrees;
DROP TABLE institutes;

ALTER TABLE resume_links ADD censorships FROZEN<SET<ASCII>>;
