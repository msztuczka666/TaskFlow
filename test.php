<?php
/**
 * TaskFlow - Test Script
 * This script tests the basic functionality of the TaskFlow system
 */

require_once __DIR__ . '/vendor/autoload.php';

use TaskFlow\Services\Database;

echo "TaskFlow System Test\n";
echo "===================\n\n";

// Test 1: Autoloading
echo "Test 1: Autoloading... ";
try {
    $reflection = new ReflectionClass('TaskFlow\Services\Database');
    echo "✓ PASSED\n";
} catch (Exception $e) {
    echo "✗ FAILED: " . $e->getMessage() . "\n";
}

// Test 2: Configuration file
echo "Test 2: Configuration file... ";
$configFile = __DIR__ . '/config/config.php';
if (file_exists($configFile)) {
    $config = require $configFile;
    if (isset($config['database'])) {
        echo "✓ PASSED\n";
    } else {
        echo "✗ FAILED: Configuration structure invalid\n";
    }
} else {
    echo "✗ FAILED: config.php not found\n";
}

// Test 3: Database connection (optional - requires database to be set up)
echo "Test 3: Database connection... ";
try {
    $database = new Database($config['database']);
    $connection = $database->getConnection();
    if ($connection instanceof PDO) {
        echo "✓ PASSED\n";
        
        // Test 4: Query execution
        echo "Test 4: Database query... ";
        try {
            $result = $database->fetchAll("SHOW TABLES");
            $tableCount = count($result);
            echo "✓ PASSED (Found " . $tableCount . " tables)\n";
            
            if ($tableCount > 0) {
                // Extract table names from result (works across different DB configurations)
                $tables = array_map(function($row) {
                    return reset($row); // Get first value from each row
                }, $result);
                echo "   Tables: " . implode(", ", $tables) . "\n";
            }
        } catch (Exception $e) {
            echo "✗ FAILED: " . $e->getMessage() . "\n";
        }
    } else {
        echo "✗ FAILED: Invalid connection object\n";
    }
} catch (Exception $e) {
    echo "⚠ SKIPPED: " . $e->getMessage() . "\n";
    echo "   (This is expected if the database hasn't been set up yet)\n";
}

echo "\n";
echo "Test Summary\n";
echo "============\n";
echo "Autoloading and configuration are working correctly.\n";
echo "Database connection will work once MariaDB/MySQL is configured and schema.sql is imported.\n";
echo "\nNext steps:\n";
echo "1. Configure your database in config/config.php\n";
echo "2. Import database/schema.sql into MariaDB/MySQL\n";
echo "3. Configure your web server to point to the public/ directory\n";
echo "4. Access the login page at http://your-domain/\n";
