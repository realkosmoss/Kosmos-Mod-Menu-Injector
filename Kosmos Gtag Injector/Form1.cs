using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpMonoInjector;

namespace Kosmos_Gtag_Injector
{
    public partial class KosmosInjector : Form
    {
        public KosmosInjector()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string modsFolderPath = Path.Combine(exeDirectory, "mods");

            if (!Directory.Exists(modsFolderPath))
            {
                MessageBox.Show("The 'mods' folder does not exist.");
                return;
            }

            string[] dllFiles = Directory.GetFiles(modsFolderPath, "*.dll");

            if (dllFiles.Length == 0)
            {
                MessageBox.Show("No DLL files found in the 'mods' folder.");
                return;
            }

            Process[] gorillaTagProcesses = Process.GetProcessesByName("Gorilla Tag");

            if (gorillaTagProcesses.Length == 0)
            {
                MessageBox.Show("The 'Gorilla Tag' process is not running.");
                return;
            }

            string dllFilePath = dllFiles[0];
            string command = $"smi.exe inject -p \"Gorilla Tag\" -a \"{dllFilePath}\" -n ModMenuPatch.HarmonyPatches -c Loader -m Load";
            ExecuteCommand(command);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string processName = "Gorilla Tag";
            Process[] gorillaTagProcesses = Process.GetProcessesByName(processName);

            if (gorillaTagProcesses.Length == 0)
            {
                MessageBox.Show("The 'Gorilla Tag' process is not running.");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLL files (*.dll)|*.dll";
            openFileDialog.Title = "Select a DLL File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string dllFilePath = openFileDialog.FileName;

                string command = $"smi.exe inject -p \"{processName}\" -a \"{dllFilePath}\" -n ModMenuPatch.HarmonyPatches -c Loader -m Load";
                ExecuteCommand(command);
            }
        }
        

        private static void ExecuteCommand(string command)
        {
            Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            });
            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine("output>>" + e.Data);
            };
            process.BeginOutputReadLine();
            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine("error>>" + e.Data);
            };
            process.BeginErrorReadLine();
            process.WaitForExit();
            Console.WriteLine("ExitCode: {0}", process.ExitCode);
            process.Close();
        }
    }

        public static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
