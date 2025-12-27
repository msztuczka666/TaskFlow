# Jak zaktualizować bazę danych TaskFlow

## Problem
Po wprowadzeniu zmian w modelach (dodanie nowych pól do Pracowników, Nieobecności, Zleceń), stare dane w bazie nie odzwierciedlają nowych pól.

## Rozwiązanie

### Opcja 1: Zastosuj migracje (Zalecane - zachowuje dane)

**W Visual Studio - Package Manager Console:**
```powershell
dotnet ef database update
```

**W CMD/PowerShell (w katalogu TaskFlow):**
```cmd
cd TaskFlow
dotnet ef database update
```

To zastosuje migrację `20251227203742_AddEwidencjaCzasuAndUpdateModels` która dodaje:
- Nowe pola do tabeli Pracownicy (Firma, NrPrzepustki, MPK, FirmaId, Stanowisko, SEPNr, SEPNapiecie, LiczbaDniWolnych, LiczbaDniNaZeszycie, LiczbaDniWykorzystanych, TypPracownika, Informacje)
- Pole LiczbaGodzin do tabeli Nieobecnosci
- Nową tabelę EwidencjaCzasu

### Opcja 2: Usuń i utwórz bazę od nowa (Traci wszystkie dane!)

**UWAGA: To usunie wszystkich pracowników, nieobecności i zlecenia!**

```cmd
# Zatrzymaj aplikację jeśli jest uruchomiona
# Usuń plik bazy danych
del taskflow.db

# Utwórz bazę od nowa
dotnet ef database update

# Uruchom aplikację - utworzy się nowy użytkownik admin
dotnet run
```

### Opcja 3: Ręczne sprawdzenie co zostało zaktualizowane

Możesz sprawdzić strukturę bazy danych używając narzędzia SQLite Browser:
1. Pobierz DB Browser for SQLite: https://sqlitebrowser.org/
2. Otwórz plik `taskflow.db` z katalogu TaskFlow
3. Zobacz zakładkę "Database Structure" - powinieneś zobaczyć wszystkie nowe kolumny

## Jak sprawdzić czy migracja została zastosowana?

**W Package Manager Console:**
```powershell
dotnet ef migrations list
```

Powinieneś zobaczyć obie migracje z gwiazdką (*) przy zastosowanych:
- 20251227174046_InitialCreate
- 20251227203742_AddEwidencjaCzasuAndUpdateModels *

## Co zrobić jeśli nadal widzisz stare dane?

1. **Wyczyść cache przeglądarki** - naciśnij Ctrl+F5 w przeglądarce
2. **Zatrzymaj i uruchom aplikację ponownie**
3. **Sprawdź czy używasz właściwego brancha:**
   ```
   git branch
   ```
   Powinieneś być na: `copilot/build-taskflow-project-structure`

## Nowe pola w formularzach

Po zastosowaniu migracji, formularze dla Pracowników będą zawierać:
- Firma (główna)
- Nr przepustki
- MPK
- Firma Id (identyfikatorowa)
- Stanowisko
- SEP nr
- SEP ważne do
- SEP Napięcie
- Liczba dni wolnych
- Liczba dni na zeszycie
- Liczba dni wykorzystanych
- Typ pracownika (Nadzór/Pracownik)
- Informacje (notatki)

## Wsparcie

Jeśli nadal masz problemy:
1. Skopiuj komunikaty błędów z konsoli
2. Dodaj komentarz do PR z opisem problemu
3. Dołącz zrzut ekranu jeśli możliwe
