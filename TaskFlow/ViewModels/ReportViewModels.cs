using System.ComponentModel.DataAnnotations;

namespace TaskFlow.ViewModels
{
    // View Model for Employee Hours Report
    public class EmployeeHoursReportViewModel
    {
        public List<int> SelectedYears { get; set; } = new();
        public List<int> SelectedMonths { get; set; } = new();
        public List<string> SelectedMPKs { get; set; } = new();
        public List<string> SelectedFirmas { get; set; } = new();
        
        public List<EmployeeHoursRow> Data { get; set; } = new();
        
        // Available filter options
        public List<int> AvailableYears { get; set; } = new();
        public List<string> AvailableMPKs { get; set; } = new();
        public List<string> AvailableFirmas { get; set; } = new();
    }

    public class EmployeeHoursRow
    {
        public int PracownikId { get; set; }
        public string PracownikImie { get; set; } = string.Empty;
        public string PracownikNazwisko { get; set; } = string.Empty;
        public string PracownikPelneImie => $"{PracownikImie} {PracownikNazwisko}";
        
        public List<DayHours> Days { get; set; } = new();
    }

    public class DayHours
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public decimal Hours { get; set; }
    }

    // View Model for Order Hours Report
    public class OrderHoursReportViewModel
    {
        public List<int> SelectedYears { get; set; } = new();
        public List<int> SelectedMonths { get; set; } = new();
        public List<string> SelectedMPKs { get; set; } = new();
        public List<string> SelectedFirmas { get; set; } = new();
        
        public List<OrderHoursRow> Data { get; set; } = new();
        
        // Available filter options
        public List<int> AvailableYears { get; set; } = new();
        public List<string> AvailableMPKs { get; set; } = new();
        public List<string> AvailableFirmas { get; set; } = new();
    }

    public class OrderHoursRow
    {
        public int ZlecenieId { get; set; }
        public string NrZlecenia { get; set; } = string.Empty;
        public string OpisZlecenia { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
    }

    // View Model for Vacation Balance Report
    public class VacationBalanceReportViewModel
    {
        public List<int> SelectedYears { get; set; } = new();
        public List<string> SelectedMPKs { get; set; } = new();
        public List<string> SelectedFirmas { get; set; } = new();
        
        public List<VacationBalanceRow> Data { get; set; } = new();
        
        // Available filter options
        public List<int> AvailableYears { get; set; } = new();
        public List<string> AvailableMPKs { get; set; } = new();
        public List<string> AvailableFirmas { get; set; } = new();
    }

    public class VacationBalanceRow
    {
        public int PracownikId { get; set; }
        public string PracownikImie { get; set; } = string.Empty;
        public string PracownikNazwisko { get; set; } = string.Empty;
        public string PracownikPelneImie => $"{PracownikImie} {PracownikNazwisko}";
        
        public int LiczbaDniWolnych { get; set; } // Przysługujące
        public int UrlopWykorzystany { get; set; } // Wykorzystane (urlop wypoczynkowy)
        public int UrlopPozostaly => LiczbaDniWolnych - UrlopWykorzystany; // Pozostałe
    }
}
