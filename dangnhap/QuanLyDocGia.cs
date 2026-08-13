using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace dangnhap
{
    public partial class QuanLyDocGia : Form
    {
        public QuanLyDocGia()
        {
            InitializeComponent();
        }
        //Khai bao chuoi ket noi
        string strCon = @"Data Source=embemilo;Initial Catalog=qldg;Integrated Security=True;Encrypt=False";

        //Khai bao doi tuong ket noi
        SqlConnection sqlCon = null;

        DataTable dt = null;
        SqlDataAdapter da = null;

        // Vị trí dòng đang chọn
        int vt = -1;

        // 1 = Thêm
        // 2 = Sửa
        int luuChon = 0;

        //ham mo ket noi
        private SqlConnection MoKetNoi()
        {
            try
            {
                if (sqlCon == null)
                {
                    sqlCon = new SqlConnection(strCon);
                }

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                return sqlCon;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối: " + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return null;
            }
        }

        //ham hien thi danh sach san pham
        private void HienThiDanhSachDocGia()
        {
            sqlCon = MoKetNoi();

            if (sqlCon == null)
                return;

            try
            {
                SqlCommand sqlCmd = new SqlCommand();

                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = "SELECT * FROM tblDocGia";

                sqlCmd.Connection = sqlCon;

                dt = new DataTable();

                da = new SqlDataAdapter();

                da.SelectCommand = sqlCmd;

                da.Fill(dt);

                dgvDanhsachDG.DataSource = dt;

                // Tạo lệnh INSERT, UPDATE, DELETE
                SqlCommandBuilder builder = new SqlCommandBuilder(da);

                dgvDanhsachDG.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dgvDanhsachDG.ReadOnly = true;

                dgvDanhsachDG.AllowUserToAddRows = false;

                // Đặt tên cột hiển thị
                dgvDanhsachDG.Columns["MaDG"].HeaderText ="Mã ĐG";

                dgvDanhsachDG.Columns["TenDG"].HeaderText ="Tên ĐG";

                dgvDanhsachDG.Columns["GioiTinh"].HeaderText ="Giới Tính";

                dgvDanhsachDG.Columns["NgaySinh"].HeaderText ="Ngày Sinh";

                dgvDanhsachDG.Columns["SDT"].HeaderText ="SĐT";

                dgvDanhsachDG.Columns["DiaChi"].HeaderText ="Địa Chỉ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi hiển thị danh sách độc giả: "
                    + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        //ham them doc gia
        private bool ThemMoiDocGia(
            List<string> docGia)
        {
            try
            {
                DataRow newRow = dt.NewRow();

                newRow["MaDG"] = docGia[0];
                newRow["TenDG"] = docGia[1];
                newRow["GioiTinh"] = docGia[2];
                newRow["NgaySinh"] = Convert.ToDateTime(docGia[3]);
                newRow["SDT"] = docGia[4];
                newRow["DiaChi"] = docGia[5];

                dt.Rows.Add(newRow);

                dgvDanhsachDG.DataSource = dt;

                int kq = da.Update(dt);

                if (kq > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm độc giả: "
                    + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        //Sua thong tin doc gia
        private bool ChinhSuaTTDocGia(List<string> docGia)
        {
            try
            {
                dt.Rows[vt]["MaDG"] =docGia[0];

                dt.Rows[vt]["TenDG"] =docGia[1];

                dt.Rows[vt]["GioiTinh"] =docGia[2];

                dt.Rows[vt]["NgaySinh"] =Convert.ToDateTime(docGia[3]);

                dt.Rows[vt]["SDT"] =docGia[4];

                dt.Rows[vt]["DiaChi"] =docGia[5];

                int kq = da.Update(dt);

                if (kq > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi sửa độc giả: "
                    + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }
        //Hàm Xóa sách
        private bool XoaSach()
        {
            try
            {
                dt.Rows[vt].Delete();

                int kq = da.Update(dt);

                if (kq > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa sách: " + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnThêm_Click(object sender, EventArgs e)
        {
            luuChon = 1;

            gbThongtinchitiet.Enabled = true;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            txtMaDG.Clear();
            txtTenDG.Clear();
            txtSĐT.Clear();
            txtDiaChi.Clear();

            cbbGioiTinh.SelectedIndex = 0;

            dtpNgaySinh.Value = DateTime.Now;

            txtMaDG.Enabled = true;

            txtMaDG.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (vt == -1)
            {
                MessageBox.Show(
                    "Vui lòng chọn độc giả cần sửa!");

                return;
            }

            luuChon = 2;

            gbThongtinchitiet.Enabled = true;

            // Không cho sửa mã độc giả
            txtMaDG.Enabled = false;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maDG = txtMaDG.Text.Trim();

            string tenDG =txtTenDG.Text.Trim();

            string gioiTinh =cbbGioiTinh.Text.Trim();

            string ngaySinh =dtpNgaySinh.Value.ToString("yyyy-MM-dd");

            string sdt =txtSĐT.Text.Trim();

            string diaChi =txtDiaChi.Text.Trim();


            // Kiểm tra mã
            if (maDG == "")
            {
                MessageBox.Show(
                    "Vui lòng nhập mã độc giả!");

                txtMaDG.Focus();

                return;
            }


            // Kiểm tra tên
            if (tenDG == "")
            {
                MessageBox.Show(
                    "Vui lòng nhập tên độc giả!");

                txtTenDG.Focus();

                return;
            }


            // Kiểm tra giới tính
            if (gioiTinh == "")
            {
                MessageBox.Show(
                    "Vui lòng chọn giới tính!");

                cbbGioiTinh.Focus();

                return;
            }


            // Kiểm tra số điện thoại
            if (sdt == "")
            {
                MessageBox.Show(
                    "Vui lòng nhập số điện thoại!");

                txtSĐT.Focus();

                return;
            }


            // Kiểm tra địa chỉ
            if (diaChi == "")
            {
                MessageBox.Show(
                    "Vui lòng nhập địa chỉ!");

                txtDiaChi.Focus();

                return;
            }


            // Tạo danh sách độc giả
            List<string> docGia =
                new List<string>();

            docGia.Add(maDG);
            docGia.Add(tenDG);
            docGia.Add(gioiTinh);
            docGia.Add(ngaySinh);
            docGia.Add(sdt);
            docGia.Add(diaChi);
            //Thêm mới độc giả

            if (luuChon == 1)
            {
                bool kq =
                    ThemMoiDocGia(docGia);

                if (kq == true)
                {
                    MessageBox.Show(
                        "Thêm độc giả thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HienThiDanhSachDocGia();
                }
                else
                {
                    MessageBox.Show(
                        "Thêm độc giả không thành công!");
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (vt == -1)
            {
                MessageBox.Show(
                    "Vui lòng chọn độc giả cần xóa!");

                return;
            }

            DialogResult result =
                MessageBox.Show(
                    "Bạn có thực sự muốn xóa độc giả này không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool kq = XoaDocGia();

                if (kq == true)
                {
                    MessageBox.Show(
                        "Xóa độc giả thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HienThiDanhSachDocGia();
                }
                else
                {
                    MessageBox.Show(
                        "Xóa độc giả không thành công!");
                }

                btnSua.Enabled = false;
                btnXoa.Enabled = false;

                vt = -1;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
                DialogResult result =
                MessageBox.Show(
                    "Bạn có chắc chắn muốn thoát khỏi quản lý độc giả không?",
                    "Xác nhận thoát",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {

        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTKMaDG.Text.Trim();

            if (tuKhoa == "")
            {
                MessageBox.Show(
                    "Vui lòng nhập mã hoặc tên độc giả cần tìm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTKMaDG.Focus();

                return;
            }

            sqlCon = MoKetNoi();

            if (sqlCon == null)
                return;

            try
            {
                string query = "";

                // Tìm theo mã
                if (radMaDG.Checked)
                {
                    query ="SELECT * FROM tblDocGia " + "WHERE MaDG LIKE @TuKhoa";
                }

                // Tìm theo tên
                else if (radTenDG.Checked)
                {
                    query ="SELECT * FROM tblDocGia " + "WHERE TenDG LIKE @TuKhoa";
                }

                else
                {
                    MessageBox.Show(
                        "Vui lòng chọn tiêu chí tìm kiếm!");

                    return;
                }


                SqlCommand cmd =
                    new SqlCommand(query, sqlCon);

                cmd.Parameters.AddWithValue(
                    "@TuKhoa",
                    "%" + tuKhoa + "%");


                // Cập nhật lại dt và da
                // để sau tìm kiếm vẫn Sửa/Xóa được
                dt = new DataTable();

                da = new SqlDataAdapter(cmd);

                da.Fill(dt);

                dgvDanhsachDG.DataSource = dt;


                // Cấu hình DataGridView
                dgvDanhsachDG.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dgvDanhsachDG.ReadOnly = true;

                dgvDanhsachDG.AllowUserToAddRows = false;


                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Không tìm thấy độc giả!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    vt = -1;

                    btnSua.Enabled = false;
                    btnXoa.Enabled = false;
                }
                else
                {
                    dgvDanhsachDG.ClearSelection();

                    vt = -1;

                    btnSua.Enabled = false;
                    btnXoa.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm: "
                    + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void dgvDanhsachDG_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            vt = e.RowIndex;

            DataGridViewRow row = dgvDanhsachDG.Rows[vt];

            txtMaDG.Text = row.Cells["MaDG"].Value.ToString();

            txtTenDG.Text = row.Cells["TenDG"].Value.ToString();

            cbbGioiTinh.Text = row.Cells["GioiTinh"].Value.ToString();

            // Ngày sinh
            if (row.Cells["NgaySinh"].Value != null &&
                row.Cells["NgaySinh"].Value != DBNull.Value)
            {
                dtpNgaySinh.Value =Convert.ToDateTime(row.Cells["NgaySinh"].Value);
            }

            txtSĐT.Text = row.Cells["SDT"].Value.ToString();

            txtDiaChi.Text = row.Cells["DiaChi"].Value.ToString();

            btnSua.Enabled = true;
            btnXoa.Enabled = true;

            gbThongtinchitiet.Enabled = false;
        }
        private void QuanLyDocGia_Load(object sender, EventArgs e)
        {
            HienThiDanhSachDocGia();

            gbThongtinchitiet.Enabled = false;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            // Giới tính
            cbbGioiTinh.Items.Clear();

            cbbGioiTinh.Items.Add("Nam");
            cbbGioiTinh.Items.Add("Nữ");

            cbbGioiTinh.SelectedIndex = 0;

            // Mặc định tìm theo mã
            radMaDG.Checked = true;
        }

        private void dgvDanhsachDG_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        //Xoa Doc Gia
        private bool XoaDocGia()
        {
            try
            {
                dt.Rows[vt].Delete();

                int kq = da.Update(dt);

                if (kq > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa độc giả: "
                    + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            HienThiDanhSachDocGia();

            // Xóa ô tìm kiếm
            txtTKMaDG.Clear();
            txtTkTenDG.Clear();

            // Xóa thông tin chi tiết
            txtMaDG.Clear();
            txtTenDG.Clear();
            txtSĐT.Clear();
            txtDiaChi.Clear();

            cbbGioiTinh.SelectedIndex = 0;


            dtpNgaySinh.Value = DateTime.Now;

            gbThongtinchitiet.Enabled = false;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            txtMaDG.Enabled = true;

            vt = -1;
            luuChon = 0;

            dgvDanhsachDG.ClearSelection();
        }

        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            txtMaDG.Clear();
            txtTenDG.Clear();
            txtSĐT.Clear();
            txtDiaChi.Clear();

            cbbGioiTinh.SelectedIndex = 0;

            dtpNgaySinh.Value = DateTime.Now;

            txtMaDG.Enabled = true;

            gbThongtinchitiet.Enabled = false;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            luuChon = 0;
            vt = -1;

            dgvDanhsachDG.ClearSelection();
        }

        private void btnTimKiem_Click_1(object sender, EventArgs e)
        {
            string tuKhoa = "";

            if (radMaDG.Checked)
            {
                tuKhoa = txtTKMaDG.Text.Trim();
            }
            else if (radTenDG.Checked)
            {
                tuKhoa = txtTkTenDG.Text.Trim();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tiêu chí tìm kiếm!");
                return;
            }

            if (tuKhoa == "")
            {
                MessageBox.Show("Vui lòng nhập mã hoặc tên độc giả cần tìm!");
                return;
            }

            sqlCon = MoKetNoi();

            if (sqlCon == null)
                return;

            try
            {
                string query = "";

                if (radMaDG.Checked)
                {
                    query = "SELECT * FROM tblDocGia WHERE MaDG LIKE @TuKhoa";
                }
                else
                {
                    query = "SELECT * FROM tblDocGia WHERE TenDG LIKE @TuKhoa";
                }

                SqlCommand cmd = new SqlCommand(query, sqlCon);

                cmd.Parameters.AddWithValue(
                    "@TuKhoa",
                    "%" + tuKhoa + "%");

                dt = new DataTable();

                da = new SqlDataAdapter(cmd);

                da.Fill(dt);

                dgvDanhsachDG.DataSource = dt;

                dgvDanhsachDG.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dgvDanhsachDG.ReadOnly = true;

                dgvDanhsachDG.AllowUserToAddRows = false;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy độc giả!");
                }

                vt = -1;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm: " + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void btnThoat_Click_1(object sender, EventArgs e)
        {

        }

        private void btnLuu_Click_1(object sender, EventArgs e)
        {

        }
    }
}
