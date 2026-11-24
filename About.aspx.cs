using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using eStoreJA.Models;

namespace eStoreJA
{
    /// <summary>About us page (dark theme). The department count and names are read from the database.</summary>
    public partial class About : Page
    {
        /// <summary>Number of departments (rows in Categories), shown on the pink card.</summary>
        protected int DepartmentCount { get; private set; }

        /// <summary>The count in words for the journey text ("six"); digits above twelve.</summary>
        protected string DepartmentCountWords { get; private set; }

        /// <summary>Department names as a sentence: "Electronics, Fashion and Beauty."</summary>
        protected string DepartmentNames { get; private set; }

        /// <summary>Switches the master to the navy theme and loads the department figures.</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-dark";

            List<string> names;
            using (var db = new ApplicationDbContext())
            {
                names = db.Categories.OrderBy(c => c.CategoryId).Select(c => c.Name).ToList();
            }

            DepartmentCount = names.Count;
            DepartmentCountWords = CountInWords(names.Count);
            DepartmentNames = JoinWithAnd(names);
        }

        /// <summary>"A, B and C." – commas between names, "and" before the last one.</summary>
        private static string JoinWithAnd(IList<string> names)
        {
            if (names.Count == 0) return "Coming soon.";
            if (names.Count == 1) return names[0] + ".";
            return string.Join(", ", names.Take(names.Count - 1)) + " and " + names[names.Count - 1] + ".";
        }

        /// <summary>0–12 as words (reads better in a sentence), anything larger as digits.</summary>
        private static string CountInWords(int n)
        {
            string[] words = { "zero", "one", "two", "three", "four", "five", "six",
                               "seven", "eight", "nine", "ten", "eleven", "twelve" };
            return n >= 0 && n < words.Length ? words[n] : n.ToString();
        }
    }
}
