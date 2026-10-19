using System;
using System.Collections.Generic;

namespace SoChiTieu.Models;

// Model do EF Core Database First sinh ra (dotnet ef dbcontext scaffold ...).
public partial class ChiTieu
{
    public int MaCt { get; set; }

    public DateOnly NgayChi { get; set; }

    public string NoiDung { get; set; } = null!;

    public string LoaiChi { get; set; } = null!;

    public decimal SoTien { get; set; }

    public string? GhiChu { get; set; }
}
