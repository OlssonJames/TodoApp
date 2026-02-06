using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TodoApp.Components.Pages

{

public partial class Home
    {
    
        public string TaskName { get; set; } = "";
        public int id { get; set; }

        private const string ConnStr = "Server=.;Database=Todo;Trusted_Connection=true;Encrypt=False";

        private bool _isSaving;

        private int LastCreatedTaskId;
        public List<TodoTask> TasksModel { get; set; } = new();
        [Inject] public AuthService Auth { get; set; } = default!;
        [Inject] public NavigationManager Nav { get; set; } = default!;
        private bool _loaded;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender || _loaded)
                return;

            _loaded = true;

            var userId = await Auth.GetUserId();
            if (userId is null)
            {
                Nav.NavigateTo("/login", true);
                return;
            }

            await LoadTask(userId.Value);
            StateHasChanged();
        }


        private async Task AddTask()
        {
            var userId = await Auth.GetUserId();
            if (userId is null)
            {
                Nav.NavigateTo("/login", true);
                return;
            }

            if (_isSaving) return;
            if (string.IsNullOrWhiteSpace(TaskName)) return;

            _isSaving = true;
            try
            {
                await using var conn = new SqlConnection(ConnStr);
                await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "dbo.InsertTask";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", userId.Value);
                cmd.Parameters.AddWithValue("@TaskName", TaskName);

                await cmd.ExecuteNonQueryAsync();

                TaskName = "";
                await LoadTask(userId.Value);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async System.Threading.Tasks.Task DeleteTask(int TaskId)
        {
            var userId = await Auth.GetUserId();
            if (userId is null)
            {
                Nav.NavigateTo("/login", true);
                return;
            }
            
            await using var conn = new SqlConnection(ConnStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.DeleteTask";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", TaskId);
            cmd.Parameters.AddWithValue("@UserId", userId.Value);

            await cmd.ExecuteNonQueryAsync();
            
          
            await LoadTask(userId.Value);

        }
        private async System.Threading.Tasks.Task LoadTask(int UserId)
        {
            TasksModel.Clear();
            await using var conn = new SqlConnection(ConnStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.GetTasks";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", UserId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                TasksModel.Add(new TodoTask
                {
                    TaskId = reader.GetInt32(0),
                    TaskName = reader.GetString(1),
                    IsDone = reader.GetBoolean(2)
                });
            }


        }
        private async System.Threading.Tasks.Task ToggleDone(int taskId, bool IsDone)
        {
            var userId = await Auth.GetUserId();
            if (userId is null)
            {
                Nav.NavigateTo("/login", true);
                return;
            }
            await using var conn = new SqlConnection(ConnStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.SetTaskDone";   // ← MISSING LINE
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", taskId);
            cmd.Parameters.AddWithValue("@IsDone", IsDone);


            await cmd.ExecuteNonQueryAsync();
            await LoadTask(userId.Value);

        }


    }
}
