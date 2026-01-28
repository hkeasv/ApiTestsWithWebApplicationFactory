using System.Collections.Generic;
using TodoApi.Data;
using TodoApi.Models;

namespace ApiTestsWithWebApplicationFactory
{
    public static class DbInitializer
    {
        // This method will create and seed the database.
        public static void Initialize(TodoContext context)
        {
            // Delete the database, if it already exists.
            //context.Database.EnsureDeleted();

            // Create the database, if it does not already exists.
            //context.Database.EnsureCreated();

            context.TodoItems.RemoveRange(context.TodoItems);

            // Seed the database with test data.
            List<TodoItem> items = new List<TodoItem>
            {
                new TodoItem {IsComplete=true, Name="Use SqLite"},
                new TodoItem {IsComplete=false, Name="Exam project"}
            };
            
            context.TodoItems.AddRange(items);
            context.SaveChanges();
        }
    }
}
