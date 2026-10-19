using Microsoft.EntityFrameworkCore;
using QuanLyTraSua.Models;
using Serilog;

namespace QuanLyTraSua;

/// <summary>
/// Task 2: CRUD + tìm kiếm trên 2 bảng DanhMucMon &amp; MonUong.
/// </summary>
public class FormThucDon : Form
{
    // ----- Nhập liệu -----
    private readonly TextBox txtTenMon = new();
    private readonly ComboBox cboDanhMuc = new();
    private readonly NumericUpDown numGiaBan = new();
    private readonly CheckBox chkDangBan = new();

    // ----- Tìm kiếm -----
    private readonly TextBox txtTim = new();
    private readonly ComboBox cboLoc = new();

    // ----- Lưới -----
    private readonly DataGridView dgvMon = new();

    private readonly Label lblThongBao = new();
    private bool dangTaiDuLieu;

    public FormThucDon()
    {
        Text = "Quản lý Thực đơn";
        Font = new Font("Segoe UI", 9.5F);
        BackColor = Color.WhiteSmoke;
        Padding = new Padding(10);
        AutoScaleMode = AutoScaleMode.Font;

        BuildUi();
        Load += (_, _) => TaiDuLieuBanDau();
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 175));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(TaoNhomThongTin(), 0, 0);
        root.Controls.Add(TaoNhomTimKiem(), 0, 1);
        root.Controls.Add(TaoLuoiMon(), 0, 2);

        Controls.Add(root);
    }

    private GroupBox TaoNhomThongTin()
    {
        var grp = new GroupBox
        {
            Text = "Thông tin món uống",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // --- Dòng 1 ---
        tlp.Controls.Add(TaoNhan("Tên món:"), 0, 0);
        txtTenMon.Dock = DockStyle.Fill;
        txtTenMon.Margin = new Padding(3, 5, 12, 3);
        tlp.Controls.Add(txtTenMon, 1, 0);

        tlp.Controls.Add(TaoNhan("Danh mục:"), 2, 0);
        cboDanhMuc.Dock = DockStyle.Fill;
        cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDanhMuc.Margin = new Padding(3, 5, 3, 3);
        tlp.Controls.Add(cboDanhMuc, 3, 0);

        // --- Dòng 2 ---
        tlp.Controls.Add(TaoNhan("Giá bán (đ):"), 0, 1);
        numGiaBan.Dock = DockStyle.Fill;
        numGiaBan.Minimum = 0;
        numGiaBan.Maximum = 10_000_000;
        numGiaBan.Increment = 1000;
        numGiaBan.ThousandsSeparator = true;
        numGiaBan.Value = 30000;
        numGiaBan.Margin = new Padding(3, 5, 12, 3);
        tlp.Controls.Add(numGiaBan, 1, 1);

        chkDangBan.Text = "Đang bán";
        chkDangBan.Checked = true;
        chkDangBan.AutoSize = true;
        chkDangBan.Margin = new Padding(3, 8, 3, 3);
        tlp.Controls.Add(chkDangBan, 2, 1);
        tlp.SetColumnSpan(chkDangBan, 2);

        // --- Dòng 3: các nút chức năng ---
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 6, 0, 0)
        };

        var btnThem = TaoNut("Thêm", Color.FromArgb(46, 139, 87), 90);
        var btnSua = TaoNut("Sửa", Color.FromArgb(30, 100, 190), 90);
        var btnXoa = TaoNut("Xóa", Color.FromArgb(190, 60, 60), 90);
        var btnLamMoi = TaoNut("Làm mới", Color.FromArgb(110, 110, 110), 100);

        btnThem.Click += (_, _) => ThemMon();
        btnSua.Click += (_, _) => SuaMon();
        btnXoa.Click += (_, _) => XoaMon();
        btnLamMoi.Click += (_, _) => { XoaNhapLieu(); TaiDuLieuBanDau(); };

        flow.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

        lblThongBao.AutoSize = true;
        lblThongBao.ForeColor = Color.DarkRed;
        lblThongBao.Margin = new Padding(16, 12, 0, 0);
        flow.Controls.Add(lblThongBao);

        tlp.Controls.Add(flow, 0, 2);
        tlp.SetColumnSpan(flow, 4);

        grp.Controls.Add(tlp);
        return grp;
    }

    private GroupBox TaoNhomTimKiem()
    {
        var grp = new GroupBox
        {
            Text = "Tìm kiếm",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        tlp.Controls.Add(TaoNhan("Tên món:"), 0, 0);
        txtTim.Dock = DockStyle.Fill;
        txtTim.PlaceholderText = "Nhập tên món cần tìm...";
        txtTim.Margin = new Padding(3, 5, 12, 3);
        txtTim.TextChanged += (_, _) => TaiLuoiMon();
        tlp.Controls.Add(txtTim, 1, 0);

        tlp.Controls.Add(TaoNhan("Danh mục:"), 2, 0);
        cboLoc.Dock = DockStyle.Fill;
        cboLoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoc.Margin = new Padding(3, 5, 3, 3);
        cboLoc.SelectedIndexChanged += (_, _) => TaiLuoiMon();
        tlp.Controls.Add(cboLoc, 3, 0);

        grp.Controls.Add(tlp);
        return grp;
    }

    private GroupBox TaoLuoiMon()
    {
        var grp = new GroupBox
        {
            Text = "Danh sách món uống",
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        dgvMon.Dock = DockStyle.Fill;
        dgvMon.AutoGenerateColumns = false;
        dgvMon.AllowUserToAddRows = false;
        dgvMon.AllowUserToDeleteRows = false;
        dgvMon.ReadOnly = true;
        dgvMon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvMon.MultiSelect = false;
        dgvMon.RowHeadersVisible = false;
        dgvMon.BackgroundColor = Color.White;
        dgvMon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvMon.ColumnHeadersHeight = 34;
        dgvMon.RowTemplate.Height = 30;
        dgvMon.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        dgvMon.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 255);

        dgvMon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colMaMon",
            HeaderText = "Mã món",
            DataPropertyName = "MaMon",
            Width = 70,
            FillWeight = 60
        });
        dgvMon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTenMon",
            HeaderText = "Tên món",
            DataPropertyName = "TenMon",
            FillWeight = 260
        });
        dgvMon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTenDm",
            HeaderText = "Tên danh mục",
            DataPropertyName = "TenDm",
            FillWeight = 150
        });
        dgvMon.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colGiaBan",
            HeaderText = "Giá bán (đ)",
            DataPropertyName = "GiaBan",
            FillWeight = 110,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgvMon.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "colDangBan",
            HeaderText = "Đang bán",
            DataPropertyName = "DangBan",
            FillWeight = 80
        });

        dgvMon.SelectionChanged += (_, _) => HienThiDongDangChon();

        grp.Controls.Add(dgvMon);
        return grp;
    }

    private static Label TaoNhan(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Anchor = AnchorStyles.Left,
        Margin = new Padding(3, 9, 3, 3)
    };

    private static Button TaoNut(string text, Color mauNen, int rong) => new()
    {
        Text = text,
        Width = rong,
        Height = 34,
        BackColor = mauNen,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Margin = new Padding(0, 0, 8, 0),
        Cursor = Cursors.Hand
    };

    // ===================== DỮ LIỆU =====================
    private void TaiDuLieuBanDau()
    {
        try
        {
            dangTaiDuLieu = true;

            using var db = Program.CreateContext();
            var danhMucs = db.DanhMucMons.AsNoTracking().OrderBy(x => x.TenDm).ToList();

            cboDanhMuc.DataSource = null;
            cboDanhMuc.DisplayMember = nameof(DanhMucMon.TenDm);
            cboDanhMuc.ValueMember = nameof(DanhMucMon.MaDm);
            cboDanhMuc.DataSource = danhMucs.ToList();

            var deLoc = new List<DanhMucMon> { new() { MaDm = 0, TenDm = "Tất cả" } };
            deLoc.AddRange(danhMucs);

            cboLoc.DataSource = null;
            cboLoc.DisplayMember = nameof(DanhMucMon.TenDm);
            cboLoc.ValueMember = nameof(DanhMucMon.MaDm);
            cboLoc.DataSource = deLoc;
            cboLoc.SelectedIndex = 0;

            dangTaiDuLieu = false;

            TaiLuoiMon();
        }
        catch (Exception ex)
        {
            dangTaiDuLieu = false;
            Log.Error(ex, "Lỗi tải danh mục / món uống.");
            MessageBox.Show("Không tải được dữ liệu thực đơn:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TaiLuoiMon()
    {
        if (dangTaiDuLieu) return;

        try
        {
            var tuKhoa = txtTim.Text.Trim();
            var maDm = (cboLoc.SelectedItem as DanhMucMon)?.MaDm ?? 0;

            using var db = Program.CreateContext();
            var query = db.MonUongs.AsNoTracking()
                .Include(m => m.MaDmNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                query = query.Where(m => m.TenMon.Contains(tuKhoa));

            if (maDm > 0)
                query = query.Where(m => m.MaDm == maDm);

            var duLieu = query
                .OrderBy(m => m.MaDm)
                .ThenBy(m => m.TenMon)
                .Select(m => new
                {
                    m.MaMon,
                    m.TenMon,
                    TenDm = m.MaDmNavigation.TenDm,
                    m.GiaBan,
                    m.DangBan
                })
                .ToList();

            dgvMon.DataSource = duLieu;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi tải danh sách món.");
            MessageBox.Show("Không tải được danh sách món:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void HienThiDongDangChon()
    {
        if (dgvMon.CurrentRow is null) return;

        dangTaiDuLieu = true;
        try
        {
            var dong = dgvMon.CurrentRow;
            txtTenMon.Text = dong.Cells["colTenMon"].Value?.ToString() ?? string.Empty;

            var gia = dong.Cells["colGiaBan"].Value;
            if (gia is decimal g)
                numGiaBan.Value = Math.Clamp(g, numGiaBan.Minimum, numGiaBan.Maximum);

            chkDangBan.Checked = dong.Cells["colDangBan"].Value is true;

            var tenDm = dong.Cells["colTenDm"].Value?.ToString();
            if (tenDm is not null && cboDanhMuc.DataSource is List<DanhMucMon> ds)
            {
                var index = ds.FindIndex(x => x.TenDm == tenDm);
                if (index >= 0) cboDanhMuc.SelectedIndex = index;
            }

            lblThongBao.Text = string.Empty;
        }
        finally
        {
            dangTaiDuLieu = false;
        }
    }

    private int LayMaMonDangChon()
    {
        if (dgvMon.CurrentRow is null) return -1;
        var value = dgvMon.CurrentRow.Cells["colMaMon"].Value;
        return value is int ma ? ma : -1;
    }

    private void XoaNhapLieu()
    {
        dangTaiDuLieu = true;
        txtTenMon.Clear();
        numGiaBan.Value = 30000;
        chkDangBan.Checked = true;
        if (cboDanhMuc.Items.Count > 0) cboDanhMuc.SelectedIndex = 0;
        lblThongBao.Text = string.Empty;
        dangTaiDuLieu = false;
    }

    private bool KiemTraNhapLieu(out string loi)
    {
        loi = string.Empty;
        if (string.IsNullOrWhiteSpace(txtTenMon.Text))
        {
            loi = "Vui lòng nhập tên món.";
            return false;
        }
        if (cboDanhMuc.SelectedItem is not DanhMucMon)
        {
            loi = "Vui lòng chọn danh mục.";
            return false;
        }
        if (numGiaBan.Value <= 0)
        {
            loi = "Giá bán phải lớn hơn 0.";
            return false;
        }
        return true;
    }

    private void ThemMon()
    {
        if (!KiemTraNhapLieu(out var loi))
        {
            lblThongBao.ForeColor = Color.DarkRed;
            lblThongBao.Text = loi;
            return;
        }

        try
        {
            var dm = (DanhMucMon)cboDanhMuc.SelectedItem!;

            using var db = Program.CreateContext();
            db.MonUongs.Add(new MonUong
            {
                TenMon = txtTenMon.Text.Trim(),
                GiaBan = numGiaBan.Value,
                DangBan = chkDangBan.Checked,
                MaDm = dm.MaDm
            });
            db.SaveChanges();

            Log.Information("Đã thêm món {TenMon}", txtTenMon.Text.Trim());
            lblThongBao.ForeColor = Color.DarkGreen;
            lblThongBao.Text = "Đã thêm món thành công.";

            XoaNhapLieu();
            TaiLuoiMon();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi thêm món.");
            MessageBox.Show("Không thêm được món:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SuaMon()
    {
        var maMon = LayMaMonDangChon();
        if (maMon < 0)
        {
            MessageBox.Show("Vui lòng chọn một món cần sửa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!KiemTraNhapLieu(out var loi))
        {
            lblThongBao.ForeColor = Color.DarkRed;
            lblThongBao.Text = loi;
            return;
        }

        try
        {
            var dm = (DanhMucMon)cboDanhMuc.SelectedItem!;

            using var db = Program.CreateContext();
            var mon = db.MonUongs.FirstOrDefault(m => m.MaMon == maMon);
            if (mon is null)
            {
                MessageBox.Show("Món không còn tồn tại.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLuoiMon();
                return;
            }

            mon.TenMon = txtTenMon.Text.Trim();
            mon.GiaBan = numGiaBan.Value;
            mon.DangBan = chkDangBan.Checked;
            mon.MaDm = dm.MaDm;
            db.SaveChanges();

            Log.Information("Đã sửa món {MaMon} -> {TenMon}", maMon, mon.TenMon);
            lblThongBao.ForeColor = Color.DarkGreen;
            lblThongBao.Text = "Đã cập nhật món.";

            TaiLuoiMon();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi sửa món.");
            MessageBox.Show("Không sửa được món:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void XoaMon()
    {
        var maMon = LayMaMonDangChon();
        if (maMon < 0)
        {
            MessageBox.Show("Vui lòng chọn một món cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show($"Bạn có chắc muốn xóa món \"{txtTenMon.Text}\"?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            using var db = Program.CreateContext();

            var daDungTrongHoaDon = db.ChiTietHoaDons.Any(c => c.MaMon == maMon);
            if (daDungTrongHoaDon)
            {
                MessageBox.Show("Món này đã xuất hiện trong hóa đơn nên không thể xóa.\n" +
                                "Bạn có thể bỏ chọn \"Đang bán\" để ngừng kinh doanh món này.",
                    "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var mon = db.MonUongs.FirstOrDefault(m => m.MaMon == maMon);
            if (mon is null) return;

            db.MonUongs.Remove(mon);
            db.SaveChanges();

            Log.Information("Đã xóa món {MaMon} - {TenMon}", maMon, mon.TenMon);
            lblThongBao.ForeColor = Color.DarkGreen;
            lblThongBao.Text = "Đã xóa món.";

            XoaNhapLieu();
            TaiLuoiMon();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi xóa món.");
            MessageBox.Show("Không xóa được món:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
