using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLyTraSua.Models;
using Serilog;

namespace QuanLyTraSua;

internal static class Program
{
    /// <summary>Cấu hình thanh toán đọc từ appsettings.json (mục "Payment").</summary>
    public static PaymentConfig PaymentSettings { get; private set; } = new();

    /// <summary>Chuỗi kết nối đọc từ appsettings.json (mục "ConnectionStrings:DefaultConnection").</summary>
    public static string ConnectionString { get; private set; } = string.Empty;

    private static string? _resolvedConnectionString;

    [STAThread]
    private static void Main()
    {
        // ---------- 1. Serilog: ghi log ra file logs/log-.txt (rolling theo ngày) ----------
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(logDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("===== Khởi động ứng dụng Quán Trà Sữa =====");

        try
        {
            // ---------- 2. Nạp appsettings.json ----------
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            PaymentSettings = configuration.GetSection("Payment").Get<PaymentConfig>() ?? new PaymentConfig();

            Log.Information("Đã nạp cấu hình. Ngân hàng: {BankId}, STK: {AccountNo}, Quán: {TenQuan}",
                PaymentSettings.BankId, PaymentSettings.AccountNoMasked, PaymentSettings.TenQuan);

            // ---------- 3. Chuẩn bị CSDL ----------
            try
            {
                using var db = CreateContext();
                db.EnsureDatabaseReady();
                Log.Information("CSDL QL_TraSua đã sẵn sàng.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Không thể kết nối / khởi tạo CSDL.");
                MessageBox.Show(
                    "Không kết nối được SQL Server.\n\nChi tiết: " + ex.Message +
                    "\n\nHãy kiểm tra SQL Server và chuỗi kết nối trong appsettings.json.",
                    "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // ---------- 4. Chạy giao diện ----------
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Lỗi không xử lý được ở tầng ứng dụng.");
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.Information("===== Kết thúc ứng dụng =====");
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Tạo DbContext với chuỗi kết nối đã được "dò" sang máy chủ SQL Server khả dụng
    /// (SQLEXPRESS / localhost / (localdb)\MSSQLLocalDB).
    /// </summary>
    public static QlTraSuaContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<QlTraSuaContext>()
            .UseSqlServer(ResolveConnectionString())
            .Options;

        return new QlTraSuaContext(options);
    }

    /// <summary>
    /// Thử lần lượt các máy chủ SQL Server phổ biến, trả về chuỗi kết nối đầu tiên kết nối được.
    /// </summary>
    public static string ResolveConnectionString()
    {
        if (_resolvedConnectionString is not null) return _resolvedConnectionString;

        SqlConnectionStringBuilder builder;
        try
        {
            builder = new SqlConnectionStringBuilder(ConnectionString);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Chuỗi kết nối trong appsettings.json không hợp lệ.");
            _resolvedConnectionString = ConnectionString;
            return _resolvedConnectionString;
        }

        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(builder.DataSource)) candidates.Add(builder.DataSource);
        foreach (var server in new[] { @".\SQLEXPRESS", ".", "localhost", @"(localdb)\MSSQLLocalDB" })
        {
            if (!candidates.Any(c => string.Equals(c, server, StringComparison.OrdinalIgnoreCase)))
                candidates.Add(server);
        }

        foreach (var server in candidates)
        {
            // Khi dò máy chủ chỉ chờ tối đa 3 giây để không làm chậm lúc khởi động
            var probe = new SqlConnectionStringBuilder(builder.ConnectionString)
            {
                DataSource = server,
                ConnectTimeout = 3
            };
            try
            {
                using var connection = new SqlConnection(probe.ConnectionString);
                connection.Open();

                // Giữ nguyên thời gian chờ theo cấu hình gốc cho các kết nối thật
                var working = new SqlConnectionStringBuilder(builder.ConnectionString) { DataSource = server };
                _resolvedConnectionString = working.ConnectionString;

                Log.Information("Kết nối SQL Server thành công tại: {Server}", server);
                return _resolvedConnectionString;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Không kết nối được SQL Server tại {Server}", server);
            }
        }

        Log.Error("Không kết nối được bất kỳ máy chủ SQL Server nào. Dùng tạm cấu hình gốc.");
        _resolvedConnectionString = builder.ConnectionString;
        return _resolvedConnectionString;
    }
}
