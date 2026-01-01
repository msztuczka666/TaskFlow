# Instrukcja Migracji TaskFlow do Visual Studio Code

## Wstęp

Ten dokument zawiera szczegółową instrukcję krok po kroku, jak zaimportować i uruchomić projekt TaskFlow w Visual Studio Code (VS Code).

**Uwaga:** Visual Studio (IDE dla C#/.NET) i Visual Studio Code (edytor kodu dla wielu języków) to różne produkty. Dla projektów PHP zalecamy **Visual Studio Code**.

---

## Wymagania Wstępne

Przed rozpoczęciem upewnij się, że masz zainstalowane:

1. **Visual Studio Code** - [Pobierz tutaj](https://code.visualstudio.com/)
2. **PHP 7.4 lub nowszy** - [Pobierz tutaj](https://www.php.net/downloads)
3. **Composer** - [Pobierz tutaj](https://getcomposer.org/download/)
4. **XAMPP** lub **WAMP** (zawiera Apache, MySQL/MariaDB, PHP) - [XAMPP](https://www.apachefriends.org/) lub [WAMP](https://www.wampserver.com/)
5. **Git** (opcjonalnie, jeśli korzystasz z kontroli wersji)

---

## Część 1: Instalacja Visual Studio Code

### Krok 1: Pobierz i Zainstaluj VS Code

1. Wejdź na stronę: https://code.visualstudio.com/
2. Kliknij **"Download for Windows"** (lub odpowiedni system)
3. Uruchom pobrany plik instalacyjny
4. Podczas instalacji zaznacz opcje:
   - ✅ "Add to PATH"
   - ✅ "Create a desktop icon"
   - ✅ "Add 'Open with Code' action to Windows Explorer context menu"
5. Kliknij **"Install"** i poczekaj na zakończenie
6. Uruchom Visual Studio Code

### Krok 2: Zainstaluj Rozszerzenia PHP

1. W VS Code kliknij ikonę **"Extensions"** (Rozszerzenia) po lewej stronie (lub naciśnij `Ctrl+Shift+X`)
2. Wyszukaj i zainstaluj następujące rozszerzenia:

   **Obowiązkowe:**
   - **PHP Intelephense** - inteligentne uzupełnianie kodu PHP
   - **PHP Debug** - debugowanie kodu PHP
   - **PHP DocBlocker** - generowanie dokumentacji
   
   **Zalecane:**
   - **phpfmt** - formatowanie kodu PHP
   - **Better Comments** - lepsze komentarze
   - **GitLens** (jeśli używasz Git)
   - **MySQL** - zarządzanie bazą danych
   - **Live Server** - serwer lokalny dla plików

3. Po zainstalowaniu każdego rozszerzenia kliknij **"Reload"** jeśli jest wymagane

---

## Część 2: Instalacja Środowiska PHP (XAMPP)

### Krok 3: Zainstaluj XAMPP

1. Pobierz XAMPP z: https://www.apachefriends.org/
2. Uruchom instalator
3. Wybierz komponenty do instalacji:
   - ✅ Apache
   - ✅ MySQL
   - ✅ PHP
   - ✅ phpMyAdmin
4. Wybierz katalog instalacji (domyślnie `C:\xampp`)
5. Kliknij **"Next"** i poczekaj na instalację
6. Po instalacji uruchom **"XAMPP Control Panel"**

### Krok 4: Uruchom Serwery

1. W XAMPP Control Panel kliknij **"Start"** przy **Apache**
2. Kliknij **"Start"** przy **MySQL**
3. Oba powinny pokazywać zielony status

**Uwaga:** Jeśli porty 80 lub 443 są zajęte (np. przez Skype), zmień port Apache:
- Kliknij **"Config"** → **"Apache (httpd.conf)"**
- Znajdź linię `Listen 80` i zmień na `Listen 8080`
- Zapisz i zrestartuj Apache

---

## Część 3: Import Projektu TaskFlow do VS Code

### Krok 5: Skopiuj Projekt do Katalogu XAMPP

1. Otwórz folder projektu TaskFlow (gdzie masz wszystkie pliki)
2. Skopiuj cały folder **TaskFlow** do:
   ```
   C:\xampp\htdocs\TaskFlow
   ```
3. Struktura powinna wyglądać tak:
   ```
   C:\xampp\htdocs\TaskFlow\
   ├── config\
   ├── database\
   ├── public\
   ├── src\
   ├── composer.json
   ├── README.md
   └── ...
   ```

### Krok 6: Otwórz Projekt w VS Code

1. Uruchom Visual Studio Code
2. Kliknij **"File"** (Plik) → **"Open Folder"** (Otwórz Folder)
3. Przejdź do `C:\xampp\htdocs\TaskFlow`
4. Kliknij **"Select Folder"** (Wybierz Folder)
5. Projekt zostanie otwarty w lewym panelu (Explorer)

### Krok 7: Zainstaluj Zależności Composer

1. W VS Code otwórz terminal:
   - Kliknij **"Terminal"** → **"New Terminal"** (Nowy Terminal)
   - Lub naciśnij `` Ctrl+` ``
2. W terminalu wpisz:
   ```bash
   composer install
   ```
3. Poczekaj, aż Composer zainstaluje wszystkie zależności
4. Powinien pojawić się folder `vendor\`

---

## Część 4: Konfiguracja Bazy Danych

### Krok 8: Utwórz Bazę Danych

1. Otwórz przeglądarkę i wejdź na: http://localhost/phpmyadmin
2. Kliknij zakładkę **"SQL"** u góry
3. Skopiuj całą zawartość pliku `database\schema.sql` z projektu
4. Wklej do okna SQL w phpMyAdmin
5. Kliknij **"Go"** (Wykonaj) na dole
6. Baza danych `taskflow` zostanie utworzona z wszystkimi tabelami

**Alternatywnie - Import pliku:**
1. W phpMyAdmin kliknij **"Import"**
2. Kliknij **"Choose File"** (Wybierz Plik)
3. Wybierz plik `C:\xampp\htdocs\TaskFlow\database\schema.sql`
4. Kliknij **"Go"**

### Krok 9: Skonfiguruj Połączenie z Bazą

1. W VS Code otwórz plik `config\config.php`
2. Upewnij się, że ustawienia są poprawne:
   ```php
   'database' => [
       'host' => 'localhost',
       'port' => 3306,
       'database' => 'taskflow',
       'username' => 'root',
       'password' => '',  // Domyślnie puste w XAMPP
       'charset' => 'utf8mb4',
       'collation' => 'utf8mb4_unicode_ci',
   ],
   ```
3. Zapisz plik (`Ctrl+S`)

---

## Część 5: Konfiguracja Serwera Apache

### Krok 10: Skonfiguruj Virtual Host (Opcjonalnie - Zalecane)

1. Otwórz plik konfiguracji Apache:
   - Ścieżka: `C:\xampp\apache\conf\extra\httpd-vhosts.conf`
   - Lub w XAMPP Control Panel: **Config** → **Apache (httpd-vhosts.conf)**

2. Dodaj na końcu pliku:
   ```apache
   <VirtualHost *:80>
       ServerName taskflow.local
       DocumentRoot "C:/xampp/htdocs/TaskFlow/public"
       
       <Directory "C:/xampp/htdocs/TaskFlow/public">
           Options Indexes FollowSymLinks
           AllowOverride All
           Require all granted
       </Directory>
       
       ErrorLog "logs/taskflow-error.log"
       CustomLog "logs/taskflow-access.log" common
   </VirtualHost>
   ```

3. Zapisz plik

4. Edytuj plik hosts Windows:
   - Otwórz Notatnik **jako Administrator**
   - Otwórz plik: `C:\Windows\System32\drivers\etc\hosts`
   - Dodaj na końcu:
     ```
     127.0.0.1    taskflow.local
     ```
   - Zapisz plik

5. Zrestartuj Apache w XAMPP Control Panel

**Bez Virtual Host:**
Jeśli nie chcesz konfigurować Virtual Host, możesz używać:
```
http://localhost/TaskFlow/public/
```

---

## Część 6: Testowanie Aplikacji

### Krok 11: Przetestuj Instalację

1. W VS Code otwórz terminal i wpisz:
   ```bash
   php test.php
   ```
2. Powinieneś zobaczyć wyniki testów:
   ```
   Test 1: Autoloading... ✓ PASSED
   Test 2: Configuration file... ✓ PASSED
   Test 3: Database connection... ✓ PASSED
   Test 4: Database query... ✓ PASSED (Found 6 tables)
   ```

### Krok 12: Otwórz Aplikację w Przeglądarce

1. Upewnij się, że Apache i MySQL są uruchomione w XAMPP
2. Otwórz przeglądarkę
3. Wejdź na:
   - **Z Virtual Host:** http://taskflow.local
   - **Bez Virtual Host:** http://localhost/TaskFlow/public/

4. Powinna pojawić się strona logowania TaskFlow
5. Zaloguj się używając domyślnych danych:
   - **Email:** admin@taskflow.com
   - **Hasło:** admin123

---

## Część 7: Konfiguracja VS Code dla Projektu

### Krok 13: Utwórz Ustawienia Workspace

1. W VS Code naciśnij `F1` lub `Ctrl+Shift+P`
2. Wpisz: "Preferences: Open Workspace Settings (JSON)"
3. Dodaj następującą konfigurację:
   ```json
   {
       "php.validate.executablePath": "C:/xampp/php/php.exe",
       "php.suggest.basic": true,
       "intelephense.environment.phpVersion": "7.4.0",
       "files.exclude": {
           "**/vendor": true,
           "**/.git": true
       },
       "files.associations": {
           "*.php": "php"
       }
   }
   ```
4. Zapisz plik

### Krok 14: Skonfiguruj Debugowanie (Opcjonalnie)

1. Zainstaluj XDebug:
   - Pobierz z: https://xdebug.org/download
   - Skopiuj plik DLL do `C:\xampp\php\ext\`

2. Edytuj `C:\xampp\php\php.ini`:
   - Znajdź `[XDebug]` lub dodaj na końcu:
     ```ini
     [XDebug]
     zend_extension = "C:\xampp\php\ext\php_xdebug.dll"
     xdebug.mode = debug
     xdebug.start_with_request = yes
     xdebug.client_port = 9003
     ```

3. Zrestartuj Apache

4. W VS Code utwórz `.vscode\launch.json`:
   ```json
   {
       "version": "0.2.0",
       "configurations": [
           {
               "name": "Listen for XDebug",
               "type": "php",
               "request": "launch",
               "port": 9003,
               "pathMappings": {
                   "/": "${workspaceFolder}"
               }
           }
       ]
   }
   ```

---

## Część 8: Przydatne Skróty Klawiszowe w VS Code

| Skrót | Funkcja |
|-------|---------|
| `Ctrl+P` | Szybkie otwieranie plików |
| `Ctrl+Shift+P` | Paleta komend |
| `Ctrl+B` | Pokaż/ukryj panel boczny |
| `` Ctrl+` `` | Otwórz/zamknij terminal |
| `Ctrl+/` | Komentarz/odkomentuj linię |
| `Alt+↑/↓` | Przesuń linię w górę/dół |
| `Shift+Alt+↓` | Duplikuj linię |
| `Ctrl+D` | Zaznacz następne wystąpienie |
| `Ctrl+F` | Znajdź w pliku |
| `Ctrl+H` | Znajdź i zamień |
| `Ctrl+Shift+F` | Znajdź w projekcie |
| `F5` | Start debugowania |
| `Ctrl+S` | Zapisz plik |
| `Ctrl+Shift+S` | Zapisz wszystkie pliki |

---

## Rozwiązywanie Problemów

### Problem: "Composer not found"

**Rozwiązanie:**
1. Sprawdź czy Composer jest zainstalowany: `composer --version` w terminalu
2. Jeśli nie, zainstaluj z: https://getcomposer.org/
3. Dodaj Composer do PATH Windows

### Problem: "Port 80 is already in use"

**Rozwiązanie:**
1. W XAMPP Control Panel kliknij **Config** → **Apache (httpd.conf)**
2. Zmień `Listen 80` na `Listen 8080`
3. Zmień `ServerName localhost:80` na `ServerName localhost:8080`
4. Zapisz i zrestartuj Apache
5. Używaj URL: `http://localhost:8080/TaskFlow/public/`

### Problem: "Access denied for user 'root'@'localhost'"

**Rozwiązanie:**
1. Otwórz phpMyAdmin
2. Sprawdź czy użytkownik `root` istnieje
3. Upewnij się, że hasło w `config\config.php` jest puste (domyślnie w XAMPP)

### Problem: "Class 'TaskFlow\...' not found"

**Rozwiązanie:**
1. Uruchom w terminalu: `composer dump-autoload`
2. Sprawdź czy folder `vendor\` istnieje
3. Sprawdź czy `composer.json` jest poprawny

---

## Dodatkowe Narzędzia

### HeidiSQL - Zarządzanie Bazą Danych

1. Pobierz z: https://www.heidisql.com/
2. Zainstaluj i uruchom
3. Utwórz nowe połączenie:
   - **Host:** localhost
   - **User:** root
   - **Password:** (puste)
   - **Port:** 3306
4. Możesz przeglądać i edytować bazę danych graficznie

### Postman - Testowanie API

1. Pobierz z: https://www.postman.com/
2. Użyj do testowania endpointów API (w przyszłości)

---

## Podsumowanie

Twój projekt TaskFlow jest teraz:
✅ Zaimportowany do Visual Studio Code
✅ Skonfigurowany z XAMPP (Apache + MySQL)
✅ Gotowy do rozwoju
✅ Dostępny w przeglądarce

**Następne kroki:**
1. Zmień domyślne hasło admina
2. Zacznij rozwijać nowe funkcje
3. Używaj Git do kontroli wersji
4. Regularnie twórz kopie zapasowe bazy danych

---

## Przydatne Linki

- **Dokumentacja PHP:** https://www.php.net/manual/pl/
- **Dokumentacja VS Code:** https://code.visualstudio.com/docs
- **Composer:** https://getcomposer.org/doc/
- **MariaDB:** https://mariadb.org/documentation/
- **XAMPP FAQ:** https://www.apachefriends.org/faq.html

---

**Powodzenia w rozwoju projektu TaskFlow!**

© 2025 Synthraxis & Mariusz Sztuczka
