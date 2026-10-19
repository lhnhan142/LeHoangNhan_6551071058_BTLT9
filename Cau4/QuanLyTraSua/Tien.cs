using System.Globalization;

namespace QuanLyTraSua;

/// <summary>Các hàm tiện ích dùng chung.</summary>
public static class Tien
{
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    /// <summary>35000 -> "35.000 đ"</summary>
    public static string DinhDang(decimal soTien) => soTien.ToString("N0", Vi) + " đ";
}
