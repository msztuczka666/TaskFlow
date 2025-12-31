<?php
/**
 * TaskFlow - Logout Handler
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

// Logout user
$authController->logout();

// Set success message
$session->start(); // Restart session to set message
$session->set('success', 'You have been logged out successfully');

// Redirect to login page
header('Location: index.php');
exit;
