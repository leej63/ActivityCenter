using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CsharpBeltExam.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId {get;set;}

        public int UserId {get;set;}

        public int ActivityId{get;set;}

        public User User {get;set;}          
        public Activityy Activity {get;set;}      
    }
}