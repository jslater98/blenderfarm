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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        //populate this from the database in the future
        public List<User> userlist = new List<User>()
        {
            new User(111, "my.admin@ndsu.edu", "adminpassword", "admincode", true, true),
            new User(222, "my.preferred@ndsu.edu", "preferredpassword", "preferredcode", true, false),
            new User(333, "my.user@ndsu.edu", "userpassword", "usercode", false, false)
        };
        private List<User> GetUsers()
        {
            return userlist;
        }
        public Admin()
        {
            InitializeComponent();
            //load data for cards
            var users = GetUsers();
            if (users.Count > 0)
                ViewUsers.ItemsSource = users;
        }

        //collect user email and password
        string useremailtopass = "e1";
        string userpasswordtopass = "p1";
        public Admin(string e, string p) : this()
        {
            useremailtopass = e;
            userpasswordtopass = p;
            this.Loaded += new RoutedEventHandler(Admin_Loaded);
        }
        void Admin_Loaded(object sender, RoutedEventArgs e)
        {
            //this is empty for a reason, do not touch
        }

        private void adminpagebtn_Click(object sender, RoutedEventArgs e)
        {
            //refresh admin page 
            Page adminpage = new Admin(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminpage);
        }

        private void normalbtn_Click(object sender, RoutedEventArgs e)
        {
            //establish the individual user from the card
            User thisuser = ((Button)sender).Tag as User;
            //test
            testtextblock.Text = thisuser.Email;
            //set admin to false

            //set preferred to false

        }

        private void preferredbtn_Click(object sender, RoutedEventArgs e)
        {
            //establish the individual user from the card
            User thisuser = ((Button)sender).Tag as User;
            //test
            testtextblock.Text = thisuser.Email;
            //set admin to false

            //set preferred to true

        }

        private void adminbtn_Click(object sender, RoutedEventArgs e)
        {
            //establish the individual user from the card
            User thisuser = ((Button)sender).Tag as User;
            //test
            testtextblock.Text = thisuser.Email;
            //set admin to true

            //set preferred to true

        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            //establish the individual user from the card
            User thisuser = ((Button)sender).Tag as User;
            //test
            testtextblock.Text = thisuser.Email;
            //remove user from database

        }

        private void logoutbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to login page
            Page login = new Login();
            this.NavigationService.Navigate(login);
        }

        private void jobstatusbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to job status page
            Page adminjobstatus = new AdminJobStatus(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminjobstatus);
        }

        private void submitjobbtn_Click(object sender, RoutedEventArgs e)
        {
            //transition to submit job page
            Page adminsubmitjob = new AdminSubmitJob(useremailtopass, userpasswordtopass);
            this.NavigationService.Navigate(adminsubmitjob);
        }
    }
}
