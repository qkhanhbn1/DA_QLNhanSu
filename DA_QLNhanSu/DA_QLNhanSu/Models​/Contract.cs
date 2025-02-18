using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Contract
{
    public int Id { get; set; }

    public string? Nameemployee { get; set; }

    public string? Image { get; set; }

    public int? Ide { get; set; }

    public DateTime? SigningDate { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? Content { get; set; }

    public string? ContractDuration { get; set; }

    public int? Idp { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }

    public virtual Position? IdpNavigation { get; set; }
}
