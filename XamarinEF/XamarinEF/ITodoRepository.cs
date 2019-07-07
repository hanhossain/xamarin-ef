using System.Collections.Generic;
using System.Threading.Tasks;
using XamarinEF.Core;

namespace XamarinEF
{
	public interface ITodoRepository
	{
		void Initialize();

		Task<List<TodoItem>> GetTodoItemsAsync();

		Task<TodoItem> AddTodoItemAsync(TodoItem todoItem);

		Task DeleteTodoItemAsync(int id);
	}
}
