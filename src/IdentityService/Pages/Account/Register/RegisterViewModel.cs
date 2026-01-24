using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.Register;

    public class RegisterViewModel
{ 
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set;}
        [Required]
        public string Username { get; set;}
        [Required]
        public string Fullname { get;}
        public string ReturnUrl { get;}
        public string Button { get;}
}

