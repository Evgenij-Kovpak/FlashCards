using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FlashCards.DAL.Enities
{
    class User
    {
        public sbyte? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
 
        public User(MySqlDataReader reader)
        {
            Id = sbyte.Parse(reader["ID"].ToString());
            Name = reader["Name"].ToString();
            Surname = reader["Surname"].ToString();
        }

        public User(string name, string surname)
        {
            Id = null;
            Name = name.Trim();
            Surname = surname.Trim();
        }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }

        public string ToInsert()
        {
            return $"('{Name}', '{Surname}')";
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var user = obj as User;
            if (user is null) return false;
            if (Name.ToLower() != user.Name.ToLower()) return false;
            if (Surname.ToLower() != user.Surname.ToLower()) return false;
            return true;
        }
    }
}
