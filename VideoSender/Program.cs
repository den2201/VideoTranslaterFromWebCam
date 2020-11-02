using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace VideoSender
{
    class Program
    {
        static IPEndPoint endPoint;
        static UdpClient udpClient = new UdpClient();

        
         
        static void Main(string[] args)
        {
            var considerIP =  ConfigurationManager.AppSettings.Get("considerIP");
            var considerPort = int.Parse(ConfigurationManager.AppSettings.Get("considerPort"));
            endPoint = new IPEndPoint(IPAddress.Parse(considerIP), considerPort);
            
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
            Console.ReadLine();
        }

        private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bitMap = new Bitmap(eventArgs.Frame, 800, 600);

            try
            {
                using (var ms = new MemoryStream())
                {
                    bitMap.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    udpClient.Send(bytes, bytes.Length, endPoint);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
