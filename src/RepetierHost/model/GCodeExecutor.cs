/*
   Copyright 2011 repetier repetierdev@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model
{
    public interface GCodeExecutor
    {
        void queueGCodeScript(string gcodeToExecute);
    }
    public class PrinterConnectionGCodeExecutor : GCodeExecutor
    {
        private PrinterConnection conn;
        private bool startImmediatelly;
        public PrinterConnectionGCodeExecutor(PrinterConnection conn, bool startImmediatelly)
        {
            this.conn = conn;
            this.startImmediatelly = startImmediatelly;
        }

        public void queueGCodeScript(string gcodeToExecute)
        {
            /*conn.connector.Job.BeginJob();
            conn.connector.GetInjectLock();
            foreach (string line in gcodeToExecute.Split(new char[] { '\n', '\r' }))
            {
                if (!String.IsNullOrEmpty(line))
                {
                    conn.connector.InjectManualCommand(line);
                }
            }
            conn.connector.ReturnInjectLock();
            conn.connector.Job.EndJob();*/


            // Load Generated GCode as current job.
            //Main.main.editor.Text = gcodeToExecute;
            Main.main.editor.setContent(0, gcodeToExecute);

            if (startImmediatelly)
            {
                // And then run it.
                conn.connector.RunJob();
            }
        }

    }
}
