using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Position
{
    public int Idp { get; set; }

    public string? Name { get; set; }

    public decimal? DailyWage { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<LeaveJob> LeaveJobs { get; set; } = new List<LeaveJob>();

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();
}
