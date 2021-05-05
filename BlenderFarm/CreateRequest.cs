using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlenderTake2
{
    public class Request
    {
        public string reqType { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string verificationCode { get; set; }
        public double jobID { get; set; }
        
    }

    public class returnMsg
    {
        public Boolean success { get; set; }
        public string errorMsg { get; set; }
        public Object[][] data { get; set; }
    }
    public class returnMsgAccount
    {
        public Boolean success { get; set; }
        public string errorMsg { get; set; }
    }
    public class returnMsgNull
    { 
        public Boolean success { get; set; }
        public string errorMsg { get; set; }
    }

    class CreateRequest
    {
        
        
        public static returnMsg request(string requestType, string username, string password, string args)
        {
            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            var port = 22;
            string sshUsername = "blenderssh";
            string sshPassword = "5h62sT9Aq4";
            Request newRequest = new Request();
            returnMsg newMessage = new returnMsg();
            newRequest.verificationCode = args;
            newRequest.reqType = requestType;
            newRequest.email = username;
            newRequest.password = password;
            string JsonRequest = JsonSerializer.Serialize(newRequest);  //serialize account information into json
            string command = $"python3 broker_to_frontend.py '{JsonRequest}' "; //make ssh command
            string receive;




            using (var client = new SshClient(host, port, sshUsername, sshPassword)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    ShellStream shellStreamSSH = client.CreateShellStream("vt-100", 200, 60, 800, 600, 65536); //The shellStream needs a (string terminalName, uint columns, uint rows, uint width, uint height, int bufferSize)
                    Console.WriteLine("connected to edlab");
                    shellStreamSSH.WriteLine(command); //runs command
                    receive = recvSSHData(shellStreamSSH);  //returns message from edlab and stores it into a string/json format
                    Console.WriteLine(receive); //writes return message
                    newMessage = JsonSerializer.Deserialize<returnMsg>(receive);  //deserialize json into newMessage item
                    
                    client.Disconnect();

                    return newMessage;

                }
                else
                {
                    Console.WriteLine("error connecting to edlab");
                    return newMessage;
                }
            }
        }
        public static returnMsgAccount requestAccount(string requestType, string username, string password, string args)
        {
            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            var port = 22;
            string sshUsername = "blenderssh";
            string sshPassword = "5h62sT9Aq4";
            Request newRequest = new Request();
            returnMsgAccount newMessage = new returnMsgAccount();
            if(requestType.Equals("verify_account"))
            {
                newRequest.verificationCode = args;

            }
            if(requestType.Equals("delete_job"))
            {
                newRequest.jobID = Double.Parse(args);
            }
            newRequest.reqType = requestType;
            newRequest.email = username;
            newRequest.password = password;
            string JsonRequest = JsonSerializer.Serialize(newRequest);  //serialize account information into json
            string command = $"python3 broker_to_frontend.py '{JsonRequest}' "; //make ssh command
            string receive;




            using (var client = new SshClient(host, port, sshUsername, sshPassword)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    ShellStream shellStreamSSH = client.CreateShellStream("vt-100", 200, 60, 800, 600, 65536); //The shellStream needs a (string terminalName, uint columns, uint rows, uint width, uint height, int bufferSize)
                    Console.WriteLine("connected to edlab");
                    shellStreamSSH.WriteLine(command); //runs command
                    receive = recvSSHData(shellStreamSSH);  //returns message from edlab and stores it into a string/json format
                    Console.WriteLine(receive); //writes return message
                    newMessage = JsonSerializer.Deserialize<returnMsgAccount>(receive);  //deserialize json into newMessage item

                    client.Disconnect();

                    return newMessage;

                }
                else
                {
                    Console.WriteLine("error connecting to edlab");
                    return newMessage;
                }
            }
        }
        public static void runNodes()
        {
            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            var port = 22;
            string sshUsername = "blenderssh";
            string sshPassword = "5h62sT9Aq4";
            
            string command = $"python3 broker_to_nodes.py"; //make ssh command




            using (var client = new SshClient(host, port, sshUsername, sshPassword)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    
                    SshCommand sc = client.CreateCommand(command); //runs commands
                    sc.BeginExecute();
                    client.Disconnect();

                    

                }
                else
                {
                    Console.WriteLine("error connecting to edlab");
                    
                }
            }
        }
        public static returnMsgNull requestNull(string requestType, string username, string password, string args)
        {
            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            var port = 22;
            string sshUsername = "blenderssh";
            string sshPassword = "5h62sT9Aq4";
            Request newRequest = new Request();
            returnMsgNull newMessage = new returnMsgNull();
            if (requestType.Equals("verify_account"))
            {
                newRequest.verificationCode = args;

            }
            if (requestType.Equals("delete_job") || requestType.Equals("pause_job") || requestType.Equals("unpause_job"))
            {
                newRequest.jobID = Double.Parse(args);
            }
            newRequest.reqType = requestType;
            newRequest.email = username;
            newRequest.password = password;
            string JsonRequest = JsonSerializer.Serialize(newRequest);  //serialize account information into json
            string command = $"python3 broker_to_frontend.py '{JsonRequest}' "; //make ssh command
            string receive;




            using (var client = new SshClient(host, port, sshUsername, sshPassword)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    ShellStream shellStreamSSH = client.CreateShellStream("vt-100", 200, 60, 800, 600, 65536); //The shellStream needs a (string terminalName, uint columns, uint rows, uint width, uint height, int bufferSize)
                    Console.WriteLine("connected to edlab");
                    shellStreamSSH.WriteLine(command); //runs command
                    receive = recvSSHData(shellStreamSSH);  //returns message from edlab and stores it into a string/json format
                    Console.WriteLine(receive); //writes return message
                    newMessage = JsonSerializer.Deserialize<returnMsgNull>(receive);  //deserialize json into newMessage item

                    client.Disconnect();

                    return newMessage;

                }
                else
                {
                    Console.WriteLine("error connecting to edlab");
                    return newMessage;
                }
            }
        }
        public static returnMsgAccount jobRequest(EnterJob jobDetails)
        {
            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            var port = 22;
            string sshUsername = "blenderssh";
            string sshPassword = "5h62sT9Aq4";
            returnMsgAccount newMessage = new returnMsgAccount();
            string JsonRequest = JsonSerializer.Serialize(jobDetails);  //serialize account information into json
            string command = $"python3 broker_to_frontend.py '{JsonRequest}' "; //make ssh command
            string receive;




            using (var client = new SshClient(host, port, sshUsername, sshPassword)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    ShellStream shellStreamSSH = client.CreateShellStream("vt-100", 200, 60, 800, 600, 65536); //The shellStream needs a (string terminalName, uint columns, uint rows, uint width, uint height, int bufferSize)
                    Console.WriteLine("connected to edlab");
                    shellStreamSSH.WriteLine(command); //runs command
                    receive = recvSSHData(shellStreamSSH);  //returns message from edlab and stores it into a string/json format
                    Console.WriteLine(receive); //writes return message
                    newMessage = JsonSerializer.Deserialize<returnMsgAccount>(receive);  //deserialize json into newMessage item

                    client.Disconnect();

                    return newMessage;

                }
                else
                {
                    Console.WriteLine("error connecting to edlab");
                    return newMessage;
                }
            }
        }
        public static string recvSSHData(ShellStream shellStreamSSH)
        {
            string returnMessage = "";
            while (true)
            {
                try
                {
                    if (shellStreamSSH != null && shellStreamSSH.DataAvailable)
                    {
                        string strData = shellStreamSSH.ReadLine(); //Reads every line that comes from the shell

                        if (strData.Contains("success"))  //makes sure to only return string with the right identifier
                        {
                            returnMessage += strData;
                            return returnMessage;  //returns edlabmessage in json format.
                        }
                        
                    }

                }
                catch
                {
                    return returnMessage;
                }


            }

        }
    }

}

