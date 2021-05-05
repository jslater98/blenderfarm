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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        string useremailtopass = "e";
        string userpasswordtopass = "p";
        //populate this from the database in the future
        public List<User> userlist = new List<User>()
        {
            new User(111, "my.admin@ndsu.edu", "adminpassword!", "admincode", true, true),
            new User(222, "my.preferred@ndsu.edu", "preferredpassword@", "preferredcode", true, false),
            new User(333, "my.user@ndsu.edu", "userpassword#", "usercode", false, false)
        };
        //new account
        private User newUser = new User();
        private returnMsg message = new returnMsg();
        private returnMsgAccount accountMessage = new returnMsgAccount();
        public Login()
        {
            InitializeComponent();
        }

        private List<User> GetUsers()
        {
            return userlist;
        }


        private void loginbtn_Click(object sender, RoutedEventArgs e)
        {
            //email and password verification
            message = CreateRequest.request("get_jobs", emailTextBox.Text, passwordTextBox.Text, "");
            if(message.success)
            {
                //get email and password to pass
                useremailtopass = emailTextBox.Text;
                userpasswordtopass = passwordTextBox.Text;
                //transition to normal job status page
                Page jobstatus = new JobStatus(useremailtopass, userpasswordtopass);
                this.NavigationService.Navigate(jobstatus);
            }
            //trigger alert
            loginalert.Text = "Invalid username or password. Please try again.";
        }

        private void codebtn_Click(object sender, RoutedEventArgs e)
        {
            returnMsgNull Nullmessage = CreateRequest.requestNull("verify_account", newUser.Email, newUser.Password, codeTextBox.Text);
            if(Nullmessage.success)
            {
                codealert.Foreground = Brushes.Blue;
                codealert.Text = "Verification successful. You may now login into your account.";
            }
            else
            {
                codealert.Foreground = Brushes.Red;
                codealert.Text = message.errorMsg;
            }
        }

        private void createAccountbtn_Click(object sender, RoutedEventArgs e)
        {
            //code verification
            
                if(newPasswordTextBox.Text.Length>7)
                {
                    if(newPasswordTextBox.Text.Contains("!") || newPasswordTextBox.Text.Contains("@") || newPasswordTextBox.Text.Contains("#")
                        || newPasswordTextBox.Text.Contains("$") || newPasswordTextBox.Text.Contains("%") || newPasswordTextBox.Text.Contains("^")
                        || newPasswordTextBox.Text.Contains("&") || newPasswordTextBox.Text.Contains("*") || newPasswordTextBox.Text.Contains("(")
                        || newPasswordTextBox.Text.Contains(")"))
                    {
                        //get rid of alert
                        accountalert.Text = "";

                        //complete account entry with new password
                        newUser.Password = newPasswordTextBox.Text;
                        newUser.Email = newEmailTextBox.Text;
                    //create account with backend
                        accountMessage = CreateRequest.requestAccount("create_account", newUser.Email, newUser.Password, "");
                        if(message.success)
                        {
                            accountalert.Foreground = Brushes.Blue;
                            accountalert.Text = "Please check your email for the verification code";
                        }
                        else
                        {
                            accountalert.Foreground = Brushes.Red;
                            accountalert.Text = accountMessage.errorMsg;
                        }
                       
                        
                    }
                }
                else
                {
                    accountalert.Text = "Password does not meet requirements.";
                }
            
            
        }
    }
}
