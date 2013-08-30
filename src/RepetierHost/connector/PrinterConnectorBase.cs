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
using System.Windows.Forms;
using Microsoft.Win32;
using RepetierHost.model;

namespace RepetierHost.connector
{
    /// <summary>
    /// Base class for all ways to connect a 3d printer to the host.
    /// Each connection type has to inherit and implement this interface.
    /// </summary>
    public abstract class PrinterConnectorBase
    {
        public delegate void OnPauseChanged(bool paused);
        /// <summary>
        /// These delegate methods are called after pause state is changed.
        /// </summary>
        public OnPauseChanged eventPauseChanged;
 
        abstract public void Activate();
        abstract public void Deactivate();
        abstract public bool Connect();
        abstract public bool Disconnect(bool force);
        abstract public bool IsConnected();
        abstract public void InjectManualCommand(string command);
        abstract public void InjectManualCommandFirst(string command);
        abstract public bool HasInjectedMCommand(int code);
        abstract public UserControl ConnectionDialog();
        abstract public string Name { get; }
        abstract public string Id { get; }
        abstract public void SetConfiguration(RegistryKey key);
        abstract public void SaveToRegistry();
        abstract public void LoadFromRegistry();
        abstract public void Emergency();
        abstract public void RunJob();
        abstract public void PauseJob(string text);
        abstract public void ContinueJob();
        abstract public void KillJob();
        abstract public bool IsJobRunning();
        abstract public void TrySendNextLine();
        abstract public void ResendLine(int line);
        abstract public void GetInjectLock();
        abstract public void ReturnInjectLock();
        abstract public bool IsPaused { get; }
        abstract public int MaxLayer { get; }
        abstract public void RunPeriodicalTasks();
        abstract public void ToggleETAMode();
        abstract public string ETA { get; }
        abstract public Printjob Job { get; }
        abstract public int InjectedCommands { get; }
        abstract public void AnalyzeResponse(string res);
    }
}
