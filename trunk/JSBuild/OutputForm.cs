using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace JSBuild
{
    public partial class OutputForm : Form
    {
        private Target target;
        private String originalName;
        public OutputForm(Target t)
        {
            this.target = (t == null ? new Target("New Target 1", "$output\\newtarget1.js", new List<string>()) : t);
            this.originalName = target.Name;
            InitializeComponent();
            files.BeginUpdate();
            Project p = Project.GetInstance();
            List<FileInfo> selFiles = p.SelectedFiles;
            ListViewItem[] incItems = new ListViewItem[target.Includes.Count];
            foreach(FileInfo sf in selFiles)
            {
                string name = p.GetPath(sf.FullName);
                int index = target.Includes.IndexOf(name);
                if(index != -1)
                {
                    (incItems[index] = new ListViewItem(new string[] { sf.Name, sf.FullName }, 9)).Name = name;
                }
                else
                {
                    ListViewItem li = files.Items.Add(name, sf.Name, 9);
                    li.SubItems.Add(sf.FullName);
                }
            }
            incs.BeginUpdate();
            foreach(ListViewItem li in incItems)
            {
                if(li != null)
                    incs.Items.Add(li);
            }
            incs.EndUpdate();

            files.EndUpdate();
            txtName.DataBindings.Add("Text", target, "Name");
            txtFile.DataBindings.Add("Text", target, "File");
            cbWrap.DataBindings.Add("Checked", target, "Shorthand");
            txtList.DataBindings.Add("Text", target, "ShorthandList");
            debug.DataBindings.Add("Checked", target, "Debug");

            if(t == null)
            {
                FileInfo fi = new FileInfo(new FileInfo(Application.ExecutablePath).Directory.FullName + "\\shorthand.txt");
                if(fi.Exists)
                {
                    StreamReader sr = new StreamReader(fi.FullName);
                    this.target.ShorthandList = sr.ReadToEnd();
                    sr.Close();
                }
            }
        }

        private void OutputForm_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Project p = Project.GetInstance();
            if(txtName.Text.Trim().Length < 1)
            {
                MessageBox.Show("A name is required.");
                txtName.Focus();
                return;
            }
            if(txtFile.Text.Trim().Length < 1)
            {
                MessageBox.Show("A file name is required.");
                txtFile.Focus();
                return;
            }
            if(incs.Items.Count < 1)
            {
                MessageBox.Show("You must select at least one file to include.");
                return;
            }
            target.Includes = new List<string>(incs.Items.Count);
            foreach(ListViewItem li in incs.Items)
            {
                target.Add(li.Name);
            }

            p.AddTarget(target, originalName);

            this.Close();
        }

        private void listView1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            String text = (String)e.Data.GetData("".GetType());
            if(!text.Equals("incs")) return;
            files.CopySelections(incs, null);
            incs.RemoveSelections();
        }

        private void listView2_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            String text = (String)e.Data.GetData("".GetType());
            if(text.Equals("files"))
            {
                incs.CopySelections(files, e);
                files.RemoveSelections();
            }
            else if(text.Equals("incs"))
            {
                incs.MoveSelections(e);
            }
        }

        private void listView2_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if(!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            String text = (String)e.Data.GetData("".GetType());
            if(text.Equals("files"))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            if(text.Equals("incs"))
            {
                e.Effect = DragDropEffects.Move;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void listView1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if(!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            String text = (String)e.Data.GetData("".GetType());
            if(text.Equals("incs"))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void files_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem clickItem = files.GetItemAt(e.X, e.Y);
            if(clickItem != null)
            {
                incs.CopySelections(files, null);
                files.RemoveSelections();
            }
        }

        private void incs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem clickItem = incs.GetItemAt(e.X, e.Y);
            if(clickItem != null)
            {
                files.CopySelections(incs, null);
                incs.RemoveSelections();
            }
        }

        private void cbWrap_CheckedChanged(object sender, EventArgs e)
        {
            txtList.Enabled = cbWrap.Checked;
        }
    }
}