using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Contract
{
    public int Id { get; set; }

    public string? Codecontract { get; set; }

    public int? Ide { get; set; }

    public DateTime? SigningDate { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? Content { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
