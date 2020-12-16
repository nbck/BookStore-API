﻿using System.ComponentModel.DataAnnotations;

namespace BookStore_UI.Models
{
    public class RegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name="Email Address")]
        public string EmailAddress { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1}", MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "the password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }
    
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name="Email Address")]
        public string EmailAddress { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}