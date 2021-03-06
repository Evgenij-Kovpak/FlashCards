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

    static class SetOfFlipCardKnowledges
    {
        public static List<FlipCardKnowledge> GetAllFlipCardKnowledges()
        {
            List<FlipCardKnowledge> flipCardKnowledges = new List<FlipCardKnowledge>();

            using (var connection = DBConnection.Instance.Connection)
            {
                MySqlCommand cmd = new MySqlCommand(query.all_flip_card_knowledge, connection);

                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    flipCardKnowledges.Add(new FlipCardKnowledge(reader));
                connection.Close();
            }
            return flipCardKnowledges;
        }
        public static bool AddNewFlipCardKnowledge(FlipCardKnowledge fck)
        {
            bool state = false;
            using (var connection = DBConnection.Instance.Connection)
            {
                MySqlCommand cmd = new MySqlCommand($"{query.add_flip_card_knowledge} {fck.ToInsert()}", connection);
                connection.Open();
                var id = cmd.ExecuteNonQuery();
                state = true;
                fck.Id = (byte)cmd.LastInsertedId;
                connection.Close();
            }
            return state;
        }

        public static bool EditFlipCardKnowledge(FlipCardKnowledge fck, ulong? idFkwl)
        {
            bool state = false;
            using (var connection = DBConnection.Instance.Connection)
            {
                string UPDATE_LEVEL = $"UPDATE flipcardknowledge SET knowledge={fck.Knowledge} WHERE ID={idFkwl}";
                MySqlCommand cmd = new MySqlCommand(UPDATE_LEVEL, connection);
                connection.Open();
                var n = cmd.ExecuteNonQuery();
                if (n == 1) state = true;

                connection.Close();
            }
            return state;
        }

        public static bool DeleteFlipCardKnowledge(FlipCardKnowledge fck)
        {
            bool state = false;
            using (var connection = DBConnection.Instance.Connection)
            {
                MySqlCommand cmd = new MySqlCommand(query.delete_flipCard_knowledge + $"{fck.Id}", connection);
                connection.Open();
                var n = cmd.ExecuteNonQuery();
                if (n == 1) state = true;

                connection.Close();
            }
            return state;
        }
    }
}
