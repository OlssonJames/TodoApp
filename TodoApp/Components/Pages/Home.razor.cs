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
        private int? _userId;
        private bool _initialized;
        public List<TodoTask> TasksModel { get; set; } = new();
        [Inject] public AuthService Auth { get; set; } = default!;
        [Inject] public NavigationManager Nav { get; set; } = default!;
        private bool _loaded;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender || _initialized) return;
    _initialized = true;

    _userId = await Auth.GetUserId();
    if (_userId is null)
    {
        Nav.NavigateTo("/login", true);
        return;
    }

    await LoadTask(_userId.Value);
    StateHasChanged();
}

        private async Task AddTask()
        {
            if (_userId is null)
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
                cmd.Parameters.AddWithValue("@UserId", _userId.Value);
                cmd.Parameters.AddWithValue("@TaskName", TaskName);

                await cmd.ExecuteNonQueryAsync();

                TaskName = "";
                await LoadTask(_userId.Value);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async System.Threading.Tasks.Task DeleteTask(int TaskId)
        {
            if (_userId is null) return;
            _isSaving = true;
            try
            {
                await using var conn = new SqlConnection(ConnStr);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "dbo.DeleteTask";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", TaskId);
                cmd.Parameters.AddWithValue("@UserId", _userId.Value);

                await cmd.ExecuteNonQueryAsync();


                await LoadTask(_userId.Value);

            }
            finally
            {
                _isSaving = false;
            }



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
        private async Task ToggleDone(int taskId, bool isDone)
        {
            if (_userId is null) return;

            await using var conn = new SqlConnection(ConnStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.SetTaskDone";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", taskId);
            cmd.Parameters.AddWithValue("@UserId", _userId.Value);
            cmd.Parameters.AddWithValue("@IsDone", isDone);

            await cmd.ExecuteNonQueryAsync();
            await LoadTask(_userId.Value);
            StateHasChanged();
        }


    }
}
