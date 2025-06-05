using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class PositionChange
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public string? Type { get; set; }

    public string? Fromrole { get; set; }

    public string? Torole { get; set; }

    public bool? Status { get; set; }

    public DateOnly? Date { get; set; }

    public int? IdAppendix { get; set; }

    public virtual ContractAppendix? IdAppendixNavigation { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
