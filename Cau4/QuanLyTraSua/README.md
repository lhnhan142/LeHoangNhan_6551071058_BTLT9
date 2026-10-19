# Quán Lý Trà Sữa (WinForms - .NET 8.0)

Ứng dụng bán hàng quán trà sữa: lập đơn, thanh toán VietQR, quản lý thực đơn và xem doanh thu theo ngày.
Dự án dùng **EF Core Database-First** (sinh model từ CSDL `QL_TraSua`).

## Cấu trúc

```
QuanLyTraSua/
├── QuanLyTraSua.sln / QuanLyTraSua.csproj
├── init_db.sql              # Script tạo CSDL QL_TraSua (4 bảng + dữ liệu mẫu)
├── appsettings.json         # Chuỗi kết nối + cấu hình thanh toán
├── PaymentConfig.cs         # Đọc cấu hình "Payment" (strongly-typed)
├── Tien.cs                  # Định dạng tiền tệ (35.000 đ)
├── Program.cs               # Nạp cấu hình, Serilog, khởi động + khởi tạo CSDL
├── FormMain.cs              # Form chính + MenuStrip chuyển màn hình
├── FormBanHang.cs           # Task 3/5/7: lập đơn + VietQR + xử lý lỗi mất mạng
├── FormThucDon.cs           # Task 2: CRUD + tìm kiếm trên DanhMucMon & MonUong
├── FormDanhSachHoaDon.cs    # Task 6: hóa đơn theo ngày + doanh thu + chi tiết
└── Models/                  # Sinh bằng `dotnet ef dbcontext scaffold`
    ├── DanhMucMon.cs, MonUong.cs, HoaDon.cs, ChiTietHoaDon.cs
    └── QlTraSuaContext.cs   # + EnsureDatabaseReady() tự tạo/seed CSDL
```

## Cách chạy

1. Tạo CSDL (tùy chọn – app tự tạo nếu chưa có):

   ```powershell
   sqlcmd -S .\SQLEXPRESS -E -C -f 65001 -i init_db.sql
   ```

2. Chỉnh chuỗi kết nối trong `appsettings.json` nếu máy chủ SQL khác.
   Ứng dụng sẽ tự dò lần lượt: máy chủ trong cấu hình → `.\SQLEXPRESS` → `.` / `localhost` → `(localdb)\MSSQLLocalDB`.

3. Chạy:

   ```powershell
   dotnet run --project QuanLyTraSua.csproj
   ```

Log ghi tại `bin\Debug\net8.0-windows\logs\log-<yyyyMMdd>.txt` (Serilog, rolling theo ngày).

## Cấu hình thanh toán (appsettings.json → "Payment")

| Khóa | Ý nghĩa |
|------|---------|
| `BankId` | Mã ngân hàng (VietQR), ví dụ `VCB` |
| `AccountNo` | Số tài khoản nhận tiền |
| `AccountName` | Tên chủ tài khoản (không dấu) |
| `Template` | Mẫu QR: `compact`, `compact2`, `qr_only`, `print` |
| `TenQuan` | Tên quán hiển thị trên tiêu đề |

Số tài khoản **không** được hard-code trong code C#, và khi ghi log chỉ ghi dạng che `***6789`.
