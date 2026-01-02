# Instrukcja Wdrożenia TaskFlow na Synology NAS

## Wstęp

Ten dokument zawiera szczegółową instrukcję krok po kroku jak wdrożyć aplikację TaskFlow na serwerze Synology NAS z Web Station, PHP, Apache i MariaDB.

---

## Wymagania Wstępne

Twój Synology NAS musi mieć zainstalowane:
- **Web Station** (serwer WWW)
- **Apache HTTP Server 2.4** lub nowszy
- **PHP 7.4** lub nowszy
- **MariaDB 10** lub **MySQL**
- **phpMyAdmin** (opcjonalnie, do łatwego zarządzania bazą danych)
- Dostęp SSH do NAS (opcjonalnie, ale zalecane)

---

## Część 1: Instalacja Wymaganych Pakietów na Synology

### Krok 1: Zainstaluj Web Station

1. Zaloguj się do **DSM** (DiskStation Manager)
2. Otwórz **Centrum Pakietów**
3. Wyszukaj **"Web Station"**
4. Kliknij **"Zainstaluj"**
5. Poczekaj na zakończenie instalacji

### Krok 2: Zainstaluj Apache HTTP Server

1. W **Centrum Pakietów** wyszukaj **"Apache HTTP Server 2.4"**
2. Kliknij **"Zainstaluj"**
3. Po instalacji Apache będzie dostępny jako backend dla Web Station

### Krok 3: Zainstaluj PHP

1. W **Centrum Pakietów** wyszukaj **"PHP 7.4"** lub **"PHP 8.0"** (zalecane)
2. Kliknij **"Zainstaluj"**
3. Zainstaluj również **"PHP 7.4"** jeśli masz starsze wersje dla kompatybilności

### Krok 4: Zainstaluj MariaDB

1. W **Centrum Pakietów** wyszukaj **"MariaDB 10"**
2. Kliknij **"Zainstaluj"**
3. Podczas instalacji ustaw hasło dla użytkownika **root**
4. **Zapisz to hasło** - będzie potrzebne później!

### Krok 5: Zainstaluj phpMyAdmin (Opcjonalnie)

1. W **Centrum Pakietów** wyszukaj **"phpMyAdmin"**
2. Kliknij **"Zainstaluj"**
3. Po instalacji będzie dostępny pod adresem: `http://twoj-nas:port/phpMyAdmin`

### Krok 6: Włącz SSH (Opcjonalnie, ale zalecane)

1. Otwórz **Panel Sterowania** → **Terminal & SNMP**
2. Zaznacz **"Włącz usługę SSH"**
3. Domyślny port SSH: **22** (możesz zmienić dla bezpieczeństwa)
4. Kliknij **"Zastosuj"**

---

## Część 2: Konfiguracja Web Station

### Krok 7: Skonfiguruj Web Station

1. Otwórz aplikację **Web Station**
2. Przejdź do zakładki **"Ustawienia ogólne"**
3. W sekcji **"Backend serwera HTTP"**:
   - Wybierz **"Apache HTTP Server 2.4"**
4. W sekcji **"Backend serwera PHP"**:
   - Wybierz **"PHP 7.4"** lub **"PHP 8.0"**
5. Kliknij **"Zastosuj"**

### Krok 8: Utwórz Wirtualny Host dla TaskFlow

1. W **Web Station** przejdź do zakładki **"Wirtualny host"**
2. Kliknij **"Utwórz"**
3. Wypełnij formularz:
   - **Nazwa hosta wirtualnego:** `taskflow`
   - **Port:** `80` (HTTP) lub `443` (HTTPS jeśli masz certyfikat)
   - **Główny katalog dokumentów:** `/web/taskflow/public`
   - **Backend HTTP:** `Apache HTTP Server 2.4`
   - **Backend PHP:** `PHP 7.4` lub `PHP 8.0`
4. Kliknij **"OK"**

**Uwaga:** Folder `/web/` to standardowy katalog Web Station w Synology.

---

## Część 3: Pobieranie i Instalacja Projektu TaskFlow

### Krok 9: Pobierz Projekt przez SSH lub File Station

#### Opcja A: Przez SSH (Zalecane)

1. Połącz się z NAS przez SSH:
   ```bash
   ssh admin@adres-ip-nas
   # Podaj hasło administratora
   ```

2. Przejdź do katalogu web:
   ```bash
   cd /volume1/web
   ```

3. Sprawdź czy masz zainstalowany Git:
   ```bash
   git --version
   ```
   
   Jeśli Git nie jest zainstalowany:
   - Zainstaluj pakiet **"Git Server"** z Centrum Pakietów
   - Lub pobierz pliki ręcznie (zobacz Opcja B)

4. Sklonuj repozytorium z poprawnym branch:
   ```bash
   git clone -b copilot/initialize-taskflow-project https://github.com/msztuczka666/TaskFlow.git taskflow
   ```
   
   **WAŻNE:** `-b copilot/initialize-taskflow-project` pobiera branch z plikami PHP!

5. Przejdź do katalogu projektu:
   ```bash
   cd taskflow
   ls -la
   ```
   
   Powinieneś zobaczyć wszystkie 19 plików projektu.

#### Opcja B: Przez File Station (Bez Git)

1. Na swoim komputerze pobierz projekt jako ZIP:
   - Wejdź na: https://github.com/msztuczka666/TaskFlow
   - Przełącz się na branch **"copilot/initialize-taskflow-project"**
   - Kliknij **"Code"** → **"Download ZIP"**

2. Rozpakuj ZIP na swoim komputerze

3. W Synology DSM otwórz **File Station**

4. Przejdź do `/web/`

5. Utwórz nowy folder **"taskflow"**

6. Prześlij wszystkie pliki z rozpakowanego ZIP do `/web/taskflow/`

### Krok 10: Zainstaluj Composer

Composer jest potrzebny do instalacji zależności PHP.

1. Przez SSH połącz się z NAS:
   ```bash
   ssh admin@adres-ip-nas
   ```

2. Pobierz i zainstaluj Composer:
   ```bash
   cd /tmp
   curl -sS https://getcomposer.org/installer | php
   sudo mv composer.phar /usr/local/bin/composer
   sudo chmod +x /usr/local/bin/composer
   ```

3. Sprawdź instalację:
   ```bash
   composer --version
   ```

### Krok 11: Zainstaluj Zależności Projektu

1. Przejdź do katalogu projektu:
   ```bash
   cd /volume1/web/taskflow
   ```

2. Zainstaluj zależności:
   ```bash
   composer install --no-dev --optimize-autoloader
   ```
   
3. Sprawdź czy folder `vendor/` został utworzony:
   ```bash
   ls -la vendor/
   ```

---

## Część 4: Konfiguracja Bazy Danych MariaDB

### Krok 12: Utwórz Bazę Danych przez phpMyAdmin

1. Otwórz phpMyAdmin w przeglądarce:
   ```
   http://adres-ip-nas:port/phpMyAdmin
   ```

2. Zaloguj się:
   - **Użytkownik:** `root`
   - **Hasło:** (hasło ustawione podczas instalacji MariaDB)

3. Kliknij zakładkę **"SQL"** u góry

4. Otwórz plik schema.sql:
   - W File Station przejdź do `/web/taskflow/database/schema.sql`
   - Otwórz plik i skopiuj całą zawartość

5. Wklej zawartość do okna SQL w phpMyAdmin

6. Kliknij **"Wykonaj"** (lub **"Go"**)

7. Sprawdź czy baza danych została utworzona:
   - W lewym panelu powinna pojawić się baza **"taskflow"**
   - Powinna zawierać 6 tabel: `users`, `remember_tokens`, `employees`, `projects`, `tasks`, `absences`

### Krok 13: (Alternatywnie) Utwórz Bazę przez SSH

1. Połącz się z MariaDB przez SSH:
   ```bash
   mysql -u root -p
   ```

2. Podaj hasło root

3. Importuj bazę danych:
   ```bash
   mysql -u root -p < /volume1/web/taskflow/database/schema.sql
   ```

4. Sprawdź czy baza została utworzona:
   ```bash
   mysql -u root -p -e "SHOW DATABASES;"
   mysql -u root -p -e "USE taskflow; SHOW TABLES;"
   ```

---

## Część 5: Konfiguracja Aplikacji

### Krok 14: Utwórz Plik Konfiguracyjny

1. Przez SSH lub File Station przejdź do `/volume1/web/taskflow/config/`

2. Skopiuj plik przykładowej konfiguracji:
   ```bash
   cd /volume1/web/taskflow/config
   cp config.example.php config.php
   ```

3. Edytuj plik `config.php`:
   ```bash
   nano config.php
   # Lub przez File Station w DSM
   ```

4. Zaktualizuj dane dostępu do bazy danych:
   ```php
   'database' => [
       'host' => 'localhost',        // Lub '127.0.0.1'
       'port' => 3306,                // Domyślny port MariaDB
       'database' => 'taskflow',
       'username' => 'root',          // Lub dedykowany użytkownik
       'password' => 'twoje-haslo',   // Hasło root MariaDB
       'charset' => 'utf8mb4',
       'collation' => 'utf8mb4_unicode_ci',
   ],
   ```

5. Zapisz plik

### Krok 15: Ustaw Uprawnienia do Plików

1. Przez SSH ustaw właściciela plików na użytkownika HTTP:
   ```bash
   sudo chown -R http:http /volume1/web/taskflow
   ```

2. Ustaw odpowiednie uprawnienia:
   ```bash
   sudo chmod -R 755 /volume1/web/taskflow
   sudo chmod -R 775 /volume1/web/taskflow/config
   ```

---

## Część 6: Konfiguracja Apache

### Krok 16: Skonfiguruj .htaccess

Plik `.htaccess` już istnieje w projekcie (`/public/.htaccess`), ale sprawdź czy jest poprawny:

1. Otwórz `/volume1/web/taskflow/public/.htaccess`

2. Upewnij się, że zawiera podstawową konfigurację:
   ```apache
   # TaskFlow .htaccess
   Options -Indexes
   DirectoryIndex index.php
   
   <IfModule mod_rewrite.c>
       RewriteEngine On
   </IfModule>
   ```

### Krok 17: Konfiguracja Virtual Host Apache (Zaawansowane)

Jeśli chcesz niestandardową konfigurację:

1. Przez SSH edytuj konfigurację Apache:
   ```bash
   sudo nano /usr/local/etc/apache24/sites-enabled/taskflow.conf
   ```

2. Dodaj konfigurację:
   ```apache
   <VirtualHost *:80>
       ServerName taskflow.local
       DocumentRoot /volume1/web/taskflow/public
       
       <Directory /volume1/web/taskflow/public>
           Options -Indexes +FollowSymLinks
           AllowOverride All
           Require all granted
       </Directory>
       
       ErrorLog /var/log/taskflow-error.log
       CustomLog /var/log/taskflow-access.log combined
   </VirtualHost>
   ```

3. Zrestartuj Apache:
   ```bash
   sudo synoservicectl --restart apache2.4
   ```

---

## Część 7: Testowanie Instalacji

### Krok 18: Test Podstawowy

1. Otwórz przeglądarkę

2. Wejdź na adres:
   ```
   http://adres-ip-nas/taskflow/public/
   ```
   
   Lub jeśli skonfigurowałeś Virtual Host:
   ```
   http://taskflow.local
   ```

3. Powinna pojawić się strona logowania TaskFlow

### Krok 19: Test Połączenia z Bazą Danych

1. Przez SSH uruchom skrypt testowy:
   ```bash
   cd /volume1/web/taskflow
   php test.php
   ```

2. Powinieneś zobaczyć:
   ```
   Test 1: Autoloading... ✓ PASSED
   Test 2: Configuration file... ✓ PASSED
   Test 3: Database connection... ✓ PASSED
   Test 4: Database query... ✓ PASSED (Found 6 tables)
   ```

### Krok 20: Test Logowania

1. W przeglądarce przejdź do strony logowania

2. Użyj domyślnych danych:
   - **Email:** `admin@taskflow.com`
   - **Hasło:** `admin123`

3. Kliknij **"Login"**

4. Powinieneś zostać przekierowany do dashboardu

---

## Część 8: Konfiguracja HTTPS (Opcjonalnie, ale zalecane)

### Krok 21: Włącz Certyfikat SSL

1. W DSM przejdź do **Panel Sterowania** → **Bezpieczeństwo** → **Certyfikat**

2. Dodaj certyfikat:
   - **Let's Encrypt** (darmowy, automatyczne odnawianie)
   - Lub importuj własny certyfikat

3. Przypisz certyfikat do wirtualnego hosta TaskFlow:
   - **Panel Sterowania** → **Usługi sieciowe** → **Web Station**
   - Wybierz wirtualny host **taskflow**
   - Włącz **HTTPS**
   - Wybierz certyfikat

4. Zaktualizuj `config.php`:
   ```php
   'session' => [
       'secure' => true,  // Zmień z false na true
   ],
   'app' => [
       'url' => 'https://twoj-nas-adres',  // Zmień http na https
   ]
   ```

5. Odkomentuj przekierowanie HTTPS w `.htaccess`:
   ```apache
   RewriteCond %{HTTPS} off
   RewriteRule ^(.*)$ https://%{HTTP_HOST}%{REQUEST_URI} [L,R=301]
   ```

---

## Część 9: Zaawansowana Konfiguracja

### Krok 22: Tworzenie Dedykowanego Użytkownika Bazy Danych

Dla bezpieczeństwa utwórz dedykowanego użytkownika zamiast używać root:

1. Zaloguj się do MariaDB:
   ```bash
   mysql -u root -p
   ```

2. Utwórz użytkownika:
   ```sql
   CREATE USER 'taskflow_user'@'localhost' IDENTIFIED BY 'silne_haslo_123';
   GRANT ALL PRIVILEGES ON taskflow.* TO 'taskflow_user'@'localhost';
   FLUSH PRIVILEGES;
   EXIT;
   ```

3. Zaktualizuj `config/config.php`:
   ```php
   'database' => [
       'username' => 'taskflow_user',
       'password' => 'silne_haslo_123',
   ],
   ```

### Krok 23: Konfiguracja Automatycznych Kopii Zapasowych

1. Utwórz skrypt backupu:
   ```bash
   sudo nano /usr/local/bin/backup-taskflow.sh
   ```

2. Dodaj zawartość:
   ```bash
   #!/bin/bash
   # Backup TaskFlow database
   BACKUP_DIR="/volume1/backups/taskflow"
   DATE=$(date +%Y%m%d_%H%M%S)
   
   mkdir -p $BACKUP_DIR
   mysqldump -u root -p'twoje-haslo' taskflow > $BACKUP_DIR/taskflow_$DATE.sql
   
   # Usuń backupy starsze niż 30 dni
   find $BACKUP_DIR -name "*.sql" -mtime +30 -delete
   ```

3. Nadaj uprawnienia:
   ```bash
   sudo chmod +x /usr/local/bin/backup-taskflow.sh
   ```

4. Dodaj do crona (harmonogram zadań):
   - W DSM: **Panel Sterowania** → **Harmonogram zadań**
   - Utwórz nowe zadanie: **Użytkownik zdefiniowany skrypt**
   - Uruchamiaj codziennie o 2:00 w nocy
   - Skrypt: `/usr/local/bin/backup-taskflow.sh`

### Krok 24: Konfiguracja PHP dla Lepszej Wydajności

1. Edytuj ustawienia PHP w Web Station:
   - **Web Station** → **Ustawienia PHP**
   - Wybierz profil PHP używany przez TaskFlow

2. Zaktualizuj wartości:
   ```ini
   memory_limit = 256M
   upload_max_filesize = 10M
   post_max_size = 10M
   max_execution_time = 300
   max_input_time = 300
   ```

3. Włącz OPcache dla lepszej wydajności:
   ```ini
   opcache.enable=1
   opcache.memory_consumption=128
   opcache.max_accelerated_files=10000
   ```

---

## Część 10: Bezpieczeństwo

### Krok 25: Zabezpieczenie Instalacji

1. **Zmień domyślne hasło admina** natychmiast po pierwszym logowaniu

2. **Wyłącz dostęp do plików wrażliwych** w `.htaccess`:
   ```apache
   <FilesMatch "^(composer\.json|composer\.lock|\.git)">
       Require all denied
   </FilesMatch>
   ```

3. **Skonfiguruj firewall** w DSM:
   - **Panel Sterowania** → **Bezpieczeństwo** → **Firewall**
   - Zezwól tylko na porty: 80 (HTTP), 443 (HTTPS), 22 (SSH)

4. **Włącz Auto Block** w DSM:
   - **Panel Sterowania** → **Bezpieczeństwo** → **Ochrona**
   - Włącz automatyczne blokowanie po 5 nieudanych próbach logowania

5. **Regularnie aktualizuj** DSM, PHP, Apache, MariaDB

---

## Rozwiązywanie Problemów

### Problem: Brak połączenia z bazą danych

**Rozwiązanie:**
1. Sprawdź czy MariaDB działa:
   ```bash
   sudo synoservicectl --status mariadb10
   ```
2. Sprawdź hasło w `config/config.php`
3. Sprawdź czy użytkownik ma uprawnienia:
   ```sql
   SHOW GRANTS FOR 'root'@'localhost';
   ```

### Problem: 500 Internal Server Error

**Rozwiązanie:**
1. Sprawdź logi Apache:
   ```bash
   sudo tail -f /var/log/httpd/error_log
   ```
2. Sprawdź uprawnienia do plików
3. Sprawdź czy `.htaccess` jest włączony w Apache

### Problem: "Class not found" errors

**Rozwiązanie:**
1. Sprawdź czy folder `vendor/` istnieje
2. Uruchom ponownie:
   ```bash
   composer install --optimize-autoloader
   ```

### Problem: Strona się nie ładuje

**Rozwiązanie:**
1. Sprawdź czy Web Station używa Apache 2.4
2. Sprawdź konfigurację Virtual Host
3. Upewnij się, że DocumentRoot wskazuje na `/volume1/web/taskflow/public`

---

## Często Zadawane Pytania (FAQ)

**Q: Czy mogę używać innego portu niż 80?**
A: Tak, skonfiguruj to w Web Station → Virtual Host → Port.

**Q: Jak zmienić hasło root MariaDB?**
A: Użyj komendy: `mysqladmin -u root -p password nowe_haslo`

**Q: Czy mogę mieć wiele aplikacji PHP na jednym NAS?**
A: Tak, utwórz osobne Virtual Hosts dla każdej aplikacji.

**Q: Jak dostosować adres URL?**
A: Skonfiguruj własną domenę i wskaż ją na IP Twojego NAS, następnie ustaw Virtual Host.

---

## Podsumowanie

Po wykonaniu wszystkich kroków Twój TaskFlow powinien być:
✅ Zainstalowany na Synology NAS
✅ Działający z Apache, PHP i MariaDB
✅ Dostępny przez przeglądarkę
✅ Zabezpieczony z HTTPS
✅ Regularnie backupowany

**Dostęp do aplikacji:**
```
http://adres-ip-nas/taskflow/public/
lub
https://twoja-domena.com
```

**Login:**
- Email: admin@taskflow.com
- Hasło: admin123 (ZMIEŃ TO!)

---

## Przydatne Komendy

```bash
# Restart Apache
sudo synoservicectl --restart apache2.4

# Restart MariaDB
sudo synoservicectl --restart mariadb10

# Restart PHP-FPM
sudo synoservicectl --restart php-fpm-7.4

# Sprawdź logi Apache
sudo tail -f /var/log/httpd/error_log

# Sprawdź logi MariaDB
sudo tail -f /var/log/mariadb10/mariadb.log

# Test połączenia z bazą
php /volume1/web/taskflow/test.php
```

---

**Powodzenia w korzystaniu z TaskFlow na Synology!**

© 2025 Synthraxis & Mariusz Sztuczka
