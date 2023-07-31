-- types

CREATE TYPE resume_address (
    region FROZEN<asciiprop>,
    street FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_education (
    start FROZEN<monthprop>,
    end FROZEN<monthprop>,
    institute FROZEN<textprop>,
    region FROZEN<asciiprop>,
    degree FROZEN<textprop>,
    grade FROZEN<textprop>,
    description FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_email (
    address FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_experience (
    start FROZEN<monthprop>,
    end FROZEN<monthprop>,
    title FROZEN<textprop>,
    company FROZEN<textprop>,
    region FROZEN<asciiprop>,
    street FROZEN<textprop>,
    description FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_github (
    username FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_headline (
    headline FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_language (
    language FROZEN<asciiprop>,
    comment FROZEN<textprop>,
    proficiency_flags TINYINT,
    ilr TINYINT,
    toefl TINYINT,
    toeic SMALLINT,
    ielts FROZEN<ielts>,
    updated TIMESTAMP
);

CREATE TYPE resume_linkedin (
    username FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_mobile (
    number FROZEN<telprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_name (
    first FROZEN<textprop>,
    middle FROZEN<textprop>,
    last FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_photo (
    info FROZEN<photoprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_skill (
    skill FROZEN<textprop>,
    level FROZEN<tinyintprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_summary (
    summary FROZEN<textprop>,
    updated TIMESTAMP
);

CREATE TYPE resume_website (
    url FROZEN<textprop>,
    updated TIMESTAMP
);

-- tables

ALTER TABLE workspaces ADD preview_job UUID;

ALTER TABLE resume_addresses ADD data FROZEN<resume_address>;
ALTER TABLE resume_educations ADD data FROZEN<resume_education>;
ALTER TABLE resume_emails ADD data FROZEN<resume_email>;
ALTER TABLE resume_experiences ADD data FROZEN<resume_experience>;
ALTER TABLE resume_githubs ADD data FROZEN<resume_github>;
ALTER TABLE resume_headlines ADD data FROZEN<resume_headline>;
ALTER TABLE resume_languages ADD data FROZEN<resume_language>;
ALTER TABLE resume_linkedins ADD data FROZEN<resume_linkedin>;
ALTER TABLE resume_mobiles ADD data FROZEN<resume_mobile>;
ALTER TABLE resume_names ADD data FROZEN<resume_name>;
ALTER TABLE resume_photos ADD data FROZEN<resume_photo>;
ALTER TABLE resume_skills ADD data FROZEN<resume_skill>;
ALTER TABLE resume_summaries ADD data FROZEN<resume_summary>;
ALTER TABLE resume_websites ADD data FROZEN<resume_website>;

CREATE TABLE resume_sample_data (
    owner UUID,
    job UUID,
    culture ASCII,
    address FROZEN<resume_address>,
    email FROZEN<resume_email>,
    github FROZEN<resume_github>,
    headline FROZEN<resume_headline>,
    linkedin FROZEN<resume_linkedin>,
    mobile FROZEN<resume_mobile>,
    name FROZEN<resume_name>,
    photo FROZEN<resume_photo>,
    summary FROZEN<resume_summary>,
    website FROZEN<resume_website>,
    PRIMARY KEY (owner, job, culture)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_educations (
    owner UUID,
    job UUID,
    culture ASCII,
    id UUID,
    position TINYINT,
    parent UUID,
    data FROZEN<resume_education>,
    PRIMARY KEY (owner, job, culture, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_experiences (
    owner UUID,
    job UUID,
    culture ASCII,
    id UUID,
    position TINYINT,
    parent UUID,
    data FROZEN<resume_experience>,
    PRIMARY KEY (owner, job, culture, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_languages (
    owner UUID,
    job UUID,
    culture ASCII,
    id UUID,
    position TINYINT,
    parent UUID,
    data FROZEN<resume_language>,
    PRIMARY KEY (owner, job, culture, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

CREATE TABLE resume_sample_skills (
    owner UUID,
    job UUID,
    culture ASCII,
    id UUID,
    position TINYINT,
    parent UUID,
    data FROZEN<resume_skill>,
    PRIMARY KEY (owner, job, culture, id)
) WITH compaction = {'class': 'LeveledCompactionStrategy'};

-- Software Engineer

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0, value: 'https://example.com' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    ccdadf00-9457-4087-9b43-0afbed1f274f,
    0,
    {
        start: { flags: 0, value: { year: 2011, month: 1 } },
        end: { flags: 0, value: { year: 2012, month: 12 } },
        institute: { flags: 0 },
        region: { flags: 0 },
        degree: { flags: 0 },
        grade: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    a09a2984-486f-454b-8b87-290e04da7fc7,
    0,
    {
        start: { flags: 0, value: { year: 2021, month: 1 } },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    520a4fa2-71b2-495e-ae8b-b7aadf0da6c2,
    1,
    {
        start: { flags: 0, value: { year: 2016, month: 3 } },
        end: { flags: 0, value: { year: 2020, month: 12 } },
        title: { flags: 0 },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    62b96f84-b11b-4d9d-b839-e7b7b50cf5e9,
    0,
    {
        skill: { flags: 0, value: 'C#' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    ac966597-982b-4386-9123-518bd46e5920,
    1,
    {
        skill: { flags: 0, value: 'Clojure' },
        level: { flags: 0, value: 3 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    419ac25d-4f72-48ec-a036-00c147d2cbab,
    2,
    {
        skill: { flags: 0, value: 'COBOL' },
        level: { flags: 0, value: 3 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    '',
    7aff0dc6-9ace-44d6-8e53-4c48607d1f46,
    3,
    {
        skill: { flags: 0, value: 'Dart' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0, value: 'Software Engineer' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en',
    01f9c051-3bd7-4a99-a612-7dc4820d9d5d,
    0,
    ccdadf00-9457-4087-9b43-0afbed1f274f,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0 },
        region: { flags: 0 },
        degree: { flags: 0 },
        grade: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

- Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en',
    9b365c73-4dbc-4323-b4fd-22f7b4cd6de5,
    0,
    a09a2984-486f-454b-8b87-290e04da7fc7,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0, value: 'Principal Software Engineer' },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

- Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
- Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en',
    dc994f4a-18e4-4450-99d7-f0c76c9f663c,
    1,
    520a4fa2-71b2-495e-ae8b-b7aadf0da6c2,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0, value: 'Senior Software Engineer' },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Don Mueang' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'khanakorn@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'Khanakorn' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'Pianchana' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-TH',
    3901afec-a721-4136-8004-4b49b6f4fd61,
    0,
    01f9c051-3bd7-4a99-a612-7dc4820d9d5d,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0, value: 'Ramkhamhaeng University' },
        region: { flags: 0, value: 'TH-10' },
        degree: { flags: 0, value: 'Master of Science (Computer Science)' },
        grade: { flags: 0, value: 'GPA 3.9' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-TH',
    a557caeb-addc-4f91-a138-17d36507fe23,
    0,
    9b365c73-4dbc-4323-b4fd-22f7b4cd6de5,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Kidde Product' },
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Bang Na' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-TH',
    7d1da28c-3f94-46c7-b0d6-82eb166487a8,
    1,
    dc994f4a-18e4-4450-99d7-f0c76c9f663c,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'PhumJaiThaiTham' },
        region: { flags: 1 },
        street: { flags: 0, value: 'Phi Phi Islands' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-TH',
    8491851c-b1f3-4117-bbd6-bdca2cb2fe68,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'Lorem ipsum dolor sit amet.' },
        proficiency_flags: 0,
        toeic: 950,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-TH',
    00af4682-3a1b-434d-8aa3-f6c1f3166513,
    1,
    {
        language: { flags: 0, value: 'th' },
        comment: { flags: 1 },
        proficiency_flags: 0,
        ilr: 5,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-US',
    {
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'jdoe@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'US', number: '2025550304' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'John' },
        middle: { flags: 0, value: 'Michael' },
        last: { flags: 0, value: 'Doe' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-US',
    72afddf2-9522-40e6-84db-51d82c9aa9f4,
    0,
    01f9c051-3bd7-4a99-a612-7dc4820d9d5d,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0, value: 'University of California, Berkeley' },
        region: { flags: 0, value: 'US-CA' },
        degree: { flags: 0, value: 'Master of Science in Computer Science' },
        grade: { flags: 1 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-US',
    b95c8bfe-9d8d-407e-b949-09b437917f74,
    0,
    9b365c73-4dbc-4323-b4fd-22f7b4cd6de5,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Hooli' },
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-US',
    1ace42c4-57a0-44e9-8c58-36c712024f1a,
    1,
    dc994f4a-18e4-4450-99d7-f0c76c9f663c,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Stark Industries' },
        region: { flags: 1 },
        street: { flags: 0, value: 'Remote' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'en-US',
    6a9e85d6-a9f4-4d63-89c4-9ecb2d4b87a2,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'Lorem ipsum dolor sit amet.' },
        proficiency_flags: 0,
        toefl: 110,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0, value: 'วิศวกรซอฟต์แวร์' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0, value: 'อีแต๋น ซิมโฟนี่เมาท์ ควิกไฮเทค พาร์ตเนอร์ออยล์ยากูซ่า แอปเปิ้ลฟาสต์ฟู้ดออดิทอเรียมวิกเซลส์แมน เฮียดีกรีอิเหนา' },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'ดอนเมือง' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'pantip@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'พันธุ์ทิพย์' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'สุนทร' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th-TH',
    93a31c2d-23d2-4753-b96b-2b279d20e899,
    0,
    {
        start: { flags: 0, value: { year: 2011, month: 1 } },
        end: { flags: 0, value: { year: 2012, month: 12 } },
        institute: { flags: 0, value: 'มหาวิทยาลัยรามคำแหง' },
        region: { flags: 0, value: 'TH-10' },
        degree: { flags: 0, value: 'วิทยาศาสตรมหาบัณฑิต (วิทยาการคอมพิวเตอร์)' },
        grade: { flags: 0, value: 'เกรด 3.9' },
        description: { flags: 0, value: 'โค้ชมาร์คโกเต็กซ์ไฟแนนซ์ สเตอริโอ พาวเวอร์ลิมูซีน มวลชนสไลด์ คาราโอเกะช็อตป๊อปเฟิร์ม สี่แยกสงบสุข ยากูซ่าโมหจริตสไลด์ซันตาคลอส

- เลดี้ปอดแหกเปเปอร์เอฟเฟ็กต์ บัส มินท์วอร์รูม ไดเอ็ตทัวร์นาเมนท์ป๊อกรูบิก โลโก้ปัจเจกชนเซ่นไหว้ฟินิกซ์' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th-TH',
    79e3f26d-48c8-47a7-b6be-498f9a93514d,
    0,
    {
        start: { flags: 0, value: { year: 2021, month: 1 } },
        end: { flags: 0 },
        title: { flags: 0, value: 'วิศวกรซอฟต์แวร์' },
        company: { flags: 0, value: 'คิดดี โปรดัก' },
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'บางนา' },
        description: { flags: 0, value: 'โค้ชมาร์คโกเต็กซ์ไฟแนนซ์ สเตอริโอ พาวเวอร์ลิมูซีน มวลชนสไลด์ คาราโอเกะช็อตป๊อปเฟิร์ม สี่แยกสงบสุข ยากูซ่าโมหจริตสไลด์ซันตาคลอส

- เลดี้ปอดแหกเปเปอร์เอฟเฟ็กต์ บัส มินท์วอร์รูม ไดเอ็ตทัวร์นาเมนท์ป๊อกรูบิก โลโก้ปัจเจกชนเซ่นไหว้ฟินิกซ์
- ภูมิทัศน์อุปนายกโกะโอเคเมจิก ฮัมกิฟท์ ตะหงิดวาซาบิ เทวาธิราช' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th-TH',
    0ebadc86-db6b-4f15-b66e-c4742cfed436,
    1,
    {
        start: { flags: 0, value: { year: 2016, month: 3 } },
        end: { flags: 0, value: { year: 2020, month: 12 } },
        title: { flags: 0, value: 'วิศวกรซอฟต์แวร์' },
        company: { flags: 0, value: 'ภูมิใจไทยทำ' },
        region: { flags: 1 },
        street: { flags: 0, value: 'เกาะพีพี' },
        description: { flags: 0, value: 'ภูมิทัศน์อุปนายกโกะโอเคเมจิก ฮัมกิฟท์ ตะหงิดวาซาบิ เทวาธิราช' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    66df85d8-0b5c-4a68-bc59-1da9e506a544,
    'th-TH',
    35c73ddf-b8a0-407d-b9e7-4469ee5da7d7,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'วาริชศาสตร์แพทเทิร์น ฮิบรู' },
        proficiency_flags: 0,
        toeic: 950,
        updated: currentTimestamp()
    }
);

-- DevOps Engineer

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0, value: 'https://example.com' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    564e0a75-b877-420f-b3a2-857023386727,
    0,
    {
        start: { flags: 0, value: { year: 2011, month: 1 } },
        end: { flags: 0, value: { year: 2012, month: 12 } },
        institute: { flags: 0 },
        region: { flags: 0 },
        degree: { flags: 0 },
        grade: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    4360d3da-e1b6-4d27-a1cb-4eee265e7c89,
    0,
    {
        start: { flags: 0, value: { year: 2021, month: 1 } },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    a514d109-f685-4775-97b3-3e295bcb4f7c,
    1,
    {
        start: { flags: 0, value: { year: 2016, month: 3 } },
        end: { flags: 0, value: { year: 2020, month: 12 } },
        title: { flags: 0 },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    f9a94aa7-42f7-4561-a7ff-ede9c2680a65,
    0,
    {
        skill: { flags: 0, value: 'AWS' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    02941e6f-3eee-4102-9e15-710779fb86f6,
    1,
    {
        skill: { flags: 0, value: 'Azure' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    04596200-82cf-4305-8bf5-ca89c53ff516,
    2,
    {
        skill: { flags: 0, value: 'GCP' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    '',
    19aac8b0-8cd7-42cc-80ba-80b0151c2495,
    3,
    {
        skill: { flags: 0, value: 'Terraform' },
        level: { flags: 0, value: 3 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0, value: 'DevOps Engineer' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en',
    56fc0bd9-6f78-4e2d-8b3e-2fdd598fe816,
    0,
    564e0a75-b877-420f-b3a2-857023386727,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0 },
        region: { flags: 0 },
        degree: { flags: 0 },
        grade: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

- Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en',
    7c792c91-cf41-4b70-a184-5e27403949ab,
    0,
    4360d3da-e1b6-4d27-a1cb-4eee265e7c89,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0, value: 'Senior DevOps Engineer' },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

- Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
- Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en',
    504ef7f0-de7d-4996-8048-c569d8991a28,
    1,
    a514d109-f685-4775-97b3-3e295bcb4f7c,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0, value: 'DevOps Engineer' },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Don Mueang' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'khanakorn@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'Khanakorn' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'Pianchana' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-TH',
    57918f5a-04f4-48da-b5eb-370bda80580f,
    0,
    56fc0bd9-6f78-4e2d-8b3e-2fdd598fe816,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0, value: 'Ramkhamhaeng University' },
        region: { flags: 0, value: 'TH-10' },
        degree: { flags: 0, value: 'Master of Science (Computer Science)' },
        grade: { flags: 0, value: 'GPA 3.9' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-TH',
    7f569d4e-6aa7-4b4d-afcd-489b1cb706ed,
    0,
    7c792c91-cf41-4b70-a184-5e27403949ab,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Kidde Product' },
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Bang Na' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-TH',
    d2ff7c88-ad72-435b-b1f6-511e4349f95b,
    1,
    504ef7f0-de7d-4996-8048-c569d8991a28,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'PhumJaiThaiTham' },
        region: { flags: 1 },
        street: { flags: 0, value: 'Phi Phi Islands' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-TH',
    078bf65b-9d9b-4eb9-b322-7bd77d3d1080,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'Lorem ipsum dolor sit amet.' },
        proficiency_flags: 0,
        toeic: 950,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-TH',
    b989e0c2-78a1-4f55-a86d-4696b6ba7e57,
    1,
    {
        language: { flags: 0, value: 'th' },
        comment: { flags: 1 },
        proficiency_flags: 0,
        ilr: 5,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-US',
    {
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'jdoe@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'US', number: '2025550304' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'John' },
        middle: { flags: 0, value: 'Michael' },
        last: { flags: 0, value: 'Doe' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-US',
    ccf4456f-4aa0-4c2b-9486-a47dde85c723,
    0,
    56fc0bd9-6f78-4e2d-8b3e-2fdd598fe816,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0, value: 'University of California, Berkeley' },
        region: { flags: 0, value: 'US-CA' },
        degree: { flags: 0, value: 'Master of Science in Computer Science' },
        grade: { flags: 1 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-US',
    1aeb3370-7034-4374-88d0-9c507b6aea77,
    0,
    7c792c91-cf41-4b70-a184-5e27403949ab,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Hooli' },
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-US',
    356b773c-e274-45ce-ae6e-d853a3bd3499,
    1,
    504ef7f0-de7d-4996-8048-c569d8991a28,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Stark Industries' },
        region: { flags: 1 },
        street: { flags: 0, value: 'Remote' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'en-US',
    5c2cca93-017c-4df5-8247-dbbda543ca7d,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'Lorem ipsum dolor sit amet.' },
        proficiency_flags: 0,
        toefl: 110,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0, value: 'วิศวกรเดฟอ๊อปส์' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0, value: 'อีแต๋น ซิมโฟนี่เมาท์ ควิกไฮเทค พาร์ตเนอร์ออยล์ยากูซ่า แอปเปิ้ลฟาสต์ฟู้ดออดิทอเรียมวิกเซลส์แมน เฮียดีกรีอิเหนา' },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'ดอนเมือง' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'pantip@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'พันธุ์ทิพย์' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'สุนทร' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th-TH',
    544b296a-8ee9-4524-a304-1ee1bb3b706e,
    0,
    {
        start: { flags: 0, value: { year: 2011, month: 1 } },
        end: { flags: 0, value: { year: 2012, month: 12 } },
        institute: { flags: 0, value: 'มหาวิทยาลัยรามคำแหง' },
        region: { flags: 0, value: 'TH-10' },
        degree: { flags: 0, value: 'วิทยาศาสตรมหาบัณฑิต (วิทยาการคอมพิวเตอร์)' },
        grade: { flags: 0, value: 'เกรด 3.9' },
        description: { flags: 0, value: 'โค้ชมาร์คโกเต็กซ์ไฟแนนซ์ สเตอริโอ พาวเวอร์ลิมูซีน มวลชนสไลด์ คาราโอเกะช็อตป๊อปเฟิร์ม สี่แยกสงบสุข ยากูซ่าโมหจริตสไลด์ซันตาคลอส

- เลดี้ปอดแหกเปเปอร์เอฟเฟ็กต์ บัส มินท์วอร์รูม ไดเอ็ตทัวร์นาเมนท์ป๊อกรูบิก โลโก้ปัจเจกชนเซ่นไหว้ฟินิกซ์' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th-TH',
    cce8cf3a-85c3-4a47-b901-2b8570909dd3,
    0,
    {
        start: { flags: 0, value: { year: 2021, month: 1 } },
        end: { flags: 0 },
        title: { flags: 0, value: 'วิศวกรเดฟอ๊อปส์' },
        company: { flags: 0, value: 'คิดดี โปรดัก' },
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'บางนา' },
        description: { flags: 0, value: 'โค้ชมาร์คโกเต็กซ์ไฟแนนซ์ สเตอริโอ พาวเวอร์ลิมูซีน มวลชนสไลด์ คาราโอเกะช็อตป๊อปเฟิร์ม สี่แยกสงบสุข ยากูซ่าโมหจริตสไลด์ซันตาคลอส

- เลดี้ปอดแหกเปเปอร์เอฟเฟ็กต์ บัส มินท์วอร์รูม ไดเอ็ตทัวร์นาเมนท์ป๊อกรูบิก โลโก้ปัจเจกชนเซ่นไหว้ฟินิกซ์
- ภูมิทัศน์อุปนายกโกะโอเคเมจิก ฮัมกิฟท์ ตะหงิดวาซาบิ เทวาธิราช' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th-TH',
    52ecca53-5ac1-46e2-b73f-b1a81c1196ea,
    1,
    {
        start: { flags: 0, value: { year: 2016, month: 3 } },
        end: { flags: 0, value: { year: 2020, month: 12 } },
        title: { flags: 0, value: 'วิศวกรเดฟอ๊อปส์' },
        company: { flags: 0, value: 'ภูมิใจไทยทำ' },
        region: { flags: 1 },
        street: { flags: 0, value: 'เกาะพีพี' },
        description: { flags: 0, value: 'ภูมิทัศน์อุปนายกโกะโอเคเมจิก ฮัมกิฟท์ ตะหงิดวาซาบิ เทวาธิราช' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    1ec9fb29-4259-4608-9806-644797928454,
    'th-TH',
    888027ee-7156-490c-afdd-c70c7a0d7670,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'วาริชศาสตร์แพทเทิร์น ฮิบรู' },
        proficiency_flags: 0,
        toeic: 950,
        updated: currentTimestamp()
    }
);

-- Product Designer

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    '',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0, value: 'https://example.com' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    '',
    5d0f8cc8-7a70-44d6-b359-3edada9d0733,
    0,
    {
        start: { flags: 0, value: { year: 2011, month: 1 } },
        end: { flags: 0, value: { year: 2012, month: 12 } },
        institute: { flags: 0 },
        region: { flags: 0 },
        degree: { flags: 0 },
        grade: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    '',
    a9fda1ed-d64e-46cb-b3fd-17523cda325a,
    0,
    {
        start: { flags: 0, value: { year: 2021, month: 1 } },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    '',
    3f8b12cf-4cec-4bd3-aa1f-ba82529037fc,
    1,
    {
        start: { flags: 0, value: { year: 2016, month: 3 } },
        end: { flags: 0, value: { year: 2020, month: 12 } },
        title: { flags: 0 },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0, value: 'Product Designer' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    d98ef9cf-3bb2-41ed-87bb-bdc38d0de29e,
    0,
    5d0f8cc8-7a70-44d6-b359-3edada9d0733,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0 },
        region: { flags: 0 },
        degree: { flags: 0 },
        grade: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

- Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    4b59d7bf-0ee9-44ba-a09b-5923fbc85297,
    0,
    a9fda1ed-d64e-46cb-b3fd-17523cda325a,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0, value: 'Product Designer' },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

- Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
- Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    2a1913f2-6096-4b30-ae20-0415da82c63d,
    1,
    3f8b12cf-4cec-4bd3-aa1f-ba82529037fc,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0, value: 'UI Designer' },
        company: { flags: 0 },
        region: { flags: 0 },
        street: { flags: 0 },
        description: { flags: 0, value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    22831839-125d-4785-ad1e-745caa65d86b,
    0,
    {
        skill: { flags: 0, value: 'UX/UI Design' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    6fffb380-0fc4-477c-a12f-ada4843b3369,
    1,
    {
        skill: { flags: 0, value: 'Wireframing' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    734f5478-9705-4b5c-a8a3-38951a7697c1,
    2,
    {
        skill: { flags: 0, value: 'User Research' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en',
    baf7ef4c-ff61-49ed-9611-f14a3c70bfa0,
    3,
    {
        skill: { flags: 0, value: 'Graphic Design' },
        level: { flags: 0, value: 3 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Don Mueang' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'khanakorn@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'khanakorn' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'Khanakorn' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'Pianchana' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-TH',
    9f725789-107a-4a23-9969-c569758d82df,
    0,
    d98ef9cf-3bb2-41ed-87bb-bdc38d0de29e,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0, value: 'Ramkhamhaeng University' },
        region: { flags: 0, value: 'TH-10' },
        degree: { flags: 0, value: 'Master of Science (Computer Science)' },
        grade: { flags: 0, value: 'GPA 3.9' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-TH',
    71e28c62-2fb7-4491-bb04-44dc646ba163,
    0,
    4b59d7bf-0ee9-44ba-a09b-5923fbc85297,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Kidde Product' },
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'Bang Na' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-TH',
    817d0cab-33ce-4f15-803a-d043263eb5b1,
    1,
    2a1913f2-6096-4b30-ae20-0415da82c63d,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'PhumJaiThaiTham' },
        region: { flags: 1 },
        street: { flags: 0, value: 'Phi Phi Islands' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-TH',
    7a97a966-86b9-4af4-b872-91487162e889,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'Lorem ipsum dolor sit amet.' },
        proficiency_flags: 0,
        toeic: 950,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-TH',
    b8f4b829-5fb9-4bd4-a5fb-70820023b248,
    1,
    {
        language: { flags: 0, value: 'th' },
        comment: { flags: 1 },
        proficiency_flags: 0,
        ilr: 5,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-US',
    {
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'jdoe@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'jdoe' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'US', number: '2025550304' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'John' },
        middle: { flags: 0, value: 'Michael' },
        last: { flags: 0, value: 'Doe' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-US',
    128e1aa2-1f29-4f4c-bfb4-468e0d2e1f60,
    0,
    d98ef9cf-3bb2-41ed-87bb-bdc38d0de29e,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        institute: { flags: 0, value: 'University of California, Berkeley' },
        region: { flags: 0, value: 'US-CA' },
        degree: { flags: 0, value: 'Master of Science in Computer Science' },
        grade: { flags: 1 },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-US',
    1723482c-d585-4622-9282-2473ffb12721,
    0,
    4b59d7bf-0ee9-44ba-a09b-5923fbc85297,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Hooli' },
        region: { flags: 0, value: 'US-CA' },
        street: { flags: 0, value: 'San Jose' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, parent, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-US',
    b21e8c81-65b1-4eb0-a1ba-ac3ca17281fd,
    1,
    2a1913f2-6096-4b30-ae20-0415da82c63d,
    {
        start: { flags: 0 },
        end: { flags: 0 },
        title: { flags: 0 },
        company: { flags: 0, value: 'Stark Industries' },
        region: { flags: 1 },
        street: { flags: 0, value: 'Remote' },
        description: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'en-US',
    8a88dd2f-b258-4e41-8cae-f2d6fcfbe1e2,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'Lorem ipsum dolor sit amet.' },
        proficiency_flags: 0,
        toefl: 110,
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th',
    {
        region: { flags: 0 },
        street: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0, value: 'นักออกแบบผลิตภัณฑ์' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0 },
        middle: { flags: 0 },
        last: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0, value: 'อีแต๋น ซิมโฟนี่เมาท์ ควิกไฮเทค พาร์ตเนอร์ออยล์ยากูซ่า แอปเปิ้ลฟาสต์ฟู้ดออดิทอเรียมวิกเซลส์แมน เฮียดีกรีอิเหนา' },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th',
    cb020d0c-0152-49a9-8ebe-349c8cabd1d8,
    0,
    {
        skill: { flags: 0, value: 'การออกแบบ UX/UI' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th',
    5575eb89-096e-4af1-a1f8-3e764d8f67e2,
    1,
    {
        skill: { flags: 0, value: 'การออกแบบไวร์เฟรม' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th',
    8b1f5643-4fdd-47ce-ae8e-faf76dd8c3b3,
    2,
    {
        skill: { flags: 0, value: 'การทำวิจัยผู้ใช้' },
        level: { flags: 0, value: 7 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_skills (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th',
    2ec0dd2f-7682-4cf0-a49c-ad819d4395f5,
    3,
    {
        skill: { flags: 0, value: 'การออกแบบกราฟิก' },
        level: { flags: 0, value: 3 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_data (owner, job, culture, address, email, github, headline, linkedin, mobile, name, photo, summary, website) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th-TH',
    {
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'ดอนเมือง' },
        updated: currentTimestamp()
    },
    {
        address: { flags: 0, value: 'pantip@example.com' },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    },
    {
        headline: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        username: { flags: 0, value: 'pantip' },
        updated: currentTimestamp()
    },
    {
        number: { flags: 0, value: { country: 'TH', number: '0891234567' } },
        updated: currentTimestamp()
    },
    {
        first: { flags: 0, value: 'พันธุ์ทิพย์' },
        middle: { flags: 1 },
        last: { flags: 0, value: 'สุนทร' },
        updated: currentTimestamp()
    },
    {
        info: { flags: 0, value: { format: 0, size: 1509 } },
        updated: currentTimestamp()
    },
    {
        summary: { flags: 0 },
        updated: currentTimestamp()
    },
    {
        url: { flags: 0 },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_educations (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th-TH',
    727b434f-7912-410d-975a-8edf76c68227,
    0,
    {
        start: { flags: 0, value: { year: 2011, month: 1 } },
        end: { flags: 0, value: { year: 2012, month: 12 } },
        institute: { flags: 0, value: 'มหาวิทยาลัยรามคำแหง' },
        region: { flags: 0, value: 'TH-10' },
        degree: { flags: 0, value: 'วิทยาศาสตรมหาบัณฑิต (วิทยาการคอมพิวเตอร์)' },
        grade: { flags: 0, value: 'เกรด 3.9' },
        description: { flags: 0, value: 'โค้ชมาร์คโกเต็กซ์ไฟแนนซ์ สเตอริโอ พาวเวอร์ลิมูซีน มวลชนสไลด์ คาราโอเกะช็อตป๊อปเฟิร์ม สี่แยกสงบสุข ยากูซ่าโมหจริตสไลด์ซันตาคลอส

- เลดี้ปอดแหกเปเปอร์เอฟเฟ็กต์ บัส มินท์วอร์รูม ไดเอ็ตทัวร์นาเมนท์ป๊อกรูบิก โลโก้ปัจเจกชนเซ่นไหว้ฟินิกซ์' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th-TH',
    7cda3695-57b0-4120-ad8b-7fb02b68fb46,
    0,
    {
        start: { flags: 0, value: { year: 2021, month: 1 } },
        end: { flags: 0 },
        title: { flags: 0, value: 'นักออกแบบผลิตภัณฑ์' },
        company: { flags: 0, value: 'คิดดี โปรดัก' },
        region: { flags: 0, value: 'TH-10' },
        street: { flags: 0, value: 'บางนา' },
        description: { flags: 0, value: 'โค้ชมาร์คโกเต็กซ์ไฟแนนซ์ สเตอริโอ พาวเวอร์ลิมูซีน มวลชนสไลด์ คาราโอเกะช็อตป๊อปเฟิร์ม สี่แยกสงบสุข ยากูซ่าโมหจริตสไลด์ซันตาคลอส

- เลดี้ปอดแหกเปเปอร์เอฟเฟ็กต์ บัส มินท์วอร์รูม ไดเอ็ตทัวร์นาเมนท์ป๊อกรูบิก โลโก้ปัจเจกชนเซ่นไหว้ฟินิกซ์
- ภูมิทัศน์อุปนายกโกะโอเคเมจิก ฮัมกิฟท์ ตะหงิดวาซาบิ เทวาธิราช' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_experiences (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th-TH',
    2de70aed-c799-4328-a21c-d103aa32b845,
    1,
    {
        start: { flags: 0, value: { year: 2016, month: 3 } },
        end: { flags: 0, value: { year: 2020, month: 12 } },
        title: { flags: 0, value: 'นักออกแบบ UX/UI' },
        company: { flags: 0, value: 'ภูมิใจไทยทำ' },
        region: { flags: 1 },
        street: { flags: 0, value: 'เกาะพีพี' },
        description: { flags: 0, value: 'ภูมิทัศน์อุปนายกโกะโอเคเมจิก ฮัมกิฟท์ ตะหงิดวาซาบิ เทวาธิราช' },
        updated: currentTimestamp()
    }
);

INSERT INTO resume_sample_languages (owner, job, culture, id, position, data) VALUES (
    00000000-0000-0000-0000-000000000000,
    c2f28dc9-23a5-4dc9-b5cc-09e9b919f262,
    'th-TH',
    86acd04b-f024-45c4-aab1-1694aa1b9110,
    0,
    {
        language: { flags: 0, value: 'en' },
        comment: { flags: 0, value: 'วาริชศาสตร์แพทเทิร์น ฮิบรู' },
        proficiency_flags: 0,
        toeic: 950,
        updated: currentTimestamp()
    }
);
