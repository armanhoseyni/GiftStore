﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftStore.Models
{
    public class Factors
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

        public DateTime FactorDate { get; set; }

        [ForeignKey("GiftCards")]
        public int GiftId { get; set; }

        public double? FactorPrice { get; set; } // New field

        // Navigation properties

        public Users User { get; set; }

     
        public GiftCards GiftCard { get; set; }
      

        public bool Status { get; set; }
        public string Type { get; set; }
        public string TransActionNumber { get; set; }
    }
}
