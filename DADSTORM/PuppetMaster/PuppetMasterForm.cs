using SharedTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class PuppetMasterForm : Form
    {
        private PuppetMaster PuppetMaster;

        public PuppetMasterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PuppetMaster.executeNext();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PuppetMaster.executeAll();
        }

        private void PuppetFormClosed(object sender, FormClosedEventArgs e)
        {
            PuppetMaster.terminate();
        }

        private void PuppetMasterForm_Load(object sender, EventArgs e)
        {

            PuppetMaster = new PuppetMaster(this);
            this.Text = "PuppetMaster";
            IEnumerable<string> files = Directory.GetFiles(Util.CONFIG_FILES_DIRECTORY);

            foreach (string file in files)
                ConfigFilestreeView.Nodes.Add(Path.GetFileName(file));

           files = Directory.GetFiles(Util.LOG_FILES_DIRECTORY);

            foreach (string file in files)
                logFilesTreeView.Nodes.Add(Path.GetFileName(file));

            this.FormClosed += new FormClosedEventHandler(PuppetFormClosed);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string instruction = textBox1.Text;
            PuppetMaster.parseInstruction(instruction);
            textBox1.Clear();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            PuppetMaster.crashAllOperarors();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            PuppetMaster.requestStatus();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PuppetMaster.startConfigFile(ConfigFilestreeView.SelectedNode.Text);
        }

        public void AddOperator(string op)
        {
            operatorList.Items.Add(op);
        }

        public void AddReplica(string rep)
        {
            replicasList.Items.Add(rep);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (replicasList.SelectedItem != null)
            {
                PuppetMaster.crashOperator((string)operatorList.SelectedItem, (int)replicasList.SelectedItem);
                replicasList.Items.Clear();
                replicasList.SelectedItem = null;
                replicasList.Text = "";
            }
        }


        private void operatorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            replicasList.Items.Clear();

            int repNum = PuppetMaster.getReplicas((string)operatorList.SelectedItem);
            for (int i = 0; i < repNum; i++)
                replicasList.Items.Add(i);

            replicasList.SelectedItem = null;
            replicasList.Text = "";
        }

        public void removeOperator(string op)
        {
            operatorList.Items.Remove(op);
        }


        private void button7_Click(object sender, EventArgs e)
        {
            if (replicasList.SelectedItem != null)
            {
                PuppetMaster.freezeOperator((string)operatorList.SelectedItem, (int)replicasList.SelectedItem);
                replicasList.SelectedItem = null;
                replicasList.Text = "";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (replicasList.SelectedItem != null)
            {
                PuppetMaster.unfreezeOperator((string)operatorList.SelectedItem, (int)replicasList.SelectedItem);
                replicasList.SelectedItem = null;
                replicasList.Text = "";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            PuppetMaster.startOperator((string)operatorList.SelectedItem);
            operatorList.SelectedItem = null;
            replicasList.SelectedItem = null;
            replicasList.Text = "";

        }

        private void button12_Click(object sender, EventArgs e)
        {
            logFilesTreeView.Nodes.Clear();

            IEnumerable<string> files = Directory.GetFiles(Util.LOG_FILES_DIRECTORY);

            foreach (string file in files)
                logFilesTreeView.Nodes.Add(Path.GetFileName(file));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string file = logFilesTreeView.SelectedNode.Text;
            logText.Text = File.ReadAllText(Util.LOG_FILES_DIRECTORY + "\\" + file);
        }
    }
}
