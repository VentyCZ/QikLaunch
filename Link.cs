using sh = IWshRuntimeLibrary;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System;

namespace QikLaunch
{
    public class Link
    {
        /// <summary>
        /// Representation of the IWshShortcut.WindowStyle integer
        /// </summary>
        public enum RunIn
        {
            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// Activates the window and displays it as a maximized window. 
            /// </summary>
            Maximized = 3,

            /// <summary>
            /// Minimizes the window and activates the next top-level window.
            /// </summary>
            Minimized = 7
        }

        /// <summary>
        /// Where's this shortcut located
        /// </summary>
        public string LinkLocation { get; private set; }

        /// <summary>
        /// Does this shortcut even exist ?
        /// </summary>
        public bool LinkExists { get { return File.Exists(LinkLocation); } }

        /// <summary>
        /// The shortcut icon
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Shortcut title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Shortcut description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Hotkey defined for the shorcut
        /// </summary>
        public string Hotkey { get; private set; }

        /// <summary>
        /// The target path
        /// </summary>
        public string TargetPath { get; private set; }

        /// <summary>
        /// The target arguments
        /// </summary>
        public string TargetArguments { get; private set; }

        /// <summary>
        /// Where to run the target
        /// </summary>
        public string TargetWorkingDirectory { get; private set; }

        /// <summary>
        /// The target startup state
        /// </summary>
        public RunIn TargetState { get; private set; }

        /// <summary>
        /// ProcessWindowStyle for the new process
        /// </summary>
        public ProcessWindowStyle TargetWindowStyle { get; private set; }

        /// <summary>
        /// If the shortcut process should be run as Admin
        /// </summary>
        public bool TargetAsAdmin { get; private set; }

        public Link(string linkLocation)
        {
            // Save the link location
            LinkLocation = linkLocation;

            // The shortcut does not exist
            if (!LinkExists)
                return;

            // Read the shortcut
            sh.IWshShortcut link = (sh.IWshShortcut)(new sh.WshShell()).CreateShortcut(linkLocation);

            // Get the relevant properties
            Icon = link.IconLocation;
            TargetPath = link.TargetPath;
            TargetArguments = link.Arguments;
            Description = link.Description;
            Title = Path.GetFileNameWithoutExtension(link.FullName);
            TargetWorkingDirectory = link.WorkingDirectory;
            Hotkey = link.Hotkey;
            TargetState = (RunIn)link.WindowStyle;

            // Convert the RunIn to ProcessWindowStyle
            if (TargetState == RunIn.Normal)
                TargetWindowStyle = ProcessWindowStyle.Normal;
            else if (TargetState == RunIn.Maximized)
                TargetWindowStyle = ProcessWindowStyle.Maximized;
            else if (TargetState == RunIn.Minimized)
                TargetWindowStyle = ProcessWindowStyle.Minimized;
            
            // NOTE: Experimental
            // (https://blogs.msdn.microsoft.com/abhinaba/2013/04/02/c-code-for-creating-shortcuts-with-admin-privilege/)
            using (FileStream fs = new FileStream(linkLocation, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(21, SeekOrigin.Begin);
                int b = fs.ReadByte();
                TargetAsAdmin = (b & 0x22) > 0;
            }
        }

        /// <summary>
        /// Runs the link
        /// </summary>
        public void Run()
        {
            try
            {
                var procInfo = new ProcessStartInfo()
                {
                    FileName = TargetPath,
                    Arguments = TargetArguments,
                    WorkingDirectory = TargetWorkingDirectory,
                    WindowStyle = TargetWindowStyle
                };

                if (TargetAsAdmin)
                {
                    procInfo.UseShellExecute = true;
                    procInfo.Verb = "runas";
                }

                Process.Start(procInfo);
            }
            catch (Exception ex)
            {
                // TODO
                MessageBox.Show(string.Format("An error while running the shortcut: {0} ({1})\n\nFile: {2}", ex.Message, ex.GetType().Name, TargetPath), "Cannot run shortcut", MessageBoxButton.OK);
            }
        }
    }
}
