using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamarinEF
{
	public interface ITodoRepository
	{
		Task<List<TodoItem>> GetTodoItemsAsync();

		Task<TodoItem> AddTodoItemAsync(TodoItem todoItem);

		Task DeleteTodoItemAsync(int id);
	}
}
