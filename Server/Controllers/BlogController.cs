using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jobbvin.Server.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class BlogController : ControllerBase
    //{
    //    public BlogController(AppDb db)
    //    {
    //        Db = db;
    //    }

    //    // GET api/blog
    //    [HttpGet]
    //    public async Task<IActionResult> GetLatest()
    //    {
    //        //try
    //        //{
    //            await Db.Connection.OpenAsync();
    //            var query = new BlogPostQuery(Db);
    //            var result = await query.LatestPostsAsync();
    //            return new OkObjectResult(result);
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //    return null;
    //        //}
    //    }

    //    // GET api/blog/5
    //    [HttpGet("{id}")]
    //    public async Task<IActionResult> GetOne(int id)
    //    {
    //        await Db.Connection.OpenAsync();
    //        var query = new BlogPostQuery(Db);
    //        var result = await query.FindOneAsync(id);
    //        if (result is null)
    //            return new NotFoundResult();
    //        return new OkObjectResult(result);
    //    }

    //    // POST api/blog
    //    [HttpPost]
    //    public async Task<IActionResult> Post([FromBody] BlogPost body)
    //    {
    //        try
    //        {
    //            await Db.Connection.OpenAsync();
    //            body.Db = Db;
    //            await body.InsertAsync();
    //            return new OkObjectResult(body);
    //        }
    //        catch (Exception ex)
    //        {
    //            return new OkObjectResult(body);
    //        }
    //    }

    //    // PUT api/blog/5
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> PutOne(int id, [FromBody] BlogPost body)
    //    {
    //        await Db.Connection.OpenAsync();
    //        var query = new BlogPostQuery(Db);
    //        var result = await query.FindOneAsync(id);
    //        if (result is null)
    //            return new NotFoundResult();
    //        result.Title = body.Title;
    //        result.Content = body.Content;
    //        await result.UpdateAsync();
    //        return new OkObjectResult(result);
    //    }

    //    // DELETE api/blog/5
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> DeleteOne(int id)
    //    {
    //        await Db.Connection.OpenAsync();
    //        var query = new BlogPostQuery(Db);
    //        var result = await query.FindOneAsync(id);
    //        if (result is null)
    //            return new NotFoundResult();
    //        await result.DeleteAsync();
    //        return new OkResult();
    //    }

    //    // DELETE api/blog
    //    [HttpDelete]
    //    public async Task<IActionResult> DeleteAll()
    //    {
    //        await Db.Connection.OpenAsync();
    //        var query = new BlogPostQuery(Db);
    //        await query.DeleteAllAsync();
    //        return new OkResult();
    //    }

    //    public AppDb Db { get; }
    //}
}
