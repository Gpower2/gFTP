using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using gFtp;
using gpower2.gControls;
using Newtonsoft.Json;

namespace gFtpGUI
{
    public partial class frmQueue : Form
    {
        public delegate void UpdateProgressDelegate(Object val);

        public frmQueue()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            Task.Run(() => LoadQueueAsync()).Wait();
                
            grdQueue.MultiSelect = true;
        }

        private async Task LoadQueueAsync()
        {
            try
            {
                string queueFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gFtpQueue.json");
                if (File.Exists(queueFile))
                {
                    using (StreamReader sr = new StreamReader(queueFile))
                    {
                        var jsonQueue = JsonConvert.DeserializeObject<IList<QueueItem>>(await sr.ReadToEndAsync());

                        grdQueue.DataSource = jsonQueue;
                    }
                }
                else
                {
                    grdQueue.DataSource = new List<QueueItem>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grdQueue.DataSource = new List<QueueItem>();
            }
        }

        private async Task SaveQueueAsync()
        {
            try
            {
                string queueFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gFtpQueue.json");
                using (StreamWriter sw = new StreamWriter(queueFile, false))
                {
                    await sw.WriteAsync(JsonConvert.SerializeObject(grdQueue.DataSource));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task AddJobsAsync(IList<Job> argJobs)
        {
            grdQueue.SuspendLayout();
            foreach (Job job in argJobs)
            {
                QueueItem q = new gFtpGUI.QueueItem();
                if (!(grdQueue.DataSource as IList<QueueItem>).Any())
                {
                    q.Order = 1;
                }
                else
                {
                    q.Order = (grdQueue.DataSource as IList<QueueItem>).Max(t => t.Order) + 1;
                }
                q.Job = job;
                q.Size = job.Size;
                q.State = JobState.Pending;

                (grdQueue.DataSource as IList<QueueItem>).Add(q);
            }
            grdQueue.ClearSelection();
            grdQueue.ResumeLayout();            
            grdQueue.Refresh();

            await SaveQueueAsync();
        }

        public async Task AddJobAsync(Job argJob)
        {
            QueueItem q = new gFtpGUI.QueueItem();
            if(!(grdQueue.DataSource as IList<QueueItem>).Any())
            {
                q.Order = 1;
            }
            else
            {
                q.Order = (grdQueue.DataSource as IList<QueueItem>).Max(t => t.Order) + 1;
            }
            q.Job = argJob;
            q.Size = argJob.Size;
            q.State = JobState.Pending;

            (grdQueue.DataSource as IList<QueueItem>).Add(q);
            grdQueue.ClearSelection();
            grdQueue.Refresh();

            await SaveQueueAsync();
        }

        private void ClearProgress()
        {
            prgBrStatus.Value = 0;
            lblStatus.Text = "";
            txtAriaData.Clear();
            gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);
        }

        private async void btnRunJobs_Click(object sender, EventArgs e)
        {
            try
            {
                btnStopJobs.Enabled = true;
                btnRunJobs.Enabled = false;

                AriaHelper.AriaProgressUpdated += AriaHelper_AriaProgressUpdated;
                int exitCode = 0;

                DateTime previousTime = DateTime.Now;

                while ((grdQueue.DataSource as IList<QueueItem>).Any(t => t.State == JobState.Pending))
                {
                    QueueItem q = (grdQueue.DataSource as IList<QueueItem>).Where(t => t.State == JobState.Pending).OrderBy(t => t.Order).FirstOrDefault();
                    Job j = q.Job;
                    gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Indeterminate);
                    prgBrStatus.Style = ProgressBarStyle.Marquee;

                    exitCode = 0;

                    await Task.Run(() =>
                        {
                            using (Process myProcess = AriaHelper.GetAriaProcess(j.AriaPath, j.FtpPath, j.FtpFilename, j.LocalPath, j.LocalFilename, j.Ftp))
                            {
                                q.State = JobState.Running;
                                q.StartTime = DateTime.Now;

                                myProcess.Start();

                                // Read the Standard output character by character
                                AriaHelper.ReadStreamPerCharacter(myProcess);

                                while (myProcess != null && !myProcess.HasExited)
                                {
                                    Application.DoEvents();
                                }
                                if (myProcess != null)
                                {
                                    exitCode = myProcess.ExitCode;
                                }
                                else
                                {
                                    exitCode = -1;
                                }
                            }
                        }
                    );

                    //while (!task.IsCompleted)
                    //{
                    //    if ((DateTime.Now - previousTime).TotalMilliseconds > 50)
                    //    {
                    //        grdQueue.Refresh();
                    //        previousTime = DateTime.Now;
                    //    }
                    //    Application.DoEvents();
                    //}

                    q.EndTime = DateTime.Now;
                    q.State = (exitCode == 0) ? JobState.Completed : JobState.Failed;
                    ClearProgress();
                    grdQueue.Refresh();

                    await SaveQueueAsync();
                    Application.DoEvents();
                }

                AriaHelper.AriaProgressUpdated -= AriaHelper_AriaProgressUpdated;

                btnStopJobs.Enabled = false;
                btnRunJobs.Enabled = true;
                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                AriaHelper.AriaProgressUpdated -= AriaHelper_AriaProgressUpdated;
                btnStopJobs.Enabled = false;
                btnRunJobs.Enabled = true;
                ClearProgress();
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateProgress(Object argData)
        {
            AriaData data = (AriaData)argData;
            if (data.Progress > 0)
            {
                prgBrStatus.Style = ProgressBarStyle.Continuous;
                prgBrStatus.Value = data.Progress;
            }
            lblStatus.Text = String.Format("{0}%", data.Progress);
            FileSize totalCompleted = new FileSize((grdQueue.DataSource as IList<QueueItem>).Where(q => q.State == JobState.Completed).Sum(q => q.Size.Size));
            FileSize total = new FileSize((grdQueue.DataSource as IList<QueueItem>).Where(q => q.State != JobState.Failed).Sum(q => q.Size.Size));
            FileSize totalRemaining = total - totalCompleted;
            txtAriaData.Text = String.Format("Downloaded: {0} | Speed: {1} | ETA: {2} | Total {3} / {4} | Remaining {5}", 
                data.Downloaded, data.Speed, data.ETA, totalCompleted, total, totalRemaining);
            if(data.Progress > 0)
            { 
                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Normal);
                gTaskbarProgress.SetValue(this, Convert.ToUInt64(data.Progress), (UInt64)100);
            }
            grdQueue.Refresh();
            Application.DoEvents();
        }

        private void AriaHelper_AriaProgressUpdated(AriaData argData)
        {
            this.Invoke(new UpdateProgressDelegate(UpdateProgress), new object[] { argData });
        }

        private async void btnStopJobs_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (QueueItem q in grdQueue.DataSource as IList<QueueItem>)
                {
                    if(q.State == JobState.Pending)
                    {
                        q.State = JobState.Waiting;
                        q.EndTime = null;
                    }
                    else if (q.State == JobState.Running)
                    {
                        q.State = JobState.Failed;
                        q.EndTime = null;
                    }
                }
                Process[] pr = Process.GetProcessesByName("aria2c");
                while (pr.Any())
                {
                    try
                    {
                        if (!pr[0].HasExited)
                        {
                            pr[0].Kill();
                            pr = Process.GetProcessesByName("aria2c");
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                ClearProgress();
                grdQueue.Refresh();
                await SaveQueueAsync();
                btnStopJobs.Enabled = false;
                btnRunJobs.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                btnStopJobs.Enabled = false;
                btnRunJobs.Enabled = true;
                ClearProgress();
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdQueue.SelectedItems.Count > 0)
                {
                    foreach (var item in grdQueue.SelectedItems)
                    {
                        (grdQueue.DataSource as IList<QueueItem>).Remove(item as QueueItem);
                    }
                    grdQueue.Refresh();
                    await SaveQueueAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmQueue_FormClosing(object sender, FormClosingEventArgs e)
        {
            // To avoid getting disposed
            e.Cancel = true;
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if(grdQueue.SelectedItem == null)
                {
                    return;
                }
                if (Directory.Exists((grdQueue.SelectedItem as QueueItem).Job.LocalPath))
                {
                    if (File.Exists(Path.Combine((grdQueue.SelectedItem as QueueItem).Job.LocalPath, (grdQueue.SelectedItem as QueueItem).Job.LocalFilename)))
                    {
                        Process.Start("explorer.exe", String.Format("/select, \"{0}\"", Path.Combine((grdQueue.SelectedItem as QueueItem).Job.LocalPath, (grdQueue.SelectedItem as QueueItem).Job.LocalFilename)));
                    }
                    else
                    {
                        Process.Start("explorer.exe", (grdQueue.SelectedItem as QueueItem).Job.LocalPath);
                    }                        
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnChangeState_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdQueue.SelectedItems.Count == 0)
                {
                    return;
                }
                foreach (QueueItem item in grdQueue.SelectedItems)
                {
                    if (item.State == JobState.Failed)
                    {
                        item.State = JobState.Pending;
                        item.StartTime = null;
                        item.EndTime = null;
                    }
                    else if (item.State == JobState.Completed)
                    {
                        item.State = JobState.Pending;
                        item.StartTime = null;
                        item.EndTime = null;
                    }
                    else if (item.State == JobState.Pending)
                    {
                        item.State = JobState.Waiting;
                        item.StartTime = null;
                        item.EndTime = null;
                    }
                    else if (item.State == JobState.Waiting)
                    {
                        item.State = JobState.Pending;
                        item.StartTime = null;
                        item.EndTime = null;
                    }
                }
                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRemoveCompleted_Click(object sender, EventArgs e)
        {
            try
            {
                while ((grdQueue.DataSource as IList<QueueItem>).Any(q => q.State == JobState.Completed))
                {
                    (grdQueue.DataSource as IList<QueueItem>).Remove((grdQueue.DataSource as IList<QueueItem>).FirstOrDefault(q => q.State == JobState.Completed));
                }
                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                (grdQueue.DataSource as IList<QueueItem>).Clear();
                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdQueue.SelectedItems.Count == 0)
                {
                    return;
                }
                IList<QueueItem> list = (grdQueue.DataSource as IList<QueueItem>);
                IList<QueueItem> selection = (grdQueue.SelectedItems as IList).Cast<QueueItem>().OrderBy(t => t.Order).ToList();
                foreach (QueueItem item in selection)
                {
                    // Check if there is an item which has a smaller Order
                    if (list.Any(t2 => t2.Order < item.Order))
                    {
                        QueueItem q = list.Where(t => t.Order == list.Where(t2 => t2.Order < item.Order).Max(t2 => t2.Order)).FirstOrDefault();
                        if (q != null)
                        {
                            Int32 itemOrder = item.Order;
                            item.Order = q.Order;
                            q.Order = itemOrder;
                        }
                    }
                }

                Int32 rowIndex = grdQueue.FirstDisplayedScrollingRowIndex;

                grdQueue.SuspendDrawing();

                grdQueue.DataSource = list.OrderBy(t => t.Order).ToList();

                //grdQueue.ClearSelection();
                foreach (DataGridViewRow row in grdQueue.Rows)
                {
                    if(selection.Any(t => t.Order == (row.DataBoundItem as QueueItem).Order))
                    {
                        row.Selected = true;
                    }
                }

                grdQueue.FirstDisplayedScrollingRowIndex = rowIndex;

                grdQueue.ResumeDrawing();

                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdQueue.SelectedItems.Count == 0)
                {
                    return;
                }

                IList<QueueItem> list = (grdQueue.DataSource as IList<QueueItem>);
                IList<QueueItem> selection = (grdQueue.SelectedItems as IList).Cast<QueueItem>().OrderByDescending(t => t.Order).ToList();
                foreach (QueueItem item in selection)
                {
                    if (list.Any(t2 => t2.Order > item.Order))
                    {
                        // Check if there is an item which has a greater Order
                        QueueItem q = list.Where(t => t.Order == list.Where(t2 => t2.Order > item.Order).Min(t2 => t2.Order)).FirstOrDefault();
                        if (q != null)
                        {
                            Int32 itemOrder = item.Order;
                            item.Order = q.Order;
                            q.Order = itemOrder;
                        }
                    }
                }

                Int32 rowIndex = grdQueue.FirstDisplayedScrollingRowIndex;
                
                grdQueue.SuspendDrawing();

                grdQueue.DataSource = list.OrderBy(t => t.Order).ToList();

                //grdQueue.ClearSelection();
                foreach (DataGridViewRow row in grdQueue.Rows)
                {
                    if (selection.Any(t => t.Order == (row.DataBoundItem as QueueItem).Order))
                    {
                        row.Selected = true;
                    }
                }

                grdQueue.FirstDisplayedScrollingRowIndex = rowIndex;

                grdQueue.ResumeDrawing();

                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grdQueue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                OpenFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenFile()
        {
            if (grdQueue.SelectedIndex == -1)
            {
                return;
            }
            if (grdQueue.LastClickedRowIndex == -1)
            {
                return;
            }
            QueueItem q = grdQueue.SelectedItem as QueueItem;
            if (File.Exists(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename)))
            {
                Task.Run(() => Process.Start(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename)));
            }
        }

        private async void btnRemoveDeleted_Click(object sender, EventArgs e)
        {
            try
            {
                while ((grdQueue.DataSource as IList<QueueItem>).Any(q => !File.Exists(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename))))
                {
                    (grdQueue.DataSource as IList<QueueItem>).Remove(
                        (grdQueue.DataSource as IList<QueueItem>).FirstOrDefault(q => !File.Exists(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename)))
                    );
                }
                grdQueue.Refresh();
                await SaveQueueAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
