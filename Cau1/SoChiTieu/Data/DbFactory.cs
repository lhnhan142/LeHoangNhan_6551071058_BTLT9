using Microsoft.EntityFrameworkCore;
using SoChiTieu.Models;

namespace SoChiTieu.Data;

/// <summary>Tạo DbContext từ connection string đọc ở appsettings.json.</summary>
public static class DbFactory
{
    public static QlChiTieuContext Create(string connectionString)
    {
        DbContextOptions<QlChiTieuContext> options = new DbContextOptionsBuilder<QlChiTieuContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new QlChiTieuContext(options);
    }
}
