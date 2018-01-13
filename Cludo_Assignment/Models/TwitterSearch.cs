using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cludo_Assignment.Models
{
    public class TwitterSearch
    {
        [Display(Name = "Please enter hashtag for search on twitter")]
        public string HashTag { get; set; }

        [Display(Name = "Please enter term for filtering search result")]
        public string Filter { get; set; }

        public List<KeyValuePair<int,string>> TwitterResult { get; set; }
    }
}