ALTER TABLE template_registrations ADD stats_initialized BOOLEAN;
ALTER TABLE templates ADD stats_initialized BOOLEAN;

CREATE TABLE template_registration_stats (
    registration_id UUID,
    resume_count COUNTER,
    PRIMARY KEY (registration_id)
);

CREATE TABLE template_stats (
    template_id BLOB,
    resume_count COUNTER,
    PRIMARY KEY (template_id)
);
