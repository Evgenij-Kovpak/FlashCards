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

    static class SetOfLanguages
    {
        public static List<Language> GetAllLanguages()
        {
            List<Language> langs = new List<Language>();

            using (var connection = DBConnection.Instance.Connection)
            {
                MySqlCommand cmd = new MySqlCommand(query.all_langs, connection);

                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    langs.Add(new Language(reader));
                connection.Close();
            }
            return langs;
        }
    }
}
