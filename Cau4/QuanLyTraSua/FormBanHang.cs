using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using QuanLyTraSua.Models;
using Serilog;

namespace QuanLyTraSua;

/// <summary>
/// Task 3, 5, 7: Lập đơn bán hàng &amp; thanh toán VietQR (có xử lý lỗi mất mạng).
/// </summary>
public class FormBanHang : Form
{
    /// <summary>Một dòng trong giỏ hàng.</summary>
    public class DongGio
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public int SoLy { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * SoLy;
    }

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(20) };

    // ----- Nhóm "Chọn món" -----
    private readonly ComboBox cboDanhMuc = new();
    private readonly ListBox lstMon = new();
    private readonly NumericUpDown numSoLy = new();
    private readonly Button btnThemVaoDon = new();

    // ----- Nhóm "Đơn hiện tại" -----
    private readonly DataGridView dgvGioHang = new();
    private readonly Button btnXoaDong = new();
    private readonly Label lblTongTien = new();

    // ----- Nhóm "Thanh toán" -----
    private readonly Button btnChuyenKhoan = new();
    private readonly Button btnTienMat = new();
    private readonly PictureBox picQR = new();
    private readonly Label lblThongTinCK = new();
    private readonly Button btnDaNhanTien = new();

    private readonly BindingList<DongGio> gioHang = new();

    public FormBanHang()
    {
        Text = "Bán hàng (Lập đơn)";
        Font = new Font("Segoe UI", 9.5F);
        BackColor = Color.WhiteSmoke;
        Padding = new Padding(10);
        AutoScaleMode = AutoScaleMode.Font;

        BuildUi();

        gioHang.ListChanged += (_, _) => CapNhatTongTien();

        Load += (_, _) =>
        {
            TaiDanhMuc();
            CapNhatTongTien();
        };
    }

    // ===================== GIAO DIỆN =====================
    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = Color.WhiteSmoke
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 29));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

        root.Controls.Add(NhomChonMon(), 0, 0);
        root.Controls.Add(NhomDonHienTai(), 1, 0);
        root.Controls.Add(NhomThanhToan(), 2, 0);

        Controls.Add(root);
    }

    private GroupBox NhomChonMon()
    {
        var grp = new GroupBox
        {
            Text = "Chọn món",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        tlp.Controls.Add(Nhan("Danh mục:"), 0, 0);
        cboDanhMuc.Dock = DockStyle.Fill;
        cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDanhMuc.Margin = new Padding(3, 5, 3, 3);
        cboDanhMuc.SelectedIndexChanged += (_, _) => TaiMonTheoDanhMuc();
        tlp.Controls.Add(cboDanhMuc, 1, 0);

        var lblMon = Nhan("Món đang bán (chọn 1 món):");
        lblMon.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblMon.ForeColor = Color.DimGray;
        lblMon.Margin = new Padding(3, 6, 3, 0);
        tlp.Controls.Add(lblMon, 0, 1);
        tlp.SetColumnSpan(lblMon, 2);

        lstMon.Dock = DockStyle.Fill;
        lstMon.IntegralHeight = false;
        lstMon.Font = new Font("Segoe UI", 10F);
        lstMon.Margin = new Padding(3, 2, 3, 6);
        lstMon.DoubleClick += (_, _) => ThemVaoDon();
        lstMon.SelectedIndexChanged += (_, _) =>
        {
            if (lstMon.SelectedIndex >= 0) numSoLy.Focus();
        };
        tlp.Controls.Add(lstMon, 0, 2);
        tlp.SetColumnSpan(lstMon, 2);

        tlp.Controls.Add(Nhan("Số ly:"), 0, 3);
        numSoLy.Dock = DockStyle.Fill;
        numSoLy.Minimum = 1;
        numSoLy.Maximum = 999;
        numSoLy.Value = 1;
        numSoLy.Margin = new Padding(3, 5, 3, 3);
        tlp.Controls.Add(numSoLy, 1, 3);

        TrangTriNut(btnThemVaoDon, "Thêm vào đơn  ➜", Color.FromArgb(46, 139, 87));
        btnThemVaoDon.Dock = DockStyle.Fill;
        btnThemVaoDon.Margin = new Padding(0, 4, 0, 0);
        btnThemVaoDon.Click += (_, _) => ThemVaoDon();
        tlp.Controls.Add(btnThemVaoDon, 0, 4);
        tlp.SetColumnSpan(btnThemVaoDon, 2);

        grp.Controls.Add(tlp);
        return grp;
    }

    private GroupBox NhomDonHienTai()
    {
        var grp = new GroupBox
        {
            Text = "Đơn hiện tại",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
        };
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        dgvGioHang.Dock = DockStyle.Fill;
        dgvGioHang.AutoGenerateColumns = false;
        dgvGioHang.AllowUserToAddRows = false;
        dgvGioHang.ReadOnly = true;
        dgvGioHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvGioHang.MultiSelect = false;
        dgvGioHang.RowHeadersVisible = false;
        dgvGioHang.BackgroundColor = Color.White;
        dgvGioHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvGioHang.ColumnHeadersHeight = 34;
        dgvGioHang.RowTemplate.Height = 30;
        dgvGioHang.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        dgvGioHang.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 255);

        dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTenMon",
            HeaderText = "Tên món",
            DataPropertyName = nameof(DongGio.TenMon),
            FillWeight = 200
        });
        dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colSoLy",
            HeaderText = "SL",
            DataPropertyName = nameof(DongGio.SoLy),
            FillWeight = 45,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colDonGia",
            HeaderText = "Đơn giá",
            DataPropertyName = nameof(DongGio.DonGia),
            FillWeight = 85,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colThanhTien",
            HeaderText = "Thành tiền",
            DataPropertyName = nameof(DongGio.ThanhTien),
            FillWeight = 100,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });

        dgvGioHang.DataSource = gioHang;
        tlp.Controls.Add(dgvGioHang, 0, 0);

        TrangTriNut(btnXoaDong, "Xóa dòng", Color.FromArgb(190, 60, 60));
        btnXoaDong.Dock = DockStyle.Left;
        btnXoaDong.Width = 120;
        btnXoaDong.Margin = new Padding(0, 5, 0, 0);
        btnXoaDong.Click += (_, _) => XoaDong();
        tlp.Controls.Add(btnXoaDong, 0, 1);

        lblTongTien.Text = "Tổng tiền: 0 đ";
        lblTongTien.Dock = DockStyle.Fill;
        lblTongTien.TextAlign = ContentAlignment.MiddleRight;
        lblTongTien.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTongTien.ForeColor = Color.FromArgb(200, 40, 40);
        tlp.Controls.Add(lblTongTien, 0, 2);

        grp.Controls.Add(tlp);
        return grp;
    }

    private GroupBox NhomThanhToan()
    {
        var grp = new GroupBox
        {
            Text = "Thanh toán",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
        };
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var lblTieuDe = new Label
        {
            Text = "Khách quét mã QR để chuyển khoản",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 100, 190),
            TextAlign = ContentAlignment.MiddleLeft
        };
        tlp.Controls.Add(lblTieuDe, 0, 0);

        lblThongTinCK.Dock = DockStyle.Fill;
        lblThongTinCK.BorderStyle = BorderStyle.FixedSingle;
        lblThongTinCK.BackColor = Color.FromArgb(250, 250, 235);
        lblThongTinCK.Padding = new Padding(6);
        lblThongTinCK.TextAlign = ContentAlignment.TopLeft;
        lblThongTinCK.Font = new Font("Segoe UI", 9F);
        lblThongTinCK.Text = "Chưa có thông tin thanh toán.";
        tlp.Controls.Add(lblThongTinCK, 0, 1);

        picQR.Dock = DockStyle.Fill;
        picQR.SizeMode = PictureBoxSizeMode.Zoom;
        picQR.BackColor = Color.White;
        picQR.BorderStyle = BorderStyle.FixedSingle;
        picQR.Margin = new Padding(0, 6, 0, 6);
        tlp.Controls.Add(picQR, 0, 2);

        TrangTriNut(btnChuyenKhoan, "Thanh toán chuyển khoản", Color.FromArgb(30, 100, 190));
        btnChuyenKhoan.Dock = DockStyle.Fill;
        btnChuyenKhoan.Margin = new Padding(0, 2, 0, 2);
        btnChuyenKhoan.Click += BtnChuyenKhoan_Click;
        tlp.Controls.Add(btnChuyenKhoan, 0, 3);

        TrangTriNut(btnTienMat, "Tiền mặt", Color.FromArgb(110, 110, 110));
        btnTienMat.Dock = DockStyle.Fill;
        btnTienMat.Margin = new Padding(0, 2, 0, 2);
        btnTienMat.Click += (_, _) => LuuHoaDon("Tiền mặt");
        tlp.Controls.Add(btnTienMat, 0, 4);

        TrangTriNut(btnDaNhanTien, "Đã nhận tiền", Color.FromArgb(34, 160, 70));
        btnDaNhanTien.Dock = DockStyle.Fill;
        btnDaNhanTien.Margin = new Padding(0, 2, 0, 2);
        btnDaNhanTien.Enabled = true;
        btnDaNhanTien.Click += (_, _) => LuuHoaDon("Chuyển khoản");
        tlp.Controls.Add(btnDaNhanTien, 0, 5);

        grp.Controls.Add(tlp);
        return grp;
    }

    private static Label Nhan(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Anchor = AnchorStyles.Left,
        Margin = new Padding(3, 9, 3, 3)
    };

    private static void TrangTriNut(Button btn, string text, Color mauNen)
    {
        btn.Text = text;
        btn.BackColor = mauNen;
        btn.ForeColor = Color.White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.Height = 38;
        btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btn.Cursor = Cursors.Hand;
    }

    // ===================== DỮ LIỆU MÓN =====================
    private void TaiDanhMuc()
    {
        try
        {
            using var db = Program.CreateContext();
            var danhMucs = db.DanhMucMons.AsNoTracking().OrderBy(x => x.TenDm).ToList();

            cboDanhMuc.DataSource = null;
            cboDanhMuc.DisplayMember = nameof(DanhMucMon.TenDm);
            cboDanhMuc.ValueMember = nameof(DanhMucMon.MaDm);
            cboDanhMuc.DataSource = danhMucs;

            if (cboDanhMuc.Items.Count > 0) cboDanhMuc.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi tải danh mục cho màn hình bán hàng.");
            MessageBox.Show("Không tải được danh mục:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TaiMonTheoDanhMuc()
    {
        if (cboDanhMuc.SelectedItem is not DanhMucMon dm) return;

        try
        {
            using var db = Program.CreateContext();
            var mons = db.MonUongs.AsNoTracking()
                .Where(m => m.DangBan && m.MaDm == dm.MaDm)
                .OrderBy(m => m.TenMon)
                .ToList();

            lstMon.BeginUpdate();
            lstMon.Items.Clear();
            foreach (var mon in mons)
                lstMon.Items.Add(new MonHienThi(mon.MaMon, mon.TenMon, mon.GiaBan));
            lstMon.EndUpdate();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi tải món theo danh mục.");
            MessageBox.Show("Không tải được danh sách món:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Item hiển thị trong ListBox: "Tên món - Giá".</summary>
    private sealed class MonHienThi
    {
        public int MaMon { get; }
        public string TenMon { get; }
        public decimal GiaBan { get; }

        public MonHienThi(int maMon, string tenMon, decimal giaBan)
        {
            MaMon = maMon;
            TenMon = tenMon;
            GiaBan = giaBan;
        }

        public override string ToString() => $"{TenMon} - {Tien.DinhDang(GiaBan)}";
    }

    // ===================== GIỎ HÀNG =====================
    private void ThemVaoDon()
    {
        if (lstMon.SelectedItem is not MonHienThi mon)
        {
            MessageBox.Show("Vui lòng chọn một món uống.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var soLy = (int)numSoLy.Value;
        var dongDaCo = gioHang.FirstOrDefault(d => d.MaMon == mon.MaMon);

        if (dongDaCo is not null)
        {
            // Món đã có trong giỏ -> cộng dồn số lượng
            dongDaCo.SoLy += soLy;
            gioHang.ResetBindings();
        }
        else
        {
            gioHang.Add(new DongGio
            {
                MaMon = mon.MaMon,
                TenMon = mon.TenMon,
                DonGia = mon.GiaBan,
                SoLy = soLy
            });
        }

        numSoLy.Value = 1;
        CapNhatTongTien();
    }

    private void XoaDong()
    {
        if (dgvGioHang.CurrentRow?.DataBoundItem is not DongGio dong)
        {
            MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        gioHang.Remove(dong);
        CapNhatTongTien();
    }

    private decimal TongTien => gioHang.Sum(d => d.ThanhTien);

    private void CapNhatTongTien()
    {
        lblTongTien.Text = "Tổng tiền: " + Tien.DinhDang(TongTien);
        dgvGioHang.Refresh();
    }

    private bool KiemTraGioHang()
    {
        if (gioHang.Count == 0)
        {
            MessageBox.Show("Đơn hàng đang trống, vui lòng thêm món.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    // ===================== THANH TOÁN CHUYỂN KHOẢN (VIETQR) =====================
    private async void BtnChuyenKhoan_Click(object? sender, EventArgs e)
    {
        if (!KiemTraGioHang()) return;

        var cfg = Program.PaymentSettings;
        var soTien = TongTien;

        try
        {
            btnChuyenKhoan.Enabled = false;

            // Mã hóa đơn dự kiến (bản ghi mới nhất + 1)
            int maHdDuKien;
            using (var db = Program.CreateContext())
            {
                var maxHd = await db.HoaDons.MaxAsync(h => (int?)h.MaHd) ?? 0;
                maHdDuKien = maxHd + 1;
            }

            var noiDung = $"TRASUA HD{maHdDuKien}";
            var url = cfg.BuildVietQrUrl(soTien, noiDung);

            // CHÚ Ý: không ghi số tài khoản đầy đủ vào log
            Log.Information(
                "Tạo QR chuyển khoản - HĐ dự kiến {MaHD}, số tiền {SoTien}, ngân hàng {BankId}, STK {AccountNo}",
                maHdDuKien, soTien, cfg.BankId, cfg.AccountNoMasked);

            lblThongTinCK.Text = cfg.BuildThongTinChuyenKhoan(soTien, noiDung) +
                                  Environment.NewLine + Environment.NewLine +
                                  "Đang tải mã QR...";

            try
            {
                var bytes = await Http.GetByteArrayAsync(url);
                using var ms = new MemoryStream(bytes);
                var anh = Image.FromStream(ms);

                picQR.Image?.Dispose();
                picQR.Image = new Bitmap(anh);

                lblThongTinCK.Text = cfg.BuildThongTinChuyenKhoan(soTien, noiDung) +
                                      Environment.NewLine + Environment.NewLine +
                                      "➜ Quét mã QR bên dưới để chuyển khoản.";

                Log.Information("Đã tải ảnh VietQR cho HĐ dự kiến {MaHD} ({SoTien} đ).", maHdDuKien, soTien);
            }
            catch (Exception exTaiAnh)
            {
                // Task 7: mất mạng -> vẫn hiển thị đủ thông tin để khách chuyển khoản tay
                Log.Error(exTaiAnh, "Tải ảnh QR thất bại, chuyển sang chế độ nhập tay (HĐ dự kiến {MaHD}).", maHdDuKien);

                picQR.Image?.Dispose();
                picQR.Image = null;

                lblThongTinCK.Text = cfg.BuildThongTinChuyenKhoan(soTien, noiDung) +
                                      Environment.NewLine + Environment.NewLine +
                                      "⚠ Không tải được mã QR (lỗi mạng). Khách vui lòng chuyển khoản theo thông tin trên, " +
                                      "sau đó bấm \"Đã nhận tiền\" hoặc chọn \"Tiền mặt\".";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi khi tạo thanh toán chuyển khoản.");
            MessageBox.Show("Không tạo được thanh toán chuyển khoản:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnChuyenKhoan.Enabled = true;
        }
    }

    // ===================== LƯU HÓA ĐƠN =====================
    private void LuuHoaDon(string hinhThuc)
    {
        if (!KiemTraGioHang()) return;

        try
        {
            var soTien = TongTien;

            using var db = Program.CreateContext();
            var hoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                TongTien = soTien,
                HinhThucTt = hinhThuc,   // "Tiền mặt" hoặc "Chuyển khoản"
                DaThanhToan = true
            };

            foreach (var dong in gioHang)
            {
                hoaDon.ChiTietHoaDons.Add(new ChiTietHoaDon
                {
                    MaMon = dong.MaMon,
                    SoLy = dong.SoLy,
                    DonGia = dong.DonGia
                });
            }

            db.HoaDons.Add(hoaDon);

            // Lưu HoaDon + toàn bộ ChiTietHoaDon trong CÙNG MỘT lần SaveChanges()
            db.SaveChanges();

            Log.Information("Đã lưu hóa đơn {MaHD} - {SoTien} đ - {HinhThuc} - {SoDong} dòng chi tiết.",
                hoaDon.MaHd, soTien, hinhThuc, gioHang.Count);

            MessageBox.Show(
                $"Thanh toán thành công!\n\nMã hóa đơn: {hoaDon.MaHd}\n" +
                $"Hình thức: {hinhThuc}\nTổng tiền: {Tien.DinhDang(soTien)}",
                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

            XoaDon();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi lưu hóa đơn ({HinhThuc}).", hinhThuc);
            MessageBox.Show("Không lưu được hóa đơn:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void XoaDon()
    {
        gioHang.Clear();
        picQR.Image?.Dispose();
        picQR.Image = null;
        lblThongTinCK.Text = "Chưa có thông tin thanh toán.";
        numSoLy.Value = 1;
        CapNhatTongTien();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            picQR.Image?.Dispose();
            picQR.Image = null;
        }
        base.Dispose(disposing);
    }
}
