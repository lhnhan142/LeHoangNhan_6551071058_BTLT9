using Serilog;
using Serilog.Events;
using SoChiTieu.Configuration;

namespace SoChiTieu;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 1. Đọc cấu hình
        AppSettings settings;
        try
        {
            settings = AppSettingsLoader.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không đọc được file cấu hình appsettings.json.\n\n" + ex.Message,
                "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 2. Cấu hình Serilog: file theo ngày trong thư mục Logs cạnh file .exe -> Logs\log-yyyyMMdd.txt
        string logDir = Path.IsPathRooted(settings.Logging.Directory)
            ? settings.Logging.Directory
            : Path.Combine(AppContext.BaseDirectory, settings.Logging.Directory);

        LogEventLevel level = Enum.TryParse(settings.Logging.MinimumLevel, true, out LogEventLevel parsed)
            ? parsed
            : LogEventLevel.Information;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(level)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.File(
                path: Path.Combine(logDir, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: settings.Logging.RetainedFileCount,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                encoding: System.Text.Encoding.UTF8)
            .CreateLogger();

        // 3. Bắt lỗi chưa xử lý
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) =>
        {
            Log.Error(e.Exception, "Lỗi chưa được xử lý trên UI thread");
            MessageBox.Show("Đã xảy ra lỗi không mong muốn. Chi tiết đã được ghi vào thư mục Logs.",
                "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };

        try
        {
            Log.Information("Ứng dụng khởi động (Machine={Machine})", Environment.MachineName);
            Application.Run(new FormChiTieu(settings));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Ứng dụng bị dừng đột ngột");
            MessageBox.Show("Ứng dụng gặp lỗi nghiêm trọng và phải đóng. Chi tiết trong thư mục Logs.",
                "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.Information("Ứng dụng đóng");
            Log.CloseAndFlush();
        }
    }
}
