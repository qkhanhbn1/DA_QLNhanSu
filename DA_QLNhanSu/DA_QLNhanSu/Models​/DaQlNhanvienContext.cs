using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DA_QLNhanSu.Models​;

public partial class DaQlNhanvienContext : DbContext
{
    public DaQlNhanvienContext()
    {
    }

    public DaQlNhanvienContext(DbContextOptions<DaQlNhanvienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Allowance> Allowances { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAllowance> EmployeeAllowances { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    public virtual DbSet<OnLeave> OnLeaves { get; set; }

    public virtual DbSet<Overtime> Overtimes { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<SalaryAdvance> SalaryAdvances { get; set; }

    public virtual DbSet<SalaryCalculation> SalaryCalculations { get; set; }

    public virtual DbSet<TimeSheet> TimeSheets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2KJH2I9;Database=DA_QL_NHANVIEN;uid=sa;pwd=1234;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("ACCOUNT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("PASSWORD");
        });

        modelBuilder.Entity<Allowance>(entity =>
        {
            entity.HasKey(e => e.Ida);

            entity.ToTable("ALLOWANCES");

            entity.Property(e => e.Ida).HasColumnName("IDA");
            entity.Property(e => e.Money)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONEY");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("CONTRACT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("CONTENT");
            entity.Property(e => e.ContractDuration)
                .HasMaxLength(250)
                .HasColumnName("CONTRACT_DURATION");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("EXPIRATION_DATE");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Idp).HasColumnName("IDP");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("datetime")
                .HasColumnName("RELEASE_DATE");
            entity.Property(e => e.SigningDate)
                .HasColumnType("datetime")
                .HasColumnName("SIGNING_DATE");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_CONTRACT_EMPLOYEE");

            entity.HasOne(d => d.IdpNavigation).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.Idp)
                .HasConstraintName("FK_CONTRACT_POSITION");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Idd);

            entity.ToTable("DEPARTMENT");

            entity.Property(e => e.Idd).HasColumnName("IDD");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("CONTENT");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.ToTable("DISCIPLINE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("CONTENT");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.NumberDiscipline).HasColumnName("NUMBER_DISCIPLINE");
            entity.Property(e => e.Punishment)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PUNISHMENT");
            entity.Property(e => e.Status).HasColumnName("STATUS");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_DISCIPLINE_EMPLOYEE");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Ide);

            entity.ToTable("EMPLOYEE");

            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Birthday)
                .HasColumnType("datetime")
                .HasColumnName("BIRTHDAY");
            entity.Property(e => e.Cccd)
                .HasMaxLength(50)
                .HasColumnName("CCCD");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("CODE");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gender).HasColumnName("GENDER");
            entity.Property(e => e.Idd).HasColumnName("IDD");
            entity.Property(e => e.Idp).HasColumnName("IDP");
            entity.Property(e => e.Idq).HasColumnName("IDQ");
            entity.Property(e => e.Image)
                .HasMaxLength(250)
                .HasColumnName("IMAGE");
            entity.Property(e => e.Marry).HasColumnName("MARRY");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("PHONE");

            entity.HasOne(d => d.IddNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Idd)
                .HasConstraintName("FK_EMPLOYEE_DEPARTMENT");

            entity.HasOne(d => d.IdpNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Idp)
                .HasConstraintName("FK_EMPLOYEE_POSITION1");

            entity.HasOne(d => d.IdqNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Idq)
                .HasConstraintName("FK_EMPLOYEE_QUALIFICATION");
        });

        modelBuilder.Entity<EmployeeAllowance>(entity =>
        {
            entity.ToTable("EMPLOYEE_ALLOWANCE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdAllowances).HasColumnName("ID_ALLOWANCES");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Money)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONEY");

            entity.HasOne(d => d.IdAllowancesNavigation).WithMany(p => p.EmployeeAllowances)
                .HasForeignKey(d => d.IdAllowances)
                .HasConstraintName("FK_EMPLOYEE_ALLOWANCE_ALLOWANCES");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.EmployeeAllowances)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_EMPLOYEE_ALLOWANCE_EMPLOYEE");
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.ToTable("INSURANCE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("EXPIRATION_DATE");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .HasColumnName("NUMBER");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("datetime")
                .HasColumnName("RELEASE_DATE");
            entity.Property(e => e.Status).HasColumnName("STATUS");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.Insurances)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_INSURANCE_EMPLOYEE");
        });

        modelBuilder.Entity<OnLeave>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LEAVE");

            entity.ToTable("ON_LEAVE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("CONTENT");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("EXPIRATION_DATE");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("datetime")
                .HasColumnName("RELEASE_DATE");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.OnLeaves)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_LEAVE_EMPLOYEE");
        });

        modelBuilder.Entity<Overtime>(entity =>
        {
            entity.HasKey(e => e.Ido);

            entity.ToTable("OVERTIME", tb => tb.HasTrigger("CalculateOvertimePay"));

            entity.Property(e => e.Ido).HasColumnName("IDO");
            entity.Property(e => e.HourlyWage)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("HOURLY_WAGE");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.OvertimePay)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("OVERTIME_PAY");
            entity.Property(e => e.SalaryCoeficient).HasColumnName("SALARY_COEFICIENT");
            entity.Property(e => e.WorkingHours).HasColumnName("WORKING_HOURS");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.Overtimes)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_OVERTIME_EMPLOYEE");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Idp);

            entity.ToTable("POSITION");

            entity.Property(e => e.Idp).HasColumnName("IDP");
            entity.Property(e => e.DailyWage)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DAILY_WAGE");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Qualification>(entity =>
        {
            entity.HasKey(e => e.Idq);

            entity.ToTable("QUALIFICATION");

            entity.Property(e => e.Idq).HasColumnName("IDQ");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.ToTable("REWARD");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("CONTENT");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.NumberRewards).HasColumnName("NUMBER_REWARDS");
            entity.Property(e => e.RewardGift)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("REWARD_GIFT");
            entity.Property(e => e.Status).HasColumnName("STATUS");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_REWARD_EMPLOYEE");
        });

        modelBuilder.Entity<SalaryAdvance>(entity =>
        {
            entity.HasKey(e => e.Idsa);

            entity.ToTable("SALARY_ADVANCE");

            entity.Property(e => e.Idsa).HasColumnName("IDSA");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Money)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONEY");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.SalaryAdvances)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_SALARY_ADVANCE_EMPLOYEE");
        });

        modelBuilder.Entity<SalaryCalculation>(entity =>
        {
            entity.HasKey(e => e.Ids);

            entity.ToTable("SALARY_CALCULATION");

            entity.Property(e => e.Ids).HasColumnName("IDS");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("DATE");
            entity.Property(e => e.IdEmployeeallowance).HasColumnName("ID_EMPLOYEEALLOWANCE");
            entity.Property(e => e.IdOvertime).HasColumnName("ID_OVERTIME");
            entity.Property(e => e.IdPosition).HasColumnName("ID_POSITION");
            entity.Property(e => e.IdSalaryadvance).HasColumnName("ID_SALARYADVANCE");
            entity.Property(e => e.IdTimesheet).HasColumnName("ID_TIMESHEET");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Month).HasColumnName("MONTH");
            entity.Property(e => e.TotalSalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("TOTAL_SALARY");
            entity.Property(e => e.Workday).HasColumnName("WORKDAY");
            entity.Property(e => e.Year).HasColumnName("YEAR");

            entity.HasOne(d => d.IdEmployeeallowanceNavigation).WithMany(p => p.SalaryCalculations)
                .HasForeignKey(d => d.IdEmployeeallowance)
                .HasConstraintName("FK_SALARY_CALCULATION_EMPLOYEE_ALLOWANCE");

            entity.HasOne(d => d.IdOvertimeNavigation).WithMany(p => p.SalaryCalculations)
                .HasForeignKey(d => d.IdOvertime)
                .HasConstraintName("FK_SALARY_CALCULATION_OVERTIME");

            entity.HasOne(d => d.IdPositionNavigation).WithMany(p => p.SalaryCalculations)
                .HasForeignKey(d => d.IdPosition)
                .HasConstraintName("FK_SALARY_CALCULATION_POSITION");

            entity.HasOne(d => d.IdSalaryadvanceNavigation).WithMany(p => p.SalaryCalculations)
                .HasForeignKey(d => d.IdSalaryadvance)
                .HasConstraintName("FK_SALARY_CALCULATION_SALARY_ADVANCE");

            entity.HasOne(d => d.IdTimesheetNavigation).WithMany(p => p.SalaryCalculations)
                .HasForeignKey(d => d.IdTimesheet)
                .HasConstraintName("FK_SALARY_CALCULATION_TIME_SHEET");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.SalaryCalculations)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_SALARY_CALCULATION_EMPLOYEE");
        });

        modelBuilder.Entity<TimeSheet>(entity =>
        {
            entity.ToTable("TIME_SHEET");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Ide).HasColumnName("IDE");
            entity.Property(e => e.Month).HasColumnName("MONTH");
            entity.Property(e => e.Workday).HasColumnName("WORKDAY");
            entity.Property(e => e.Year).HasColumnName("YEAR");

            entity.HasOne(d => d.IdeNavigation).WithMany(p => p.TimeSheets)
                .HasForeignKey(d => d.Ide)
                .HasConstraintName("FK_TIME_SHEET_EMPLOYEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
