/* ============================================================
   init_db.sql
   Tao CSDL QL_TraSua cho ung dung "Ban hang quan tra sua" (WinForms .NET 8)
   Chay: sqlcmd -S .\SQLEXPRESS -E -i init_db.sql
   ============================================================ */

IF DB_ID(N'QL_TraSua') IS NULL
BEGIN
    CREATE DATABASE QL_TraSua;
END
GO

USE QL_TraSua;
GO

/* ---------- Xoa bang cu (neu chay lai script) ---------- */
IF OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NOT NULL DROP TABLE dbo.ChiTietHoaDon;
IF OBJECT_ID(N'dbo.HoaDon',        N'U') IS NOT NULL DROP TABLE dbo.HoaDon;
IF OBJECT_ID(N'dbo.MonUong',       N'U') IS NOT NULL DROP TABLE dbo.MonUong;
IF OBJECT_ID(N'dbo.DanhMucMon',    N'U') IS NOT NULL DROP TABLE dbo.DanhMucMon;
GO

/* ---------- 1. DanhMucMon ---------- */
CREATE TABLE dbo.DanhMucMon
(
    MaDM    INT IDENTITY(1,1) NOT NULL,
    TenDM   NVARCHAR(50)      NOT NULL,
    CONSTRAINT PK_DanhMucMon PRIMARY KEY (MaDM)
);
GO

/* ---------- 2. MonUong ---------- */
CREATE TABLE dbo.MonUong
(
    MaMon    INT IDENTITY(1,1) NOT NULL,
    TenMon   NVARCHAR(100)     NOT NULL,
    GiaBan   DECIMAL(18,0)     NOT NULL,
    DangBan  BIT               NOT NULL,
    MaDM     INT               NOT NULL,
    CONSTRAINT PK_MonUong PRIMARY KEY (MaMon),
    CONSTRAINT FK_MonUong_DanhMucMon FOREIGN KEY (MaDM) REFERENCES dbo.DanhMucMon (MaDM)
);
GO

/* ---------- 3. HoaDon ---------- */
CREATE TABLE dbo.HoaDon
(
    MaHD         INT IDENTITY(1,1) NOT NULL,
    NgayLap      DATETIME          NOT NULL,
    TongTien     DECIMAL(18,0)     NOT NULL,
    HinhThucTT   NVARCHAR(20)      NOT NULL,   -- N'Tien mat' / N'Chuyen khoan'
    DaThanhToan  BIT               NOT NULL,
    CONSTRAINT PK_HoaDon PRIMARY KEY (MaHD)
);
GO

/* ---------- 4. ChiTietHoaDon (khoa chinh kep) ---------- */
CREATE TABLE dbo.ChiTietHoaDon
(
    MaHD     INT           NOT NULL,
    MaMon    INT           NOT NULL,
    SoLy     INT           NOT NULL,
    DonGia   DECIMAL(18,0) NOT NULL,
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaMon),
    CONSTRAINT FK_ChiTietHoaDon_HoaDon  FOREIGN KEY (MaHD)  REFERENCES dbo.HoaDon (MaHD),
    CONSTRAINT FK_ChiTietHoaDon_MonUong FOREIGN KEY (MaMon) REFERENCES dbo.MonUong (MaMon)
);
GO

/* ---------- Du lieu mau: 4 danh muc ---------- */
INSERT INTO dbo.DanhMucMon (TenDM) VALUES
 (N'Trà sữa'),
 (N'Trà trái cây'),
 (N'Topping'),
 (N'Đồ ăn vặt');
GO

/* ---------- Du lieu mau: 12 mon uong ---------- */
INSERT INTO dbo.MonUong (TenMon, GiaBan, DangBan, MaDM) VALUES
 (N'Trà sữa trân châu đường đen', 35000, 1, 1),
 (N'Trà sữa matcha',              32000, 1, 1),
 (N'Trà sữa khoai môn',           32000, 1, 1),
 (N'Trà sữa ô long nướng',        35000, 1, 1),
 (N'Trà đào cam sả',              30000, 1, 2),
 (N'Trà chanh',                   20000, 1, 2),
 (N'Trà vải hạt sen',             30000, 1, 2),
 (N'Trà xoài nhiệt đới',          30000, 1, 2),
 (N'Trân châu trắng',              5000, 1, 3),
 (N'Thạch rau câu',                5000, 1, 3),
 (N'Bánh flan',                   15000, 1, 4),
 (N'Bánh croissant',              20000, 1, 4);
GO

PRINT N'>>> Da tao xong CSDL QL_TraSua.';
GO
