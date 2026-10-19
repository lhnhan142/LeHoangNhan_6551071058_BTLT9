using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTraSua.Models;

public partial class QlTraSuaContext : DbContext
{
    public QlTraSuaContext(DbContextOptions<QlTraSuaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DanhMucMon> DanhMucMons { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<MonUong> MonUongs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaMon });

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHoaDon_HoaDon");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHoaDon_MonUong");
        });

        modelBuilder.Entity<DanhMucMon>(entity =>
        {
            entity.HasKey(e => e.MaDm);

            entity.ToTable("DanhMucMon");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenDm)
                .HasMaxLength(50)
                .HasColumnName("TenDM");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd);

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.HinhThucTt)
                .HasMaxLength(20)
                .HasColumnName("HinhThucTT");
            entity.Property(e => e.NgayLap).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<MonUong>(entity =>
        {
            entity.HasKey(e => e.MaMon);

            entity.ToTable("MonUong");

            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenMon).HasMaxLength(100);

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.MonUongs)
                .HasForeignKey(d => d.MaDm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonUong_DanhMucMon");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    // ==================================================================
    //  Tự động tạo CSDL (nếu chưa tồn tại) + seed dữ liệu mẫu:
    //  4 danh mục (Trà sữa, Trà trái cây, Topping, Đồ ăn vặt)
    //  12 món uống mẫu
    // ==================================================================
    public void EnsureDatabaseReady()
    {
        Database.EnsureCreated();

        if (!DanhMucMons.Any())
        {
            DanhMucMons.AddRange(
                new DanhMucMon { TenDm = "Trà sữa" },
                new DanhMucMon { TenDm = "Trà trái cây" },
                new DanhMucMon { TenDm = "Topping" },
                new DanhMucMon { TenDm = "Đồ ăn vặt" });
            SaveChanges();
        }

        if (!MonUongs.Any())
        {
            var dm = DanhMucMons.ToDictionary(x => x.TenDm, x => x.MaDm);

            MonUongs.AddRange(
                new MonUong { TenMon = "Trà sữa trân châu đường đen", GiaBan = 35000, DangBan = true, MaDm = dm["Trà sữa"] },
                new MonUong { TenMon = "Trà sữa matcha", GiaBan = 32000, DangBan = true, MaDm = dm["Trà sữa"] },
                new MonUong { TenMon = "Trà sữa khoai môn", GiaBan = 32000, DangBan = true, MaDm = dm["Trà sữa"] },
                new MonUong { TenMon = "Trà sữa ô long nướng", GiaBan = 35000, DangBan = true, MaDm = dm["Trà sữa"] },
                new MonUong { TenMon = "Trà đào cam sả", GiaBan = 30000, DangBan = true, MaDm = dm["Trà trái cây"] },
                new MonUong { TenMon = "Trà chanh", GiaBan = 20000, DangBan = true, MaDm = dm["Trà trái cây"] },
                new MonUong { TenMon = "Trà vải hạt sen", GiaBan = 30000, DangBan = true, MaDm = dm["Trà trái cây"] },
                new MonUong { TenMon = "Trà xoài nhiệt đới", GiaBan = 30000, DangBan = true, MaDm = dm["Trà trái cây"] },
                new MonUong { TenMon = "Trân châu trắng", GiaBan = 5000, DangBan = true, MaDm = dm["Topping"] },
                new MonUong { TenMon = "Thạch rau câu", GiaBan = 5000, DangBan = true, MaDm = dm["Topping"] },
                new MonUong { TenMon = "Bánh flan", GiaBan = 15000, DangBan = true, MaDm = dm["Đồ ăn vặt"] },
                new MonUong { TenMon = "Bánh croissant", GiaBan = 20000, DangBan = true, MaDm = dm["Đồ ăn vặt"] });

            SaveChanges();
        }
    }
}
