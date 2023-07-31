CREATE TABLE feedbacks (
    id BLOB,
    score TINYINT,
    detail TEXT,
    contact TEXT,
    user UUID,
    ip INET,
    PRIMARY KEY (id)
);

CREATE MATERIALIZED VIEW feedbacks_by_score AS
    SELECT * FROM feedbacks WHERE id IS NOT NULL AND score IS NOT NULL
    PRIMARY KEY (score, id);
