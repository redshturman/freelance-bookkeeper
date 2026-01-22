using FreelanceBookkeeper.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FreelanceBookkeeper.Data
{
    /// <summary>
    /// Represents the application's database context.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Gets the database set for managing expense entities within the context.
        /// </summary>
        /// <remarks>Use this property to query, add, update, or remove expenses from the underlying
        /// database. Changes made to the returned set are tracked by the context and persisted when SaveChanges is
        /// called.</remarks>
        public DbSet<Expense> Expenses => Set<Expense>();

        /// <summary>
        /// Gets the set of customer transaction entities for querying and saving.
        /// </summary>
        /// <remarks>This property provides access to the collection of <see cref="CustomerTransaction"/>
        /// entities in the context. Use this property to perform LINQ queries, add new transactions, or update existing
        /// ones within the database context.</remarks>
        public DbSet<CustomerTransaction> CustomerTransactions => Set<CustomerTransaction>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FreelanceBookkeeper");

            Directory.CreateDirectory(appDataPath);

            var dbPath = Path.Combine(appDataPath, "freelance_bookkeeper.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
