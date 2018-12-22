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
using System.Drawing;
using System.Security.Cryptography;
using System.Xml;
using System.Text.RegularExpressions;

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
            Warning = 4,
            Threads = 0;
        bool Connected = false;

        ListViewItem lastItemChecked;
        List<string> devices = new List<string>();

        Timer COMPortReloader;

        Dictionary<string, Dictionary<string, string>> partitions = new Dictionary<string, Dictionary<string, string>>();

        public Form1()
        {
            Icon = Properties.Resources.onelabs;
            InitializeComponent();
            log.SelectionChanged += log_disableSelection;
            partList.SelectionChanged += parts_disableSelection;
            partList.ColumnHeaderMouseClick += parts_columnHeaderClick;
            backup_type.SelectedIndex = 0;
            data_type.SelectedIndex = 0;
            detectThreadLimit();
            writeLog("Program Başlatıldı.", Default);
            getProgrammersAsync();

            reloadCOMPorts(null, null);
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

        static string s(string input)
        {
            MD5CryptoServiceProvider m = new MD5CryptoServiceProvider();
            byte[] btr = Encoding.UTF8.GetBytes(input);
            btr = m.ComputeHash(btr);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in btr)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        static string d(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        void detectThreadLimit()
        {
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                Threads += int.Parse(item["NumberOfCores"].ToString());
            }
        }

        async Task downloadFileAsync(string file, string output, bool call = false)
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
            else if (call)
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

        public async Task<bool> backupPartitionAsync(string partition, string sector_size, string sector_offset, string filename, string label, string partition_sectors, string partition_number, string size_kb, string sparse, string start_byte, string start_sector)
        {
            bool error = false;

            writeLog("Bölüm Yedekleniyor : " + label, Warning);

            if (partition.Length >= 3 && partition.Substring(partition.Length - 3) == "bak")
            {
                writeLog("Bölüm Yedeklendi : " + label, Default);
                return error;
            }

            if (partition == "userdata" && data_type.SelectedIndex == 1)
            {
                Directory.CreateDirectory(tmpDir + "/data");
                runCommand(appPath + "/Library/make_ext4fs.exe", "-l " + (Int64.Parse(size_kb.Split('.')[0]) * 1000) + " -a data -s userdata.img data/", tmpDir, async delegate (object s, EventArgs a)
                {
                    Process p = (Process)s;
                    string o = await p.StandardOutput.ReadToEndAsync();
                    if (o.Contains("Created filesystem with"))
                    {
                        writeLog("Bölüm Yedeklendi : " + label, Default);
                    }
                    else
                    {
                        writeLog("Bölüm Yedeklenemedi : " + label, Error);
                        error = true;
                    }
                });
                Directory.Delete(tmpDir + "/data", true);
            }
            else
            {
                string xml = createXML("<read SECTOR_SIZE_IN_BYTES=\"" + sector_size + "\" file_sector_offset=\"" + sector_offset + "\" filename=\"" + filename + "\" label=\"" + label + "\" num_partition_sectors=\"" + partition_sectors + "\" physical_partition_number=\"" + partition_number + "\" size_in_KB=\"" + size_kb + "\" sparse=\"" + sparse + "\" start_byte_hex=\"" + start_byte + "\" start_sector=\"" + start_sector + "\"/>");
                string output = await runCommand(appPath + "/Library/OneLoader.exe", "--port=\\\\.\\" + comPort + " --rawxml=" + xml, tmpDir);
                if (output.Contains("All Finished Successfully"))
                {
                    writeLog("Bölüm Yedeklendi : " + label, Default);
                }
                else
                {
                    writeLog("Bölüm Yedeklenemedi : " + label, Error);
                    error = true;
                }
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
            if (devices.Count != tmpDevices.Count)
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
            }
        }

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
                if (!file.Name.Contains("QCOM_USB_Driver")) programmers.Add(new ListViewItem(file.Name, programmerList.Groups[1]));
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
                    string info = null;
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
                    tw.WriteLine(item.Cells[2].Value + " : (" + info + ") " + item.Cells[3].Value);
                }
            }
            writeLog("Log Kaydedildi : log.txt", Success);
        }

        private async void materialRaisedButton2_ClickAsync(object sender, EventArgs e)
        {
            await downloadFileAsync("QCOM_USB_Driver.exe", "QCOM_USB_Driver.exe", true);
        }
        async Task readPartitionsAsync()
        {
            string partitionXml = await runCommand(appPath + "/Library/gp1.exe", "gpt_main0.bin", tmpDir);
            File.WriteAllText(tmpDir + "/partition.xml", partitionXml);
            await runCommand(appPath + "/Library/gp2.exe", "-x partition.xml", tmpDir);

            XmlDocument rawprogramXml = new XmlDocument();
            rawprogramXml.PreserveWhitespace = true;
            rawprogramXml.Load(tmpDir + "/rawprogram0.xml");
            XmlNodeList programs = rawprogramXml.GetElementsByTagName("program");
            File.Delete(tmpDir + "/rawprogram0.xml");

            XmlDocument partXml = new XmlDocument();
            partXml.PreserveWhitespace = true;
            partXml.Load(tmpDir + "/partition.xml");
            XmlNodeList parts = partXml.GetElementsByTagName("partition");

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            string userDataSize = null;

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i].Attributes["label"].Value == "userdata") userDataSize = parts[i].Attributes["size_in_kb"].Value;
            }

            for (int i = 0; i < programs.Count; i++)
            {
                var vars = new Dictionary<string, string>();
                string SECTOR_SIZE_IN_BYTES = programs[i].Attributes["SECTOR_SIZE_IN_BYTES"].Value,
                    file_sector_offset = programs[i].Attributes["file_sector_offset"].Value,
                    label = programs[i].Attributes["label"].Value,
                    filename = label + ".img",
                    num_partition_sectors = programs[i].Attributes["num_partition_sectors"].Value,
                    physical_partition_number = programs[i].Attributes["physical_partition_number"].Value,
                    size_in_KB = (label == "userdata" ? userDataSize : programs[i].Attributes["size_in_KB"].Value),
                    sparse = programs[i].Attributes["sparse"].Value,
                    start_byte_hex = "0x" + programs[i].Attributes["start_byte_hex"].Value.Substring(2).ToUpper(),
                    start_sector = programs[i].Attributes["start_sector"].Value;

                if (label != "PrimaryGPT" && label != "BackupGPT")
                {
                    string[] LBAS = partitionXml.Split(new string[] { "<partition label=\"" + label + "\"" }, StringSplitOptions.None)[0].Split(new string[] { "<!-- First LBA: " }, StringSplitOptions.None);
                    string fLBA = LBAS[LBAS.Count() - 1].Split(new string[] { " / " }, StringSplitOptions.None)[0];

                    start_byte_hex = "0x" + (Int64.Parse(fLBA) * Int64.Parse(SECTOR_SIZE_IN_BYTES)).ToString("X").ToUpper();
                    start_sector = fLBA;
                }

                if (label == "PrimaryGPT") filename = "gpt_main0.bin";
                else if (label == "BackupGPT") filename = "gpt_backup0.bin";

                vars.Add("SECTOR_SIZE_IN_BYTES", SECTOR_SIZE_IN_BYTES);
                vars.Add("file_sector_offset", file_sector_offset);
                vars.Add("label", label);
                vars.Add("filename", filename);
                vars.Add("num_partition_sectors", num_partition_sectors);
                vars.Add("physical_partition_number", physical_partition_number);
                vars.Add("size_in_KB", size_in_KB);
                vars.Add("sparse", sparse);
                vars.Add("start_byte_hex", start_byte_hex);
                vars.Add("start_sector", start_sector);
                partitions.Add(label, vars);
            }
            if (partList.InvokeRequired)
            {
                partList.Invoke(new MethodInvoker(delegate ()
                {
                    partList.Rows.Clear();
                    partList.Refresh();
                    int i = 0;
                    foreach (KeyValuePair<string, Dictionary<string, string>> entry in partitions)
                    {
                        Dictionary<string, string> part = entry.Value;
                        partList.Rows.Add(part["label"], GetBytesReadable(Convert.ToInt64(part["size_in_KB"].Split('.')[0])), 1, part["SECTOR_SIZE_IN_BYTES"], part["file_sector_offset"], part["filename"], part["num_partition_sectors"], part["physical_partition_number"], part["size_in_KB"], part["sparse"], part["start_byte_hex"], part["start_sector"]);
                        if (part["label"] == "PrimaryGPT" || part["label"] == "BackupGPT") partList.Rows[i].Visible = false;
                        i++;
                    }
                    partList.Sort(partList.Columns[0], ListSortDirection.Ascending);
                }
                ));
            }
            else
            {
                partList.Rows.Clear();
                partList.Refresh();
                int i = 0;
                foreach (KeyValuePair<string, Dictionary<string, string>> entry in partitions)
                {
                    Dictionary<string, string> part = entry.Value;
                    partList.Rows.Add(part["label"], GetBytesReadable(Convert.ToInt64(part["size_in_KB"].Split('.')[0])), 1, part["SECTOR_SIZE_IN_BYTES"], part["file_sector_offset"], part["filename"], part["num_partition_sectors"], part["physical_partition_number"], part["size_in_KB"], part["sparse"], part["start_byte_hex"], part["start_sector"]);
                    if (part["label"] == "PrimaryGPT" || part["label"] == "BackupGPT") partList.Rows[i].Visible = false;
                    i++;
                }
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
            if (Directory.Exists(appPath + "/Output")) Directory.Delete(appPath + "/Output", true);

            backup_type.Enabled = false;
            data_type.Enabled = false;
            bool doAction = false,
                    error = false;
            foreach (DataGridViewRow row in partList.Rows)
            {
                if (row.Cells["read"].Value.ToString().Equals("1"))
                {
                    if (error) break;
                    if (!doAction) doAction = true;
                    error = await backupPartitionAsync(row.Cells["partition"].Value.ToString(), row.Cells["sector_size"].Value.ToString(), row.Cells["sector_offset"].Value.ToString(), row.Cells["filename"].Value.ToString(), row.Cells["partition"].Value.ToString(), row.Cells["partition_sectors"].Value.ToString(), row.Cells["partition_number"].Value.ToString(), row.Cells["size_kb"].Value.ToString(), row.Cells["sparse"].Value.ToString(), row.Cells["start_byte"].Value.ToString(), row.Cells["start_sector"].Value.ToString());
                }
            }
            if (doAction && !error)
            {
                List<string> joined = new List<string>();

                if (backup_type.SelectedIndex == 0)
                {
                    // QFIL
                    File.Copy(appPath + "/" + programmerPrefix + "/" + programmerList.CheckedItems[0].Text, tmpDir + "/" + programmerList.CheckedItems[0].Text, true);

                    List<string> rawProgramContent = new List<string>();
                    rawProgramContent.Add("<?xml version=\"1.0\" ?>");
                    rawProgramContent.Add("<data>");
                    rawProgramContent.Add("  <!--NOTE: This is an ** Autogenerated file **-->");
                    rawProgramContent.Add("  <!--NOTE: Copyright caneray@OneTeam (Used OneDumper)-->");

                    foreach (DataGridViewRow row in partList.Rows)
                    {
                        var nm = row.Cells["partition"].Value.ToString();
                        string filename = (nm.Length >= 3 && nm.Substring(nm.Length - 3) == "bak" ? nm.Substring(0, nm.Length - 3) : nm) + ".img";
                        if (nm == "PrimaryGPT") filename = "gpt_main0.bin";
                        else if (nm == "BackupGPT") filename = "gpt_backup0.bin";

                        rawProgramContent.Add("  <program SECTOR_SIZE_IN_BYTES=\"" + row.Cells["sector_size"].Value.ToString() + "\" file_sector_offset=\"" + row.Cells["sector_offset"].Value.ToString() + "\" filename=\"" + (row.Cells["read"].Value.ToString().Equals("1") || row.Cells["partition"].Value.ToString() == "PrimaryGPT" || row.Cells["partition"].Value.ToString() == "BackupGPT" ? filename : "") + "\" label=\"" + row.Cells["partition"].Value.ToString() + "\" num_partition_sectors=\"" + row.Cells["partition_sectors"].Value.ToString() + "\" physical_partition_number=\"" + row.Cells["partition_number"].Value.ToString() + "\" size_in_KB=\"" + (row.Cells["partition"].Value.ToString() == "userdata" ? "0" : row.Cells["size_kb"].Value.ToString()) + "\" sparse=\"" + row.Cells["sparse"].Value.ToString() + "\" start_byte_hex=\"" + row.Cells["start_byte"].Value.ToString() + "\" start_sector=\"" + row.Cells["start_sector"].Value.ToString() + "\"/>");
                    }
                    rawProgramContent.Add("</data>");
                    File.WriteAllBytes(tmpDir + "/rawprogram0.xml", Encoding.Default.GetBytes(String.Join("\r\n", rawProgramContent.ToArray())));
                }
                else
                {
                    // FASTBOOT
                    if (!Directory.Exists(tmpDir + "/FWImages")) Directory.CreateDirectory(tmpDir + "/FWImages");
                    if (!Directory.Exists(appPath + "/Output/FWImages")) Directory.CreateDirectory(appPath + "/Output/FWImages");

                    if (File.Exists(tmpDir + "/gpt_backup0.bin")) File.Delete(tmpDir + "/gpt_backup0.bin");
                    if (File.Exists(tmpDir + "/gpt_both0.bin")) File.Delete(tmpDir + "/gpt_both0.bin");
                    if (File.Exists(tmpDir + "/gpt_main0.bin")) File.Delete(tmpDir + "/gpt_main0.bin");
                    if (File.Exists(tmpDir + "/partition.xml")) File.Delete(tmpDir + "/partition.xml");
                    if (File.Exists(tmpDir + "/patch0.xml")) File.Delete(tmpDir + "/patch0.xml");

                    foreach (string newPath in Directory.GetFiles(tmpDir, "*.*")) File.Move(newPath, newPath.Replace(tmpDir, tmpDir + "/FWImages"));

                    List<string> fbCommands = new List<string>();
                    fbCommands.Add("@shift /0");
                    fbCommands.Add("@shift /0");
                    fbCommands.Add("@shift /0");
                    fbCommands.Add("@shift /0");
                    fbCommands.Add("@shift /0");
                    fbCommands.Add("@shift /0");
                    fbCommands.Add("@echo off&title Stock Rom Yukleyici - caneray@OneTeam&COLOR f0 & mode con cols=79 lines=29");
                    fbCommands.Add("echo Rom Kurulumu Baslatiliyor...");
                    fbCommands.Add("echo Userdata Siliniyor...");
                    fbCommands.Add("fastboot erase userdata");
                    fbCommands.Add("echo Userdata Silindi.");
                    foreach (DataGridViewRow row in partList.Rows)
                    {
                        if (row.Cells["read"].Value.ToString().Equals("1"))
                        {
                            var nm = row.Cells["partition"].Value.ToString();
                            string filename = (nm.Length >= 3 && nm.Substring(nm.Length - 3) == "bak" ? nm.Substring(0, nm.Length - 3) : nm) + ".img";

                            fbCommands.Add("echo " + nm + " Flashlaniyor...");
                            fbCommands.Add("fastboot flash " + nm + " FWImages/" + filename);
                            fbCommands.Add("echo " + nm + " Flashlandi.");
                        }
                    }
                    fbCommands.Add("fastboot reboot");
                    fbCommands.Add("Rom Kurulumu Tamamlandi.");
                    File.WriteAllBytes(tmpDir + "/Flashla.bat", Encoding.Default.GetBytes(String.Join("\r\n", fbCommands.ToArray())));
                }

                Directory.CreateDirectory(appPath + "/Output");
                foreach (string newPath in Directory.GetFiles(tmpDir, "*.*", SearchOption.AllDirectories)) File.Move(newPath, newPath.Replace(tmpDir, appPath + "/Output"));
                Process.Start(appPath + "/Output");

                writeLog("Yedeleme İşlemi Tamamlandı.", Success);
                backup_type.Enabled = true;
                data_type.Enabled = true;

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
        void runCommand(string command, string args, string path, EventHandler e)
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
            if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);

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

            writeLog("Cihaza Bağlanılıyor : " + comPort, Warning);

            if (!Connected)
            {
                writeLog("Sahara Protokolü Başlatılıyor : " + selectedProgrammer, Warning);

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
            }
            else
            {
                writeLog("Cihaz İle Bağlantı Kuruldu.", Success);
                getGPTAsync();
            }
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

        private void parts_columnHeaderClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                foreach (DataGridViewRow row in partList.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                    chk.Value = (chk.Value.ToString() == "1" ? 0 : 1);
                }
            }
        }
    }
}
 