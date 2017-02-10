using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using quotingDojo.Models;
using Microsoft.Extensions.Options;
using System;


namespace quotingDojo.Factory
{
    public class QuoteFactory : IFactory<Quote>
    {

        private readonly IOptions<MySqlOptions> mysqlConfig;
        public QuoteFactory(IOptions<MySqlOptions> conf) {
            mysqlConfig = conf;
        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(mysqlConfig.Value.ConnectionString);
            }
        }
        public void Add(Quote item, int id)
        {
            using (IDbConnection dbConnection = Connection) {
                string query =  string.Format("INSERT INTO quotes (text, created_at, user_id) VALUES ('{0}', '{1}', '{2}')", item.Text, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), id);
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
        public void Delete(int toDelete)
        {
            using (IDbConnection dbConnection = Connection) {
                string query = string.Format("DELETE FROM quotes WHERE id = {0}", toDelete);
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
        public IEnumerable<Quote> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var query = "SELECT * FROM quotes JOIN users ON user_id = users.id ORDER BY created_at DESC";
                return dbConnection.Query<Quote, User, Quote>(query, (Quote, user) => { Quote.user = user; return Quote; });
            }
        }
        public Quote FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Quote>("SELECT * FROM quotes WHERE id = @Id", new { Id = id }).FirstOrDefault();
            }
        }
    }
}