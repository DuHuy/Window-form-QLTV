using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV_sach_so
{
    public partial class TrangChuAdmin : Form
    {
        public TrangChuAdmin()
        {
            InitializeComponent();
        }

        private void linkLabel_qldanhmuc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            QL_danh_muc qL_Danh_Muc = new QL_danh_muc();
            this.Hide();
            qL_Danh_Muc.ShowDialog();
            this.Show();
        }

        private void linkLabel_qlmuontra_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            QL_Mượn_Trả qL_Mượn_Trả = new QL_Mượn_Trả();
            this.Hide();
            qL_Mượn_Trả.ShowDialog();
            this.Show();
        }

        private void linkLabel_qldattruoc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Dat_truoc dat_Truoc = new Dat_truoc();
            this.Hide();
            dat_Truoc.ShowDialog();
            this.Show();
        }

        private void linkLabel_qlbaocao_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BaoCaoTK baoCaoTK = new BaoCaoTK();
            this.Hide();
            baoCaoTK.ShowDialog();
            this.Show();
        }

        private void linkLabel_qltaikhoan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            QLThongTinTaiKhoan qLThongTinTaiKhoan = new QLThongTinTaiKhoan();
            this.Hide();
            qLThongTinTaiKhoan.ShowDialog();
            this.Show();
        }

        private void linkLabel_dangxuatdm_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Restart();
        }



        //private void Pic_Click(object sender, EventArgs e)
        //{
        //    PictureBox pic = (PictureBox)sender;
        //    string maSach = pic.Tag.ToString(); // Lấy ID sách từ Tag của PictureBox
        //    Chitietsach formChiTiet = new Chitietsach(maSach);
        //    formChiTiet.ShowDialog();
        //}

        private void TrangChuAdmin_Load(object sender, EventArgs e)
        {

        }

        //}
        //private void LoadSachLenGiaoDien()
        //{
        //    using (MySqlConnection conn = Connection.GetMySqlConnection())
        //    {
        //        try
        //        {
        //            conn.Open();
        //            string query = "SELECT ma_sach, ten_sach, hinh_anh FROM sach";
        //            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
        //            DataTable dt = new DataTable();
        //            adapter.Fill(dt);

        //            flowLayoutPanel1.Controls.Clear(); // Xóa dữ liệu cũ

        //            foreach (DataRow row in dt.Rows)
        //            {
        //                PictureBox pic = new PictureBox();
        //                pic.Width = 100;
        //                pic.Height = 150;
        //                pic.SizeMode = PictureBoxSizeMode.StretchImage;

        //                // Chuyển dữ liệu ảnh từ byte[] thành Image
        //                byte[] imgData = (byte[])row["hinh_anh"];
        //                using (MemoryStream ms = new MemoryStream(imgData))
        //                {
        //                    pic.Image = Image.FromStream(ms);
        //                }

        //                pic.Tag = row["ma_sach"].ToString(); // Lưu ID sách
        //                pic.Cursor = Cursors.Hand; // Thêm hiệu ứng con trỏ
        //                pic.Click += Pic_Click; // Gán sự kiện Click

        //                flowLayoutPanel1.Controls.Add(pic);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi khi tải danh sách sách: " + ex.Message);
        //        }
        //    }
        //}
        //private void Pic_Click(object sender, EventArgs e)
        //{
        //    PictureBox pic = (PictureBox)sender;
        //    string maSach = "lichsudang"; // Lấy ID sách
        //    Chi_tiết_sách formChiTiet = new Chi_tiết_sách(maSach);
        //    formChiTiet.ShowDialog();


        //}

        //private void TrangChuAdmin_Load(object sender, EventArgs e)
        //{
        //    LoadSachLenGiaoDien();
        //}
    }
}
