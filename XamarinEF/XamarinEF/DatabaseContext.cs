using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace XamarinEF
{
	public class DatabaseContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			SQLitePCL.Batteries_V2.Init();

			// found path from:
			// https://stackoverflow.com/questions/47237414/what-is-the-best-environment-specialfolder-for-store-application-data-in-xamarin
			string databasePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				"..",
				"Library",
				"database.db");

			optionsBuilder.UseSqlite($"Filename={databasePath}");
		}

		public DbSet<TodoItem> TodoItems { get; set; }
	}

	public class TodoItem
	{
		[Key]
		public int Id { get; set; }

		public string Description { get; set; }
	}
}
