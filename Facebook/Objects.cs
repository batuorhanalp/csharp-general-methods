using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook
{
    public class APIKey
    {
        public string ConstId { get; set; }
        public string RedirectUrl { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string FullRedirectUrl { get; set; }
    }
    public class UserInfo
    {
        public decimal FacebookId { get; set; }
        public string FacebookName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }
        public bool Verified { get; set; }
        public int Gender { get; set; }
        public string FacebookUrl { get; set; }
        public decimal Location { get; set; }
    }
}
