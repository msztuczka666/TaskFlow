# TaskFlow - Architecture Documentation

## Overview

TaskFlow follows a clean, modular architecture with separation of concerns. The application is built using PHP with PSR-4 autoloading standards and follows modern web development best practices.

## Architecture Layers

### 1. Presentation Layer (`public/`)

The public directory contains all web-accessible files:

- **index.php**: Login page entry point
- **login_process.php**: Handles login form submission
- **logout.php**: Handles user logout
- **dashboard.php**: Main application dashboard
- **assets/**: Static files (CSS, JavaScript)

### 2. Application Layer (`src/`)

#### Controllers (`src/Controllers/`)

Controllers handle the business logic and coordinate between models and views.

**AuthController.php**
- Manages user authentication
- Handles login/logout operations
- Manages "Remember Me" functionality
- Session validation

#### Models (`src/Models/`)

Models represent data structures and handle database operations.

**User.php**
- User CRUD operations
- Password verification
- Remember token management
- User lookup by email/ID

#### Services (`src/Services/`)

Services provide reusable functionality across the application.

**Database.php**
- Singleton PDO connection
- Query execution
- Prepared statements
- Fetch operations

**Session.php**
- Session lifecycle management
- Secure session configuration
- Session data handling
- Session destruction

### 3. Data Layer (`database/`)

**schema.sql**
- Complete database schema
- Table definitions with constraints
- Foreign key relationships
- Indexes for performance
- Default admin user

## Design Patterns

### Singleton Pattern

The Database service uses the Singleton pattern to ensure only one database connection exists throughout the application lifecycle.

```php
private static ?PDO $connection = null;

public function getConnection(): PDO
{
    if (self::$connection === null) {
        // Create connection
    }
    return self::$connection;
}
```

### Dependency Injection

Controllers receive their dependencies through constructor injection:

```php
public function __construct(User $userModel, Session $session, array $config)
{
    $this->userModel = $userModel;
    $this->session = $session;
    $this->config = $config;
}
```

### MVC Pattern (Model-View-Controller)

- **Models**: Handle data and business logic
- **Views**: PHP templates in public/ directory
- **Controllers**: Coordinate between models and views

## Security Features

### Password Security

- **Bcrypt Hashing**: All passwords are hashed using PHP's `password_hash()` with `PASSWORD_DEFAULT` (bcrypt)
- **Secure Verification**: `password_verify()` for timing-attack resistant verification

### SQL Injection Prevention

- **Prepared Statements**: All database queries use PDO prepared statements
- **Parameter Binding**: User input is always parameterized

```php
$sql = "SELECT * FROM users WHERE email = ? LIMIT 1";
return $this->db->fetchOne($sql, [$email]);
```

### Session Security

- **Secure Configuration**: HttpOnly cookies, SameSite policy
- **Session Regeneration**: ID regenerated on login to prevent fixation attacks
- **Proper Destruction**: Complete session cleanup on logout

### Remember Me Security

- **Secure Tokens**: 64-character random tokens (256-bit entropy)
- **Token Expiration**: Automatic expiration handling
- **Token Cleanup**: Expired tokens are periodically removed

### XSS Prevention

- **Output Escaping**: All user data is escaped with `htmlspecialchars()`
- **Content-Type Headers**: Proper content-type headers set

### CSRF Protection

Ready for implementation in future iterations using token-based validation.

## Database Schema

### Entity Relationship

```
users (1) -------- (0..1) employees
  |                           |
  |                           |
  | (1)                  (1) |
  |                           |
remember_tokens          absences
                              
                         
projects (1) ---- (N) tasks
   |
   | (manager_id)
   |
employees (1) ---- (N) tasks (assigned_to)
```

### Normalization

The database follows Third Normal Form (3NF):

1. **First Normal Form (1NF)**: All tables have atomic values
2. **Second Normal Form (2NF)**: No partial dependencies
3. **Third Normal Form (3NF)**: No transitive dependencies

### Indexes

Strategic indexes for performance:

- **Primary Keys**: All tables
- **Foreign Keys**: Relationship integrity
- **Email Fields**: Fast user lookup
- **Status Fields**: Efficient filtering
- **Date Ranges**: Optimized date queries

## Configuration Management

Configuration is centralized in `config/config.php`:

```php
return [
    'database' => [...],    // Database credentials
    'session' => [...],     // Session configuration
    'remember_me' => [...], // Remember Me settings
    'app' => [...]         // Application settings
];
```

**Benefits:**
- Single source of truth
- Easy environment switching
- Excluded from version control (gitignore)

## Request Flow

### Login Request Flow

1. User submits login form (`index.php`)
2. Form data sent to `login_process.php`
3. `AuthController->login()` is called
4. User model finds user by email
5. Password is verified
6. Session is created and user data stored
7. Remember token created if requested
8. JSON response sent to client
9. JavaScript redirects to dashboard

### Remember Me Flow

1. User visits site with remember cookie
2. `index.php` checks if already authenticated
3. If not, `authenticateFromRememberMe()` is called
4. Token is validated and expiration checked
5. If valid, session is created automatically
6. User is redirected to dashboard

### Logout Flow

1. User clicks logout button
2. `logout.php` is called
3. Remember token is deleted from database
4. Remember cookie is removed
5. Session is destroyed
6. User is redirected to login page

## Autoloading

PSR-4 autoloading standard with Composer:

```json
{
    "autoload": {
        "psr-4": {
            "TaskFlow\\": "src/"
        }
    }
}
```

**Namespace to Path Mapping:**
- `TaskFlow\Controllers\AuthController` → `src/Controllers/AuthController.php`
- `TaskFlow\Models\User` → `src/Models/User.php`
- `TaskFlow\Services\Database` → `src/Services/Database.php`

## Error Handling

### Production Mode

- Display errors disabled
- Errors logged to files
- Generic error messages to users

### Development Mode

- Display errors enabled
- Detailed stack traces
- Debug information visible

Configuration in `.htaccess` or `php.ini`:

```apache
php_flag display_errors Off
php_flag log_errors On
```

## Future Enhancements

### Planned Features

1. **CSRF Protection**: Token-based form validation
2. **API Layer**: RESTful API endpoints
3. **Rate Limiting**: Login attempt throttling
4. **Email Service**: Password reset, notifications
5. **Two-Factor Authentication**: Enhanced security
6. **Audit Logging**: Track all user actions
7. **File Uploads**: Employee documents, task attachments
8. **Reporting**: PDF/CSV export functionality

### Architectural Improvements

1. **Router Class**: URL routing with pretty URLs
2. **Template Engine**: Separation of PHP and HTML
3. **Service Container**: Dependency injection container
4. **Middleware**: Request/response pipeline
5. **Event System**: Decoupled event handling
6. **Caching Layer**: Redis/Memcached integration
7. **Queue System**: Background job processing

## Testing Strategy

### Manual Testing

Current approach using test.php to verify:
- Autoloading functionality
- Configuration loading
- Database connectivity
- Table structure

### Future Automated Testing

Planned test coverage:
- **Unit Tests**: PHPUnit for models and services
- **Integration Tests**: Database operations
- **Functional Tests**: Authentication flow
- **Security Tests**: SQL injection, XSS prevention

## Performance Considerations

### Current Optimizations

1. **Singleton Database Connection**: Reduces connection overhead
2. **Prepared Statement Caching**: PDO prepares statements once
3. **Optimized Autoloader**: Composer's optimized classmap
4. **Database Indexes**: Fast lookups on frequently queried fields
5. **Static Asset Caching**: Browser caching via .htaccess

### Future Optimizations

1. **Query Caching**: Cache frequent database queries
2. **Lazy Loading**: Load related data only when needed
3. **Connection Pooling**: Persistent database connections
4. **Asset Minification**: Compress CSS/JS files
5. **CDN Integration**: Serve static assets from CDN

## Deployment

### Development Environment

- Local PHP built-in server or XAMPP/WAMP
- Development database
- Error display enabled

### Production Environment

- Apache/Nginx with PHP-FPM
- Production database with backups
- HTTPS enabled
- Error display disabled
- Monitoring and logging configured

## Maintenance

### Regular Tasks

1. **Database Backups**: Automated daily backups
2. **Log Rotation**: Clean up old log files
3. **Token Cleanup**: Remove expired remember tokens
4. **Security Updates**: Keep PHP and dependencies updated
5. **Performance Monitoring**: Track response times

---

© 2025 Synthraxis & Mariusz Sztuczka
