# TaskFlow - Installation Guide

## Quick Start Installation

Follow these steps to get TaskFlow up and running on your system.

### Prerequisites

Before you begin, ensure you have:

- PHP 7.4 or higher installed
- MariaDB 10.3+ or MySQL 5.7+ installed and running
- Composer installed ([getcomposer.org](https://getcomposer.org))
- A web server (Apache or Nginx)

### Step-by-Step Installation

#### 1. Clone or Download the Repository

```bash
git clone https://github.com/msztuczka666/TaskFlow.git
cd TaskFlow
```

#### 2. Install PHP Dependencies

```bash
composer install
```

This will:
- Install any required dependencies
- Generate the PSR-4 autoloader

#### 3. Configure the Database

Edit the configuration file:

```bash
nano config/config.php
```

Update the database credentials:

```php
'database' => [
    'host' => 'localhost',      // Your database host
    'port' => 3306,             // Your database port
    'database' => 'taskflow',   // Database name
    'username' => 'root',       // Your database username
    'password' => '',           // Your database password
    'charset' => 'utf8mb4',
    'collation' => 'utf8mb4_unicode_ci',
],
```

#### 4. Create and Import the Database

Option A - Using command line:

```bash
mysql -u root -p < database/schema.sql
```

Option B - Using phpMyAdmin:
1. Open phpMyAdmin
2. Click "Import" tab
3. Choose file: `database/schema.sql`
4. Click "Go"

The schema will create:
- Database: `taskflow`
- All necessary tables
- A default admin user

#### 5. Configure Your Web Server

##### For Apache

Create a virtual host configuration:

```apache
<VirtualHost *:80>
    ServerName taskflow.local
    DocumentRoot /path/to/TaskFlow/public
    
    <Directory /path/to/TaskFlow/public>
        AllowOverride All
        Require all granted
        Options -Indexes
    </Directory>
    
    ErrorLog ${APACHE_LOG_DIR}/taskflow-error.log
    CustomLog ${APACHE_LOG_DIR}/taskflow-access.log combined
</VirtualHost>
```

Enable the site and restart Apache:

```bash
sudo a2ensite taskflow
sudo systemctl restart apache2
```

##### For Nginx

Create a server block:

```nginx
server {
    listen 80;
    server_name taskflow.local;
    root /path/to/TaskFlow/public;
    
    index index.php index.html;
    
    location / {
        try_files $uri $uri/ /index.php?$query_string;
    }
    
    location ~ \.php$ {
        fastcgi_pass unix:/var/run/php/php7.4-fpm.sock;
        fastcgi_index index.php;
        fastcgi_param SCRIPT_FILENAME $document_root$fastcgi_script_name;
        include fastcgi_params;
    }
    
    location ~ /\.ht {
        deny all;
    }
}
```

Restart Nginx:

```bash
sudo systemctl restart nginx
```

#### 6. Update Your Hosts File (Optional - for local development)

If using a custom domain like `taskflow.local`:

```bash
sudo nano /etc/hosts
```

Add:
```
127.0.0.1    taskflow.local
```

#### 7. Set File Permissions

```bash
# For Apache
sudo chown -R www-data:www-data /path/to/TaskFlow
sudo chmod -R 755 /path/to/TaskFlow

# For Nginx
sudo chown -R nginx:nginx /path/to/TaskFlow
sudo chmod -R 755 /path/to/TaskFlow
```

#### 8. Test the Installation

Run the test script:

```bash
php test.php
```

You should see:
```
Test 1: Autoloading... ✓ PASSED
Test 2: Configuration file... ✓ PASSED
Test 3: Database connection... ✓ PASSED
Test 4: Database query... ✓ PASSED (Found 6 tables)
```

#### 9. Access the Application

Open your web browser and navigate to:
- `http://taskflow.local` (if using local domain)
- `http://localhost` (if using default)
- `http://your-server-ip` (if on remote server)

#### 10. Login

Use the default admin credentials:
- **Email:** admin@taskflow.com
- **Password:** admin123

⚠️ **Important:** Change the default admin password immediately after first login!

## Troubleshooting

### Issue: "Database connection failed"

**Solution:**
1. Check if MariaDB/MySQL is running: `sudo systemctl status mariadb`
2. Verify database credentials in `config/config.php`
3. Ensure the database has been imported: `mysql -u root -p -e "SHOW DATABASES;"`

### Issue: "404 Not Found" on login page

**Solution:**
1. Ensure your web server document root points to the `public/` directory
2. Check Apache/Nginx error logs for details
3. Verify `.htaccess` is enabled (for Apache)

### Issue: "Autoload not found" errors

**Solution:**
```bash
composer install --optimize-autoloader
```

### Issue: "Permission denied" errors

**Solution:**
```bash
sudo chmod -R 755 /path/to/TaskFlow
sudo chown -R www-data:www-data /path/to/TaskFlow  # For Apache
```

### Issue: Session not working

**Solution:**
1. Check PHP session directory permissions:
   ```bash
   ls -la /var/lib/php/sessions
   sudo chmod 1733 /var/lib/php/sessions
   ```

## Security Recommendations

After installation:

1. **Change the default admin password**
2. **Update `config/config.php` with strong database credentials**
3. **Enable HTTPS** (uncomment HTTPS redirect in `.htaccess`)
4. **Set `display_errors = Off`** in PHP production settings
5. **Regularly backup your database**
6. **Keep PHP and MariaDB/MySQL updated**

## Next Steps

Now that TaskFlow is installed:

1. Explore the dashboard
2. Add employees
3. Create projects
4. Assign tasks
5. Manage absences

For more information, see the main README.md file.

## Getting Help

If you encounter issues:
1. Check the error logs (Apache/Nginx and PHP)
2. Review this installation guide
3. Open an issue on GitHub with error details

---

© 2025 Synthraxis & Mariusz Sztuczka
