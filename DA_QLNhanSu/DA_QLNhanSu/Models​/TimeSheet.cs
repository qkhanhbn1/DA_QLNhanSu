using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class TimeSheet
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? Workday { get; set; }

    public virtual Employee? IdeNavigation { get; set; }

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();
}
