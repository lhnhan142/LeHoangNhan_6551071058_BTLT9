namespace QuanLyTraSua;

/// <summary>
/// Cấu hình thanh toán (strongly-typed) đọc từ mục "Payment" trong appsettings.json.
/// TUYỆT ĐỐI không hard-code thông tin tài khoản trong code C#.
/// </summary>
public class PaymentConfig
{
    public string BankId { get; set; } = string.Empty;

    public string AccountNo { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    /// <summary>compact / compact2 / qr_only / print</summary>
    public string Template { get; set; } = "compact2";

    public string TenQuan { get; set; } = string.Empty;

    /// <summary>Số tài khoản đã che, dùng để ghi log an toàn (ví dụ: ***6789).</summary>
    public string AccountNoMasked
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AccountNo)) return "***";
            var so = AccountNo.Trim();
            if (so.Length <= 4) return new string('*', so.Length);
            return "***" + so[^4..];
        }
    }

    /// <summary>Ghép đường dẫn ảnh VietQR động.</summary>
    public string BuildVietQrUrl(decimal soTien, string noiDung)
    {
        return $"https://img.vietqr.io/image/{BankId}-{AccountNo}-{Template}.png" +
               $"?amount={soTien:0}" +
               $"&addInfo={Uri.EscapeDataString(noiDung ?? string.Empty)}" +
               $"&accountName={Uri.EscapeDataString(AccountName ?? string.Empty)}";
    }

    /// <summary>Thông tin chuyển khoản dạng chữ (dùng khi mất mạng / không tải được ảnh QR).</summary>
    public string BuildThongTinChuyenKhoan(decimal soTien, string noiDung)
    {
        return $"Ngân hàng: {BankId}{Environment.NewLine}" +
               $"Số tài khoản: {AccountNo}{Environment.NewLine}" +
               $"Chủ tài khoản: {AccountName}{Environment.NewLine}" +
               $"Số tiền: {Tien.DinhDang(soTien)}{Environment.NewLine}" +
               $"Nội dung CK: {noiDung}";
    }
}
