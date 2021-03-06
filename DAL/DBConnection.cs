using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FlashCards.DAL
{
    class DBConnection
    {
        private MySqlConnectionStringBuilder conStrBuilder = new MySqlConnectionStringBuilder();

        private static DBConnection instance = null;
        public static DBConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConnection();
                }
                return instance;
            }
        }

        private DBConnection()
        {
            conStrBuilder.UserID = Properties.Settings.Default.userID;
            conStrBuilder.Server = Properties.Settings.Default.server;
            conStrBuilder.Database = Properties.Settings.Default.database;
            conStrBuilder.Port = Properties.Settings.Default.port;
            conStrBuilder.Password = Properties.Settings.Default.password;
        }

        public MySqlConnection Connection => new MySqlConnection(conStrBuilder.ToString());
    }
}
