<?php
/**
 * Database Configuration
 * Copy this file to config.php and update with your database credentials
 */

return [
    'database' => [
        'host' => 'localhost',
        'port' => 3306,
        'database' => 'taskflow',
        'username' => 'root',
        'password' => '',
        'charset' => 'utf8mb4',
        'collation' => 'utf8mb4_unicode_ci',
    ],
    'session' => [
        'name' => 'TASKFLOW_SESSION',
        'lifetime' => 7200, // 2 hours in seconds
        'path' => '/',
        'domain' => '',
        'secure' => false, // Set to true if using HTTPS
        'httponly' => true,
        'samesite' => 'Lax'
    ],
    'remember_me' => [
        'cookie_name' => 'TASKFLOW_REMEMBER',
        'token_lifetime' => 2592000, // 30 days in seconds
    ],
    'app' => [
        'name' => 'TaskFlow',
        'url' => 'http://localhost',
        'timezone' => 'UTC',
    ]
];
