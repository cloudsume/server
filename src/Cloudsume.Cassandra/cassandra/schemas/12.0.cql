ALTER TABLE resume_addresses DROP (region, street, updated_at);
ALTER TABLE resume_educations DROP (start, end, institute, region, campus_id, degree, degree_id, grade, description, updated_at);
ALTER TABLE resume_emails DROP (value, updated_at);
ALTER TABLE resume_experiences DROP (start, end, title, company, region, street, description, updated_at);
ALTER TABLE resume_githubs DROP (value, updated_at);
ALTER TABLE resume_headlines DROP (value, updated_at);
ALTER TABLE resume_languages DROP (language_id, comment, proficiency_flags, ilr_scale, toefl_score, toeic_score, ielts_score, updated_at);
ALTER TABLE resume_linkedins DROP (value, updated_at);
ALTER TABLE resume_mobiles DROP (value, updated_at);
ALTER TABLE resume_names DROP (first_name, middle_name, last_name, updated_at);
ALTER TABLE resume_photos DROP (info, updated_at);
ALTER TABLE resume_skills DROP (skill_name, skill_id, skill_level, updated_at);
ALTER TABLE resume_summaries DROP (value, updated_at);
ALTER TABLE resume_websites DROP (value, updated_at);

CREATE TABLE guest_sessions (
    user_id UUID,
    issuer ASCII,
    key_id ASCII,
    requester INET,
    created_at TIMESTAMP,
    PRIMARY KEY (user_id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};
