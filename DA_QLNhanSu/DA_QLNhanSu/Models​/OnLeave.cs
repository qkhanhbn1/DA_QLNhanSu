using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class OnLeave
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? Content { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
