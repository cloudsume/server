DROP MATERIALIZED VIEW skills_by_type;

DROP TABLE skill_categories;
DROP TABLE skills;

CREATE TABLE resume_sample_addresses (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_address>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_emails (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_email>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_githubs (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_github>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_headlines (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_headline>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_linkedins (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_linkedin>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_mobiles (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_mobile>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_names (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_name>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_photos (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_photo>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_summaries (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_summary>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_websites (
    owner UUID,
    job UUID,
    culture ASCII,
    data FROZEN<resume_website>,
    parent_job UUID,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

ALTER TABLE guest_sessions ADD (transferred_to UUID, transferred_at TIMESTAMP);
ALTER TABLE resume_sample_educations ADD parent_job UUID;
ALTER TABLE resume_sample_experiences ADD parent_job UUID;
ALTER TABLE resume_sample_languages ADD parent_job UUID;
ALTER TABLE resume_sample_skills ADD parent_job UUID;

-- Default job

INSERT INTO resume_sample_addresses (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Don Mueang' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_addresses (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_addresses (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'ดอนเมือง' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_emails (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        address: { flags: 0, value: 'khanakorn@example.com' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_emails (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        address: { flags: 0, value: 'jdoe@example.com' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_emails (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        address: { flags: 0, value: 'pantip@example.com' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_githubs (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_githubs (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_githubs (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_linkedins (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_linkedins (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_linkedins (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_mobiles (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_mobiles (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        number: { flags: 0, value: { country: 'US', number: '2025550304' } },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_mobiles (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_names (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        first: { flags: 0, value: 'Khanakorn' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'Pianchana' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_names (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        first: { flags: 0, value: 'John' },
        middle: { flags: 0, value: 'Michael' },
        last: { flags: 0, value: 'Doe' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_names (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        first: { flags: 0, value: 'พันธุ์ทิพย์' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'สุนทร' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_photos (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-TH',
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_photos (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en-US',
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_photos (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th-TH',
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_summaries (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'en',
    {
        summary: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_summaries (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    'th',
    {
        summary: { flags: 0, value: 'อีแต๋น ซิมโฟนี่เมาท์ ควิกไฮเทค พาร์ตเนอร์ออยล์ยากูซ่า แอปเปิ้ลฟาสต์ฟู้ดออดิทอเรียมวิกเซลส์แมน เฮียดีกรีอิเหนา' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_websites (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    00000000-0000-0000-0000-000000000000,
    '',
    {
        url: { flags: 0, value: 'https://example.com' },
        updated: currentTimestamp()
    }
);

-- Product Designer

INSERT INTO resume_sample_headlines (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    {
        headline: { flags: 0, value: 'Product Designer' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_headlines (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th',
    {
        headline: { flags: 0, value: 'นักออกแบบผลิตภัณฑ์' },
        updated: currentTimestamp()
    }
);

-- DevOps Engineer

INSERT INTO resume_sample_headlines (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en',
    {
        headline: { flags: 0, value: 'DevOps Engineer' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_headlines (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th',
    {
        headline: { flags: 0, value: 'วิศวกรเดฟอ๊อปส์' },
        updated: currentTimestamp()
    }
);

-- Software Engineer

INSERT INTO resume_sample_headlines (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en',
    {
        headline: { flags: 0, value: 'Software Engineer' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_headlines (owner, job, culture, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th',
    {
        headline: { flags: 0, value: 'วิศวกรซอฟต์แวร์' },
        updated: currentTimestamp()
    }
);
