using System.ComponentModel.DataAnnotations;

namespace ForgetPasswordAPI.Models
{

        public class User
        {
            [Key]
            public long Id { get; set; }

            [Required]
            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            [MinLength(8)]
            public string? Password { get; set; }
        
    }
}
