using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GiftStore.ViewModels
{
    public class AddActivityLogViewModel
    {

        
        
       
        public int UserId { get; set; }

        public string Description { get; set; } // "Income" or "Out"
       
   
    }
}
