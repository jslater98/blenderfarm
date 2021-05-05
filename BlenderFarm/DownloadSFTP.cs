using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace BlenderTake2
{
    class DownloadSFTP
    {
        //takes two arguments, path to .blend file to be downloaded and path on local machine to download location (note: local path must include a name and .blend extention for the file)
        public static void Down(string[] args)
        {

            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            var port = 22;
            var username = "blenderssh";
            var password = "5h62sT9Aq4";

            var remotePath = @args[0];  //path to file in edlab that you want to download
            var localPath = @args[1]; //path to where you want to download file on local machine (including file name)

            using (var client = new SftpClient(host, port, username, password)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    Debug.WriteLine("connected to edlab");

                    using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    //using (Stream fileStream = File.Create(localPath)) //create location for file to be downloaded to
                    {

                        client.BufferSize = 4 * 1024; //bypasses Payload error for large files
                        client.DownloadFile(remotePath, fileStream); //download file
                    }
                }
                else
                {
                    Debug.WriteLine("error connecting to edlab");
                }
            }
        }
    }
}
