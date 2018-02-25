using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using System.ComponentModel; // CancelEventArgs

namespace waifu2x_chainer_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var dirInfo = new DirectoryInfo(App.directory);
            var langlist = dirInfo.GetFiles("UILang.*.xaml");
            string[] langcodelist = new string[langlist.Length];
            for (int i = 0; i < langlist.Length; i++)
            {
                var fn_parts = langlist[i].ToString().Split('.');
                langcodelist[i] = fn_parts[1];
            }

            foreach (var langcode in langcodelist)
            {
                MenuItem mi = new MenuItem();
                mi.Tag = langcode;
                mi.Header = langcode;
                mi.Click += new RoutedEventHandler(MenuItem_Style_Click);
                menuLang.Items.Add(mi);
            }
            foreach (MenuItem item in menuLang.Items)
            {
                if (item.Tag.ToString().Equals(CultureInfo.CurrentUICulture.Name))
                {
                    item.IsChecked = true;
                }
            }
            // 設定をファイルから読み込む
            txtExt.Text = Properties.Settings.Default.informat;

            if (Properties.Settings.Default.output_dir != "null")
            { txtDstPath.Text = Properties.Settings.Default.output_dir; }

            if (Properties.Settings.Default.temporary_dir == "%TEMP%")
            {
               txtTempPath.Text = System.IO.Path.GetTempPath();
            }
            else
            {
               txtTempPath.Text = Properties.Settings.Default.temporary_dir;
            }

            txtWaifu2x_chainerPath.Text = Properties.Settings.Default.waifu2x_chainer_dir;

            if (System.Text.RegularExpressions.Regex.IsMatch(
                Properties.Settings.Default.Device_ID,
                @"^(\d+|-1)$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                txtDevice.Text = Properties.Settings.Default.Device_ID;
            }

            btn64.IsChecked = true;

            if (Properties.Settings.Default.block_size == "256")
            { btn256.IsChecked = true; }
            if (Properties.Settings.Default.block_size == "128")
            { btn128.IsChecked = true; }
            if (Properties.Settings.Default.block_size == "64")
            { btn64.IsChecked = true; }
            if (Properties.Settings.Default.block_size == "32")
            { btn32.IsChecked = true; }

            //btnCUDA.IsChecked = true;
            btnDenoise0.IsChecked = true;

            if (Properties.Settings.Default.noise_level == "3")
            { btnDenoise3.IsChecked = true; }
            if (Properties.Settings.Default.noise_level == "2")
            { btnDenoise2.IsChecked = true; }
            if (Properties.Settings.Default.noise_level == "1")
            { btnDenoise1.IsChecked = true; }
            if (Properties.Settings.Default.noise_level == "0")
            { btnDenoise0.IsChecked = true; }

            btnRGB.IsChecked = true;

            if (Properties.Settings.Default.Arch == "RGB")
            { btnRGB.IsChecked = true; }
            if (Properties.Settings.Default.Arch == "UpRGB")
            { btnUpRGB.IsChecked = true; }
            if (Properties.Settings.Default.Arch == "ResRGB")
            { btnResRGB.IsChecked = true; }

            btnModeScale.IsChecked = true;

            if (Properties.Settings.Default.mode == "scale")
            { btnModeScale.IsChecked = true; }
            if (Properties.Settings.Default.mode == "noise_scale")
            { btnModeNoiseScale.IsChecked = true; }
            if (Properties.Settings.Default.mode == "noise")
            { btnModeNoise.IsChecked = true; }
            if (Properties.Settings.Default.mode == "auto_scale")
            { btnModeAutoScale.IsChecked = true; }

            output_width.Clear();

            if (Properties.Settings.Default.output_width != 0)
            { output_width.Text = Properties.Settings.Default.output_width.ToString(); }

            output_height.Clear();

            if (Properties.Settings.Default.output_height != 0)
            { output_height.Text = Properties.Settings.Default.output_height.ToString(); }

            if (this.output_width.Text.Trim() != "")
            {
                slider_zoom.IsEnabled = false;
                slider_value.IsEnabled = false;
            }

            if (this.output_height.Text.Trim() != "")
            {
                slider_zoom.IsEnabled = false;
                slider_value.IsEnabled = false;
            }

            txtOutQuality.Clear();

            if (Properties.Settings.Default.OutQuality != 0)
            { txtOutQuality.Text = Properties.Settings.Default.OutQuality.ToString(); }

            checkTTAmode.IsChecked = Properties.Settings.Default.TTAmode;
            checkDandD.IsChecked = Properties.Settings.Default.DandD_check;
            checkAspect_ratio_keep.IsChecked = Properties.Settings.Default.Aspect_ratio_keep;
            checkSoundBeep.IsChecked = Properties.Settings.Default.SoundBeep;
            checkAlphachannel_ImageMagick.IsChecked = Properties.Settings.Default.Alphachannel_ImageMagick;
            checkStore_output_dir.IsChecked = Properties.Settings.Default.store_output_dir;
            checkOutput_no_overwirit.IsChecked = Properties.Settings.Default.output_no_overwirit;

            slider_value.Text = Properties.Settings.Default.scale_ratio;
            slider_zoom.Value = double.Parse(Properties.Settings.Default.scale_ratio);

            //cbTTA.IsChecked = false;

            txtOutExt.SelectedValue = Properties.Settings.Default.outformat;
            ComboAlphachannel_background.SelectedValue = Properties.Settings.Default.Alphachannel_background;

        }
        public static StringBuilder waifu2xbinary = new StringBuilder("python waifu2x.py");

        public static StringBuilder param_src= new StringBuilder("");
        public static StringBuilder param_dst = new StringBuilder("");
        public static StringBuilder param_dst_dd = new StringBuilder("");
        public static StringBuilder param_informat = new StringBuilder("*.jpg *.jpeg *.png *.bmp *.tif *.tiff");
        //public static StringBuilder param_outformat = new StringBuilder("png");
        public static StringBuilder param_mag = new StringBuilder("2");
        public static StringBuilder param_denoise = new StringBuilder("");
        public static StringBuilder param_arch = new StringBuilder(@"-a 2");
        //public static StringBuilder param_device = new StringBuilder("-p gpu");
        public static StringBuilder param_block = new StringBuilder("-l 64");
        public static StringBuilder param_mode = new StringBuilder("noise_scale");
        public static StringBuilder param_device = new StringBuilder("");
        public static StringBuilder param_TTAmode = new StringBuilder("");
        public static StringBuilder param_outquality = new StringBuilder("");
        public static StringBuilder param_outformat = new StringBuilder(".png");
        public static StringBuilder param_tempdir = new StringBuilder("%TEMP%");
        public static StringBuilder param_waifu2x_chainer_dir = new StringBuilder("C:\\waifu2x-chainer");
        public static StringBuilder param_Alphachannel_background = new StringBuilder("none");

        public static StringBuilder random32 = new StringBuilder("");
        public static StringBuilder Not_Aspect_ratio_keep_argument = new StringBuilder("");
        public static StringBuilder Aspect_ratio_keep_argument = new StringBuilder("");
        public static StringBuilder flagAlphachannel_ImageMagick = new StringBuilder("");
        public static StringBuilder flagOutput_no_overwirit = new StringBuilder("");

        public static bool DandD_Mode = false;
        public static int FileCount = (0);

        //public static StringBuilder param_tta = new StringBuilder("-t 0");
        public static Process pHandle = new Process();
        public static ProcessStartInfo psinfo = new ProcessStartInfo();

        public static StringBuilder console_buffer = new StringBuilder();
        public static StringBuilder waifu2x_bat = new StringBuilder("");
        // public static bool flagAbort = false;
        public static bool queueFlag = false;

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
                
            // 設定を保存
            if (txtExt.Text.Trim() != "")
            {
               Properties.Settings.Default.informat = txtExt.Text;
            } else
            {
               Properties.Settings.Default.informat = "*.jpg *.jpeg *.png *.bmp *.tif *.tiff";
            }

            // 前回出力したパスを記憶する
            if (checkStore_output_dir.IsChecked == true)
            {
                if (txtDstPath.Text.Trim() != "")
                {
                    Properties.Settings.Default.output_dir = txtDstPath.Text;
                } else
                {
                    Properties.Settings.Default.output_dir = "null";
                }

            }
            else
            {
                Properties.Settings.Default.output_dir = "null";
            }

            Properties.Settings.Default.outformat = txtOutExt.SelectedValue.ToString();
            Properties.Settings.Default.Alphachannel_background = ComboAlphachannel_background.SelectedValue.ToString();

            if (System.Text.RegularExpressions.Regex.IsMatch(
                txtOutQuality.Text,
                @"^\d+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                Properties.Settings.Default.OutQuality = double.Parse(txtOutQuality.Text);
            }
            else
            {
                Properties.Settings.Default.OutQuality = 0;
            }

            if (Directory.Exists(this.txtTempPath.Text))
            {
                Properties.Settings.Default.temporary_dir = txtTempPath.Text;
            }
            else
            {
                Properties.Settings.Default.temporary_dir = System.IO.Path.GetTempPath();
            }

            if (Directory.Exists(this.txtWaifu2x_chainerPath.Text))
            {
                Properties.Settings.Default.waifu2x_chainer_dir = txtWaifu2x_chainerPath.Text;
            }
            else
            {
                Properties.Settings.Default.waifu2x_chainer_dir = "C:\\waifu2x-chainer";
            }

            // Properties.Settings.Default.Device_ID = txtDevice.Text;

            if (System.Text.RegularExpressions.Regex.IsMatch(
                txtDevice.Text,
                @"^(\d+|-1)$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
               Properties.Settings.Default.Device_ID = txtDevice.Text;
            } else 
            {
               Properties.Settings.Default.Device_ID = "Unspecified";
            }

            string param_block_r = param_block.ToString().Replace("-l ", "");
            Properties.Settings.Default.block_size = param_block_r;
            
            string param_denoise_r = param_denoise.ToString().Replace("-n ", "");
            Properties.Settings.Default.noise_level = param_denoise_r;
            
            if (param_arch.ToString().Trim() == "-a 0")
            {Properties.Settings.Default.Arch = "RGB";}
            if (param_arch.ToString().Trim() == "-a 1")
            {Properties.Settings.Default.Arch = "UpRGB";}
            if (param_arch.ToString().Trim() == "-a 2")
            {Properties.Settings.Default.Arch = "ResRGB";}

            Properties.Settings.Default.mode = param_mode.ToString();

            Properties.Settings.Default.TTAmode = Convert.ToBoolean(checkTTAmode.IsChecked);
            Properties.Settings.Default.DandD_check = Convert.ToBoolean(checkDandD.IsChecked);
            Properties.Settings.Default.Aspect_ratio_keep = Convert.ToBoolean(checkAspect_ratio_keep.IsChecked);
            Properties.Settings.Default.SoundBeep = Convert.ToBoolean(checkSoundBeep.IsChecked);
            Properties.Settings.Default.Alphachannel_ImageMagick = Convert.ToBoolean(checkAlphachannel_ImageMagick.IsChecked);
            Properties.Settings.Default.store_output_dir = Convert.ToBoolean(checkStore_output_dir.IsChecked);
            Properties.Settings.Default.output_no_overwirit = Convert.ToBoolean(checkOutput_no_overwirit.IsChecked);

            if (System.Text.RegularExpressions.Regex.IsMatch(
                slider_value.Text,
                @"^\d+(\.\d+)?$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
               Properties.Settings.Default.scale_ratio = slider_value.Text;
            } else 
            {
               Properties.Settings.Default.scale_ratio = "2";
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(
                output_width.Text,
                @"^\d+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                Properties.Settings.Default.output_width = double.Parse(output_width.Text);
            }
            else
            {
                Properties.Settings.Default.output_width = 0;
            }


            if (System.Text.RegularExpressions.Regex.IsMatch(
                output_height.Text,
                @"^\d+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                Properties.Settings.Default.output_height = double.Parse(output_height.Text);
            }
            else
            {
                Properties.Settings.Default.output_height = 0;
            }

            Properties.Settings.Default.Save();

            try
            {
                KillProcessTree(pHandle);
            }
            catch (Exception) { /*Nothing*/ }

            if (waifu2x_bat.ToString() != "")
            {
                if (File.Exists(waifu2x_bat.ToString()))
                { System.IO.File.Delete(@waifu2x_bat.ToString()); }
            }
        }

        private void OnMenuHelpClick(object sender, RoutedEventArgs e)
        {
            string msg =
                "This is a multilingual graphical user-interface\n" +
                "for the waifu2x-chainer commandline program.\n" +
                "You need a working copy of waifu2x-chainer first\n" +
                "then copy everything from the GUI archive to\n" +
                "waifu2x-chainer folder.\n" +
                "DO NOT rename any subdirectories inside waifu2x-chainer folder\n" +
                "To make a translation, copy one of the bundled xaml file\n" +
                "then edit the copy with a text editor.\n" +
                "Whenever you see a language code like en-US, change it to\n" +
                "the target language code like zh-TW, ja-JP.\n" +
                "The filename needs to be changed too.";
            MessageBox.Show(msg);
        }

        private void OnMenuVersionClick(object sender, RoutedEventArgs e)
        {
            string msg =
                "Multilingual GUI for waifu2x-chainer\n" +
                "nanashi (2018)\n" +
                "Version 1.0.5\n" +
                "BuildDate: 25 Feb,2018\n" +
                "License: Do What the Fuck You Want License";
            MessageBox.Show(msg);
        }

        private void OnBtnSrc(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg= new OpenFileDialog();
            if (fdlg.ShowDialog() == true)
            {
                this.txtSrcPath.Text = fdlg.FileName;
            }
        }

        private void OnSrcClear(object sender, RoutedEventArgs e)
        {
            this.txtSrcPath.Clear();
        }

        private void OnBtnDst(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Filter = "PNG Image | *.png";
            fdlg.DefaultExt = "png";
            if (fdlg.ShowDialog() == true)
            {
                this.txtDstPath.Text = fdlg.FileName;
            }
        }

        private void OnDstClear(object sender, RoutedEventArgs e)
        {
            this.txtDstPath.Clear();
        }

        private void OnFormatReset(object sender, RoutedEventArgs e)
        {
            this.txtExt.Text = "*.jpg *.jpeg *.png *.bmp *.tif *.tiff";
        }

        private void MenuItem_Style_Click(object sender, RoutedEventArgs e)
        {
            foreach(MenuItem item in menuLang.Items)
            {
                item.IsChecked = false;
            }
            MenuItem mi = (MenuItem)sender;
            mi.IsChecked = true;
            App.Instance.SwitchLanguage(mi.Tag.ToString());
        }

        private void On_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects= DragDropEffects.None;
            }
            
            e.Handled = true;
        }

        private async void On_SrcDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fn = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                if (checkDandD.IsChecked == false)
                { this.txtSrcPath.Text = fn[0];}
                
                if (checkDandD.IsChecked == true) if (this.btnRun.IsEnabled == true)
                {
                    this.btnRun.IsEnabled = false;
                    DandD_Mode = true;
                    this.txtSrcPath.Clear();
                    Guid g = System.Guid.NewGuid();
                    random32.Clear();
                    random32.Append(g.ToString("N").Substring(0, 32));
                    waifu2x_bat.Clear();
                    waifu2x_bat.Append("waifu2x_" + random32.ToString() + ".bat");
                    StreamWriter sw = new System.IO.StreamWriter(
                               @waifu2x_bat.ToString(),
                               false
                               // ,
                               // System.Text.Encoding.GetEncoding("utf-8")
                               );
                    sw.WriteLine("@echo off");
                    sw.WriteLine("chcp 65001 >nul");
                    sw.WriteLine("cd \"" + this.txtWaifu2x_chainerPath.Text + "\"");
                    // waifu2x-chainerがインストールされているかチェックする
                    sw.WriteLine("Python waifu2x.py -h >nul 2>&1");
                    sw.WriteLine("if not \"%ERRORLEVEL%\"==\"0\" echo waifu2x-chainer is not installed. && del \"%~dp0" + waifu2x_bat.ToString() + "\" && exit");
                    // ImageMagick 6がインストールされているかチェックする
                    sw.WriteLine("convert.exe -version | find \"ImageMagick 6\" >nul 2>&1");
                    sw.WriteLine("if not \"%ERRORLEVEL%\"==\"0\" echo ImageMagick 6 is not installed. && del \"%~dp0" + waifu2x_bat.ToString() + "\" && exit");
                    sw.WriteLine("set \"ProcessedCount=0\"");
                    FileCount = 0;
                    string stCsvData = txtExt.Text;
                    string[] stArrayData = stCsvData.Split(' ');
                    for (int i = 0; i < fn.Length; i++)
                    {
                            if (Directory.Exists(fn[i]))
                            {
                                for (int f = 0; f < stArrayData.Length; f++)
                                {
                                    await Task.Run(() => FileCount = FileCount + Directory.GetFiles(fn[i], stArrayData[f], SearchOption.AllDirectories).Length);
                                }
                            }
                            if (File.Exists(fn[i]))
                            { FileCount++; }
                        string list = fn[i];
                        string list2 = list.Replace("%", "%%");
                        sw.WriteLine("set list_path=\"" + list2 + "\"&&call :list_Allocation");
                    }
                    sw.Close(); ;
                    OnRun(sender,e);
                }
            }
        }

        private void On_DstDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fn = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.txtDstPath.Text = fn[0];
            }
        }

        private void On_TempDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fn = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.txtTempPath.Text = fn[0];
            }
        }

        private void On_waifu2x_chainerDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fn = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.txtWaifu2x_chainerPath.Text = fn[0];
            }
        }

        private void OnSetModeChecked(object sender, RoutedEventArgs e)
        {
            gpDenoise.IsEnabled = true;
            if (btnModeNoise.IsChecked == false) 
            {
               output_width.IsEnabled = true;
               output_height.IsEnabled = true;
               if (this.output_height.Text.Trim() == "") if (this.output_width.Text.Trim() == "") 
               {
                  slider_zoom.IsEnabled = true;
                  slider_value.IsEnabled = true;
               }
            }

            param_mode.Clear();
            RadioButton optsrc = sender as RadioButton;
            param_mode.Append(optsrc.Tag.ToString());
            if (btnModeScale.IsChecked == true)
            { gpDenoise.IsEnabled = false;}
            
            if (btnModeNoise.IsChecked == true)
            { 
              output_width.IsEnabled = false;
              output_height.IsEnabled = false;
              slider_zoom.IsEnabled = false;
              slider_value.IsEnabled = false;
            }
        }

        private void OutputSize_TextChanged(object sender, EventArgs e)
        {

            if (btnModeNoise.IsChecked == false) if (this.output_width.Text.Trim() != "")
                {
                slider_zoom.IsEnabled = false;
                slider_value.IsEnabled = false;
            }

            if (btnModeNoise.IsChecked == false) if (this.output_height.Text.Trim() != "")
            {
                slider_zoom.IsEnabled = false;
                slider_value.IsEnabled = false;
            }
            if (btnModeNoise.IsChecked == false) if (this.output_height.Text.Trim() == "") if (this.output_width.Text.Trim() == "")
                {
                    slider_zoom.IsEnabled = true;
                    slider_value.IsEnabled = true;
                }
        }

        private void OnDenoiseChecked(object sender, RoutedEventArgs e)
        {
            param_denoise.Clear();
            RadioButton optsrc= sender as RadioButton;
            param_denoise.Append(optsrc.Tag.ToString());
        }

        private void OnArchChecked(object sender, RoutedEventArgs e)
        {
            param_arch.Clear();
            RadioButton optsrc= sender as RadioButton;
            param_arch.Append(optsrc.Tag.ToString());
        }

        /*private void OnDeviceChecked(object sender, RoutedEventArgs e)
        {
            param_device.Clear();
            RadioButton optsrc= sender as RadioButton;
            param_device.Append(optsrc.Tag.ToString());
        }
        */

        private void OnBlockChecked(object sender, RoutedEventArgs e)
        {
            param_block.Clear();
            RadioButton optsrc= sender as RadioButton;
            param_block.Append(optsrc.Tag.ToString());
        }

        /*private void OnTTAChecked(object sender, RoutedEventArgs e)
        {
            param_tta.Clear();
            CheckBox optsrc= sender as CheckBox;
            if (optsrc.IsChecked.Value)
            {
                param_tta.Append(optsrc.Tag.ToString());
            }
            
        }
        */
        private void OnConsoleDataRecv(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {

                console_buffer.Append(e.Data);
                console_buffer.Append(Environment.NewLine);
                if (queueFlag) return;
                queueFlag = true;
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    queueFlag = false;
                    CLIOutput.Focus();
                    this.CLIOutput.AppendText(e.Data);
                    this.CLIOutput.AppendText(Environment.NewLine);
                    CLIOutput.Select(CLIOutput.Text.Length, 0);
                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
            }
            
        }

        private void KillProcessTree(System.Diagnostics.Process process)
        {
          string taskkill = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "taskkill.exe");
          using (var procKiller = new System.Diagnostics.Process()) {
            procKiller.StartInfo.FileName = taskkill;
            procKiller.StartInfo.Arguments = string.Format("/PID {0} /T /F", process.Id);
            procKiller.StartInfo.CreateNoWindow = true;
            procKiller.StartInfo.UseShellExecute = false;
            procKiller.Start();
            procKiller.WaitForExit();
          }
        }
        
        private void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                pHandle.CancelOutputRead();
            }
            catch (Exception)
            {
                //No need to throw
                //throw;
            }

            if (waifu2x_bat.ToString() != "")
            {
                if (File.Exists(waifu2x_bat.ToString()))
                { System.IO.File.Delete(@waifu2x_bat.ToString()); }
            }

            pHandle.Close();
            Dispatcher.BeginInvoke(new Action(delegate
            {
                if (checkSoundBeep.IsChecked == true) if (this.btnRun.IsEnabled == false)
                { System.Media.SystemSounds.Beep.Play(); }
                
                this.btnAbort.IsEnabled = false;
                this.btnRun.IsEnabled = true;
                //this.CLIOutput.Text = console_buffer.ToString();

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
            // flagAbort = false;
            DandD_Mode = false;
        }

        private async void OnAbort(object sender, RoutedEventArgs e)
        {
            this.btnAbort.IsEnabled = false;
            try
            {
                pHandle.CancelOutputRead();
            }
            catch (Exception) { /*Nothing*/ }

            if (!pHandle.HasExited)
            {
                try
                {
                    await Task.Run(() => KillProcessTree(pHandle));
                }
                catch (Exception) { /*Nothing*/ }

                if (waifu2x_bat.ToString() != "")
                    {
                        if (File.Exists(waifu2x_bat.ToString()))
                        { System.IO.File.Delete(@waifu2x_bat.ToString()); }
                    }


                // flagAbort = true;
                this.CLIOutput.Clear();
            }
        }
        
        public void Errormessage(string x)
        { 
          if (DandD_Mode == true)
          {
             System.IO.File.Delete(@waifu2x_bat.ToString());
             DandD_Mode = false;
          }
          System.Media.SystemSounds.Beep.Play();
          MessageBox.Show(@x);
          btnAbort.IsEnabled = false;
          btnRun.IsEnabled = true;
          return; 
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {

            // Sets Source
            // The source must be a file or folder that exists
            if (DandD_Mode == false) if (File.Exists(this.txtSrcPath.Text) || Directory.Exists(this.txtSrcPath.Text))
            {
                if (this.txtSrcPath.Text.Trim() == "") //When source path is empty, replace with current folder
                {
                    param_src.Clear();
                    //param_src.Append("-i ");
                    //param_src.Append("\"");
                    param_src.Append(App.directory);
                    //param_src.Append("\"");
                }
                else
                {
                    param_src.Clear();
                    //param_src.Append("-i ");
                    //param_src.Append("\"");
                    param_src.Append(this.txtSrcPath.Text);
                    //param_src.Append("\"");
                }
            }
            else
            {
                Errormessage(@"The source folder or file does not exists!");
                return;
            }
            // 一時フォルダを設定する
            param_tempdir.Clear();
            if (this.txtTempPath.Text.Trim() == "") txtTempPath.Text = System.IO.Path.GetTempPath();
            
            // 一時フォルダのパスの末尾に\が無かったら付ける
            txtTempPath.Text = System.Text.RegularExpressions.Regex.Replace(
              txtTempPath.Text, @"^(.+?)\\*$", @"$1\",
              System.Text.RegularExpressions.RegexOptions.Singleline
            );
            
            if (Directory.Exists(this.txtTempPath.Text))
            {
                param_tempdir.Append(txtTempPath.Text);
            }
            else
            {
                Errormessage("Temporary folder is missing!");
                return;
            }

            // waifu2x-chainerのパスを設定する
            param_waifu2x_chainer_dir.Clear();
            if (File.Exists(this.txtWaifu2x_chainerPath.Text + "\\waifu2x.py"))
            {
                param_waifu2x_chainer_dir.Append(txtWaifu2x_chainerPath.Text);
            }
            else
            {
                Errormessage("waifu2x-chainer folder is missing!");
                return;
            }

            //  出力形式を決定する
            param_outformat.Clear();
            param_outformat.Append(txtOutExt.Text);

            // アルファチャンネルの背景色を設定する
            param_Alphachannel_background.Clear();
            if (ComboAlphachannel_background.Text != "none")
            {
                param_Alphachannel_background.Append("   convert.exe %Image_path% ^( +clone -alpha opaque -fill " + ComboAlphachannel_background.Text + " -colorize 100%% ^) +swap -geometry +0+0 -compose Over -composite -alpha off png24:\"%Temporary_dir%%Temporary_Name%_RGB.png\"");
            }
            else
            {
                param_Alphachannel_background.Append("   convert.exe %Image_path% -channel RGB -combine -alpha off png24:\"%Temporary_dir%%Temporary_Name%_RGB.png\"");
            }

            // D&D処理時に出力先フォルダが見つからなければ出力先をクリアする
            if (DandD_Mode == true)
            {
                if (this.txtDstPath.Text.Trim() != "") if (Directory.Exists(this.txtDstPath.Text)==false)
                {
                    this.txtDstPath.Clear();
                }

            }

            // 数字が入力されてなかったらクリアする

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                output_width.Text,
                @"^\d+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                output_width.Clear();
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                output_height.Text,
                @"^\d+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                output_height.Clear();
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(
                txtDevice.Text,
                @"^(\d+|-1)$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                param_device.Clear();
                param_device.Append("-g ");
                param_device.Append(txtDevice.Text);
            }
            else
            {
                param_device.Clear();
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(
                txtOutQuality.Text,
                @"^\d+$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                param_outquality.Clear();
                param_outquality.Append("-quality ");
                param_outquality.Append(txtOutQuality.Text);
            }
            else
            {
                param_outquality.Clear();
                txtOutQuality.Clear();
            }
            
            // 入力拡張子の指定書式が正しいかチェックする
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                txtExt.Text,
                @"^\*\.\w{2,4}( \*\.\w{2,4})*$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                // 機械翻訳で出した文なので通じるのか不安
                Errormessage("Input filename extension specification format is incorrect!");
                return;
            }
            // logをクリアする
            this.CLIOutput.Clear();

            // TTAモードの引数を追加する
            param_TTAmode.Clear();
            if (checkTTAmode.IsChecked == true)
            { param_TTAmode.Append("-t"); }

            // 縦横比を保たない引数を追加する
            Not_Aspect_ratio_keep_argument.Clear();
            if (checkAspect_ratio_keep.IsChecked == false)
            { Not_Aspect_ratio_keep_argument.Append("!"); }

            // 縦横比を保つ引数を追加する
            Aspect_ratio_keep_argument.Clear();
            if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() != "") if (checkAspect_ratio_keep.IsChecked == true)
            { Aspect_ratio_keep_argument.Append("1"); }

            // アルファチャンネルをImageMagickで分離して処理するかどうか判断するフラグを追加
            flagAlphachannel_ImageMagick.Clear();
            flagAlphachannel_ImageMagick.Append(checkAlphachannel_ImageMagick.IsChecked.ToString());

            // 出力ファイルを上書きするかどうか判断するフラグを追加
            flagOutput_no_overwirit.Clear();
            flagOutput_no_overwirit.Append(checkOutput_no_overwirit.IsChecked.ToString());


            // Set Destination
            param_dst_dd.Clear();
            if (this.txtDstPath.Text.Trim() == "")
            {
                //

                param_dst.Clear();
                // nanashi Append
                //param_dst.Append("-o ");
                if (DandD_Mode == false)
                {
                    if (File.Exists(this.txtSrcPath.Text))
                    { param_dst.Append("\""); }
                    System.IO.DirectoryInfo hDirInfo = System.IO.Directory.GetParent(this.txtSrcPath.Text);
                    param_dst.Append(hDirInfo.FullName);
                    param_dst.Append("\\");
                    if (File.Exists(this.txtSrcPath.Text))
                    { param_dst.Append(System.IO.Path.GetFileNameWithoutExtension(this.txtSrcPath.Text)); }
                    if (Directory.Exists(this.txtSrcPath.Text))
                    { param_dst.Append(System.IO.Path.GetFileName(this.txtSrcPath.Text)); }
                }

                if (param_arch.ToString() == "-a 0")
                   { param_dst.Append("(RGB)"); }
                    if (param_arch.ToString() == "-a 1")
                    { param_dst.Append("(UpRGB)"); }
                    if (param_arch.ToString() == "-a 2")
                    { param_dst.Append("(ResRGB)"); }
            }
            else
            {
                param_dst.Clear();
                //param_dst.Append("-o ");
                if (DandD_Mode == false)
                {
                    if (File.Exists(this.txtSrcPath.Text))
                    { param_dst.Append("\""); }
                    
                    param_dst.Append(this.txtDstPath.Text);
                    
                    if (Directory.Exists(this.txtSrcPath.Text))
                    { param_dst.Append("\\"); }
                    
                    // 入力先が画像で出力先がフォルダの場合
                    if (File.Exists(this.txtSrcPath.Text)) if (Directory.Exists(this.txtDstPath.Text))
                    {
                      param_dst.Append("\\");
                      param_dst.Append(System.IO.Path.GetFileNameWithoutExtension(this.txtSrcPath.Text)); 
                      param_dst.Append(".png");
                    }
                    
                    if (File.Exists(this.txtSrcPath.Text))
                    { param_dst.Append("\""); }
                }

                if (DandD_Mode == true)
                {
                    if (Directory.Exists(this.txtDstPath.Text))
                    {
                        param_dst_dd.Append(System.IO.Path.GetFullPath(this.txtDstPath.Text));
                        param_dst_dd.Replace("%", "%%");
                        param_dst_dd.Append("\\");
                    }
                }
            }

            // Set input format
            param_informat.Clear();
            //param_informat.Append("-l ");
            param_informat.Append(this.txtExt.Text);
            //param_informat.Append(@":");
            //param_informat.Append(this.txtExt.Text.ToUpper());

            // Set output format
            //param_outformat.Clear();
            //param_outformat.Append("-e ");
            //param_outformat.Append(this.txtOExt.Text);

            // Set scale ratio

            param_mag.Clear();
            param_mag.Append("-s ");
            param_mag.Append("%scale_ratio%");



            // Set mode
            if (param_mode.ToString() == "noise_scale")
            {
                if (this.txtDstPath.Text.Trim() == "")
                {
                    param_dst.Append("(noise_scale)");

                    if (param_denoise.ToString() == "-n 0")
                    { param_dst.Append("(Level0)"); }
                    if (param_denoise.ToString() == "-n 1")
                    { param_dst.Append("(Level1)"); }
                    if (param_denoise.ToString() == "-n 2")
                    { param_dst.Append("(Level2)"); }
                    if (param_denoise.ToString() == "-n 3")
                    { param_dst.Append("(Level3)"); }

                    if (checkTTAmode.IsChecked == true)
                    { param_dst.Append("(tta)"); }

                    if (this.output_width.Text.Trim() == "") if (this.output_height.Text.Trim() == "")
                        {
                            param_dst.Append("(x");
                            param_dst.Append(this.slider_zoom.Value.ToString());
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() == "")
                        {
                            param_dst.Append("(width ");
                            param_dst.Append(this.output_width.Text);
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() == "") if (this.output_height.Text.Trim() != "")
                        {
                            param_dst.Append("(height ");
                            param_dst.Append(this.output_height.Text);
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() != "")
                        {
                            param_dst.Append("(");
                            if (checkAspect_ratio_keep.IsChecked == true)
                            { param_dst.Append("within "); }

                            param_dst.Append(this.output_width.Text);
                            param_dst.Append("x");
                            param_dst.Append(this.output_height.Text);
                            param_dst.Append(")");
                        }
                }
            }
            if (param_mode.ToString() == "auto_scale")
            {
                if (this.txtDstPath.Text.Trim() == "")
                {
                    param_dst.Append("(auto_scale)");

                    if (param_denoise.ToString() == "-n 0")
                    { param_dst.Append("(Level0)"); }
                    if (param_denoise.ToString() == "-n 1")
                    { param_dst.Append("(Level1)"); }
                    if (param_denoise.ToString() == "-n 2")
                    { param_dst.Append("(Level2)"); }
                    if (param_denoise.ToString() == "-n 3")
                    { param_dst.Append("(Level3)"); }

                    if (checkTTAmode.IsChecked == true)
                    { param_dst.Append("(tta)"); }

                    if (this.output_width.Text.Trim() == "") if (this.output_height.Text.Trim() == "")
                        {
                            param_dst.Append("(x");
                            param_dst.Append(this.slider_zoom.Value.ToString());
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() == "")
                        {
                            param_dst.Append("(width ");
                            param_dst.Append(this.output_width.Text);
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() == "") if (this.output_height.Text.Trim() != "")
                        {
                            param_dst.Append("(height ");
                            param_dst.Append(this.output_height.Text);
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() != "")
                        {
                            param_dst.Append("(");
                            if (checkAspect_ratio_keep.IsChecked == true)
                            { param_dst.Append("within "); }

                            param_dst.Append(this.output_width.Text);
                            param_dst.Append("x");
                            param_dst.Append(this.output_height.Text);
                            param_dst.Append(")");
                        }
                }
            }
            if (param_mode.ToString() == "noise")
            {
                param_mag.Clear();
                if (this.txtDstPath.Text.Trim() == "")
                {
                    param_dst.Append("(noise)");

                    if (param_denoise.ToString() == "-n 0")
                    { param_dst.Append("(Level0)"); }
                    if (param_denoise.ToString() == "-n 1")
                    { param_dst.Append("(Level1)"); }
                    if (param_denoise.ToString() == "-n 2")
                    { param_dst.Append("(Level2)"); }
                    if (param_denoise.ToString() == "-n 3")
                    { param_dst.Append("(Level3)"); }

                    if (checkTTAmode.IsChecked == true)
                    { param_dst.Append("(tta)"); }
                }
            }
            if (param_mode.ToString() == "scale")
            {
                // param_denoise.Clear();
                if (this.txtDstPath.Text.Trim() == "")
                {
                    param_dst.Append("(scale)");

                    if (checkTTAmode.IsChecked == true)
                    { param_dst.Append("(tta)"); }

                    if (this.output_width.Text.Trim() == "") if (this.output_height.Text.Trim() == "")
                        {
                        param_dst.Append("(x");
                        param_dst.Append(this.slider_zoom.Value.ToString());
                        param_dst.Append(")");
                    }
                    if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() == "")
                        {
                            param_dst.Append("(width ");
                            param_dst.Append(this.output_width.Text);
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() == "") if (this.output_height.Text.Trim() != "")
                        {
                            param_dst.Append("(height ");
                            param_dst.Append(this.output_height.Text);
                            param_dst.Append(")");
                        }
                    if (this.output_width.Text.Trim() != "") if (this.output_height.Text.Trim() != "")
                        {
                            param_dst.Append("(");
                            if (checkAspect_ratio_keep.IsChecked == true)
                            { param_dst.Append("within "); }

                            param_dst.Append(this.output_width.Text);
                            param_dst.Append("x");
                            param_dst.Append(this.output_height.Text);
                            param_dst.Append(")");
                        }
                }
            }

            if (DandD_Mode == false) if (this.txtDstPath.Text.Trim() == "")
            {
                if (File.Exists(this.txtSrcPath.Text))
                { param_dst.Append(".png\""); }
                if (Directory.Exists(this.txtSrcPath.Text))
                { param_dst.Append("\\"); }
            }

            this.btnRun.IsEnabled = false;
            this.btnAbort.IsEnabled = true;

            if (DandD_Mode == false)
            {
                Guid g = System.Guid.NewGuid();
                random32.Clear();
                random32.Append(g.ToString("N").Substring(0, 32));
                waifu2x_bat.Clear();
                waifu2x_bat.Append("waifu2x_" + random32.ToString() + ".bat");
            }
            // %をエスケープする
            param_src.Replace("%", "%%");
            param_dst.Replace("%", "%%");

            if (DandD_Mode == false) if (File.Exists(this.txtSrcPath.Text))
            {
                // Assemble parameters
                string TextBox1 = "@echo off\r\n" +
                "chcp 65001 >nul\r\n" +
                "cd \"" + param_waifu2x_chainer_dir.ToString() + "\"\r\n" +
                 // waifu2x-chainerがインストールされているかチェックする
                 "Python waifu2x.py -h >nul 2>&1\r\n" +
                 "if not \"%ERRORLEVEL%\"==\"0\" echo waifu2x-chainer is not installed. && del \"%~dp0" + waifu2x_bat.ToString() + "\" && exit\r\n" +
                 // ImageMagick 6がインストールされているかチェックする
                 "convert.exe -version | find \"ImageMagick 6\" >nul 2>&1\r\n" +
                 "if not \"%ERRORLEVEL%\"==\"0\" echo ImageMagick 6 is not installed. && del \"%~dp0" + waifu2x_bat.ToString() + "\" && exit\r\n" +
                 "set Image_path=\"" + param_src.ToString() + "\"\r\n" +
                 ":waifu2x_run\r\n" +
                 "setlocal\r\n" +
                 "FOR %%A IN (" + param_dst.ToString() + ") DO set \"OUTPUT_Name=%%~nA" + param_outformat.ToString() + "\"\r\n" +
                 "FOR %%A IN (" + param_dst.ToString() + ") DO set \"Output_dir=%%~dpA\"\r\n" +
                 // bat共通の処理
                 "set \"Output_no_overwirit=" + flagOutput_no_overwirit.ToString() + "\"\r\n" +
                 "if \"%Output_no_overwirit%\"==\"True\" if exist \"%Output_dir%%OUTPUT_Name%\" goto waifu2x_run_skip\r\n" +
                 "set \"Temporary_dir=" + param_tempdir.ToString() + "\"\r\n" +
                 "set \"Alphachannel_ImageMagick=" + flagAlphachannel_ImageMagick.ToString() + "\"\r\n" +
                 "set \"keep_aspect_ratio=" + Aspect_ratio_keep_argument.ToString() + "\"\r\n" +
                 "set \"scale_ratio=" + this.slider_zoom.Value.ToString() + "\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" set \"output_width=" + this.output_width.Text + "\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" set \"output_height=" + this.output_height.Text + "\"\r\n" +
                 "if not \"%output_width%\"==\"\" if not \"%output_height%\"==\"\" set \"resize_argument=-resize %output_width%x%output_height%" + Not_Aspect_ratio_keep_argument.ToString() + "\"\r\n" +
                 "if not \"%output_width%\"==\"\" if \"%output_height%\"==\"\" set \"resize_argument=-resize %output_width%x\"\r\n" +
                 "if \"%output_width%\"==\"\" if not \"%output_height%\"==\"\" set \"resize_argument=-resize x%output_height%\"\r\n" +
                 "FOR %%A IN (%Image_path%) DO set \"Image_ext=%%~xA\"\r\n" +
                 "if \"%Alphachannel_ImageMagick%\"==\"True\" if /i \"%Image_ext%\"==\".png\" identify.exe -format \"%%A\" %Image_path% | find \"Blend\"> NUL && set image_alpha=true\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" if not \"%output_width%%output_height%\"==\"\" (\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%w\" %Image_path%') do set \"image_width=%%a\"\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%h\" %Image_path%') do set \"image_height=%%a\"\r\n" +
                 "   set scale_ratio=1\r\n" +
                 "   call :scale_ratio_set\r\n" +
                 ")\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" if not \"%output_width%%output_height%\"==\"\" if \"%image_width%%image_height%\"==\"\" goto waifu2x_run_skip\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" (\r\n" +
                 "   if /i \"%Image_ext%\"==\".jpg\" set jpg=1\r\n" +
                 "   if /i \"%Image_ext%\"==\".jpeg\" set jpg=1\r\n" +
                 ")\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" if \"%jpg%\"==\"1\" set \"Mode=noise_scale " + param_denoise.ToString() + "\"\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" if not \"%jpg%\"==\"1\" set \"Mode=scale\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"auto_scale\" if not \"" + param_mode.ToString() + "\"==\"scale\" (\r\n" +
                 "   set \"Mode=" + param_mode.ToString() + " " + param_denoise.ToString() + "\"\r\n" +
                 ") else (\r\n" +
                 "   set \"Mode=" + param_mode.ToString() + "\r\n" +
                 ")\r\n" +
                 "set \"Output_format=" + param_outformat.ToString() + "\"\r\n" +
                 "for %%i in (.jpg,.jp2,.ppm) do if \"%image_alpha%\"==\"true\" if \"%Output_format%\"==\"%%~i\" set \"alpha_off_argument=^( +clone -alpha opaque -fill white -colorize 100%% ^) +swap -geometry +0+0 -compose Over -composite -alpha off\" >NUL\r\n" +
                 "set \"Temporary_Name=" + random32.ToString() + "_%RANDOM%_%RANDOM%_%RANDOM%_\"\r\n" +
                 // アルファチャンネルが無い場合は普通に拡大
                 "if not \"%image_alpha%\"==\"true\" " + waifu2xbinary.ToString() + " " + "-i" + " " + "%Image_path%" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" 2>&1\r\n" +
                 // 元のファイル名がユニコードでも処理出来るようにテンポフォルダに別名でコピーする
                 "if not \"%image_alpha%\"==\"true\" if not \"%ERRORLEVEL%\"==\"0\" (\r\n" +
                 "   copy /Y %Image_path% \"%Temporary_dir%%Temporary_Name%%Image_ext%\" >nul\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + "-i" + " " + "\"%Temporary_dir%%Temporary_Name%%Image_ext%\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" 2>&1\r\n" +
                 ")\r\n" +
                 "if not \"%image_alpha%\"==\"true\" if not \"%output_width%%output_height%\"==\"\" (\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ")\r\n" +
                 "if not \"%image_alpha%\"==\"true\" if \"%output_width%%output_height%\"==\"\" if /I \"" + param_outformat.ToString() + "\"==\".png\" (\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ") else (\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ")\r\n" +
                 // アルファチャンネルが有ったらImageMagickで分離して拡大
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 param_Alphachannel_background.ToString() + "\r\n" +
                 "   convert.exe %Image_path% -channel matte -separate +matte png24:\"%Temporary_dir%%Temporary_Name%_alpha.png\" >NUL\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%k\" \"%Temporary_dir%%Temporary_Name%_alpha.png\"') do set \"image_alpha_color=%%a\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + " -i" + " " + "\"%Temporary_dir%%Temporary_Name%_RGB.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\" 2>&1\r\n" +
                 ")\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha_color%\"==\"1\" for /f \"delims=\" %%a in ('identify.exe -format \"%%w\" \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"') do set \"image_2x_width=%%a\"\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha_color%\"==\"1\" for /f \"delims=\" %%a in ('identify.exe -format \"%%h\" \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"') do set \"image_2x_height=%%a\"\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 "   if \"%image_alpha_color%\"==\"1\" convert.exe \"%Temporary_dir%%Temporary_Name%_alpha.png\" -sample %image_2x_width%x%image_2x_height%! \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\" >NUL\r\n" +
                 "   if not \"%image_alpha_color%\"==\"1\" " + waifu2xbinary.ToString() + " " + "-i" + " " + "\"%Temporary_dir%%Temporary_Name%_alpha.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\" 2>&1\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\" \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\" -compose CopyOpacity -composite %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_RGB.png\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha.png\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\" >NUL\r\n" +
                 ")\r\n" +
                 // 出力形式がアルファチャンネルをサポートしてないので最初に非透過pngにする
                 "if not \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 "   convert.exe %Image_path% %alpha_off_argument% png24:\"%Temporary_dir%%Temporary_Name%_alpha_off.png\" >nul\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + " -i" + " " + "\"%Temporary_dir%%Temporary_Name%_alpha_off.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" 2>&1\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha_off.png\" >nul\r\n" +
                 ")\r\n" +
                 "if exist \"%Temporary_dir%%Temporary_Name%%Image_ext%\" del \"%Temporary_dir%%Temporary_Name%%Image_ext%\" >nul\"\r\n" +
                 "if exist \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" del \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" >nul\"\r\n" +
                 ":waifu2x_run_skip\r\n" +
                 "endlocal\r\n" +
                 "set Image_path=\r\n" +


                 "del \"%~dp0" + waifu2x_bat.ToString() + "\"\r\n" +
                 "exit /b\r\n" +
                 "\"\r\n" +
                 ":scale_ratio_set\r\n" +
                 "if \"%image_width%%image_height%\"==\"\" exit /b\r\n" +
                 "if not \"%keep_aspect_ratio%\"==\"1\" (\r\n" +
                 "   call :scale_ratio_set_width\r\n" +
                 "   call :scale_ratio_set_height\r\n" +
                 "   exit /b\r\n" +
                 ")\r\n" +
                 "set /a image_height_nx=%image_height%*%scale_ratio%\r\n" +
                 "set /a image_width_nx=%image_width%*%scale_ratio%\r\n" +
                 "if %output_height% LEQ %image_height_nx% exit /b\r\n" +
                 "if %output_width% LEQ %image_width_nx% exit /b\r\n" +
                 "set /a scale_ratio=%scale_ratio%*2\r\n" +
                 "goto scale_ratio_set\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":scale_ratio_set_width\r\n" +
                 "if \"%output_width%\"==\"\" exit /b\r\n" +
                 "set /a image_width_nx=%image_width%*%scale_ratio%\r\n" +
                 "if not %output_width% LEQ %image_width_nx% (\r\n" +
                 "   set /a scale_ratio=%scale_ratio%*2\r\n" +
                 "   goto scale_ratio_set_width\r\n" +
                 ")\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":scale_ratio_set_height\r\n" +
                 "if \"%output_height%\"==\"\" exit /b\r\n" +
                 "set /a image_height_nx=%image_height%*%scale_ratio%\r\n" +
                 "if not %output_height% LEQ %image_height_nx% (\r\n" +
                 "   set /a scale_ratio=%scale_ratio%*2\r\n" +
                 "   goto scale_ratio_set_height\r\n" +
                 ")\r\n" +
                 "exit /b\r\n" 
            ;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(
                @waifu2x_bat.ToString(),
                false
                // ,
                // System.Text.Encoding.GetEncoding("utf-8")
                );
                sw.Write(TextBox1);
                sw.Close();

                    psinfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                    psinfo.Arguments = string.Format(@"/c {0}", waifu2x_bat.ToString());
                    psinfo.RedirectStandardError = true;
                    psinfo.RedirectStandardOutput = true;
                    psinfo.UseShellExecute = false;
                    psinfo.WorkingDirectory = App.directory;
                    psinfo.CreateNoWindow = true;
                    psinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pHandle.StartInfo = psinfo;
                    pHandle.EnableRaisingEvents = true;
                    pHandle.OutputDataReceived += new DataReceivedEventHandler(OnConsoleDataRecv);
                    //pHandle.ErrorDataReceived += new DataReceivedEventHandler(OnConsoleDataRecv);
                    pHandle.Exited += new EventHandler(OnProcessExit);

                // Starts working
                    console_buffer.Clear();


            }
            if (DandD_Mode == false) if (Directory.Exists(this.txtSrcPath.Text))
            {

                
                // Assemble parameters

                    //param_tta.ToString()
                    // Setup ProcessStartInfo
                       string TextBox1 = "@echo off\r\n" +
                       "chcp 65001 >nul\r\n" +
                       "cd \"" + param_waifu2x_chainer_dir.ToString() + "\"\r\n" +
                       // waifu2x-chainerがインストールされているかチェックする
                       "Python waifu2x.py -h >nul 2>&1\r\n" +
                       "if not \"%ERRORLEVEL%\"==\"0\" echo waifu2x-chainer is not installed. && del \"%~dp0" + waifu2x_bat.ToString() + "\" && exit\r\n" +
                        // ImageMagick 6がインストールされているかチェックする
                        "convert.exe -version | find \"ImageMagick 6\" >nul 2>&1\r\n" +
                        "if not \"%ERRORLEVEL%\"==\"0\" echo ImageMagick 6 is not installed. && del \"%~dp0" + waifu2x_bat.ToString() + "\" && exit\r\n" +
                        "set \"OutputFolder=" + param_dst.ToString() + "\"\r\n" +
                        //"set OutputFolder=%OutputFolder:\"=%\r\n" +
                        "for %%A IN (\"" + param_src.ToString() + "\") do set \"A=%%~aA\"\r\n" +
                        "IF not \"%A:~0,1%\"==\"d\" goto end\r\n" +
                        "if \"" + param_informat.ToString() +"\"==\"\" goto end\r\n" +
                        "set \"str=" + param_src.ToString() + "\"\r\n" +
                        "set \"len=0\"\r\n" +
                        "call :word_count\r\n" +
                        "if %len% neq 3 set /a len+=1\r\n" +
                        "set \"FileCount=0\"\r\n" +
                        "set \"ProcessedCount=0\"\r\n" +
                        "pushd \"" + param_src.ToString() + "\"\r\n" +
                        "FOR /f \"DELIMS=\" %%A IN ('dir " + param_informat.ToString() + " /A-D /S /B ^| find /c /v \"\"') DO SET \"FileCount=%%A\"\r\n" +
                        "popd\r\n" +
                        "for /r \"" + param_src.ToString() + "\" %%i in (" + param_informat.ToString() + ") do set Image_path=\"%%i\"&&call :waifu2x_run\r\n" +
                        "\r\n" +
                        "goto end\r\n" +
                        "\r\n" +
                        ":word_count\r\n" +
                        "if not \"%str%\"==\"\" (\r\n" +
                        "    set \"str=%str:~1%\"\r\n" +
                        "    set /a len=%len%+1\r\n" +
                        "    goto :word_count\r\n" +
                        ")\r\n" +
                        "exit /b\r\n" +
                        "\r\n" +
                        ":waifu2x_run\r\n" +
                        "echo progress %ProcessedCount%/%FileCount%\r\n" +
                        // "cls\r\n" +
                        "setlocal\r\n" +
                        "FOR %%A IN (%Image_path%) DO set \"relative_path=%%~dpA\"\r\n" +
                        "FOR %%A IN (%Image_path%) DO set \"OUTPUT_Name=%%~nA" + param_outformat.ToString() + "\"\r\n" +
                        "call set \"relative_path=%%relative_path:~%len%%%\"\r\n" +
                        "set \"Output_dir=%OutputFolder%%relative_path%\"\r\n" +
                        "if not exist \"%Output_dir%\" mkdir \"%Output_dir%\"\r\n" +


                 // bat共通の処理
                 "set \"Output_no_overwirit=" + flagOutput_no_overwirit.ToString() + "\"\r\n" +
                 "if \"%Output_no_overwirit%\"==\"True\" if exist \"%Output_dir%%OUTPUT_Name%\" goto waifu2x_run_skip\r\n" +
                 "set \"Temporary_dir=" + param_tempdir.ToString() + "\"\r\n" +
                 "set \"Alphachannel_ImageMagick=" + flagAlphachannel_ImageMagick.ToString() + "\"\r\n" +
                 "set \"keep_aspect_ratio=" + Aspect_ratio_keep_argument.ToString() + "\"\r\n" +
                 "set \"scale_ratio=" + this.slider_zoom.Value.ToString() + "\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" set \"output_width=" + this.output_width.Text + "\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" set \"output_height=" + this.output_height.Text + "\"\r\n" +
                 "if not \"%output_width%\"==\"\" if not \"%output_height%\"==\"\" set \"resize_argument=-resize %output_width%x%output_height%" + Not_Aspect_ratio_keep_argument.ToString() + "\"\r\n" +
                 "if not \"%output_width%\"==\"\" if \"%output_height%\"==\"\" set \"resize_argument=-resize %output_width%x\"\r\n" +
                 "if \"%output_width%\"==\"\" if not \"%output_height%\"==\"\" set \"resize_argument=-resize x%output_height%\"\r\n" +
                 "FOR %%A IN (%Image_path%) DO set \"Image_ext=%%~xA\"\r\n" +
                 "if \"%Alphachannel_ImageMagick%\"==\"True\" if /i \"%Image_ext%\"==\".png\" identify.exe -format \"%%A\" %Image_path% | find \"Blend\"> NUL && set image_alpha=true\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" if not \"%output_width%%output_height%\"==\"\" (\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%w\" %Image_path%') do set \"image_width=%%a\"\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%h\" %Image_path%') do set \"image_height=%%a\"\r\n" +
                 "   set scale_ratio=1\r\n" +
                 "   call :scale_ratio_set\r\n" +
                 ")\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" if not \"%output_width%%output_height%\"==\"\" if \"%image_width%%image_height%\"==\"\" goto waifu2x_run_skip\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" (\r\n" +
                 "   if /i \"%Image_ext%\"==\".jpg\" set jpg=1\r\n" +
                 "   if /i \"%Image_ext%\"==\".jpeg\" set jpg=1\r\n" +
                 ")\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" if \"%jpg%\"==\"1\" set \"Mode=noise_scale " + param_denoise.ToString() + "\"\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" if not \"%jpg%\"==\"1\" set \"Mode=scale\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"auto_scale\" if not \"" + param_mode.ToString() + "\"==\"scale\" (\r\n" +
                 "   set \"Mode=" + param_mode.ToString() + " " + param_denoise.ToString() + "\"\r\n" +
                 ") else (\r\n" +
                 "   set \"Mode=" + param_mode.ToString() + "\r\n" +
                 ")\r\n" +
                 "set \"Output_format=" + param_outformat.ToString() + "\"\r\n" +
                 "for %%i in (.jpg,.jp2,.ppm) do if \"%image_alpha%\"==\"true\" if \"%Output_format%\"==\"%%~i\" set \"alpha_off_argument=^( +clone -alpha opaque -fill white -colorize 100%% ^) +swap -geometry +0+0 -compose Over -composite -alpha off\"\r\n" +
                 "set \"Temporary_Name=" + random32.ToString() + "_%RANDOM%_%RANDOM%_%RANDOM%_\"\r\n" +
                 // アルファチャンネルが無い場合は普通に拡大
                 "if not \"%image_alpha%\"==\"true\" " + waifu2xbinary.ToString() + " " + "-i" + " " + "%Image_path%" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" >nul\r\n" +
                 // 元のファイル名がユニコードでも処理出来るようにテンポフォルダに別名でコピーする
                 "if not \"%image_alpha%\"==\"true\" if not \"%ERRORLEVEL%\"==\"0\" (\r\n" +
                 "   copy /Y %Image_path% \"%Temporary_dir%%Temporary_Name%%Image_ext%\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + "-i" + " " + "\"%Temporary_dir%%Temporary_Name%%Image_ext%\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\"\r\n" +
                 ") >nul\r\n" +
                 "if not \"%image_alpha%\"==\"true\" if not \"%output_width%%output_height%\"==\"\" (\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ")\r\n" +
                 "if not \"%image_alpha%\"==\"true\" if \"%output_width%%output_height%\"==\"\" if /I \"" + param_outformat.ToString() + "\"==\".png\" (\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ") else (\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ")\r\n" +
                 // アルファチャンネルが有ったらImageMagickで分離して拡大
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 param_Alphachannel_background.ToString() + "\r\n" +
                 "   convert.exe %Image_path% -channel matte -separate +matte png24:\"%Temporary_dir%%Temporary_Name%_alpha.png\"\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%k\" \"%Temporary_dir%%Temporary_Name%_alpha.png\"') do set \"image_alpha_color=%%a\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + " -i" + " " + "\"%Temporary_dir%%Temporary_Name%_RGB.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"\r\n" +
                 ") >nul\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha_color%\"==\"1\" for /f \"delims=\" %%a in ('identify.exe -format \"%%w\" \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"') do set \"image_2x_width=%%a\"\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha_color%\"==\"1\" for /f \"delims=\" %%a in ('identify.exe -format \"%%h\" \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"') do set \"image_2x_height=%%a\"\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 "   if \"%image_alpha_color%\"==\"1\" convert.exe \"%Temporary_dir%%Temporary_Name%_alpha.png\" -sample %image_2x_width%x%image_2x_height%! \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\"\r\n" +
                 "   if not \"%image_alpha_color%\"==\"1\" " + waifu2xbinary.ToString() + " " + "-i" + " " + "\"%Temporary_dir%%Temporary_Name%_alpha.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\"\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\" \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\" -compose CopyOpacity -composite %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_RGB.png\"\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha.png\"\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\"\r\n" +
                 ") >nul\r\n" +
                 // 出力形式がアルファチャンネルをサポートしてないので最初に非透過pngにする
                 "if not \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 "   convert.exe %Image_path% %alpha_off_argument% png24:\"%Temporary_dir%%Temporary_Name%_alpha_off.png\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + " -i" + " " + "\"%Temporary_dir%%Temporary_Name%_alpha_off.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\"\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha_off.png\"\r\n" +
                 ") >nul\r\n" +
                 "if exist \"%Temporary_dir%%Temporary_Name%%Image_ext%\" del \"%Temporary_dir%%Temporary_Name%%Image_ext%\" >nul\"\r\n" +
                 "if exist \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" del \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" >nul\"\r\n" +
                 ":waifu2x_run_skip\r\n" +
                 "endlocal\r\n" +
                 "set Image_path=\r\n" +



                        "set /a ProcessedCount=%ProcessedCount%+1\r\n" +
                        "exit /b\r\n" +
                        "\r\n" +
                        ":scale_ratio_set\r\n" +
                        "if \"%image_width%%image_height%\"==\"\" exit /b\r\n" +
                        "if not \"%keep_aspect_ratio%\"==\"1\" (\r\n" +
                        "   call :scale_ratio_set_width\r\n" +
                        "   call :scale_ratio_set_height\r\n" +
                        "   exit /b\r\n" +
                        ")\r\n" +
                        "set /a image_height_nx=%image_height%*%scale_ratio%\r\n" +
                        "set /a image_width_nx=%image_width%*%scale_ratio%\r\n" +
                        "if %output_height% LEQ %image_height_nx% exit /b\r\n" +
                        "if %output_width% LEQ %image_width_nx% exit /b\r\n" +
                        "set /a scale_ratio=%scale_ratio%*2\r\n" +
                        "goto scale_ratio_set\r\n" +
                        "exit /b\r\n" +
                        "\r\n" +
                        ":scale_ratio_set_width\r\n" +
                        "if \"%output_width%\"==\"\" exit /b\r\n" +
                        "set /a image_width_nx=%image_width%*%scale_ratio%\r\n" +
                        "if not %output_width% LEQ %image_width_nx% (\r\n" +
                        "   set /a scale_ratio=%scale_ratio%*2\r\n" +
                        "   goto scale_ratio_set_width\r\n" +
                        ")\r\n" +
                        "exit /b\r\n" +
                        "\r\n" +
                        ":scale_ratio_set_height\r\n" +
                        "if \"%output_height%\"==\"\" exit /b\r\n" +
                        "set /a image_height_nx=%image_height%*%scale_ratio%\r\n" +
                        "if not %output_height% LEQ %image_height_nx% (\r\n" +
                        "   set /a scale_ratio=%scale_ratio%*2\r\n" +
                        "   goto scale_ratio_set_height\r\n" +
                        ")\r\n" +
                        "exit /b\r\n" +
                        "\r\n" +
                        ":end\r\n" +
                        "echo progress %ProcessedCount%/%FileCount%\r\n" +
                        "del \"%~dp0" + waifu2x_bat.ToString() + "\"\r\n" +
                        "exit /b\r\n"
                   ;
                   System.IO.StreamWriter sw = new System.IO.StreamWriter(
                   @waifu2x_bat.ToString(),
                   false
                   // ,
                   // System.Text.Encoding.GetEncoding("utf-8")
                   );
                   sw.Write(TextBox1);
                   sw.Close();

                    psinfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                    psinfo.Arguments = string.Format(@"/c {0}", waifu2x_bat.ToString());
                    psinfo.RedirectStandardError = true;
                    psinfo.RedirectStandardOutput = true;
                    psinfo.UseShellExecute = false;
                    psinfo.WorkingDirectory = App.directory;
                    psinfo.CreateNoWindow = true;
                    psinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pHandle.StartInfo = psinfo;
                    pHandle.EnableRaisingEvents = true;
                    pHandle.OutputDataReceived += new DataReceivedEventHandler(OnConsoleDataRecv);
                    //pHandle.ErrorDataReceived += new DataReceivedEventHandler(OnConsoleDataRecv);
                    pHandle.Exited += new EventHandler(OnProcessExit);

                    console_buffer.Clear();
                    //console_buffer.Append(full_param);
                    //console_buffer.Append("\n");

            }
            if (DandD_Mode == true)
            { 
                // Assemble parameters

                //param_tta.ToString()
                // Setup ProcessStartInfo
                string TextBox1 = "goto end\r\n" +
                 "\r\n" +
                 ":list_Allocation\r\n" +
                 "set FileCount=" + FileCount + "\r\n" +
                 "set \"OutputFolder=" + param_dst_dd.ToString() + "\"\r\n" +
                 "for %%A IN (%list_path%) do set \"A=%%~aA\"\r\n" +
                 "IF not \"%A:~0,1%\"==\"d\" set list_path_attribute=file\r\n" +
                 "IF \"%A:~0,1%\"==\"d\" set list_path_attribute=folder\r\n" +
                 // ファイルの処理
                 "if \"%list_path_attribute%\"==\"file\" set Image_path=%list_path%\r\n" +
                 "if \"%list_path_attribute%\"==\"file\" call :waifu2x_run\r\n" +
                 // フォルダの処理
                 "if \"%list_path_attribute%\"==\"folder\" if \"%OutputFolder%\"==\"\" for %%A in (%list_path%) do set \"str=%%~A\"\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" if not \"%OutputFolder%\"==\"\" for %%A in (%list_path%) do set \"str=%%~dpA\"\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" set \"len=0\"\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" call :word_count\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" for /r %list_path% %%i in (" + param_informat.ToString() + ") do (\r\n" +
                 "   set \"OUTPUT_Name=%%~ni" + param_outformat.ToString() + "\"\r\n" +
                 "   set Image_path=\"%%i\"\r\n" +
                 "   call :waifu2x_run\r\n" +
                 ")\r\n" +
                 "exit /b\r\n" +
                 ":word_count\r\n" +
                 "if not \"%str%\"==\"\" (\r\n" +
                 "    set \"str=%str:~1%\"\r\n" +
                 "    set /a len=%len%+1\r\n" +
                 "    goto :word_count\r\n" +
                 ")\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":waifu2x_run\r\n" +
                 "echo progress %ProcessedCount%/%FileCount%\r\n" +
                 "setlocal\r\n" +
                 // ファイルの処理
                 "if \"%list_path_attribute%\"==\"file\" for %%A IN (%list_path%) do set \"OUTPUT_Name=%%~nA" + param_dst.ToString() + param_outformat.ToString() + "\"\r\n" +
                 "if \"%list_path_attribute%\"==\"file\" if \"%OutputFolder%\"==\"\" for %%A IN (%list_path%) DO set \"Output_dir=%%~dpA\"\r\n" +
                 "if \"%list_path_attribute%\"==\"file\" if not \"%OutputFolder%\"==\"\" set \"Output_dir=%OutputFolder%\"\r\n" +
                 //フォルダの処理
                 "if \"%list_path_attribute%\"==\"folder\" if \"%OutputFolder%\"==\"\" for %%A IN (%list_path%) DO set \"OutputFolder=%%~A" + param_dst.ToString() + "\\\"\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" FOR %%A IN (%Image_path%) DO set \"relative_path=%%~dpA\"\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" call set \"relative_path=%%relative_path:~%len%%%\"\r\n" +
                 "if \"%list_path_attribute%\"==\"folder\" set \"Output_dir=%OutputFolder%%relative_path%\"\r\n" +
                 "if not exist \"%Output_dir%\" mkdir \"%Output_dir%\"\r\n" +


                 // bat共通の処理
                 "cd \"" + param_waifu2x_chainer_dir.ToString() + "\"\r\n" +
                 "set \"Output_no_overwirit=" + flagOutput_no_overwirit.ToString() + "\"\r\n" +
                 "if \"%Output_no_overwirit%\"==\"True\" if exist \"%Output_dir%%OUTPUT_Name%\" goto waifu2x_run_skip\r\n" +
                 "set \"Temporary_dir=" + param_tempdir.ToString() + "\"\r\n" +
                 "set \"Alphachannel_ImageMagick=" + flagAlphachannel_ImageMagick.ToString() + "\"\r\n" +
                 "set \"keep_aspect_ratio=" + Aspect_ratio_keep_argument.ToString() + "\"\r\n" +
                 "set \"scale_ratio=" + this.slider_zoom.Value.ToString() + "\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" set \"output_width=" + this.output_width.Text + "\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" set \"output_height=" + this.output_height.Text + "\"\r\n" +
                 "if not \"%output_width%\"==\"\" if not \"%output_height%\"==\"\" set \"resize_argument=-resize %output_width%x%output_height%" + Not_Aspect_ratio_keep_argument.ToString() + "\"\r\n" +
                 "if not \"%output_width%\"==\"\" if \"%output_height%\"==\"\" set \"resize_argument=-resize %output_width%x\"\r\n" +
                 "if \"%output_width%\"==\"\" if not \"%output_height%\"==\"\" set \"resize_argument=-resize x%output_height%\"\r\n" +
                 "FOR %%A IN (%Image_path%) DO set \"Image_ext=%%~xA\"\r\n" +
                 "if \"%Alphachannel_ImageMagick%\"==\"True\" if /i \"%Image_ext%\"==\".png\" identify.exe -format \"%%A\" %Image_path% | find \"Blend\"> NUL && set image_alpha=true\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" if not \"%output_width%%output_height%\"==\"\" (\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%w\" %Image_path%') do set \"image_width=%%a\"\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%h\" %Image_path%') do set \"image_height=%%a\"\r\n" +
                 "   set scale_ratio=1\r\n" +
                 "   call :scale_ratio_set\r\n" +
                 ")\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"noise\" if not \"%output_width%%output_height%\"==\"\" if \"%image_width%%image_height%\"==\"\" goto waifu2x_run_skip\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" (\r\n" +
                 "   if /i \"%Image_ext%\"==\".jpg\" set jpg=1\r\n" +
                 "   if /i \"%Image_ext%\"==\".jpeg\" set jpg=1\r\n" +
                 ")\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" if \"%jpg%\"==\"1\" set \"Mode=noise_scale " + param_denoise.ToString() + "\"\r\n" +
                 "if \"" + param_mode.ToString() + "\"==\"auto_scale\" if not \"%jpg%\"==\"1\" set \"Mode=scale\"\r\n" +
                 "if not \"" + param_mode.ToString() + "\"==\"auto_scale\" if not \"" + param_mode.ToString() + "\"==\"scale\" (\r\n" +
                 "   set \"Mode=" + param_mode.ToString() + " " + param_denoise.ToString() + "\"\r\n" +
                 ") else (\r\n" +
                 "   set \"Mode=" + param_mode.ToString() + "\r\n" +
                 ")\r\n" +
                 "set \"Output_format=" + param_outformat.ToString() + "\"\r\n" +
                 "for %%i in (.jpg,.jp2,.ppm) do if \"%image_alpha%\"==\"true\" if \"%Output_format%\"==\"%%~i\" set \"alpha_off_argument=^( +clone -alpha opaque -fill white -colorize 100%% ^) +swap -geometry +0+0 -compose Over -composite -alpha off\"\r\n" +
                 "set \"Temporary_Name=" + random32.ToString() + "_%RANDOM%_%RANDOM%_%RANDOM%_\"\r\n" +
                 // アルファチャンネルが無い場合は普通に拡大
                 "if not \"%image_alpha%\"==\"true\" " + waifu2xbinary.ToString() + " " + "-i" + " " + "%Image_path%" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" >nul\r\n" +
                 // 元のファイル名がユニコードでも処理出来るようにテンポフォルダに別名でコピーする
                 "if not \"%image_alpha%\"==\"true\" if not \"%ERRORLEVEL%\"==\"0\" (\r\n" +
                 "   copy /Y %Image_path% \"%Temporary_dir%%Temporary_Name%%Image_ext%\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + "-i" + " " + "\"%Temporary_dir%%Temporary_Name%%Image_ext%\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\"\r\n" +
                 ") >nul\r\n" +
                 "if not \"%image_alpha%\"==\"true\" if not \"%output_width%%output_height%\"==\"\" (\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ")\r\n" +
                 "if not \"%image_alpha%\"==\"true\" if \"%output_width%%output_height%\"==\"\" if /I \"" + param_outformat.ToString() + "\"==\".png\" (\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ") else (\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 ")\r\n" +
                 // アルファチャンネルが有ったらImageMagickで分離して拡大
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 param_Alphachannel_background.ToString() + "\r\n" +
                 "   convert.exe %Image_path% -channel matte -separate +matte png24:\"%Temporary_dir%%Temporary_Name%_alpha.png\"\r\n" +
                 "   for /f \"delims=\" %%a in ('identify.exe -format \"%%k\" \"%Temporary_dir%%Temporary_Name%_alpha.png\"') do set \"image_alpha_color=%%a\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + " -i" + " " + "\"%Temporary_dir%%Temporary_Name%_RGB.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"\r\n" +
                 ") >nul\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha_color%\"==\"1\" for /f \"delims=\" %%a in ('identify.exe -format \"%%w\" \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"') do set \"image_2x_width=%%a\"\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha_color%\"==\"1\" for /f \"delims=\" %%a in ('identify.exe -format \"%%h\" \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"') do set \"image_2x_height=%%a\"\r\n" +
                 "if \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 "   if \"%image_alpha_color%\"==\"1\" convert.exe \"%Temporary_dir%%Temporary_Name%_alpha.png\" -sample %image_2x_width%x%image_2x_height%! \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\"\r\n" +
                 "   if not \"%image_alpha_color%\"==\"1\" " + waifu2xbinary.ToString() + " " + "-i" + " " + "\"%Temporary_dir%%Temporary_Name%_alpha.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\"\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\" \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\" -compose CopyOpacity -composite %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_RGB.png\"\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_RGB_2x.png\"\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha.png\"\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha_2x.png\"\r\n" +
                 ") >nul\r\n" +
                 // 出力形式がアルファチャンネルをサポートしてないので最初に非透過pngにする
                 "if not \"%alpha_off_argument%\"==\"\" if \"%image_alpha%\"==\"true\" (\r\n" +
                 "   convert.exe %Image_path% %alpha_off_argument% png24:\"%Temporary_dir%%Temporary_Name%_alpha_off.png\"\r\n" +
                 "   " + waifu2xbinary.ToString() + " " + " -i" + " " + "\"%Temporary_dir%%Temporary_Name%_alpha_off.png\"" + " " + "-m %mode%" + " " + param_mag.ToString() + " " + param_arch.ToString() + " " + param_block.ToString() + " " + param_device.ToString() + " " + param_TTAmode.ToString() + " " + "-o \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\"\r\n" +
                 "   convert.exe \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" %resize_argument% " + param_outquality.ToString() + " \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" >NUL\r\n" +
                 "   move /Y \"%Temporary_dir%%Temporary_Name%_penultimate" + param_outformat.ToString() + "\" \"%Output_dir%%OUTPUT_Name%\" >NUL\r\n" +
                 "   del \"%Temporary_dir%%Temporary_Name%_alpha_off.png\"\r\n" +
                 ") >nul\r\n" +
                 "if exist \"%Temporary_dir%%Temporary_Name%%Image_ext%\" del \"%Temporary_dir%%Temporary_Name%%Image_ext%\" >nul\"\r\n" +
                 "if exist \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" del \"%Temporary_dir%%Temporary_Name%_BefConvExt.png\" >nul\"\r\n" +
                 ":waifu2x_run_skip\r\n" +
                 "endlocal\r\n" +
                 "set Image_path=\r\n" +



                 "set /a ProcessedCount=%ProcessedCount%+1\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":scale_ratio_set\r\n" +
                 "if \"%image_width%%image_height%\"==\"\" exit /b\r\n" +
                 "if not \"%keep_aspect_ratio%\"==\"1\" (\r\n" +
                 "   call :scale_ratio_set_width\r\n" +
                 "   call :scale_ratio_set_height\r\n" +
                 "   exit /b\r\n" +
                 ")\r\n" +
                 "set /a image_height_nx=%image_height%*%scale_ratio%\r\n" +
                 "set /a image_width_nx=%image_width%*%scale_ratio%\r\n" +
                 "if %output_height% LEQ %image_height_nx% exit /b\r\n" +
                 "if %output_width% LEQ %image_width_nx% exit /b\r\n" +
                 "set /a scale_ratio=%scale_ratio%*2\r\n" +
                 "goto scale_ratio_set\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":scale_ratio_set_width\r\n" +
                 "if \"%output_width%\"==\"\" exit /b\r\n" +
                 "set /a image_width_nx=%image_width%*%scale_ratio%\r\n" +
                 "if not %output_width% LEQ %image_width_nx% (\r\n" +
                 "   set /a scale_ratio=%scale_ratio%*2\r\n" +
                 "   goto scale_ratio_set_width\r\n" +
                 ")\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":scale_ratio_set_height\r\n" +
                 "if \"%output_height%\"==\"\" exit /b\r\n" +
                 "set /a image_height_nx=%image_height%*%scale_ratio%\r\n" +
                 "if not %output_height% LEQ %image_height_nx% (\r\n" +
                 "   set /a scale_ratio=%scale_ratio%*2\r\n" +
                 "   goto scale_ratio_set_height\r\n" +
                 ")\r\n" +
                 "exit /b\r\n" +
                 "\r\n" +
                 ":end\r\n" +
                 "echo progress %ProcessedCount%/%FileCount%\r\n" +
                 "del \"%~dp0" + waifu2x_bat.ToString() + "\"\r\n" +
                 "exit /b\r\n"
            ;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(
                @waifu2x_bat.ToString(),
                true
                // ,
                // System.Text.Encoding.GetEncoding("utf-8")
                );
                sw.Write(TextBox1);
                sw.Close();

                psinfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                psinfo.Arguments = string.Format(@"/c {0}", waifu2x_bat.ToString());
                psinfo.RedirectStandardError = true;
                psinfo.RedirectStandardOutput = true;
                psinfo.UseShellExecute = false;
                psinfo.WorkingDirectory = App.directory;
                psinfo.CreateNoWindow = true;
                psinfo.WindowStyle = ProcessWindowStyle.Hidden;
                pHandle.StartInfo = psinfo;
                pHandle.EnableRaisingEvents = true;
                pHandle.OutputDataReceived += new DataReceivedEventHandler(OnConsoleDataRecv);
                //pHandle.ErrorDataReceived += new DataReceivedEventHandler(OnConsoleDataRecv);
                pHandle.Exited += new EventHandler(OnProcessExit);

                console_buffer.Clear();
                //console_buffer.Append(full_param);
                //console_buffer.Append("\n");
            }
            try
            {
                //MessageBox.Show(full_param);
                bool pState = pHandle.Start();

            }
            catch (Exception)
            {
                KillProcessTree(pHandle);
                MessageBox.Show("Some parameters do not mix well and crashed...");
                //throw;
            }
            
            try
            {
                pHandle.BeginOutputReadLine();
            }
            catch (Exception)
            {
                this.CLIOutput.Clear();
                this.CLIOutput.Text = "BeginOutputReadLine crashed...";
            }
            //pHandle.BeginErrorReadLine();
            //MessageBox.Show("Some parameters do not mix well and crashed...");

            //pHandle.WaitForExit();
            /*
            pHandle.CancelOutputRead();
            pHandle.Close();
            this.btnAbort.IsEnabled = false;
            this.btnRun.IsEnabled = true;
            this.CLIOutput.Text = console_buffer.ToString();
            */

        }
    }
}
