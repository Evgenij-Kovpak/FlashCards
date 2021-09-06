using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FlashCards.DAL.Commands
{
    using Enities;

    using query = Properties.Resources;

    static class SetOfUsers
    {
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (var connection = DBConnection.Instance.Connection)
            {
                MySqlCommand cmd = new MySqlCommand(query.all_users, connection);

                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    users.Add(new User(reader));
                connection.Close();
            }
            return users;
        }

        public static bool AddNewUser(User u)
        {
            bool state = false;
            using(var connection = DBConnection.Instance.Connection)
            {
                MySqlCommand cmd = new MySqlCommand(query.add_user, connection);

                cmd.Parameters.AddWithValue("@uName", u.Name);
                cmd.Parameters.AddWithValue("@uSurname", u.Surname);

                try
                {
                    connection.Open();
                    var id = cmd.ExecuteNonQuery();
                    state = true;
                    u.Id = (sbyte)cmd.LastInsertedId;
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            return state;
        }
    }
}
