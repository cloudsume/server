CREATE TABLE user_activities (
    user UUID,
    id BLOB,
    type UUID,
    data BLOB,
    ip INET,
    user_agent TEXT,
    PRIMARY KEY (user, id)
) WITH CLUSTERING ORDER BY (id DESC);

CREATE TABLE template_cancel_purchase_feedbacks (
    template UUID,
    id BLOB,
    user UUID,
    stripe_payment_intent TEXT,
    reason TINYINT,
    ip INET,
    user_agent TEXT,
    PRIMARY KEY (template, id)
) WITH CLUSTERING ORDER BY (id DESC);
