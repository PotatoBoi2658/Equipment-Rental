namespace Equipment_Rental.Models
{
    using Microsoft.AspNetCore.Identity;

    public class RentalRequest
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Purpose { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected

        
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public ICollection<RentalRequestItem> Items { get; set; }
    }
}
