using Serilog;

namespace QuanLyTraSua;

/// <summary>
/// Form chính: chứa MenuStrip để chuyển đổi giữa 3 màn hình
/// (Bán hàng - Lập đơn / Quản lý Thực đơn / Danh sách Hóa đơn).
/// </summary>
public class FormMain : Form
{
    private readonly MenuStrip _menuStrip = new();
    private readonly Panel _panelNoiDung = new();
    private readonly ToolStripStatusLabel _lblTrangThai = new();
    private Form? _formDangHienThi;

    public FormMain()
    {
        Text = Program.PaymentSettings.TenQuan + " - Quản lý bán hàng";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1150, 700);
        Size = new Size(1366, 800);
        Font = new Font("Segoe UI", 9.5F);

        // ---------------- MenuStrip ----------------
        var mnuBanHang = new ToolStripMenuItem("Bán hàng (Lập đơn)");
        var mnuThucDon = new ToolStripMenuItem("Quản lý Thực đơn");
        var mnuHoaDon = new ToolStripMenuItem("Danh sách Hóa đơn");
        var mnuThoat = new ToolStripMenuItem("Thoát");

        mnuBanHang.Click += (_, _) => MoForm(new FormBanHang());
        mnuThucDon.Click += (_, _) => MoForm(new FormThucDon());
        mnuHoaDon.Click += (_, _) => MoForm(new FormDanhSachHoaDon());
        mnuThoat.Click += (_, _) => Close();

        _menuStrip.Items.AddRange(new ToolStripItem[] { mnuBanHang, mnuThucDon, mnuHoaDon, mnuThoat });
        _menuStrip.Dock = DockStyle.Top;
        _menuStrip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _menuStrip.Padding = new Padding(6, 3, 0, 3);

        // ---------------- Panel nội dung ----------------
        _panelNoiDung.Dock = DockStyle.Fill;
        _panelNoiDung.Padding = new Padding(6);
        _panelNoiDung.BackColor = Color.WhiteSmoke;

        // ---------------- StatusStrip ----------------
        var statusStrip = new StatusStrip { SizingGrip = false };
        _lblTrangThai.Text = "Sẵn sàng";
        _lblTrangThai.ForeColor = Color.DarkGreen;
        statusStrip.Items.Add(_lblTrangThai);

        Controls.Add(_panelNoiDung);
        Controls.Add(statusStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;

        // Mở mặc định màn hình bán hàng
        Shown += (_, _) => MoForm(new FormBanHang());
    }

    /// <summary>Hiển thị một form con (không viền) trong panel nội dung.</summary>
    private void MoForm(Form form)
    {
        try
        {
            _formDangHienThi?.Close();
            _formDangHienThi?.Dispose();

            _formDangHienThi = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            _panelNoiDung.Controls.Clear();
            _panelNoiDung.Controls.Add(form);
            form.Show();

            _lblTrangThai.Text = "Đang mở: " + form.Text;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi khi mở form {Form}", form.GetType().Name);
            MessageBox.Show("Không mở được màn hình: " + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
