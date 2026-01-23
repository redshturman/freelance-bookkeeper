using System;
using System.Collections.Generic;
using System.Text;

namespace FreelanceBookkeeper.Models
{
    /// <summary>
    /// Represents a group of months for filtering.
    /// </summary>
    public class MonthGroup
    {
        /// <summary>
        /// The actual group of months.
        /// </summary>
        public int Group { get; set; }
        /// <summary>
        /// The name of the group of months to show.
        /// </summary>
        public string Display { get; set; } = string.Empty;
    }
}
