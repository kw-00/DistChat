SET search_path TO distchat, public;

/* 
DOMAIN: Users 
*/
-- Users
CREATE TABLE IF NOT EXISTS users (
    id UUID DEFAULT uuidv7(),
    login TEXT NOT NULL,
    email TEXT NOT NULL,
    passwordHash TEXT NOT NULL,
);
ALTER TABLE users ADD CONSTRAINT pk_users PRIMARY KEY (id);
ALTER TABLE users ADD CONSTRAINT users_login_unique UNIQUE (login);
ALTER TABLE users ADD CONSTRAINT users_email_unique UNIQUE (email);

-- Friendships
CREATE TABLE IF NOT EXISTS friendships (
    userId UUID NOT NULL,
    friendId UUID NOT NULL,
);
ALTER TABLE friendships ADD CONSTRAINT pk_friendships PRIMARY KEY (userId, friendId);
ALTER TABLE friendships ADD CONSTRAINT friendships_fk_userId
    FOREIGN KEY (userId) REFERENCES users (id)
    ON DELETE CASCADE
;
ALTER TABLE friendships ADD CONSTRAINT friendships_fk_friendId
    FOREIGN KEY (friendId) REFERENCES users (id)
    ON DELETE CASCADE
;

-- FriendRequests
CREATE TABLE IF NOT EXISTS friendRequests (
    requestingUserId UUID NOT NULL,
    targetUserId UUID NOT NULL,
);
ALTER TABLE friendRequests 
    ADD CONSTRAINT pk_friendRequests PRIMARY KEY (requestingUserId, targetUserId)
ALTER TABLE friendRequests
    ADD CONSTRAINT friendRequests_fk_requestingUserId
        FOREIGN KEY (requestingUserId) REFERENCES users (id)
        ON DELETE CASCADE
;
ALTER TABLE friendRequests
    ADD CONSTRAINT friendRequests_fk_targetUserId
        FOREIGN KEY (targetUserId) REFERENCES users (id)
        ON DELETE CASCADE
;
CREATE UNIQUE INDEX IF NOT EXISTS friendRequests_no_mutual_requests
ON friendRequests (
    LEAST(requestingUserId, targetUserId),
    GREATEST(requestingUserId, targetUserId)
);

/* 
DOMAIN: Chat
*/
-- Rooms
CREATE TABLE IF NOT EXISTS rooms (
    id UUID,
    name TEXT DEFAULT NULL,
    type TEXT NOT NULL,
    dmUserLowId UUID DEFAULT NULL
    dmUserHighId UUID DEFAULT NULL,
);
ALTER TABLE rooms ADD CONSTRAINT pk_rooms PRIMARY KEY (id);
ALTER TABLE rooms ADD CONSTRAINT rooms_type_check
    CHECK (type IN ('group', 'dm'))
;
CREATE INDEX rooms_unique_dms ON rooms (dmUserHighId, dmUserLowId)
WHERE type = 'dm';
ALTER TABLE rooms ADD CONSTRAINT rooms_fk_dmUserLowId FOREIGN KEY (dmUserLowId)
    REFERENCES users (id)
    ON DELETE SET NULL
;
ALTER TABLE rooms ADD CONSTRAINT rooms_fk_dmUserHighId FOREIGN KEY (dmUserHighId)
    REFERENCES users (id)
    ON DELETE SET NULL
;
ALTER TABLE rooms ADD CONSTRAINT rooms_dm_user_order
    CHECK (dmUserLowId < dmUserHighId)
;

-- Roles
CREATE TABLE IF NOT EXISTS roles (
    id TEXT PRIMARY KEY,
    level INTEGER UNIQUE
);
INSERT INTO roles (id, level)
VALUES
    ('owner', 3),
    ('elder', 2),
    ('member', 1)
;

-- Memberships
CREATE TABLE IF NOT EXISTS memberships (
    userId UUID NOT NULL,
    roomId UUID NOT NULL,
    roleId TEXT,
);
ALTER TABLE membership ADD CONSTRAINT pk_membership PRIMARY KEY (userId, roomId);
ALTER TABLE membership ADD CONSTRAINT memberships_fk_userId
    FOREIGN KEY (userId) REFERENCES users (id)
    ON DELETE CASCADE
;
ALTER TABLE membership ADD CONSTRAINT memberships_fk_roomId
    FOREIGN KEY (roomId) REFERENCES rooms (id)
    ON DELETE CASCADE
;
ALTER TABLE membership ADD CONSTRAINT memberships_fk_roleId
    FOREIGN KEY (roleId) REFERENCES roles (id)
    ON DELETE RESTRICT
;

-- Messages
CREATE TABLE IF NOT EXISTS messages (
    id UUID DEFAULT uuidv7(),
    userId UUID NOT NULL,
    roomId UUID NOT NULL,
    content TEXT NOT NULL,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
ALTER TABLE messages ADD CONSTRAINT pk_messages PRIMARY KEY (id);
ALTER TABLE messages ADD CONSTRAINT messages_fk_userId
    FOREIGN KEY (userId) REFERENCES users (id)
    ON DELETE CASCADE
;
ALTER TABLE messages ADD CONSTRAINT messages_fk_roomId
    FOREIGN KEY (roomId) REFERENCES rooms (id)
    ON DELETE CASCADE
;


/*
DOMAIN: Auth
*/
-- RefreshTokens
CREATE TABLE IF NOT EXISTS refreshTokens (
    id UUID DEFAULT uuidv7(),
    userId UUID NOT NULL,
    isUsed BOOLEAN NOT NULL DEFAULT FALSE,
    expiresAt TIMESTAMPTZ NOT NULL
);
ALTER TABLE refreshTokens ADD CONSTRAINT pk_refreshTokens PRIMARY KEY (id);
ALTER TABLE refreshTokens ADD CONSTRAINT refreshTokens_fk_userId
    FOREIGN KEY (userId) REFERENCES users (id)
    ON DELETE CASCADE
;

-- PendingRegistrations
CREATE TABLE IF NOT EXISTS pendingRegistrations (
    id UUID DEFAULT uuidv7(),
    login TEXT NOT NULL,
    email TEXT NOT NULL,
    passwordHash TEXT NOT NULL
);
ALTER TABLE pendingRegistrations 
    ADD CONSTRAINT pk_pendingRegistrations PRIMARY KEY (id)
;
ALTER TABLE pendingRegistrations 
    ADD CONSTRAINT pendingRegistrations_login_unique UNIQUE (login)
;
ALTER TABLE pendingRegistrations 
    ADD CONSTRAINT pendingRegistrations_email_unique UNIQUE (email)
;
