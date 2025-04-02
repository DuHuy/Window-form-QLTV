using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace QLTV_sach_so
{
    internal class Modify
    {
        public Modify()
        {
        }
        MySqlCommand mySqlCommand; // Dùng MySqlCommand thay vì SqlCommand
        MySqlDataReader dataReader; // Dùng MySqlDataReader thay vì SqlDataReader

        public List<TaiKhoan> Taikhoans(string query)
        {
            List<TaiKhoan> taiKhoans = new List<TaiKhoan>();

            using (MySqlConnection mySqlConnection = Connection.GetMySqlConnection()) // Dùng MySqlConnection
            {
                mySqlConnection.Open();
                mySqlCommand = new MySqlCommand(query, mySqlConnection);
                dataReader = mySqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    taiKhoans.Add(new TaiKhoan(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2)));
                }

                mySqlConnection.Close();
            }

            return taiKhoans;
        }
    }
}

