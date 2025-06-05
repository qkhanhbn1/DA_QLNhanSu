using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Contract
{
    public int Id { get; set; }

    public string? Codecontract { get; set; }

    public int? Ide { get; set; }

    public DateOnly? SigningDate { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? Content { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<ContractAppendix> ContractAppendices { get; set; } = new List<ContractAppendix>();

    public virtual Employee? IdeNavigation { get; set; }
}
