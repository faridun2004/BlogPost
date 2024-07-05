using BlogPost.Data;
using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BlogPost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly IMemoryCache _cache;
        private const string PostsCacheKey = "postList";

        public PostController(BlogDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("{id}")]
        public IActionResult GetPostByIdCeche(int id)
        {
            var cacheKey = $"Post_{id}";
            var cachedPost = _cache.Get<Post>(cacheKey);

            if (cachedPost != null)
            {
                return Ok(cachedPost);
            }

            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            _cache.Set(cacheKey, post);

            return Ok(post);
        }
        [HttpGet("GetAllPostsCache")]
        public IActionResult GetPostsCache()
        {
            if (!_cache.TryGetValue(PostsCacheKey, out List<Post>? posts))
            {
                posts = _context.Posts.ToList();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(PostsCacheKey, posts, cacheOptions);
            }

            return Ok(posts);
        }
        [HttpPost("CacheCreateBook")]
        public IActionResult CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
            _cache.Remove(PostsCacheKey); 
            return CreatedAtAction(nameof(GetPosts), new { id = post.Id }, post);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.Include(b => b.User).ToListAsync();
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Post>> GetPost(int id)
        //{
        //    var book = await _context.Posts.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }

        //    return book;
        //}

        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPostByIdCeche), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}