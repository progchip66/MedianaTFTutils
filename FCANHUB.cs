using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileCreater;
using System.Threading;
using UART;
using RS485;
using COMMAND;
using ExtHubComm;
using TESTAKVA;

namespace TFTprog
{
    
    public partial class FormHUB : Form
    {
        private SWORKAKVATEST WORKAKVATEST;
        TFileManager fcreater = new TFileManager();
        SCANHUB CANHUB = new SCANHUB();
        
        SGRAF_FILES GRAF_FILES = new SGRAF_FILES();
        
        bool isInitComboSpeed = false;

        public void LoadDefaultSetting()
        {
            tBresultFilename.Text = Properties.Settings.Default.NameResultFile;
            TBSourseDir.Text = Properties.Settings.Default.DirGrafFiles;
            tBresultPath.Text = Properties.Settings.Default.DirResultGrafFile;
            tBcodefilename.Text = Properties.Settings.Default.CodeFilename;

            tBgrafDIR.Text=Properties.Settings.Default.FolderGRAF;


            tBstartAdr.Text = Properties.Settings.Default.StartFLASHadr;
            tBendAdr.Text = Properties.Settings.Default.EndFLASHadr;
            cBoxEnMess.Checked = Properties.Settings.Default.EnServMess;

            /*        else
                    {
                        index = LBoxInterface.SelectedIndex;
                        Properties.Settings.Default.Interface = LBoxInterface.Items[index].ToString();
                    }

                    /*  tBbasefile.Text = Properties.Settings.Default.NameBaseFile;
                      tBinsFile.Text = Properties.Settings.Default.NameInsFile;
                      tBadrins.Text = Properties.Settings.Default.StartInsAddr;*/

        }

 



        public void SaveDefaultSetting()
        {
            Properties.Settings.Default.NameResultFile = tBresultFilename.Text;
            Properties.Settings.Default.DirGrafFiles = TBSourseDir.Text;
            Properties.Settings.Default.DirResultGrafFile = tBresultPath.Text;
            Properties.Settings.Default.CodeFilename = tBcodefilename.Text;

            Properties.Settings.Default.StartFLASHadr= tBstartAdr.Text;
            Properties.Settings.Default.EndFLASHadr= tBendAdr.Text;
            Properties.Settings.Default.EnServMess = cBoxEnMess.Checked;

            Properties.Settings.Default.Save();

        }

        public void EnDisComponent(bool En)
        {


            if (En)
            {
                PBFileToMem.Value = 0;
                PBFileToMem.Maximum = 100;
                PBFileToMem.Step = 1;

                PBwrFilePro.Value = 0;
                PBwrFilePro.Maximum = 100;
                PBwrFilePro.Step = 1;
                Thread.Sleep(50);
                SLprocess.Text = "";
                SLfileName.Text = "";
                cBLoadPict.Checked = GRAF_FILES.oldStopExtExhData;
                       
            }
            else
            {
                PBFileToMem.Value = 0;
                PBwrFilePro.Value = 0;
                SLprocess.Text = "";
                SLfileName.Text = "";
                GRAF_FILES.oldStopExtExhData = cBLoadPict.Checked;
                cBLoadPict.Checked = false;
                cBLoadPict.Checked = true;
            }
            groupBox2.Enabled = En;
        }
        //public enum Efl_DEV { fld_PC = 0, fld_HUB, fld_MainBoard, fld_TFTboard, fld_FEUdetect, fld_none = 0x0f };//тип устройства Ошибка в

        public string TryOpenDev(BoardVer Dev, Efl_DEV DevType,bool ShowMess)
        {//public enum Efl_DEV { fld_PC = 0, fld_HUB, fld_MainBoard, fld_TFTboard, fld_FEUdetect, fld_none = 0x0f };//тип устройства
            string ret;
            try
            {
                SLprocess.Text = Properties.Settings.Default.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
                ret = CANHUB.GetVerDev(Dev, DevType);
                if (ShowMess)
                    switch (DevType)
                    {
                        case Efl_DEV.fld_HUB:
                            MessageBox.Show("Определено устройство - USB HUB", "USB HUB успешно подключен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case Efl_DEV.fld_MainBoard:
                            MessageBox.Show("Определено устройство - Main Board", "Плата PLC контроллера", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case Efl_DEV.fld_TFTboard:
                            MessageBox.Show("Определено устройство - TFT Board", "Плата TFT контроллера", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        default:
                            return ("");//ответ неизвестного устройства
                    }

                return Dev.Version;
            }
            catch (Exception)
            {

                if (ShowMess)
                    switch (DevType)
                    {
                        case Efl_DEV.fld_HUB:
                            MessageBox.Show( "USB HUB не найден", "Обнаружение устройства USB HUB", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case Efl_DEV.fld_MainBoard:
                            MessageBox.Show( "Плата Main Board не обнаружена", "Обнаружение устройства - Main Board", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case Efl_DEV.fld_TFTboard:
                            MessageBox.Show( "Плата TFT контроллера не обнаружена", "Обнаружение устройства - TFT Board", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        default:
                            break;
                    }

                return "";//требуемое устройство не обнаружено
            }

        }

        public void Scan_and_OpenHUBTFTCOMport(string ComPortName, int baudrate, bool ShowMessage)
        {
            LBoxInterface.Items.Clear();
            //определён хотя бы один порт
            if (CANHUB.GetOpenComport(ComPortName, baudrate, ShowMessage))
            {// пытаемся соединиться с устройством посредство порта, сохранённых в property по умолчанию
                try
                {
                    int i = 0;
                    string tmpStr;

                    tmpStr = TryOpenDev(CANHUB.CAN_HUB, Efl_DEV.fld_HUB, false);
                    if (tmpStr != "")
                    {
                        tmpStr=CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }

                    tmpStr = TryOpenDev(CANHUB.MAIN_Board, Efl_DEV.fld_MainBoard, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }

                    
                    SLprocess.Text = ComPortName + "  Baud:" + baudrate.ToString();
                    tmpStr = TryOpenDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }


                    if (ShowMessage)
                    {
                        if (i == 0)//ни одно из устройств не найдено на COM порту по умолчанию
                            MessageBox.Show("Ни одно из устройств не найдено", "Проверьте подключены ли устройства и повторите попытку", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show("Обнаружены устройства", "Обнаруженные устройства подключены", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }


                    return;
                }
                catch (Exception)
                {
                }
            }
            //если не получается, пытаемся законнектиться посредством других определённых портов


            try
            {
                for (int i = 0; i < listComPort.Items.Count; i++)
                {
                    string sport = listComPort.Items[i].ToString();
                    if (ComPortName == sport)
                        break;//этот компорт мы уже ранее опросил

                    if (CANHUB.GetOpenComport(sport, Properties.Settings.Default.COMportBaud, false))                   
                    {//пытаемся установить связь с устройством с помощью очередного COM порта из списка

                        try

                        {
                            CANHUB.GetVerDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard);
                            //если получили отклик через COM порт запоминаем его номер
                            Properties.Settings.Default.COMportName = CANHUB.PortName;
                            Properties.Settings.Default.Save();
                            this.Text = "ProgChip " + CANHUB.TFT_Board.Version;

                            MessageBox.Show("Определено устройство - USB HUB", "USB HUB успешно подключен подключен", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return;
                        }
                        catch (Exception)
                        {
                            if (ShowMessage)
                            {
                                MessageBox.Show("Выберете номер подключенного COM потра, затем подключите датчик и нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с прибором",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                MessageBox.Show("Проверьте подключение USBto CAN хаба, выберете строку подключенного COM потра, затем нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с прибором", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
            }

        }



        public  void SelectComPort(string comPortNumber, ListBox listComPort)
        {
            // Ищем номер COM порта в списке
            for (int i = 0; i < listComPort.Items.Count; i++)
            {
                string itemText = listComPort.Items[i].ToString();
                if (itemText.Equals(comPortNumber, StringComparison.OrdinalIgnoreCase))
                {
                    // Если найдено совпадение, выделяем строку
                    listComPort.SelectedIndex = i;
                    return;
                }
            }

            // Если номер порта не найден, снимаем выделение
            listComPort.ClearSelected();
        }



        public FormHUB()
        {
            InitializeComponent();
           
            WORKAKVATEST = new SWORKAKVATEST(dGtimers, dGparam);
            // Подписываемся на событие DataReceivedEvent
            CANHUB.DataReceivedEvent += OnDataReceived;
            // Обработчик события

            cBtimeSpeed.SelectedIndex = 0;
            tabControl1.SelectedIndex = 2;
            WORKAKVATEST.UpdateComboBoxRejak(cBrej);


            this.Text = "DrawConfig  " + Properties.Settings.Default.Version;
            LBoxInterface.Items.Clear();
            LoadDefaultSetting();

            // Отрисовка таблицы dGparam
            /* похоже надо убрать        WORKAKVATEST.AdjustRowHeights(dGparam);

                     // Устанавливаем режим сортировки для всех столбцов
                     foreach (DataGridViewColumn column in dGparam.Columns)
                     {
                         column.SortMode = DataGridViewColumnSortMode.NotSortable;
                     }

                     // Устанавливаем режим выделения колонок с заголовком
                     dGparam.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

                     //Инициализируем таблицу с таймерами
                     WORKAKVATEST.FormatTimersGridView( 120, 30, new string[]{ "Rej", "CountSec", "LastStamp_mSec", "MaxCountSec", "DamageSec" }, WORKAKVATEST.GetTextHead(0, 7));
              */

            cBrej.SelectedIndex = 0;


            if (CANHUB.IsPortOpen(listComPort.Items))
            {
                Scan_and_OpenHUBTFTCOMport(Properties.Settings.Default.COMportName, Properties.Settings.Default.COMportBaud, cBoxEnMess.Checked);
                SelectComPort(Properties.Settings.Default.COMportName, listComPort);
            }


           }



        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Преобразуем массив байт в строку в формате HEX
            //string hexString = BitConverter.ToString(e.Data).Replace("-", " ");

            // Поскольку событие может быть вызвано из другого потока, используем Invoke
            if (InvokeRequired)
            {
                byte[] RXdata = new byte[e.Data.Length-6];//создаём массив для хранения данных
                Array.Copy(e.Data,4, RXdata,0, e.Data.Length - 6);//копируем в него принятые данные без заголовка и CRC

                //WORKAKVATEST.FromByteArray(e.Data);//извлекаем сырые считанные данные  new
                //извлекаем данные из таймеров
                WORKAKVATEST.TimersParFromByteArray(RXdata);

                Invoke(new Action(() => WORKAKVATEST.DisplayInDataGridView()));

                //Invoke(new Action(() => textBox1.Text = hexString));

            }
            else
            {
                WORKAKVATEST.DisplayInDataGridView();
            }
        }




        private void bRUN_Click(object sender, EventArgs e)
        {
            bCreatCode.Enabled = false;

            if (!Directory.Exists(tBgrafDIR.Text))
            {
                MessageBox.Show("Не существует директории: \"+tBgrafDIR.Text+\"", "Процесс остановлен",
                                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((!Directory.Exists(tBgrafDIR.Text + "\\MENU")) || (!Directory.Exists(tBgrafDIR.Text + "\\PICT")) || (!Directory.Exists(tBgrafDIR.Text + "\\FONT")))
            {
                MessageBox.Show("Не существует поддиректорий: \" MENU, PICT, FONT\"", "Процесс остановлен",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }




            fcreater.SourseDir = TBSourseDir.Text;
            fcreater.resultPath = tBresultPath.Text;
            fcreater.codefilename = tBcodefilename.Text + ".txt";
            fcreater.resultFilename = tBresultFilename.Text + ".bin";


            fcreater.GetParameters(fcreater.SourseDir, fcreater.resultPath, fcreater.resultFilename);

            tBCode.Clear();

            bCreatCode.Enabled = true;
 
        }



        private void bSEL1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderDialog1 = new FolderBrowserDialog();

            if (FolderDialog1.ShowDialog() == DialogResult.OK)
            {
                TBSourseDir.Text = FolderDialog1.SelectedPath;
                Properties.Settings.Default.DirGrafFiles = TBSourseDir.Text;
                Properties.Settings.Default.Save();
            }

            /*
                        OpenFileDialog dlg = new OpenFileDialog();

                        dlg.Filter = "TEXT file|*.txt|txt feles|*.*";
                        dlg.InitialDirectory = Properties.Settings.Default.DirGrafFiles;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            TBSourseDir.Text = dlg.;
                            Properties.Settings.Default.DirGrafFiles = TBSourseDir.Text;
                            Properties.Settings.Default.Save(); tBgrafDIR
                        }*/
        }

        private void bSEL2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "TEXT file|*.raw|font files|*.*";
            dlg.InitialDirectory = "c:\\Projects\\WaterMassProd1\\AKVAGRAF\\GRAFdata\\Fonts\\RobotoRusPlus_FR\\Roboto - Regular_16\\L4";

            //        dlg.FileName= "c:\\Projects\WaterMassProd1\\AKVAGRAF\\GRAFdata\\Fonts\\RobotoRusPlus_FR\\Roboto - Regular_16\\L4\\Roboto - Regular_16_L4.raw";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Чтение файла в виде байтового массива
                byte[] fileBytes = File.ReadAllBytes(dlg.FileName);
                int countButes = fileBytes.Length;
                int i = 0;
                // Создание потока для записи в файл с расширением .bin
                string filebin = Path.ChangeExtension(dlg.FileName, "bin");
                using (StreamWriter writer = new StreamWriter(filebin))
                {
                    int count = -1;

                    // Преобразование каждого байта в шестнадцатеричное представление
                    while (countButes-- > 0)
                    {
                        if (count == 15)
                        {
                            // Новая строка после каждых 16 значений
                            writer.WriteLine();
                            count = 0;
                        }
                        else
                            count++;

                        // Запись шестнадцатеричного представления с разделением запятой
                        if (countButes > 0)
                            writer.Write($"0x{fileBytes[i++]:X2}, ");
                        else
                            writer.Write($"0x{fileBytes[i++]:X2}");
                    }
                }

            }
        }
        private void bRES_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderResult = new FolderBrowserDialog();

            if (FolderResult.ShowDialog() == DialogResult.OK)
            {
                tBresultPath.Text = FolderResult.SelectedPath;
                Properties.Settings.Default.DirResultGrafFile = tBresultPath.Text;
                Properties.Settings.Default.Save();
            }

        }

        private void bGRAFdir_Click(object sender, EventArgs e)
        {

 FolderBrowserDialog FolderResult = new FolderBrowserDialog();
             string dirName = Properties.Settings.Default.FolderGRAF;
             if (FolderResult.ShowDialog() == DialogResult.OK)
             {
                 tBgrafDIR.Text = FolderResult.SelectedPath;
                 Properties.Settings.Default.FolderGRAF = tBgrafDIR.Text;
                 Properties.Settings.Default.Save();
             }
        }



        private void bSaveConf_Click(object sender, EventArgs e)
        {
            SaveDefaultSetting();
        }

        private void backWorkDo(object sender, DoWorkEventArgs e)
        {

        }

        private void backWorkReport(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backWorkComplet(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void bCreatCode_Click(object sender, EventArgs e)
        {
            tBCode.Clear();
            string tmpstr;
            tmpstr = fcreater.RESfont.DefValStruct("FONT");
            tBCode.Text += tmpstr;
            tmpstr = fcreater.RESjpg.DefValStruct("PICT");
            tBCode.Text += tmpstr;
            tmpstr = fcreater.RESmenu.DefValStruct("MENU");
            tBCode.Text += tmpstr;
            //  List<string> code = new List<string>();

            string[] arrstr;
            arrstr = fcreater.RESfont.CreatDefStruct("FONT");
            foreach (string code in arrstr)
                tBCode.Text += code;//+ "\r\n";
            arrstr = fcreater.RESjpg.CreatDefStruct("PICT");
            foreach (string code in arrstr)
                tBCode.Text += code;//+ "\r\n";
            arrstr = fcreater.RESmenu.CreatDefStruct("MENU");
            foreach (string code in arrstr)
                tBCode.Text += code;//+ "\r\n";
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            tBCode.Clear();


                tBCode.Text += "aaa\r\nbbb\r\nccc";

            
        }

        private void butINSERT_Click(object sender, EventArgs e)
        {
   /*         Properties.Settings.Default.NameBaseFile = tBbasefile.Text;
            Properties.Settings.Default.NameInsFile = tBinsFile.Text;
            Properties.Settings.Default.StartInsAddr = tBadrins.Text;

            int InsAdr = fcreater.ConvHextoInt(tBadrins.Text);

            using (FileStream INSstream = File.Open(tBinsFile.Text, FileMode.Open))
            {
                // выделяем массив для считывания данных из файла
                byte[] buffer = new byte[INSstream.Length];
                // считываем данные
                INSstream.Read(buffer, 0, buffer.Length);

                using (FileStream BASEstream = File.Open(tBbasefile.Text, FileMode.Open))
                {
                    BASEstream.Seek(InsAdr, SeekOrigin.Begin);

                    BASEstream.Write(buffer, 0, buffer.Length);
                }
            }*/
        }

        private void butSelBaseFile_Click(object sender, EventArgs e)
        {
    /*        string basefileName = tBbasefile.Text;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                if (File.Exists(basefileName))
                {
                    openFileDialog.InitialDirectory = Path.GetFileName(basefileName);
                }
                else
                    openFileDialog.InitialDirectory = "c:\\";


                openFileDialog.Filter = "BIN files (*.bin)|*.bin";
               // openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    tBbasefile.Text = openFileDialog.FileName;
                    Properties.Settings.Default.NameBaseFile = tBbasefile.Text;
                    Properties.Settings.Default.Save();
                }
            }*/
        }

        private void butSelinsFile_Click(object sender, EventArgs e)
        {
  /*          string insfileName = tBinsFile.Text;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                if (File.Exists(insfileName))
                {
                    openFileDialog.InitialDirectory = Path.GetFileName(insfileName);
                }
                else
                    openFileDialog.InitialDirectory = "c:\\";


                openFileDialog.Filter = "BIN files (*.bin)|*.bin";
                // openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    tBinsFile.Text = openFileDialog.FileName;
                    Properties.Settings.Default.NameInsFile = tBinsFile.Text;
                    Properties.Settings.Default.Save();
                } 
            }*/
        }

        private void bfindPort_Click(object sender, EventArgs e)
        {
            CANHUB.IsPortOpen(listComPort.Items);
        }

        private void bCOMselect_Click(object sender, EventArgs e)
        {

            int index = listComPort.Items.IndexOf(Properties.Settings.Default.COMportName);
            if (index == -1)
            {
                listComPort.SelectedIndex = 0;
            }
            else
            {
                listComPort.SelectedIndex = index;
            }

            if (CANHUB.GetOpenComport(listComPort.SelectedItem.ToString(), Properties.Settings.Default.COMportBaud, true))
            //    if (Sens.GetOpenComport(Properties.Settings.Default.COMportName, Properties.Settings.Default.COMportBaud, false))
            {
                Properties.Settings.Default.COMportName = CANHUB.PortName;
                Properties.Settings.Default.Save();
                SLprocess.Text = Properties.Settings.Default.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
                try

                {
                   //вставить сюда команду считывания версии ПО и платы DCOM.WRUFComm(0);
                }
                catch (Exception)
                {
                    MessageBox.Show("Выберете номер подключенного COM потра, затем подключите датчик и нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с прибором",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {

                    /*  ReadAllFromSens();
                      if (rBresSensAvto.Checked == false)
                      {
                          bTest.Visible = true;
                          labelRsens.Visible = true;
                      }
                      else
                      {
                          bTest.Visible = false;
                          labelRsens.Visible = false;
                      }*/
                    this.Text = "TFTprog " + Properties.Settings.Default.Version + " ©С - Progchip" + " (" + "версия платы: " + 1.00/*DCOM.parBoardVerHard*/ + "  версия микрокода: " + 1.00/*DCOM.parBoardVerSoft*/ + " )";
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Не удаётся выполнить команду, возможно требуется обновить микрокод платы", ex.Message,
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CANHUB.GetOpenComport(listComPort.SelectedItem.ToString(), Properties.Settings.Default.COMportBaud, false))

            {

                SLprocess.Text = Properties.Settings.Default.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
                try

                {
                   // вставить сюда команду считывания версии  DCOM.WRUFComm(0);
                    if (Properties.Settings.Default.COMportName != CANHUB.PortName)
                        Properties.Settings.Default.Save();

                }
                catch (Exception)
                {
                    MessageBox.Show("Выберете номер подключенного COM потра, затем подключите датчик и нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с прибором",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
        }



        private void bCOMselect_Click_1(object sender, EventArgs e)
        {
            string itemText = "";
            LBoxInterface.Items.Clear();
            try
            {
                itemText = listComPort.Items[listComPort.SelectedIndex].ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Не выделен ни один порт", "Выделите порт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            

            if (CANHUB.GetOpenComport(itemText, Properties.Settings.Default.COMportBaud, false))
            {// пытаемся соединиться с устройством посредством выделенного порта
                try
                {
                    int i = 0;
                    string tmpStr;

                    tmpStr = TryOpenDev(CANHUB.CAN_HUB, Efl_DEV.fld_HUB, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }

                    tmpStr = TryOpenDev(CANHUB.MAIN_Board, Efl_DEV.fld_MainBoard, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }


                    SLprocess.Text = itemText + "  Baud:" + Properties.Settings.Default.COMportBaud;
                    tmpStr = TryOpenDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }


                 //   if (ShowMessage)
                    {
                        if (i == 0)//ни одно из устройств не найдено на COM порту по умолчанию
                            MessageBox.Show("Ни одно из устройств не найдено", "Проверьте подключены ли устройства и повторите попытку", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show("Обнаружены устройства", "Обнаруженные устройства подключены", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }


                    return;
                }
                catch (Exception)
                {
                }
            }







            /*        int index = listComPort.Items.IndexOf(Properties.Settings.Default.COMportName);

                    if (index == -1)
                    {
                        listComPort.SelectedIndex = 0;
                    }
                    else
                    {
                        listComPort.SelectedIndex = index;
                    }*/

            if (CANHUB.GetOpenComport(listComPort.SelectedItem.ToString(), Properties.Settings.Default.COMportBaud, true))
            //    if (Sens.GetOpenComport(Properties.Settings.Default.COMportName, Properties.Settings.Default.COMportBaud, false))
            {
                Properties.Settings.Default.COMportName = CANHUB.PortName;
                Properties.Settings.Default.Save();
                SLprocess.Text = Properties.Settings.Default.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
                try

                {
                    CANHUB.GetVerDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard);
                }
                catch (Exception)
                {
                    MessageBox.Show("Выберете номер подключенного COM потра, затем подключите датчик и нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с прибором",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



/*
                catch (Exception ex)
                {
                    MessageBox.Show("Не удаётся выполнить команду, возможно требуется обновить микрокод платы", ex.Message,
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
                MessageBox.Show("Определено устройство - USB HUB", "USB HUB успешно подключен",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try

            {
                button1.Text=CANHUB.GetVerDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard);
                bTestProWRFLASH.Text = "TestProWRFLASH";
            }
            catch (Exception)
            {
  /*              MessageBox.Show("Выберете номер подключенного COM потра, затем подключите датчик и нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с прибором",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;*/
            }

            
        }




//прямая функция, извлекающая данные из таблицы и вставляющая их в массив байт

        private byte[] ExtractFloatDataFromTable(DataTable table)
        {
            List<byte> byteList = new List<byte>();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (column.DataType == typeof(float))
                    {
                        float floatValue = (float)row[column];
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);
                        byteList.AddRange(floatBytes);
                        float vback = BitConverter.ToSingle(floatBytes, 0);
                    }
                }
            }

            return byteList.ToArray();
        }

//обратная функция извлекающая данные из массива байт и вставляющая их в таблицу
        private void InsertFloatDataIntoTable(byte[] data, DataTable table)
        {
            int floatSizeInBytes = sizeof(float);
            int totalFloats = data.Length / floatSizeInBytes;

            if (totalFloats * floatSizeInBytes != data.Length)
            {
                throw new ArgumentException("Invalid data length for float values.");
            }

            int columnIndex = 0;
            int rowIndex = 0;

            for (int i = 0; i < totalFloats; i++)
            {
                byte[] floatBytes = new byte[floatSizeInBytes];
                Array.Copy(data, i * floatSizeInBytes, floatBytes, 0, floatSizeInBytes);

                float floatValue = BitConverter.ToSingle(floatBytes, 0);

                DataColumn column = table.Columns[columnIndex];
                DataRow row = table.Rows[rowIndex];

                if (column.DataType == typeof(float))
                {
                    row[column] = floatValue;
                    columnIndex++;
                    if (columnIndex >= table.Columns.Count)
                    {
                        columnIndex = 0;
                        rowIndex++;
                        if (rowIndex >= table.Rows.Count)
                        {
                            // Handle the case where there are more floats than cells in the table
                            throw new ArgumentException("Too many float values for the given table size.");
                        }
                    }
                }
            }
        }

//проверочная функция, которая сначала извлекает данные из таблицы в массив байт, а потом вставляет их обратно
        private byte[] ExtractAndInsertFloatData(DataTable originalTable)
        {
            List<byte> extractedData = new List<byte>();

            foreach (DataRow row in originalTable.Rows)
            {
                foreach (DataColumn column in originalTable.Columns)
                {
                    if (column.DataType == typeof(float))
                    {
                        float floatValue = (float)row[column];
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);
                        extractedData.AddRange(floatBytes);
                    }
                }
            }

  /*          DataTable newTable = new DataTable();

            // Assume the columns are already created in the same order as during extraction
            foreach (DataColumn column in originalTable.Columns)
            {
                newTable.Columns.Add(column.ColumnName, column.DataType);
            }
  */
 //           InsertFloatDataIntoTable(extractedData.ToArray(), originalTable);
  
            // Now newTable should contain the extracted data

            // Optionally, you can compare the originalTable and newTable to verify the correctness

            return extractedData.ToArray();
        }

        

        public void FormHUB_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            DataTable tablePAR = new DataTable();
            tablePAR.Columns.Add("trueParam", typeof(string));
            tablePAR.Columns.Add("min", typeof(float));
            tablePAR.Columns.Add("LevelWarn", typeof(float));
            tablePAR.Columns.Add("LevelErr", typeof(float));
            tablePAR.Columns.Add("SetLevel", typeof(float));
            tablePAR.Columns.Add("MAX", typeof(float));
            tablePAR.Columns.Add("Dimension", typeof(string));
            tablePAR.Columns.Add("Value", typeof(float));


            dataGridView1.DataSource = tablePAR;
            dataGridView1.AllowUserToAddRows = false;

            tablePAR.Rows.Add("val > P0", 0,0,0,0, 0, "Бар", 0);//enump_P0  - минимальное давление исходной воды
            tablePAR.Rows.Add("val < QT0", 0, 0, 0, 0, 0, "мкСм/см", 0); //enump_QT0  - максимальная УЭП исходной воды
            tablePAR.Rows.Add("val > FT1", 0, 0, 0, 0, 0, "л/мин", 0);//enump_F1_P5  минимальный расход пермиата по расходомеру FT1 для пресета P5
            tablePAR.Rows.Add("val > FT0-FT1", 0, 0, 0, 0, 0, "л/мин", 0);//enump_F1_P5  минимальный расход пермиата по расходомеру FT1 для пресета P5
            tablePAR.Rows.Add("val < QT1", 0, 0, 0, 0, 0, "мкСм/см", 0);//enump_QT1  - максимальная УЭП пермеата
            tablePAR.Rows.Add("val < QT2", 0, 0, 0, 0, 0, "мкСм/см", 0);//enump_QT2  - максимальная УЭП фильтрата
            tablePAR.Rows.Add("val > FT0/FT1", 0, 0, 0, 0, 0, "л/мин", 0);//
            tablePAR.Rows.Add("val < FT0/FT1", 0, 0, 0, 0, 0, "л/мин", 0);//


            /*
            
            tablePAR.Rows.Add("val < QT0",300,1000*0.8,1000,2000, "мкСм/см",500 ); //enump_QT0  - максимальная УЭП исходной воды
            tablePAR.Rows.Add("val < QT1", 20,40*0.8, 40, 100, "мкСм/см", 30);//enump_QT1  - максимальная УЭП пермеата
            tablePAR.Rows.Add("val < QT2", 1, 10 * 0.8, 10, 20 , "мкСм/см", 5);//enump_QT2  - максимальная УЭП фильтрата
            tablePAR.Rows.Add("val < Tfilt ", 25, 35 * 0.8,35, 50 ,"C", 27);//enump_tfilt  - максимальная температура фильтрата
            tablePAR.Rows.Add("val > P0", 0.1, 0.5 * 1.2, 0.5, 1.5,  "Бар", 1.3);//enump_P0  - минимальное давление исходной воды
            tablePAR.Rows.Add("val > R1",20, 35 * 1.2, 35,50,"%", 45);//enump_R1  - минимальная степень отбора в процентах
            tablePAR.Rows.Add("val < R2",50,65 * 0.8,65, 80,"%", 55);//enump_R2  -максимальная степень отбора в процентах

            tablePAR.Rows.Add("val > FT1_P5", 0.1, 2 * 1.2, 2, 5, "л/мин", 3);//enump_F1_P5  минимальный расход пермиата по расходомеру FT1 для пресета P5
            tablePAR.Rows.Add("val > FT1_P10", 0.1, 5 * 1.2, 5,7,"л / мин", 10);//enump_F1_P10  минимальный расход пермиата по расходомеру FT1 для пресета P10
            tablePAR.Rows.Add("val > FT1_P20", 0.1, 10 * 1.2, 10, 20 , "л/мин", 15);//enump_F1_P20  минимальный расход пермиата по расходомеру FT1 для пресета P20

            tablePAR.Rows.Add("val > F2_P5", 0.1, 2 * 1.2, 2, 3, "л/мин", 5);//enump_F2_P5  минимальный расход концентрата по расходомеру FT0-FT1 для пресета P5
            tablePAR.Rows.Add("val > F2_P10", 0.1, 5 * 1.2, 5, 10, "л/мин", 7);//enump_F2_P10  минимальный расход концентрата по расходомеру FT0-FT1 для пресета P10
            tablePAR.Rows.Add("val > F2_P20", 0.1, 10 * 1.2, 10, 20, "л/мин", 15);//enump_F2_P20  минимальный расход концентрата по расходомеру FT0-FT1 для пресета P20
            */

            ExtractAndInsertFloatData(tablePAR);
     //       byte[] DataFromTable = ExtractFloatDataFromTable(tablePAR);


            DataTable tableTimer = new DataTable();
            dataGridTimers.AllowUserToAddRows = false;
            tableTimer.Columns.Add("Timer", typeof(string));
            tableTimer.Columns.Add("min", typeof(float));
            tableTimer.Columns.Add("predef", typeof(float));
            tableTimer.Columns.Add("MAX", typeof(float));
            tableTimer.Columns.Add("Dimension", typeof(string));
            tableTimer.Columns.Add("Value", typeof(float));

            tableTimer.Rows.Add("T1 простоя", 1, 15, 60, "мин",15);//enump_T1  - таймер простоя
            tableTimer.Rows.Add("T2 промывки", 1, 2, 10, "мин", 2);//eapar_T2 - таймер промывки
            tableTimer.Rows.Add("T3 ожидания ", 5, 240, 240, "мин", 240);//enump_T3   - таймер ожидания
            tableTimer.Rows.Add("T4 MFC/ACC промывки", 5, 10, 20, "мин", 10);//enump_T4  - таймер промывки MFC/ACC
            tableTimer.Rows.Add("T5 ROC промывки", 60, 180, 360, "мин", 180);//enump_T5  - таймер промывки ROC



            dataGridTimers.DataSource = tableTimer;
            dataGridTimers.AllowUserToAddRows = false;
            foreach (DataGridViewColumn column in dataGridTimers.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }





        private void bTestProWRFLASH_Click(object sender, EventArgs e)
        {

        }
        private void bwr_tft_flash_Click(object sender, EventArgs e)
        {
            EnDisComponent(false);
            WRpro.RunWorkerAsync();
            EnDisComponent(true);
        }

        private void bTestFileToFLASH_Click(object sender, EventArgs e)
        {        

            try
            {
                
                CANHUB.StartFLASHadrFlowPro = CANHUB.StartFLASHadr;

                EnDisComponent(false);

                
                WRpro.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                EnDisComponent(true);
                tabControl1.Enabled = true;
                MessageBox.Show(ex.Message, "Ошибка инициализации цикла", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bStopTest_Click(object sender, EventArgs e)
        {


            /*  
            try
            {
                CANHUB.DirFlowPro = "BLOB";
                CANHUB.StartFLASHadrFlowPro = CANHUB.StartFLASHadrBLOB;
                CANHUB.Up1Doun0adrFLASH = 1;

                EnDisComponent(false);
                WRpro.RunWorkerAsync();
                EnDisComponent(true);
            }
            catch (Exception ex)
            {
                EnDisComponent(true);
                tabControl1.Enabled = true;
                MessageBox.Show(ex.Message, "Ошибка выполнения цикла", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            */
        }
        private void PBFileToMem_Click(object sender, EventArgs e)
        {

        }


        private void cBCreatBEN_CheckedChanged(object sender, EventArgs e)
        {
            CANHUB.enProFontPict = Convert.ToInt32 (cBfontPict.Checked);
        }
        private void cBpict_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cBmenu_CheckedChanged(object sender, EventArgs e)
        {
            CANHUB.enProMenu = Convert.ToInt32(cBmenu.Checked);
        }




        private void tBAdrOneFile_TextChanged(object sender, EventArgs e)
        {

        }
        //c:\Projects\WaterMassProd1\NEWAKVAimage\MENU\000_zagruz.jpg
        private void button2_Click(object sender, EventArgs e)
        {//запись одного файла по заданному адресу

                CANHUB.enProFontPict = 0;
                CANHUB.enProMenu = 0;
                CANHUB.enProFontPict = 0;
                CANHUB.enProAddOneMenuFile = 0;

                string filename = tBNameOneFile.Text;
                string DIR_FlowPro = Properties.Settings.Default.FolderGRAF;//директория;
                if (!GRAF_FILES.CheckFileExist(filename, DIR_FlowPro, "MENU"))
                    return;//файл либо путь к нему указан не правильно
                CANHUB.OneFileName = filename;//имя файла подлежит записи

            try
            {

                CANHUB.enProAddOneMenuFile = 1;//Флаг перезаписи одного файла
                CANHUB.OneFileNum = GRAF_FILES.GetFileOrder(filename);//определяем порядковый номер файла в директории
                EnDisComponent(false);
                WRpro.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                EnDisComponent(true);
                tabControl1.Enabled = true;
                MessageBox.Show(ex.Message, "Ошибка инициализации цикла", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }




        private void WRpro_DoWork(object sender, DoWorkEventArgs e)
        {
            int PbarDataLen=0;
            int OneFileCount = 0;
            e.Result = 0;
            e.Result = "Все операции выполнены успешно";
            EMemPRO statMemPRO = EMemPRO.eopmpro_setpar;
            int NumFile = 0;
            string DIR_FlowPro = Properties.Settings.Default.FolderGRAF;
            string MENUfileFLASHadr = DIR_FlowPro + "\\DOC\\defAdrMENU.txt";
            int[] MENUfilesStartAdr = CANHUB.GetNumbersFromFile(MENUfileFLASHadr);//формируем массив длин файлов на основе файла описания

            string[] MENUfilenames = GRAF_FILES.Init_WRtoFLASHfiles(DIR_FlowPro, "MENU");//получаем список файлов директории c изображениями меню
            Array.Sort(MENUfilenames);
            int MENUfilecount = MENUfilenames.Length;
            
            if (CANHUB.enProAddOneMenuFile > 0)
            {//перезапись/дозапись одного графического файла в TFT FLASH память 
                CANHUB.enProMenu = 0;
                CANHUB.enProFontPict = 0;
                CANHUB.enProAddOneMenuFile = 1;

                try
                {

                    //определяем длину файла
                    string MENU_FULLfilename = MENUfilenames[CANHUB.OneFileNum];
                    FileInfo MENUfileInfo = new FileInfo(MENU_FULLfilename);
                    long fileSizeInBytes = MENUfileInfo.Length;
                    if (CANHUB.OneFileNum< (MENUfilesStartAdr.Length-1))
                    {//только если файл не последний в списке делаем проверку не наезжает ли новый записываемый файл на начало следующего
                        if (fileSizeInBytes>(MENUfilesStartAdr[CANHUB.OneFileNum+1]- MENUfilesStartAdr[CANHUB.OneFileNum]))
                        {
                            MessageBox.Show("Длина файла слишком велика для перезаписи, необходимо создание новых заголовочных структур файлов и полная перезапись всего меню, либо дозапись в конец под другим именем", "Ошибка записи файла",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Result = null;
                        }
                    }
                    
                    CANHUB.Up1Doun0adrFLASH = 1;
                    int pPbarCount = 0;

                    //НОВЫЙ КОД ЗАПИСИ изображения меню ИЗ ФАЙЛА!!!
                    CANHUB.WritePict(MENUfilenames[CANHUB.OneFileNum], MENUfilesStartAdr[CANHUB.OneFileNum], WRpro);

                    if (WRpro.CancellationPending)
                    {//остановка работы по нажатию кнопки останова в приложении
                        e.Cancel = true;
                        return;
                    }

                    //отображение загруженной картинки меню на TFT панели
                     //CommSendAnsv(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, byte SubCom = 0, int TimeOutStartAnsv = 500, int TimeOutNextByte = 100)
                        int j = GRAF_FILES.NumMenuOneMenuFile;
                        byte[] numMenu = { 0, 0, 0, 0, 0, 0, 0, 0 };
                        numMenu[4] = Convert.ToByte(j & 0xff);
                        numMenu[5] = Convert.ToByte((j >> 8) & 0xff);
                        CANHUB.CommSendAnsv(ECommand.cmd_TFTmenu, Efl_DEV.fld_TFTboard, numMenu, 1000);
                        e.Result = "Файл  успешно записан в TFT FLASH память";
                        return;//в режиме записи одного файла один файл записан

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка записи файла",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Result = null;
                }
            }


            if (CANHUB.enProMenu > 0)
            {//запись заставок меню
                try
                {


                    CANHUB.Up1Doun0adrFLASH = 1;
                    CANHUB.enProMenu = 0;
                    CANHUB.DirFlowPro = "MENU";
                    //  похоже это артефакт    int pPbarCount = 0;
                    int NumMENU = 0; ;

                    // похоже это артефакт   int FLASHwrFileStartAdr = CANHUB.StartFLASHadrFlowPro;

                    while (NumMENU < MENUfilecount)
                    {
                        int proMenuFiles = ((NumMENU + 1) * 100) / MENUfilecount;//нормализованный счётчик прогрессбара количества файлов
                        string MENU_FULLfilename = MENUfilenames[NumMENU];
                        // похоже это артефакт  pPbarCount = 0;

                        FileInfo MENUfileInfo = new FileInfo(MENU_FULLfilename);
                        long fileSizeInBytes = MENUfileInfo.Length;
                        string MENUfilename = MENUfileInfo.Name;
                        int MENUalignFileLen = fcreater.RESjpg.GetAlignVol((int)fileSizeInBytes, CANHUB.FLASHalign, 1);//получаем выравненную длину файла, в данном случае с увеличением

                        WRpro.ReportProgress(proMenuFiles, MENUfilename);

                        //НОВЫЙ КОД ЗАПИСИ изображения меню ИЗ ФАЙЛА!!!
                        CANHUB.WritePict(MENUfilenames[NumMENU], MENUfilesStartAdr[NumMENU], WRpro);


                        if (CANHUB.DirFlowPro == "MENU")
                        {//отображение загруженной картики меню на TFT панели
                         //CommSendAnsv(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, byte SubCom = 0, int TimeOutStartAnsv = 500, int TimeOutNextByte = 100)
                            int j = NumMENU;
                            byte[] num_Menu = { 0, 0, 0, 0, 0, 0, 0, 0 };
                            num_Menu[4] = Convert.ToByte(j & 0xff);
                            num_Menu[5] = Convert.ToByte((j >> 8) & 0xff);
                            CANHUB.CommSendAnsv(ECommand.cmd_TFTmenu, Efl_DEV.fld_TFTboard, num_Menu,  1000);

                        }
                        if (OneFileCount > 0)
                        {
                            e.Result = "Файл  успешно записан в TFT FLASH память";
                            return;//в режиме записи одного файла один файл записан
                        }

                        NumMENU++;
                        // похоже это артефакт  FLASHwrFileStartAdr = CANHUB.Resurs.GetAlignVol(FLASHwrFileStartAdr /* похоже это артефакт + CANHUB.FlowProRAMtoMEM.Length*/, CANHUB.FLASHalign, CANHUB.Up1Doun0adrFLASH);//устанавливаем новый адрес


                        if ((statMemPRO == EMemPRO.eopmpro_ready) || (statMemPRO == EMemPRO.eopmpro_wrFLASHOK))
                        { //пакет успешно отослан

                            e.Result = "Все файлы успешно Записаны в TFT FLASH память";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка записи файла",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Result = null;
                }
            }




            if (CANHUB.enProFontPict > 0)
            {//запись фонтов и картинок в память FLASH
                try
                {

                    CANHUB.Up1Doun0adrFLASH = 0;
                    CANHUB.enProFontPict = 0;
                    CANHUB.DirFlowPro = "FONT";
                    int NumPICT = 0; ;
                    int FLASHwrFileStartAdr = CANHUB.StartFLASHadrFlowPro;
                    string[] FONTfilenames = GRAF_FILES.Init_WRtoFLASHfiles(Properties.Settings.Default.FolderGRAF, "FONT");//получаем список файлов директории c фонтами
                    string[] PICTfilenames = GRAF_FILES.Init_WRtoFLASHfiles(Properties.Settings.Default.FolderGRAF, "PICT");//получаем список файлов директории с картинками


                    int FONTfilecount = FONTfilenames.Length;
                    int PICTfilecount = PICTfilenames.Length;
                    int FileCount = FONTfilecount + PICTfilecount;
                    NumFile = 0;
                    while (NumFile < FileCount)
                    {
                        int proFiles = ((NumFile+1) * 100) / FileCount;//нормализованный счётчик прогрессбара количества файлов
                        int pPbarCount = 0;
                        string FULLfilename;
                        if (NumFile < FONTfilecount)
                        {
                            FULLfilename = FONTfilenames[NumFile];
                        }
                        else
                        {
                            FULLfilename = PICTfilenames[NumFile - FONTfilecount];
                            CANHUB.DirFlowPro = "PICT";
                        }

                        FileInfo fileInfo = new FileInfo(FULLfilename);
                        long fileSizeInBytes = fileInfo.Length;
                        string fileName = fileInfo.Name;
                        int alignFileLen = fcreater.RESjpg.GetAlignVol((int)fileSizeInBytes, CANHUB.FLASHalign, 1);//получаем выравненную длину файла, в данном случае с увеличением

                        WRpro.ReportProgress(proFiles, fileName);
                        //адресация файлов фонтов и картинок идёт с понижением адреса, поэтому для вычисления стартового адреса необходим вычесть длину CANHUB.fileDataFLASH.Length
                        FLASHwrFileStartAdr = FLASHwrFileStartAdr - alignFileLen;//устанавливаем новый адрес с уменьшением!


                        //НОВЫЙ КОД ЗАПИСИ КАРТИНКИ ИЗ ФАЙЛА!!!
                        CANHUB.WritePict(FULLfilename, FLASHwrFileStartAdr, WRpro);


                        if (CANHUB.DirFlowPro == "PICT")
                        {//отображение загруженной картики  на TFT панели
                         //CommSendAnsv(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, byte SubCom = 0, int TimeOutStartAnsv = 500, int TimeOutNextByte = 100)

                            byte[] comPICT = { 1, 0, 0, 0, 0, 0, 0, 0 };//признаком отображения картинки служит единица в младшем байте, если ноль, то грузилось бы меню
                            comPICT[4] = Convert.ToByte(NumPICT & 0xff);
                            comPICT[5] = Convert.ToByte((NumPICT >> 8) & 0xff);
  //		public void CommSendAnsv(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, byte SubCom = 0, int TimeOutStartAnsv = 1000, int TimeOutNextByte = 100)

                            CANHUB.CommSendAnsv(ECommand.cmd_TFTmenu, Efl_DEV.fld_TFTboard, comPICT,  1000);// команда отображение картинки
                            NumPICT++;
                        }
                        NumFile++;


                    }

                    if ((statMemPRO == EMemPRO.eopmpro_ready) || (statMemPRO == EMemPRO.eopmpro_wrFLASHOK))
                    { //пакет успешно отослан
                        e.Result = "Все данные успешно Записаны в память";
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка cmd_WR_flowMem",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Result = null;
                }
            }
        }

        private void WRpro_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            string Str = e.UserState as string;//Status, numSteep,warning

            if (Str != null)
            {
                this.SLfileName.Text = Str;
                this.PBFileToMem.Value = e.ProgressPercentage;
                this.PBwrFilePro.Value = 0;
                return;
            }

            if (e.ProgressPercentage > 0)
            {

                this.PBwrFilePro.Value = e.ProgressPercentage;
               
            }

        }

        private void WRpro_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string s;
            if (e.Cancelled == true)
            {
                EnDisComponent(true);
                tabControl1.Enabled = true;
              //  toolStripProgressBar1.Value = 0;
                MessageBox.Show("Процесс прерван пользователем", "Процесс прерван", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusStrip1.Text = "Процесс прерван!";
            }
            else
            {
                if (e.Error != null)
                {
                    EnDisComponent(true);
                    tabControl1.Enabled = true;
                  //  toolStripProgressBar1.Value = 0;
                    MessageBox.Show("Не удаётся выполнить процесс до конца", "Cистемная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusStrip1.Text = "Ошибка!";
                }
                else
                {
                    if (e.Result != null)
                    { 
                        s = e.Result as string;
                    MessageBox.Show(s, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusStrip1.Text = "OK";
                    }
                    /*
                    if ((int)e.Result == 0)
                    {
                        MessageBox.Show("Все операции цикла выполнены успешно", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        statusStrip1.Text = "Выполнено";
                    }
                    else
                    {
                        MessageBox.Show("В ходе выполнения цикла возникли ошибки", "Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusStrip1.Text = "Ошибка";
                    }*/
                }
            }


            EnDisComponent(true);
            tabControl1.Enabled = true;
            cBmenu.Checked = false;
            cBfontPict.Checked = false;

        }





        private void bProcessCreater_Click(object sender, EventArgs e)
        {
            
            try
            {
                /*
                         public int RAMsize { get { return FM.RAMsize; } }
        public int FLASHsize { get { return FM.FLASHsize; } }
        public int StartFLASHadr { get { return FM.StartFLASHadr; } }
        public int FLASHalign { get { return FM.FLASHalign; } }
        public int RAMalign { get { return FM.RAMalign; } } */

                GRAF_FILES.Init_ArrFLASHaddr_MENU(Properties.Settings.Default.FolderGRAF, CANHUB.StartFLASHadr, GRAF_FILES.const_sizeTFTFLASHalign, GRAF_FILES.const_AddLenFileSize);
                GRAF_FILES.Init_FONTE_PICT(Properties.Settings.Default.FolderGRAF, CANHUB.StartFLASHadr, CANHUB.adrBASEMenuGPUpict);

 //               GRAF_FILES.Init_MENU_enum(Properties.Settings.Default.FolderGRAF, 0x100000);//load filenames
                GRAF_FILES.codeMENUcreater_(Properties.Settings.Default.FolderGRAF);

                /*
                EnDisComponent(false);
                WRpro.RunWorkerAsync();
                EnDisComponent(true);
                */
                MessageBox.Show( "Файлы помещены в папку DOC", "Все файлы успешно созданы", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                EnDisComponent(true);
                tabControl1.Enabled = true;
                MessageBox.Show(ex.Message, "Ошибка выполнения цикла", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bParamRead_Click(object sender, EventArgs e)
        {
            CANHUB.GetAKVAparFromMainBoard();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

                // Задайте полные пути к файлам
                string fileName1 = "c:\\Projects\\WaterMassProd1\\AKVAGRAF\\GRAFdata\\Fonts\\RobotoRus\\Roboto-Regular_16\\L4\\Roboto-Regular_16_L4.raw";
                string fileName2 = "c:\\Projects\\WaterMassProd1\\AKVAGRAF\\GRAFdata\\Fonts\\RobotoRusPlus_FR\\Roboto-Regular_16\\L4\\Roboto-Regular_16_L4.raw";

                // Проверка существования файлов
                if (!File.Exists(fileName1) || !File.Exists(fileName2))
                {
                    Console.WriteLine("Один или оба файла не существуют.");
                    return;
                }

                // Сравнение файлов
                CompareFiles(fileName1, fileName2);
            }

            static void CompareFiles(string fileName1, string fileName2)
            {
                using (FileStream fs1 = new FileStream(fileName1, FileMode.Open))
                using (FileStream fs2 = new FileStream(fileName2, FileMode.Open))
                {
                    // Проверка на равенство размеров файлов
                    if (fs1.Length != fs2.Length)
                    {
                        Console.WriteLine("Файлы имеют разные размеры.");
                        return;
                    }

                    int bufferSize = 4096; // Размер буфера для сравнения байтов
                    byte[] buffer1 = new byte[bufferSize];
                    byte[] buffer2 = new byte[bufferSize];

                    long position = 148;//148 - длина метрики, метрики должны получиться разными, их точно придётся заменять
                    long firstMismatchStart = -1;
                    long lastMismatchEnd = -1;

                    while (position < fs1.Length)
                    {
                        int bytesRead1 = fs1.Read(buffer1, 0, bufferSize);
                        int bytesRead2 = fs2.Read(buffer2, 0, bufferSize);

                        for (int i = 0; i < bytesRead1; i++)
                        {
                            if (buffer1[i] != buffer2[i])
                            {
                                if (firstMismatchStart == -1)
                                {
                                    firstMismatchStart = position + i;
                                }
                                lastMismatchEnd = position + i;
                            }
                        }

                        position += bytesRead1;
                    }

                    if (firstMismatchStart == -1)
                    {
                        Console.WriteLine("Файлы идентичны.");
                    }
                    else
                    {
                        Console.WriteLine($"Первое отличие в позиции 0x{firstMismatchStart:X}, последнее отличие в позиции 0x{lastMismatchEnd:X}.");
                    }
                }
            }

        private void tP_TFT_WR_RD_Click(object sender, EventArgs e)
        {

        }

        private void LBoxInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
 
        }

  /*      private void listComPort_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }*/



        private void bParamWrite_Click(object sender, EventArgs e)
        {

        }

        private void listComPort_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SaveDefaultSetting();
        }

        private void buttestSw_Click(object sender, EventArgs e)
        {
            
            try
            {

                //	public enum Efl_DEV { fld_PC = 0, fld_HUB, fld_MainBoard, fld_TFTboard, fld_FEUdetect, fld_none = 0x0f };//тип устройства
                if (CANHUB.ChangeDEVExhRejWork(ERejWork.evrTFTcalibr, Efl_DEV.fld_TFTboard) == ERejWork.ervNewSetOK)
                {
                    DialogResult result = MessageBox.Show("Запуск калибровки TFT панели", "Дождитесь окончания калибровки панели и нажмите на кнопку ОК", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                       // funk(); // Вызов функции funk() после нажатия OK
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show( "Проверьте соединение", "Не получен ответ от устройства", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cBLoadPict_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!cBLoadPict.Checked)
                {

                    CANHUB.ChangeDEVExhRejWork(ERejWork.ervTFT_master, Efl_DEV.fld_TFTboard);

                }
                else
                {
                    CANHUB.ChangeDEVExhRejWork(ERejWork.ervPICTupd, Efl_DEV.fld_TFTboard);
       
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось изменить режим работы", "Не получен ответ от устройства", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region ProSendDataTest

        private int successCount = 0;
        private int errorCount = 0;
        private CancellationTokenSource cancellationTokenSource;

        // Код функции, которую вы упомянули
        private void Func()
        {
            // Здесь должна быть ваша функция, пока оставлю как пример

            CANHUB.TestPictureWrite(240, Efl_DEV.fld_TFTboard);
        }

        private void RunFunc(object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Func();
                    Interlocked.Increment(ref successCount);
                    UpdateSuccessLabel();
                }
                catch
                {
                    Interlocked.Increment(ref errorCount);
                    UpdateErrorLabel();
                }
            }
        }

        private void UpdateSuccessLabel()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSuccessLabel));
            }
            else
            {
                lbNumGOOD.Text = successCount.ToString();
            }
        }

        private void UpdateErrorLabel()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateErrorLabel));
            }
            else
            {
                lbNum_BAD.Text = errorCount.ToString();
            }
        }

        private void bStartTestWrPict_Click(object sender, EventArgs e)
        {
            successCount = 0;
            errorCount = 0;
            lbNumGOOD.Text = "0";
            lbNum_BAD.Text = "0";
            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                ThreadPool.QueueUserWorkItem(new WaitCallback(RunFunc), cancellationToken);
            }


        }

        private void bStopTestWrPict_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }




        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void lBrejak_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void b_newPARfilename_Click(object sender, EventArgs e)
        {

        }

        private void b_PARtableLoad_Click(object sender, EventArgs e)
        {

        }

        private void cBtimeSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitComboSpeed)
            {
                string selectedItem = cBtimeSpeed.SelectedItem.ToString();
                CANHUB.SetSpeedFromStr(selectedItem);//команда изменения скорости работы таймеров
            }
            else
            {
                isInitComboSpeed = true;
            }
        }

        private void bSetBOARDwinDT_Click(object sender, EventArgs e)
        {
            string StartDataTime;
            //           long tmpLong = TimeConverter.Allsec("00:00:00 01.01.2000");
            long tmpLong = CANHUB.GetRTCDateTime(Efl_DEV.fld_MainBoard);
            StartDataTime = TimeConverter.GetDataTime(tmpLong, "00:00:00 01.01.2000");


            lBDT.Text = StartDataTime;

        }

        private void bSetBoardDT_Click(object sender, EventArgs e)
        {
            long WinNowTime = TimeConverter.WindowsTimeTosec("00:00:00 01.01.2000");
            CANHUB.SetRTCDateTime(WinNowTime, Efl_DEV.fld_MainBoard);
        }

        private void bCalibr_Click(object sender, EventArgs e)
        {
            CANHUB.StartTFTcalibr(Efl_DEV.fld_TFTboard);
        }

        private void cBrej_SelectedIndexChanged(object sender, EventArgs e)
        {
            WORKAKVATEST.selectedMode = WORKAKVATEST.SetRejakFromComboBox((ComboBox)sender);
            WORKAKVATEST.SelectColumn(dGparam, cBrej.SelectedIndex);
        }

    }

    
}
