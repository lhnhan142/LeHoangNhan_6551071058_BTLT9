namespace SoChiTieu.Models;

/// <summary>Dòng hiển thị trên DataGridView (NgayChi là DateTime để định dạng dd/MM/yyyy dễ dàng).</summary>
public class ChiTieuRow
{
    public int MaCt { get; set; }
    public DateTime NgayChi { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public string LoaiChi { get; set; } = string.Empty;
    public decimal SoTien { get; set; }
    public string? GhiChu { get; set; }
}
