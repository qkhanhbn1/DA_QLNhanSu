using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class SalaryHistory
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public decimal? Salary { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public string? Note { get; set; }

    public virtual Employee? IdeNavigation { get; set; }

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();
}
