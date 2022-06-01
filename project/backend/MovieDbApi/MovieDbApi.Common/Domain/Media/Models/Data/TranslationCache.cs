﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    [Index(nameof(Language))]
    public class TranslationCache
    {
        [Key]
        public int Id { get; set; }

        public string Language { get; set; }

        [Column(TypeName = "LONGTEXT")]
        public string? Source { get; set; }

        [Column(TypeName = "LONGTEXT")]
        public string? Target { get; set; }
    }
}
