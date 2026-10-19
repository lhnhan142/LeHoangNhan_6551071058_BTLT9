#nullable disable
namespace SoChiTieu;

partial class FormChiTieu
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        grpThongTin = new GroupBox();
        lblNgayChi = new Label();
        dtpNgayChi = new DateTimePicker();
        lblNoiDung = new Label();
        txtNoiDung = new TextBox();
        lblLoaiChi = new Label();
        cboLoaiChi = new ComboBox();
        lblSoTien = new Label();
        nudSoTien = new NumericUpDown();
        lblGhiChu = new Label();
        txtGhiChu = new TextBox();
        grpTimKiem = new GroupBox();
        lblTuKhoa = new Label();
        txtTimKiem = new TextBox();
        lblLoaiLoc = new Label();
        cboLoaiLoc = new ComboBox();
        lblTuNgay = new Label();
        dtpTuNgay = new DateTimePicker();
        lblDenNgay = new Label();
        dtpDenNgay = new DateTimePicker();
        btnTim = new Button();
        btnThem = new Button();
        btnSua = new Button();
        btnXoa = new Button();
        btnLamMoi = new Button();
        dgvChiTieu = new DataGridView();
        lblTong = new Label();
        grpThongTin.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudSoTien).BeginInit();
        grpTimKiem.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvChiTieu).BeginInit();
        SuspendLayout();
        //
        // grpThongTin
        //
        grpThongTin.Controls.Add(lblNgayChi);
        grpThongTin.Controls.Add(dtpNgayChi);
        grpThongTin.Controls.Add(lblNoiDung);
        grpThongTin.Controls.Add(txtNoiDung);
        grpThongTin.Controls.Add(lblLoaiChi);
        grpThongTin.Controls.Add(cboLoaiChi);
        grpThongTin.Controls.Add(lblSoTien);
        grpThongTin.Controls.Add(nudSoTien);
        grpThongTin.Controls.Add(lblGhiChu);
        grpThongTin.Controls.Add(txtGhiChu);
        grpThongTin.Location = new Point(12, 10);
        grpThongTin.Name = "grpThongTin";
        grpThongTin.Size = new Size(490, 200);
        grpThongTin.TabIndex = 0;
        grpThongTin.TabStop = false;
        grpThongTin.Text = "Thông tin khoản chi";
        //
        // lblNgayChi
        //
        lblNgayChi.AutoSize = true;
        lblNgayChi.Location = new Point(15, 32);
        lblNgayChi.Name = "lblNgayChi";
        lblNgayChi.Text = "Ngày chi";
        //
        // dtpNgayChi
        //
        dtpNgayChi.Format = DateTimePickerFormat.Custom;
        dtpNgayChi.CustomFormat = "dd/MM/yyyy";
        dtpNgayChi.Location = new Point(85, 28);
        dtpNgayChi.Name = "dtpNgayChi";
        dtpNgayChi.Size = new Size(150, 23);
        dtpNgayChi.TabIndex = 0;
        //
        // lblNoiDung
        //
        lblNoiDung.AutoSize = true;
        lblNoiDung.Location = new Point(15, 66);
        lblNoiDung.Name = "lblNoiDung";
        lblNoiDung.Text = "Nội dung";
        //
        // txtNoiDung
        //
        txtNoiDung.Location = new Point(85, 62);
        txtNoiDung.MaxLength = 100;
        txtNoiDung.Name = "txtNoiDung";
        txtNoiDung.Size = new Size(150, 23);
        txtNoiDung.TabIndex = 1;
        //
        // lblLoaiChi
        //
        lblLoaiChi.AutoSize = true;
        lblLoaiChi.Location = new Point(15, 100);
        lblLoaiChi.Name = "lblLoaiChi";
        lblLoaiChi.Text = "Loại chi";
        //
        // cboLoaiChi
        //
        cboLoaiChi.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoaiChi.Location = new Point(85, 96);
        cboLoaiChi.Name = "cboLoaiChi";
        cboLoaiChi.Size = new Size(150, 23);
        cboLoaiChi.TabIndex = 2;
        //
        // lblSoTien
        //
        lblSoTien.AutoSize = true;
        lblSoTien.Location = new Point(260, 32);
        lblSoTien.Name = "lblSoTien";
        lblSoTien.Text = "Số tiền";
        //
        // nudSoTien
        //
        nudSoTien.Location = new Point(320, 28);
        nudSoTien.Name = "nudSoTien";
        nudSoTien.Size = new Size(155, 23);
        nudSoTien.TabIndex = 3;
        nudSoTien.TextAlign = HorizontalAlignment.Right;
        //
        // lblGhiChu
        //
        lblGhiChu.AutoSize = true;
        lblGhiChu.Location = new Point(15, 136);
        lblGhiChu.Name = "lblGhiChu";
        lblGhiChu.Text = "Ghi chú";
        //
        // txtGhiChu
        //
        txtGhiChu.Location = new Point(85, 130);
        txtGhiChu.MaxLength = 200;
        txtGhiChu.Multiline = true;
        txtGhiChu.Name = "txtGhiChu";
        txtGhiChu.ScrollBars = ScrollBars.Vertical;
        txtGhiChu.Size = new Size(390, 56);
        txtGhiChu.TabIndex = 4;
        //
        // grpTimKiem
        //
        grpTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        grpTimKiem.Controls.Add(lblTuKhoa);
        grpTimKiem.Controls.Add(txtTimKiem);
        grpTimKiem.Controls.Add(lblLoaiLoc);
        grpTimKiem.Controls.Add(cboLoaiLoc);
        grpTimKiem.Controls.Add(lblTuNgay);
        grpTimKiem.Controls.Add(dtpTuNgay);
        grpTimKiem.Controls.Add(lblDenNgay);
        grpTimKiem.Controls.Add(dtpDenNgay);
        grpTimKiem.Controls.Add(btnTim);
        grpTimKiem.Location = new Point(512, 10);
        grpTimKiem.Name = "grpTimKiem";
        grpTimKiem.Size = new Size(346, 200);
        grpTimKiem.TabIndex = 1;
        grpTimKiem.TabStop = false;
        grpTimKiem.Text = "Tìm kiếm";
        //
        // lblTuKhoa
        //
        lblTuKhoa.AutoSize = true;
        lblTuKhoa.Location = new Point(15, 32);
        lblTuKhoa.Name = "lblTuKhoa";
        lblTuKhoa.Text = "Nội dung";
        //
        // txtTimKiem
        //
        txtTimKiem.Location = new Point(85, 28);
        txtTimKiem.Name = "txtTimKiem";
        txtTimKiem.PlaceholderText = "Nhập nội dung...";
        txtTimKiem.Size = new Size(245, 23);
        txtTimKiem.TabIndex = 0;
        txtTimKiem.KeyDown += txtTimKiem_KeyDown;
        //
        // lblLoaiLoc
        //
        lblLoaiLoc.AutoSize = true;
        lblLoaiLoc.Location = new Point(15, 66);
        lblLoaiLoc.Name = "lblLoaiLoc";
        lblLoaiLoc.Text = "Loại chi";
        //
        // cboLoaiLoc
        //
        cboLoaiLoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoaiLoc.Location = new Point(85, 62);
        cboLoaiLoc.Name = "cboLoaiLoc";
        cboLoaiLoc.Size = new Size(245, 23);
        cboLoaiLoc.TabIndex = 1;
        cboLoaiLoc.SelectedIndexChanged += cboLoaiLoc_SelectedIndexChanged;
        //
        // lblTuNgay
        //
        lblTuNgay.AutoSize = true;
        lblTuNgay.Location = new Point(15, 100);
        lblTuNgay.Name = "lblTuNgay";
        lblTuNgay.Text = "Từ ngày";
        //
        // dtpTuNgay
        //
        dtpTuNgay.Format = DateTimePickerFormat.Custom;
        dtpTuNgay.CustomFormat = "dd/MM/yyyy";
        dtpTuNgay.Location = new Point(85, 96);
        dtpTuNgay.Name = "dtpTuNgay";
        dtpTuNgay.Size = new Size(150, 23);
        dtpTuNgay.TabIndex = 2;
        //
        // lblDenNgay
        //
        lblDenNgay.AutoSize = true;
        lblDenNgay.Location = new Point(15, 136);
        lblDenNgay.Name = "lblDenNgay";
        lblDenNgay.Text = "Đến ngày";
        //
        // dtpDenNgay
        //
        dtpDenNgay.Format = DateTimePickerFormat.Custom;
        dtpDenNgay.CustomFormat = "dd/MM/yyyy";
        dtpDenNgay.Location = new Point(85, 132);
        dtpDenNgay.Name = "dtpDenNgay";
        dtpDenNgay.Size = new Size(150, 23);
        dtpDenNgay.TabIndex = 3;
        //
        // btnTim
        //
        btnTim.Location = new Point(250, 130);
        btnTim.Name = "btnTim";
        btnTim.Size = new Size(80, 28);
        btnTim.TabIndex = 4;
        btnTim.Text = "Tìm";
        btnTim.UseVisualStyleBackColor = true;
        btnTim.Click += btnTim_Click;
        //
        // btnThem
        //
        btnThem.Location = new Point(20, 222);
        btnThem.Name = "btnThem";
        btnThem.Size = new Size(90, 32);
        btnThem.TabIndex = 2;
        btnThem.Text = "Thêm";
        btnThem.UseVisualStyleBackColor = true;
        btnThem.Click += btnThem_Click;
        //
        // btnSua
        //
        btnSua.Location = new Point(125, 222);
        btnSua.Name = "btnSua";
        btnSua.Size = new Size(90, 32);
        btnSua.TabIndex = 3;
        btnSua.Text = "Sửa";
        btnSua.UseVisualStyleBackColor = true;
        btnSua.Click += btnSua_Click;
        //
        // btnXoa
        //
        btnXoa.Location = new Point(230, 222);
        btnXoa.Name = "btnXoa";
        btnXoa.Size = new Size(90, 32);
        btnXoa.TabIndex = 4;
        btnXoa.Text = "Xóa";
        btnXoa.UseVisualStyleBackColor = true;
        btnXoa.Click += btnXoa_Click;
        //
        // btnLamMoi
        //
        btnLamMoi.Location = new Point(335, 222);
        btnLamMoi.Name = "btnLamMoi";
        btnLamMoi.Size = new Size(90, 32);
        btnLamMoi.TabIndex = 5;
        btnLamMoi.Text = "Làm mới";
        btnLamMoi.UseVisualStyleBackColor = true;
        btnLamMoi.Click += btnLamMoi_Click;
        //
        // dgvChiTieu
        //
        dgvChiTieu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvChiTieu.BackgroundColor = SystemColors.Window;
        dgvChiTieu.Location = new Point(12, 266);
        dgvChiTieu.Name = "dgvChiTieu";
        dgvChiTieu.Size = new Size(846, 205);
        dgvChiTieu.TabIndex = 6;
        dgvChiTieu.SelectionChanged += dgvChiTieu_SelectionChanged;
        //
        // lblTong
        //
        lblTong.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblTong.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTong.ForeColor = Color.Red;
        lblTong.Location = new Point(508, 480);
        lblTong.Name = "lblTong";
        lblTong.Size = new Size(350, 28);
        lblTong.TabIndex = 7;
        lblTong.Text = "Tổng chi: 0 đ";
        lblTong.TextAlign = ContentAlignment.MiddleRight;
        //
        // FormChiTieu
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(870, 516);
        Controls.Add(grpThongTin);
        Controls.Add(grpTimKiem);
        Controls.Add(btnThem);
        Controls.Add(btnSua);
        Controls.Add(btnXoa);
        Controls.Add(btnLamMoi);
        Controls.Add(dgvChiTieu);
        Controls.Add(lblTong);
        MinimumSize = new Size(886, 555);
        Name = "FormChiTieu";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sổ chi tiêu cá nhân";
        Load += FormChiTieu_Load;
        grpThongTin.ResumeLayout(false);
        grpThongTin.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudSoTien).EndInit();
        grpTimKiem.ResumeLayout(false);
        grpTimKiem.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvChiTieu).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox grpThongTin;
    private Label lblNgayChi;
    private DateTimePicker dtpNgayChi;
    private Label lblNoiDung;
    private TextBox txtNoiDung;
    private Label lblLoaiChi;
    private ComboBox cboLoaiChi;
    private Label lblSoTien;
    private NumericUpDown nudSoTien;
    private Label lblGhiChu;
    private TextBox txtGhiChu;
    private GroupBox grpTimKiem;
    private Label lblTuKhoa;
    private TextBox txtTimKiem;
    private Label lblLoaiLoc;
    private ComboBox cboLoaiLoc;
    private Label lblTuNgay;
    private DateTimePicker dtpTuNgay;
    private Label lblDenNgay;
    private DateTimePicker dtpDenNgay;
    private Button btnTim;
    private Button btnThem;
    private Button btnSua;
    private Button btnXoa;
    private Button btnLamMoi;
    private DataGridView dgvChiTieu;
    private Label lblTong;
}
