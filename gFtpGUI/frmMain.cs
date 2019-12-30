using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using gFtp;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using gpower2.gSettings;

namespace gFtpGUI
{
    public partial class frmMain : Form
    {
        public gFTP ftp = null;
        public Settings settings = new Settings();
        private frmQueue _FrmQueue = null;

        public frmMain()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            trvFtpFolders.ImageList = new ImageList();
            trvFtpFolders.ImageList.Images.Add("folder", Properties.Resources.Folder);
            trvFtpFolders.ImageList.Images.Add("folder_open", Properties.Resources.Folder_Open);
            trvFtpFolders.ImageKey = "folder";
            trvFtpFolders.SelectedImageKey = "folder_open";

            trvLocalFolders.ImageList = new ImageList();
            trvLocalFolders.ImageList.Images.Add("folder", Properties.Resources.Folder);
            trvLocalFolders.ImageList.Images.Add("folder_open", Properties.Resources.Folder_Open);
            trvLocalFolders.ImageKey = "folder";
            trvLocalFolders.SelectedImageKey = "folder_open";

            InitLocalDrives();
            InitFtpConnections();
            InitSettings();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            _FrmQueue = new frmQueue();
            _FrmQueue.Show();
        }

        private void InitLocalDrives()
        {
            DriveInfo[] localDrives = DriveInfo.GetDrives();
            cmbLocalDrives.DisplayMember = "Name";
            cmbLocalDrives.ValueMember = "Name";
            cmbLocalDrives.DataSource = localDrives;
        }

        private void InitFtpConnections()
        {
            String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connections.xml");
            if (!File.Exists(filename))
            {
                cmbFtpConnections.DisplayMember = "Server";
                cmbFtpConnections.DataSource = new List<FtpConnection>();
            }
            else
            {
                cmbFtpConnections.DisplayMember = "Server";

                XmlSerializer serializer = new XmlSerializer(typeof(List<FtpConnection>));
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    cmbFtpConnections.DataSource = (List<FtpConnection>)serializer.Deserialize(XmlReader.Create(fs));
                }
            }
        }

        private void InitSettings()
        {
            String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.ini");

            gSettingsManager.ReadSettings(settings, filename);
            if(!String.IsNullOrWhiteSpace(settings.Aria2Path))
            {
                txtAria.Text = settings.Aria2Path;
            }
            else
            {
                txtAria.Clear();
            }
        }

        private void ClearFtpFolder()
        {
            trvFtpFolders.Nodes.Clear();
            grdFtpFiles.DataSource = null;
            txtFtpPath.Clear();
        }

        private void ClearLocalFolder()
        {
            trvLocalFolders.Nodes.Clear();
            grdLocalFiles.DataSource = null;
            txtLocalPath.Clear();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFtpFolder();

                grpFtpServer.Enabled = false;
                grpFtpServer.Cursor = Cursors.WaitCursor;

                grpFtpFiles.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;

                ftp = new gFTP(txtFtpServer.Text, txtUsername.Text, txtPassword.Text);
                FtpFolder f = await ftp.GetFtpFolderDetailsAsync("", 0);
                FillFtpDirectoryTree(f);

                grpFtpServer.Enabled = true;
                grpFtpServer.Cursor = Cursors.Default;

                grpFtpFiles.Enabled = true;
                grpFtpFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpFtpServer.Cursor = Cursors.Default; 
                grpFtpFiles.Cursor = Cursors.Default;
                
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpServer.Enabled = true;
                grpFtpFiles.Enabled = true;
            }
        }

        private void FillFtpDirectoryTree(FtpFolder argFtpInfo)
        {
            TreeNode rootNode = new TreeNode(argFtpInfo.Name);
            rootNode.Tag = argFtpInfo;
            rootNode.ImageKey = "folder";
            rootNode.SelectedImageKey = "folder_open";
            GetFtpDirectories(rootNode, 0);
            // First clear nodes
            trvFtpFolders.Nodes.Clear();
            // Add the new nodes
            trvFtpFolders.Nodes.Add(rootNode);
        }

        private void GetFtpDirectories(TreeNode nodeToAddTo, Int32 argLevel)
        {
            if (argLevel >= 0)
            {
                // First clear the nodes
                nodeToAddTo.Nodes.Clear();
                IList<FtpFolder> subDirs = null;
                try
                {
                    subDirs = (nodeToAddTo.Tag as FtpFolder).Folders;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (subDirs != null)
                {
                    foreach (FtpFolder subDir in subDirs)
                    {
                        //FtpFolder f = ftp.GetFtpFolderDetails(subDir.FullPath, 0);
                        TreeNode aNode = new TreeNode(subDir.Name, 0, 0);
                        aNode.Tag = subDir;
                        aNode.ImageKey = "folder";
                        aNode.SelectedImageKey = "folder_open";
                        GetFtpDirectories(aNode, argLevel - 1);
                        nodeToAddTo.Nodes.Add(aNode);
                    }
                }
            }
        }

        private async void trvFtpFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                grpFtpFiles.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;

                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = e.Node;
                selNode.Tag = await ftp.GetFtpFolderDetailsAsync((selNode.Tag as FtpFolder).FullPath, 0);
                FtpFolder selDirInfo = selNode.Tag as FtpFolder;
                txtFtpPath.Text = selDirInfo.FullPath;
                GetFtpDirectories(selNode, 0);

                // Read the contents and add them to the ListView
                // First clear the list view contents
                grdFtpFiles.DataSource = null;

                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem();
                    itemUp.Type = "Directory";
                    itemUp.Name = "...";
                    itemUp.Icon = Properties.Resources.folder1.ToBitmap();
                    itemUp.Size = null;
                    items.Add(itemUp);
                }

                IList<FtpFolder> localDirs = null;
                try
                {
                    localDirs = selDirInfo.Folders;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (FtpFolder dir in localDirs)
                    {
                        FileItem item = new FileItem();
                        item.Type = "Directory";
                        item.Name = dir.Name;
                        item.Icon = Properties.Resources.folder1.ToBitmap();
                        item.Date = dir.Date;
                        item.Size = null;
                        items.Add(item);
                    }
                }

                IList<FtpFile> localFiles = null;
                try
                {
                    localFiles = selDirInfo.Files;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FtpFile file in localFiles)
                    {
                        FileItem item = new FileItem();
                        item.Type = file.Extension();
                        item.Name = file.Name;
                        item.Icon = IconReader.GetFileIcon(file.Extension(), IconReader.IconSize.Small, false).ToBitmap();
                        item.Date = file.Date;
                        item.Size = new gFtpGUI.FileSize(file.Size);
                        items.Add(item);
                    }
                }

                grdFtpFiles.DataSource = items;

                grpFtpFiles.Enabled = true;

                grpFtpFiles.PerformLayout();

                grpFtpFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpFtpFiles.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpFiles.Enabled = true;
            }
        }

        private void cmbLocalDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearLocalFolder();
                if (cmbLocalDrives.SelectedIndex > -1)
                {
                    DriveInfo selDrive = cmbLocalDrives.SelectedItem as DriveInfo;
                    if (selDrive.IsReady)
                    {
                        txtLocalDriveInfo.Text = String.Format("[{0}] | Size {1} | Free space {2}", selDrive.VolumeLabel, SizeHelper.GetSize(selDrive.TotalSize), SizeHelper.GetSize(selDrive.TotalFreeSpace));
                        txtLocalPath.Text = selDrive.RootDirectory.FullName;
                        FillLocalDirectoryTree(selDrive.RootDirectory);
                    }
                    else
                    {
                        txtLocalDriveInfo.Text = "Device not ready";
                    }
                }
                else
                {
                    txtLocalDriveInfo.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefreshLocalPath_Click(object sender, EventArgs e)
        {
            try
            {
                // Refresh the local drive info
                if (cmbLocalDrives.SelectedIndex > -1)
                {
                    DriveInfo selDrive = cmbLocalDrives.SelectedItem as DriveInfo;
                    if (selDrive.IsReady)
                    {
                        txtLocalDriveInfo.Text = String.Format("[{0}] | Size {1} | Free space {2}", selDrive.VolumeLabel, SizeHelper.GetSize(selDrive.TotalSize), SizeHelper.GetSize(selDrive.TotalFreeSpace));
                    }
                    else
                    {
                        txtLocalDriveInfo.Text = "Device not ready";
                    }
                }

                if (trvLocalFolders.SelectedNode != null)
                {
                    // Read the Sub Directories of the selected node and update the Folder Tree View
                    TreeNode selNode = trvLocalFolders.SelectedNode;
                    DirectoryInfo selDirInfo = selNode.Tag as DirectoryInfo;
                    txtLocalPath.Text = selDirInfo.FullName;
                    GetDirectories(selNode, 1);

                    // Read the contents and add them to the ListView
                    // Keep the sorted column and order
                    DataGridViewColumn sortColumn = grdLocalFiles.SortedColumn;
                    SortOrder sortOrder = grdLocalFiles.SortOrder;
                    int oldSelection = grdLocalFiles.SelectedIndex;
                    int oldVerticalOffset = grdLocalFiles.FirstDisplayedScrollingRowIndex;
                    // First clear the list view contents
                    grdLocalFiles.DataSource = null;

                    List<FileItem> items = new List<FileItem>();

                    // First add the Up Directory only if there is a parent node
                    if (selNode.Parent != null)
                    {
                        FileItem itemUp = new FileItem();
                        itemUp.Type = "Directory";
                        itemUp.Name = "...";
                        itemUp.Icon = Properties.Resources.folder1.ToBitmap();
                        itemUp.Size = null;
                        items.Add(itemUp);
                    }

                    DirectoryInfo[] localDirs = null;
                    try
                    {
                        localDirs = selDirInfo.GetDirectories();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    if (localDirs != null)
                    {
                        foreach (DirectoryInfo dir in localDirs)
                        {
                            FileItem item = new FileItem();
                            item.Type = "Directory";
                            item.Name = dir.Name;
                            item.Icon = Properties.Resources.folder1.ToBitmap();
                            item.Date = dir.LastWriteTime;
                            item.Size = null;
                            items.Add(item);
                        }
                    }

                    FileInfo[] localFiles = null;
                    try
                    {
                        localFiles = selDirInfo.GetFiles();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    if (localFiles != null)
                    {
                        foreach (FileInfo file in localFiles)
                        {
                            FileItem item = new FileItem();
                            item.Type = file.Extension;
                            item.Name = file.Name;
                            item.Icon = IconReader.GetFileIcon(file.Extension, IconReader.IconSize.Small, false).ToBitmap();
                            item.Date = file.LastWriteTime;
                            item.Size = new gFtpGUI.FileSize(file.Length);
                            items.Add(item);
                        }
                    }

                    grdLocalFiles.DataSource = items;
                    // Set the sortedColumn and order again
                    if (sortColumn != null)
                    {
                        // Find the new DataGridViewColumn
                        DataGridViewColumn newSortColumn = grdLocalFiles.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == sortColumn.Name);
                        if (newSortColumn != null)
                        {
                            if (sortOrder == SortOrder.Ascending)
                            {
                                grdLocalFiles.Sort(newSortColumn, ListSortDirection.Ascending);
                            }
                            else if (sortOrder == SortOrder.Descending)
                            {
                                grdLocalFiles.Sort(newSortColumn, ListSortDirection.Descending);
                            }
                        }
                    }
                    grdLocalFiles.SetSelectedRowByIndex(oldSelection);
                    grdLocalFiles.FirstDisplayedScrollingRowIndex = oldVerticalOffset;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillLocalDirectoryTree(DirectoryInfo argInfo)
        {
            TreeNode rootNode = new TreeNode(argInfo.Name);
            rootNode.Tag = argInfo;
            rootNode.ImageKey = "folder";
            rootNode.SelectedImageKey = "folder_open";
            GetDirectories(rootNode, 1);
            // First clear nodes
            trvLocalFolders.Nodes.Clear();
            // Add the new nodes
            trvLocalFolders.Nodes.Add(rootNode);
        }

        private void GetDirectories(TreeNode nodeToAddTo, Int32 argLevel)
        {
            if (argLevel > 0)
            {
                // First clear the nodes
                nodeToAddTo.Nodes.Clear();
                DirectoryInfo[] subDirs = null;
                try
                {
                    subDirs = (nodeToAddTo.Tag as DirectoryInfo).GetDirectories();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (subDirs != null)
                {
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        TreeNode aNode = new TreeNode(subDir.Name, 0, 0);
                        aNode.Tag = subDir;
                        aNode.ImageKey = "folder";
                        aNode.SelectedImageKey = "folder_open";
                        GetDirectories(aNode, argLevel - 1);
                        nodeToAddTo.Nodes.Add(aNode);
                    }
                }
            }
        }

        private void trvLocalFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = e.Node;
                DirectoryInfo selDirInfo = selNode.Tag as DirectoryInfo;
                txtLocalPath.Text = selDirInfo.FullName;
                GetDirectories(selNode, 1);


                // Read the contents and add them to the ListView
                // First clear the list view contents
                grdLocalFiles.DataSource = null;

                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem();
                    itemUp.Type = "Directory";
                    itemUp.Name = "...";
                    itemUp.Icon = Properties.Resources.folder1.ToBitmap();
                    itemUp.Size = null;
                    items.Add(itemUp);
                }

                DirectoryInfo[] localDirs = null;
                try
                {
                    localDirs = selDirInfo.GetDirectories();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (DirectoryInfo dir in localDirs)
                    {
                        FileItem item = new FileItem();
                        item.Type = "Directory";
                        item.Name = dir.Name;
                        item.Icon = Properties.Resources.folder1.ToBitmap();
                        item.Date = dir.LastWriteTime;
                        item.Size = null;
                        items.Add(item);
                    }
                }

                FileInfo[] localFiles = null;
                try
                {
                    localFiles = selDirInfo.GetFiles();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FileInfo file in localFiles)
                    {
                        FileItem item = new FileItem();
                        item.Type = file.Extension;
                        item.Name = file.Name;
                        item.Icon = IconReader.GetFileIcon(file.Extension, IconReader.IconSize.Small, false).ToBitmap();
                        item.Date = file.LastWriteTime;
                        item.Size = new gFtpGUI.FileSize(file.Length);
                        items.Add(item);
                    }
                }

                grdLocalFiles.DataSource = items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAria_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // check if the drop data is actually a file or folder
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    String[] s = (String[])e.Data.GetData(DataFormats.FileDrop, false);
                    ((TextBox)sender).Text = s[0];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAria_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.All;
                }
                else
                    e.Effect = DragDropEffects.None;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseAria_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select aria2...";
                ofd.FileName = "aria2c.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtAria.Text = ofd.FileName;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if(grdFtpFiles.SelectedIndex == -1)
                {
                    throw new Exception("No Ftp File selected!");
                }
                if(String.IsNullOrWhiteSpace( txtLocalPath.Text))
                {
                    throw new Exception("No local path was selected!");
                }
                if (String.IsNullOrWhiteSpace(txtAria.Text))
                {
                    throw new Exception("No Aria2 path was provided!");
                }
                foreach (FileItem f in grdFtpFiles.SelectedItems)
                {
                    if (f.Type != "Directory")
                    {
                        Job j = new gFtpGUI.Job();
                        j.AriaPath = txtAria.Text;
                        j.FtpPath = txtFtpPath.Text;
                        j.FtpFilename = f.Name;
                        j.LocalPath = txtLocalPath.Text;
                        j.LocalFilename = f.Name;
                        j.Size = f.Size;
                        j.Ftp = ftp;

                        await _FrmQueue.AddJobAsync(j);
                    }
                    else
                    {
                        // First we have to get all the Folders and Directories are inside
                        FtpFolder fd = await ftp.GetFtpFolderDetailsAsync(UrlHelper.Combine(txtFtpPath.Text, f.Name), Int32.MaxValue);
                        DownloadFtpFolder(fd, txtLocalPath.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDeleteRemoteFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdFtpFiles.SelectedIndex == -1)
                {
                    throw new Exception("No Ftp File selected!");
                }

                // Ask for confirmation from user
                if (MessageBox.Show(
                    "Are you sure you want to delete remote files?", 
                    "Are you sure?", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                foreach (FileItem f in grdFtpFiles.SelectedItems)
                {
                    if (f.Type != "Directory")
                    {
                        await ftp.DeleteRemoteFile(UrlHelper.Combine(txtFtpPath.Text, f.Name));
                    }
                    else
                    {
                        continue;
                    }
                }

                // Refresh the ftp grid
                btnRefreshFtpPath_Click(null, null);

                MessageBox.Show($"The remote file(s) was deleted successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DownloadFtpFolder(FtpFolder argFolder, String argLocalRoot)
        {
            // Make all the necessary local folders
            String localDestFolder = Path.Combine(argLocalRoot, argFolder.Name);
            if (!Directory.Exists(localDestFolder))
            {
                Directory.CreateDirectory(localDestFolder);
            }
            List<Job> jobs = new List<gFtpGUI.Job>();
            foreach (FtpFile file in argFolder.Files)
            {
                Job j = new gFtpGUI.Job();
                j.AriaPath = txtAria.Text;
                j.FtpPath = argFolder.FullPath;
                j.FtpFilename = file.Name;
                j.LocalPath = localDestFolder;
                j.LocalFilename = file.Name;
                j.Size = new FileSize(file.Size);
                j.Ftp = ftp;

                jobs.Add(j);

                 Application.DoEvents();
            }
            await _FrmQueue.AddJobsAsync(jobs);
            foreach (FtpFolder folder in argFolder.Folders)
            {
                if (folder.Name != "..." && folder.Name != ".." && folder.Name != ".")
                {
                    DownloadFtpFolder(folder, localDestFolder);
                }
            }
        }

        private void grdFtpFiles_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if(grdFtpFiles.SelectedIndex == -1)
                {
                    return;
                }
                if (grdFtpFiles.LastClickedRowIndex == -1)
                {
                    return;
                }
                FileItem f = grdFtpFiles.SelectedItem as FileItem;
                if(f.Type == "Directory")
                {
                    if (f.Name == "..." || f.Name == ".." || f.Name == ".")
                    {
                        if (trvFtpFolders.SelectedNode.Parent != null)
                        {
                            trvFtpFolders.SelectedNode = trvFtpFolders.SelectedNode.Parent;
                        }
                    }
                    else
                    {
                        String argPath = UrlHelper.Combine(txtFtpPath.Text, f.Name).Replace("//", "/");
                        TreeNode node = FindFtpNode(trvFtpFolders.Nodes[0], argPath);
                        if (node != null)
                        {
                            trvFtpFolders.SelectedNode = node;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grdLocalFiles_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (grdLocalFiles.SelectedIndex == -1)
                {
                    return;
                }
                if(grdLocalFiles.LastClickedRowIndex == -1)
                {
                    return;
                }
                FileItem f = grdLocalFiles.SelectedItem as FileItem;
                if (f.Type == "Directory")
                {
                    if (f.Name == "...")
                    {
                        if (trvLocalFolders.SelectedNode.Parent != null)
                        {
                            trvLocalFolders.SelectedNode = trvLocalFolders.SelectedNode.Parent;
                        }
                    }
                    else
                    {
                        String argPath = Path.Combine(txtLocalPath.Text, f.Name);
                        TreeNode node = FindLocalNode(trvLocalFolders.Nodes[0], argPath);
                        if (node != null)
                        {
                            trvLocalFolders.SelectedNode = node;
                        }
                    }
                }
                else
                {
                    Process.Start(Path.Combine(txtLocalPath.Text, (grdLocalFiles.SelectedItem as FileItem).Name));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TreeNode FindLocalNode(TreeNode argNode, String argPath)
        {
            if( (argNode.Tag as DirectoryInfo).FullName == argPath)
            {
                return argNode;
            }
            else
            {
                if(argNode.Nodes.Count > 0)
                {
                    foreach (TreeNode node in argNode.Nodes)
                    {
                        TreeNode selNode = FindLocalNode(node, argPath);
                        if(selNode != null)
                        {
                            return selNode;
                        }
                    }
                }
            }
            return null;
        }

        private TreeNode FindFtpNode(TreeNode argNode, String argPath)
        {
            if ((argNode.Tag as FtpFolder).FullPath == argPath)
            {
                return argNode;
            }
            else
            {
                if (argNode.Nodes.Count > 0)
                {
                    foreach (TreeNode node in argNode.Nodes)
                    {
                        TreeNode selNode = FindFtpNode(node, argPath);
                        if (selNode != null)
                        {
                            return selNode;
                        }
                    }
                }
            }
            return null;
        }

        private async void btnRefreshFtpPath_Click(object sender, EventArgs e)
        {
            try
            {
                if (trvFtpFolders.SelectedNode == null)
                {
                    return;
                }

                grpFtpFiles.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;

                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = trvFtpFolders.SelectedNode;
                selNode.Tag = await ftp.GetFtpFolderDetailsAsync((selNode.Tag as FtpFolder).FullPath, 0);
                FtpFolder selDirInfo = selNode.Tag as FtpFolder;
                txtFtpPath.Text = selDirInfo.FullPath;
                GetFtpDirectories(selNode, 0);

                // Read the contents and add them to the ListView
                // Keep the sorted column and order
                DataGridViewColumn sortColumn = grdFtpFiles.SortedColumn;
                SortOrder sortOrder = grdFtpFiles.SortOrder;
                int oldVerticalOffset = grdFtpFiles.FirstDisplayedScrollingRowIndex;
                int oldSelectedIndex = grdFtpFiles.SelectedIndex;
                // First clear the list view contents
                grdFtpFiles.DataSource = null;

                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem();
                    itemUp.Type = "Directory";
                    itemUp.Name = "...";
                    itemUp.Icon = Properties.Resources.folder1.ToBitmap();
                    itemUp.Size = null;
                    items.Add(itemUp);
                }

                IList<FtpFolder> localDirs = null;
                try
                {
                    localDirs = selDirInfo.Folders;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (FtpFolder dir in localDirs)
                    {
                        FileItem item = new FileItem();
                        item.Type = "Directory";
                        item.Name = dir.Name;
                        item.Icon = Properties.Resources.folder1.ToBitmap();
                        item.Date = dir.Date;
                        item.Size = null;
                        items.Add(item);
                    }
                }

                IList<FtpFile> localFiles = null;
                try
                {
                    localFiles = selDirInfo.Files;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FtpFile file in localFiles)
                    {
                        FileItem item = new FileItem();
                        item.Type = file.Extension();
                        item.Name = file.Name;
                        item.Icon = IconReader.GetFileIcon(file.Extension(), IconReader.IconSize.Small, false).ToBitmap();
                        item.Date = file.Date;
                        item.Size = new gFtpGUI.FileSize(file.Size);
                        items.Add(item);
                    }
                }

                grdFtpFiles.DataSource = items;
                // Set the sortedColumn and order again
                if (sortColumn != null)
                {
                    // Find the new DataGridViewColumn
                    DataGridViewColumn newSortColumn = grdFtpFiles.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == sortColumn.Name);
                    if (newSortColumn != null)
                    {
                        if (sortOrder == SortOrder.Ascending)
                        {
                            grdFtpFiles.Sort(newSortColumn, ListSortDirection.Ascending);
                        }
                        else if (sortOrder == SortOrder.Descending)
                        {
                            grdFtpFiles.Sort(newSortColumn, ListSortDirection.Descending);
                        }
                    }
                }
                
                // Check if Vertical Offset is now valid
                if (oldVerticalOffset > grdFtpFiles.Rows.Count - 1)
                {
                    oldVerticalOffset = grdFtpFiles.Rows.Count - 1;
                }                

                grdFtpFiles.FirstDisplayedScrollingRowIndex = oldVerticalOffset;


                // Check if selected index is now valid
                if (oldSelectedIndex > grdFtpFiles.Rows.Count - 1)
                {
                    oldSelectedIndex = grdFtpFiles.Rows.Count - 1;
                }

                grdFtpFiles.SetSelectedRowByIndex(oldSelectedIndex, false);

                grpFtpFiles.Enabled = true;

                grdFtpFiles.PerformLayout();

                grpFtpFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grpFtpFiles.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpFiles.Enabled = true;
            }
        }

        private void cmbFtpConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearFtpFolder();
                if (cmbFtpConnections.SelectedItem == null)
                {
                    txtFtpServer.Clear();
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
                else
                {
                    FtpConnection myCon = cmbFtpConnections.SelectedItem as FtpConnection;
                    txtFtpServer.Text = myCon.Server;
                    txtUsername.Text = myCon.Username;
                    txtPassword.Text = myCon.Password;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveFtpConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtFtpServer.Text))
                {
                    throw new Exception("The ftp server cannot be empty!");
                }
                if (String.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    throw new Exception("The ftp username cannot be empty!");
                }
                if (String.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    throw new Exception("The ftp password cannot be empty!");
                }

                if (
                    (cmbFtpConnections.DataSource as IList<FtpConnection>)
                    .Any(f => 
                        f.Server.ToLower().Trim().Equals(txtFtpServer.Text.ToLower().Trim())
                        && f.Username.ToLower().Trim().Equals(txtUsername.Text.ToLower().Trim())
                    )
                )
                {
                    throw new Exception("The FTP connection already exists!");
                }

                FtpConnection newCon = new FtpConnection();
                newCon.Server = txtFtpServer.Text.Trim();
                newCon.Username = txtUsername.Text.Trim();
                newCon.Password = txtPassword.Text.Trim();

                List<FtpConnection> newList = cmbFtpConnections.DataSource as List<FtpConnection>;
                newList.Add(newCon);

                String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connections.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<FtpConnection>));
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    serializer.Serialize(fs, newList);
                }

                InitFtpConnections();
                cmbFtpConnections.SelectedItem = (cmbFtpConnections.Items as IList<FtpConnection>).FirstOrDefault(t => t.Server == newCon.Server);

                MessageBox.Show("The connection was saved!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteFtpConnection_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if an FtpConnections was selected
                if (cmbFtpConnections.SelectedIndex == -1)
                {
                    return;
                }

                // First ask the user
                if (MessageBox.Show(
                    "Do you really want to delete the selected Ftp Connection?", 
                    "Are you sure?", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Get the selected index
                int prevSelectionIndex = cmbFtpConnections.SelectedIndex;

                // Get the items as list
                List<FtpConnection> connectionsList = cmbFtpConnections.DataSource as List<FtpConnection>;

                // Remove the selected FtpConnection from the list
                connectionsList.Remove(cmbFtpConnections.SelectedItem as FtpConnection);

                // Update the file with the ftp connections
                String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connections.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<FtpConnection>));
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    serializer.Serialize(fs, connectionsList);
                }

                // Read the ftp connections from the file
                InitFtpConnections();

                // Check if the previous selected index is now valid
                if (prevSelectionIndex > cmbFtpConnections.Items.Count - 1)
                {
                    prevSelectionIndex = cmbFtpConnections.Items.Count - 1;
                }

                cmbFtpConnections.SelectedIndex = prevSelectionIndex;

                MessageBox.Show("The connection was deleted!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveAriaPath_Click(object sender, EventArgs e)
        {
            try
            {
                String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.ini");
                settings.Aria2Path = txtAria.Text.Trim();
                gSettingsManager.SaveSettings(settings, filename);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnJobs_Click(object sender, EventArgs e)
        {
            try
            {
                _FrmQueue.Show();
                _FrmQueue.Select();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
