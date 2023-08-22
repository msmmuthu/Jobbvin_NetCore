using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jobbvin.Shared.Models;
using Jobbvin.Server.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Jobbvin.Server
{
    public class PoductsQuery<T> where T : class
    {
        public AppDb Db { get; }
        private readonly IConfiguration _configuration; 
        private ILogger<T> _logger;

        public PoductsQuery(AppDb db, IConfiguration configuration, ILogger<T> logger)
        {
            Db = db;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<ProductListViewModel>> GetProductListByFilter(PoductFilterModel filterModel)
        {
            var prodLists = new List<ProductListViewModel>();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT po.pic_ads_id, po.pic_user_id,po.pic_user_fullname, po.pic_title, po.pic_discription, po.pic_category,po.pic_price,po.addpost_status,po.pic_postdate, loc.loc_name, img.pic_file_url FROM `pic_addpost` AS po 
                                    left JOIN pic_addpost_locations as loc on po.pic_ads_id = loc.addpost_uni_id
                                    left join pic_addpost_profiles as img on  po.pic_ads_id = img.pic_ads_id
                                    where po.pic_category=" + filterModel.cat_id + " order by po.pic_id desc ";
                var result = await ReadAllCategoryDataAsync(await cmd.ExecuteReaderAsync());
                var prodList = result.GroupBy(p => p.pic_ads_id);

                if (filterModel.offset > 0) //Pagination
                {
                    prodList = prodList.Skip((filterModel.offset - 1) * 10).Take(10).ToList();
                }

                foreach (var group in prodList)
                {
                    var prodListViewModel = new ProductListViewModel();
                    prodListViewModel = group.FirstOrDefault();
                    prodListViewModel.pic_add_images.AddRange(group.Select(g => g.pic_file_url = _configuration.GetSection("Baseurl").Value + "/" + g.pic_file_url).ToList());

                    var loc_name = group.Select(g => g.pic_locations).ToList();
                    prodListViewModel.pic_Addpost_Locations.AddRange(from loc in loc_name
                                                                     select new pic_addpost_locations { loc_name = loc });
                    prodListViewModel.pic_star_rating = await GetStarRatingByByAdId(prodListViewModel.pic_ads_id);
                    //prodListViewModel.pic_Addpost_Locations = await GetAdLocationByByAdId(prodListViewModel.pic_ads_id);

                    prodListViewModel.DisplayInFieldValues = await GetDisplayInListFields(prodListViewModel.pic_ads_id);

                    prodLists.Add(prodListViewModel);
                }
                return prodLists;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        public async Task<List<ProductListViewModel>> GetProductListApplyByFilter(PostAdViewModel filterModel)
        {
            var prodLists = new List<ProductListViewModel>();
            try
            {
                PoductFilterModel catFilter = new PoductFilterModel();
                catFilter.cat_id = filterModel.pic_Addpost.pic_category;
                var categoryResult = await GetProductListByFilter(catFilter);
                return categoryResult;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        async Task<StarRating> GetStarRatingByByAdId(int adId)
        {
            var starRatings = new StarRating();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT itemId, AVG(ratingNumber) as Average,COUNT(itemId) AS TotalRatings
                            FROM `item_rating` WHERE  itemId=" + adId + " GROUP by itemId";
                starRatings = await ReadAllStaratingsDataAsync(await cmd.ExecuteReaderAsync());

                return starRatings;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        async Task<List<pic_addpost_locations>> GetAdLocationByByAdId(int adId)
        {
            var locations = new List<pic_addpost_locations>();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT id, addpost_uni_id,loc_name,pic_add_lon,pic_add_lat
                            FROM `pic_addpost_locations` WHERE  addpost_uni_id=" + adId;
                locations = await ReadAllLocationDataAsync(await cmd.ExecuteReaderAsync());

                return locations;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        async Task<List<DisplayInFieldValue>> GetDisplayInListFields(int adId)
        {
            var displayInFieldValues = new List<DisplayInFieldValue>();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                cmd.CommandText = @"select DISTINCT(papf.addpost_fields_title),papf.addpost_fields_value,papf.addpost_fields_type from pic_addpost_field papf join pic_categories_fields pcf on papf.addpost_fields_categories_id = pcf.fields_categories_id and papf.field_id=pcf.fields_id where (papf.addpost_fields_type!='Chain' and papf.addpost_uni_id = @adid) or (papf.addpost_fields_type='Numeric' and papf.pots_field_DV_id=0 and papf.addpost_uni_id = @adid) group by papf.addpost_fields_title ORDER BY pcf.field_priority,pcf.fields_id ASC";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@adId",
                    DbType = DbType.Int32,
                    Value = adId,
                });
                displayInFieldValues = await ReadDisplayinFields(await cmd.ExecuteReaderAsync());

                return displayInFieldValues;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        private async Task<List<ProductListViewModel>> ReadAllCategoryDataAsync(DbDataReader reader)
        {
            try
            {
                var pod_List = new List<ProductListViewModel>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new ProductListViewModel()
                        {
                            pic_ads_id = Convert.ToInt32(reader["pic_ads_id"]),
                            pic_user_id = Convert.ToInt32(reader["pic_user_id"]),
                            pic_title = Convert.ToString(reader["pic_title"]),
                            pic_discription = Convert.ToString(reader["pic_discription"]),
                            pic_price = Convert.ToInt32(reader["pic_price"]),
                            addpost_status = Convert.ToInt32(reader["addpost_status"]) == 1 ? "Active" : "Pending",
                            pic_user_fullname = Convert.ToString(reader["pic_user_fullname"]),
                            pic_postdate= Convert.ToDateTime(reader["pic_postdate"]),
                            pic_categoryId = Convert.ToInt32(reader["pic_category"]),
                            pic_file_url = !string.IsNullOrEmpty(Convert.ToString(reader["pic_file_url"])) ? Convert.ToString(reader["pic_file_url"]) : "img/avatar.jpg",
                            pic_locations = Convert.ToString(reader["loc_name"]),
                        };
                        pod_List.Add(post);
                    }
                }
                return pod_List;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<int> ReadStarRatingByUser(DbDataReader reader)
        {
            try
            {
                int starRating = 0;
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        starRating = Convert.ToInt32(reader["ratingNumber"]);
                    }
                }
                return starRating;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private async Task<List<ProductListViewModel>> ReadAllHistoryDataAsync(DbDataReader reader)
        {
            try
            {
                var pod_List = new List<ProductListViewModel>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new ProductListViewModel()
                        {
                            pic_ads_id = Convert.ToInt32(reader["pic_ads_id"]),
                            pic_user_id = Convert.ToInt32(reader["pic_user_id"]),
                            pic_title = Convert.ToString(reader["pic_title"]),
                            pic_discription = Convert.ToString(reader["pic_discription"]),
                            pic_price = Convert.ToInt32(reader["pic_price"]),
                            addpost_status = Convert.ToInt32(reader["addpost_status"]) == 1 ? "Active" : "Pending",
                            pic_user_fullname = Convert.ToString(reader["pic_user_fullname"]),
                            pic_category = Convert.ToString(reader["categories_name"]),
                            pic_postdate = Convert.ToDateTime(reader["pic_postdate"]),
                            EnableCalender = Convert.ToInt32(reader["EnableCalender"]),
                            DisplayOnHomePage = Convert.ToInt32(reader["DisplayOnHomePage"]),

                        };
                        pod_List.Add(post);
                    }
                }
                return pod_List;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<StarRating> ReadAllStaratingsDataAsync(DbDataReader reader)
        {
            try
            {
                var starRating = new StarRating();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        starRating.Average = Convert.ToDouble(reader["Average"]);
                        starRating.TotalRatings = Convert.ToInt32(reader["TotalRatings"]);
                    }
                }
                return starRating;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<pic_addpost_locations>> ReadAllLocationDataAsync(DbDataReader reader)
        {
            try
            {
                var locations = new List<pic_addpost_locations>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new pic_addpost_locations()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            loc_name = Convert.ToString(reader["loc_name"]),
                            addpost_uni_id = Convert.ToInt32(reader["addpost_uni_id"]),
                            pic_add_lon = Convert.ToDouble(reader["pic_add_lon"]),
                            pic_add_lat = Convert.ToDouble(reader["pic_add_lat"])
                        };
                        locations.Add(post);
                    }
                }
                return locations;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<DisplayInFieldValue>> ReadDisplayinFields (DbDataReader reader)
        {
           
            try
            {
                var pod_Fields = new List<DisplayInFieldValue>();
                DataTable dataTable = new DataTable();

                dataTable.Load(reader);

                foreach (DataRow row in dataTable.Rows)
                {
                    var pod_Field = new DisplayInFieldValue();
                    if (!string.IsNullOrEmpty(Convert.ToString(row["addpost_fields_value"])))
                    {
                        var addpost_fields_value = Convert.ToString(row["addpost_fields_value"]);
                        var field = new ProductDetailsFieds();
                        field.FieldTitle = Convert.ToString(row["addpost_fields_title"]);
                        field.FieldValue = addpost_fields_value;
                        if (Convert.ToString(row["addpost_fields_type"]) == "DropDown")
                        {
                            pod_Field = await ReadDropdownFieldValueForDisplayInField(field);
                        }
                        else
                        {
                            pod_Field = await ReadNonDropdownFieldValueForDisplayInField(field);
                        }
                        if(pod_Field!= null)
                        pod_Fields.Add(pod_Field);
                    }
                }

                return pod_Fields;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<int> GetStarRatingByUser(int itemId, int userId)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT itemId,userId,ratingNumber FROM `item_rating` WHERE `userId` = @userId AND `itemId` = @itemId";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.Int32,
                    Value = userId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@itemId",
                    DbType = DbType.Int32,
                    Value = itemId,
                });
                var result = await ReadStarRatingByUser(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return 0;
            }
        }

        public async Task<bool> CheckLikeExistsRecord(string likesProductId, string customerId, string likeAdsUserId)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT likes_product_id, likes_cus_id, likes_ads_user_id FROM `pic_likes` WHERE `likes_product_id` = @likesProductId AND `likes_cus_id` = @customerId AND `likes_ads_user_id`=@likes_ads_user_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likesProductId",
                    DbType = DbType.String,
                    Value = likesProductId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@customerId",
                    DbType = DbType.String,
                    Value = customerId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_ads_user_id",
                    DbType = DbType.String,
                    Value = likeAdsUserId,
                });
                CommonQuery<pic_likes> commonQuery = new CommonQuery<pic_likes>();
                var result = await commonQuery.ReadUserAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return false;
            }
        }

        public async Task<bool> CheckAdPostFieldExistsRecord(int addpost_uni_id, int field_id, int addpost_fields_categories_id)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT addpost_uni_id, field_id, addpost_fields_categories_id FROM `pic_addpost_field` WHERE 
                    `addpost_uni_id` = @addpost_uni_id AND `field_id` = @field_id AND `addpost_fields_categories_id`=@addpost_fields_categories_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.Int32,
                    Value = addpost_uni_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@field_id",
                    DbType = DbType.Int32,
                    Value = field_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_categories_id",
                    DbType = DbType.Int32,
                    Value = addpost_fields_categories_id,
                });
                CommonQuery<pic_likes> commonQuery = new CommonQuery<pic_likes>();
                var result = await commonQuery.ReadUserAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return false;
            }
        }

        public async Task<bool> InsertStarRatingAsync(StarRatingModel starRatingModel)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `item_rating` (`itemId`, `userId`, `ratingNumber`, `created`) VALUES (@itemId, @userId,@ratingNumber, @created);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@itemId",
                    DbType = DbType.Int32,
                    Value = starRatingModel.itemId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.Int32,
                    Value = starRatingModel.userId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@ratingNumber",
                    DbType = DbType.Int32,
                    Value = starRatingModel.ratingNumber,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@created",
                    DbType = DbType.DateTime,
                    Value = DateTime.Now,
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

        public async Task<bool> UpdateStarRatingAsync(StarRatingModel starRatingModel)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `item_rating` SET `ratingNumber` = @ratingNumber, `modified` = @modified  WHERE `userId` = @userId AND `itemId` = @itemId";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@itemId",
                    DbType = DbType.Int32,
                    Value = starRatingModel.itemId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@userId",
                    DbType = DbType.Int32,
                    Value = starRatingModel.userId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@ratingNumber",
                    DbType = DbType.Int32,
                    Value = starRatingModel.ratingNumber,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@modified",
                    DbType = DbType.DateTime,
                    Value = DateTime.Now,
                });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> InsertLikeAsync(pic_likes picLikes)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `pic_likes` (`likes_product_id`, `likes_cus_id`, `likes_cus_ip`, `likes_cus_name`,  
                                                            `likes_cus_mobile`, `likes_cus_email`, `likes_ads_user_id`, `contact_no`) 
                                    VALUES (@likes_product_id, @likes_cus_id,@likes_cus_ip, @likes_cus_name,
                                            @likes_cus_mobile, @likes_cus_email,@likes_ads_user_id,@contact_no)";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_product_id",
                    DbType = DbType.String,
                    Value = picLikes.likes_product_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_id",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_ip",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_ip,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_name",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_name,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_mobile",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_mobile,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_email",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_email,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_ads_user_id",
                    DbType = DbType.String,
                    Value = picLikes.likes_ads_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@contact_no",
                    DbType = DbType.String,
                    Value = picLikes.contact_no,
                });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateLikeAsync(pic_likes picLikes)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `pic_likes` SET 
                `likes_product_id` = @likes_product_id,
                `likes_cus_id` = @likes_cus_id,
                `likes_cus_ip` = @likes_cus_ip,
                `likes_cus_name` = @likes_cus_name,
                `likes_cus_mobile` = @likes_cus_mobile,
                `likes_cus_email` = @likes_cus_email,
                `likes_ads_user_id` = @likes_ads_user_id,
                `contact_no` = @contact_no  WHERE `likes_product_id` = @likes_product_id AND `likes_cus_id` = @likes_cus_id AND `likes_ads_user_id`=@likes_ads_user_id ";

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_product_id",
                    DbType = DbType.String,
                    Value = picLikes.likes_product_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_id",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_ip",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_ip,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_name",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_name,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_mobile",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_mobile,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_cus_email",
                    DbType = DbType.String,
                    Value = picLikes.likes_cus_email,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@likes_ads_user_id",
                    DbType = DbType.String,
                    Value = picLikes.likes_ads_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@contact_no",
                    DbType = DbType.String,
                    Value = picLikes.contact_no,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@like_id",
                    DbType = DbType.Int32,
                    Value = picLikes.like_id,
                });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        ///ProductDetails
        public async Task<ProductListViewModel> GetProductDetailsById(string adId)
        {
            var prodLists = new List<ProductListViewModel>();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT po.pic_ads_id, po.pic_user_fullname,po.pic_user_id, po.pic_title, po.pic_discription, po.pic_price,po.addpost_status,po.pic_postdate,po.pic_category, loc.loc_name, img.pic_file_url FROM `pic_addpost` AS po 
                                    left JOIN pic_addpost_locations as loc on po.pic_ads_id = loc.addpost_uni_id
                                    left join pic_addpost_profiles as img on  po.pic_ads_id = img.pic_ads_id
                                    where po.pic_ads_id=" + adId;
                var result = await ReadAllCategoryDataAsync(await cmd.ExecuteReaderAsync());
                var prodList = result.GroupBy(p => p.pic_ads_id);
                foreach (var group in prodList)
                {
                    var prodListViewModel = new ProductListViewModel();
                    prodListViewModel = group.FirstOrDefault();
                    prodListViewModel.pic_add_images.AddRange(group.Select(g => g.pic_file_url = _configuration.GetSection("Baseurl").Value + "/"  + g.pic_file_url).ToList());
                    prodListViewModel.pic_star_rating = await GetStarRatingByByAdId(prodListViewModel.pic_ads_id);
                    prodListViewModel.pic_Addpost_Locations = await GetAdLocationByByAdId(prodListViewModel.pic_ads_id);
                    prodLists.Add(prodListViewModel);
                }
                return prodLists.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        public async Task<List<ProductListViewModel>> GetPostHistoryByUserId(string userId)
        {
            var prodLists = new List<ProductListViewModel>();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT po.pic_ads_id,po.EnableCalender,po.DisplayOnHomePage, po.pic_postdate, po.pic_user_fullname,po.pic_user_id, po.pic_title, po.pic_discription, po.pic_price,
                                    po.addpost_status,sc.categories_name from pic_addpost as po
                                    join pic_categories as sc
                                    on po.pic_category = sc.categories_id
                                    where po.pic_user_id=" + userId + " order by pic_id desc ";
                var result = await ReadAllHistoryDataAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        public async Task<List<ProductDetailsFieds>> GetProductFieldDataById(string adId)
        {
            var prodLists = new List<ProductDetailsFieds>();
            try
            {
                using (var cmd = Db.Connection.CreateCommand())
                {
                    cmd.CommandText = @"select DISTINCT(papf.addpost_fields_title),papf.addpost_fields_value,papf.addpost_fields_type from pic_addpost_field papf 
                                    join pic_categories_fields pcf on papf.addpost_fields_categories_id = pcf.fields_categories_id and papf.field_id=pcf.fields_id 
                                    where (papf.addpost_fields_type!='Chain' and papf.addpost_uni_id = @adId) 
                                    or (papf.addpost_fields_type='Numeric' and papf.pots_field_DV_id=0 and papf.addpost_uni_id = @adId) 
                                    group by papf.addpost_fields_title ORDER BY pcf.field_priority,pcf.fields_id ASC";
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@adId",
                        DbType = DbType.String,
                        Value = adId,
                    });
                    prodLists = await ReadProductDetailsFiedsDataAsync(await cmd.ExecuteReaderAsync());
                    return prodLists;
                }
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }

        public async Task<List<ProductDetailsFiles>> GetProductFilesById(string adId)
        {
            var prodFileLists = new List<ProductDetailsFiles>();
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"select * from pic_addpost_files where pic_ads_id = @adId";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@adId",
                    DbType = DbType.String,
                    Value = adId,
                });
                var reader = await cmd.ExecuteReaderAsync();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var prodFile = new ProductDetailsFiles();
                        prodFile.PicFileUrl = _configuration.GetSection("Baseurl").Value + "/"  + Convert.ToString(reader["pic_file_url"]);
                        var fileEXtension = Path.GetExtension(Convert.ToString(reader["pic_file_url"]));
                        if(fileEXtension == ".pdf")
                        {
                            prodFile.PicFileIconPath = _configuration.GetSection("Baseurl").Value  + "/img/pdfIcon.png";
                        }
                        else if(fileEXtension.StartsWith(".doc"))
                        {
                            prodFile.PicFileIconPath = _configuration.GetSection("Baseurl").Value + "/" + "/img/wordIcon.png";
                        }
                        prodFileLists.Add(prodFile);
                    }
                }
                return prodFileLists;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
        }


        public async Task<int> InsertAdPostAsync(pic_addpost picAdPost)
        {

            try
            {
                picAdPost.pic_ads_id = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `pic_addpost` 
                    (`pic_ads_id`, `pic_title`, `pic_category`, `pic_price`,`pic_discription`,`pic_postdate`,`pic_is_freeads`,
                    `addpost_scheme_user_id`,`pic_user_email`,`pic_user_id`,`pic_post_city`,`pic_user_mobile`,`pic_user_fullname`,`pic_user_type`,
                    `pic_user_address`,`pic_add_town`,`addpost_status`,`pic_request`,`pic_sms`,
                    `pic_privacy`,`pic_refer_id`,`pic_special`,`pic_validity`,`pic_qty`,`EnableCalender`,`DisplayOnHomePage`) 
                    VALUES 
                    (@pic_ads_id, @pic_title, @pic_category, @pic_price,@pic_discription,@pic_postdate,@pic_is_freeads,
                    @addpost_scheme_user_id,@pic_user_email,@pic_user_id,@pic_post_city,@pic_user_mobile,@pic_user_fullname,@pic_user_type,
                    @pic_user_address,@pic_add_town,@addpost_status,@pic_request,@pic_sms,
                    @pic_privacy,@pic_refer_id,@pic_special,@pic_validity,@pic_qty,@EnableCalender,@DisplayOnHomePage);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_ads_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_ads_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_title",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_title,
                }); 
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_category",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_category,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_price",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_price,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_discription",
                    DbType = DbType.String,
                    Value = picAdPost.pic_discription,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_postdate",
                    DbType = DbType.DateTime,
                    Value = picAdPost.pic_postdate,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_is_freeads",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_is_freeads,
                }); 
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_scheme_user_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.addpost_scheme_user_id,
                }); 
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_email",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_email,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_post_city",
                    DbType = DbType.String,
                    Value = picAdPost.pic_post_city,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_mobile",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_mobile,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_fullname",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_fullname,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_type",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_user_type,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_address",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_address,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_add_town",
                    DbType = DbType.String,
                    Value = picAdPost.pic_add_town,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_status",
                    DbType = DbType.Int32,
                    Value = picAdPost.addpost_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_request",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_request,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_sms",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_sms,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_privacy",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_privacy,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_refer_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_refer_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_special",
                    DbType = DbType.String,
                    Value = picAdPost.pic_special,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_validity",
                    DbType = DbType.DateTime,
                    Value = picAdPost.pic_validity,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_qty",
                    DbType = DbType.DateTime,
                    Value = picAdPost.pic_qty,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@EnableCalender",
                    DbType = DbType.Boolean,
                    Value = picAdPost.EnableCalender,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@DisplayOnHomePage",
                    DbType = DbType.DateTime,
                    Value = picAdPost.DisplayOnHomePage,
                });
                await cmd.ExecuteNonQueryAsync();
                return picAdPost.pic_ads_id;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertAdPostAsync Query execution : " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> InsertOrUpdateAdPostFieldsAsync(List<pic_addpost_field> pic_addpost_fields)
        {
            _logger.LogInformation("InsertOrUpdateAdPostFieldsAsync Starts : Count" + pic_addpost_fields != null ? pic_addpost_fields.Count.ToString() : "0");

            try
            {
                foreach (var adPostFields in pic_addpost_fields)
                {
                    var exists = await CheckAdPostFieldExistsRecord(adPostFields.addpost_uni_id, adPostFields.field_id, adPostFields.addpost_fields_categories_id);
                    if (exists)
                    {
                        await UpdateAdPostFieldsAsync(adPostFields);
                    }
                    else
                    {
                        await InsertAdPostFieldsAsync(adPostFields);
                    }
                }
                _logger.LogInformation("InsertOrUpdateAdPostFieldsAsync ends ");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertOrUpdateAdPostFieldsAsync Query execution : " + ex.Message);
                return false;
            }
        }


        public async Task<bool> InsertAdPostFieldsAsync(pic_addpost_field adPostFields)
        {
            _logger.LogInformation("InsertAdPostFieldsAsync Starts ");

            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"INSERT INTO `pic_addpost_field` 
                    (`addpost_fields_categories_id`, `addpost_uni_id`, `addpost_fields_title`, `addpost_fields_type`,`addpost_fields_value`,
                     `field_id`,`pots_field_DV_id`, `addpost_fields_lan`,`addpost_fields_lon`) 
                    VALUES 
                    (@addpost_fields_categories_id, @addpost_uni_id, @addpost_fields_title, @addpost_fields_type,@addpost_fields_value,
                        @field_id,@pots_field_DV_id,@addpost_fields_lan,@addpost_fields_lon)";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_categories_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.addpost_fields_categories_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.addpost_uni_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_title",
                    DbType = DbType.String,
                    Value = adPostFields.addpost_fields_title,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_type",
                    DbType = DbType.String,
                    Value = adPostFields.addpost_fields_type,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_value",
                    DbType = DbType.String,
                    Value = adPostFields.addpost_fields_value,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@field_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.field_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pots_field_DV_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.pots_field_DV_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_lan",
                    DbType = DbType.Double,
                    Value = adPostFields.addpost_fields_lan,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_lon",
                    DbType = DbType.Double,
                    Value = adPostFields.addpost_fields_lon,
                });
                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("InsertAdPostFieldsAsync Success");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertAdPostFieldsAsync Query execution : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAdPostFieldsAsync(pic_addpost_field adPostFields)
        {
            _logger.LogInformation("UpdateAdPostFieldsAsync Starts ");

            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"UPDATE `pic_addpost_field` 
                    SET `addpost_fields_categories_id`=@addpost_fields_categories_id,
                        `addpost_uni_id`=@addpost_uni_id, 
                        `addpost_fields_title` = @addpost_fields_title,
                        `addpost_fields_type` = @addpost_fields_type,
                        `addpost_fields_value`=@addpost_fields_value,
                        `field_id` = @field_id,
                        `pots_field_DV_id` =@pots_field_DV_id, 
                        `addpost_fields_lan`=@addpost_fields_lan
                         WHERE `addpost_fields_categories_id`=@addpost_fields_categories_id AND
                        `addpost_uni_id`=@addpost_uni_id AND  `field_id` = @field_id";

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_categories_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.addpost_fields_categories_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.addpost_uni_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_title",
                    DbType = DbType.String,
                    Value = adPostFields.addpost_fields_title,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_type",
                    DbType = DbType.String,
                    Value = adPostFields.addpost_fields_type,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_value",
                    DbType = DbType.String,
                    Value = adPostFields.addpost_fields_value,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@field_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.field_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pots_field_DV_id",
                    DbType = DbType.Int32,
                    Value = adPostFields.pots_field_DV_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_lan",
                    DbType = DbType.Double,
                    Value = adPostFields.addpost_fields_lan,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_fields_lon",
                    DbType = DbType.Double,
                    Value = adPostFields.addpost_fields_lon,
                });
                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("UpdateAdPostFieldsAsync Success");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateAdPostFieldsAsync Query execution : " + ex.Message);
                return false;
            }
        }


        public async Task<int> UpdateAdPostAsync(pic_addpost picAdPost)
        {

            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `pic_addpost` 
                    SET `pic_title` = @pic_title,
                        `pic_category`=@pic_category,
                        `pic_price`=@pic_price,
                        `pic_discription`=@pic_discription,
                        `pic_postdate` = @pic_postdate,
                        `pic_is_freeads`=@pic_is_freeads,
                        `addpost_scheme_user_id`=@addpost_scheme_user_id,
                        `pic_user_email` =@pic_user_email,
                        `pic_user_id` =@pic_user_id,
                        `pic_post_city`=@pic_post_city,
                        `pic_user_mobile`=@pic_user_mobile,
                        `pic_user_fullname`=@pic_user_fullname,
                        `pic_user_type`=@pic_user_type,
                        `pic_user_address`=@pic_user_address,
                        `pic_add_town`=@pic_add_town,
                        `addpost_status`=@addpost_status,
                        `pic_request`=@pic_request,
                        `pic_sms`=@pic_sms,
                        `pic_privacy`=@pic_privacy,
                        `pic_refer_id`=@pic_refer_id,
                        `pic_special`=@pic_special,
                        `pic_validity`=@pic_validity,
                        `EnableCalender`=@EnableCalender,
                        `DisplayOnHomePage`=@DisplayOnHomePage,
                        `pic_qty`=@pic_qty WHERE  `pic_ads_id`=@pic_ads_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_ads_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_ads_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_title",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_title,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_category",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_category,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_price",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_price,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_discription",
                    DbType = DbType.String,
                    Value = picAdPost.pic_discription,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_postdate",
                    DbType = DbType.DateTime,
                    Value = picAdPost.pic_postdate,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_is_freeads",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_is_freeads,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_scheme_user_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.addpost_scheme_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_email",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_email,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_post_city",
                    DbType = DbType.String,
                    Value = picAdPost.pic_post_city,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_mobile",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_mobile,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_fullname",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_fullname,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_type",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_user_type,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_user_address",
                    DbType = DbType.String,
                    Value = picAdPost.pic_user_address,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_add_town",
                    DbType = DbType.String,
                    Value = picAdPost.pic_add_town,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_status",
                    DbType = DbType.Int32,
                    Value = picAdPost.addpost_status,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_request",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_request,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_sms",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_sms,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_privacy",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_privacy,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_refer_id",
                    DbType = DbType.Int32,
                    Value = picAdPost.pic_refer_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_special",
                    DbType = DbType.String,
                    Value = picAdPost.pic_special,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_validity",
                    DbType = DbType.DateTime,
                    Value = picAdPost.pic_validity,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_qty",
                    DbType = DbType.DateTime,
                    Value = picAdPost.pic_qty,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@EnableCalender",
                    DbType = DbType.Boolean,
                    Value = picAdPost.EnableCalender,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@DisplayOnHomePage",
                    DbType = DbType.DateTime,
                    Value = picAdPost.DisplayOnHomePage,
                });
                await cmd.ExecuteNonQueryAsync();
                return picAdPost.pic_ads_id;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertAdPostAsync Query execution : " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> InsertAdPostLocationAsync(List<pic_addpost_locations> picAdPostLocations, int adPostId)
        {
            _logger.LogInformation("InsertAdPostLocationAsync Starts : Count" + picAdPostLocations!=null? picAdPostLocations.Count.ToString() :"0");

            await DeleteAdPostLocationAsync(adPostId);
           
            foreach (var location in picAdPostLocations)
            {
                try
                {
                    using var cmd = Db.Connection.CreateCommand();

                    cmd.CommandText = @"INSERT INTO `pic_addpost_locations` 
                    (`addpost_uni_id`, `loc_name`, `pic_add_lon`, `pic_add_lat`) 
                    VALUES 
                    (@addpost_uni_id, @loc_name, @pic_add_lon, @pic_add_lat);";
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@addpost_uni_id",
                        DbType = DbType.Int32,
                        Value = location.addpost_uni_id,
                    });
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@loc_name",
                        DbType = DbType.String,
                        Value = location.loc_name,
                    });
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@pic_add_lon",
                        DbType = DbType.Double,
                        Value = location.pic_add_lon,
                    });
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@pic_add_lat",
                        DbType = DbType.Double,
                        Value = location.pic_add_lat,
                    });
                    await cmd.ExecuteNonQueryAsync();
                    _logger.LogInformation("InsertAdPostLocationAsync Success after insert");

                    return true;
                    //Id = (int)cmd.LastInsertedId;
                }
                catch (Exception ex)
                {
                    _logger.LogError("InsertAdPostLocationAsync Query execution : " + ex.Message);
                    return false;
                }
            }
            _logger.LogInformation("InsertAdPostLocationAsync Success");

            return true;
        }

        public async Task<bool> DeleteAdPostLocationAsync(int addpost_uni_id)
        {
            _logger.LogInformation("DeleteAdPostLocationAsync Starts");

            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"DELETE from `pic_addpost_locations` 
                   WHERE addpost_uni_id=@addpost_uni_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.Int32,
                    Value = addpost_uni_id,
                });

                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("DeleteAdPostLocationAsync Success");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteAdPostLocationAsync Query execution : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> InsertAdPostDocFilesAsync(int adId, string fileName)
        {
            _logger.LogInformation("InsertAdPostDocFilesAsync Starts ");
            await DeleteAdPostDocFilesAsync(adId);
            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"INSERT INTO `pic_addpost_files` 
                    (`pic_ads_id`, `pic_file_url`, `pic_file_added_on`) 
                    VALUES 
                    (@pic_ads_id, @pic_file_url, @pic_file_added_on);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_ads_id",
                    DbType = DbType.Int32,
                    Value = adId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_file_url",
                    DbType = DbType.String,
                    Value = "media/files/" + fileName,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_file_added_on",
                    DbType = DbType.DateTime,
                    Value = DateTime.Now,
                });

                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("InsertAdPostDocFilesAsync Success");

                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertAdPostDocFilesAsync Query execution : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAdPostDocFilesAsync(int pic_ads_id)
        {
            _logger.LogInformation("DeleteAdPostDocFilesAsync Starts");

            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"DELETE from `pic_addpost_files` 
                   WHERE pic_ads_id=@pic_ads_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_ads_id",
                    DbType = DbType.Int32,
                    Value = pic_ads_id,
                });

                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("DeleteAdPostDocFilesAsync Success");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteAdPostDocFilesAsync Query execution : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> InsertAdPostPofileImageAsync(int adId, string fileName)
        {
            _logger.LogInformation("InsertAdPostPofileAsync Starts ");
            await DeleteAdPostPofileImageAsync(adId);
            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"INSERT INTO `pic_addpost_profiles` 
                    (`pic_ads_id`, `pic_file_url`, `pic_file_added_on`) 
                    VALUES 
                    (@pic_ads_id, @pic_file_url, @pic_file_added_on);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_ads_id",
                    DbType = DbType.Int32,
                    Value = adId,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_file_url",
                    DbType = DbType.String,
                    Value = "media/files/" + fileName,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_file_added_on",
                    DbType = DbType.DateTime,
                    Value = DateTime.Now,
                });

                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("InsertAdPostPofileAsync Success");

                return true;
                //Id = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertAdPostPofileAsync Query execution : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAdPostPofileImageAsync(int pic_ads_id)
        {
            _logger.LogInformation("DeleteAdPostPofileImageAsync Starts");

            try
            {
                using var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = @"DELETE from `pic_addpost_profiles` 
                   WHERE pic_ads_id=@pic_ads_id";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@pic_ads_id",
                    DbType = DbType.Int32,
                    Value = pic_ads_id,
                });

                await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("DeleteAdPostPofileImageAsync Success");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteAdPostPofileImageAsync Query execution : " + ex.Message);
                return false;
            }
        }

        private async Task<List<ProductDetailsFieds>> ReadProductDetailsFiedsDataAsync(DbDataReader reader)
        {
            try
            {
                var pod_Fields = new List<ProductDetailsFieds>();
                DataTable dataTable = new DataTable();

                dataTable.Load(reader);

                foreach (DataRow row in dataTable.Rows)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(row["addpost_fields_value"])))
                    {
                        var addpost_fields_value = Convert.ToString(row["addpost_fields_value"]);
                        var field = new ProductDetailsFieds();
                        field.FieldTitle = Convert.ToString(row["addpost_fields_title"]);
                        if (Convert.ToString(row["addpost_fields_type"]) == "DropDown")
                        {
                            field.FieldValue = await ReadDropdownFieldValue(addpost_fields_value);
                        }
                        else
                        {
                            field.FieldValue = Convert.ToString(row["addpost_fields_value"]);
                        }
                        pod_Fields.Add(field);
                    }
                }

                return pod_Fields;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<string> ReadDropdownFieldValue(string addpost_fields_value)
        {
            string fieldValue = "";
            try
            {
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                using (var cmdDDField = Db.Connection.CreateCommand())
                {
                    cmdDDField.CommandText = "select fields_title,field_value from pic_categories_fields where fields_id=@addpost_fields_value";

                    cmdDDField.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@addpost_fields_value",
                        DbType = DbType.String,
                        Value = addpost_fields_value,
                    });
                    using (var reader = await cmdDDField.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            fieldValue = Convert.ToString(reader["field_value"]);
                            return fieldValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return fieldValue;
        }

        private async Task<DisplayInFieldValue> ReadDropdownFieldValueForDisplayInField(ProductDetailsFieds productDetailsFieds)
        {
            var displayInFieldValue = new DisplayInFieldValue();
            try
            {
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                using (var cmdDDField = Db.Connection.CreateCommand())
                {
                    cmdDDField.CommandText = "select fields_title,field_value from pic_categories_fields where fields_id=@addpost_fields_value and displayinlist=1";

                    cmdDDField.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@addpost_fields_value",
                        DbType = DbType.String,
                        Value = productDetailsFieds.FieldValue,
                    });
                    using (var reader = await cmdDDField.ExecuteReaderAsync())
                    {
                        
                        while (await reader.ReadAsync())
                        {
                            displayInFieldValue.FieldTitle = productDetailsFieds.FieldTitle;
                            displayInFieldValue.FieldValue = Convert.ToString(reader["field_value"]);
                            return displayInFieldValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        private async Task<DisplayInFieldValue> ReadNonDropdownFieldValueForDisplayInField(ProductDetailsFieds productDetailsFieds)
        {
            var displayInFieldValue = new DisplayInFieldValue();
            try
            {
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                using (var cmdDDField = Db.Connection.CreateCommand())
                {
                    cmdDDField.CommandText = "select fields_title from pic_categories_fields where fields_title=@addpost_fields_title and displayinlist=1";

                    cmdDDField.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@addpost_fields_title",
                        DbType = DbType.String,
                        Value = productDetailsFieds.FieldTitle,
                    });
                    using (var reader = await cmdDDField.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            displayInFieldValue.FieldTitle = productDetailsFieds.FieldTitle;
                            displayInFieldValue.FieldValue = productDetailsFieds.FieldValue;
                            return displayInFieldValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public async Task<List<ProductCategoryFields>> GetPostFieldCategoriesDataByCategoryId(int categoryId)
        {
            var cateFields = new List<ProductCategoryFields>();
            try
            {
                using (var cmd = Db.Connection.CreateCommand())
                {
                    cmd.CommandText = @"select * from pic_categories_fields where fields_categories_id =@categoryId order by field_priority,fields_id ASC";
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@categoryId",
                        DbType = DbType.String,
                        Value = categoryId,
                    });
                    var category_Fields = await ReadPicCategoiesFieldDataAsync(await cmd.ExecuteReaderAsync());
                    foreach (var field in category_Fields)
                    {
                        if (field.fields_type.ToLower() == "dropdown" && field.field_DV_id > 0)
                        {
                            var exists = cateFields.Where(c => c.FieldId == field.fields_id);
                            if (exists.Count() < 1)
                            {
                                var dvIdexists = cateFields.Where(c => c.FieldDVId == field.field_DV_id);
                                if (dvIdexists.Count() > 0)
                                {
                                    dvIdexists.FirstOrDefault().FieldListValues.Add(new ProductCategoryFields
                                    {
                                        FieldId = field.fields_id,
                                        FieldDVId = field.field_DV_id,
                                        FieldCategoryId = field.fields_categories_id,
                                        DisplayInFilter=true,
                                        FieldValue = field.field_value
                                    });
                                }
                                else
                                {
                                    cateFields.Add(new ProductCategoryFields
                                    {
                                        FieldId = field.fields_id,
                                        FieldDVId = field.field_DV_id,
                                        FieldCategoryId = field.fields_categories_id,
                                        FieldTitle = field.fields_title,
                                        DisplayInFilter=true,
                                        FieldType = field.fields_type,
                                        FieldListValues = new List<ProductCategoryFields>
                                        { new ProductCategoryFields
                                            {
                                                FieldId = field.fields_id,
                                                FieldDVId = field.field_DV_id,
                                                FieldCategoryId = field.fields_categories_id,
                                               
                                                FieldValue = field.field_value
                                            }
                                    }
                                    });
                                }
                            }
                        }
                        else if (field.fields_type.ToLower() == "text" || field.fields_type.ToLower() == "textbox")
                        {
                            cateFields.Add(new ProductCategoryFields
                            {
                                FieldId = field.fields_id,
                                FieldDVId = field.field_DV_id,
                                FieldCategoryId = field.fields_categories_id,
                                FieldTitle = field.fields_title,
                                DisplayInFilter=true,
                                FieldType = field.fields_type,
                                FieldValue = field.field_value
                            });
                        }
                        else if (field.fields_type.ToLower() == "numeric")
                        {
                            cateFields.Add(new ProductCategoryFields
                            {
                                FieldId = field.fields_id,
                                DisplayInFilter=true,
                                FieldDVId = field.field_DV_id,
                                FieldCategoryId = field.fields_categories_id,
                                FieldTitle = field.fields_title,
                                FieldType = field.fields_type,
                                FieldValue = field.field_value
                            });
                        }
                        else if (field.fields_type.ToLower() == "chain")
                        {

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return null;
            }
            return cateFields;
        }

        public async Task<int> GetBalanceScheme(int userId)
        {
         
            int balanceScheme = 0;
            try
            {
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                using (var cmdDDField = Db.Connection.CreateCommand())
                {
                    cmdDDField.CommandText = "select SUM(pic_scheme_balance_qty) AS sum_ads from pic_scheme_user where payment_status = 'Approved' and pic_user_id = @userId and pic_scheme_balance_qty!= 0";

                    cmdDDField.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@userId",
                        DbType = DbType.Int32,
                        Value = userId,
                    });
                    using (var reader = await cmdDDField.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            balanceScheme = Convert.ToInt32(reader[0]);
                            return balanceScheme;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return balanceScheme;
            }
            return balanceScheme;

        }

        private async Task<List<pic_categories_fields>> ReadPicCategoiesFieldDataAsync(DbDataReader reader)
        {
            try
            {
                var pod_List = new List<pic_categories_fields>();
                using (reader)
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new pic_categories_fields()
                        {
                            displayinlist = Convert.ToInt32(reader["displayinlist"]),
                            fields_categories_id = Convert.ToInt32(reader["fields_categories_id"]),
                            fields_id = Convert.ToInt32(reader["fields_id"]),
                            fields_title = Convert.ToString(reader["fields_title"]),
                            fields_type = Convert.ToString(reader["fields_type"]),
                            field_chain_value = Convert.ToString(reader["field_chain_value"]),
                            field_DV_id = Convert.ToInt32(reader["field_DV_id"]),
                            field_priority = Convert.ToInt32(reader["field_priority"]),
                            field_quickedit = Convert.ToInt32(reader["field_quickedit"]),
                            field_sample = Convert.ToString(reader["field_sample"]),
                            field_value = Convert.ToString(reader["field_value"]),
                            multi = Convert.ToInt32(reader["multi"])
                        };
                        pod_List.Add(post);
                    }
                }
                return pod_List;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> CheckExistsCalenderRecord(int addpost_uni_id, int addpost_user_id, DateTime Cal_Date)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"SELECT addpost_uni_id,addpost_user_id,Cal_Date FROM `pic_calendar` WHERE `addpost_user_id` = @addpost_user_id AND `addpost_uni_id` = @addpost_uni_id  AND DATE(`Cal_Date`)= DATE(@Cal_Date)";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_user_id",
                    DbType = DbType.Int32,
                    Value = addpost_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Date",
                    DbType = DbType.DateTime,
                    Value = Cal_Date,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.DateTime,
                    Value = addpost_uni_id,
                });
                CommonQuery<pic_calendar> commonQuery = new CommonQuery<pic_calendar>();
                var result = await commonQuery.ReadUserAsync(await cmd.ExecuteReaderAsync());
                return result;
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                return false;
            }
        }

        public async Task<bool> InsertCalendarDataAsync(pic_calendar calendarModel)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `pic_calendar` (`addpost_uni_id`, `addpost_user_id`, `Cal_Date`, `Cal_Available`, `Cal_Options`) 
                                                        VALUES (@addpost_uni_id, @addpost_user_id,@Cal_Date, @Cal_Available,@Cal_Options);";
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.Int32,
                    Value = calendarModel.addpost_uni_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_user_id",
                    DbType = DbType.Int32,
                    Value = calendarModel.addpost_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Date",
                    DbType = DbType.String,
                    Value = calendarModel.Cal_Date,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Available",
                    DbType = DbType.DateTime,
                    Value = calendarModel.Cal_Available,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Options",
                    DbType = DbType.String,
                    Value = calendarModel.Cal_Options,
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

        public async Task<bool> UpdateCalendarDataAsync(pic_calendar calendarModel)
        {
            try
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"UPDATE `pic_calendar` 
                    SET `addpost_uni_id`=@addpost_uni_id,
                    `addpost_user_id` = @addpost_user_id,
                    `Cal_Date`=@Cal_Date,
                    `Cal_Available` = @Cal_Available, 
                    `Cal_Options`= @Cal_Options
                     WHERE `addpost_user_id` = @addpost_user_id AND `addpost_uni_id` = @addpost_uni_id  AND DATE(`Cal_Date`)= DATE(@Cal_Date)";

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_uni_id",
                    DbType = DbType.Int32,
                    Value = calendarModel.addpost_uni_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@addpost_user_id",
                    DbType = DbType.Int32,
                    Value = calendarModel.addpost_user_id,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Date",
                    DbType = DbType.String,
                    Value = calendarModel.Cal_Date,
                });

                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Available",
                    DbType = DbType.DateTime,
                    Value = calendarModel.Cal_Available,
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@Cal_Options",
                    DbType = DbType.String,
                    Value = calendarModel.Cal_Options,
                });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}