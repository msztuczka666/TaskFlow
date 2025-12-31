<?php
/**
 * TaskFlow - Login Page
 */

require_once __DIR__ . '/../vendor/autoload.php';

use TaskFlow\Services\Database;
use TaskFlow\Services\Session;
use TaskFlow\Models\User;
use TaskFlow\Controllers\AuthController;

// Load configuration
$config = require __DIR__ . '/../config/config.php';

// Initialize services
$database = new Database($config['database']);
$session = new Session($config['session']);
$session->start();

$userModel = new User($database);
$authController = new AuthController($userModel, $session, $config);

// Check if already authenticated
if ($authController->isAuthenticated()) {
    header('Location: dashboard.php');
    exit;
}

// Try to authenticate from remember me cookie
if ($authController->authenticateFromRememberMe()) {
    header('Location: dashboard.php');
    exit;
}

// Get flash message if any
$errorMessage = $session->get('error');
$successMessage = $session->get('success');
$session->remove('error');
$session->remove('success');
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login - TaskFlow</title>
    <link rel="stylesheet" href="assets/css/style.css">
</head>
<body>
    <div class="container">
        <div class="login-card">
            <div class="login-header">
                <h1>TaskFlow</h1>
                <p>Employee & Task Management System</p>
            </div>

            <?php if ($errorMessage): ?>
                <div class="alert alert-error">
                    <?php echo htmlspecialchars($errorMessage); ?>
                </div>
            <?php endif; ?>

            <?php if ($successMessage): ?>
                <div class="alert alert-success">
                    <?php echo htmlspecialchars($successMessage); ?>
                </div>
            <?php endif; ?>

            <form id="loginForm" method="POST" action="login_process.php">
                <div class="form-group">
                    <label for="email">Email Address</label>
                    <input 
                        type="email" 
                        id="email" 
                        name="email" 
                        placeholder="Enter your email"
                        required
                        autocomplete="email"
                    >
                </div>

                <div class="form-group">
                    <label for="password">Password</label>
                    <input 
                        type="password" 
                        id="password" 
                        name="password" 
                        placeholder="Enter your password"
                        required
                        autocomplete="current-password"
                    >
                </div>

                <div class="remember-me">
                    <input 
                        type="checkbox" 
                        id="rememberMe" 
                        name="remember_me"
                    >
                    <label for="rememberMe">Remember me for 30 days</label>
                </div>

                <button type="submit" class="btn-login">
                    Login
                </button>
            </form>
        </div>
    </div>

    <footer>
        <p>&copy; 2025 Synthraxis &amp; Mariusz Sztuczka</p>
    </footer>

    <script src="assets/js/login.js"></script>
</body>
</html>
