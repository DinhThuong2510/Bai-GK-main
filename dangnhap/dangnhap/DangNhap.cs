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
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }
        Modify modify = new Modify();
        private void btndangnhap_Click(object sender, EventArgs e)
        {
            string tentk = txtTenTaiKhoan.Text;
            string matkhau = txtmatkhau.Text;

            if (tentk.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản");
            }
            else if (matkhau.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu");
            }
            else
            {
                string query = "SELECT * FROM TaiKhoan WHERE TenTaiKhoan = '"
                             + tentk + "' AND MatKhau = '" + matkhau + "'";

                if (modify.TaiKhoans(query).Count != 0)
                {
                    MessageBox.Show(
                        "Đăng nhập thành công",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // CHUYỂN SANG HOME
                    Home home = new Home(tentk);

                    this.Hide();
                    home.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show(
                        "Tên tài khoản hoặc mật khẩu không đúng",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
           "Bạn có thực sự muốn thoát hay không?",
           "Hộp thoại",
           MessageBoxButtons.YesNo,
           MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Close();
        }

        private void lkDangky_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DangKy dangky = new DangKy();
            dangky.ShowDialog();
        }
    }
}
