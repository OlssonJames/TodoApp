namespace TodoApp
{
    using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

    public class AuthService
    {
        private readonly ProtectedSessionStorage _storage;
        private const string Key = "UserId";

        public AuthService(ProtectedSessionStorage storage)
        {
            _storage = storage;
        }

        public async Task SetUserId(int userId)
        {
            await _storage.SetAsync(Key, userId);
        }

        public async Task<int?> GetUserId()
        {
            var result = await _storage.GetAsync<int>(Key);
            return result.Success ? result.Value : null;
        }

        public async Task Logout()
        {
            await _storage.DeleteAsync(Key);
        }
    }
}
