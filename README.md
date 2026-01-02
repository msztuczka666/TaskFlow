# TaskFlow

Employee & Task Management System

## Overview

TaskFlow is a modern web-based system for managing employees, projects, tasks, and absences. Built with PHP and MariaDB, it features a clean, responsive UI and a robust backend architecture.

## Features

- **User Authentication**: Secure login/logout with password hashing (bcrypt)
- **Remember Me**: Persistent sessions with secure token-based authentication
- **Normalized Database**: Well-structured schema for users, employees, projects, tasks, and absences
- **Modern UI**: Responsive design with gradient backgrounds and smooth animations
- **PSR-4 Autoloading**: Professional code organization with Composer
- **Session Management**: Secure session handling with regeneration and proper cookie configuration

## Requirements

- PHP 7.4 or higher
- MariaDB 10.3+ or MySQL 5.7+
- Composer
- Web server (Apache/Nginx)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/msztuczka666/TaskFlow.git
cd TaskFlow
```

### 2. Install Dependencies

```bash
composer install
```

### 3. Configure Database

Copy the example configuration file:

```bash
cp config/config.example.php config/config.php
```

Edit `config/config.php` and update the database credentials:

```php
'database' => [
    'host' => 'localhost',
    'port' => 3306,
    'database' => 'taskflow',
    'username' => 'your_username',
    'password' => 'your_password',
    'charset' => 'utf8mb4',
    'collation' => 'utf8mb4_unicode_ci',
]
```

### 4. Create Database

Import the database schema:

```bash
mysql -u your_username -p < database/schema.sql
```

This will create:
- Database: `taskflow`
- Tables: `users`, `remember_tokens`, `employees`, `projects`, `tasks`, `absences`
- Default admin user: `admin@taskflow.com` / `admin123`

### 5. Configure Web Server

#### Apache

Point your document root to the `public/` directory. Example virtual host:

```apache
<VirtualHost *:80>
    ServerName taskflow.local
    DocumentRoot /path/to/TaskFlow/public
    
    <Directory /path/to/TaskFlow/public>
        AllowOverride All
        Require all granted
    </Directory>
</VirtualHost>
```

#### Nginx

```nginx
server {
    listen 80;
    server_name taskflow.local;
    root /path/to/TaskFlow/public;
    
    index index.php;
    
    location / {
        try_files $uri $uri/ /index.php?$query_string;
    }
    
    location ~ \.php$ {
        fastcgi_pass unix:/var/run/php/php7.4-fpm.sock;
        fastcgi_index index.php;
        fastcgi_param SCRIPT_FILENAME $document_root$fastcgi_script_name;
        include fastcgi_params;
    }
}
```

### 6. Set Permissions

```bash
chmod -R 755 public/
```

## Usage

### Login

1. Navigate to `http://taskflow.local` (or your configured domain)
2. Use the default credentials:
   - Email: `admin@taskflow.com`
   - Password: `admin123`
3. Check "Remember me" for persistent login (30 days)

### Logout

Click the "Logout" button on the dashboard to end your session.

## Project Structure

```
TaskFlow/
├── config/              # Configuration files
│   ├── config.php       # Active configuration (not in git)
│   └── config.example.php
├── database/            # Database schema and migrations
│   └── schema.sql
├── public/              # Web-accessible files
│   ├── assets/
│   │   ├── css/
│   │   │   └── style.css
│   │   └── js/
│   │       └── login.js
│   ├── index.php        # Login page
│   ├── login_process.php
│   ├── logout.php
│   └── dashboard.php
├── src/                 # Application source code
│   ├── Controllers/
│   │   └── AuthController.php
│   ├── Models/
│   │   └── User.php
│   └── Services/
│       ├── Database.php
│       └── Session.php
├── vendor/              # Composer dependencies (auto-generated)
├── .gitignore
├── composer.json
└── README.md
```

## Database Schema

### Users Table
- Authentication and authorization
- Password hashing with bcrypt
- Role-based access (admin, manager, employee)

### Remember Tokens Table
- Persistent authentication tokens
- Automatic expiration handling

### Employees Table
- Employee information
- Optional link to user accounts
- Status tracking (active, inactive, terminated)

### Projects Table
- Project management
- Status tracking
- Budget management
- Manager assignment

### Tasks Table
- Task assignment and tracking
- Priority levels
- Status management
- Time tracking (estimated vs actual hours)

### Absences Table
- Employee absence tracking
- Multiple absence types
- Approval workflow

## Security Features

- **Password Hashing**: All passwords are hashed using bcrypt (PASSWORD_DEFAULT)
- **Prepared Statements**: All database queries use PDO prepared statements to prevent SQL injection
- **Session Security**: Secure session configuration with httponly cookies and session regeneration
- **CSRF Protection**: Ready for CSRF token implementation in future iterations
- **Input Validation**: Email and password validation on both client and server side

## Future Enhancements

- Task creation and management interface
- Employee CRUD operations
- Project management dashboard
- Absence request and approval system
- Reporting and analytics
- Email notifications
- API endpoints
- Export functionality (PDF, CSV)

## Credits

© 2025 Synthraxis & Mariusz Sztuczka

## License

MIT License
