using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class ContractAppendix
{
    public int Id { get; set; }

    public int? IdContract { get; set; }

    public DateOnly? SigningDate { get; set; }

    public string? TypeAppendix { get; set; }

    public string? Content { get; set; }

    public virtual Contract? IdContractNavigation { get; set; }

    public virtual ICollection<PositionChange> PositionChanges { get; set; } = new List<PositionChange>();

    public virtual ICollection<SalaryHistory> SalaryHistories { get; set; } = new List<SalaryHistory>();
}
