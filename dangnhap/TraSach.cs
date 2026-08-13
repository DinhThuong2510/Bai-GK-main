using System;
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
    public partial class TraSach : Form
    {
        public TraSach()
        {
            InitializeComponent();
            LoadDanhSachTraSach();
        }
        private void LoadDanhSachTraSach()
        {
            string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";
            using (SqlConnection sqlCon = new SqlConnection(strCon))
            {
                try
                {
                    sqlCon.Open();
                    // Chỉ lấy những phiếu mượn có trạng thái chưa trả (TrangThai = 0)
                    string query = "SELECT MaPM AS [Mã PM], MaDG AS [Mã ĐG], MaSach AS [Mã Sách], NgayMuon AS [Ngày Mượn], HanTra AS [Hạn Trả] FROM tblPhieuMuon WHERE TrangThai = 0";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvDanhSach.DataSource = dt;
                    dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDanhSach.Rows[e.RowIndex];

                // Đẩy dữ liệu cơ bản lên các ô bên phải
                txtMaPM.Text = row.Cells["Mã PM"].Value.ToString();
                txtMaDG.Text = row.Cells["Mã ĐG"].Value.ToString();
                txtMaSach.Text = row.Cells["Mã Sách"].Value.ToString();

                // Lấy hạn trả từ bảng lên
                DateTime hanTra = Convert.ToDateTime(row.Cells["Hạn Trả"].Value);
                DateTime ngayTra = dtpNgayTra.Value; // Ngày trả là ngày hiện tại trên giao diện

                // Tính số ngày quá hạn
                TimeSpan khoangCach = ngayTra.Date - hanTra.Date;
                int soNgayQuaHan = khoangCach.Days;

                if (soNgayQuaHan > 0)
                {
                    // Xóa dữ liệu cũ trong ComboBox (nếu có) rồi thêm số ngày vào
                    cbbQuaHan.Items.Clear();
                    cbbQuaHan.Items.Add(soNgayQuaHan.ToString());
                    cbbQuaHan.SelectedIndex = 0; // Tự động chọn dòng đầu tiên vừa thêm

                    // Tính tiền phạt như bình thường
                    decimal tienPhat = soNgayQuaHan * 5000;
                    txtTienPhat.Text = tienPhat.ToString("N0");
                }
                else
                {
                    cbbQuaHan.Items.Clear();
                    cbbQuaHan.Items.Add("0");
                    cbbQuaHan.SelectedIndex = 0;
                    txtTienPhat.Text = "0";
                }
            }
        }

        private void btnLuuPhieu_Click(object sender, EventArgs e)
        {
            string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";
            using (SqlConnection sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                SqlTransaction transaction = sqlCon.BeginTransaction(); // Bắt đầu Transaction đảm bảo an toàn dữ liệu

                try
                {
                    // 1. Cập nhật phiếu mượn thành đã trả (TrangThai = 1)
                    string updatePhieu = "UPDATE tblPhieuMuon SET TrangThai = 1 WHERE MaPM = @MaPM";
                    SqlCommand cmdPhieu = new SqlCommand(updatePhieu, sqlCon, transaction);
                    cmdPhieu.Parameters.AddWithValue("@MaPM", txtMaPM.Text);
                    cmdPhieu.ExecuteNonQuery();

                    // 2. Cộng trả 1 sách vào kho (Tìm sách dựa vào Mã Sách trên phiếu)
                    string updateSach = "UPDATE tblSach SET SoLuong = SoLuong + 1 WHERE MaSach = @MaSach";
                    SqlCommand cmdSach = new SqlCommand(updateSach, sqlCon, transaction);
                    cmdSach.Parameters.AddWithValue("@MaSach", txtMaSach.Text);
                    cmdSach.ExecuteNonQuery();

                    transaction.Commit(); // Chốt giao dịch
                    MessageBox.Show("Trả sách thành công! Kho đã được cập nhật.");

                    LoadDanhSachTraSach(); // Reload lại bảng
                                           // Xóa trắng các ô nhập liệu sau khi lưu
                    txtMaPM.Clear(); txtMaDG.Clear(); txtMaSach.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Nếu lỗi thì hủy toàn bộ, không bị mất dữ liệu
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadDanhSachTraSach();
        }
        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu các ô TextBox
            txtMaPM.Clear();
            txtMaDG.Clear();
            txtMaSach.Clear();
            txtTienPhat.Clear();

            // Xóa dữ liệu ComboBox và reset ngày tháng
            cbbQuaHan.Items.Clear(); // Xóa sạch danh sách trong CBB (nếu có)
            dtpNgayTra.Value = DateTime.Now; // Đưa ngày về hiện tại

            // Đưa con trỏ chuột về ô Mã PM để sẵn sàng nhập mới
            txtMaPM.Focus();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
