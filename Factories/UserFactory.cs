using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using quotingDojo.Models;
using Microsoft.Extensions.Options;

namespace quotingDojo.Factory
{
    public class UserFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> mysqlConfig;
        
        public UserFactory(IOptions<MySqlOptions> conf) {
                mysqlConfig = conf;
        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(mysqlConfig.Value.ConnectionString);
            }
        }
        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection) {
                string query =  "INSERT INTO users (firstname, lastname, email, password) VALUES (@FirstName, @LastName, @Email, @Password)";
                dbConnection.Open();
                dbConnection.Execute(query, item);
            }
        }
        public IEnumerable<User> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users");
            }
        }
        public User FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users WHERE id = @Id", new { Id = id }).FirstOrDefault();
            }
        }
        public User FindByEmail(string email)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users WHERE email = @email", new { email = email}).FirstOrDefault();
            }
        }
    }
}