using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BlenderTake2
{
    public class Job
    {
        public string Name { get; set; }
        public double No { get; set; }
        //public string Engine { get; set; }
        public string Format { get; set; }
        //public string VideoFormat { get; set; }
        public string StepFrame { get; set; }
        //public string XReso { get; set; }
        //public string YReso { get; set; }
        public string StartFrame { get; set; }
        public string EndFrame { get; set; }
        public DateTime JobStart { get; set; }
        public DateTime JobEnd { get; set; }
        public string BlendFile { get; set; }
        public string Version { get; set; }
        public double Progress { get; set; }

        //most of these numbers are strings so that they can be turned into a large string for the render command
        //and because they need to be strings for the displays
        public Job(string name, double no, string stepframe, string format,
            string startframe, string endframe, TimeSpan jobtime, DateTime jobstart, DateTime jobend, string blendfile, string version)
        {
            Name = name;
            No = no; //job number
            //Engine = engine;
            Format = format;
            //VideoFormat = videoformat;
            StepFrame = stepframe;
            //XReso = xreso;
            //YReso = yreso;
            StartFrame = startframe;
            EndFrame = endframe;
            JobStart = jobstart;
            JobEnd = jobend;
            BlendFile = blendfile;
            Version = version;
            Progress = 0;
        }

        //alt constructor where JobTime is calculated instead of inserted
        public Job(string name, double no, string stepframe, string format,
            string startframe, string endframe, DateTime jobstart, DateTime jobend, string blendfile, string version)
        {
            Name = name;
            No = no;
            //Engine = engine;
            Format = format;
            //VideoFormat = videoformat;
            StepFrame = stepframe;
            //XReso = xreso;
            //YReso = yreso;
            StartFrame = startframe;
            EndFrame = endframe;
            JobStart = jobstart;
            JobEnd = jobend;
            BlendFile = blendfile;
            Version = version;
            Progress = 0;
        }

        //default constructor
        public Job()
        {
            Name = "a";
            No = -1;
            //Engine = "b";
            StepFrame = "b";
            Format = "c";
            StartFrame = "d";
            EndFrame = "e";
            JobStart = new DateTime(1988, 1, 19);
            JobEnd = new DateTime();
            BlendFile = "f";
            Version = "g";
            Progress = 0;
        }
    }
    public class EnterJob
    {
        public string reqType { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string title { get; set; }
        public double No { get; set; }
        //public string Engine { get; set; }
        public string outputImagetype { get; set; }
        //public string VideoFormat { get; set; }
        public int stepLength { get; set; }
        //public string XReso { get; set; }
        //public string YReso { get; set; }
        public int startFrame { get; set; }
        public int endFrame { get; set; }

        public DateTime JobStart { get; set; }

        public string BlendFile { get; set; }
        public string version { get; set; }
        public double Progress { get; set; }

        //most of these numbers are strings so that they can be turned into a large string for the render command
        //and because they need to be strings for the displays
        public EnterJob(string username, string inPassword, string inTitle, double no, int stepframe, string inOutputImagetype,
            int startframe, int endframe, DateTime jobstart, string blendfile, string InVersion)
        {
            reqType = "create_job";
            email = username;
            password = inPassword;
            title = inTitle;
            No = no; //job number
            //Engine = engine;
            outputImagetype = inOutputImagetype;
            //VideoFormat = videoformat;
            stepLength = stepframe;
            //XReso = xreso;
            //YReso = yreso;
            startFrame = startframe;
            endFrame = endframe;
            
            JobStart = jobstart;
         
            BlendFile = blendfile;
            version = InVersion;
            Progress = 0;
        }

        

        //default constructor
        public EnterJob()
        {
            title = "a";
            No = -1;
            //Engine = "b";
            stepLength = 1;
            outputImagetype = "c";
            startFrame = 0;
            endFrame = 0;
            JobStart = new DateTime(1988, 1, 19);
            BlendFile = "f";
            version = "g";
            Progress = 0;
        }
    }
}
