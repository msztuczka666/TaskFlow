<?php

namespace TaskFlow\Models;

use TaskFlow\Services\Database;

/**
 * User Model
 * Handles user data and authentication
 */
class User
{
    private Database $db;

    public function __construct(Database $db)
    {
        $this->db = $db;
    }

    /**
     * Find user by email
     */
    public function findByEmail(string $email): ?array
    {
        $sql = "SELECT * FROM users WHERE email = ? LIMIT 1";
        return $this->db->fetchOne($sql, [$email]);
    }

    /**
     * Find user by ID
     */
    public function findById(int $id): ?array
    {
        $sql = "SELECT * FROM users WHERE id = ? LIMIT 1";
        return $this->db->fetchOne($sql, [$id]);
    }

    /**
     * Verify password
     */
    public function verifyPassword(string $password, string $hash): bool
    {
        return password_verify($password, $hash);
    }

    /**
     * Create remember token
     */
    public function createRememberToken(int $userId, int $lifetime): string
    {
        // Generate a secure random token
        $token = bin2hex(random_bytes(32));
        $expiresAt = date('Y-m-d H:i:s', time() + $lifetime);

        $sql = "INSERT INTO remember_tokens (user_id, token, expires_at) VALUES (?, ?, ?)";
        $this->db->query($sql, [$userId, $token, $expiresAt]);

        return $token;
    }

    /**
     * Find user by remember token
     */
    public function findByRememberToken(string $token): ?array
    {
        $sql = "SELECT u.* FROM users u 
                INNER JOIN remember_tokens rt ON u.id = rt.user_id 
                WHERE rt.token = ? AND rt.expires_at > NOW() 
                LIMIT 1";
        return $this->db->fetchOne($sql, [$token]);
    }

    /**
     * Delete remember token
     */
    public function deleteRememberToken(string $token): void
    {
        $sql = "DELETE FROM remember_tokens WHERE token = ?";
        $this->db->query($sql, [$token]);
    }

    /**
     * Delete all remember tokens for user
     */
    public function deleteAllRememberTokens(int $userId): void
    {
        $sql = "DELETE FROM remember_tokens WHERE user_id = ?";
        $this->db->query($sql, [$userId]);
    }

    /**
     * Clean expired remember tokens
     */
    public function cleanExpiredTokens(): void
    {
        $sql = "DELETE FROM remember_tokens WHERE expires_at < NOW()";
        $this->db->query($sql);
    }

    /**
     * Create new user
     */
    public function create(string $email, string $password, string $role = 'employee'): int
    {
        $hashedPassword = password_hash($password, PASSWORD_DEFAULT);
        $sql = "INSERT INTO users (email, password, role) VALUES (?, ?, ?)";
        $this->db->query($sql, [$email, $hashedPassword, $role]);
        return (int) $this->db->lastInsertId();
    }
}
