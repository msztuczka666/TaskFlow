<?php

namespace TaskFlow\Services;

use PDO;
use PDOException;

/**
 * Database Service
 * Handles database connections and queries
 */
class Database
{
    private static ?PDO $connection = null;
    private array $config;

    public function __construct(array $config)
    {
        $this->config = $config;
    }

    /**
     * Get database connection (Singleton pattern)
     */
    public function getConnection(): PDO
    {
        if (self::$connection === null) {
            try {
                $dsn = sprintf(
                    "mysql:host=%s;port=%d;dbname=%s;charset=%s",
                    $this->config['host'],
                    $this->config['port'],
                    $this->config['database'],
                    $this->config['charset']
                );

                self::$connection = new PDO(
                    $dsn,
                    $this->config['username'],
                    $this->config['password'],
                    [
                        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
                        PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
                        PDO::ATTR_EMULATE_PREPARES => false,
                    ]
                );
            } catch (PDOException $e) {
                error_log("Database connection failed: " . $e->getMessage());
                throw new PDOException("Database connection failed");
            }
        }

        return self::$connection;
    }

    /**
     * Execute a query
     */
    public function query(string $sql, array $params = []): \PDOStatement
    {
        $connection = $this->getConnection();
        $statement = $connection->prepare($sql);
        $statement->execute($params);
        return $statement;
    }

    /**
     * Fetch all rows
     */
    public function fetchAll(string $sql, array $params = []): array
    {
        return $this->query($sql, $params)->fetchAll();
    }

    /**
     * Fetch single row
     */
    public function fetchOne(string $sql, array $params = []): ?array
    {
        $result = $this->query($sql, $params)->fetch();
        return $result ?: null;
    }

    /**
     * Get last insert ID
     */
    public function lastInsertId(): string
    {
        return $this->getConnection()->lastInsertId();
    }
}
