using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CsharpBeltExam.Models
{
    public class User
    {
        [Key]
        [Required]
        public int UserId {get;set;}

        [Required(ErrorMessage = " First Name must be atleast 2 characters.")]
        [MinLength(2)]
        [Display(Name = "First Name: ")]
        public string FirstName {get;set;}

        [Required(ErrorMessage = "Last Name must be atleast 2 characters.")]
        [MinLength(2)]
        [Display(Name = "Last Name: ")]
        public string LastName {get;set;}

        [Required(ErrorMessage = "A valid email address is required.")]
        [EmailAddress]
        [Display(Name = "Email: ")]
        public string Email {get;set;}

        [Required(ErrorMessage = "Password must be atleast 8 characters.")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password: ")]
        public string Password {get;set;}

        [Required]
        [NotMapped]
        [MinLength(8)]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password: ")]
        public string Confirm {get;set;}

        public List<Participant> Participating {get;set;}
        public List<Activityy> CreatedActivities {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}