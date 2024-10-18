using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10283090_.Models
{
    public class Claims
    {
        [Key]
        public int ClaimID { get; set; }

        [Display(Name = "Lecturer ID")]
        public string? UserID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Claims period start date is required.")]
        public DateTime ClaimsPeriodStart { get; set; }

        [Required(ErrorMessage = "Claims period end date is required.")]
        public DateTime ClaimsPeriodEnd { get; set; }

        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Rate per hour is required.")]
        public double RatePerHour { get; set; }

        
        public double TotalAmount { get; set; }

        public string DescriptionofWork { get; set; }

        public string? Status { get; set; }

        [Display(Name = "File Data")]
        public byte[]? Data { get; set; }

        [Display(Name = "File Name")]
        public string? FileName { get; set; }

        [Display(Name = "Content Type")]
        public string? ContentType { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile SingleFile { get; set; }

        [Display(Name = "File Size")]
        public long? Length { get; set; }
    }
}
