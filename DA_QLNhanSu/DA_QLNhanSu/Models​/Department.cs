using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Department
{
    public int Idd { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
