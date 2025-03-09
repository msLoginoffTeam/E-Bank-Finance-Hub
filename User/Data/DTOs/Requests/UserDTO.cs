using System.ComponentModel.DataAnnotations;
using UserApi.Data.Models;

namespace UserApi.Data.DTOs.Requests
{
    public class UserDTO
    {
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.(?:[a-zA-Z]{2,})(?:\\.[a-zA-Z]{2,})*$", ErrorMessage = "Неверная электронная почта")]
        public string Email { get; set; }


        [RegularExpression("^.{6,128}$", ErrorMessage = "Пароль должен быть длиной от 6 до 128 символов")]
        public string Password { get; set; }


        [RegularExpression("^([A-Za-zА-Яа-я]+\\s){1,2}[A-Za-zА-Яа-я]+$", ErrorMessage = "Полное имя должно состоять как минимум из фамилии и имени")]
        public string FullName { get; set; }
    }
}
