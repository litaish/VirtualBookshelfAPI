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
                var books = await context.Books.ToListAsync();

                return mapper.Map<List<BookDTO>>(books);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving books data from the database");
            } 
        }

        [HttpGet("{id:int}")] // Expect a value at the end of an endpoint - api/books/3
        public async Task<ActionResult<BookDTO>> Get(int id)
        {
            try
            {
                var book = await context.Books
                    .Where(book => book.Id == id)
                    .FirstOrDefaultAsync();

                if (book == null) return NotFound();

                return mapper.Map<BookDTO>(book);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BookCreationDTO bookCreationDTO)
        {
            // Prevent duplicate values for Authors and Categories table when a new book is added
            var authors = bookCreationDTO.Authors;

            foreach (var author in authors)
            {
                var authorTransformed = mapper.Map<Author>(author);
                context.Authors.AddIfNotExists<Author>(authorTransformed, a => a.Name == author.Name);
            }

            var categories = bookCreationDTO.Categories;

            foreach (var category in categories)
            {
                var categoryTransformed = mapper.Map<Category>(category);
                context.Categories.AddIfNotExists<Category>(categoryTransformed, c => c.Name == category.Name);
            }

            await context.SaveChangesAsync();

            var authorNames = authors.Select(a => a.Name.ToLower().Trim()).ToList();
            var categoriesNames = categories.Select(c => c.Name.ToLower().Trim()).ToList();

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
                Authors = context.Authors.Where(a => authorNames.Contains(a.Name.ToLower().Trim())).ToList(),
                Categories = context.Categories.Where(c => categoriesNames.Contains(c.Name.ToLower().Trim())).ToList(),
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Book book)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}
