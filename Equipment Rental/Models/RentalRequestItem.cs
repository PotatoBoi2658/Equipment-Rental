namespace Equipment_Rental.Models
{
    public class RentalRequestItem
    {
        public int RentalRequestId { get; set; }
        public RentalRequest RentalRequest { get; set; }

        public int EquipmentItemId { get; set; }
        public EquipmentItem EquipmentItem { get; set; }

        public int Quantity { get; set; }
    }
}
