namespace GiftStore.ViewModels
{
    public class UpdateUserViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string? Email { get; set; }

        public string? Password { get; set; }
        public string? RePassword { get; set; }
        public string Role { get; set; }

    }
}
