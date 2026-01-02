<?php
/**
 * TaskFlow - Login Process Handler
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

// Only accept POST requests
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['success' => false, 'message' => 'Method not allowed']);
    exit;
}

// Get POST data
$email = $_POST['email'] ?? '';
$password = $_POST['password'] ?? '';
$rememberMe = isset($_POST['remember_me']) && $_POST['remember_me'] === '1';

// Validate input
if (empty($email) || empty($password)) {
    echo json_encode(['success' => false, 'message' => 'Email and password are required']);
    exit;
}

// Attempt login
$result = $authController->login($email, $password, $rememberMe);

// Return JSON response
header('Content-Type: application/json');
echo json_encode($result);
