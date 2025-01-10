using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Overtime
{
    public int Ido { get; set; }

    public int? Ide { get; set; }

    public int? WorkingHours { get; set; }

    public double? SalaryCoeficient { get; set; }

    public decimal? HourlyWage { get; set; }

    public decimal? OvertimePay { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Employee? IdeNavigation { get; set; }

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();
}
