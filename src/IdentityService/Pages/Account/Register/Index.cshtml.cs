using System.Security.Claims;
using Duende.IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
    public class IndexModel : PageModel
    {
 private readonly UserManager<ApplicationUser> _usermanager;

 public IndexModel(UserManager<ApplicationUser> usermanager)
    {
    _usermanager = usermanager;
    }

    [BindProperty]
     public RegisterViewModel Input { get; set; }

     [BindProperty]
     public bool RegisterSuccess { get; set;}
        
        public IActionResult OnGet(string returnUrl = null)
         {
         Input = new RegisterViewModel 
         {
             ReturnUrl = returnUrl
         };
     
        return Page();
         }
        public async Task<IActionResult> OnPost()
        {
         if (Input.Button != "register")
         {
             return Redirect(Input.ReturnUrl ?? "~/");      
         }
           if(ModelState.IsValid)
           {
             var user = new ApplicationUser 
             { 
                 UserName = Input.Username, 
                 Email = Input.Email,
                 FullName = Input.FullName,
                 EmailConfirmed = true
             };
             var result = await _usermanager.CreateAsync(user, Input.Password);
             if (result.Succeeded)
             {
                await _usermanager.AddClaimAsync(user, 
                new Claim( JwtClaimTypes.Name, Input.FullName ?? ""));
                 RegisterSuccess = true;
                
             }
             foreach (var error in result.Errors)
             {
                 ModelState.AddModelError(string.Empty, error.Description);
             }
            
           }
             return Page();
         }
    }
