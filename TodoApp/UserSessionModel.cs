namespace TodoApp
{
    public class UserSessionModel
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsLoggedIn => UserId.HasValue;
    }
}
