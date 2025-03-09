using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Discipline
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public DateOnly? DisciplineDate { get; set; }

    public string? Content { get; set; }

    public string? Punishment { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
