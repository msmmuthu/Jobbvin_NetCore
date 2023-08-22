using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jobbvin.Shared.Models;
using Jobbvin.Server.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Jobbvin.Server
{
    public class UserQuery<T> where T : class
    {
        public AppDb Db { get; }
        private ILogger<T> _logger;

        private readonly IConfiguration _configuration;
        public UserQuery(AppDb db, IConfiguration configuration, ILogger<T> logger)
        {
            Db = db;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<pic_categories>> GetAllActiveMenuItems()
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT * FROM `pic_categories` WHERE `categories_status` =1";


                var result = await ReadAllCategoryDataAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        private async Task<List<pic_categories>> ReadAllCategoryDataAsync(DbDataReader reader)
        {
            try
            {
                var pic_categories = new List<pic_categories>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new pic_categories()
                        {
                            categories_id = Convert.ToInt32(reader["categories_id"]),
                            categories_name = Convert.ToString(reader["categories_name"]),
                            categories_desc = Convert.ToString(reader["categories_desc"]),
                            categories_image = Convert.ToString(reader["categories_image"]),
                            categories_status = Convert.ToInt32(reader["categories_status"]),
                            categories_parent = Convert.ToInt32(reader["categories_parent"]),
                            categories_sub = Convert.ToInt32(reader["categories_sub"]),
                            categories_hidden = Convert.ToInt32(reader["categories_hidden"]),
                            categories_homepage = Convert.ToInt32(reader["categories_homepage"]),
                            categories_price_label = Convert.ToString(reader["categories_price_label"]),
                            categories_desc_label = Convert.ToString(reader["categories_desc_label"]),
                            category_order = Convert.ToInt32(reader["category_order"])
                        };
                        pic_categories.Add(post);
                    }
                }
                return pic_categories;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> InsertTempUserAsync(temp_user tmUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `temp_user` (`user_username`, `user_password`, `user_email`, `user_mobile`, `user_city`, `user_status`,`mobile_val`,`user_id_unique`)
                VALUES (@user_username, @user_password, @user_email, @user_mobile, @user_city, @user_status,@mobile_val,@user_id_unique);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_username",
                    DbType = DbType.String,
                    Value = tmUser.user_username,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_password",
                    DbType = DbType.String,
                    Value = tmUser.user_password,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_email",
                    DbType = DbType.String,
                    Value = tmUser.user_email,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_mobile",
                    DbType = DbType.String,
                    Value = tmUser.user_mobile,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_city",
                    DbType = DbType.String,
                    Value = tmUser.user_city,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_status",
                    DbType = DbType.Int32,
                    Value = tmUser.user_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile_val",
                    DbType = DbType.String,
                    Value = string.IsNullOrEmpty(tmUser.mobile_val) ? "Valid": tmUser.mobile_val,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id_unique",
                    DbType = DbType.String,
                    Value = tmUser.user_id_unique,
                });

                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> InsertTempUserIntoUserAsync(temp_user tmUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `pic_user` (`user_username`, `user_password`, `user_email`, `user_mobile`, `user_city`, `user_status`,`mobile_val`,`user_id_unique`, `user_refer`,`user_type`)
                VALUES (@user_username, @user_password, @user_email, @user_mobile, @user_city, @user_status,@mobile_val,@user_id_unique, @user_refer, @user_type);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_username",
                    DbType = DbType.String,
                    Value = tmUser.user_username,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_password",
                    DbType = DbType.String,
                    Value = tmUser.user_password,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_email",
                    DbType = DbType.String,
                    Value = tmUser.user_email,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_mobile",
                    DbType = DbType.String,
                    Value = tmUser.user_mobile,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_city",
                    DbType = DbType.String,
                    Value = tmUser.user_city,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_status",
                    DbType = DbType.Int32,
                    Value = tmUser.user_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile_val",
                    DbType = DbType.String,
                    Value = string.IsNullOrEmpty(tmUser.mobile_val) ? "Valid" : tmUser.mobile_val,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id_unique",
                    DbType = DbType.String,
                    Value = tmUser.user_id_unique,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_refer",
                    DbType = DbType.String,
                    Value = _configuration.GetSection("RefererId").Value,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_type",
                    DbType = DbType.String,
                    Value = "customer",
                });
                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> InsertPicUserAsync(pic_user tmUser)
        { 
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `pic_user` (`user_username`, `user_password`, `user_email`, `user_mobile`, `user_city`, `user_status`,`mobile_val`,`user_id_unique`, `user_refer`,`user_type`,`user_dob`,`user_sex`,`user_pic`)
                VALUES (@user_username, @user_password, @user_email, @user_mobile, @user_city, @user_status,@mobile_val,@user_id_unique, @user_refer, @user_type,@user_dob,@user_sex, @user_pic)";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_username",
                    DbType = DbType.String,
                    Value = tmUser.user_username,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_sex",
                    DbType = DbType.String,
                    Value = tmUser.user_sex,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_dob",
                    DbType = DbType.String,
                    Value = tmUser.user_dob,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_password",
                    DbType = DbType.String,
                    Value = tmUser.user_password,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_email",
                    DbType = DbType.String,
                    Value = tmUser.user_email,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_mobile",
                    DbType = DbType.String,
                    Value = tmUser.user_mobile,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_city",
                    DbType = DbType.String,
                    Value = tmUser.user_city,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_status",
                    DbType = DbType.Int32,
                    Value = tmUser.user_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile_val",
                    DbType = DbType.String,
                    Value = string.IsNullOrEmpty(tmUser.mobile_val) ? "Valid" : tmUser.mobile_val,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id_unique",
                    DbType = DbType.String,
                    Value = tmUser.user_id_unique,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_refer",
                    DbType = DbType.String,
                    Value = tmUser.user_refer,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_type",
                    DbType = DbType.String,
                    Value = tmUser.user_type,
                });

                byte[] blobOfImageFile = Encoding.ASCII.GetBytes(tmUser.user_pic_url);
                MySqlParameter blob = new MySqlParameter("@user_pic", MySqlDbType.Blob, blobOfImageFile.Length);
                blob.Value = blobOfImageFile;

                cmd.Parameters.Add(blob);
              
                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("Insert new User Query execution : " + ex.Message);

                return false;
            }
        }

        public async Task<bool> UdatePicUserAsync(pic_user tmUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `pic_user` 
                 set `user_username` = @user_username, 
                `user_password` = @user_password, 
                `user_email` = @user_email,
                `user_mobile` = @user_mobile, 
                `user_city` = @user_city,
                `user_status` = @user_status,
                `mobile_val` = @mobile_val,
                `user_refer` = @user_refer,
                `user_type` = @user_type,
                `user_dob` = @user_dob,
                `user_sex` = @user_sex,
                `user_id_unique` = @user_id_unique
                   WHERE  `user_email` = @user_email and `user_mobile` = @user_mobile and `user_id` = @user_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_username",
                    DbType = DbType.String,
                    Value = tmUser.user_username,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_sex",
                    DbType = DbType.String,
                    Value = tmUser.user_sex,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_dob",
                    DbType = DbType.String,
                    Value = tmUser.user_dob,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_refer",
                    DbType = DbType.String,
                    Value = tmUser.user_refer,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_type",
                    DbType = DbType.String,
                    Value = tmUser.user_type,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_password",
                    DbType = DbType.String,
                    Value = tmUser.user_password,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_email",
                    DbType = DbType.String,
                    Value = tmUser.user_email,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_mobile",
                    DbType = DbType.String,
                    Value = tmUser.user_mobile,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_city",
                    DbType = DbType.String,
                    Value = tmUser.user_city,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_status",
                    DbType = DbType.Int32,
                    Value = tmUser.user_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id",
                    DbType = DbType.Int32,
                    Value = tmUser.user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile_val",
                    DbType = DbType.String,
                    Value = tmUser.mobile_val,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id_unique",
                    DbType = DbType.String,
                    Value = tmUser.user_id_unique,
                });

                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> DeleteTempUserIntoUserAsync(temp_user tmUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"DELETE FROM `temp_user`  WHERE  `user_email` = @user_email and `user_mobile` = @user_mobile";
               
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_email",
                    DbType = DbType.String,
                    Value = tmUser.user_email,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_mobile",
                    DbType = DbType.String,
                    Value = tmUser.user_mobile,
                });

             
              
                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UdateTempUserAsync(temp_user tmUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `temp_user` 
                 set `user_username` = @user_username, 
                `user_password` = @user_password, 
                `user_email` = @user_email,
                `user_mobile` = @user_mobile, 
                `user_city` = @user_city,
                `user_status` = @user_status,
                `mobile_val` = @mobile_val,
                `user_id_unique` = @user_id_unique
                   WHERE  `user_email` = @user_email and `user_mobile` = @user_mobile and `user_id` = @user_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_username",
                    DbType = DbType.String,
                    Value = tmUser.user_username,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_password",
                    DbType = DbType.String,
                    Value = tmUser.user_password,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_email",
                    DbType = DbType.String,
                    Value = tmUser.user_email,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_mobile",
                    DbType = DbType.String,
                    Value = tmUser.user_mobile,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_city",
                    DbType = DbType.String,
                    Value = tmUser.user_city,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_status",
                    DbType = DbType.Int32,
                    Value = tmUser.user_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id",
                    DbType = DbType.Int32,
                    Value = tmUser.user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile_val",
                    DbType = DbType.String,
                    Value = tmUser.mobile_val,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@user_id_unique",
                    DbType = DbType.String,
                    Value = tmUser.user_id_unique,
                });

                await cmd.ExecuteNonQueryAsync();
                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckEmailsExists(string emailId)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT user_email,user_mobile FROM `pic_user` WHERE `user_email` = @emailId";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@emailId",
                    DbType = DbType.String,
                    Value = emailId,
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

       public async Task<int> CheckEmailMobilExists(string user_email, string user_mobile, bool requestFromUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                if(requestFromUser)
                cmd.CommandText = @"SELECT user_email,user_mobile,user_id FROM `temp_user` WHERE `user_email` = @emailId and `user_mobile` = @mobile";
                else
                    cmd.CommandText = @"SELECT user_email,user_mobile,user_id FROM `pic_user` WHERE `user_email` = @emailId and `user_mobile` = @mobile";

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@emailId",
                    DbType = DbType.String,
                    Value = user_email,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile",
                    DbType = DbType.String,
                    Value = user_mobile,
                });

                var result = await ReadUserIdAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return -1;
            }
        }

        public async Task<ApiResponse> CheckOtP(temp_user temp_User)
        {
            var response = new ApiResponse();

            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT user_email,user_mobile FROM `temp_user` WHERE `user_email` = @emailId and `user_mobile` = @mobile and `mobile_val` = @mobileVal";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@emailId",
                    DbType = DbType.String,
                    Value = temp_User.user_email,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobile",
                    DbType = DbType.String,
                    Value = temp_User.user_mobile,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobileVal",
                    DbType = DbType.String,
                    Value = temp_User.mobile_val,
                });

                var result = await ReadUserAsync(await cmd.ExecuteReaderAsync());
                if (result)
                {
                    response.Message = "all_success";
                }
                else
                {
                    response.Message = "Invalid otp";
                }
                return response;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return response;
            }
        }


        public async Task<bool> CheckMobileExists(string mobileNumber)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT user_mobile FROM `pic_user` WHERE `user_mobile` = @mobileNumber";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@mobileNumber",
                    DbType = DbType.String,
                    Value = mobileNumber,
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

        private async Task<int> ReadUserIdAsync(DbDataReader reader)
        {
            int user_id = -1;
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    if (reader.HasRows)
                        return  Convert.ToInt32(reader["user_id"]);
                    else
                        return user_id;
                }
                 
            }
            return user_id;
        }

        public async Task<List<pic_user>> GetUserById(int userid, bool addReferFilter)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT * from `pic_user` WHERE `user_id` = @userId";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.String,
                    Value = userid,
                });

                var result = await ReadAllUserDataByRefererAsync(await cmd.ExecuteReaderAsync());
                if (addReferFilter)
                {
                    var refer = "PA00"+ userid;
                    return await GetUserByReferId(refer);
                }
                else
                { return result; }
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new List<pic_user>();
            }
        }

        public async Task<List<pic_user>> GetUserByReferId(string refer)
        {
            try
            {
                using var cmd1 = Db.Connection.CreateCommand();
                cmd1.CommandText = @"SELECT * from `pic_user` WHERE `user_refer` = @refer";
                cmd1.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@refer",
                    DbType = DbType.String,
                    Value = refer,
                });

                var customers = await ReadAllUserDataByRefererAsync(await cmd1.ExecuteReaderAsync());

                return customers;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new List<pic_user>();
            }
        }

        private async Task<List<pic_user>> ReadAllUserDataByRefererAsync(DbDataReader reader)
        {
            try
            {
                var pic_users = new List<pic_user>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        byte[] res;
                        var user = new pic_user()
                        {
                            user_id = Convert.ToInt32(reader["user_id"]),
                            user_city = Convert.ToString(reader["user_city"]),
                            user_email = Convert.ToString(reader["user_email"]),
                            user_id_unique = Convert.ToString(reader["user_id_unique"]),
                            user_type = Convert.ToString(reader["user_type"]),
                            user_mobile = Convert.ToString(reader["user_mobile"]),
                            user_username = Convert.ToString(reader["user_username"]),
                            user_refer = Convert.ToString(reader["user_refer"]),
                            user_status = Convert.ToInt32(reader["user_status"]),
                            user_password = Convert.ToString(reader["user_password"]),
                            user_pic = res = reader["user_pic"] != null ? (byte[])reader["user_pic"] : new byte[0],
                            user_pic_url = Encoding.UTF8.GetString(res, 0, res.Length)

                    };
                        pic_users.Add(user);
                    }
                }
                return pic_users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<pic_scheme_user>> GetPicSchemeUser(int userid)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select * from pic_scheme_user where pic_user_id= @userId ORDER BY  `pic_scheme_user_id` DESC";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.String,
                    Value = userid,
                });

                var result = await ReadAllPicSchemeUser(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new List<pic_scheme_user>();
            }
        }

        public async Task<List<Pic_Ads_Count>> GetSchemeAdsCount(int userid)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select sum(total_ads) as a,sum(pic_scheme_balance_qty) as b from pic_scheme_user where pic_user_id=@userId and payment_status='Approved'";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.String,
                    Value = userid,
                });

                var result = await ReadAllAdsCount(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new List<Pic_Ads_Count>();
            }
        }

        public async Task<List<SchemeListModel>> GetSchemeList()
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select * from pic_scheme where scheme_status=1";
             
                var result = await ReadAllSchemeList(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new List<SchemeListModel>();
            }
        }

        public async Task<pic_scheme> GetPicScheme(int schemeId)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select * from pic_scheme where scheme_id=@schemeid";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@schemeid",
                    DbType = DbType.Int32,
                    Value = schemeId,
                });
                var result = await ReadPicScheme(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return new pic_scheme();
            }
        }

        public async Task<int> GetTopPicSchemeUser()
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT * FROM `pic_scheme_user` ORDER BY `pic_scheme_user`.`pic_scheme_user_id` DESC limit 1";
               
                var result = await ReadAllPicSchemeUser(await cmd.ExecuteReaderAsync());
                return result.FirstOrDefault().pic_scheme_user_id;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return 0;
            }
        }



        public async Task<bool> GetSchemeListSubmitButton(int userid)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select *,SUM(pic_scheme_balance_qty) AS sum_ads from pic_scheme_user where pic_user_id=@userid and pic_scheme_balance_qty!=0";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.String,
                    Value = userid,
                });
                var result = await ReadSumValue(await cmd.ExecuteReaderAsync());
                if (result > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return false;
            }
        }

        private async Task<int> ReadSumValue(DbDataReader reader)
        {
            int count = 0;
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    if (reader.HasRows)
                        return Convert.ToInt32(reader["sum_ads"]);
                    else
                        return count;
                }

            }
            return count;
        }

        public async Task<int> GetTotalScehmeCount(int userid)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select * from pic_scheme_user where pic_user_id=@userId and payment_status='Approved'";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.String,
                    Value = userid,
                });

                CommonQuery<Pic_Ads_Count> commonQuery = new CommonQuery<Pic_Ads_Count>();

                var count = await commonQuery.ReadRowsCount(await cmd.ExecuteReaderAsync());
                return count;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return 0;
            }
        }

        public async Task<bool> InsertPurchaseSchemeAsync(pic_scheme_user tmUser)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `pic_scheme_user` (`pic_scheme_id`, `pic_scheme_name`, `pic_scheme_desc`, `pic_scheme_balance_qty`,`total_ads`,`cost_scheme`,`payment_status`,`payment_method`, 
                                        `pic_user_id`,`scheme_purchased_date`,`scheme_purpose`,`scheme_cash_id`,payment_details,photo_limit,ads_valid)
                VALUES (@pic_scheme_id, @pic_scheme_name, @pic_scheme_desc, @pic_scheme_balance_qty,@total_ads,@cost_scheme,@payment_status,@payment_method, 
                                        @pic_user_id,@scheme_purchased_date,@scheme_purpose,@scheme_cash_id,@payment_details,@photo_limit,@ads_valid);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_scheme_id",
                    DbType = DbType.Int32,
                    Value = tmUser.pic_scheme_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_scheme_name",
                    DbType = DbType.String,
                    Value = tmUser.pic_scheme_name,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_scheme_desc",
                    DbType = DbType.String,
                    Value = tmUser.pic_scheme_desc,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_scheme_balance_qty",
                    DbType = DbType.Int32,
                    Value = tmUser.pic_scheme_balance_qty,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@total_ads",
                    DbType = DbType.Int32,
                    Value = tmUser.total_ads,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@cost_scheme",
                    DbType = DbType.Int32,
                    Value = tmUser.cost_scheme,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@payment_status",
                    DbType = DbType.String,
                    Value = tmUser.payment_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@payment_method",
                    DbType = DbType.String,
                    Value = tmUser.payment_method,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_id",
                    DbType = DbType.String,
                    Value = tmUser.pic_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@scheme_purchased_date",
                    DbType = DbType.DateTime,
                    Value = tmUser.scheme_purchased_date,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@scheme_purpose",
                    DbType = DbType.String,
                    Value = tmUser.scheme_purpose,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@scheme_cash_id",
                    DbType = DbType.String,
                    Value = tmUser.scheme_cash_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@payment_details",
                    DbType = DbType.String,
                    Value = tmUser.payment_details,
                }); 
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@photo_limit",
                    DbType = DbType.Int32,
                    Value = tmUser.photo_limit,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@ads_valid",
                    DbType = DbType.Int32,
                    Value = tmUser.ads_valid,
                });

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        private async Task<List<pic_scheme_user>> ReadAllPicSchemeUser(DbDataReader reader)
        {
            try
            {
                var pic_scheme_users = new List<pic_scheme_user>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        byte[] res;
                        var user = new pic_scheme_user()
                        {
                            scheme_purchased_date = Convert.ToDateTime(reader["scheme_purchased_date"]),
                            pic_scheme_name = Convert.ToString(reader["pic_scheme_name"]),
                            total_ads = Convert.ToInt32(reader["total_ads"]),
                            cost_scheme = Convert.ToInt32(reader["cost_scheme"]),
                            payment_status = Convert.ToString(reader["payment_status"]),
                            payment_method = Convert.ToString(reader["payment_method"]),
                            pic_scheme_user_id = Convert.ToInt32(reader["pic_scheme_user_id"])
                        };
                        pic_scheme_users.Add(user);
                    }
                }
                return pic_scheme_users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<Pic_Ads_Count>> ReadAllAdsCount(DbDataReader reader)
        {
            try
            {
                var Pic_Ads_Counts = new List<Pic_Ads_Count>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new Pic_Ads_Count()
                        {
                            TotalAds = Convert.ToInt32(reader["a"]),
                            TotalUsedAds = Convert.ToInt32(reader["a"])- Convert.ToInt32(reader["b"]),
                            TotalBalanceAds = Convert.ToInt32(reader["b"])
                        };
                        Pic_Ads_Counts.Add(user);
                    }
                }
                return Pic_Ads_Counts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private async Task<List<SchemeListModel>> ReadAllSchemeList(DbDataReader reader)
        {
            try
            {
                var SchemeListModels = new List<SchemeListModel>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var schemes = new SchemeListModel()
                        {
                            SchemeId = Convert.ToInt32(reader["scheme_id"]),
                            SchemeName = Convert.ToString(reader["scheme_name"]),
                            SchemeDescription = Convert.ToString(reader["scheme_desc"]),
                            Price = Convert.ToInt32(reader["scheme_price"])
                        };
                        SchemeListModels.Add(schemes);
                    }
                }
                return SchemeListModels;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<pic_scheme> ReadPicScheme(DbDataReader reader)
        {
            try
            {
                var pic_Scheme = new pic_scheme();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        pic_Scheme.scheme_id = Convert.ToInt32(reader["scheme_id"]);
                        pic_Scheme.scheme_name = Convert.ToString(reader["scheme_name"]);
                        pic_Scheme.scheme_desc = Convert.ToString(reader["scheme_desc"]);
                        pic_Scheme.scheme_ads_qty = Convert.ToInt32(reader["scheme_ads_qty"]);
                        pic_Scheme.scheme_price = Convert.ToInt32(reader["scheme_price"]);
                        pic_Scheme.scheme_date = Convert.ToDateTime(reader["scheme_date"]);
                        pic_Scheme.scheme_photo = Convert.ToInt32(reader["scheme_photo"]);
                        pic_Scheme.scheme_valid = Convert.ToInt32(reader["scheme_valid"]);
                        pic_Scheme.scheme_status = Convert.ToInt32(reader["scheme_status"]);
                    }
                }
                return pic_Scheme;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}