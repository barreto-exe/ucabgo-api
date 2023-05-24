﻿using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Auth.Inputs
{
    public class ChangePasswordInput : BaseRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one letter and one number.")]
        public string NewPassword { get; set; }
    }
}