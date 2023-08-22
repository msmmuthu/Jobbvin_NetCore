using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Query
{
    public class CommonQuery<T> where T : class
    {
        public async Task<bool> ReadUserAsync(DbDataReader reader)
        {
            var posts = new List<T>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    if (reader.HasRows)
                        return true;
                }
            }
            return false;
        }

        public async Task<int> ReadRowsCount(DbDataReader reader)
        {
            int count = 0;
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    if (reader.HasRows)
                        count++;
                }
            }
            return count;
        }

        public async Task<int> ReadSumValue(DbDataReader reader)
        {
            int count = 0;
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    if (reader.HasRows)
                        return Convert.ToInt32(reader[0]);
                    else
                        return count;
                }

            }
            return count;
        }
    }
}
