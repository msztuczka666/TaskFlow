using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Helpers;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        public IActionResult Index(int? year, int? month)
        {
            // Default to current month/year if not specified
            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;

            // Validate ranges
            if (selectedMonth < 1) selectedMonth = 1;
            if (selectedMonth > 12) selectedMonth = 12;
            if (selectedYear < 2020) selectedYear = 2020;
            if (selectedYear > 2100) selectedYear = 2100;

            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.MonthName = new DateTime(selectedYear, selectedMonth, 1).ToString("MMMM yyyy", new System.Globalization.CultureInfo("pl-PL"));

            // Get first and last day of month
            var firstDay = new DateTime(selectedYear, selectedMonth, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Get first day of week for the month (Monday = 1, Sunday = 7)
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            if (firstDayOfWeek == 0) firstDayOfWeek = 7; // Sunday

            // Calculate days to show from previous month
            var daysInPreviousMonth = firstDayOfWeek - 1;

            // Build calendar days
            var calendarDays = new List<CalendarDay>();

            // Add days from previous month
            var previousMonth = firstDay.AddMonths(-1);
            var daysInPrevMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            for (int i = daysInPrevMonth - daysInPreviousMonth + 1; i <= daysInPrevMonth; i++)
            {
                calendarDays.Add(new CalendarDay
                {
                    Date = new DateTime(previousMonth.Year, previousMonth.Month, i),
                    IsCurrentMonth = false,
                    IsWeekend = false,
                    IsHoliday = false
                });
            }

            // Add days of current month
            for (int day = 1; day <= lastDay.Day; day++)
            {
                var date = new DateTime(selectedYear, selectedMonth, day);
                var dayOfWeek = date.DayOfWeek;

                calendarDays.Add(new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = true,
                    IsWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday,
                    IsHoliday = PolishHolidays.IsHoliday(date),
                    DayName = date.ToString("dddd", new System.Globalization.CultureInfo("pl-PL"))
                });
            }

            // Add days from next month to complete the grid
            var daysToAdd = 42 - calendarDays.Count; // 6 weeks * 7 days
            var nextMonth = lastDay.AddDays(1);
            for (int i = 1; i <= daysToAdd; i++)
            {
                calendarDays.Add(new CalendarDay
                {
                    Date = new DateTime(nextMonth.Year, nextMonth.Month, i),
                    IsCurrentMonth = false,
                    IsWeekend = false,
                    IsHoliday = false
                });
            }

            return View(calendarDays);
        }
    }

    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }
        public string DayName { get; set; }
    }
}
