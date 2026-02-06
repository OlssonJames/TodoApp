namespace TodoApp
{
    public class TodoTask
    {
        public string TaskName { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }

        public bool IsDone { get; set; }
        public DateTime DueAt { get; set; }

    }
}
