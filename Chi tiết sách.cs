//using MySql.Data.MySqlClient;
//using Org.BouncyCastle.Utilities;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace QLTV_sach_so
//{
//    public partial class Chi_tiết_sách : Form
//    {
//        private string maSach;
//        public Chi_tiết_sách(string maSach)
//        {
//            InitializeComponent();
//            this.maSach = maSach;
//            LoadThongTinSach();
//        }
//        private void LoadThongTinSach()
//        {
//            using (MySqlConnection conn = Connection.GetMySqlConnection())
//            {
//                try
//                {
//                    conn.Open();
//                    string query = "SELECT * FROM sach WHERE ma_sach = @maSach";
//                    MySqlCommand cmd = new MySqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@maSach", maSach);
//                    MySqlDataReader reader = cmd.ExecuteReader();

//                    if (reader.Read())
//                    {
//                        lbltensach.Text = reader["ten_sach"].ToString();
//                        lbltacgia.Text = reader["tac_gia"].ToString();
//                        lblnamsanxuat.Text = reader["nam_xuat_ban"].ToString();
//                        lbltinhtrang.Text = reader["tinh_trang_sach"].ToString();

//                        // Load hình ảnh sách
//                        //string imagePath = Path.Combine(Application.StartupPath, "Resources", maSach + "image_2024032114122991.jpg");
//                        //if (File.Exists(imagePath))
//                        //{
//                        //    picSach.Image = Image.FromFile(imagePath);
//                        //}
//                        //else
//                        //{
//                        //    picSach.Image = Image.FromFile("Resources/default.jpg"); // Ảnh mặc định
//                        //}
//                    }
//                    reader.Close();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("Lỗi tải thông tin sách: " + ex.Message);
//                }
//            }
//        }
//    }
//}
