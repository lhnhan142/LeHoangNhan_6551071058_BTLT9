using System.Globalization;
using Serilog;
using SoChiTieu.Configuration;
using SoChiTieu.Data;
using SoChiTieu.Models;
using Microsoft.EntityFrameworkCore;

namespace SoChiTieu;

public partial class FormChiTieu : Form
{
    private const string TatCa = "Tất cả";
    private const string TieuDe = "Sổ chi tiêu cá nhân";
    private static readonly string[] DanhSachLoai = { "Ăn uống", "Đi lại", "Học tập", "Giải trí", "Khác" };
    private static readonly CultureInfo Vi = new("vi-VN");

    private readonly AppSettings _settings;
    private int? _selectedId;     // MaCT của dòng đang chọn
    private bool _loading;        // chặn SelectionChanged khi đang nạp lưới / xóa form
    private bool _ready;          // chặn sự kiện khi đang khởi tạo control

    public FormChiTieu(AppSettings settings)
    {
        _settings = settings;
        InitializeComponent();
        InitControls();
        ConfigureGrid();
    }

    // ===================== KHỞI TẠO =====================

    private void InitControls()
    {
        // Form nhập
        cboLoaiChi.Items.AddRange(DanhSachLoai);
        cboLoaiChi.SelectedIndex = 0;
        dtpNgayChi.Value = DateTime.Today;

        nudSoTien.Minimum = 0;
        nudSoTien.Maximum = 1_000_000_000_000M;
        nudSoTien.Increment = 1000;      // bước nhảy 1.000
        nudSoTien.DecimalPlaces = 0;
        nudSoTien.ThousandsSeparator = true;

        // Bộ lọc: mặc định từ đầu tháng đến hôm nay
        cboLoaiLoc.Items.Add(TatCa);
        cboLoaiLoc.Items.AddRange(DanhSachLoai);
        cboLoaiLoc.SelectedIndex = 0;
        DateTime today = DateTime.Today;
        dtpTuNgay.Value = new DateTime(today.Year, today.Month, 1);
        dtpDenNgay.Value = today;

        _ready = true;
    }

    private void ConfigureGrid()
    {
        dgvChiTieu.AutoGenerateColumns = false;
        dgvChiTieu.AllowUserToAddRows = false;
        dgvChiTieu.AllowUserToDeleteRows = false;
        dgvChiTieu.ReadOnly = true;
        dgvChiTieu.MultiSelect = false;
        dgvChiTieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvChiTieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvChiTieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgvChiTieu.Columns.Clear();
        dgvChiTieu.Columns.Add(MakeColumn(nameof(ChiTieuRow.MaCt), "Mã", 40, "000", DataGridViewContentAlignment.MiddleCenter));
        dgvChiTieu.Columns.Add(MakeColumn(nameof(ChiTieuRow.NgayChi), "Ngày chi", 70, "dd/MM/yyyy", DataGridViewContentAlignment.MiddleCenter));
        dgvChiTieu.Columns.Add(MakeColumn(nameof(ChiTieuRow.NoiDung), "Nội dung", 120, null, DataGridViewContentAlignment.MiddleCenter));
        dgvChiTieu.Columns.Add(MakeColumn(nameof(ChiTieuRow.LoaiChi), "Loại chi", 70, null, DataGridViewContentAlignment.MiddleLeft));
        dgvChiTieu.Columns.Add(MakeColumn(nameof(ChiTieuRow.SoTien), "Số tiền", 70, "N0", DataGridViewContentAlignment.MiddleRight));
        dgvChiTieu.Columns.Add(MakeColumn(nameof(ChiTieuRow.GhiChu), "Ghi chú", 130, null, DataGridViewContentAlignment.MiddleCenter));
    }

    private static DataGridViewTextBoxColumn MakeColumn(
        string property, string header, float fillWeight, string? format, DataGridViewContentAlignment align)
    {
        var col = new DataGridViewTextBoxColumn
        {
            DataPropertyName = property,
            Name = "col" + property,
            HeaderText = header,
            FillWeight = fillWeight
        };
        col.DefaultCellStyle.Alignment = align;
        col.DefaultCellStyle.FormatProvider = Vi;
        if (format != null) col.DefaultCellStyle.Format = format;
        return col;
    }

    private void FormChiTieu_Load(object? sender, EventArgs e) => LoadData();

    // ===================== NẠP DỮ LIỆU / TÌM KIẾM =====================

    /// <summary>Nạp lưới theo bộ lọc hiện tại và cập nhật nhãn "Tổng chi".</summary>
    private void LoadData(int? selectId = null)
    {
        try
        {
            string keyword = txtTimKiem.Text.Trim();
            string? loai = cboLoaiLoc.SelectedItem as string;
            DateOnly tuNgay = DateOnly.FromDateTime(dtpTuNgay.Value.Date);
            DateOnly denNgay = DateOnly.FromDateTime(dtpDenNgay.Value.Date);

            List<ChiTieuRow> rows;
            using (var db = DbFactory.Create(_settings.ConnectionStrings.QL_ChiTieu))
            {
                IQueryable<ChiTieu> query = db.ChiTieus.AsNoTracking()
                    .Where(x => x.NgayChi >= tuNgay && x.NgayChi <= denNgay);

                if (keyword.Length > 0)
                    query = query.Where(x => x.NoiDung.Contains(keyword));

                if (!string.IsNullOrEmpty(loai) && loai != TatCa)
                    query = query.Where(x => x.LoaiChi == loai);

                rows = query
                    .OrderBy(x => x.NgayChi).ThenBy(x => x.MaCt)
                    .AsEnumerable()
                    .Select(x => new ChiTieuRow
                    {
                        MaCt = x.MaCt,
                        NgayChi = x.NgayChi.ToDateTime(TimeOnly.MinValue),
                        NoiDung = x.NoiDung,
                        LoaiChi = x.LoaiChi,
                        SoTien = x.SoTien,
                        GhiChu = x.GhiChu
                    })
                    .ToList();
            }

            _loading = true;
            try
            {
                dgvChiTieu.DataSource = rows;

                // Chọn lại dòng vừa sửa (nếu có), ngược lại bỏ chọn
                DataGridViewRow? target = selectId == null
                    ? null
                    : dgvChiTieu.Rows.Cast<DataGridViewRow>()
                        .FirstOrDefault(r => (r.DataBoundItem as ChiTieuRow)?.MaCt == selectId);

                if (target != null)
                {
                    dgvChiTieu.ClearSelection();
                    target.Selected = true;
                    dgvChiTieu.CurrentCell = target.Cells[0];
                    _selectedId = selectId;
                }
                else
                {
                    dgvChiTieu.ClearSelection();
                    _selectedId = null;
                }
            }
            finally
            {
                _loading = false;
            }

            // Tổng chi tính theo đúng kết quả đang hiển thị
            decimal tong = rows.Sum(r => r.SoTien);
            lblTong.Text = $"Tổng chi: {tong.ToString("N0", Vi)} đ";
        }
        catch (Exception ex)
        {
            HandleDbError("tải danh sách chi tiêu", ex);
        }
    }

    private void btnTim_Click(object? sender, EventArgs e)
    {
        if (dtpTuNgay.Value.Date > dtpDenNgay.Value.Date)
        {
            Warn("\"Từ ngày\" không được lớn hơn \"Đến ngày\".", dtpTuNgay);
            return;
        }
        LoadData();
    }

    private void txtTimKiem_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            btnTim.PerformClick();
        }
    }

    private void cboLoaiLoc_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_ready && IsHandleCreated) btnTim.PerformClick();
    }

    // ===================== CHỌN DÒNG -> HIỆN LÊN CONTROL =====================

    private void dgvChiTieu_SelectionChanged(object? sender, EventArgs e)
    {
        if (_loading || dgvChiTieu.SelectedRows.Count == 0) return;
        if (dgvChiTieu.SelectedRows[0].DataBoundItem is not ChiTieuRow r) return;

        _selectedId = r.MaCt;
        dtpNgayChi.Value = r.NgayChi;
        txtNoiDung.Text = r.NoiDung;
        int idx = cboLoaiChi.Items.IndexOf(r.LoaiChi);
        cboLoaiChi.SelectedIndex = idx >= 0 ? idx : DanhSachLoai.Length - 1; // không khớp -> "Khác"
        nudSoTien.Value = Math.Min(Math.Max(r.SoTien, nudSoTien.Minimum), nudSoTien.Maximum);
        txtGhiChu.Text = r.GhiChu ?? string.Empty;
    }

    // ===================== THÊM / SỬA / XÓA / LÀM MỚI =====================

    private void btnThem_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        try
        {
            var entity = new ChiTieu();
            ReadInputTo(entity);

            using (var db = DbFactory.Create(_settings.ConnectionStrings.QL_ChiTieu))
            {
                db.ChiTieus.Add(entity);
                db.SaveChanges();
            }

            Log.Information("Thêm khoản chi MaCT={MaCT}, NoiDung={NoiDung}, LoaiChi={LoaiChi}, SoTien={SoTien}",
                entity.MaCt, entity.NoiDung, entity.LoaiChi, entity.SoTien);

            ClearInput();
            LoadData();
        }
        catch (Exception ex)
        {
            HandleDbError("thêm khoản chi", ex);
        }
    }

    private void btnSua_Click(object? sender, EventArgs e)
    {
        if (_selectedId == null)
        {
            Warn("Hãy chọn một dòng trong danh sách để sửa.", dgvChiTieu);
            return;
        }
        if (!ValidateInput()) return;

        int id = _selectedId.Value;
        try
        {
            bool found;
            using (var db = DbFactory.Create(_settings.ConnectionStrings.QL_ChiTieu))
            {
                ChiTieu? entity = db.ChiTieus.Find(id);
                found = entity != null;
                if (entity != null)
                {
                    ReadInputTo(entity);
                    db.SaveChanges();
                }
            }

            if (!found)
            {
                Warn("Khoản chi này không còn tồn tại (có thể đã bị xóa ở nơi khác).", null);
                LoadData();
                return;
            }

            Log.Information("Sửa khoản chi MaCT={MaCT}, NoiDung={NoiDung}, LoaiChi={LoaiChi}, SoTien={SoTien}",
                id, txtNoiDung.Text.Trim(), cboLoaiChi.SelectedItem, nudSoTien.Value);

            LoadData(selectId: id);
        }
        catch (Exception ex)
        {
            HandleDbError("sửa khoản chi", ex);
        }
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        if (_selectedId == null)
        {
            Warn("Hãy chọn một dòng trong danh sách để xóa.", dgvChiTieu);
            return;
        }

        int id = _selectedId.Value;
        DialogResult confirm = MessageBox.Show(
            $"Bạn có chắc muốn xóa khoản chi \"{txtNoiDung.Text}\" (Mã {id:000}) không?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (confirm != DialogResult.Yes) return;

        try
        {
            using (var db = DbFactory.Create(_settings.ConnectionStrings.QL_ChiTieu))
            {
                ChiTieu? entity = db.ChiTieus.Find(id);
                if (entity != null)
                {
                    db.ChiTieus.Remove(entity);
                    db.SaveChanges();
                }
            }

            Log.Information("Xóa khoản chi MaCT={MaCT}", id);

            ClearInput();
            LoadData();
        }
        catch (Exception ex)
        {
            HandleDbError("xóa khoản chi", ex);
        }
    }

    private void btnLamMoi_Click(object? sender, EventArgs e)
    {
        ClearInput();

        // Đưa bộ lọc về mặc định: tất cả loại, từ đầu tháng đến hôm nay
        txtTimKiem.Clear();
        _ready = false;
        cboLoaiLoc.SelectedIndex = 0;
        _ready = true;
        DateTime today = DateTime.Today;
        dtpTuNgay.Value = new DateTime(today.Year, today.Month, 1);
        dtpDenNgay.Value = today;

        LoadData();
    }

    // ===================== HỖ TRỢ =====================

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
        {
            Warn("Nội dung không được để trống.", txtNoiDung);
            return false;
        }
        if (cboLoaiChi.SelectedIndex < 0)
        {
            Warn("Hãy chọn loại chi.", cboLoaiChi);
            return false;
        }
        if (nudSoTien.Value <= 0)
        {
            Warn("Số tiền phải lớn hơn 0.", nudSoTien);
            return false;
        }
        if (dtpNgayChi.Value.Date > DateTime.Today)
        {
            Warn("Ngày chi không được lớn hơn ngày hôm nay.", dtpNgayChi);
            return false;
        }
        return true;
    }

    private void ReadInputTo(ChiTieu entity)
    {
        entity.NgayChi = DateOnly.FromDateTime(dtpNgayChi.Value.Date);
        entity.NoiDung = txtNoiDung.Text.Trim();
        entity.LoaiChi = (string)cboLoaiChi.SelectedItem!;
        entity.SoTien = nudSoTien.Value;
        entity.GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim();
    }

    private void ClearInput()
    {
        _loading = true;
        try
        {
            _selectedId = null;
            dtpNgayChi.Value = DateTime.Today;
            txtNoiDung.Clear();
            cboLoaiChi.SelectedIndex = 0;
            nudSoTien.Value = 0;
            txtGhiChu.Clear();
            dgvChiTieu.ClearSelection();
        }
        finally
        {
            _loading = false;
        }
        txtNoiDung.Focus();
    }

    private static void Warn(string message, Control? focus)
    {
        MessageBox.Show(message, TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        focus?.Focus();
    }

    /// <summary>Ghi log mức Error và báo cho người dùng một thông báo thân thiện.</summary>
    private static void HandleDbError(string action, Exception ex)
    {
        Log.Error(ex, "Lỗi truy cập CSDL khi {Action}", action);
        MessageBox.Show(
            $"Không thể {action}.\n\n" +
            "Vui lòng kiểm tra SQL Server đã chạy chưa và connection string trong file appsettings.json " +
            "có đúng không, rồi thử lại.\n\nChi tiết lỗi đã được ghi vào thư mục Logs.",
            TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
