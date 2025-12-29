using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskFlow.Helpers
{
    public static class PolishHolidays
    {
        // Lista polskich świąt państwowych (dni wolne od pracy)
        public static List<DateTime> GetHolidays(int year)
        {
            var holidays = new List<DateTime>
            {
                // Stałe święta
                new DateTime(year, 1, 1),   // Nowy Rok
                new DateTime(year, 1, 6),   // Trzech Króli
                new DateTime(year, 5, 1),   // Święto Pracy
                new DateTime(year, 5, 3),   // Święto Konstytucji 3 Maja
                new DateTime(year, 8, 15),  // Wniebowzięcie NMP / Święto Wojska Polskiego
                new DateTime(year, 11, 1),  // Wszystkich Świętych
                new DateTime(year, 11, 11), // Narodowe Święto Niepodległości
                new DateTime(year, 12, 25), // Boże Narodzenie (pierwszy dzień)
                new DateTime(year, 12, 26)  // Boże Narodzenie (drugi dzień)
            };

            // Od 1 stycznia 2025 Wigilia (24 grudnia) jest świętem i dniem wolnym od pracy
            if (year >= 2025)
            {
                holidays.Add(new DateTime(year, 12, 24)); // Wigilia
            }

            // Święta ruchome (zależne od Wielkanocy)
            var easter = CalculateEaster(year);
            holidays.Add(easter);                          // Wielkanoc (niedziela)
            holidays.Add(easter.AddDays(1));               // Poniedziałek Wielkanocny
            holidays.Add(easter.AddDays(49));              // Zielone Świątki (niedziela)
            holidays.Add(easter.AddDays(60));              // Boże Ciało (czwartek)

            return holidays.OrderBy(d => d).ToList();
        }

        // Algorytm Meeusa do obliczania daty Wielkanocy
        private static DateTime CalculateEaster(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;
            
            return new DateTime(year, month, day);
        }

        public static bool IsHoliday(DateTime date)
        {
            var holidays = GetHolidays(date.Year);
            return holidays.Any(h => h.Date == date.Date);
        }
    }

    public static class OvertimeCalculator
    {
        // Oblicza mnożnik nadgodzin na podstawie daty i godzin pracy
        public static OvertimeDetails CalculateOvertimeMultiplier(DateTime date, TimeSpan startTime, TimeSpan endTime, decimal hours)
        {
            var dayOfWeek = date.DayOfWeek;
            var isHoliday = PolishHolidays.IsHoliday(date);

            // Niedziela lub święto: całość × 2.0
            if (dayOfWeek == DayOfWeek.Sunday || isHoliday)
            {
                return new OvertimeDetails
                {
                    StandardHours = hours,
                    Multiplier = 2.0m,
                    TimeZone = dayOfWeek == DayOfWeek.Sunday ? "Niedziela" : "Święto",
                    IsWorkOnSundayOrHoliday = true
                };
            }

            // Sobota: całość × 1.5
            if (dayOfWeek == DayOfWeek.Saturday)
            {
                return new OvertimeDetails
                {
                    StandardHours = hours,
                    Multiplier = 1.5m,
                    TimeZone = "Sobota",
                    IsWorkOnSundayOrHoliday = false
                };
            }

            // Dni powszednie (poniedziałek-piątek): strefy czasowe
            // Dla uproszczenia: jeśli praca rozpoczyna się:
            // - 6:30-14:30: standardowe (×1.0)
            // - 14:30-21:00: ×1.5
            // - 21:00-6:30: ×2.0

            var start630 = new TimeSpan(6, 30, 0);
            var end1430 = new TimeSpan(14, 30, 0);
            var end2100 = new TimeSpan(21, 0, 0);

            if (startTime >= start630 && startTime < end1430)
            {
                // Strefa standardowa
                return new OvertimeDetails
                {
                    StandardHours = hours,
                    Multiplier = 1.0m,
                    TimeZone = "6:30-14:30 (standardowe)",
                    IsWorkOnSundayOrHoliday = false
                };
            }
            else if (startTime >= end1430 && startTime < end2100)
            {
                // Strefa nadgodzin 1.5x
                return new OvertimeDetails
                {
                    StandardHours = hours,
                    Multiplier = 1.5m,
                    TimeZone = "14:30-21:00 (nadgodziny 1.5x)",
                    IsWorkOnSundayOrHoliday = false
                };
            }
            else
            {
                // Strefa nocna 2.0x (21:00-6:30)
                return new OvertimeDetails
                {
                    StandardHours = hours,
                    Multiplier = 2.0m,
                    TimeZone = "21:00-6:30 (nadgodziny nocne 2.0x)",
                    IsWorkOnSundayOrHoliday = false
                };
            }
        }

        // Sprawdza czy dana godzina rozpoczęcia pracy kwalifikuje się jako nadgodziny
        public static bool IsOvertime(DateTime date, TimeSpan startTime)
        {
            var dayOfWeek = date.DayOfWeek;
            var isHoliday = PolishHolidays.IsHoliday(date);

            // Sobota, niedziela lub święto - zawsze nadgodziny
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday || isHoliday)
            {
                return true;
            }

            // Dni powszednie: nadgodziny jeśli nie w strefie standardowej (6:30-14:30)
            // ZAWSZE zwracamy true dla dni powszednich, aby pokazać również nadgodziny w strefach 14:30-21:00 i 21:00-6:30
            // Filtrowanie według mnożnika != 1.0 jest w kontrolerze
            return true;
        }
    }

    public class OvertimeDetails
    {
        public decimal StandardHours { get; set; }
        public decimal Multiplier { get; set; }
        public string TimeZone { get; set; } = string.Empty;
        public bool IsWorkOnSundayOrHoliday { get; set; }
        public decimal OvertimeHours => StandardHours * Multiplier;
    }
}
