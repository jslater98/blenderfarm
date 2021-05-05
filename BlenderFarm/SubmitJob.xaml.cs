using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlenderTake2
{
    /// <summary>
    /// Interaction logic for SubmitJob.xaml
    /// </summary>
    public partial class SubmitJob : Page
    {
        public string[] files;
        //.blend file
        public string file = "g";
        //.blend file path
        public string filepath = "j";
        //array for uploading
        public string[] forupload = new string[2];
        //blender version
        public string version = "h";

        public SubmitJob()
        {
            InitializeComponent();
            beginbtn.IsEnabled = false;
        }

        //collect user email and password
        string useremailtopass = "e4";
        string userpasswordtopass = "p4";
        public SubmitJob(string e, string p) : this()
        {
            useremailtopass = e;
            userpasswordtopass = p;
            this.Loaded += new RoutedEventHandler(SubmitJob_Loaded);
        }
        void SubmitJob_Loaded(object sender, RoutedEventArgs e)
        {
            //this is empty for a reason, do not touch
        }

        private void jobstatusbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to job status page
            Page jobstatus = new JobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(jobstatus);
        }

        private void submitjobbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to/refresh submit job page
            Page submitjob = new SubmitJob(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(submitjob);
        }

        private void logoutbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to login page
            Page login = new Login();
            this.NavigationService.Navigate(login);
        }

        private void BlendFilePanel_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                //just want the first one, but if user inputs another file, the new one becomes the first one
                string anothertemp = files[0];
                //preserve the path
                filepath = files[0];
                forupload[0] = filepath;
                //split up the path
                char[] del = { '\\' };
                string[] temp = anothertemp.Split(del);
                anothertemp = temp[temp.Length - 1];
                if (anothertemp.Contains(".blend") == false)
                {
                    draglabel.Text = "File must be a .blend file. Please try again.";
                    draglabel.Foreground = Brushes.Gray;
                }
                else
                {
                    file = temp[temp.Length - 1];
                    draglabel.Text = file;
                    draglabel.Foreground = Brushes.Black;
                    beginbtn.IsEnabled = true;
                }
            }
        }

        //submit job button
        private void beginbtn_Click(object sender, RoutedEventArgs e)
        {
            //jobstart and jobend, jobend will be provided by database in future
            DateTime now = DateTime.Now;

            //no, will be provided by database in future
            double dbNumber = 2;

            //step frame
            int stepframe = 1;
            int step = 1;
            if (string.IsNullOrEmpty(stepframeTextBox.Text))
            {
                stepframe = 1;
            }
            if (int.TryParse(stepframeTextBox.Text, out step) == true)
            {
                stepframe = step;
            }
            else
            {
                stepframe = 1;
            }
            //format
            string format = "mem";
            if (pngbtn.IsChecked == true)
            {
                format = "PNG";
            }
            if (jpgbtn.IsChecked == true)
            {
                format = "JPG";
            }
            if (tiffbtn.IsChecked == true)
            {
                format = "TIFF";
            }
            else
            {
                format = "OPEN_EXR";
            }

            //startframe
            int sf = 0;
            int startframe = 0;
            if (string.IsNullOrEmpty(startframeTextBox.Text))
            {
                startframe = 0;
            }
            if (int.TryParse(startframeTextBox.Text, out sf) == true)
            {
                startframe = sf;
            }
            else
            {
                startframe = 0;
            }

            //endframe
            int ef = 0;
            int endframe = 0;
            if (string.IsNullOrEmpty(endframeTextBox.Text))
            {
                endframe = 0;
            }
            if (int.TryParse(endframeTextBox.Text, out ef) == true)
            {
                endframe = ef;
            }
            else
            {
                endframe = 0;
            }

            //version
            string version = "0";
            if (Two76btn.IsChecked == true)
            {
                version = "2.76";
            }
            if (Two77btn.IsChecked == true)
            {
                version = "2.77";
            }
            if (Two78btn.IsChecked == true)
            {
                version ="2.78";
            }
            if (Two80btn.IsChecked == true)
            {
                version = "2.80";
            }
            if (Two81btn.IsChecked == true)
            {
                version = "2.81";
            }
            if (Two82btn.IsChecked == true)
            {
                version = "2.82";
            }
            if (Two83btn.IsChecked == true)
            {
                version = "2.83";
            }
            if (Two90btn.IsChecked == true)
            {
                version = "2.90";
            }
            if (Two91btn.IsChecked == true)
            {
                version = "2.91";
            }
            else
            {
                version = "2.92";
            }

            EnterJob myJob = new EnterJob(useremailtopass, userpasswordtopass, nameTextBox.Text, dbNumber, stepframe, format, startframe, endframe, now, file, version);
            returnMsgAccount message = new returnMsgAccount();
            returnMsg returnMessage = new returnMsg();
            returnMsgNull msg = new returnMsgNull();
            string jobID = "";
            //add myJob to database
            message = CreateRequest.jobRequest(myJob);
            

            returnMessage = CreateRequest.request("get_jobs", useremailtopass, userpasswordtopass, null); //returns message from broker
            //testlabel2.Content = returnMessage.data[returnMessage.data.GetLength(0) - 1][0].ToString(); // testing
            jobID = returnMessage.data[returnMessage.data.GetLength(0) - 1][0].ToString();  //gets jobID from data and turns it into a string
            
            //upload blend file
            forupload[1] = "/opt/lablibs/blender/data/" + jobID + "/";
            UploadSFTP.Up(forupload);
            msg = CreateRequest.requestNull("unpause_job", useremailtopass, userpasswordtopass, jobID); //runs unpause request after uploading blend files
            CreateRequest.runNodes();
            //transition to job status page
            Page jobstatus = new JobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(jobstatus);
        }

    }
}
