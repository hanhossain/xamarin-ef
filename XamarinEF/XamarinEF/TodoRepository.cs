using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace XamarinEF
{
	public class TodoRepository : ITodoRepository
	{
		public void Initialize()
		{
			using (var context = new DatabaseContext())
			{
				// TODO: look into migrations
				context.Database.EnsureCreated();
				context.SaveChanges();
			}
		}

		public async Task<List<TodoItem>> GetTodoItemsAsync()
		{
			using (var context = new DatabaseContext())
			{
				return await context.TodoItems.ToListAsync();
			}
		}

		public async Task<TodoItem> AddTodoItemAsync(TodoItem todoItem)
		{
			using (var context = new DatabaseContext())
			{
				var savedItem = context.Add(todoItem);
				await context.SaveChangesAsync();

				return savedItem.Entity;
			}
		}

		public async Task DeleteTodoItemAsync(int id)
		{
			using (var context = new DatabaseContext())
			{
				var savedItem = await context.TodoItems.FindAsync(id);
				context.Remove(savedItem);
				await context.SaveChangesAsync();
			}
		}
	}
}
