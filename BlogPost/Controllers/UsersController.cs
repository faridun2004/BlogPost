using BlogPost.Data;
using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public UsersController(BlogDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAuthors()
        {
            return await _context.Users.Include(a => a.Posts).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetAuthor(int id)
        {
            var author = await _context.Users.Include(a => a.Posts).FirstOrDefaultAsync(a => a.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostAuthor(User author)
        {
            _context.Users.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, User author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Users.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Users.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
