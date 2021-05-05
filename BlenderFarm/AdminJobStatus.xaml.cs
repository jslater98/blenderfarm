using BlenderTake2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlenderTake2
{
    /// <summary>
    /// Interaction logic for JobStatus.xaml
    /// </summary>
    public partial class AdminJobStatus : Page
    {
        //.blend file path
        public string filepath = "j";
        //array for video downloading
        public string[] forvideodownload = new string[2];
        //array for frames downloading
        public string[] forframedownload = new string[2];
        //populate this from the database in the future
        public List<Job> joblist = new List<Job>()
        {
            new Job("one", 1, "two", "three", "four", "five", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("i", 1, "don't", "know", "why", "you're", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("a", 1, "mighty", "fortress", "is", "our", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("how", 1, "much", "wood", "could", "a", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("nae", 1, "pi", "ttam", "nunmeul", "nae", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("the", 1, "fitness", "gram", "pacer", "test", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("we", 1, "the", "people", "in", "order", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion"),
            new Job("closed", 1, "on", "Sunday", "you're", "my", new DateTime(2020, 1, 23), new DateTime(2020, 1, 28), "boop.blend", "someversion")
        };
        public AdminJobStatus()
        {
            InitializeComponent();

            //load data for cards
            var jobs = GetJobs();
            if (jobs.Count > 0)
                ViewJobs.ItemsSource = jobs;
        }

        //collect user email and password
        string useremailtopass = "e3";
        string userpasswordtopass = "p3";
        public AdminJobStatus(string e, string p) : this()
        {
            useremailtopass = e;
            userpasswordtopass = p;
            this.Loaded += new RoutedEventHandler(AdminJobStatus_Loaded);
        }
        void AdminJobStatus_Loaded(object sender, RoutedEventArgs e)
        {
            //this is empty for a reason, do not touch
        }

        //populate list with jobs for cards
        private List<Job> GetJobs()
        {
            return joblist;
        }
        private void jobstatusbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to/refresh job status page
            Page adminjobstatus = new AdminJobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminjobstatus);
        }

        private void submitjobbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to submit job page
            Page adminsubmitjob = new AdminSubmitJob(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminsubmitjob);
        }

        private void logoutbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to login page
            Page login = new Login();
            this.NavigationService.Navigate(login);
        }

        //download video button
        private void vdownbtn_Click(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            //get job id
            double id = thisjob.No;
            //get job path from backend
            string jobpath;
            jobpath = "/opt/lablibs/blender/btest/simple.blend";
            forvideodownload[0] = jobpath;
            //make user pick path for download
            string userpath = "";
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                userpath = path;
            }
            //add the file's name to the end
            userpath += "\\simple.blend";
            forvideodownload[1] = userpath;
            DownloadSFTP.Down(forvideodownload);
        }

        //download frames button
        private void fdownbtn_Click(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            //get job id
            double id = thisjob.No;
            //get job path from backend
            string jobpath;
            jobpath = "/opt/lablibs/blender/btest/simple.blend";
            forframedownload[0] = jobpath;
            //make user pick path for download
            string userpath = "";
            //Create a "Save As" dialog for selecting a directory(HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                userpath = path;
            }
            //add the file's name to the end
            userpath += "\\simple.blend";
            forframedownload[1] = userpath;
            DownloadSFTP.Down(forframedownload);
        }

        //delete job button
        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            //test
            testtextblock.Text = thisjob.Name;
        }

        //pause/resume button
        private void pausebtn_Click_1(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            //test
            testtextblock.Text = useremailtopass.ToString();
        }

        private void adminpagebtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to admin page
            Page admin = new Admin(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(admin);
        }
    }
}
