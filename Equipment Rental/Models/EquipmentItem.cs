using System.ComponentModel.DataAnnotations;

namespace Equipment_Rental.Models
{
    public class EquipmentItem
    {
        public int Id { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public int QuantityAvailable { get; set; }

        public string ImageUrl { get; set; }

        public string Condition { get; set; }

        public ICollection<RentalRequestItem> RentalItems { get; set; }
    }
}
