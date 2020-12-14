using AutoMapper;
using BookStore_API.Data;
using BookStore_API.DTOs;

namespace BookStore_API.Mapper
{
    public class Maps : Profile
    {
        public Maps()
        {
            this.CreateMap<Author, AuthorDTO>().ReverseMap();
            this.CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            this.CreateMap<Author, AuthorUpdateDTO>().ReverseMap();
            this.CreateMap<Book, BookDTO>().ReverseMap();
            this.CreateMap<Book, BookCreateDTO>().ReverseMap();
            this.CreateMap<Book, BookUpdateDTO>().ReverseMap();
        }
    }
}