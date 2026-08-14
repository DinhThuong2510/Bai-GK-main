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
using Excel = Microsoft.Office.Interop.Excel;

namespace dangnhap
{
    public partial class ThongKe : Form
    {
        public ThongKe()
        {
            InitializeComponent();
        }

        private void LoadThongKe(string query)
        {
            string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";
            using (SqlConnection sqlCon = new SqlConnection(strCon))
            {
                try
                {
                    sqlCon.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvThongKe.DataSource = dt;
                    dgvThongKe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnDangMuon_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM tblPhieuMuon WHERE TrangThai = 0";
            LoadThongKe(query);
        }

        private void btnQuaHan_Click(object sender, EventArgs e)
        {
            // Lọc những phiếu chưa trả (TrangThai=0) và Hạn Trả nhỏ hơn ngày hôm nay
            string query = "SELECT * FROM tblPhieuMuon WHERE TrangThai = 0 AND HanTra < GETDATE()";
            LoadThongKe(query);
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            string strCon = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLySach;Integrated Security=True;Encrypt=False";
            using (SqlConnection sqlCon = new SqlConnection(strCon))
            {
                try
                {
                    sqlCon.Open();
                    string query = "SELECT * FROM tblPhieuMuon WHERE NgayMuon >= @TuNgay AND NgayMuon <= @DenNgay";

                    // Khởi tạo SqlCommand để gán tham số
                    using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                    {
                        // Gán giá trị ngày bắt đầu (lấy từ 00:00:00) và ngày kết thúc (lấy đến cuối ngày 23:59:59)
                        cmd.Parameters.AddWithValue("@TuNgay", dtpTuNgay.Value.Date);
                        cmd.Parameters.AddWithValue("@DenNgay", dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1));

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvThongKe.DataSource = dt;
                        dgvThongKe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvThongKe.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Application.Workbooks.Add(Type.Missing);

                // Xuất tiêu đề cột
                for (int i = 1; i < dgvThongKe.Columns.Count + 1; i++)
                {
                    excelApp.Cells[1, i] = dgvThongKe.Columns[i - 1].HeaderText;
                }

                // Xuất dữ liệu từng dòng
                for (int i = 0; i < dgvThongKe.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvThongKe.Columns.Count; j++)
                    {
                        excelApp.Cells[i + 2, j + 1] = dgvThongKe.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                excelApp.Columns.AutoFit();
                excelApp.Visible = true;
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để xuất Excel!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM tblPhieuMuon";
            LoadThongKe(query);
        }
    }
}
