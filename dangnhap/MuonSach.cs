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
    public partial class MuonSach : Form
    {
        public MuonSach()
        {
            InitializeComponent();
            LoadDanhSachSach();
        }

        private void LoadDanhSachSach()
        {
            string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";
            using (SqlConnection sqlCon = new SqlConnection(strCon))
            {
                try
                {
                    sqlCon.Open();
                    string query = "SELECT MaSach AS [Mã Sách], TenSach AS [Tên Sách], TacGia AS [Tác Giả], SoLuong AS [Số Lượng Còn] FROM tblSach";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvDanhSach.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có click vào phần chứa dữ liệu không
            // (Tránh trường hợp click nhầm vào thanh tiêu đề cột phía trên cùng gây lỗi)
            if (e.RowIndex >= 0)
            {
                // Lấy thông tin của toàn bộ cái dòng mà chuột vừa click vào
                DataGridViewRow row = dgvDanhSach.Rows[e.RowIndex];

                // Lấy giá trị nằm ở cột "Mã Sách" của dòng đó, ép sang kiểu chuỗi (string) 
                // và đưa nó vào ô TextBox txtMaSach
                txtMaSach.Text = row.Cells["Mã Sách"].Value.ToString();
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadDanhSachSach();
        }

        private void btnLuuPhieu_Click(object sender, EventArgs e)
        {
            // Kiểm tra người dùng đã nhập đủ dữ liệu chưa
            if (string.IsNullOrWhiteSpace(txtMaPM.Text) ||
                string.IsNullOrWhiteSpace(txtMaDG.Text) ||
                string.IsNullOrWhiteSpace(txtMaSach.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ Mã phiếu, Mã độc giả và Mã sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";

            using (SqlConnection sqlCon = new SqlConnection(strCon))
            {
                try
                {
                    sqlCon.Open();

                    // Kiểm tra cuốn sách này có tồn tại và còn số lượng không?
                    string checkSachSql = "SELECT SoLuong FROM tblSach WHERE MaSach = @MaSach";
                    using (SqlCommand checkSachCmd = new SqlCommand(checkSachSql, sqlCon))
                    {
                        checkSachCmd.Parameters.AddWithValue("@MaSach", txtMaSach.Text.Trim());
                        object objSoLuong = checkSachCmd.ExecuteScalar();

                        if (objSoLuong == null)
                        {
                            MessageBox.Show("Mã sách này không tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int soLuong = Convert.ToInt32(objSoLuong);
                        if (soLuong <= 0)
                        {
                            MessageBox.Show("Sách này đã được mượn hết (Số lượng = 0)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Kiểm tra mã độc giả có hợp lệ không (Vì đã tạo khóa ngoại, nếu mã không có sẽ hiển thị lỗi)
                    string checkDGSql = "SELECT COUNT(*) FROM tblDocGia WHERE MaDG = @MaDG";
                    using (SqlCommand checkDGCmd = new SqlCommand(checkDGSql, sqlCon))
                    {
                        checkDGCmd.Parameters.AddWithValue("@MaDG", txtMaDG.Text.Trim());
                        int countDG = (int)checkDGCmd.ExecuteScalar();
                        if (countDG == 0)
                        {
                            MessageBox.Show("Mã độc giả không tồn tại, vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Kiểm tra mã phiếu mượn đã bị trùng chưa
                    string checkPMSql = "SELECT COUNT(*) FROM tblPhieuMuon WHERE MaPM = @MaPM";
                    using (SqlCommand checkPMCmd = new SqlCommand(checkPMSql, sqlCon))
                    {
                        checkPMCmd.Parameters.AddWithValue("@MaPM", txtMaPM.Text.Trim());
                        int countPM = (int)checkPMCmd.ExecuteScalar();
                        if (countPM > 0)
                        {
                            MessageBox.Show("Mã phiếu mượn này đã tồn tại. Vui lòng nhập mã khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // THỰC HIỆN LƯU VÀO DB (Sử dụng Transaction để đảm bảo tính an toàn dữ liệu)
                    using (SqlTransaction transaction = sqlCon.BeginTransaction())
                    {
                        try
                        {
                            // Thêm mới phiếu mượn (Mặc định truyền TrangThai = 0 tức là chưa trả)
                            string insertSql = "INSERT INTO tblPhieuMuon (MaPM, MaDG, MaSach, NgayMuon, HanTra, TrangThai) VALUES (@MaPM, @MaDG, @MaSach, @NgayMuon, @HanTra, 0)";
                            using (SqlCommand insertCmd = new SqlCommand(insertSql, sqlCon, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@MaPM", txtMaPM.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@MaDG", txtMaDG.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@MaSach", txtMaSach.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@NgayMuon", dtpNgayMuon.Value);
                                insertCmd.Parameters.AddWithValue("@HanTra", dtpHanTra.Value);
                                insertCmd.ExecuteNonQuery();
                            }

                            // Cập nhật lại số lượng sách (Trừ đi 1)
                            string updateSql = "UPDATE tblSach SET SoLuong = SoLuong - 1 WHERE MaSach = @MaSach";
                            using (SqlCommand updateCmd = new SqlCommand(updateSql, sqlCon, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@MaSach", txtMaSach.Text.Trim());
                                updateCmd.ExecuteNonQuery();
                            }

                            // Nếu cả 2 lệnh trên đều không có lỗi -> Xác nhận lưu toàn bộ (Commit)
                            transaction.Commit();
                            MessageBox.Show("Lập phiếu mượn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDanhSachSach();

                        }
                        catch (Exception exTrans)
                        {
                            // Nếu có bất kỳ lỗi nào xảy ra trong lúc Insert hoặc Update, hủy bỏ toàn bộ thay đổi (Rollback)
                            transaction.Rollback();
                            MessageBox.Show("Có lỗi khi ghi dữ liệu: " + exTrans.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            txtMaPM.Clear();
            txtMaDG.Clear();
            txtMaSach.Clear();

            // Đặt lại ngày mượn và hạn trả về ngày hôm nay
            dtpNgayMuon.Value = DateTime.Now;
            dtpHanTra.Value = DateTime.Now;

            // Đưa con trỏ chuột nhấp nháy về lại ô Mã Phiếu Mượn
            txtMaPM.Focus();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
