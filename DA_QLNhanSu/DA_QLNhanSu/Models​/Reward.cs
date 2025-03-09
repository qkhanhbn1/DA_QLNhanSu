using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Reward
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public DateOnly? RewardDate { get; set; }

    public string? Content { get; set; }

    public decimal? RewardGift { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
