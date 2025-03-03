using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class EmployeeAllowance
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public int? IdAllowances { get; set; }

    public decimal? Money { get; set; }

    public virtual Allowance? IdAllowancesNavigation { get; set; }

    public virtual Employee? IdeNavigation { get; set; }

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();
}
