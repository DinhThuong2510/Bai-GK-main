using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dangnhap
{
    public partial class Home : Form
    {
        private string tenTaiKhoan;

        public Home(string tentk)
        {
            InitializeComponent();

            tenTaiKhoan = tentk;

            // Hiển thị tên tài khoản
            lblXinChao.Text = "Xin chào: " + tenTaiKhoan;

            // Hiển thị ngày hiện tại
            lblNgay.Text = "Ngày: " + DateTime.Now.ToString("dd/MM/yyyy");
        }
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void btnQLSach_Click(object sender, EventArgs e)
        {
            QuanLySach frm = new QuanLySach();
            frm.ShowDialog();
        }

        private void btnQLDGia_Click(object sender, EventArgs e)
        {
            QuanLyDocGia frm = new QuanLyDocGia();
            frm.ShowDialog();
        }

        private void btnMSach_Click(object sender, EventArgs e)
        {
            MuonSach frm = new MuonSach();
            frm.ShowDialog();
        }

        private void btnTSach_Click(object sender, EventArgs e)
        {
            TraSach frm = new TraSach();
            frm = new TraSach();
            frm.ShowDialog();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            ThongKe frm = new ThongKe();
            frm.ShowDialog();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Bạn có chắc chắn muốn đăng xuất không?",
        "Xác nhận đăng xuất",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    );

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
