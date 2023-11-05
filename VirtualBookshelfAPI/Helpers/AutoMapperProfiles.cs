using AutoMapper;
using VirtualBookshelfAPI.DTOs;
using VirtualBookshelfAPI.Entities;

namespace VirtualBookshelfAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<BookDTO, Book>().ReverseMap();
            CreateMap<AuthorDTO, Author>();
            CreateMap<CategoryDTO, Category>();
        }
    }
}
