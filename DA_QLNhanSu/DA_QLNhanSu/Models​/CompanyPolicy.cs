using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class CompanyPolicy
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }
}
