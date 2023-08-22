using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MySqlConnector;

namespace Jobbvin.Server
{
    public class AppDb : IDisposable
    {
        public MySqlConnection Connection { get; }

        public AppDb(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public void Dispose() => Connection.Dispose();
    }
}
