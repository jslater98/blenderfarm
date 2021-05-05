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
    class UploadSFTP
    {
        public static void Up(string[] args)  //takes two arguments, path to .blend file to be uploaded and path to location on remote machine to store file (/opt/lablibs/blender)
        {

            //set ssh values
            var host = "edlab.cs.ndsu.nodak.edu";
            //var host = "lab00.cs.ndsu.nodak.edu";
            var port = 22;
            var username = "blenderssh";
            var password = "5h62sT9Aq4";
            

            var uploadFile = @args[0]; //path for file you want to upload
            var uploadLocation = @args[1]; //path to where to upload file on remote machine

            using (var client = new SftpClient(host, port, username, password)) //create SSH client
            {
                client.Connect();  //attempt to connect to edlab
                if (client.IsConnected)
                {
                    Debug.WriteLine("connected to edlab");
                    client.ChangeDirectory(uploadLocation);

                    using (var fileStream = new FileStream(uploadFile, FileMode.Open)) //create stream for sending files
                    {
                        
                        client.BufferSize = 4 * 1024; //bypasses Payload error for large files
                        client.UploadFile(fileStream, Path.GetFileName(uploadFile)); //upload file
                        //client.RenameFile(Path.GetFileName(uploadFile), "src.blend");
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
