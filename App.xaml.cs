using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace QikLaunch
{
    public partial class App : Application
    {
        private static List<Link> links;

        protected override void OnStartup(StartupEventArgs e)
        {
            startIndexing();

            var q = new Spotlight();
            q.Show();
        }

        /// <summary>
        /// Gets matching links for the specified term (against link title)
        /// </summary>
        /// <param name="term">The search query</param>
        /// <returns>Matching links</returns>
        public static IOrderedEnumerable<Link> Autocomplete(string term)
        {
            // Get the lowercase version of the term
            var lc = term.ToLower();

            // Get all the matched links
            return from lnk in links
                   where lnk.Matches(lc)
                   orderby lnk.Title ascending
                   select lnk;
        }

        /// <summary>
        /// Initializes indexing of the Start Menu shortcuts
        /// </summary>
        private static void startIndexing()
        {
            if (links != null)
                links.Clear();
            else
                links = new List<Link>();

            // Get path to the Start Menu folder
            var StartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            var CMStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

            // TODO: Make async
            indexFolder(CMStartMenuPath);
            indexFolder(StartMenuPath);

            Console.WriteLine(links.Count);
        }

        /// <summary>
        /// Indexes the specified folder
        /// </summary>
        /// <param name="folder">Full path to a folder</param>
        private static void indexFolder(string folder)
        {
            // Sanity check
            if (!Directory.Exists(folder))
                return;

            // TODO: A lot
            try
            {
                Console.WriteLine("Indexing {0} ...", folder);

                var files = from f in Directory.EnumerateFiles(folder) where f.EndsWith(".lnk") select f;

                // TODO: Index the files
                foreach (string file in files)
                {
                    // Parse the lnk
                    var l = new Link(file);

                    // Add the link to the list
                    if (l.LinkExists)
                        links.Add(l);
                }

                // Index the sub-directories
                foreach (string dir in Directory.EnumerateDirectories(folder))
                    indexFolder(dir);
            }
            catch (Exception ex)
            {
                // NOTE: Mostly gonna get UnauthorizedAccessExceptions ...

                Console.WriteLine("{0} on {1}! ({2})", ex.GetType().Name, folder, ex.Message);
            }
        }
    }
}
