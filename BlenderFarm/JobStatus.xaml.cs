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
    public partial class JobStatus : Page
    {
        public JobStatus()
        {
            InitializeComponent();
            
            

            //load data for cards
            var jobs = GetJobs();
            if (jobs.Count > 0)
                ViewJobs.ItemsSource = jobs;
        }

        //collect user email and password to pass among pages
        string useremailtopass = "e2";
        string userpasswordtopass = "p2";
        
        public JobStatus(string e, string p) : this()
        {
            useremailtopass = e;
            userpasswordtopass = p;
            //gets jobs from database
            returnMsg message = new returnMsg();
            message = CreateRequest.request("get_jobs", useremailtopass, userpasswordtopass, null);
            //if there is a job available it will be added to a list to be displayed to the user
            if (message.data.GetLength(0) != 0)
                for (int i = 0; i < message.data.GetLength(0); i++)
                {
                    Job newJob = new Job();
                    newJob.No = Int32.Parse(message.data[i][0].ToString());
                    newJob.StepFrame = message.data[i][1].ToString();
                    newJob.Name = message.data[i][2].ToString();
                    newJob.StartFrame = message.data[i][3].ToString();
                    newJob.EndFrame = message.data[i][4].ToString();
                    newJob.Version = message.data[i][8].ToString();
                    newJob.Format = message.data[i][10].ToString();
                    newJob.JobStart = DateTime.Parse(message.data[i][14].ToString());
                    if(message.data[i][15] != null)
                    {
                        newJob.JobEnd = DateTime.Parse(message.data[i][15].ToString());
                    }
                    
                    
                    joblist.Add(newJob);
                }

            this.Loaded += new RoutedEventHandler(JobStatus_Loaded);
        }
        void JobStatus_Loaded(object sender, RoutedEventArgs e)
        {
            //this is empty for a reason, do not touch
        }

        //.blend file path
        public string filepath = "j";
        //array for video downloading
        public string[] forvideodownload = new string[2];
        //array for frames downloading
        public string[] forframedownload = new string[2];
        //populate this from the database in the future
        public List<Job> joblist = new List<Job>()
        {
            new Job()
        };

        //populate list with jobs for cards
        private List<Job> GetJobs()
        {
              
            return joblist;
        }
        private void jobstatusbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to/refresh job status page
            Page jobstatus = new JobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(jobstatus);
        }

        private void submitjobbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to submit job page
            Page submitjob = new SubmitJob(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(submitjob);
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
            if (thisjob.JobEnd != new DateTime()) //makes sure that button will only run once the job has actually ended
            {
                //get job id
                string id = thisjob.No.ToString();
                //get job path from backend
                string jobpath;
                jobpath = "/opt/lablibs/blender/data/" + id + "/frames.mp4";
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
                //add file's name to the path
                userpath += "\\frames.mp4";
                forvideodownload[1] = userpath;
                DownloadSFTP.Down(forvideodownload);
            }

                //using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                //{
                //    dlg.Description = Description;
                //    dlg.SelectedPath = Text;
                //    dlg.ShowNewFolderButton = true;
                //    DialogResult result = dlg.ShowDialog();
                //    if (result == System.Windows.Forms.DialogResult.OK)
                //    {
                //        Text = dlg.SelectedPath;
                //        BindingExpression be = GetBindingExpression(TextProperty);
                //        if (be != null)
                //            be.UpdateSource();
                //    }
                //}
                //FolderBrowserDialog fbd = new FolderBrowserDialog();
                //if(fbd.ShowDialog == System.Windows.Forms.DialogResult.OK)
                //{
                //    userpath = fbd.SelectedPath;
                //}
            }

        //download frames button
        private void fdownbtn_Click(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            if (thisjob.JobEnd != new DateTime()) //makes sure that button will only run once the job has actually ended
            {
                //get job id
                string id = thisjob.No.ToString();
                //get job path from backend
                string jobpath;
                jobpath = "/opt/lablibs/blender/data/" + id + "/frames.zip";
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
                userpath += "\\frames.zip";
                forframedownload[1] = userpath;
                DownloadSFTP.Down(forframedownload);
            }
            
        }

        //delete job button
        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            returnMsgNull msg = CreateRequest.requestNull("delete_job", useremailtopass, userpasswordtopass, thisjob.No.ToString()); // deletes job from database
            //testtextblock.Text = msg.success.ToString();
            //reloads page
            Page jobstatus = new JobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(jobstatus);
        }

        //pause/resume button
        private void pausebtn_Click_1(object sender, RoutedEventArgs e)
        {
            Job thisjob = ((Button)sender).Tag as Job;
            CreateRequest.requestNull("unpause_job", useremailtopass, userpasswordtopass, thisjob.No.ToString());
            
            
            //test
            
        }
    }
}
