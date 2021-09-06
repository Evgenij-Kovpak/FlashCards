using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FlashCards.DAL.Enities
{
    class Language
    {
        public sbyte Id { get; set; }
        public string LangName { get; set; }
 
        public Language(MySqlDataReader reader)
        {
            Id = sbyte.Parse(reader["ID"].ToString());
            LangName = reader["Name"].ToString();
        }

        public override string ToString()
        {
            return $"{LangName}";
        }
    }
}
