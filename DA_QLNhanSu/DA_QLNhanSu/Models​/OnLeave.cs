using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class OnLeave
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public DateOnly? RequestDate { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? Content { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
