using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class SalaryCalculation
{
    public int Ids { get; set; }

    public int? Ide { get; set; }

    public int? IdSalaryhistory { get; set; }

    public int? IdEmployeeallowance { get; set; }

    public int? IdTimesheet { get; set; }

    public int? IdSalaryadvance { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public decimal? TotalSalary { get; set; }

    public DateOnly? Date { get; set; }

    public virtual EmployeeAllowance? IdEmployeeallowanceNavigation { get; set; }

    public virtual SalaryAdvance? IdSalaryadvanceNavigation { get; set; }

    public virtual SalaryHistory? IdSalaryhistoryNavigation { get; set; }

    public virtual TimeSheet? IdTimesheetNavigation { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
