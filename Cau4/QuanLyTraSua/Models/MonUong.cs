using System;
using System.Collections.Generic;

namespace QuanLyTraSua.Models;

public partial class MonUong
{
    public int MaMon { get; set; }

    public string TenMon { get; set; } = null!;

    public decimal GiaBan { get; set; }

    public bool DangBan { get; set; }

    public int MaDm { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual DanhMucMon MaDmNavigation { get; set; } = null!;
}
