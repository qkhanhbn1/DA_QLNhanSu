using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Account
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? Idrole { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Role? IdroleNavigation { get; set; }
}
