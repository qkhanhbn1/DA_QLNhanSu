using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class LeaveJob
{
    public int Id { get; set; }

    public string? Nameemployee { get; set; }

    public int? Idp { get; set; }

    public string? Image { get; set; }

    public bool? Type { get; set; }

    public bool? Status { get; set; }

    public string? Date { get; set; }

    public virtual Position? IdpNavigation { get; set; }
}
