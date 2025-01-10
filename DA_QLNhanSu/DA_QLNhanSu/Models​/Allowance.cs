using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Allowance
{
    public int Ida { get; set; }

    public string? Name { get; set; }

    public decimal? Money { get; set; }

    public virtual ICollection<EmployeeAllowance> EmployeeAllowances { get; set; } = new List<EmployeeAllowance>();
}
