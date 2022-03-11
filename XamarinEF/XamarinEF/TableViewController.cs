using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using XamarinEF.Core;

namespace XamarinEF
{
	public class TableViewController : UITableViewController
	{
		private const string _cellId = "cellId";

		private readonly ITodoRepository _todoRepository;

		private List<TodoItem> _todoCache = new List<TodoItem>();

		public TableViewController(ITodoRepository todoRepository)
		{
			_todoRepository = todoRepository;
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			TableView.RegisterClassForCellReuse(typeof(SubtitleTableViewCell), _cellId);

			Title = "Todo";

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, async (o, e) =>
			{
				// bring up alert
				var alert = UIAlertController.Create("Add task", null, UIAlertControllerStyle.Alert);

				alert.AddTextField(textField => textField.Placeholder = "Description");

				var cancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
				alert.AddAction(cancel);

				var ok = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, async alertAction =>
				{
					var text = alert.TextFields.First().Text;
					await AddTodoItemAsync(text);
				});
				alert.AddAction(ok);

				await PresentViewControllerAsync(alert, true);
			});

			_todoCache = await _todoRepository.GetTodoItemsAsync();
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return _todoCache.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(_cellId, indexPath);

			var item = _todoCache[indexPath.Row];
			cell.TextLabel.Text = item.Description;
			cell.DetailTextLabel.Text = item.Completed ? "Done" : string.Empty;

			return cell;
		}

		public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
		{
			var deleteAction = UIContextualAction.FromContextualActionStyle(
				UIContextualActionStyle.Destructive,
				"Delete",
				async (contextualAction, view, success) =>
				{
					await DeleteTodoItemAsync(indexPath);
					success(true);
				});

			return UISwipeActionsConfiguration.FromActions(new[] { deleteAction });
		}

        public override UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
			var completedAction = UIContextualAction.FromContextualActionStyle(
				UIContextualActionStyle.Normal,
				"Complete",
				async (contextualAction, view, success) =>
                {
					await CompleteTodoItemAsync(indexPath);
					success(true);
                });

			return UISwipeActionsConfiguration.FromActions(new[] { completedAction });
        }

        private async Task AddTodoItemAsync(string description)
		{
			var todoItem = await _todoRepository.AddTodoItemAsync(new TodoItem()
			{
				Description = description
			});

			_todoCache.Add(todoItem);

			var indexPath = NSIndexPath.FromRowSection(_todoCache.Count - 1, 0);
			InvokeOnMainThread(() => TableView.InsertRows(new[] { indexPath }, UITableViewRowAnimation.Automatic));
		}

		private async Task DeleteTodoItemAsync(NSIndexPath indexPath)
		{
			var cachedItem = _todoCache[indexPath.Row];

			await _todoRepository.DeleteTodoItemAsync(cachedItem.Id);
			_todoCache.RemoveAt(indexPath.Row);

			InvokeOnMainThread(() => TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic));
		}

		private async Task CompleteTodoItemAsync(NSIndexPath indexPath)
        {
			var cachedItem = _todoCache[indexPath.Row];

			await _todoRepository.CompleteTodoItemAsync(cachedItem.Id);
			cachedItem.Completed = true;

			InvokeOnMainThread(() => TableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.Automatic));
        }
	}
}
