# Instrukcja dodania logo TaskFlow

## Twoje logo jest gotowe do użycia!

Zaktualizowałem kod, aby logo było wyświetlane w:
- ✅ Nagłówku aplikacji (lewy górny róg)
- ✅ Stronie logowania (nad formularzem)

## Jak dodać plik logo:

### Opcja 1: Bezpośrednie kopiowanie (ZALECANE)

1. **Pobierz logo z Google Drive:**
   - Otwórz link: https://drive.google.com/file/d/1gwWd2gYk4DLX3CnWlYipX7sa2FGzBw4S/view?usp=sharing
   - Kliknij "Pobierz" (ikona strzałki w dół)
   - Zapisz plik na swoim komputerze

2. **Skopiuj do projektu:**
   ```
   # Windows (w Eksploratorze plików):
   Skopiuj pobrany plik do: TaskFlow\wwwroot\images\logo.png
   
   # Lub w terminalu/PowerShell:
   copy C:\Users\TWOJE_IMIĘ\Downloads\logo.png TaskFlow\wwwroot\images\logo.png
   ```

3. **Gotowe!** Logo pojawi się automatycznie po uruchomieniu aplikacji.

### Opcja 2: Visual Studio

1. W Visual Studio, w Solution Explorer:
2. Kliknij prawym na folder `wwwroot\images`
3. Wybierz "Add" → "Existing Item..."
4. Wskaż pobrany plik logo
5. Upewnij się, że nazywa się `logo.png`

### Opcja 3: Jeśli logo ma inną nazwę

Jeśli Twój plik ma inną nazwę (np. `TaskFlow_logo.png`), możesz:
- Zmienić nazwę pliku na `logo.png`, LUB
- Zmienić ścieżki w kodzie:
  - `Views/Shared/_Layout.cshtml` - linia 17
  - `Views/Account/Login.cshtml` - linia 50

## Weryfikacja

Po dodaniu logo:
1. Uruchom aplikację (F5 w Visual Studio)
2. Sprawdź stronę logowania - logo powinno być widoczne nad formularzem
3. Zaloguj się - logo powinno być w lewym górnym rogu

## Dostosowanie rozmiaru

Jeśli logo jest za duże lub za małe, możesz zmienić rozmiar w:
- **Nagłówek**: `_Layout.cshtml` - zmień `height: 40px`
- **Login**: `Login.cshtml` - zmień `height: 80px`

## Problem?

Jeśli logo się nie wyświetla:
1. Sprawdź czy plik znajduje się dokładnie w: `TaskFlow/wwwroot/images/logo.png`
2. Sprawdź wielkość liter - nazwa musi być dokładnie `logo.png`
3. Odśwież cache przeglądarki (Ctrl+F5)
4. Sprawdź konsolę przeglądarki (F12) czy są błędy
