using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace QLTV_sach_so
{
    internal class Connection
    {
        private static string stringConnection = "Server =localhost; Database=library_management;User ID =root; Password=123456;Port=3306;SslMode=none;";
        public static MySqlConnection GetMySqlConnection()
        {
            return new MySqlConnection(stringConnection);
        }
    }
}
