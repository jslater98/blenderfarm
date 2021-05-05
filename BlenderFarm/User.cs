using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BlenderTake2
{
    public class User
    {
        public double ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public bool Preferred { get; set; }
        public bool Admin { get; set; }

        //contructor
        public User(double id, string email, string password, string code, bool preferred, bool admin)
        {
            ID = id;
            Email = email;
            Password = password;
            Code = code;
            Preferred = preferred;
            Admin = admin;
        }

        //constructor without bools, bools set to false, normal user
        public User(double id, string email, string password, string code)
        {
            ID = id;
            Email = email;
            Password = password;
            Code = code;
            Preferred = false;
            Admin = false;
        }

        //default contructor
        public User()
        {
            ID = -1;
            Email = "sample.user@ndsu.edu";
            Password = "samplepassword";
            Code = "default";
            Preferred = false;
            Admin = false;
        }
    }
}
