using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SoChiTieu.Models;

// DbContext do EF Core Database First sinh ra với tùy chọn --no-onconfiguring
// (connection string KHÔNG hard-code ở đây mà được nạp từ appsettings.json qua DbFactory).
public partial class QlChiTieuContext : DbContext
{
    public QlChiTieuContext()
    {
    }

    public QlChiTieuContext(DbContextOptions<QlChiTieuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTieu> ChiTieus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTieu>(entity =>
        {
            entity.HasKey(e => e.MaCt).HasName("PK_ChiTieu");

            entity.ToTable("ChiTieu");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.LoaiChi).HasMaxLength(30);
            entity.Property(e => e.NoiDung).HasMaxLength(100);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
