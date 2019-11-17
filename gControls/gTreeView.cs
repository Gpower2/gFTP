using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace gpower2.gControls
{
    public class gTreeView : TreeView
    {
        public List<gTreeNode> CheckedNodes { get; set; }

        private  Dictionary<Object, String> _ValueIndex = new Dictionary<object,string>();

        public  Dictionary<Object, String> ValueIndex
        {
            get { return _ValueIndex; }
        }

        private IList _DataSource = null;

        public IList DataSource
        {
            get { return _DataSource; }
            set
            {
                _DataSource = value;
                FillStructure();
            }
        }

        private String _ValueMember = "ID";

        public String ValueMember
        {
            get { return _ValueMember; }
            set { _ValueMember = value; FillStructure(); }
        }

        private String _DisplayMember = "Name";

        public String DisplayMember
        {
            get { return _DisplayMember; }
            set { _DisplayMember = value; FillStructure(); }
        }

        private String _ParentLinkMember = "ParentID";

        public String ParentLinkMember
        {
            get { return _ParentLinkMember; }
            set { _ParentLinkMember = value; FillStructure(); }
        }

        private String _IsSelectableMember = "IsSelectable";

        public String IsSelectableMember
        {
            get { return _IsSelectableMember; }
            set { _IsSelectableMember = value; FillStructure(); }
        }

        private String _SortByMember;

        public String SortByMember
        {
            get { return _SortByMember; }
            set { _SortByMember = value; FillStructure(); }
        }

        private Boolean _InFillStructure = false;

        public gTreeView()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.HideSelection = false;

            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
        }

        private void FillStructure()
        {
            _InFillStructure = true;
            if (!(_DataSource == null || string.IsNullOrEmpty(_ValueMember) || string.IsNullOrEmpty(_DisplayMember) || string.IsNullOrEmpty(_ParentLinkMember) || string.IsNullOrEmpty(_IsSelectableMember)))
            {
                this.BeginUpdate();
                this.Nodes.Clear();
                ValueIndex.Clear();
                CheckedNodes = new List<gTreeNode>();
                // TODO: Implement Sorting
                //_DataSource.DefaultView.Sort = _SortByMember;
                // ================================================
                foreach (var DataItem in _DataSource)
                {
                    Object parentLinkValue = DataItem.GetType().GetProperty(_ParentLinkMember).GetValue(DataItem, null);
                    if (parentLinkValue == null || Convert.ToInt32(parentLinkValue) == 0)
                    {
                        Font nodeFont = default(System.Drawing.Font);
                        if (!(bool)DataItem.GetType().GetProperty(_IsSelectableMember).GetValue(DataItem, null))
                        {
                            nodeFont = new System.Drawing.Font(this.Font, FontStyle.Italic);
                        }
                        else
                        {
                            nodeFont = new System.Drawing.Font(this.Font, FontStyle.Regular);
                        }
                        gTreeNode newNode = new gTreeNode(DataItem.GetType().GetProperty(_DisplayMember).GetValue(DataItem, null) as string,
                            DataItem.GetType().GetProperty(_ValueMember).GetValue(DataItem, null),
                            DataItem,
                            (bool)DataItem.GetType().GetProperty(_IsSelectableMember).GetValue(DataItem, null),
                            nodeFont);
                        this.Nodes.Add(newNode);

                        ValueIndex.Add(newNode.Key, newNode.FullPath);
                    }
                }
                bool NodesAdded = false;
                do
                {
                    NodesAdded = false;
                    foreach (var DataItem in _DataSource)
                    {
                        Object parentLinkValue = DataItem.GetType().GetProperty(_ParentLinkMember).GetValue(DataItem, null);
                        if (parentLinkValue != null
                            && ValueIndex.ContainsKey(parentLinkValue))
                        {
                            Font nodeFont = default(System.Drawing.Font);
                            if (!(bool)DataItem.GetType().GetProperty(_IsSelectableMember).GetValue(DataItem, null))
                            {
                                nodeFont = new System.Drawing.Font(this.Font, FontStyle.Italic);
                            }
                            else
                            {
                                nodeFont = new System.Drawing.Font(this.Font, FontStyle.Regular);
                            }
                            gTreeNode newNode = new gTreeNode(DataItem.GetType().GetProperty(_DisplayMember).GetValue(DataItem, null) as string,
                            DataItem.GetType().GetProperty(_ValueMember).GetValue(DataItem, null),
                            DataItem,
                            (bool)DataItem.GetType().GetProperty(_IsSelectableMember).GetValue(DataItem, null),
                                nodeFont);
                            if (!ValueIndex.ContainsKey(newNode.Key))
                            {
                                FindGTreeNode(ValueIndex[DataItem.GetType().GetProperty(_ParentLinkMember).GetValue(DataItem, null)]).Nodes.Add(newNode);
                                ValueIndex.Add(newNode.Key, newNode.FullPath);
                                NodesAdded = true;
                            }
                        }
                    }
                } while (NodesAdded);
                this.EndUpdate();
            }
            else
            {
                this.Nodes.Clear();
            }
            this.SelectedNode = null;
            _InFillStructure = false;
        }


        public gTreeNode FindGTreeNode(string Path)
        {
            string[] NNames = Path.Split(new String[] { this.PathSeparator }, StringSplitOptions.None);
            bool NodeFound = false;
            Object RootNode = this;
            foreach (String nodeName in NNames)
            {
                NodeFound = false;
                foreach (gTreeNode TempNode in RootNode is TreeView ? ((TreeView)RootNode).Nodes : ((TreeNode)RootNode).Nodes)
                {
                    if (TempNode.Text == nodeName)
                    {
                        RootNode = TempNode;
                        NodeFound = true;
                        break;
                    }
                }
                if (!NodeFound)
                {
                    return null;
                }
            }
            return RootNode as gTreeNode;
        }

        public void SetSelectedKey(Object NodeKey)
        {
            SetSelectedNode(this, NodeKey);
        }

        private void SetSelectedNode(Object StartNode, Object SelectedKey)
        {
            this.SuspendLayout();
            try
            {
                if (!((StartNode is TreeView ? ((TreeView)StartNode).Nodes : ((TreeNode)StartNode).Nodes).Count == 0))
                {
                    foreach (TreeNode n in StartNode is TreeView ? ((TreeView)StartNode).Nodes : ((TreeNode)StartNode).Nodes)
                    {
                        if (Convert.ChangeType(((gTreeNode)n).Key, SelectedKey.GetType()).Equals(SelectedKey))
                        {
                            this.SelectedNode = n;
                            return;
                        }
                        if (n.Nodes.Count > 0)
                        {
                            SetSelectedNode(n, SelectedKey);
                        }
                    }
                }
                else
                {
                    if (!(StartNode is TreeView))
                    {
                        this.SelectedNode = StartNode as TreeNode;
                        return;
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
            this.ResumeLayout();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (!_InFillStructure)
            {
                base.OnAfterSelect(e);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // gTreeView
            // 
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.gTreeView_BeforeCheck);
            this.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.gTreeView_AfterCheck);
            this.ResumeLayout(false);
        }

        private void gTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Checked)
                {
                    bool found = false;
                    gTreeNode a = null;
                    foreach (gTreeNode a_loopVariable in CheckedNodes)
                    {
                        a = a_loopVariable;
                        if (e.Node.FullPath == a.FullPath)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        CheckedNodes.Add((gTreeNode)e.Node);
                }
                else
                {
                    int a = 0;
                    for (a = 0; a < CheckedNodes.Count; a++)
                    {
                        if (e.Node.FullPath == CheckedNodes[a].FullPath)
                        {
                            CheckedNodes.RemoveAt(a);
                            break; 
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (e.Node.Checked)
            {
                try
                {
                    CheckedNodes.Add((gTreeNode)e.Node);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                try
                {
                    CheckedNodes.Remove((gTreeNode)e.Node);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void gTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (!this.Visible)
            {
                return;
            }
        }
    }

    [Serializable]
    public class gTreeNode : TreeNode
    {
        private Object _DataObject;

        public Object DataObject
        {
            get { return _DataObject; }
            set { _DataObject = value; }
        }

        private Object _Key;

        public Object Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        public Boolean IsSelectable { get; set; }

        public gTreeNode(string Text, Object Key, Object DataObject, bool IsSelectable, Font Font)
        {
	        base.NodeFont = Font;
	        base.Text = Text;
	        this.Key = Key;
            this.DataObject = DataObject;
	        this.IsSelectable = IsSelectable;
        }

    }
}
