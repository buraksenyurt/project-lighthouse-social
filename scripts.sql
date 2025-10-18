CREATE DATABASE keycloak;

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
    taken_at TIMESTAMP NOT NULL,
    is_primary BOOLEAN NOT NULL DEFAULT false
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

-- OpenAI GPT 4.1'e hazırlatılmış örnek veriler
-- 10 örnek kullanıcı
INSERT INTO users (id, external_id, full_name, email) VALUES
('00000000-0000-0000-0000-000000000001', 'ext1', 'Alice Smith', 'alice@example.com'),
('00000000-0000-0000-0000-000000000002', 'ext2', 'Bob Jones', 'bob@example.com'),
('00000000-0000-0000-0000-000000000003', 'ext3', 'Charlie Brown', 'charlie@example.com'),
('00000000-0000-0000-0000-000000000004', 'ext4', 'Diana Prince', 'diana@example.com'),
('00000000-0000-0000-0000-000000000005', 'ext5', 'Ethan Hunt', 'ethan@example.com'),
('00000000-0000-0000-0000-000000000006', 'ext6', 'Fiona Glenanne', 'fiona@example.com'),
('00000000-0000-0000-0000-000000000007', 'ext7', 'George Miller', 'george@example.com'),
('00000000-0000-0000-0000-000000000008', 'ext8', 'Hannah Lee', 'hannah@example.com'),
('00000000-0000-0000-0000-000000000009', 'ext9', 'Ian Curtis', 'ian@example.com'),
('00000000-0000-0000-0000-000000000010', 'ext10', 'Julia Child', 'julia@example.com');

-- 10 deniz feneri (ülke id'leri countries tablosundan alınmalı, örnek: 6=Canada, 17=Germany, 31=Spain, 35=Turkey, 47=United States)
INSERT INTO lighthouses (id, name, country_id, latitude, longitude) VALUES
('10000000-0000-0000-0000-000000000001', 'Portland Head Light', 47, 43.6231, -70.2084),
('10000000-0000-0000-0000-000000000002', 'Peggy''s Point Lighthouse', 6, 44.4942, -63.9156),
('10000000-0000-0000-0000-000000000003', 'Tower of Hercules', 31, 43.3853, -8.4065),
('10000000-0000-0000-0000-000000000004', 'Cape Hatteras Lighthouse', 47, 35.2510, -75.5281),
('10000000-0000-0000-0000-000000000005', 'Lindau Lighthouse', 17, 47.5461, 9.6867),
('10000000-0000-0000-0000-000000000006', 'St. Augustine Lighthouse', 47, 29.8852, -81.2883),
('10000000-0000-0000-0000-000000000007', 'Les Éclaireurs Lighthouse', 1, -54.8678, -68.1056),
('10000000-0000-0000-0000-000000000008', 'Byron Bay Lighthouse', 2, -28.6389, 153.6331),
('10000000-0000-0000-0000-000000000009', 'Fanad Head Lighthouse', 24, 55.3272, -7.6266),
('10000000-0000-0000-0000-000000000010', 'Chania Lighthouse', 19, 35.5181, 24.0183);

-- Her deniz feneri için en az bir fotoğraf, popüler 3 için fazladan
INSERT INTO photos (id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at, is_primary) VALUES
('20000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'portland1.jpg', now(), '24-70mm', '4000x3000', 'Canon 5D', now(), true),
('20000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'portland2.jpg', now(), '24-70mm', '4000x3000', 'Canon 5D', now(), false),
('20000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002', 'peggys1.jpg', now(), '18-55mm', '3000x2000', 'Nikon D750', now(), true),
('20000000-0000-0000-0000-000000000004', '00000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000003', 'hercules1.jpg', now(), '50mm', '6000x4000', 'Sony A7', now(), true),
('20000000-0000-0000-0000-000000000005', '00000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000004', 'hatteras1.jpg', now(), '24-70mm', '4000x3000', 'Canon 5D', now(), true),
('20000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000005', 'lindau1.jpg', now(), '18-55mm', '3000x2000', 'Nikon D750', now(), true),
('20000000-0000-0000-0000-000000000007', '00000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000006', 'augustine1.jpg', now(), '50mm', '6000x4000', 'Sony A7', now(), true),
('20000000-0000-0000-0000-000000000008', '00000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000007', 'eclaireurs1.jpg', now(), '24-70mm', '4000x3000', 'Canon 5D', now(), true),
('20000000-0000-0000-0000-000000000009', '00000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000008', 'byron1.jpg', now(), '18-55mm', '3000x2000', 'Nikon D750', now(), true),
('20000000-0000-0000-0000-000000000010', '00000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000009', 'fanad1.jpg', now(), '50mm', '6000x4000', 'Sony A7', now(), true),
('20000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'portland3.jpg', now(), '24-70mm', '4000x3000', 'Canon 5D', now(), false),
('20000000-0000-0000-0000-000000000012', '00000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'peggys2.jpg', now(), '18-55mm', '3000x2000', 'Nikon D750', now(), false),
('20000000-0000-0000-0000-000000000013', '00000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000003', 'hercules2.jpg', now(), '50mm', '6000x4000', 'Sony A7', now(), false);

-- Popüler 3 deniz feneri için rastgele yorumlar
INSERT INTO comments (id, user_id, photo_id, text, rating) VALUES
('30000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', 'Harika bir manzara!', 5),
('30000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000002', 'Çok etkileyici.', 4),
('30000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000011', 'Favorim!', 5),
('30000000-0000-0000-0000-000000000004', '00000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000003', 'Çok güzel.', 4),
('30000000-0000-0000-0000-000000000005', '00000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000012', 'Mutlaka gidin.', 5),
('30000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000004', 'Tarihi atmosfer.', 4),
('30000000-0000-0000-0000-000000000007', '00000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000013', 'Çok iyi korunmuş.', 5);

-- Diğer deniz fenerleri için örnek yorumlar (isteğe bağlı)
INSERT INTO comments (id, user_id, photo_id, text, rating) VALUES
('30000000-0000-0000-0000-000000000008', '00000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000005', 'Güzel bir yer.', 3),
('30000000-0000-0000-0000-000000000009', '00000000-0000-0000-0000-000000000009', '20000000-0000-0000-0000-000000000006', 'Fena değil.', 3),
('30000000-0000-0000-0000-000000000010', '00000000-0000-0000-0000-000000000010', '20000000-0000-0000-0000-000000000007', 'Beğendim.', 4);

-- Outbox Pattern kullanımı için
CREATE TABLE IF NOT EXISTS outbox_events (
    id UUID PRIMARY KEY,
    occurred_at TIMESTAMP WITH TIME ZONE NOT NULL,
    event_type VARCHAR(255) NOT NULL,
    event_data JSONB NOT NULL,
    aggregate_id VARCHAR(255),
    version INTEGER NOT NULL DEFAULT 1,
    is_processed BOOLEAN NOT NULL DEFAULT FALSE,
    processed_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

CREATE INDEX IF NOT EXISTS idx_outbox_events_is_processed ON outbox_events(is_processed);
CREATE INDEX IF NOT EXISTS idx_outbox_events_event_type ON outbox_events(event_type);
CREATE INDEX IF NOT EXISTS idx_outbox_events_aggregate_id ON outbox_events(aggregate_id);
CREATE INDEX IF NOT EXISTS idx_outbox_events_created_at ON outbox_events(created_at);
CREATE INDEX IF NOT EXISTS idx_outbox_events_processed_at ON outbox_events(processed_at);