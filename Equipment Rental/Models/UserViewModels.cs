using System.ComponentModel.DataAnnotations;

namespace Equipment_Rental.Models
{
    public class UserListViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Потребителското име е задължително.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Потребителското име е задължително.")]
        public string UserName { get; set; }

        // Добавяме тези 3 нови свойства:
        public string Role { get; set; }
        public List<string> Roles { get; set; } // За падащото меню

        [DataType(DataType.Password)]
        public string NewPassword { get; set; } // Не е Required, защото може да не искаме да я сменяме
    }
}
