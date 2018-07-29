using ByteSizeLib;
using MaterialSkin;
using MaterialSkin.Controls;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using Kaitai;
using System.Drawing;

namespace OneDumper
{
    public partial class Form1 : MaterialForm
    {
        string tmpDir = Program.tmpDir,
            appPath = AppDomain.CurrentDomain.BaseDirectory,
            comPort = null,
            programmerPrefix = null;
        int Default = 1,
            Error = 2,
            Success = 3,
            Warning = 4;

        ListViewItem lastItemChecked;
        List<string> devices = new List<string>();

        Timer COMPortReloader;

        public Form1()
        {
            Icon = Properties.Resources.onelabs;
            InitializeComponent();
            log.SelectionChanged += log_disableSelection;
            partList.SelectionChanged += parts_disableSelection;
            writeLog("Program Başlatıldı.", Default);
            getProgrammersAsync();
            reloadCOMPorts(null,null);
            COMPortReloader = new Timer();
            COMPortReloader.Tick += reloadCOMPorts;
            COMPortReloader.Interval = 800;
            COMPortReloader.Start();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue600, Primary.Blue700, Primary.Blue200, Accent.Red100, TextShade.WHITE);

            programmerList.ItemCheck += programmerList_ItemCheck;
            programmerList.ItemChecked += programmerList_ItemCheckedAsync;
    }
        
        async Task downloadFileAsync(string file,string output,bool call=false)
        {
            if (!File.Exists(output))
            {
                writeLog(file + " İndiriliyor...", Warning);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://github.com/OneLabsTools/Programmers/raw/master/" + file);
                response.EnsureSuccessStatusCode();
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                using (FileStream fileStream = new FileStream(output, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
                writeLog(file + " Başarıyla İndirildi.", Success);
                if (call) Process.Start(file);
            }
            else if(call)
            {
                Process.Start(file);
            }
        }

        async Task downloadProgrammerAsync(string programmer)
        {
            await downloadFileAsync(programmer, "DownloadedProgrammers\\" + programmer);
        }

        public string GetBytesReadable(long size)
        {
            return ByteSize.FromKiloBytes(size).ToString();
        }

        public string createXML(string xml)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><data>" + xml + "</data>"));
        }

        public async Task<bool> backupPartitionAsync(string sector_size, string sector_offset, string filename, string label, string partition_sectors, string partition_number, string size_kb, string sparse, string start_byte, string start_sector)
        {
            bool error = false;
            string xml = createXML("<read SECTOR_SIZE_IN_BYTES=\"" + sector_size + "\" file_sector_offset=\"" + sector_offset + "\" filename=\"" + filename + "\" label=\"" + label + "\" num_partition_sectors=\"" + partition_sectors + "\" physical_partition_number=\"" + partition_number + "\" size_in_KB=\"" + size_kb + "\" sparse=\"" + sparse + "\" start_byte_hex=\"" + start_byte + "\" start_sector=\"" + start_sector + "\"/>");
            writeLog("Bölüm Yedekleniyor : "+label, Warning);
            string output = await runCommand(appPath + "/Library/OneLoader.exe", "--port=\\\\.\\" + comPort + " --rawxml=" + xml, tmpDir);
                if (output.Contains("All Finished Successfully"))
                {
                    writeLog("Bölüm Yedeklendi : " + label, Success);
                }
                else
                {
                    writeLog("Bölüm Yedeklenemedi : "+label, Error);
                    error = true;
                }
            return error;
        }

        private void reloadCOMPorts(object sender, EventArgs e)
        {
            List<string> tmpDevices = new List<string>();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                foreach (string s in portList)
                {
                    if (s.Contains("QDLoader 9008")) tmpDevices.Add(s);
                }
            }
            if(devices.Count!=tmpDevices.Count)
            {
                deviceList.DataSource = null;
                deviceList.Items.Clear();
                deviceList.DataSource = tmpDevices;

                devices = tmpDevices;

                if (deviceList.Items.Count != 0)
                {
                    deviceList.SelectedIndex = 0;
                }
                else
                {
                    deviceList.Text = null;
                }
            }        }

        async Task getProgrammersAsync()
        {
            List<ListViewItem> programmers = new List<ListViewItem>();

            var response = await new GitHubClient(new ProductHeaderValue("OneDumper"))
                .Repository
                .Content.GetAllContents("OneLabsTools", "Programmers");

            IReadOnlyList<RepositoryContent> files = response;
            
            if (Directory.Exists("CustomProgrammers"))
            {
                foreach (string file in Directory.GetFiles("CustomProgrammers", "*.*").Where(s => s.EndsWith(".mbn") || s.EndsWith(".elf") || s.EndsWith(".bin")).Select(p => Path.GetFileName(p)).ToArray())
                {
                    programmers.Add(new ListViewItem(file, programmerList.Groups[0]));
                }
            }

            foreach (RepositoryContent file in files)
            {
                if(!file.Name.Contains("QCOM_USB_Driver")) programmers.Add(new ListViewItem(file.Name,programmerList.Groups[1]));
            }

            programmerList.BeginUpdate();
            ListView.ListViewItemCollection lvic = new ListView.ListViewItemCollection(programmerList);
            lvic.AddRange(programmers.ToArray());
            programmerList.EndUpdate();
        }
        private void materialRaisedButton3_ClickAsync(object sender, EventArgs e)
        {
            if (File.Exists("log.txt")) File.Delete("log.txt");
            using (var tw = new StreamWriter("log.txt"))
            {
                foreach (DataGridViewRow item in log.Rows)
                {
                    string info=null;
                    int type = (int)item.Cells[1].Value;
                    if (type == Success)
                    {
                        info = "Başarılı";
                    }
                    else if (type == Error)
                    {
                        info = "Hata";
                    }
                    else if (type == Warning)
                    {
                        info = "Uyarı";
                    }
                    else if (type == Default)
                    {
                        info = "Bilgi";
                    }
                    tw.WriteLine(item.Cells[2].Value+" : ("+info+") "+item.Cells[3].Value);
                }
            }
            writeLog("Log Kaydedildi : log.txt", Success);
        }

        private async void materialRaisedButton2_ClickAsync(object sender, EventArgs e)
        {
            string targetOS = (Environment.Is64BitOperatingSystem ? "_x64" : "_x86");
            await downloadFileAsync("QCOM_USB_Driver" + targetOS + ".exe", "QCOM_USB_Driver" + targetOS + ".exe", true);
        }
        async Task readPartitionsAsync()
        {
            List<Dictionary<string, string>> partitions = new List<Dictionary<string, string>> ();
            byte[] gptFile = File.ReadAllBytes(tmpDir + "/gpt_main0.bin");
            var gpt = new GptPartitionTable(new KaitaiStream(gptFile));
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            bool backupGPT = false;
            foreach (GptPartitionTable.PartitionEntry part in gpt.Primary.Entries)
            {
                var vars = new Dictionary<string, string>();
                string label = new String(part.Name.Where(c => Char.IsLetterOrDigit(c)).ToArray()),
                    filename = label + ".img",
                    SECTOR_SIZE_IN_BYTES = part.M_Root.SectorSize.ToString(),
                    start_sector = part.FirstLba.ToString(),
                    end_sector = part.LastLba.ToString(),
                    num_partition_sectors = (int.Parse(end_sector) - int.Parse(start_sector) + 1).ToString(),
                    size_in_KB = (int.Parse(num_partition_sectors) / 2.0).ToString("0.0"),
                    start_byte_hex = "0x"+(part.FirstLba * (ulong)part.M_Root.SectorSize).ToString("X");

                if(string.IsNullOrEmpty(label.Trim()))
                {
                    /*
                     * 
                     * Primary Sector : 17408
                     * Backup Sector : 16896
                     * 
                     */
                    if (backupGPT)
                    {
                        label = "BackupGPT";
                        filename = "gpt_backup0.bin";
                        if (SECTOR_SIZE_IN_BYTES.Equals(4096))
                        {
                            start_sector = "NUM_DISK_SECTORS-5.";
                            num_partition_sectors = "5";
                        }
                        else
                        {
                            start_sector = "NUM_DISK_SECTORS-33.";
                            num_partition_sectors = "33";
                        }
                        start_byte_hex = "(" + SECTOR_SIZE_IN_BYTES+ "*NUM_DISK_SECTORS)-16896.";
                    }
                    else
                    {
                        label = "PrimaryGPT";
                        filename = "gpt_main0.bin";
                        start_sector = "0";
                        if (SECTOR_SIZE_IN_BYTES.Equals(4096))
                        {
                            num_partition_sectors = "6";
                        }
                        else
                        {
                            num_partition_sectors = "34";
                        }
                        start_byte_hex = "0x0";

                        backupGPT = true;
                    }
                    size_in_KB = (int.Parse(num_partition_sectors) / 2.0).ToString("0.0");
                }
                vars.Add("label", label);
                vars.Add("filename", filename);
                vars.Add("SECTOR_SIZE_IN_BYTES", SECTOR_SIZE_IN_BYTES);
                vars.Add("start_sector", start_sector);
                vars.Add("end_sector", end_sector);
                vars.Add("num_partition_sectors", num_partition_sectors);
                vars.Add("size_in_KB", size_in_KB);
                vars.Add("start_byte_hex", start_byte_hex);
                vars.Add("file_sector_offset", "0");
                vars.Add("physical_partition_number", "0");
                vars.Add("sparse", "false");
                partitions.Add(vars);
            }

            File.Delete(tmpDir + "/gpt_main0.bin");
            
            if (partList.InvokeRequired)
            {
                partList.Invoke(new MethodInvoker(delegate ()
                {
                    partList.Rows.Clear();
                    partList.Refresh();
                    partitions.ForEach(delegate (Dictionary<string, string> part)
                    {
                        partList.Rows.Add(part["label"], GetBytesReadable(Convert.ToInt64(part["size_in_KB"].Split('.')[0])), false, part["SECTOR_SIZE_IN_BYTES"], part["file_sector_offset"], part["filename"], part["num_partition_sectors"], part["physical_partition_number"], part["size_in_KB"], part["sparse"], part["start_byte_hex"], part["start_sector"]);
                    });
                    partList.Sort(partList.Columns[0], ListSortDirection.Ascending);
                }
                ));
            }
            else
            {
                partList.Rows.Clear();
                partList.Refresh();
                partitions.ForEach(delegate (Dictionary<string, string> part)
                {
                    partList.Rows.Add(part["label"], GetBytesReadable(Convert.ToInt64(part["size_in_KB"].Split('.')[0])), false, part["SECTOR_SIZE_IN_BYTES"], part["file_sector_offset"], part["filename"], part["num_partition_sectors"], part["physical_partition_number"], part["size_in_KB"], part["sparse"], part["start_byte_hex"], part["start_sector"]);
                });
                partList.Sort(partList.Columns[0], ListSortDirection.Ascending);
            }
            if (materialRaisedButton1.InvokeRequired)
            {
                materialRaisedButton1.Invoke(new MethodInvoker(delegate ()
                {
                    materialRaisedButton1.Enabled = true;
                }
                ));
            }
            else
            {
                materialRaisedButton1.Enabled = true;
            }
            writeLog("Bölümler Okundu.", Success);
        }

        private async void materialRaisedButton1_ClickAsync(object sender, EventArgs e)
        {
            bool doAction = false,
                    error=false;
            foreach (DataGridViewRow row in partList.Rows)
            {
                if (row.Cells["read"].Value.ToString().Equals("1"))
                {
                    if (error) break;
                    if(!doAction) doAction = true;
                    error=await backupPartitionAsync(row.Cells["sector_size"].Value.ToString(), row.Cells["sector_offset"].Value.ToString(), row.Cells["filename"].Value.ToString(), row.Cells["partition"].Value.ToString(), row.Cells["partition_sectors"].Value.ToString(), row.Cells["partition_number"].Value.ToString(), row.Cells["size_kb"].Value.ToString(), row.Cells["sparse"].Value.ToString(), row.Cells["start_byte"].Value.ToString(), row.Cells["start_sector"].Value.ToString());
                }
            }
            if(doAction && !error)
            {
                writeLog("Yedekleme İşlemi Tamamlandı.", Success);
            }
            else if (doAction && error)
            {
                writeLog("Yedeklme İşlemi Başarısız.", Error);
            }
            else if (!doAction)
            {
                writeLog("Yedeklenecek Bölüm Seçilmedi.", Error);
            }
        }

        void getGPTAsync()
        {
            writeLog("Bölümler Okunuyor...", Warning);
            runCommand(appPath + "/Library/OneLoader.exe", "--port=\\\\.\\" + comPort + " --getgptmainbackup=gpt_both0.bin", tmpDir, async delegate (object s, EventArgs a)
              {
                  File.Delete(tmpDir + "/gpt_backup0.bin");
                  Process p = (Process)s;
                  string o = await p.StandardOutput.ReadToEndAsync();
                  if (o.Contains("All Finished Successfully"))
                  {
                      await readPartitionsAsync();
                  }
                  else
                  {
                      writeLog("Bölümler Okunamadı.", Error);
                  }
              });
        }
        void runCommand(string command,string args, string path, EventHandler e)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo {
                    FileName =command,
                    Arguments= args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = path,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            };
            process.Exited += e;
            process.Start();
            process.WaitForExit();
        }

        async Task<string> runCommand(string command, string args, string path)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = path,
                    RedirectStandardOutput = true,
                },
            };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();
            return output;
        }

        private void materialRaisedButton4_ClickAsync(object sender, EventArgs e)
        {            
            if (String.IsNullOrEmpty(deviceList.Text))
            {
                writeLog("Cihaz Seçilmedi.", Error);
                return;
            }
            comPort = deviceList.Text.ToString().Split('-')[0].Trim();
            if (programmerList.CheckedItems.Count != 1)
            {
                writeLog("Programmer Seçilmedi.", Error);
                return;
            }
            string selectedProgrammer = programmerList.CheckedItems[0].Text;
            if (!File.Exists(programmerPrefix + "/" + selectedProgrammer))
            {
                MessageBox.Show("Programmer Geçersiz.");
                return;
            }
            COMPortReloader.Stop();
            COMPortReloader.Enabled = false;
            deviceList.Enabled = false;
            programmerList.Enabled = false;
            materialRaisedButton4.Enabled = false;

            if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);
            writeLog("Cihaza Bağlanılıyor : " + comPort, Warning);
            writeLog("Sahara Protokolü Başlatılıyor : " + selectedProgrammer, Warning);
            /*
            string checkCom = await runCommand("Library/OneLoader.exe", "--port=\\\\.\\" + comPort, appPath);
            if (checkCom.Contains("All Finished Successfully"))
            {
                writeLog("Cihaz İle Bağlantı Kuruldu.", Success);
                materialRaisedButton1.Enabled = true;
                getGPTAsync();
            }
            else
            {
                */
            runCommand("Library/QSaharaServer.exe", "-p \\\\.\\" + comPort + " -s 13:" + programmerPrefix + "/" + selectedProgrammer, appPath, delegate (object s, EventArgs a)
                {
                    Process p = (Process)s;
                    if (p.StandardOutput.ReadToEnd().Contains("Sahara protocol completed"))
                    {
                        writeLog("Cihaz İle Bağlantı Kuruldu.", Success);
                        materialRaisedButton1.Enabled = true;
                        getGPTAsync();
                    }
                    else
                    {
                        writeLog("Programmer Hatalı, Cihazı Yeniden EDL Moduna Alınız.", Error);
                        COMPortReloader.Enabled = true;
                        COMPortReloader.Start();
                        deviceList.Enabled = true;
                        programmerList.Enabled = true;
                        materialRaisedButton4.Enabled = true;
                        materialRaisedButton1.Enabled = false;
                    }
                });
            //}
        }

        private void programmerList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (lastItemChecked != null && lastItemChecked.Checked
         && lastItemChecked != programmerList.Items[e.Index]) lastItemChecked.Checked = false;

            lastItemChecked = programmerList.Items[e.Index];
        }
        private async void programmerList_ItemCheckedAsync(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Group.Name == "r" && e.Item.Checked)
            {
                await downloadProgrammerAsync(e.Item.Text);
                programmerPrefix = "DownloadedProgrammers";
            }
            else
            {
                programmerPrefix = "CustomProgrammers";
            }
        }

        void writeLog(string text, int type)
        {
            string color = "#4169e1";
            DateTime date = DateTime.Now;
            if (type == Success)
            {
                color = "#007f00";
            }
            else if (type == Error)
            {
                color = "#e50000";
            }
            else if (type == Warning)
            {
                color = "#cc8400";
            }

            if (log.InvokeRequired)
            {
                log.Invoke(new MethodInvoker(delegate ()
                {
                    var index = log.Rows.Add();
                    log.Rows[index].DefaultCellStyle.ForeColor = ColorTranslator.FromHtml(color);
                    log.Rows[index].Cells[0].Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    log.Rows[index].Cells[1].Value = type;
                    log.Rows[index].Cells[2].Value = date.ToString("dd.MM.yyyy HH:mm:ss");
                    log.Rows[index].Cells[3].Value = text;
                }));
            }
            else
            {
                var index = log.Rows.Add();
                log.Rows[index].DefaultCellStyle.ForeColor = ColorTranslator.FromHtml(color);
                log.Rows[index].Cells[0].Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                log.Rows[index].Cells[1].Value = type;
                log.Rows[index].Cells[2].Value = date.ToString("dd.MM.yyyy HH:mm:ss");
                log.Rows[index].Cells[3].Value = text;
            }
            log.Sort(unixtime, ListSortDirection.Descending);
            System.Threading.Thread.Sleep(1000);
        }
        private void log_disableSelection(Object sender, EventArgs e)
        {
            log.ClearSelection();
        }
        private void parts_disableSelection(Object sender, EventArgs e)
        {
            partList.ClearSelection();
        }
    }
}