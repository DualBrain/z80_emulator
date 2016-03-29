using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Z80_Emulator;

namespace Z80_Emulator_UI
{
    public partial class MainForm : Form
    {
        private readonly DataTable _memoryTable;
        private Z80 _cpu;

        public MainForm()
        {
            InitializeComponent();

            _memoryTable = new DataTable("Memory");
            _memoryTable.Columns.Add("Address", typeof(string));
            _memoryTable.Columns.Add("Value", typeof(string));
            gridMemory.DataSource = _memoryTable;

            openFileDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            if (openFileDia.ShowDialog() != DialogResult.OK)
                return;

            var fileName = openFileDia.FileName;
            if (File.Exists(fileName) == false)
                return;

            var fileBytes = File.ReadAllBytes(fileName);
            if (fileBytes.Length > Memory.MEMORY_SIZE)
            {
                MessageBox.Show("The program file is too big.\nMax file size = " + Memory.MEMORY_SIZE + " bytes",
                    "Large File",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Memory mem = new Memory(fileBytes.Length);
            mem.LoadRom(fileBytes);

            _cpu = new Z80(mem);
            RefreshMemory();
            RefreshRegisters();
            RefreshFlags();
        }

        private void RefreshRegisters()
        {
            if (_cpu == null) return;

            txtA.Text = _cpu.A.ToString("X2");
            txtA_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtF.Text = _cpu.A.ToString("X2");
            txtF_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtB.Text = _cpu.A.ToString("X2");
            txtB_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtC.Text = _cpu.A.ToString("X2");
            txtC_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtD.Text = _cpu.A.ToString("X2");
            txtD_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtE.Text = _cpu.A.ToString("X2");
            txtE_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtH.Text = _cpu.A.ToString("X2");
            txtH_Alt.Text = _cpu.A_Alt.ToString("X2");
            txtL.Text = _cpu.A.ToString("X2");
            txtL_Alt.Text = _cpu.A_Alt.ToString("X2");

            txtIX.Text = _cpu.IX.ToString("X2");
            txtIY.Text = _cpu.IY.ToString("X2");
            txtSP.Text = _cpu.SP.ToString("X2");
            txtPC.Text = _cpu.PC.ToString("X2");
        }

        private void RefreshFlags()
        {
            if (_cpu == null) return;

            txtZF.Text = _cpu.CheckFlag(Z80.Flags.Z) ? "1" : "0";
            txtHF.Text = _cpu.CheckFlag(Z80.Flags.H) ? "1" : "0";
            txtCF.Text = _cpu.CheckFlag(Z80.Flags.CF) ? "1" : "0";
            txtNF.Text = _cpu.CheckFlag(Z80.Flags.N) ? "1" : "0";
            txtP_VF.Text = _cpu.CheckFlag(Z80.Flags.P_V) ? "1" : "0";
            txtSF.Text = _cpu.CheckFlag(Z80.Flags.S) ? "1" : "0";
        }

        private void RefreshMemory()
        {
            if (_cpu == null) return;

            _memoryTable.BeginLoadData();
            _memoryTable.Rows.Clear();
            for (ushort i = 0; i < Memory.MEMORY_SIZE; i++)
            {
                _memoryTable.Rows.Add(i.ToString("X"), _cpu.Memory[i].ToString("X2"));
            }
            _memoryTable.EndLoadData();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            Step();
        }

        private void Step()
        {
            if (_cpu == null) return;

            _cpu.StepEmulation();
            RefreshUI();
        }

        // ReSharper disable once InconsistentNaming
        private void RefreshUI()
        {
            RefreshFlags();
            RefreshRegisters();

            gridMemory.ClearSelection();
            gridMemory.Rows[_cpu.PC].Selected = true;
            gridMemory.FirstDisplayedScrollingRowIndex++;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (_cpu == null) return;

            timer.Interval = (int)(numStepInterval.Value * 1000);
            numStepInterval.Enabled = false;
            btnRun.Enabled = false;
            btnStop.Enabled = true;
            btnStep.Enabled = false;
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e) => Step();

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            btnStep.Enabled = true;
            numStepInterval.Enabled = true;
            btnStop.Enabled = false;
            btnRun.Enabled = true;
        }

        private void refreshMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshMemory();
        }

        private void refreshRegistersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshRegisters();
        }

        private void refreshFlagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshFlags();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Z80 Emulator");
            str.AppendLine("Created By: Feras Dawod, Hani Mounla, Rasha Malandi");
            str.AppendLine("Version: 0.1 BETA");

            MessageBox.Show(str.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
