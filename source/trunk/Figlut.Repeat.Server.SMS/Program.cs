using Figlut.Repeat.SMS.Clickatell;
using Figlut.Repeat.SMS.Zoom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Figlut.Repeat.SMS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SendSmsViaZoom();
                //SendSmsViaClickatell();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        private static void SendSmsViaZoom()
        {
            //ZoomSmsSender sender = new ZoomSmsSender(
            //    "https://zoomconnect.com/zoom/api/rest/v1/sms/send",
            //    "paul.kolozsvari@binarychef.com",
            //    "14242598-7cd2-470c-80a0-9a8e80c999eb",
            ZoomSmsSender sender = new ZoomSmsSender(
                "https://zoomconnect.com/zoom/api/rest/v1/sms/send",
                "paul.kolozsvari@binarychef.com",
                "b6d5fa52-56e2-4190-b742-ec4d3697f034",
                true);
            SmsResponse response = sender.SendSms(new ZoomSmsRequest("0833958283", "Hello from Paul at Zoom. This is the second test.", 130, null, null, null));
        }

        private static void SendSmsViaClickatell()
        {
            //ClickatellSmsSender sender = new ClickatellSmsSender(
            //    "http://api.clickatell.com/http/sendmsg", 
            //    "paulkolo", 
            //    "XMRDMMgTCbERFP", 
            //    "3531847");
            ClickatellSmsSender sender = new ClickatellSmsSender(
                "http://api.clickatell.com/http/sendmsg",
                "paulkolo",
                "password",
                "3531847",
                true);
            SmsResponse response = sender.SendSms(new ClickatellSmsRequest("0833958283", "This is another test by Paul from Clickatell", 130, null, null, null));
        }
    }
}
