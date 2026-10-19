using Microsoft.Extensions.Configuration;

namespace SoChiTieu.Configuration;

/// <summary>Lớp cấu hình có kiểu (strongly-typed) ánh xạ với appsettings.json.</summary>
public class AppSettings
{
    public ConnectionStringsSettings ConnectionStrings { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
}

public class ConnectionStringsSettings
{
    // Tên thuộc tính trùng với khóa "QL_ChiTieu" trong appsettings.json
    public string QL_ChiTieu { get; set; } = string.Empty;
}

public class LoggingSettings
{
    /// <summary>Thư mục log. Đường dẫn tương đối sẽ tính từ thư mục chứa file .exe.</summary>
    public string Directory { get; set; } = "Logs";

    /// <summary>Verbose | Debug | Information | Warning | Error | Fatal</summary>
    public string MinimumLevel { get; set; } = "Information";

    public int RetainedFileCount { get; set; } = 30;
}

public static class AppSettingsLoader
{
    /// <summary>
    /// Đọc appsettings.json (bắt buộc) rồi ghi đè bằng appsettings.{môi trường}.json (nếu có).
    /// Debug build mặc định là môi trường "Development"; bản publish là "Production"
    /// (có thể đổi bằng biến môi trường DOTNET_ENVIRONMENT).
    /// </summary>
    public static AppSettings Load()
    {
        string? env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
#if DEBUG
        env ??= "Development";
#endif
        env ??= "Production";

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .Build();

        AppSettings settings = config.Get<AppSettings>() ?? new AppSettings();

        if (string.IsNullOrWhiteSpace(settings.ConnectionStrings.QL_ChiTieu))
            throw new InvalidOperationException(
                "Thiếu connection string \"ConnectionStrings:QL_ChiTieu\" trong appsettings.json.");

        return settings;
    }
}
