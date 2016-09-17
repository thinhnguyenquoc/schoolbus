using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace SchoolBus.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<UsersContext>(null);
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<SchoolProfile> SchoolProfiles { get; set; }
        public DbSet<StudentAddress> StudentAddresses { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Station> Stations { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    [Table("SchoolProfile")]
    public class SchoolProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public ICollection<StudentAddress> Student { get; set; }
    }

    [Table("StudentAddress")]
    public class StudentAddress
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentAddressId { get; set; }
        public string Address { get; set; }
        public string GAddress { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int SchoolId { get; set; }
        
        public int WardId { get; set; }
        public virtual Ward Ward { get; set; }
        public virtual SchoolProfile School { get; set; }
    }

    [Table("Ward")]
    public class Ward
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WardId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; }
        public int? DistrictId { get; set; }
        public virtual District Districts { get; set; }
        public ICollection<StudentAddress> StudentAddresses { get; set; }
        public ICollection<Station> Stations { get; set; }
    }

    [Table("District")]
    public class District
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DistrictId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; }
        public int? CityId { get; set; }

        public ICollection<Ward> Wards { get; set; }
    }

    [Table("Station")]
    public class Station
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StationId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Neighbor { get; set; }
        public string Street { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }

        public virtual Ward Ward { get; set; }
    }

    public class MyMap
    {
        public List<SchoolProfile> schools { get; set; }
        public List<StudentAddress> studentaddresses { get; set; }
        public List<Ward> wards { get; set; }
        public List<District> districts { get; set; }
    }
}
