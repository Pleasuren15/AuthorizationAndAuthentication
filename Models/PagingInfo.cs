using System;

namespace AuthorizationAndAuthentication.Models
{
    public class PagingInfo
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int NumberOfPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
    }
}
