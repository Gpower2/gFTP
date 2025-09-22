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

#pragma warning disable IDE1006 // Naming Styles
namespace gFtpGUI
{
    public partial class frmQueue : Form
    {
        private bool _isDarkModeEnabled = false; // Field to store current theme state

        // Theme Colors copied from frmMain.cs
        private static readonly Color DarkModeBackColor = Color.FromArgb(32, 32, 32);
        private static readonly Color DarkModeForeColor = Color.White;
        private static readonly Color DarkModeTextBoxBack = Color.FromArgb(64, 64, 64);
        private static readonly Color DarkModeButtonBack = Color.FromArgb(80, 80, 80);
        private static readonly Color LightModeBackColor = SystemColors.Control;
        private static readonly Color LightModeForeColor = SystemColors.ControlText;
        private static readonly Color LightModeTextBoxBack = SystemColors.Window;
        private static readonly Color LightModeButtonBack = SystemColors.Control;

        public delegate void UpdateProgressDelegate(Object val);

        public frmQueue()
        {
            InitializeComponent();
            // this.UseImmersiveDarkMode(true); // Theme will be applied by frmMain

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            LoadQueue();

            grdQueue.MultiSelect = true;
            grdQueue.DataSourceChanged += grdQueue_DataSourceChanged;
            this.grdQueue.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdQueue_ColumnHeaderMouseClick);
        }

        private void grdQueue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // _isDarkModeEnabled field should exist from previous steps
            if (this._isDarkModeEnabled)
            {
                ApplyThemeToDataGridView(grdQueue, true);
            }
        }

        private void grdQueue_DataSourceChanged(object sender, EventArgs e)
        {
            // Check if the control is visible and its handle has been created to avoid errors during initialization.
            if (grdQueue.Visible && grdQueue.IsHandleCreated && grdQueue.DataSource != null)
            {
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled);
            }
        }

        private void LoadQueue()
        {
            try
            {
                string queueFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gFtpQueue.json");
                if (File.Exists(queueFile))
                {
                    using (StreamReader sr = new StreamReader(queueFile))
                    {
                        var jsonQueue = JsonConvert.DeserializeObject<IList<QueueItem>>(sr.ReadToEnd());

                        grdQueue.DataSource = jsonQueue;
                        ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
                    }
                }
                else
                {
                    grdQueue.DataSource = new List<QueueItem>();
                    ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
                }

                UpdateSizeInfo();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grdQueue.DataSource = new List<QueueItem>();
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
            }
        }

        private void UpdateSizeInfo()
        {
            // Calculate the Size Info
            IList<QueueItem> queue = grdQueue.DataSource as IList<QueueItem>;

            var totalSize = queue.Sum(j => j.Size.Size);
            var completedSize = queue.Where(j => j.State == JobState.Completed).Sum(j => j.Size.Size);
            var failedSize = queue.Where(j => j.State == JobState.Failed).Sum(j => j.Size.Size);
            var remainingdSize = queue.Where(j => j.State == JobState.Running || j.State == JobState.Pending || j.State == JobState.Ready).Sum(j => j.Size.Size);
            var waitingSize = queue.Where(j => j.State == JobState.Waiting).Sum(j => j.Size.Size);

            string info = $"Total: {new FileSize(completedSize)} / {new FileSize(totalSize)} | Pending: {new FileSize(remainingdSize)} | Failed: {new FileSize(failedSize)} | Paused: {new FileSize(waitingSize)}";

            txtSizeInfo.Text = info;
        }

        private async Task SaveQueueAsync()
        {
            try
            {
                UpdateSizeInfo();

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
            foreach (Job job in argJobs.OrderBy(j => j.FtpFilename))
            {
                QueueItem q = new gFtpGUI.QueueItem();
                var queue = (grdQueue.DataSource as IList<QueueItem>);

                // Check if the job is already added to the queue
                if (queue.Any(i =>
                        i.Job.FtpFilename.ToLower().Equals(job.FtpFilename.ToLower())
                        && i.Job.FtpPath.ToLower().Equals(job.FtpPath.ToLower())
                        && i.Job.LocalFilename.ToLower().Equals(job.LocalFilename.ToLower())
                        && i.Job.LocalPath.ToLower().Equals(job.LocalPath.ToLower())
                    )
                )
                {
                    MessageBox.Show($"The job {job} is already added to the queue!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                if (!queue.Any())
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

                queue.Add(q);
            }
            grdQueue.ClearSelection();
            grdQueue.ResumeLayout();
            grdQueue.Refresh();

            await SaveQueueAsync();
        }

        public async Task AddJobAsync(Job argJob)
        {
            QueueItem q = new gFtpGUI.QueueItem();
            var queue = (grdQueue.DataSource as IList<QueueItem>);

            // Check if the job is already added to the queue
            if (queue.Any(i =>
                    i.Job.FtpFilename.ToLower().Equals(argJob.FtpFilename.ToLower())
                    && i.Job.FtpPath.ToLower().Equals(argJob.FtpPath.ToLower())
                    && i.Job.LocalFilename.ToLower().Equals(argJob.LocalFilename.ToLower())
                    && i.Job.LocalPath.ToLower().Equals(argJob.LocalPath.ToLower())
                )
            )
            {
                MessageBox.Show($"The job {argJob} is already added to the queue!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!queue.Any())
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

            queue.Add(q);
            grdQueue.ClearSelection();
            ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
            grdQueue.Refresh();

            await SaveQueueAsync();
        }

        private void ClearProgress()
        {
            prgBrStatus.Style = ProgressBarStyle.Continuous;
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
                    var fullLocalPath = Path.Combine(j.LocalPath, j.LocalFilename);
                    if (fullLocalPath.Length > 250)
                    {
                        var charactersToRemove = fullLocalPath.Length - 250;

                        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(j.LocalFilename);

                        filenameWithoutExtension = filenameWithoutExtension.Remove(filenameWithoutExtension.Length - charactersToRemove - 1, charactersToRemove);

                        j.LocalFilename = $"{filenameWithoutExtension}{Path.GetExtension(j.LocalFilename)}";
                    }

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
            if (data.Progress > 0)
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
                    if (q.State == JobState.Pending)
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
                if (grdQueue.SelectedItems.Count == 0)
                {
                    return;
                }

                var answer = MessageBox.Show("Are you sure you want to remove the selected items?", "Are you sure?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (answer == DialogResult.Cancel)
                {
                    return;
                }

                foreach (var item in grdQueue.SelectedItems)
                {
                    (grdQueue.DataSource as IList<QueueItem>).Remove(item as QueueItem);
                }
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
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

        private void frmQueue_FormClosing(object sender, FormClosingEventArgs e)
        {
            // To avoid getting disposed
            e.Cancel = true;
        }

        private async void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdQueue.SelectedItem == null)
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                if (await DirectoryAsync.ExistsAsync((grdQueue.SelectedItem as QueueItem).Job.LocalPath))
                {
                    if (await FileAsync.ExistsAsync(Path.Combine((grdQueue.SelectedItem as QueueItem).Job.LocalPath, (grdQueue.SelectedItem as QueueItem).Job.LocalFilename)))
                    {
                        await ProcessAsync.StartAsync(
                            "explorer.exe",
                            String.Format("/select, \"{0}\"", Path.Combine((grdQueue.SelectedItem as QueueItem).Job.LocalPath, (grdQueue.SelectedItem as QueueItem).Job.LocalFilename)));
                    }
                    else
                    {
                        await ProcessAsync.StartAsync("explorer.exe", (grdQueue.SelectedItem as QueueItem).Job.LocalPath);
                    }
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
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
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
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
                IList<QueueItem> items = grdQueue.DataSource as IList<QueueItem>;
                if (!items.Any(q => q.State == JobState.Completed))
                {
                    return;
                }

                var answer = MessageBox.Show("Are you sure you want to remove ALL completed items?", "Are you sure?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (answer == DialogResult.Cancel)
                {
                    return;
                }

                while (items.Any(q => q.State == JobState.Completed))
                {
                    items.Remove((grdQueue.DataSource as IList<QueueItem>).FirstOrDefault(q => q.State == JobState.Completed));
                }

                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
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
                var answer = MessageBox.Show("Are you sure you want to remove ALL items?", "Are you sure?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (answer == DialogResult.Cancel)
                {
                    return;
                }

                (grdQueue.DataSource as IList<QueueItem>).Clear();
                // DataSourceChanged might not fire on Clear(), so explicitly call ApplyThemeToDataGridView.
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled);
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
                    if (selection.Any(t => t.Order == (row.DataBoundItem as QueueItem).Order))
                    {
                        row.Selected = true;
                    }
                }

                grdQueue.FirstDisplayedScrollingRowIndex = rowIndex;

                grdQueue.ResumeDrawing();
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set

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
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set

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

        private async void grdQueue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                await OpenFileAsync();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                await OpenFileAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OpenFileAsync()
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

            this.Cursor = Cursors.WaitCursor;

            if (await FileAsync.ExistsAsync(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename)))
            {
                await ProcessAsync.StartAsync(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename));
            }

            this.Cursor = Cursors.Default;
        }

        private async void btnRemoveDeleted_Click(object sender, EventArgs e)
        {
            try
            {
                var queue = (grdQueue.DataSource as IList<QueueItem>);

                if (!queue.Any(q => q.State == JobState.Completed))
                {
                    return;
                }

                var answer = MessageBox.Show("Are you sure you want to remove ALL deleted items?", "Are you sure?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (answer == DialogResult.Cancel)
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                grdQueue.SuspendDrawing();

                long totalFilesRemoved = 0;
                while (queue.Any(q => q.State == JobState.Completed))
                {
                    bool removeOccured = false;
                    for (int i = 0; i < queue.Count; i++)
                    {
                        var q = queue[i];
                        if (!await FileAsync.ExistsAsync(Path.Combine(q.Job.LocalPath, q.Job.LocalFilename)))
                        {
                            queue.RemoveAt(i);
                            removeOccured = true;
                            totalFilesRemoved++;
                            // An elemet was removed, restart the check from the beginning
                            break;
                        }
                    }

                    // If no removal has occured, break the while
                    if (!removeOccured)
                    {
                        break;
                    }
                }

                grdQueue.ResumeDrawing();

                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled); // Explicit call after DataSource set
                grdQueue.Refresh();

                await SaveQueueAsync();
                this.Cursor = Cursors.Default;
                MessageBox.Show($"{totalFilesRemoved} deleted file(s) were removed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Debug.WriteLine(ex);
                grdQueue.Refresh();
                await SaveQueueAsync();
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Copied from frmMain.cs and adapted for frmQueue
        private void ApplyThemeToDataGridView(gpower2.gControls.gDataGridView gdv, bool darkModeEnabled)
        {
            if (darkModeEnabled)
            {
                gdv.EnableHeadersVisualStyles = false;
                gdv.BackgroundColor = DarkModeBackColor;
                gdv.GridColor = Color.FromArgb(100, 100, 100);

                gdv.DefaultCellStyle.BackColor = DarkModeTextBoxBack;
                gdv.DefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 130);
                gdv.DefaultCellStyle.SelectionForeColor = Color.White;

                gdv.AlternatingRowsDefaultCellStyle.BackColor = DarkModeBackColor;
                gdv.AlternatingRowsDefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);
                gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

                gdv.ColumnHeadersDefaultCellStyle.BackColor = DarkModeButtonBack;
                gdv.ColumnHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkModeButtonBack;

                gdv.RowHeadersDefaultCellStyle.BackColor = DarkModeBackColor;
                gdv.RowHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);

                if (gdv.ContextMenuStrip != null)
                {
                    gdv.ContextMenuStrip.BackColor = DarkModeButtonBack;
                    gdv.ContextMenuStrip.ForeColor = DarkModeForeColor;
                    foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                    {
                        item.BackColor = DarkModeButtonBack;
                        item.ForeColor = DarkModeForeColor;
                        if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                        {
                            menuItem.DropDown.BackColor = DarkModeButtonBack;
                            menuItem.DropDown.ForeColor = DarkModeForeColor;
                            foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                            {
                                dropDownItem.BackColor = DarkModeButtonBack;
                                dropDownItem.ForeColor = DarkModeForeColor;
                            }
                        }
                    }
                }
            }
            else // Light Mode
            {
                // General settings for light mode
                gdv.EnableHeadersVisualStyles = true;
                gdv.RowHeadersVisible = false; // As per grdQueue designer

                // Properties from grdQueue in designer
                gdv.BackgroundColor = System.Drawing.Color.White;
                gdv.GridColor = System.Drawing.Color.Gainsboro;
                gdv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

                // AlternatingRowsDefaultCellStyle (from dataGridViewCellStyle1)
                gdv.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
                gdv.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
                gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
                // Font, Alignment, WrapMode not set in dataGridViewCellStyle1

                // DefaultCellStyle (from dataGridViewCellStyle2)
                gdv.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
                gdv.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                gdv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                gdv.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                gdv.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
                gdv.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
                gdv.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

                // ColumnHeadersDefaultCellStyle (Standard light mode, consistent with frmMain)
                gdv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
                gdv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                gdv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
                gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Control;
                gdv.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.ControlText;
                // No specific Alignment or WrapMode typically set for column headers from designer

                // RowHeadersDefaultCellStyle (Standard light mode, consistent with frmMain, though not visible)
                gdv.RowHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
                gdv.RowHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                gdv.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
                gdv.RowHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
                gdv.RowHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
                // No specific Alignment or WrapMode typically set for row headers from designer

                // ContextMenuStrip styling (Standard light mode)
                if (gdv.ContextMenuStrip != null)
                {
                    gdv.ContextMenuStrip.BackColor = System.Drawing.SystemColors.ControlLightLight;
                    gdv.ContextMenuStrip.ForeColor = System.Drawing.SystemColors.ControlText;
                    foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                    {
                        item.BackColor = System.Drawing.SystemColors.ControlLightLight;
                        item.ForeColor = System.Drawing.SystemColors.ControlText;
                        if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                        {
                            menuItem.DropDown.BackColor = System.Drawing.SystemColors.ControlLightLight;
                            menuItem.DropDown.ForeColor = System.Drawing.SystemColors.ControlText;
                            foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                            {
                                dropDownItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
                                dropDownItem.ForeColor = System.Drawing.SystemColors.ControlText;
                            }
                        }
                    }
                }
            }
            gdv.Refresh(); // Refresh the grid after applying styles
        }

        private void ApplyThemeToControl(Control c, bool darkModeEnabled)
        {
            if (darkModeEnabled)
            {
                c.BackColor = DarkModeBackColor;
                c.ForeColor = DarkModeForeColor;

                // Generic styling for most controls
                c.ForeColor = DarkModeForeColor;
                if (!(c is SplitContainer)) // SplitContainer doesn't have a relevant BackColor
                {
                    c.BackColor = DarkModeBackColor;
                }

                if (c is TextBox || c is RichTextBox || c is ListBox || c is gpower2.gControls.gTextBox) // gTextBox for txtSizeInfo
                {
                    c.BackColor = DarkModeTextBoxBack;
                    if (c is TextBox tb) { tb.BorderStyle = BorderStyle.FixedSingle; }
                    else if (c is RichTextBox rtb) { rtb.BorderStyle = BorderStyle.FixedSingle; }
                    else if (c is ListBox lb) { lb.BorderStyle = BorderStyle.FixedSingle; }
                    else if (c is gpower2.gControls.gTextBox gtb) { gtb.BorderStyle = BorderStyle.FixedSingle; } // Assuming gTextBox has BorderStyle
                }
                else if (c is ComboBox cmb) // Should not be any in frmQueue based on Designer, but good to have
                {
                    cmb.BackColor = DarkModeTextBoxBack;
                    cmb.ForeColor = DarkModeForeColor;
                    cmb.FlatStyle = FlatStyle.Flat;
                    // If gComboBox is used here and has specific properties, handle them.
                }
                else if (c is Button btn)
                {
                    btn.BackColor = DarkModeButtonBack;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    btn.FlatAppearance.BorderSize = 1;
                }
                else if (c is gpower2.gControls.gDataGridView gdv) // Handles grdQueue
                {
                    gdv.EnableHeadersVisualStyles = false;
                    gdv.BackgroundColor = DarkModeBackColor;
                    gdv.GridColor = Color.FromArgb(100, 100, 100);

                    gdv.DefaultCellStyle.BackColor = DarkModeTextBoxBack;
                    gdv.DefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 130);
                    gdv.DefaultCellStyle.SelectionForeColor = Color.White;
                    // gdv.DefaultCellStyle.Font = new Font("Segoe UI", 9F); 

                    gdv.AlternatingRowsDefaultCellStyle.BackColor = DarkModeBackColor;
                    gdv.AlternatingRowsDefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);
                    gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

                    gdv.ColumnHeadersDefaultCellStyle.BackColor = DarkModeButtonBack;
                    gdv.ColumnHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkModeButtonBack;
                    // gdv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                    gdv.RowHeadersDefaultCellStyle.BackColor = DarkModeBackColor;
                    gdv.RowHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);
                    // gdv.RowHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F);

                    if (gdv.ContextMenuStrip != null)
                    {
                        gdv.ContextMenuStrip.BackColor = DarkModeButtonBack;
                        gdv.ContextMenuStrip.ForeColor = DarkModeForeColor;
                        foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                        {
                            item.BackColor = DarkModeButtonBack;
                            item.ForeColor = DarkModeForeColor;
                            if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                            {
                                menuItem.DropDown.BackColor = DarkModeButtonBack;
                                menuItem.DropDown.ForeColor = DarkModeForeColor;
                                foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                                {
                                    dropDownItem.BackColor = DarkModeButtonBack;
                                    dropDownItem.ForeColor = DarkModeForeColor;
                                }
                            }
                        }
                    }
                }
                else if (c is GroupBox gb)
                {
                    gb.ForeColor = DarkModeForeColor;
                }
                else if (c is ProgressBar pb) // Handle prgBrStatus
                {
                    pb.BackColor = DarkModeTextBoxBack;
                    pb.ForeColor = DarkModeButtonBack; // Try to set bar color
                }
                else if (c is Label lbl)
                {
                    if (lbl.Parent is GroupBox || lbl.Parent is Panel)
                    {
                        lbl.BackColor = Color.Transparent;
                    }
                    else
                    {
                        lbl.BackColor = DarkModeBackColor;
                    }
                }
                else if (c is Panel panel)
                {
                    panel.BackColor = DarkModeBackColor;
                }
            }
            else // Light Mode
            {
                c.ForeColor = LightModeForeColor;
                if (!(c is SplitContainer))
                {
                    c.BackColor = LightModeBackColor;
                }

                if (c is TextBox || c is RichTextBox || c is ListBox || c is gpower2.gControls.gTextBox)
                {
                    c.BackColor = LightModeTextBoxBack;
                    if (c is TextBox tb) { tb.BorderStyle = BorderStyle.Fixed3D; }
                    else if (c is RichTextBox rtb) { rtb.BorderStyle = BorderStyle.Fixed3D; }
                    else if (c is ListBox lb) { lb.BorderStyle = BorderStyle.FixedSingle; }
                    else if (c is gpower2.gControls.gTextBox gtb) { gtb.BorderStyle = BorderStyle.FixedSingle; }
                }
                else if (c is ComboBox cmb)
                {
                    cmb.BackColor = LightModeTextBoxBack;
                    cmb.ForeColor = LightModeForeColor;
                    cmb.FlatStyle = FlatStyle.Standard;
                }
                else if (c is Button btn)
                {
                    btn.BackColor = LightModeButtonBack;
                    btn.FlatStyle = FlatStyle.System;
                    btn.FlatAppearance.BorderColor = SystemColors.ControlDark;
                    btn.FlatAppearance.BorderSize = 1;
                }
                else if (c is gpower2.gControls.gDataGridView gdv)
                {
                    gdv.EnableHeadersVisualStyles = true;
                    gdv.BackgroundColor = SystemColors.AppWorkspace; // This is a generic fallback. ApplyThemeToDataGridView will override for grdQueue.
                    gdv.GridColor = SystemColors.ControlDark;

                    gdv.DefaultCellStyle.BackColor = SystemColors.Window;
                    gdv.DefaultCellStyle.ForeColor = SystemColors.ControlText;
                    gdv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    gdv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    gdv.DefaultCellStyle.Font = SystemFonts.DefaultFont;

                    gdv.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Window; // Generic, ApplyThemeToDataGridView will override for grdQueue.
                    gdv.AlternatingRowsDefaultCellStyle.ForeColor = SystemColors.ControlText;
                    gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    // gdv.AlternatingRowsDefaultCellStyle.Font = SystemFonts.DefaultFont;

                    gdv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                    gdv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText; // Corrected
                    gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
                    gdv.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
                    gdv.ColumnHeadersDefaultCellStyle.Font = SystemFonts.DefaultFont;

                    gdv.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                    gdv.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
                    gdv.RowHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    gdv.RowHeadersDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    gdv.RowHeadersDefaultCellStyle.Font = SystemFonts.DefaultFont;

                    if (gdv.ContextMenuStrip != null)
                    {
                        gdv.ContextMenuStrip.BackColor = SystemColors.ControlLightLight;
                        gdv.ContextMenuStrip.ForeColor = SystemColors.ControlText;
                        foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                        {
                            item.BackColor = SystemColors.ControlLightLight;
                            item.ForeColor = SystemColors.ControlText;
                            if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                            {
                                menuItem.DropDown.BackColor = SystemColors.ControlLightLight;
                                menuItem.DropDown.ForeColor = SystemColors.ControlText;
                                foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                                {
                                    dropDownItem.BackColor = SystemColors.ControlLightLight;
                                    dropDownItem.ForeColor = SystemColors.ControlText;
                                }
                            }
                        }
                    }
                }
                else if (c is GroupBox gb)
                {
                    // gb.ForeColor already LightModeForeColor
                }
                else if (c is ProgressBar pb)
                {
                    pb.BackColor = LightModeTextBoxBack;
                    pb.ForeColor = SystemColors.Highlight;
                }
                else if (c is Label lbl)
                {
                    if (lbl.Parent is GroupBox || lbl.Parent is Panel)
                    {
                        lbl.BackColor = Color.Transparent;
                    }
                    else
                    {
                        lbl.BackColor = LightModeBackColor;
                    }
                }
                else if (c is Panel panel)
                {
                    panel.BackColor = LightModeBackColor;
                }
            }

            // Specific TextBoxes in frmQueue like txtAriaData, txtSizeInfo
            if (c.Name == "txtAriaData" || c.Name == "txtSizeInfo")
            {
                c.BackColor = darkModeEnabled ? DarkModeTextBoxBack : LightModeTextBoxBack;
                c.ForeColor = darkModeEnabled ? DarkModeForeColor : LightModeForeColor;
            }


            if (c.Controls.Count > 0)
            {
                ApplyThemeToControlCollection(c.Controls, darkModeEnabled);
            }
        }

        private void ApplyThemeToControlCollection(Control.ControlCollection controls, bool darkModeEnabled)
        {
            foreach (Control c in controls)
            {
                ApplyThemeToControl(c, darkModeEnabled);
            }
        }

        public void ApplyTheme(bool darkModeEnabled)
        {
            _isDarkModeEnabled = darkModeEnabled; // Update theme state field
            this.UseImmersiveDarkMode(darkModeEnabled);
            this.BackColor = darkModeEnabled ? DarkModeBackColor : LightModeBackColor;
            ApplyThemeToControlCollection(this.Controls, darkModeEnabled); // Recursive call for all controls

            //grdQueue.Refresh(); // ApplyThemeToDataGridView will call Refresh
            if (grdQueue != null && grdQueue.IsHandleCreated) // Check if grid is ready
            {
                ApplyThemeToDataGridView(grdQueue, _isDarkModeEnabled);
            }
        }
    }
}
