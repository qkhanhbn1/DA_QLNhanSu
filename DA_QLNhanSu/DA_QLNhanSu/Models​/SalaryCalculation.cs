using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DA_QLNhanSu.Models​;

public partial class SalaryCalculation
{
    public int Ids { get; set; }

    public int? Ide { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
    public int? IdPosition { get; set; }
    
    public int? IdOvertime { get; set; }
    
    public int? IdEmployeeallowance { get; set; }
    
    public int? IdTimesheet { get; set; }
    
    public int? IdSalaryadvance { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? Workday { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
    public decimal? TotalSalary { get; set; }

    public DateTime? Date { get; set; }

    public virtual EmployeeAllowance? IdEmployeeallowanceNavigation { get; set; }

    public virtual Overtime? IdOvertimeNavigation { get; set; }
    
    public virtual Position? IdPositionNavigation { get; set; }

    public virtual SalaryAdvance? IdSalaryadvanceNavigation { get; set; }

    public virtual TimeSheet? IdTimesheetNavigation { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
