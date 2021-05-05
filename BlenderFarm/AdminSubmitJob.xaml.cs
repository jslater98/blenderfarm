using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for SubmitJob.xaml
    /// </summary>
    public partial class AdminSubmitJob : Page
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

        public AdminSubmitJob()
        {
            InitializeComponent();
            beginbtn.IsEnabled = false;
        }

        //collect user email and password
        string useremailtopass = "e4";
        string userpasswordtopass = "p4";
        public AdminSubmitJob(string e, string p) : this()
        {
            useremailtopass = e;
            userpasswordtopass = p;
            this.Loaded += new RoutedEventHandler(AdminSubmitJob_Loaded);
        }
        void AdminSubmitJob_Loaded(object sender, RoutedEventArgs e)
        {
            //this is empty for a reason, do not touch
        }

        private void jobstatusbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to job status page
            Page adminjobstatus = new AdminJobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminjobstatus);
        }

        private void submitjobbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to/refresh submit job page
            Page adminsubmitjob = new AdminSubmitJob(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminsubmitjob);
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
            string stepframe = "gregian";
            int step = 1;
            if (string.IsNullOrEmpty(stepframeTextBox.Text))
            {
                stepframe = "1";
            }
            if (int.TryParse(stepframeTextBox.Text, out step) == true)
            {
                stepframe = Convert.ToString(step);
            }
            else
            {
                stepframe = "1";
            }

            //format
            string format = "mem";
            if (mp4btn.IsChecked == true)
            {
                format = "MP4";
            }
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
            string startframe = "grain";
            if (string.IsNullOrEmpty(startframeTextBox.Text))
            {
                startframe = "0";
            }
            if (int.TryParse(startframeTextBox.Text, out sf) == true)
            {
                startframe = Convert.ToString(sf);
            }
            else
            {
                startframe = "0";
            }

            //endframe
            int ef = 0;
            string endframe = "bumbo";
            if (string.IsNullOrEmpty(endframeTextBox.Text))
            {
                endframe = "0";
            }
            if (int.TryParse(endframeTextBox.Text, out ef) == true)
            {
                endframe = Convert.ToString(ef);
            }
            else
            {
                endframe = "0";
            }

            //version
            if (Two76btn.IsChecked == true)
            {
                format = "2.76";
            }
            if (Two77btn.IsChecked == true)
            {
                format = "2.77";
            }
            if (Two78btn.IsChecked == true)
            {
                format = "2.78";
            }
            if (Two80btn.IsChecked == true)
            {
                format = "2.80";
            }
            if (Two81btn.IsChecked == true)
            {
                format = "2.81";
            }
            if (Two82btn.IsChecked == true)
            {
                format = "2.82";
            }
            if (Two83btn.IsChecked == true)
            {
                format = "2.83";
            }
            if (Two90btn.IsChecked == true)
            {
                format = "2.90";
            }
            if (Two91btn.IsChecked == true)
            {
                format = "2.91";
            }
            else
            {
                format = "2.92";
            }

            Job myJob = new Job(nameTextBox.Text, dbNumber, stepframe, format, startframe, endframe, now, now, file, version);

            //add myJob to database

            //pass values to backend

            //upload blend file
            forupload[1] = "/opt/lablibs/blender/btest/";
            UploadSFTP.Up(forupload);

            //transition to job status page
            Page adminjobstatus = new AdminJobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminjobstatus);
        }

        private void adminpagebtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to admin page
            Page adminpage = new Admin(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminpage);
        }
    }
}
