using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualBookshelfAPI.DTOs;
using VirtualBookshelfAPI.Entities;
using VirtualBookshelfAPI.Extensions;
using VirtualBookshelfAPI.Filters;

namespace VirtualBookshelfAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ILogger<BooksController> logger, ApplicationDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDTO>>> Get()
        {
            try
            {
                var books = await context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .ToListAsync();

                return mapper.Map<List<BookDTO>>(books);
            }
            catch (Exception ex)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving books data from the database");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            } 
        }

        [HttpGet("{id:int}")] // Expect a value at the end of an endpoint - api/books/3
        public async Task<ActionResult<BookDTO>> Get(int id)
        {
            try
            {
                var book = await context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .FirstOrDefaultAsync(book => book.Id == id);

                if (book == null) return NotFound();

                return mapper.Map<BookDTO>(book);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving book of Id {id} from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BookCreationDTO bookCreationDTO)
        {
            // Prevent duplicate values for Authors and Categories table when a new book is added
            var authors = mapper.Map<List<Author>>(bookCreationDTO.Authors);

            foreach (var author in authors)
            {
                context.Authors.AddIfNotExists<Author>(author, a => a.Name == author.Name);
            }

            var categories = mapper.Map<List<Category>>(bookCreationDTO.Categories);

            foreach (var category in categories)
            {
                context.Categories.AddIfNotExists<Category>(category, c => c.Name == category.Name);
            }

            await context.SaveChangesAsync();

            // Todo: map through automapper, for authors and categories seperate queries for the mapped object
            var book = new Book
            {
                ISBN10 = bookCreationDTO.ISBN10,
                ISBN13 = bookCreationDTO.ISBN13,
                Title = bookCreationDTO.Title,
                PageCount = bookCreationDTO.PageCount,
                ImgSrc = bookCreationDTO.ImgSrc,
                Rating = bookCreationDTO.Rating,
                Notes = bookCreationDTO.Notes,
                Read = bookCreationDTO.Read,
                Authors = context.Authors
                    // Select all authors which are also in the authors list (from POST body)
                    .Where(a => authors
                        .Select(a => (a.Name ?? string.Empty).ToLower())
                        .Contains((a.Name ?? string.Empty).ToLower()))
                    .ToList(),
                Categories = context.Categories
                    .Where(c => categories
                        .Select(c => (c.Name ?? string.Empty).ToLower())
                        .Contains((c.Name ?? string.Empty).ToLower()))
                    .ToList(),
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] BookCreationDTO book)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}
