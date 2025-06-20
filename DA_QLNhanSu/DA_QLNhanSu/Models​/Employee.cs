﻿using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Employee
{
    public int Ide { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public bool? Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Cccd { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public int? Idd { get; set; }

    public int? Idp { get; set; }

    public int? Idq { get; set; }

    public bool? Marry { get; set; }

    public bool? Status { get; set; }

    public int? Idrole { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();

    public virtual ICollection<EmployeeAllowance> EmployeeAllowances { get; set; } = new List<EmployeeAllowance>();

    public virtual Department? IddNavigation { get; set; }

    public virtual Position? IdpNavigation { get; set; }

    public virtual Qualification? IdqNavigation { get; set; }

    public virtual Account? IdroleNavigation { get; set; }

    public virtual ICollection<LeaveJob> LeaveJobs { get; set; } = new List<LeaveJob>();

    public virtual ICollection<OnLeave> OnLeaves { get; set; } = new List<OnLeave>();

    public virtual ICollection<PositionChange> PositionChanges { get; set; } = new List<PositionChange>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual ICollection<SalaryAdvance> SalaryAdvances { get; set; } = new List<SalaryAdvance>();

    public virtual ICollection<SalaryCalculation> SalaryCalculations { get; set; } = new List<SalaryCalculation>();

    public virtual ICollection<SalaryHistory> SalaryHistories { get; set; } = new List<SalaryHistory>();

    public virtual ICollection<TimeSheet> TimeSheets { get; set; } = new List<TimeSheet>();
}
