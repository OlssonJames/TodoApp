using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;


namespace TodoApp.Components.Pages
{
    public partial class Login
    {

        public string UserName { get; set; }
        public string Gmail { get; set; }
        public string Number { get; set; }

        private const string ConnStr = "Server=.;Database=Todo;Trusted_Connection=true;Encrypt=False";

        public string Status { get; set; } = "";


        private async Task InsertUser()
        {
            Status = "Clicked...";

            if (string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(Gmail) ||
                string.IsNullOrWhiteSpace(Number))
            {
                Status = "Please fill in all fields.";
                return;
            }

            try
            {
                await using var conn = new SqlConnection(ConnStr);
                await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "dbo.InsertUser";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Email", Gmail);
                cmd.Parameters.AddWithValue("@Number", Number);

                var result = await cmd.ExecuteScalarAsync();
                if (result is null)
                {
                    Status = "InsertUser returned null (check stored procedure OUTPUT).";
                    return;
                }

                int userId = Convert.ToInt32(result);

                await Auth.SetUserId(userId);

                Status = $"Logged in as {UserName} (Id={userId}). Redirecting...";
                Nav.NavigateTo("/home");
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
        }


    }
}
