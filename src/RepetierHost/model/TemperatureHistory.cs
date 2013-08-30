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
using System.Drawing;
using RepetierHost.view.utils;

namespace RepetierHost.model
{
    public class TemperatureEntry
    {
        public long time;
        // values <-1000 are not present 
        public float output;
        public float extruder;
        public float bed;
        public float avgExtruder;
        public float avgBed;
        public float avgOutput;
        public float targetBed;
        public float targetExtruder;

        public TemperatureEntry(float ext, float _bed, float tar, float tare)
        {
            time = DateTime.UtcNow.Ticks;
            output = avgExtruder = avgBed = avgOutput = -10000;
            bed = _bed;
            extruder = ext;
            targetBed = tar;
            targetExtruder = tare;
        }
        public TemperatureEntry(int mon, float tmp, float outp, float tar, float tare)
        {
            time = DateTime.UtcNow.Ticks;
            output = outp;
            avgExtruder = bed = extruder = avgBed = avgOutput = -10000;
            targetBed = tar;
            targetExtruder = tare;
            switch (mon)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    extruder = tmp;
                    break;
                case 100:
                    bed = tmp;
                    break;
            }
        }
    }
    public class TemperatureList
    {
        public double minTime;
        public double maxTime;
        public LinkedList<TemperatureEntry> entries;
        public TemperatureList()
        {
            entries = new LinkedList<TemperatureEntry>();
        }
    }

    public class TemperatureHistory
    {
        public string name;
        public Color backgroundColor;
        public Color gridColor;
        public Color axisColor;
        public Color fontColor;
        public Color extruderColor;
        public Color avgExtruderColor;
        public Color bedColor;
        public Color avgBedColor;
        public Color targetExtruderColor;
        public Color targetBedColor;
        public Color outputColor;
        public Color avgOutputColor;
        private bool showOutput;
        private bool showBed;
        private bool showExtruder;
        private bool showTarget;
        private bool showAverage;
        public double xpos;
        private bool autoscoll;
        public TemperatureList history;
        public TemperatureList hourHistory;
        public LinkedList<TemperatureList> lists;
        public TemperatureList currentHistory;
        public int currentPos=0;
        public long currentHour;
        public int avgPeriod;
        public int currentZoomLevel;
        public double extruderWidth;
        public double avgExtruderWidth;
        public double targetExtruderWidth;
        public double bedWidth;
        public double avgBedWidth;
        public double targetBedWidth;
        public double avgOutputWidth;

        public TemperatureHistory()
        {
            Main.conn.eventTempHistory += addNotify;
            history = new TemperatureList();
            history.maxTime = DateTime.UtcNow.Ticks;
            history.minTime = history.maxTime - 36000000000;
            hourHistory = null;
            lists = new LinkedList<TemperatureList>();
            currentHour = -1;
            currentHistory = history;
            setupColor();
            /* NSUserDefaults *d = NSUserDefaults.standardUserDefaults;
             NSArray *arr = [NSArray arrayWithObjects:@"tempBackgroundColor",
                             @"tempGridColor",@"tempAxisColor",@"tempFontColor",  
                             @"tempExtruderColor",@"tempAvgExtruderColor",
                             @"tempBedColor",@"tempAvgBedColor",@"tempTargetExtruderColor",@"tempTargetBedColor",
                             @"tempOutputColor",@"tempAvgOutputColor",
                             @"tempShowExtruder",@"tempShowAverage",@"tempShowBed",@"tempAutoscroll",
                             @"tempShowOutput",@"tempShowTarget",@"tempZoomLevel",
                             @"tempAverageSeconds",@"tempExtruderWidth",@"tempAvgExtruderWidth",
                             @"tempTargetExtruderWidth",@"tempBedWidth",@"tempAvgBedWidth",
                             @"tempTargetBedWidth",@"tempAvgOutputWidth",
                             nil];
             bindingsArray = arr.retain;
             zoomLevel = [[NSArray arrayWithObjects:[NSNumber numberWithDouble:3600],
                           [NSNumber numberWithDouble:1800],
                           [NSNumber numberWithDouble:900],[NSNumber numberWithDouble:300],
                           [NSNumber numberWithDouble:100],[NSNumber numberWithDouble:60],
                           nil] retain];
             */
            xpos = 100;
            // for(NSString *key in arr)
            //     [d addObserver:self forKeyPath:key options:NSKeyValueObservingOptionNew context:NULL];
            ToolStripMenuItem item = new ToolStripMenuItem(Trans.T("L_PAST_60_MINUTES"),null,Main.main.selectTimePeriod);
            item.Tag = 0;
            lists.AddLast(history);
            Main.main.timeperiodMenuItem.DropDownItems.Add(item);
            CurrentPos = 0;
        }
        public int CurrentPos
        {
            get { return currentPos; }
            set
            {
                currentPos = value;
                currentHistory = lists.ElementAt(currentPos);
                foreach (ToolStripMenuItem mi in Main.main.timeperiodMenuItem.DropDownItems)
                {
                    mi.Checked = ((int)mi.Tag == currentPos);
                }
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public int CurrentZoomLevel
        {
            get { return currentZoomLevel; }
            set
            {
                currentZoomLevel = value; RegMemory.SetInt("tempZoomLevel", currentZoomLevel);
                foreach (ToolStripMenuItem mi in Main.main.temperatureZoomMenuItem.DropDownItems)
                {
                    mi.Checked = (mi.Tag.ToString() == currentZoomLevel.ToString());
                }
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public int AvgPeriod
        {
            get { return avgPeriod; }
            set
            {
                avgPeriod = value; RegMemory.SetInt("tempAverageSeconds", avgPeriod);
                foreach (ToolStripMenuItem mi in Main.main.buildAverageOverMenuItem.DropDownItems)
                {
                    mi.Checked = (mi.Tag.ToString() == avgPeriod.ToString());
                }
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public bool ShowExtruder
        {
            get { return showExtruder; }
            set { showExtruder = value; Main.main.showExtruderTemperaturesMenuItem.Checked = value;
            RegMemory.SetBool("tempShowExtruder", value);
            if (Main.main.tabControlView.SelectedIndex == 1)
                Main.main.tabPageTemp.Refresh();
            }
        }
        public bool ShowBed
        {
            get { return showBed; }
            set
            {
                showBed = value; Main.main.showHeatedBedTemperaturesMenuItem.Checked = value;
                RegMemory.SetBool("tempShowBed", value);
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public bool ShowAverage
        {
            get { return showAverage; }
            set
            {
                showAverage = value; Main.main.showAverageTemperaturesMenuItem.Checked = value;
                RegMemory.SetBool("tempShowAverage", value);
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public bool Autoscroll
        {
            get { return autoscoll; }
            set
            {
                autoscoll = value; Main.main.autoscrollTemperatureViewMenuItem.Checked = value;
                RegMemory.SetBool("tempAutoscroll", value);
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public bool ShowOutput
        {
            get { return showOutput; }
            set
            {
                showOutput = value; Main.main.showHeaterPowerMenuItem.Checked = value;
                RegMemory.SetBool("tempShowOutput", value);
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public bool ShowTarget
        {
            get { return showTarget; }
            set
            {
                showTarget = value; Main.main.showTargetTemperaturesMenuItem.Checked = value;
                RegMemory.SetBool("tempShowTarget", value);
                if (Main.main.tabControlView.SelectedIndex == 1)
                    Main.main.tabPageTemp.Refresh();
            }
        }
        public void setupColor()
        {
            CurrentZoomLevel = RegMemory.GetInt("tempZoomLevel", 15);
            AvgPeriod = RegMemory.GetInt("tempAverageSeconds", 120);
            ShowExtruder = RegMemory.GetBool("tempShowExtruder", true);
            ShowAverage = RegMemory.GetBool("tempShowAverage", true);
            ShowBed = RegMemory.GetBool("tempShowBed", true);
            Autoscroll = RegMemory.GetBool("tempAutoscroll", true);
            ShowOutput = RegMemory.GetBool("tempShowOutput", true);
            ShowTarget = RegMemory.GetBool(@"tempShowTarget", true);
            extruderWidth = RegMemory.GetDouble("tempExtruderWidth", 2);
            avgExtruderWidth = RegMemory.GetDouble("tempAvgExtruderWidth", 7);
            targetExtruderWidth = RegMemory.GetDouble("tempTargetExtruderWidth", 1);
            bedWidth = RegMemory.GetDouble("tempBedWidth", 2);
            avgBedWidth = RegMemory.GetDouble("tempAvgBedWidth", 7);
            targetBedWidth = RegMemory.GetDouble("tempTargetBedWidth", 1);
            avgOutputWidth = RegMemory.GetDouble("tempAvgOutputWidth", 2);
            backgroundColor = RegMemory.GetColor("tempBackgroundColor", Color.Black);
            gridColor = RegMemory.GetColor("tempGridColor", Color.FromArgb(255, 0, 70, 0));
            axisColor = RegMemory.GetColor("tempAxisColor", Color.FromArgb(255, 255, 255, 255));
            fontColor = RegMemory.GetColor("tempFontColor", Color.FromArgb(255, 229, 229, 229));
            extruderColor = RegMemory.GetColor("tempExtruderColor", Color.FromArgb(255, 255, 2, 61));
            avgExtruderColor = RegMemory.GetColor("tempAvgExtruderColor", Color.FromArgb(178, 222, 114, 0));
            bedColor = RegMemory.GetColor("tempBedColor", Color.FromArgb(255, 0, 208, 210));
            avgBedColor = RegMemory.GetColor("tempAvgBedColor", Color.FromArgb(178, 0, 122, 165));
            targetExtruderColor = RegMemory.GetColor("tempTargetExtruderColor", Color.FromArgb(255, 153, 102, 204));
            targetBedColor = RegMemory.GetColor("tempTargetBedColor", Color.FromArgb(255, 153, 102, 204));
            outputColor = RegMemory.GetColor("tempOutputColor", Color.FromArgb(178, 13, 128, 15));
            avgOutputColor = RegMemory.GetColor("tempAvgOutputColor", Color.FromArgb(255, 0, 128, 255));
        }
        public void addNotify(TemperatureEntry ent)
        {
            history.entries.AddLast(ent);
            // Remove old entries
            long time = DateTime.UtcNow.Ticks;
            long ltime = (long)time;
            long lhour = ltime / 36000000000;
            double mintime = time - 36000000000;
            while (history.entries.First.Value.time < mintime)
                history.entries.RemoveFirst();
            // Create average values
            int nExtruder = 0, nBed = 0, nOut = 0;
            float sumExtruder = 0, sumBed = 0, sumOutput = 0;
            mintime = DateTime.UtcNow.Ticks - avgPeriod * 10000000;
            foreach (TemperatureEntry e in history.entries)
            {
                if (e.time < mintime) continue;
                if (e.extruder > -1000)
                {
                    nExtruder++;
                    sumExtruder += e.extruder;
                }
                if (e.bed > -1000)
                {
                    nBed++;
                    sumBed += e.bed;
                }
                if (e.output > -1000)
                {
                    nOut++;
                    sumOutput += e.output;
                }
            }
            if (nExtruder > 0)
                ent.avgExtruder = sumExtruder / (float)nExtruder;
            if (nBed > 0)
                ent.avgBed = sumBed / (float)nBed;
            if (nOut > 0)
                ent.avgOutput = sumOutput / (float)nOut;
            history.maxTime = time;
            history.minTime = time - 36000000000;

            if (lhour != currentHour)
            {
                currentHour = lhour;
                DateTime now = DateTime.Now;
                string stime = now.ToString("MMMM dd, HH");
                ToolStripMenuItem item = new ToolStripMenuItem(stime,null, Main.main.selectTimePeriod);
                item.Tag = lists.Count;
                hourHistory = new TemperatureList();
                hourHistory.minTime = lhour * 36000000000;
                hourHistory.maxTime = hourHistory.minTime + 36000000000;
                lists.AddLast(hourHistory);
                Main.main.timeperiodMenuItem.DropDownItems.Add(item);
            }
            hourHistory.entries.AddLast(ent);
            if (Main.main.tabControlView.SelectedIndex == 1)
                Main.main.tabPageTemp.Refresh();
        }
    }
}
