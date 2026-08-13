using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dangnhap
{
    public partial class QuanLySach : Form
    {
        public QuanLySach()
        {
            InitializeComponent();
        }
        // Khai báo chuỗi kết nối
        string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";

        // Khai báo đối tượng kết nối
        SqlConnection sqlCon = null;

        DataTable dt = null;
        SqlDataAdapter da = null;

        // Lưu vị trí dòng đang chọn
        int vt = -1;

        // 1 = Thêm
        // 2 = Sửa
        int luuChon = 0;
        //Ham kết nối cơ sở dữ liệu
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
        //Ham hiển thị danh sách sách
        private void HienThiDanhSachSach()
        {
            sqlCon = MoKetNoi();

            if (sqlCon != null)
            {
                try
                {
                    SqlCommand sqlCmd = new SqlCommand();

                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM tblSach";

                    // Gán kết nối
                    sqlCmd.Connection = sqlCon;

                    dt = new DataTable();

                    da = new SqlDataAdapter();

                    da.SelectCommand = sqlCmd;

                    da.Fill(dt);

                    dgvDSS.DataSource = dt;

                    // Tạo lệnh INSERT, UPDATE, DELETE
                    SqlCommandBuilder builder = new SqlCommandBuilder(da);

                    dgvDSS.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    dgvDSS.ReadOnly = true;

                    dgvDSS.AllowUserToAddRows = false;

                    // Đặt tên cột hiển thị
                    dgvDSS.Columns["MaSach"].HeaderText = "Mã Sách";
                    dgvDSS.Columns["TenSach"].HeaderText = "Tên Sách";
                    dgvDSS.Columns["TacGia"].HeaderText = "Tác Giả";
                    dgvDSS.Columns["TheLoai"].HeaderText = "Thể Loại";
                    dgvDSS.Columns["NXB"].HeaderText = "NXB";
                    dgvDSS.Columns["SoLuong"].HeaderText = "Số Lượng";
                    dgvDSS.Columns["MoTa"].HeaderText = "Mô Tả";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Lỗi hiển thị danh sách sách: " + ex.Message,
                        "Thông báo lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    "Không mở được kết nối",
                    "Hộp thoại",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void dgvDanhSachSach_CellClick(
    object sender,
    DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            vt = e.RowIndex;

            DataGridViewRow row =dgvDSS.Rows[vt];

            txtMaSach.Text =row.Cells["MaSach"].Value.ToString();

            txtTenSach.Text =row.Cells["TenSach"].Value.ToString();

            txtTacGia.Text =row.Cells["TacGia"].Value.ToString();

            cbbTheLoai.Text =row.Cells["TheLoai"].Value.ToString();

            txtNXB.Text =row.Cells["NXB"].Value.ToString();

            txtSoLuong.Text =row.Cells["SoLuong"].Value.ToString();

            txtMoTa.Text =row.Cells["MoTa"].Value.ToString();

            btnSua.Enabled = true;
            btnXoa.Enabled = true;

            gbThongtinchitiet.Enabled = false;
        }
        //Hàm Thêm sách
        private bool ThemMoiSach(List<string> sach)
        {
            try
            {
                DataRow newRow = dt.NewRow();

                newRow["MaSach"] = sach[0];
                newRow["TenSach"] = sach[1];
                newRow["TacGia"] = sach[2];
                newRow["TheLoai"] = sach[3];
                newRow["NXB"] = sach[4];
                newRow["SoLuong"] = sach[5];
                newRow["MoTa"] = sach[6];

                dt.Rows.Add(newRow);

                dgvDSS.DataSource = dt;

                int kq = da.Update(dt);

                if (kq > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm sách: " + ex.Message,
                    "Thông báo lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }
        //Hàm Sửa sách
        private bool ChinhSuaTTSach(List<string> sach)
        {
            try
            {
                dt.Rows[vt]["MaSach"] = sach[0];
                dt.Rows[vt]["TenSach"] = sach[1];
                dt.Rows[vt]["TacGia"] = sach[2];
                dt.Rows[vt]["TheLoai"] = sach[3];
                dt.Rows[vt]["NXB"] = sach[4];
                dt.Rows[vt]["SoLuong"] = sach[5];
                dt.Rows[vt]["MoTa"] = sach[6];

                int kq = da.Update(dt);

                if (kq > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi sửa sách: " + ex.Message,
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

        private void QuanLySach_Load(object sender, EventArgs e)
        {
            HienThiDanhSachSach();

            gbThongtinchitiet.Enabled = false;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            // Danh sách thể loại
            cbbTheLoai.Items.Clear();

            cbbTheLoai.Items.Add("Lập trình");
            cbbTheLoai.Items.Add("Công nghệ thông tin");
            cbbTheLoai.Items.Add("Khoa học");
            cbbTheLoai.Items.Add("Văn học");
            cbbTheLoai.Items.Add("Kinh tế");
            cbbTheLoai.Items.Add("Ngoại ngữ");
            cbbTheLoai.Items.Add("Trí tuệ nhân tạo");

            cbbTheLoai.SelectedIndex = 0;

            // Mặc định tìm theo mã
            radMaSach.Checked = true;
        }

        private void btnThêm_Click(object sender, EventArgs e)
        {
            luuChon = 1;

            gbThongtinchitiet.Enabled = true;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            txtMaSach.Clear();
            txtTenSach.Clear();
            txtTacGia.Clear();
            txtNXB.Clear();
            txtSoLuong.Clear();
            txtMoTa.Clear();

            cbbTheLoai.SelectedIndex = 0;

            txtMaSach.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (vt == -1)
            {
                MessageBox.Show("Vui lòng chọn sách cần sửa!");
                return;
            }

            luuChon = 2;

            gbThongtinchitiet.Enabled = true;

            // Không cho sửa mã sách
            txtMaSach.Enabled = false;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu người dùng nhập
            string maSach = txtMaSach.Text.Trim();
            string tenSach = txtTenSach.Text.Trim();
            string tacGia = txtTacGia.Text.Trim();
            string theLoai = cbbTheLoai.Text.Trim();
            string nxb = txtNXB.Text.Trim();
            string moTa = txtMoTa.Text.Trim();

            // Kiểm tra mã sách
            if (maSach == "")
            {
                MessageBox.Show("Vui lòng nhập mã sách!");
                txtMaSach.Focus();
                return;
            }

            // Kiểm tra tên sách
            if (tenSach == "")
            {
                MessageBox.Show("Vui lòng nhập tên sách!");
                txtTenSach.Focus();
                return;
            }

            // Kiểm tra tác giả
            if (tacGia == "")
            {
                MessageBox.Show("Vui lòng nhập tác giả!");
                txtTacGia.Focus();
                return;
            }

            // Kiểm tra số lượng
            int soLuong;

            if (!int.TryParse(txtSoLuong.Text.Trim(), out soLuong))
            {
                MessageBox.Show("Số lượng phải là số nguyên!");
                txtSoLuong.Focus();
                return;
            }

            if (soLuong < 0)
            {
                MessageBox.Show("Số lượng không được nhỏ hơn 0!");
                txtSoLuong.Focus();
                return;
            }

            // Tạo danh sách sách
            List<string> sach = new List<string>();

            sach.Add(maSach);
            sach.Add(tenSach);
            sach.Add(tacGia);
            sach.Add(theLoai);
            sach.Add(nxb);
            sach.Add(soLuong.ToString());
            sach.Add(moTa);

            // Thêm sách

            if (luuChon == 1)
            {
                bool kq = ThemMoiSach(sach);

                if (kq == true)
                {
                    MessageBox.Show(
                        "Thêm sách thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HienThiDanhSachSach();
                }
                else
                {
                    MessageBox.Show(
                        "Thêm sách không thành công!");
                }
            }

            // Sửa sách
            else if (luuChon == 2)
            {
                bool kq = ChinhSuaTTSach(sach);

                if (kq == true)
                {
                    MessageBox.Show(
                        "Chỉnh sửa thông tin sách thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HienThiDanhSachSach();
                }
                else
                {
                    MessageBox.Show(
                        "Chỉnh sửa thông tin sách không thành công!");
                }
            }

            // Xóa dữ liệu
            txtMaSach.Clear();
            txtTenSach.Clear();
            txtTacGia.Clear();
            txtNXB.Clear();
            txtSoLuong.Clear();
            txtMoTa.Clear();

            cbbTheLoai.SelectedIndex = 0;

            txtMaSach.Enabled = true;

            gbThongtinchitiet.Enabled = false;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            luuChon = 0;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (vt == -1)
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có thực sự muốn xóa sách này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool kq = XoaSach();

                if (kq == true)
                {
                    MessageBox.Show(
                        "Xóa sách thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HienThiDanhSachSach();
                }
                else
                {
                    MessageBox.Show(
                        "Xóa sách không thành công!");
                }

                btnSua.Enabled = false;
                btnXoa.Enabled = false;

                vt = -1;
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            HienThiDanhSachSach();

            txtMaSach.Clear();
            txtTenSach.Clear();
            txtTacGia.Clear();
            txtNXB.Clear();
            txtSoLuong.Clear();
            txtMoTa.Clear();

            cbbTheLoai.SelectedIndex = 0;

            gbThongtinchitiet.Enabled = false;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            txtMaSach.Enabled = true;

            vt = -1;
            luuChon = 0;

            dgvDSS.ClearSelection();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu
            txtMaSach.Clear();
            txtTenSach.Clear();
            txtTacGia.Clear();
            txtNXB.Clear();
            txtSoLuong.Clear();
            txtMoTa.Clear();

            cbbTheLoai.SelectedIndex = 0;

            // Mở lại mã sách
            txtMaSach.Enabled = true;

            // Khóa phần chi tiết
            gbThongtinchitiet.Enabled = false;

            // Khóa sửa xóa
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            // Reset
            luuChon = 0;
            vt = -1;

            dgvDSS.ClearSelection();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Bạn có chắc chắn muốn thoát khỏi quản lý sách không?",
        "Xác nhận thoát",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = "";

            if (radMaSach.Checked)
            {
                tuKhoa = txtTKMaSach.Text.Trim();
            }
            else if (radTenSach.Checked)
            {
                tuKhoa = txtTkTenSach.Text.Trim();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tiêu chí tìm kiếm!");
                return;
            }

            if (tuKhoa == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin cần tìm!");
                return;
            }

            try
            {
                DataView dv = dt.DefaultView;

                if (radMaSach.Checked)
                {
                    dv.RowFilter = "MaSach LIKE '%" + tuKhoa + "%'";
                }
                else
                {
                    dv.RowFilter = "TenSach LIKE '%" + tuKhoa + "%'";
                }

                dgvDSS.DataSource = dv;

                if (dv.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sách!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }
    }
}
