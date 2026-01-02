<?php
/**
 * TaskFlow - Dashboard
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

// Check if authenticated
if (!$authController->isAuthenticated()) {
    // Try remember me
    if (!$authController->authenticateFromRememberMe()) {
        $session->set('error', 'Please login to access this page');
        header('Location: index.php');
        exit;
    }
}

// Get current user
$currentUser = $authController->getCurrentUser();
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Dashboard - TaskFlow</title>
    <link rel="stylesheet" href="assets/css/style.css">
    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        }
        
        .dashboard-container {
            flex: 1;
            padding: 40px 20px;
            max-width: 1200px;
            margin: 0 auto;
        }
        
        .dashboard-header {
            background: white;
            padding: 30px;
            border-radius: 12px;
            margin-bottom: 30px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
        }
        
        .dashboard-header h1 {
            color: var(--primary-color);
            margin-bottom: 10px;
        }
        
        .user-info {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }
        
        .user-details {
            color: var(--text-light);
        }
        
        .user-details p {
            margin: 5px 0;
        }
        
        .badge {
            display: inline-block;
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }
        
        .badge-admin {
            background: #e74c3c;
            color: white;
        }
        
        .badge-manager {
            background: #f39c12;
            color: white;
        }
        
        .badge-employee {
            background: #3498db;
            color: white;
        }
        
        .btn-logout {
            padding: 10px 25px;
            background: #e74c3c;
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s ease;
        }
        
        .btn-logout:hover {
            background: #c0392b;
            transform: translateY(-2px);
        }
        
        .welcome-card {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
        }
        
        .welcome-card h2 {
            color: var(--text-dark);
            margin-bottom: 15px;
        }
        
        .welcome-card p {
            color: var(--text-light);
            line-height: 1.6;
        }
    </style>
</head>
<body>
    <div class="dashboard-container">
        <div class="dashboard-header">
            <h1>TaskFlow Dashboard</h1>
            <div class="user-info">
                <div class="user-details">
                    <p><strong>Email:</strong> <?php echo htmlspecialchars($currentUser['email']); ?></p>
                    <p><strong>Role:</strong> <span class="badge badge-<?php echo htmlspecialchars($currentUser['role']); ?>"><?php echo htmlspecialchars($currentUser['role']); ?></span></p>
                    <p><strong>Member since:</strong> <?php echo date('F j, Y', strtotime($currentUser['created_at'])); ?></p>
                </div>
                <div>
                    <a href="logout.php" class="btn-logout">Logout</a>
                </div>
            </div>
        </div>
        
        <div class="welcome-card">
            <h2>Welcome to TaskFlow!</h2>
            <p>You have successfully logged in to the TaskFlow Employee & Task Management System.</p>
            <p>This is a foundational infrastructure that includes:</p>
            <ul style="margin: 15px 0; padding-left: 20px; color: var(--text-light);">
                <li>User authentication with secure password hashing</li>
                <li>"Remember Me" functionality for persistent sessions</li>
                <li>Normalized database schema for users, employees, projects, tasks, and absences</li>
                <li>PSR-4 autoloading with Composer</li>
                <li>Modern, responsive UI</li>
            </ul>
            <p>Future iterations will build upon this foundation to include task records, employee management, reports, and more.</p>
        </div>
    </div>

    <footer>
        <p>&copy; 2025 Synthraxis &amp; Mariusz Sztuczka</p>
    </footer>
</body>
</html>
