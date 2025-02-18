using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class CareerDevelopment
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public string? Type { get; set; }

    public string? Fromrole { get; set; }

    public string? Torole { get; set; }

    public bool? Status { get; set; }

    public DateTime? Date { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
