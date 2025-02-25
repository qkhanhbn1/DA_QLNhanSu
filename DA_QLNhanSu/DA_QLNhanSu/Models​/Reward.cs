using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DA_QLNhanSu.Models​;

public partial class Reward
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public DateTime? RewardDate { get; set; }

    public string? Content { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
    public decimal? RewardGift { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
