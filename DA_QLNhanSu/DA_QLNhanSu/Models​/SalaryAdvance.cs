using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DA_QLNhanSu.Models​;

public partial class SalaryAdvance
{
    public int Idsa { get; set; }

    public int? Ide { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
    public decimal? Money { get; set; }

    public bool? Status { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public virtual Employee? IdeNavigation { get; set; }

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();
}
