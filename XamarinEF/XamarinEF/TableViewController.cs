using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Microsoft.EntityFrameworkCore;
using UIKit;

namespace XamarinEF
{
	public class TableViewController : UITableViewController
	{
		private const string _cellId = "cellId";

		private List<TodoItem> _todoCache = new List<TodoItem>();

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			TableView.RegisterClassForCellReuse(typeof(UITableViewCell), _cellId);

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
					await AddTodoItem(text);
				});
				alert.AddAction(ok);

				await PresentViewControllerAsync(alert, true);
			});

			using (var context = new DatabaseContext())
			{
				_todoCache = await context.TodoItems.ToListAsync();
			}
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return _todoCache.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(_cellId, indexPath);

			cell.TextLabel.Text = _todoCache[indexPath.Row].Description;

			return cell;
		}

		public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
		{
			var deleteAction = UIContextualAction.FromContextualActionStyle(
				UIContextualActionStyle.Destructive,
				"Delete",
				async (contextualAction, view, success) =>
				{
					await DeleteTodoItem(indexPath);
					success(true);
				});

			return UISwipeActionsConfiguration.FromActions(new[] { deleteAction });
		}

		private async Task AddTodoItem(string description)
		{
			using (var context = new DatabaseContext())
			{
				var todoItem = new TodoItem()
				{
					Description = description
				};

				var savedItem = context.Add(todoItem);

				await context.SaveChangesAsync();

				_todoCache.Add(savedItem.Entity);

				var indexPath = NSIndexPath.FromRowSection(_todoCache.Count - 1, 0);
				InvokeOnMainThread(() => TableView.InsertRows(new[] { indexPath }, UITableViewRowAnimation.Automatic));
			}
		}

		private async Task DeleteTodoItem(NSIndexPath indexPath)
		{
			using (var context = new DatabaseContext())
			{
				var cachedItem = _todoCache[indexPath.Row];
				var todoItem = await context.TodoItems.FindAsync(cachedItem.Id);
				context.Remove(todoItem);

				await context.SaveChangesAsync();
				_todoCache.RemoveAt(indexPath.Row);

				InvokeOnMainThread(() => TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic));
			}
		}
	}
}
