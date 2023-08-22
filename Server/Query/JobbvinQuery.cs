using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jobbvin.Shared.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Jobbvin.Server
{
    public class JobbvinQuery
    {
        public AppDb Db { get; }
        private readonly IConfiguration _configuration;

        public JobbvinQuery(AppDb db, IConfiguration configuration)
        {
            Db = db;
            _configuration = configuration;

        }

        public async Task<pic_user> FindOneAsync(string userName, string pass)
        {

            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT * FROM `pic_user` WHERE `user_email` = @userName AND `user_password`= @pass";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userName",
                    DbType = DbType.String,
                    Value = userName,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pass",
                    DbType = DbType.String,
                    Value = pass,
                });
    
                var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new pic_user();
            }
        }

        public async Task<CalendarDisplayData> FindDisplayCalendaAsync(int userId)
        {

            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT pic_user_id,pic_id,DisplayOnHomePage FROM `pic_addpost` WHERE `addpost_scheme_user_id` = @userId AND `DisplayOnHomePage`= 1";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.Int32,
                    Value = userId
                });
               
                var result = await ReadCalendarDataAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new CalendarDisplayData();
            }
        }

        public async Task<List<pic_calendar>> FindMultipleAsync(UserInput userInput)
        {
            try
            {
                var lastDay = DateTime.DaysInMonth(userInput.year, userInput.month);
                string lastdate = "0";
                if (lastDay.ToString().Length == 1)
                    lastdate = "0" + lastDay;
                else
                    lastdate = lastDay.ToString();

                DateTime startDate = Convert.ToDateTime( userInput.year+"-"+userInput.month+"-"+"01");
                DateTime endDate = Convert.ToDateTime(userInput.year + "-" + userInput.month + "-" + lastdate);

                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT addpost_uni_id,addpost_user_id,Cal_Date,Cal_Available,Cal_Options FROM `pic_calendar` WHERE `addpost_uni_id` = @ad_id AND `Cal_Date` between DATE(@startDate) AND DATE(@endDate)";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@ad_id",
                    DbType = DbType.Int32,
                    Value = userInput.adid,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@startDate",
                    DbType = DbType.DateTime,
                    Value = startDate,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@endDate",
                    DbType = DbType.DateTime,
                    Value = endDate,
                });
                var result = await ReadAllCalendaDataAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        public async Task<List<users>> FindMultipleByUserIdAsync(UserInput userInput)
        {
            try
            {
               

                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT user_id,DATE(user_date),on_off FROM `users` WHERE `user_id` = @user_id  AND `on_off`=1";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id",
                    DbType = DbType.String,
                    Value = userInput.userid,
                });
               
                var result = await ReadAllUserDataAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        public async Task<List<pic_websiteDb>> GetAllBusinessServiceData()
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT * FROM `pic_website` WHERE `status` = 1";
               
                var result = await ReadAllBusinessServiceDataAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }


        public async Task<bool> CheckExistsRecord(int userId, DateTime date)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT user_id,user_date FROM `users` WHERE `user_id` = @user_id AND DATE(`user_date`)= DATE(@date)";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id",
                    DbType = DbType.Int32,
                    Value = userId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@date",
                    DbType = DbType.DateTime,
                    Value = date,
                });

                var result = await ReadUserAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return false;
            }
        }


        private async Task<pic_user> ReadAllAsync(DbDataReader reader)
        {
            var pic_User = new pic_user();

            try
            {
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.HasRows)
                        {
                            pic_User.user_id = Convert.ToInt32(reader["user_id"]);
                            pic_User.user_city = Convert.ToString(reader["user_city"]);
                            pic_User.user_email = Convert.ToString(reader["user_email"]);
                            pic_User.user_id_unique = Convert.ToString(reader["user_id_unique"]);
                            pic_User.user_type = Convert.ToString(reader["user_type"]);
                            pic_User.user_mobile = Convert.ToString(reader["user_mobile"]);
                            pic_User.user_refer = Convert.ToString(reader["user_refer"]);
                            pic_User.user_sex = Convert.ToString(reader["user_sex"]);
                            pic_User.user_dob = Convert.ToDateTime(reader["user_dob"]);
                            pic_User.user_username = Convert.ToString(reader["user_username"]);
                            var res = pic_User.user_pic = reader["user_pic"] != null ? (byte[])reader["user_pic"] : new byte[0];
                            pic_User.user_pic_url = string.Format("{0}/{1}/{2}", _configuration.GetSection("Baseurl").Value, "media/profile", Encoding.UTF8.GetString(res, 0, res.Length));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pic_User;
        }

        private async Task<CalendarDisplayData> ReadCalendarDataAsync(DbDataReader reader)
        {
            var calendar = new CalendarDisplayData();

            try
            {
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.HasRows)
                        {
                            calendar.addpost_user_id = Convert.ToInt32(reader["pic_user_id"]);
                            calendar.addpost_uni_id = Convert.ToInt32(reader["pic_id"]);
                            calendar.DisplayOnHomePage = Convert.ToInt32(reader["DisplayOnHomePage"]) == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return calendar;
        }


        private async Task<bool> ReadUserAsync(DbDataReader reader)
        {
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

        private async Task<List<users>> ReadAllUserDataAsync(DbDataReader reader)
        {
            try
            {
                var posts = new List<users>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new users(Db)
                        {
                            user_id = reader.GetInt32(0),
                            user_date = reader.GetDateTime(1),
                            on_Off = reader.GetInt32(2),
                        };
                        posts.Add(post);
                    }
                }
                return posts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<pic_calendar>> ReadAllCalendaDataAsync(DbDataReader reader)
        {
            try
            {
                var calendars = new List<pic_calendar>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var calendar = new pic_calendar();
                        calendar.addpost_uni_id = Convert.ToInt32(reader["addpost_uni_id"]);
                        calendar.Cal_Date = Convert.ToDateTime(reader["Cal_Date"]);
                        calendar.Cal_Available = Convert.ToInt32(reader["Cal_Available"]);
                        calendar.Cal_Options = Convert.ToString(reader["Cal_Options"]);
                        calendar.addpost_user_id = Convert.ToInt32(reader["addpost_user_id"]);

                        calendars.Add(calendar);
                    }
                }
                return calendars;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<pic_websiteDb>> ReadAllBusinessServiceDataAsync(DbDataReader reader)
        {
            try
            {
                var posts = new List<pic_websiteDb>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new pic_websiteDb(Db)
                        {
                            id = reader.GetInt32(0),
                            website_name = reader.GetString(1),
                            website_url = reader.GetString(2),
                            logo = reader.GetString(3),
                            status= reader.GetInt32(4),

                        };
                        posts.Add(post);
                    }
                }
                return posts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class ApiResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public string ValidationName { get; set; }
    }

    public class UserInput
    {
        public int adid { get; set; }
        public int userid { get; set; }
        public int month { get; set; }
        public int year { get; set; }
    }

    public class users
    {
        internal AppDb Db { get; set; }

        public users()
        {
        }

        internal users(AppDb db)
        {
            Db = db;
        }

        public int user_id { get; set; }
        public DateTime user_date { get; set; }
        public int on_Off { get; set; }

        public async Task<bool> InsertAsync()
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `users` (`user_id`, `user_date`, `on_Off`) VALUES (@user_id, @user_date,@on_Off);";
                BindParams(cmd);
                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
               return false;
            }
        }

        public async Task<bool> UpdateAsync()
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `users` SET `on_Off` = @on_Off WHERE `user_id` = @user_id AND DATE(`user_date`)=DATE(@user_date)";
                BindParams(cmd);
                //BindId(cmd);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //private void BindId(MySqlCommand cmd)
        //{
        //    cmd.Parameters.Add(new MySqlParameter
        //    {
        //        ParameterName = "@id",
        //        DbType = DbType.Int32,
        //        Value = Id,
        //    });
        //}

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@user_id",
                DbType = DbType.Int32,
                Value = user_id,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@user_date",
                DbType = DbType.String,
                Value = user_date,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@on_Off",
                DbType = DbType.Int32,
                Value = on_Off,
            });
        }
    }


    public class data
    {
        public data()
        {
            pic_website = new List<pic_website>();
        }

       
        public List<pic_website> pic_website;
    }

    public class pic_websiteDb
    {
        public int id { get; set; }
        public String website_name { get; set; }
        public AppDb Db { get; }
        internal pic_websiteDb(AppDb db)
        {
            Db = db;
        }
      
       public String website_url { get; set; }
        public String logo { get; set; }
        public int status { get; set; }
        
    }

    public class pic_website
    {
        public string id { get; set; }
        public String website_name { get; set; }
        public String website_url { get; set; }
        public String logo { get; set; }
        //public String complete_url { get; set; }
        public int status { get; set; }
        public string url { get; set; }
    }
}
