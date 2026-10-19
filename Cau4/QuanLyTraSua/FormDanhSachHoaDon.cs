using Microsoft.EntityFrameworkCore;
using QuanLyTraSua.Models;
using Serilog;

namespace QuanLyTraSua;

/// <summary>
/// Task 6: Danh sách hóa đơn theo ngày + doanh thu + chi tiết hóa đơn.
/// </summary>
public class FormDanhSachHoaDon : Form
{
    private readonly DateTimePicker dtpNgay = new();
    private readonly Label lblDoanhThu = new();
    private readonly DataGridView dgvHoaDon = new();
    private readonly DataGridView dgvChiTiet = new();
    private readonly Label lblTieuDeChiTiet = new();

    public FormDanhSachHoaDon()
    {
        Text = "Danh sách Hóa đơn";
        Font = new Font("Segoe UI", 9.5F);
        BackColor = Color.WhiteSmoke;
        Padding = new Padding(10);
        AutoScaleMode = AutoScaleMode.Font;

        BuildUi();
        Load += (_, _) => TaiHoaDon();
    }

    // ===================== GIAO DIỆN =====================
    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.WhiteSmoke
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 52));

        root.Controls.Add(ThanhLoc(), 0, 0);
        root.Controls.Add(TaoNhomHoaDon(), 0, 1);
        root.Controls.Add(TaoNhomChiTiet(), 0, 2);

        Controls.Add(root);
    }

    private Control ThanhLoc()
    {
        var grp = new GroupBox
        {
            Text = "Lọc theo ngày",
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 4, 10, 4),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tlp.Controls.Add(new Label
        {
            Text = "Ngày:",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 3, 3)
        }, 0, 0);

        dtpNgay.Format = DateTimePickerFormat.Custom;
        dtpNgay.CustomFormat = "dd/MM/yyyy";
        dtpNgay.Dock = DockStyle.Fill;
        dtpNgay.Margin = new Padding(3, 3, 12, 3);
        dtpNgay.ValueChanged += (_, _) => TaiHoaDon();
        tlp.Controls.Add(dtpNgay, 1, 0);

        lblDoanhThu.Text = "Doanh thu ngày: 0 đ";
        lblDoanhThu.Dock = DockStyle.Fill;
        lblDoanhThu.TextAlign = ContentAlignment.MiddleLeft;
        lblDoanhThu.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblDoanhThu.ForeColor = Color.FromArgb(200, 40, 40);
        tlp.Controls.Add(lblDoanhThu, 2, 0);

        grp.Controls.Add(tlp);
        return grp;
    }

    private GroupBox TaoNhomHoaDon()
    {
        var grp = new GroupBox
        {
            Text = "Hóa đơn trong ngày",
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        CauHinhLuoi(dgvHoaDon);

        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colMaHd",
            HeaderText = "Mã HD",
            DataPropertyName = "MaHd",
            FillWeight = 60
        });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colGioLap",
            HeaderText = "Giờ lập",
            DataPropertyName = "GioLap",
            FillWeight = 90,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colHinhThuc",
            HeaderText = "Hình thức",
            DataPropertyName = "HinhThucTt",
            FillWeight = 110
        });
        dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTongTien",
            HeaderText = "Tổng tiền",
            DataPropertyName = "TongTien",
            FillWeight = 120,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });

        dgvHoaDon.SelectionChanged += (_, _) => TaiChiTietHoaDon();

        grp.Controls.Add(dgvHoaDon);
        return grp;
    }

    private GroupBox TaoNhomChiTiet()
    {
        var grp = new GroupBox
        {
            Text = "Chi tiết hóa đơn",
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        CauHinhLuoi(dgvChiTiet);

        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTenMon",
            HeaderText = "Tên món",
            DataPropertyName = "TenMon",
            FillWeight = 220
        });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colSoLy",
            HeaderText = "Số ly",
            DataPropertyName = "SoLy",
            FillWeight = 60,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colDonGiaCt",
            HeaderText = "Đơn giá",
            DataPropertyName = "DonGia",
            FillWeight = 100,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colThanhTienCt",
            HeaderText = "Thành tiền",
            DataPropertyName = "ThanhTien",
            FillWeight = 110,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });

        lblTieuDeChiTiet.Text = string.Empty;
        lblTieuDeChiTiet.Dock = DockStyle.Top;
        lblTieuDeChiTiet.Height = 24;
        lblTieuDeChiTiet.TextAlign = ContentAlignment.MiddleLeft;
        lblTieuDeChiTiet.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
        lblTieuDeChiTiet.ForeColor = Color.DimGray;

        grp.Controls.Add(dgvChiTiet);
        grp.Controls.Add(lblTieuDeChiTiet);
        return grp;
    }

    private static void CauHinhLuoi(DataGridView dgv)
    {
        dgv.Dock = DockStyle.Fill;
        dgv.AutoGenerateColumns = false;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.ReadOnly = true;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.RowHeadersVisible = false;
        dgv.BackgroundColor = Color.White;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.ColumnHeadersHeight = 34;
        dgv.RowTemplate.Height = 30;
        dgv.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 255);
    }

    // ===================== DỮ LIỆU =====================
    private void TaiHoaDon()
    {
        try
        {
            var ngay = dtpNgay.Value.Date;
            var ngayKeTiep = ngay.AddDays(1);

            using var db = Program.CreateContext();
            var hoaDons = db.HoaDons.AsNoTracking()
                .Where(h => h.NgayLap >= ngay && h.NgayLap < ngayKeTiep)
                .OrderBy(h => h.NgayLap)
                .Select(h => new
                {
                    h.MaHd,
                    h.NgayLap,
                    h.HinhThucTt,
                    h.TongTien
                })
                .ToList();

            var duLieu = hoaDons
                .Select(h => new
                {
                    h.MaHd,
                    GioLap = h.NgayLap.ToString("HH:mm:ss"),
                    h.HinhThucTt,
                    h.TongTien
                })
                .ToList();

            dgvHoaDon.DataSource = duLieu;

            var doanhThu = hoaDons.Sum(h => h.TongTien);
            lblDoanhThu.Text = "Doanh thu ngày: " + Tien.DinhDang(doanhThu);

            Log.Information("Tải danh sách hóa đơn ngày {Ngay}: {SoHd} hóa đơn, doanh thu {DoanhThu} đ.",
                ngay.ToString("dd/MM/yyyy"), hoaDons.Count, doanhThu);

            TaiChiTietHoaDon();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi tải danh sách hóa đơn.");
            MessageBox.Show("Không tải được danh sách hóa đơn:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TaiChiTietHoaDon()
    {
        try
        {
            var maHd = LayMaHoaDonDangChon();
            if (maHd is null)
            {
                dgvChiTiet.DataSource = null;
                lblTieuDeChiTiet.Text = string.Empty;
                return;
            }

            using var db = Program.CreateContext();
            var chiTiets = db.ChiTietHoaDons.AsNoTracking()
                .Where(c => c.MaHd == maHd)
                .Include(c => c.MaMonNavigation)
                .ToList();

            var duLieu = chiTiets
                .Select(c => new
                {
                    TenMon = c.MaMonNavigation.TenMon,
                    c.SoLy,
                    c.DonGia,
                    ThanhTien = c.DonGia * c.SoLy
                })
                .ToList();

            dgvChiTiet.DataSource = duLieu;
            lblTieuDeChiTiet.Text = $"Hóa đơn #{maHd} - {duLieu.Count} món - Tổng: " +
                                     Tien.DinhDang(chiTiets.Sum(c => c.DonGia * c.SoLy));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi tải chi tiết hóa đơn.");
            MessageBox.Show("Không tải được chi tiết hóa đơn:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private int? LayMaHoaDonDangChon()
    {
        if (dgvHoaDon.CurrentRow is null) return null;
        var value = dgvHoaDon.CurrentRow.Cells["colMaHd"].Value;
        return value is int ma ? ma : null;
    }
}
