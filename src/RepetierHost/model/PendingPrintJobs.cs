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
using System.IO;

namespace RepetierHost.model
{
    public class PendingPrintJobsException : Exception
    {
        public PendingPrintJobsException(string message, Exception e) : base(message, e) {}
    }

    /// <summary>
    /// This class provides methods to get the list of pending jobs.
    /// </summary>
    public class PendingPrintJobs
    {
        public const string RepetierExtension = "repstate";
        public const string PendingJobsDirName = "pending";

        public static List<PendingPrintJob> GetPendingJobs()
        {
            List<PendingPrintJob> list = new List<PendingPrintJob>();
            foreach (string file in GetPendingJobsFiles())
            {
                PendingPrintJob job = new PendingPrintJob(file);
                list.Add(job);
            }
            return list;
        }

        public static void Add(PrintingStateSnapshot snapshot, string name)
        {
            PendingPrintJob job = new PendingPrintJob(snapshot, PendingJobsDir + Path.DirectorySeparatorChar + name + "." + RepetierExtension);
            job.Save();
        }

        /// <summary>
        /// Returns the list of files that are inside the pending jobs directory.
        /// </summary>
        /// <returns></returns>
        private static string[] GetPendingJobsFiles()
        {
            try
            {
                if (!Directory.Exists(PendingJobsDir))
                {
                    Directory.CreateDirectory(PendingJobsDir);
                }
                string[] files = Directory.GetFiles(PendingJobsDir, "*." + RepetierExtension);

                return files;
            }
            catch (Exception e)
            {
                throw new PendingPrintJobsException("Can't read or create pending jobs directory.", e);
            }
        }

        public static PendingPrintJob GetPendingJobWithName(string snapshotName)
        {
            try
            {
                if (!Directory.Exists(PendingJobsDir))
                {
                    Directory.CreateDirectory(PendingJobsDir);
                }

                string pendingJobFilePath = PendingJobsDir + Path.DirectorySeparatorChar + snapshotName + "." + RepetierExtension;
                if (File.Exists(pendingJobFilePath))
                {
                    return new PendingPrintJob(pendingJobFilePath);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new PendingPrintJobsException("Can't read or create pending jobs directory.", e);
            }
        }

        public static string PendingJobsDir
        {
            get { return Main.globalSettings.Workdir + Path.DirectorySeparatorChar + PendingJobsDirName; }
        }
    }

    /// <summary>
    /// This classs represents a pending job.
    /// </summary>
    public class PendingPrintJob
    {
        private string path;
        private PrintingStateSnapshot snapshot;

        public PendingPrintJob(string path)
        {
            this.path = path;
        }

        public PendingPrintJob(PrintingStateSnapshot snapshot, string path)
        {
            this.path = path;
            this.snapshot = snapshot;
        }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(path); }
        }

        public PrintingStateSnapshot GetSnapshot()
        {
            if (snapshot == null)
            {
                // Lazy loading
                snapshot = PrintingStateSnapshotSerialization.LoadSnapshotFile(path);
            }
            return snapshot;
        }

        public void Save()
        {
            PrintingStateSnapshotSerialization.SaveSnapshotFile(snapshot, path);
        }

        public void Delete()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void Rename(string newName)
        {
            if (IsInvalidSnapshotName(newName))
            {
                throw new IOException("Invalid job name");
            }
            string oldPath = this.path;
            string newPath = Path.GetDirectoryName(oldPath) + Path.DirectorySeparatorChar + newName + "." + PendingPrintJobs.RepetierExtension;
            File.Move(oldPath, newPath);
            this.path = newPath;
        }

        /// <summary>
        /// Returns true if and only if the snapshot name is valid.
        /// This method doesn't accept null values.
        /// </summary>
        /// <param name="snapshotName"></param>
        /// <returns></returns>
        public static bool IsInvalidSnapshotName(string snapshotName)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                if (snapshotName.Contains(invalidChar))
                {
                    return false;
                }
            }
            return snapshotName.StartsWith(" ") || snapshotName.EndsWith(" ") || snapshotName.Length == 0 || snapshotName.Length >= 128;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// This class provides methods to serialize a pending job.
    /// </summary>
    public class PrintingStateSnapshotSerialization
    {
        public const string SnapshotTypeStateAndRemainingGCode = "state-and-remaining-gcode";
        public const string ContainerVersion = "1.0";


        public static PrintingStateSnapshot LoadSnapshotFile(string path)
        {
            Stream stream = File.OpenRead(path);
            try
            {
                return LoadSnapshotFile(stream);
            }
            finally
            {
                stream.Close();
            }
        }
        public static PrintingStateSnapshot LoadSnapshotFile(Stream stream)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(SnapshotContainer));
            try
            {
                SnapshotContainer container = (SnapshotContainer)x.Deserialize(stream);
                ValidateSnapshot(container);
                return container.snapshot;
            }
            catch (InvalidOperationException e)
            {
                throw new IOException("Invalid state file.", e);
            }
        }

        private static void ValidateSnapshot(SnapshotContainer container)
        {
            if (container.version == null || !container.version.Equals(ContainerVersion))
            {
                throw new IOException("Invalid state file. Unsupported version number.");
            }
            else if (!container.type.Equals(SnapshotTypeStateAndRemainingGCode))
            {
                throw new IOException("Invalid state file. Unknown type.");
            }
        }


        public static void SaveSnapshotFile(PrintingStateSnapshot state, string path)
        {
            Stream stream = File.OpenWrite(path);
            try
            {
                SaveSnapshotFile(state, stream);
            }
            finally
            {
                stream.Close();
            }
        }
        public static void SaveSnapshotFile(PrintingStateSnapshot state, Stream fileStream)
        {
            SnapshotContainer container = new SnapshotContainer();

            container.type = SnapshotTypeStateAndRemainingGCode;
            container.version = ContainerVersion;
            container.snapshot = state;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(container.GetType());
            try
            {
                x.Serialize(fileStream, container);
            }
            catch (InvalidOperationException ex)
            {
                throw new IOException("Failed to write state file.", ex);
            }
        }

        public class SnapshotContainer
        {
            public String version;
            public String type;
            public PrintingStateSnapshot snapshot;
        }
    }

}
