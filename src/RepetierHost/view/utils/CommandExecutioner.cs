using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace RepetierHost.view.utils
{
    /// <summary>
    /// Executes a command asynchronously and writes output into the log file.
    /// </summary>
    public class CommandExecutioner : IDisposable
    {
        private Process process;
        private string exe = "";
        private string arguments = "";
        private string exeName = "execute";
        private static List<CommandExecutioner> list = new List<CommandExecutioner>();

        public void setExeArgs(string cmd)
        {
            cmd = cmd.Trim();
            int exeEnd = 0;
            int cmdLength = cmd.Length;
            if (cmd.StartsWith("\""))
            {
                exeEnd = 1;
                while (exeEnd < cmdLength && cmd[exeEnd] != '"') exeEnd++;
                exe = cmd.Substring(1, exeEnd - 1);
            }
            else
            {
                while (exeEnd < cmdLength && cmd[exeEnd] != ' ') exeEnd++;
                exe = cmd.Substring(0, exeEnd);
            }
            if (exeEnd + 1 < cmdLength)
                arguments = cmd.Substring(exeEnd + 1);
            else
                arguments = "";
            FileInfo f = new FileInfo(exe);
            exeName = f.Name;
        }
        public string wrapQuotes(string text)
        {
            if (text.StartsWith("\"") && text.EndsWith("\"")) return text;
            return "\"" + text.Replace("\"", "\\\"") + "\"";
        }
        public void run()
        {
            if (!File.Exists(exe)) return;
            list.Add(this);
            process = new Process();
            try
            {
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(runFinsihed);
                process.StartInfo.FileName = Main.IsMono ? exe : wrapQuotes(exe);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                process.StartInfo.Arguments = arguments;
                process.Start();
                // Start the asynchronous read of the standard output stream.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
                list.Remove(this);
            }
        }
        private void runFinsihed(object sender, System.EventArgs e)
        {
            process.Close();
            process.Dispose();
            process = null;
            list.Remove(this);
        }
        private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Main.conn.log("<" + exeName + "> " + outLine.Data, false, 4);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (process != null)
                {
                    process.Dispose();
                    process = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Disposable types implement a finalizer.
        ~CommandExecutioner()
        {
            Dispose(false);
        }
    }
}
