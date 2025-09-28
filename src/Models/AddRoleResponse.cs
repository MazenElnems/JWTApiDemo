namespace JWTAuthApp.Models
{
    public class AddRoleResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
}
