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

    // View Model for Order Settlement Report (Rozliczanie zlecenia)
    public class OrderSettlementReportViewModel
    {
        public List<int> SelectedYears { get; set; } = new();
        public List<int> SelectedMonths { get; set; } = new();
        public List<string> SelectedMPKs { get; set; } = new();
        public List<string> SelectedFirmas { get; set; } = new();
        public List<string> SelectedOrderNumbers { get; set; } = new();
        
        public List<OrderSettlementRow> Data { get; set; } = new();
        
        // Available filter options
        public List<int> AvailableYears { get; set; } = new();
        public List<string> AvailableMPKs { get; set; } = new();
        public List<string> AvailableFirmas { get; set; } = new();
        public List<string> AvailableOrderNumbers { get; set; } = new();
    }

    public class OrderSettlementRow
    {
        public int ZlecenieId { get; set; }
        public string NrZlecenia { get; set; } = string.Empty;
        public string OpisZlecenia { get; set; } = string.Empty;
        
        public List<WorkDescriptionGroup> WorkGroups { get; set; } = new();
        
        public decimal TotalHours => WorkGroups.Sum(wg => wg.TotalHours);
        public decimal TotalCost => WorkGroups.Sum(wg => wg.TotalCost);
    }

    public class WorkDescriptionGroup
    {
        public string OpisPrac { get; set; } = string.Empty;
        public string DisplayOpisPrac => string.IsNullOrWhiteSpace(OpisPrac) ? "(Brak opisu)" : OpisPrac;
        
        public List<EmployeeWorkDetail> EmployeeDetails { get; set; } = new();
        
        public decimal TotalHours => EmployeeDetails.Sum(ed => ed.Hours);
        public decimal TotalCost => EmployeeDetails.Sum(ed => ed.Cost);
    }

    public class EmployeeWorkDetail
    {
        public int PracownikId { get; set; }
        public string PracownikImie { get; set; } = string.Empty;
        public string PracownikNazwisko { get; set; } = string.Empty;
        public string PracownikPelneImie => $"{PracownikImie} {PracownikNazwisko}";
        
        public decimal Hours { get; set; }
        public decimal StawkaZlH { get; set; }
        public decimal Cost => Hours * StawkaZlH;
    }

    // View Model for Employee Overtime Report (Ilość nadgodzin pracowników)
    public class EmployeeOvertimeReportViewModel
    {
        public List<int> SelectedYears { get; set; } = new();
        public List<int> SelectedMonths { get; set; } = new();
        public List<string> SelectedMPKs { get; set; } = new();
        public List<string> SelectedFirmas { get; set; } = new();
        
        public List<EmployeeOvertimeRow> Data { get; set; } = new();
        
        // Available filter options
        public List<int> AvailableYears { get; set; } = new();
        public List<string> AvailableMPKs { get; set; } = new();
        public List<string> AvailableFirmas { get; set; } = new();
    }

    public class EmployeeOvertimeRow
    {
        public int PracownikId { get; set; }
        public string PracownikImie { get; set; } = string.Empty;
        public string PracownikNazwisko { get; set; } = string.Empty;
        public string PracownikPelneImie => $"{PracownikImie} {PracownikNazwisko}";
        
        public List<DayOvertime> Days { get; set; } = new();
    }

    public class DayOvertime
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public decimal StandardHours { get; set; } // Godziny bez mnożnika
        public decimal Multiplier { get; set; } // Mnożnik (1.5 lub 2.0)
        public decimal OvertimeHours => StandardHours * Multiplier; // Godziny z mnożnikiem
        public string TimeZone { get; set; } = string.Empty; // Strefa czasowa
    }
}
