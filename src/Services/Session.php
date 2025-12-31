<?php

namespace TaskFlow\Services;

/**
 * Session Service
 * Handles session management
 */
class Session
{
    private array $config;
    private bool $started = false;

    public function __construct(array $config)
    {
        $this->config = $config;
    }

    /**
     * Start the session
     */
    public function start(): void
    {
        if ($this->started || session_status() === PHP_SESSION_ACTIVE) {
            return;
        }

        session_name($this->config['name']);
        
        session_set_cookie_params([
            'lifetime' => $this->config['lifetime'],
            'path' => $this->config['path'],
            'domain' => $this->config['domain'],
            'secure' => $this->config['secure'],
            'httponly' => $this->config['httponly'],
            'samesite' => $this->config['samesite']
        ]);

        session_start();
        $this->started = true;

        // Regenerate session ID periodically for security
        if (!$this->has('_session_started')) {
            session_regenerate_id(true);
            $this->set('_session_started', time());
        }
    }

    /**
     * Set session value
     */
    public function set(string $key, $value): void
    {
        $_SESSION[$key] = $value;
    }

    /**
     * Get session value
     */
    public function get(string $key, $default = null)
    {
        return $_SESSION[$key] ?? $default;
    }

    /**
     * Check if session key exists
     */
    public function has(string $key): bool
    {
        return isset($_SESSION[$key]);
    }

    /**
     * Remove session value
     */
    public function remove(string $key): void
    {
        unset($_SESSION[$key]);
    }

    /**
     * Destroy session
     */
    public function destroy(): void
    {
        if (session_status() === PHP_SESSION_ACTIVE) {
            $_SESSION = [];
            
            // Delete session cookie
            if (ini_get("session.use_cookies")) {
                $params = session_get_cookie_params();
                setcookie(
                    session_name(),
                    '',
                    time() - 42000,
                    $params["path"],
                    $params["domain"],
                    $params["secure"],
                    $params["httponly"]
                );
            }
            
            session_destroy();
            $this->started = false;
        }
    }

    /**
     * Regenerate session ID
     */
    public function regenerate(): void
    {
        if (session_status() === PHP_SESSION_ACTIVE) {
            session_regenerate_id(true);
        }
    }
}
