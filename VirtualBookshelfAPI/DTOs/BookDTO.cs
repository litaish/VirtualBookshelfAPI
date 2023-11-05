﻿using System.ComponentModel.DataAnnotations;
using VirtualBookshelfAPI.Entities;

namespace VirtualBookshelfAPI.DTOs
{
    public class BookDTO
    {
        public string Id { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public string ImgSrc { get; set; }
        public int Rating { get; set; }
        public string Notes { get; set; }
        public bool Read { get; set; }
    }
}