using DAL.Account;
using Hotel.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Hotel.Models
{
    public class UserWithRoleViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        public UserWithRoleViewModel()
        { }

        public UserWithRoleViewModel(ApplicationUser user)
        {
            ApplicationRole role = RoleAccess.GetRoleByUser(user);
            Id = user?.Id;
            RoleName = role?.Name;
            RoleId = role?.Id;
            UserName = user?.UserName;
            PhoneNumber = user?.PhoneNumber;
            Email = user?.Email;
        }
    }
}