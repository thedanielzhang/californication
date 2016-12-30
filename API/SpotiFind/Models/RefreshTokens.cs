using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpotiFind.Models
{
    public class RefreshTokens
    {
        public int Id { get; set; }
        [Required]
        public string RefreshToken { get; set; }

    }
}