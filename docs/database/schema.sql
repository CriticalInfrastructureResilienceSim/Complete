-- Critical Infrastructure Resilience Simulator
-- Run this whole script in MySQL Workbench (File > Open SQL Script, then the lightning bolt).
-- It matches the User model and AppDbContext in Cirs.Web exactly.

CREATE DATABASE IF NOT EXISTS cirs_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE cirs_db;

CREATE TABLE IF NOT EXISTS users (
  id                  INT           NOT NULL AUTO_INCREMENT,
  full_name           VARCHAR(100)  NOT NULL,
  email               VARCHAR(150)  NOT NULL,
  password_hash       VARCHAR(255)  NOT NULL,
  role                VARCHAR(20)   NOT NULL DEFAULT 'User',
  is_active           TINYINT(1)    NOT NULL DEFAULT 1,
  failed_login_count  INT           NOT NULL DEFAULT 0,
  lockout_end         DATETIME      NULL,
  last_login_at       DATETIME      NULL,
  created_at          DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT uq_users_email UNIQUE (email)
) ENGINE = InnoDB;

-- Handy checks while testing (the password column holds a BCrypt hash, never the real password):
-- SELECT id, full_name, email, role, is_active, failed_login_count, lockout_end, last_login_at, created_at FROM users;
-- UPDATE users SET failed_login_count = 0, lockout_end = NULL WHERE email = 'test@example.com';  -- unlock an account
