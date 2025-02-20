using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class LeaveJob
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public bool? Status { get; set; }

    public DateTime? Date { get; set; }

    public string? ReasonLeave { get; set; }

    public string? TypeTermination { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
