using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using RepetierHost.model;

namespace RepetierHost.view.utils
{
    public partial class RHUpdater : Form
    {
        public static string currentVersion = "0.90C";
        public static int buildVersion = 63;
        public static string newestVersion = "";
        public static int newestBuildVersion = 0;
        public static string updateText = "";
        public static bool silent = false;
        public static string downloadUrl = Custom.GetString("downloadUrl", "http://www.repetier.com/download/");
        public static bool running = false;
        public static Thread thread = null;
        public static string url = "";
        public static int error = 0;
        public RHUpdater()
        {
            InitializeComponent();
            translate();
            Main.main.languageChanged += translate;
        }
        public void translate()
        {
            Text = Trans.T("W_REPETIER_HOST_UPDATE_CHECK");
            buttonDownload.Text = Trans.T("B_DOWNLOAD");
            buttonRemindMe.Text = Trans.T("B_REMIND_ME_LATER");
            buttonSkipVersion.Text = Trans.T("B_SKIP_VERSION");
            labelAvailable.Text = Trans.T("L_AVAILABLE_VERSION");
            labelInstalled.Text = Trans.T("L_INSTALLED_VERSION");
            labelInformationOnUpdate.Text = Trans.T("L_INFORMATION_ON_UPDATE");
        }
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        public static long timeInSeconds()
        {
            DateTime dt = DateTime.Now;
            return dt.Ticks / TimeSpan.TicksPerSecond;
        }
        public static bool pingServer(string hostNameOrAddress)
        {
            bool pingStatus = false;

            Ping p = new Ping();
            byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            try {
                    PingReply reply = p.Send(hostNameOrAddress, 120, buffer);
                    pingStatus = (reply.Status == IPStatus.Success);
            }
            catch(Exception) {
                //Console.WriteLine("Ping:"+e.ToString());
                pingStatus = false;
            }

            return pingStatus;
        }
        public static MethodInvoker Finished = delegate
        {
            running = false;
            Main.main.checkForUpdatesToolStripMenuItem.Enabled = true;
            if (error == 1 && !silent)
                MessageBox.Show(Trans.T("L_UPDATE_NO_CONNECTION"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            error = 0;
        };
        public static void checkForUpdates(bool silent) {
            RHUpdater.silent = silent;
            if (Custom.GetBool("removeUpdates", false)) return; // Do not try to look for updates.
            url = "http://www.repetier.com/updates/rh/version_windows.txt";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                url = "http://www.repetier.com/updates/rh/version_linux.txt";
            url = Custom.GetString("updateUrl", url);

            long lastcheck = RegMemory.GetLong("lastUpdateCheck", 0);
            if (silent && timeInSeconds() - lastcheck < 86400 * 7) return; // Test only once a week silently
            //if (!pingServer("176.9.149.139")) return; // No network

            thread = new Thread(RHUpdater.CheckThread);
            running = true;
            Main.main.checkForUpdatesToolStripMenuItem.Enabled = false;
            thread.Start();
        }
        
        public static void CheckThread() {
            // Get the URI from the command line.
            Uri httpSite = new Uri(url);

            // Create the request object.
            WebRequest wreq = WebRequest.Create(httpSite);

            // Create the state object.
            RequestState rs = new RequestState();

            // Put the request into the state object so it can be passed around.
            rs.Request = wreq;

            // Issue the async request.
            IAsyncResult r = (IAsyncResult)wreq.BeginGetResponse(
               new AsyncCallback(RespCallback), rs);

            // Wait until the ManualResetEvent is set so that the application 
            // does not exit until after the callback is called.
            //allDone.WaitOne();

            //Console.WriteLine(rs.RequestData.ToString());
        }

        private static void RespCallback(IAsyncResult ar)
        {
            try
            {
                // Get the RequestState object from the async result.
                RequestState rs = (RequestState)ar.AsyncState;

                // Get the WebRequest from RequestState.
                WebRequest req = rs.Request;

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                WebResponse resp = req.EndGetResponse(ar);

                //  Start reading data from the response stream.
                Stream ResponseStream = resp.GetResponseStream();

                // Store the response stream in RequestState to read 
                // the stream asynchronously.
                rs.ResponseStream = ResponseStream;

                //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
                IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0,
                   BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
            }
            catch (Exception)
            {
                //Console.WriteLine("Update:"+e);
                error = 1;
                Main.main.Invoke(RHUpdater.Finished);
            }
        }


        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                // Get the RequestState object from AsyncResult.
                RequestState rs = (RequestState)asyncResult.AsyncState;

                // Retrieve the ResponseStream that was set in RespCallback. 
                Stream responseStream = rs.ResponseStream;

                // Read rs.BufferRead to verify that it contains data. 
                int read = responseStream.EndRead(asyncResult);
                if (read > 0)
                {
                    // Prepare a Char array buffer for converting to Unicode.
                    Char[] charBuffer = new Char[BUFFER_SIZE];

                    // Convert byte stream to Char array and then to String.
                    // len contains the number of characters converted to Unicode.
                    int len =
                       rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);

                    String str = new String(charBuffer, 0, len);

                    // Append the recently read data to the RequestData stringbuilder
                    // object contained in RequestState.
                    rs.RequestData.Append(
                       Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                    // Continue reading data until 
                    // responseStream.EndRead returns –1.
                    IAsyncResult ar = responseStream.BeginRead(
                       rs.BufferRead, 0, BUFFER_SIZE,
                       new AsyncCallback(ReadCallBack), rs);
                }
                else
                {
                    if (rs.RequestData.Length > 0)
                    {
                        //  Display data to the console.
                        string strContent;
                        strContent = rs.RequestData.ToString();
                        parseVersion(strContent);
                        if (buildVersion < newestBuildVersion)
                        {
                            if (silent && RegMemory.GetInt("checkUpdateSkipBuild", 0) == newestBuildVersion)
                                return; // User didn't want to see this update.
                            Main.main.Invoke(RHUpdater.Execute);
                        }
                        else if (!silent)
                        {
                            // MessageBox.Show("No new updates available.\r\nYou are using the latest version.", "Update status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageBox.Show(Trans.T("L_NO_NEW_UPDATES"), Trans.T("L_UPDATE_STATUS"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    // Close down the response stream.
                    responseStream.Close();
                    // Set the ManualResetEvent so the main thread can exit.
                    allDone.Set();
                }
            }
            finally
            {
                Main.main.Invoke(RHUpdater.Finished);
            }
            return;
        }
        static RHUpdater form = null;
        public static MethodInvoker Execute = delegate
        {
            if (form == null)
                form = new RHUpdater();
            form.labelInstalledVersion.Text = currentVersion;
            form.labelAvailableVersion.Text = newestVersion;
            form.textUpdate.Text = updateText;
            form.textUpdate.Select(0, 0);
            form.Show();
        };
        static public void parseVersion(string s)
        {
            string[] arr = s.Split('\n');
            if (arr.Length < 4) return;
            newestVersion = arr[0].Trim();
            int.TryParse(arr[1].Trim(), out newestBuildVersion);
            downloadUrl = arr[2].Trim();
            StringBuilder b = new StringBuilder();
            for (int i = 3; i < arr.Length; i++)
            {
                b.AppendLine(arr[i].Trim());
            }
            updateText = b.ToString();
            RegMemory.SetLong("lastUpdateCheck", timeInSeconds());
        }

        private void buttonRemindMe_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            Main.main.openLink(downloadUrl); 
            Hide();
        }

        private void buttonSkipVersion_Click(object sender, EventArgs e)
        {
            RegMemory.SetInt("checkUpdateSkipBuild", newestBuildVersion);
            Hide();
        }
    }
    // The RequestState class passes data across async calls.
    public class RequestState
    {
        const int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;
        // Create Decoder for appropriate enconding type.
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();

        public RequestState()
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(String.Empty);
            Request = null;
            ResponseStream = null;
        }
    }
}
