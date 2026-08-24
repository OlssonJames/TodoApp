# TodoApp

A multi-user todo application built with Blazor (Interactive Server render mode) on .NET 10, as part of my .NET developer studies. Users sign up, get a session, and manage their own task list stored in SQL Server.

## Features

- Signup flow that creates a user and starts a session (ProtectedSessionStorage via a small AuthService)
- Add tasks, mark them as done and delete them
- Tasks are stored per user in SQL Server
- All database access uses parameterized stored procedures (InsertUser, InsertTask, GetTasks, SetTaskDone, DeleteTask)

## Tech stack

- C# / .NET 10
- Blazor (Interactive Server components)
- SQL Server with ADO.NET (Microsoft.Data.SqlClient)

## Getting started

1. Create a SQL Server database named Todo with Users and Tasks tables plus the stored procedures listed above.
2. Adjust the connection string in the page code-behind files if your SQL Server instance differs (default: Server=.;Database=Todo;Trusted_Connection=true).
3. Run the app from the TodoApp project folder:

   ```
   dotnet run
   ```

## Project structure

- Components/Pages/Home.razor – the task list (load, add, toggle done, delete)
- Components/Pages/Singup.razor – signup page that creates a user and redirects to the task list
- AuthService.cs – keeps the logged-in user id in protected session storage
- TodoTask.cs, UserModel.cs – model classes

## Planned improvements

- Move the connection string to appsettings.json
- Add a proper login page for returning users
