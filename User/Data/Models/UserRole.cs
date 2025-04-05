using UserApi.Data.Models;

namespace User_Api.Data.Models
{
    public class UserRole
    {
        public User User { get; set; }

        public Role Role { get; set; }


        public UserRole() {}
        public UserRole(User User, Role Role)
        {
            this.User = User;
            this.Role = Role;
        }
    }
}
