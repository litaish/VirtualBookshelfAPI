using AutoMapper;
using VirtualBookshelfAPI.DTOs;
using VirtualBookshelfAPI.Entities;

namespace VirtualBookshelfAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<BookEditingDTO, Book>()
                .ForMember(dest => dest.Authors, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<BookDTO, Book>();
            CreateMap<Book, BookDTO>()
                    .ForMember(dest => dest.Authors, opts => opts.MapFrom(src => src.Authors.Select(a => new AuthorDTO { Name = a.Name })))
                    .ForMember(dest => dest.Categories, opts => opts.MapFrom(src => src.Categories.Select(c => new CategoryDTO { Name = c.Name })));
            CreateMap<AuthorDTO, Author>();
            CreateMap<CategoryDTO, Category>();
        }
    }
}
