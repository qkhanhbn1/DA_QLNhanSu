﻿using System;
using System.Collections.Generic;

namespace DA_QLNhanSu.Models​;

public partial class Insurance
{
    public int Id { get; set; }

    public int? Ide { get; set; }

    public string? Number { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool? Status { get; set; }

    public virtual Employee? IdeNavigation { get; set; }
}
