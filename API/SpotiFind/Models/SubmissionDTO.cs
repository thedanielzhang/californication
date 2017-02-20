using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotiFind.Models
{
    public class SubmissionDTO
    {
        public int LocationId { get; set; }
        public string TrackId { get; set; }
        public string AccessToken { get; set; }
    }
}