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
            _memoryTable.Columns.Add("Address", typeof (ushort));
            _memoryTable.Columns.Add("Value", typeof (byte));
            dataGridView1.DataSource = _memoryTable;

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
                MessageBox.Show("The program file is too big.\nMax file size = " + Memory.MEMORY_SIZE + " bytes", "Large File",
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
        }

        private void RefreshFlags()
        {
            if (_cpu == null) return;
        }

        private void RefreshMemory()
        {
            if (_cpu == null) return;

            _memoryTable.BeginLoadData();
            _memoryTable.Rows.Clear();
            for (ushort i = 0; i < Memory.MEMORY_SIZE; i++)
            {
                _memoryTable.Rows.Add(i, _cpu.Memory[i]);
            }
            _memoryTable.EndLoadData();
        }
    }
}
