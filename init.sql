
-- DROP TABLE

DROP TABLE IF EXISTS To_inspect_hack;
DROP TABLE IF EXISTS Loved_hack;
DROP TABLE IF EXISTS Comment;
DROP TABLE IF EXISTS Hack;
DROP TABLE IF EXISTS User;

-- CREATE TABLE

CREATE TABLE User(
    id_user SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    username VARCHAR(100),
    pwd VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    profile_picture VARCHAR(255),
    description VARCHAR(255),
    ban BOOLEAN DEFAULT FALSE,
    ban_time DATE DEFAULT NULL,
    is_admin BOOLEAN DEFAULT FALSE,
-- CLEFS
    PRIMARY KEY(id_user)
);

CREATE TABLE Hack(
    id_hack SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    title VARCHAR(255),
    img_url VARCHAR(255),
    description VARCHAR(255),
    nbLikes INT DEFAULT 0,
    reported BOOLEAN DEFAULT FALSE,
    reason_reported VARCHAR(255),
    hack_type VARCHAR(15),
    id_user SMALLINT UNSIGNED NOT NULL,
-- CLEFS
    PRIMARY KEY(id_hack),
    FOREIGN KEY(id_user) REFERENCES User(id_user)
);

CREATE TABLE Comment(
    id_comment SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    content VARCHAR(255),
    nb_likes INT DEFAULT 0,
    id_hack SMALLINT UNSIGNED NOT NULL,
    id_user SMALLINT UNSIGNED NOT NULL,
-- CLEFS
    PRIMARY KEY(id_comment),
    FOREIGN KEY(id_hack) REFERENCES Hack(id_hack),
    FOREIGN KEY(id_user) REFERENCES User(id_user)
);

CREATE TABLE Loved_hack(
    id_hack SMALLINT UNSIGNED NOT NULL,
    id_user SMALLINT UNSIGNED NOT NULL,
-- CLEFS
    PRIMARY KEY(id_hack, id_user),
    FOREIGN KEY(id_hack) REFERENCES Hack(id_hack),
    FOREIGN KEY(id_user) REFERENCES User(id_user)
);

CREATE TABLE To_inspect_hack(
    id_hack SMALLINT UNSIGNED NOT NULL,
    id_user SMALLINT UNSIGNED NOT NULL,
-- CLEFS
    PRIMARY KEY(id_hack, id_user),
    FOREIGN KEY(id_hack) REFERENCES Hack(id_hack),
    FOREIGN KEY(id_user) REFERENCES User(id_user)
);