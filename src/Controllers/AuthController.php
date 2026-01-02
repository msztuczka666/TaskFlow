<?php

namespace TaskFlow\Controllers;

use TaskFlow\Models\User;
use TaskFlow\Services\Session;

/**
 * Auth Controller
 * Handles authentication logic
 */
class AuthController
{
    private User $userModel;
    private Session $session;
    private array $config;

    public function __construct(User $userModel, Session $session, array $config)
    {
        $this->userModel = $userModel;
        $this->session = $session;
        $this->config = $config;
    }

    /**
     * Login user
     */
    public function login(string $email, string $password, bool $rememberMe = false): array
    {
        // Clean expired tokens periodically
        $this->userModel->cleanExpiredTokens();

        // Find user by email
        $user = $this->userModel->findByEmail($email);

        if (!$user) {
            return ['success' => false, 'message' => 'Invalid email or password'];
        }

        // Verify password
        if (!$this->userModel->verifyPassword($password, $user['password'])) {
            return ['success' => false, 'message' => 'Invalid email or password'];
        }

        // Set session data
        $this->session->set('user_id', $user['id']);
        $this->session->set('user_email', $user['email']);
        $this->session->set('user_role', $user['role']);
        $this->session->regenerate();

        // Handle remember me
        if ($rememberMe) {
            $token = $this->userModel->createRememberToken(
                $user['id'],
                $this->config['remember_me']['token_lifetime']
            );

            // Set remember me cookie
            setcookie(
                $this->config['remember_me']['cookie_name'],
                $token,
                time() + $this->config['remember_me']['token_lifetime'],
                '/',
                '',
                false, // secure - set to true for HTTPS
                true   // httponly
            );
        }

        return ['success' => true, 'message' => 'Login successful'];
    }

    /**
     * Logout user
     */
    public function logout(): void
    {
        // Delete remember me cookie and token if exists
        if (isset($_COOKIE[$this->config['remember_me']['cookie_name']])) {
            $token = $_COOKIE[$this->config['remember_me']['cookie_name']];
            $this->userModel->deleteRememberToken($token);

            setcookie(
                $this->config['remember_me']['cookie_name'],
                '',
                time() - 3600,
                '/',
                '',
                false,
                true
            );
        }

        // Destroy session
        $this->session->destroy();
    }

    /**
     * Check if user is authenticated
     */
    public function isAuthenticated(): bool
    {
        return $this->session->has('user_id');
    }

    /**
     * Get current user
     */
    public function getCurrentUser(): ?array
    {
        $userId = $this->session->get('user_id');
        
        if (!$userId) {
            return null;
        }

        return $this->userModel->findById($userId);
    }

    /**
     * Try to authenticate from remember me cookie
     */
    public function authenticateFromRememberMe(): bool
    {
        if (!isset($_COOKIE[$this->config['remember_me']['cookie_name']])) {
            return false;
        }

        $token = $_COOKIE[$this->config['remember_me']['cookie_name']];
        $user = $this->userModel->findByRememberToken($token);

        if (!$user) {
            // Invalid or expired token, delete cookie
            setcookie(
                $this->config['remember_me']['cookie_name'],
                '',
                time() - 3600,
                '/',
                '',
                false,
                true
            );
            return false;
        }

        // Set session data
        $this->session->set('user_id', $user['id']);
        $this->session->set('user_email', $user['email']);
        $this->session->set('user_role', $user['role']);
        $this->session->regenerate();

        return true;
    }
}
