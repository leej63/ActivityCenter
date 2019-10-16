using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CsharpBeltExam.Models
{
    public class Activityy
    {
        [Key]
        [Required]
        public int ActivityId {get;set;}

        [Required]
        public string Title {get;set;}

        [FutureDate]
        public DateTime Date {get;set;}

        // public int Time {get;set;}

        [Range(1,10000)]
        public int Duration {get;set;}
        public string DurationString {get;set;}

        [Required]
        public string Description {get;set;}

        public int UserId {get;set;}
        public int User {get;set;}
        public List<Participant> Participants {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
    
    public class FutureDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime today = DateTime.Now;
            if(today > (DateTime)value)
            {
                return new ValidationResult("Wedding must be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}