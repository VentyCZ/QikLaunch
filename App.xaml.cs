using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace QikLaunch
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //startIndexing();

            var q = new Spotlight();
            q.Show();
        }

        /// <summary>
        /// Initializes indexing of the Start Menu shortcuts
        /// </summary>
        private static void startIndexing()
        {
            var StartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

            // TODO: Make async
            indexFolder(StartMenuPath);
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
                //foreach (string file in files)
                //{
                //    var l = new Link(file);
                //}

                foreach (string dir in Directory.EnumerateDirectories(folder))
                {
                    indexFolder(dir);
                }
            }
            catch (Exception ex)
            {
                // NOTE: Mostly gonna get UnauthorizedAccessExceptions ...

                Console.WriteLine("{0} on {1}! ({2})", ex.GetType().Name, folder, ex.Message);
            }
        }
    }
}
