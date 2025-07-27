-- Postgresql Db Name : lighthousedb

-- Comments Table
CREATE TABLE comments (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    photo_id UUID NOT NULL,
    text VARCHAR(250) NOT NULL,
    rating INT NOT NULL,
    created_at TIMESTAMP DEFAULT now()
);

-- Photos Table
CREATE TABLE photos (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    lighthouse_id UUID NOT NULL,
    filename VARCHAR(50) NOT NULL,
    upload_date TIMESTAMP NOT NULL,
    lens TEXT NOT NULL,
    resolution TEXT NOT NULL,
    camera_model TEXT NOT NULL,
    taken_at TIMESTAMP NOT NULL
);

-- Countries Table (Lookup Table)
CREATE TABLE countries (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL
);

-- Sample Countries
INSERT INTO countries (name)
VALUES 
    ('Argentina'),
    ('Australia'),
    ('Bangladesh'),
    ('Belgium'),
    ('Brazil'),
    ('Canada'),
    ('Chile'),
    ('China'),
    ('Colombia'),
    ('Croatia'),
    ('Cuba'),
    ('Denmark'),
    ('Dominican Republic'),
    ('Egypt'),
    ('Estonia'),
    ('Finland'),
    ('France'),
    ('Germany'),
    ('Greece'),
    ('Greenland'),
    ('Iceland'),
    ('India'),
    ('Indonesia'),
    ('Iran'),
    ('Ireland'),
    ('Israel'),
    ('Italy'),
    ('Japan'),
    ('Latvia'),
    ('Lithuania'),
    ('Malaysia'),
    ('Mexico'),
    ('Morocco'),
    ('Netherlands'),
    ('New Zealand'),
    ('Nigeria'),
    ('Norway'),
    ('Pakistan'),
    ('Peru'),
    ('Philippines'),
    ('Poland'),
    ('Portugal'),
    ('Russia'),
    ('South Africa'),
    ('South Korea'),
    ('Spain'),
    ('Sri Lanka'),
    ('Sweden'),
    ('Turkey'),
    ('United Kingdom'),
    ('United States');

-- Lighthouses Table
CREATE TABLE lighthouses (
    id UUID PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    country_id INTEGER NOT NULL REFERENCES countries(id),
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL
);

-- Users Table
CREATE TABLE users(
    id UUID PRIMARY KEY,
    external_id TEXT NOT NULL, -- KeyCloack gibi bir Identity Provider için benzersiz kullanıcı ID bilgisini tutar
    full_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    joined_at TIMESTAMP DEFAULT now()
);
CREATE UNIQUE INDEX idx_users_external_id ON users (external_id); -- External Id için tekillik garantisi
CREATE UNIQUE INDEX idx_users_email ON users (email); -- email için tekillik garantisi
CREATE UNIQUE INDEX ux_lighthouses_identity ON lighthouses (name, country_id, latitude, longitude); -- Denizfeneri verisi için tekillik garantisi
