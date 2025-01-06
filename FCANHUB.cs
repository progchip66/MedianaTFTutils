


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
        public  SParameterManager Proper = new SParameterManager();
        public SWORKAKVATEST WORKAKVATEST;
        public SAKVApar  AKVApar;

        TFileManager fcreater = new TFileManager();
        
        SCANHUB CANHUB = new SCANHUB();
        
        SGRAF_FILES GRAF_FILES = new SGRAF_FILES();
        
        bool isInitComboSpeed = false;


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
                SLprocess.Text = CANHUB.PortName + "  Baud:" + CANHUB.BaudRate.ToString();
                ret = CANHUB.GetVerDev(Dev, DevType, ShowMess);
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

                return ret;// Dev.Version;
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

        public int Scan_and_OpenHUBTFTCOMport(string ComPortName, int baudrate, bool ShowMessage)
        {
            LBoxInterface.Items.Clear();
            int i = 0;
            //определён хотя бы один порт
            if (CANHUB.GetOpenComport(ComPortName, baudrate, ShowMessage))
            {// пытаемся соединиться с устройством посредство порта, сохранённых в property по умолчанию
                try
                {
                    
                    string tmpStr;
  //                  CANHUB.PortName = ComPortName;

                    tmpStr = TryOpenDev(CANHUB.MainBoard, Efl_DEV.fld_MainBoard, false);
                    if ((tmpStr != "") && (!tmpStr.Contains(" not ")))
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }
                    
                    SLprocess.Text = ComPortName + "  Baud:" + baudrate.ToString();
                    tmpStr = TryOpenDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard, false);
                    if ((tmpStr != "") && (!tmpStr.Contains(" not ")))
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

                    if (i>0)
                     return i;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            //если не получается, пытаемся законнектиться посредством других определённых портов


            try
            {
                for (int j = 0; j < listComPort.Items.Count; j++)
                {
                    string sport = listComPort.Items[j].ToString();
                    if (ComPortName == sport)
                        continue;//этот компорт мы уже ранее опросил
                    if (CANHUB.GetOpenComport(sport, Proper.COMportBaud, false))
                    {//пытаемся установить связь с устройством с помощью очередного COM порта из списка
                        try
                        {

                            i = 0;
                            string tmpStr;

                            tmpStr = TryOpenDev(CANHUB.MainBoard, Efl_DEV.fld_MainBoard, false);
                            if ((tmpStr != "") && (!tmpStr.Contains(" not ")))
                            {
                                tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                                LBoxInterface.Items.Add(tmpStr);
                                i++;
                            }

                            
                            tmpStr = TryOpenDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard, false);
                            if ((tmpStr != "") && (!tmpStr.Contains(" not ")))
                            {
                                tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                                LBoxInterface.Items.Add(tmpStr);
                                i++;
                            }

                            if (i>0)
                            {//На COM портру определены рабочие устройства, значит далее работаем именно с ним!
                                SLprocess.Text = ComPortName + "  Baud:" + baudrate.ToString();
                                Proper.COMportName = CANHUB.PortName;
                                MessageBox.Show( "Номер порта сохранён в файле инициализации", "Определено устройство", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //new Thread(CANHUB.SlaveReceiveLoop) { IsBackground = true }.Start();
                                return i;                
                            }
                            else
                            {
                                MessageBox.Show("Необходимо подсоединить устройство к COM порту и повторно запустить программу", "Устройство не обнаружено",
                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                // Закрываем приложение независимо от выбора пользователя
                                Environment.Exit(0);

                            }
                            
                        }
                        catch (Exception)
                        {
                            if (ShowMessage)
                            {
                                MessageBox.Show("Необходимо подсоединить устройство к COM порту и повторно запустить программу", "Устройство не обнаружено",
                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                // Закрываем приложение независимо от выбора пользователя
                                Environment.Exit(0);
                            }
                        }
                    }
                }
                MessageBox.Show("Проверьте правильность подключение устройства, выберете строку подключенного COM потра, затем нажмите на кнопку \"Выбрать порт\"", "Не удаётся связаться с устройством!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            catch (Exception)
            {
                return 0;
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
           
            WORKAKVATEST = new SWORKAKVATEST(dGtimers, dGparam);//инициализация таблиц таймеров и таблиц параметров
            AKVApar = new SAKVApar();
            LoadSaveTable.LoadDataGridViewFromCsv(dGparam, true, Proper,true);//загрузка последнего сохранённого варианта таблицы параметров




            cBtimeSpeed.SelectedIndex = 0;
            tabControl1.SelectedIndex = 2;
            WORKAKVATEST.UpdateComboBoxRejak(cBrej);


            this.Text = "DrawConfig  " + Proper.Version;
            LBoxInterface.Items.Clear();

            cBrej.SelectedIndex = 0;
            cBrejSimulator.SelectedIndex = 0;

            if (CANHUB.IsPortOpen(listComPort.Items))
            {
                Scan_and_OpenHUBTFTCOMport(Proper.COMportName, Proper.COMportBaud, cBoxEnMess.Checked);
                SelectComPort(Proper.COMportName, listComPort);
                // Подписываемся на событие DataReceivedEvent
                CANHUB.DataReceivedEvent += OnDataReceived; // Обработчик события
                CANHUB.StartReceiveThread();
            }

           // WORKAKVATEST
         }



        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {//СЧИТЫВАНИЕ И ОБРАБОТКА ДАННЫХ, ПРИНЯТЫХ В РЕЖИМЕ SLAVE по по RS-485
            ErejAKVA receiveAKVA= WORKAKVATEST.selectedMode;
            // Поскольку событие может быть вызвано из другого потока, используем Invoke
            if (InvokeRequired)
            {
            // *************  !!!  РУЧНОЕ ИЗМЕНЕНИЕ ДАННЫХ ИМЕЕТ АБСОЛЮТНЫЙ ПРИОРИТЕТ  !!! *********
                if (WORKAKVATEST.HandlAKVAchange >= 0) 
                {//попыка ВРУЧНУЮ с помощью ComboBox sBrej изменить режим работы
                 //отрисовка выеделения нового столбца таблицы
                    Invoke(new Action(() => WORKAKVATEST.SelectColumn(dGparam, WORKAKVATEST.HandlAKVAchange)));
                    Invoke(new Action(() => WORKAKVATEST.SetNewRej(WORKAKVATEST.HandlAKVAchange)));
                 // извлечение данных структуры AKVAPAR из таблицы и отправка в TFT контроллер
                        Invoke(new Action(() => AKVApar.LoadFromDataGridViewColumn(dGparam, WORKAKVATEST.HandlAKVAchange)));// извлекаем  данные из таблицы в структуру AKVAPAR 
                        byte[] arrAKVAPAR = AKVApar.AKVAPARtoByteArray();//копируем данные в массив
                                                                         
                        WORKAKVATEST.HandlAKVAchange = -1;
                    //отправляем структуру в TFT контроллер без требования ответа
                    CANHUB.CommSendAnsv(ECommand.cmd_ExhParams, Efl_DEV.fld_TFTboard, arrAKVAPAR, 0);
                    return;
                }
                
                //обработка данных пришедших от TFT контроллера
                byte[] RXdata = new byte[e.Data.Length - 6];//создаём массив для хранения данных
                Array.Copy(e.Data, 4, RXdata, 0, e.Data.Length - 6);
                ECommand Comm = (ECommand)(e.Data[1]);//извлекаем команду

                if (Comm == ECommand.cmd_exhSimulator)
                {//обрабатываем принятую по RS-485 посылку данных                   
                    int Rejim = e.Data[4];//копируем режим принятых данных
                    byte[] SIMULdata = new byte[e.Data.Length - 7];//массив для "чистых данных" симулятора
                    Array.Copy(e.Data, 5, SIMULdata, 0, e.Data.Length - 7);//копируем в него ЧИСТЫЕ принятые данные без заголовка CRC и режима
                    switch (Rejim)
                    {
  /*                      case 9://приём и установка нового режима работы

                            break;*/
                        case 10://приём и отображение данных о всех таймерах и ответ в зависимости от того какие параметры были изменены

                            //ОБНОВЛЕНИЕ СТРУКТУРЫ ТАЙМЕРОВ производим только в случае, если не было изменений вручную, если были изменения будут учтены при следующем приходе данных через секунду
                            WORKAKVATEST.TimersParFromByteArray(SIMULdata);//обновление структуры таймеров
                            Invoke(new Action(() => WORKAKVATEST.DisplayInTimersGridView()));
                            break;
                    }
                    return;
                }
                if (Comm == ECommand.cmd_ExhParams)
                {//выполнение команды содержащей текущий режим работы  TFT контроллера от TFT контроллера
                    {//данные из пришедшей посылки от TFT контроллера находятся в RXdata
                     // Конвертируем массив байтов в 32-битное целое число
                        int _Rejim = BitConverter.ToInt32(RXdata, 0);
                        receiveAKVA = (ErejAKVA)_Rejim;
                        if (WORKAKVATEST.selectedMode != receiveAKVA)
                        {//TFTконтроллер сменил режим работы
                            WORKAKVATEST.AKVAint = AKVApar.GetNumVol(receiveAKVA);//устанавливаем новый номер строки в таблице
                            WORKAKVATEST.NewAKVAint = WORKAKVATEST.AKVAint;//требуется выделить в таблице новый столбец!
                            WORKAKVATEST.selectedMode = receiveAKVA;//устанавливаем новый режим работы
                            //режим работы сменили

                        }

                     // извлечение данных структуры AKVAPAR из таблицы и отправка в TFT контроллер
                        Invoke(new Action(() => AKVApar.LoadFromDataGridViewColumn(dGparam, AKVApar.GetNumVol(WORKAKVATEST.selectedMode))));// извлекаем  данные из таблицы в структуру AKVAPAR 
                        byte[] arrAKVAPAR = AKVApar.AKVAPARtoByteArray();//копируем данные в массив
                                                                         //отправляем структуру в TFT контроллер без требования ответа
                        WORKAKVATEST.NewAKVAint = -1;
                        CANHUB.CommSendAnsv(ECommand.cmd_ExhParams, Efl_DEV.fld_TFTboard, arrAKVAPAR, 0);
                    }
                }

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
                Proper.DirGrafFiles = TBSourseDir.Text;
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
                Proper.DirResultGrafFile = tBresultPath.Text;
            }

        }

        private void bGRAFdir_Click(object sender, EventArgs e)
        {

 FolderBrowserDialog FolderResult = new FolderBrowserDialog();
             string dirName = Proper.FolderGRAF;
             if (FolderResult.ShowDialog() == DialogResult.OK)
             {
                 tBgrafDIR.Text = FolderResult.SelectedPath;
                Proper.FolderGRAF = tBgrafDIR.Text;
             }
        }



        private void bSaveConf_Click(object sender, EventArgs e)
        {
           // SaveDefaultSetting();
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

            int index = listComPort.Items.IndexOf(Proper.COMportName);
            if (index == -1)
            {
                listComPort.SelectedIndex = 0;
            }
            else
            {
                listComPort.SelectedIndex = index;
            }

            if (CANHUB.GetOpenComport(listComPort.SelectedItem.ToString(), Proper.COMportBaud, true))
            //    if (Sens.GetOpenComport(Properties.Settings.Default.COMportName, Properties.Settings.Default.COMportBaud, false))
            {
                Proper.COMportName = CANHUB.PortName;
                SLprocess.Text = Proper.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
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
                    this.Text = "TFTprog " + Proper.Version + " ©С - Progchip" + " (" + "версия платы: " + 1.00/*DCOM.parBoardVerHard*/ + "  версия микрокода: " + 1.00/*DCOM.parBoardVerSoft*/ + " )";
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
            if (CANHUB.GetOpenComport(listComPort.SelectedItem.ToString(), Proper.COMportBaud, false))

            {

                SLprocess.Text = Proper.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
                try

                {
                   // вставить сюда команду считывания версии  DCOM.WRUFComm(0);
                    if (Proper.COMportName != CANHUB.PortName)
                        Proper.COMportName= CANHUB.PortName;

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
            

            if (CANHUB.GetOpenComport(itemText, Proper.COMportBaud, false))
            {// пытаемся соединиться с устройством посредством выделенного порта
                try
                {
                    int i = 0;
                    string tmpStr;

                    tmpStr = TryOpenDev(CANHUB.MainBoard, Efl_DEV.fld_MainBoard, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                    }


                    tmpStr = TryOpenDev(CANHUB.CAN_HUB, Efl_DEV.fld_HUB, false);
                    if (tmpStr != "")
                    {
                        tmpStr = CANHUB.ConcatenateStrings(tmpStr, "", "");
                        LBoxInterface.Items.Add(tmpStr);
                        i++;
                        Proper.COMportName= itemText;
                    }




                    SLprocess.Text = itemText + "  Baud:" + Proper.COMportBaud;
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

            if (CANHUB.GetOpenComport(listComPort.SelectedItem.ToString(), Proper.COMportBaud, true))
            //    if (Sens.GetOpenComport(Properties.Settings.Default.COMportName, Properties.Settings.Default.COMportBaud, false))
            {
                Proper.COMportName = CANHUB.PortName;
                SLprocess.Text = Proper.COMportName + "  Baud:" + CANHUB.BaudRate.ToString();
                try

                {
                    CANHUB.GetVerDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard,true);
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
                button1.Text=CANHUB.GetVerDev(CANHUB.TFT_Board, Efl_DEV.fld_TFTboard,true);
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
            
            foreach (DataGridViewColumn column in dGparam.Columns)
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
                string DIR_FlowPro = Proper.FolderGRAF;//директория;
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
            string DIR_FlowPro = Proper.FolderGRAF;
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
                    string[] FONTfilenames = GRAF_FILES.Init_WRtoFLASHfiles(Proper.FolderGRAF, "FONT");//получаем список файлов директории c фонтами
                    string[] PICTfilenames = GRAF_FILES.Init_WRtoFLASHfiles(Proper.FolderGRAF, "PICT");//получаем список файлов директории с картинками


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

                GRAF_FILES.Init_ArrFLASHaddr_MENU(Proper.FolderGRAF, CANHUB.StartFLASHadr, GRAF_FILES.const_sizeTFTFLASHalign, GRAF_FILES.const_AddLenFileSize);
                GRAF_FILES.Init_FONTE_PICT(Proper.FolderGRAF, CANHUB.StartFLASHadr, CANHUB.adrBASEMenuGPUpict);

 //               GRAF_FILES.Init_MENU_enum(Properties.Settings.Default.FolderGRAF, 0x100000);//load filenames
                GRAF_FILES.codeMENUcreater_(Proper.FolderGRAF);

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


        private void bParamRead_Click(object sender, EventArgs e)
        {

            LoadSaveTable.LoadDataGridViewFromCsv(dGparam, true,Proper,false);
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
            LoadSaveTable.SaveTableWithFileDialog(dGparam, true,Proper);
        }

        private void listComPort_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
//            SaveDefaultSetting();
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
  //          if (WORKAKVATEST.isUpdating) return; // Предотвращаем самоблокировку

            ComboBox sBrej = sender as ComboBox;
 //           DataGridView dGparam = Controls["dGparam"] as DataGridView;

            int selectedColumn = sBrej.SelectedIndex;

            if (selectedColumn != WORKAKVATEST.AKVAint)
            {
                if (selectedColumn >= 0 && selectedColumn < dGparam.ColumnCount)
                {
 //              !!! ВСЕ ОТРИСОВКИ ТАБЛИЦЫ ПАРАМЕТРОВ ПРОИВЗОДЯТСЯ В СОБЫИИ ОБНОВЛЕНИЯ !!!     WORKAKVATEST.SelectColumn(dGparam, selectedColumn);
 //                   WORKAKVATEST.SetNewRej(selectedColumn);
                    WORKAKVATEST.HandlAKVAchange = selectedColumn; // Обновляем глобальную переменну
                }
            }
        }


        private void DGparam_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
 /*           if (WORKAKVATEST.isUpdating) return; // Предотвращаем самоблокировку

 //           DataGridView dGparam = sender as DataGridView;
            ComboBox sBrej = Controls["sBrej"] as ComboBox;

            if (e.ColumnIndex != WORKAKVATEST.AKVAint)
            {
                WORKAKVATEST.AKVAint = e.ColumnIndex; // Обновляем глобальную переменную
                WORKAKVATEST.NewAKVAint = WORKAKVATEST.AKVAint;//обновляем переменную для передачи данных об изменении режима работы в прибор
                WORKAKVATEST.isUpdating = true;
                sBrej.SelectedIndex = WORKAKVATEST.AKVAint; // Синхронизируем ComboBox

                WORKAKVATEST.isUpdating = false;
            }*/
        }


        private void cBrejSimulator_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (cBrejSimulator.SelectedIndex)
                {
                    case 0:
                        if (CANHUB.IsOpen)
                            CANHUB.ChangeDEVExhRejWork(ERejWork.ervTFT_master, Efl_DEV.fld_TFTboard);
                        break;
                    case 1:
                        if (CANHUB.IsOpen)
                            CANHUB.ChangeDEVExhRejWork(ERejWork.evrPCsimulator, Efl_DEV.fld_TFTboard);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Возникла ошибка при попытке сменить режим работы", "Не удаётся сменить режим работы",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void butTest_Click_1(object sender, EventArgs e)
        {
            AKVApar.LoadFromDataGridViewColumn(dGparam, 0);
            AKVApar.PutGridViewColumn(dGparam, 0);
        }
    }


    #region TablesLoadSave

    public static class LoadSaveTable
    {

        public static void LoadDataGridViewFromCsv(DataGridView dataGridView, bool loadHeader, SParameterManager Prop, bool AvtoLoad)
        {
            dataGridView.AllowUserToAddRows = false;

            string filePath = Prop.LastTableFile;

            if (AvtoLoad && File.Exists(filePath))
            {
                // Если AvtoLoad == true и файл существует, загружаем его без показа диалога
                LoadFile(filePath, dataGridView, loadHeader, Prop, AvtoLoad);
            }
            else
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                    openFileDialog.FileName = Prop.LastTableFile;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                        LoadFile(filePath, dataGridView, loadHeader, Prop, AvtoLoad);
                    }
                }
            }
        }

        private static void LoadFile(string filePath, DataGridView dataGridView, bool loadHeader, SParameterManager Prop, bool AvtoLoad)
        {
            try
            {
                dataGridView.Rows.Clear();
                if (loadHeader)
                    dataGridView.Columns.Clear();

                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    bool isFirstLine = true;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(';');

                        if (isFirstLine && loadHeader)
                        {
                            // Добавляем столбцы (верхний заголовок)
                            for (int i = 1; i < values.Length; i++) // Начинаем с 1, чтобы пропустить левый заголовок
                            {
                                dataGridView.Columns.Add(values[i], values[i]);
                            }
                            isFirstLine = false;
                        }
                        else
                        {
                            // Создаем новую строку
                            DataGridViewRow newRow = new DataGridViewRow();
                            newRow.CreateCells(dataGridView);

                            // Устанавливаем левый заголовок строки
                            if (values.Length > 0)
                            {
                                newRow.HeaderCell.Value = values[0]; // Левый заголовок строки
                            }

                            // Заполняем данные строки
                            for (int i = 1; i < values.Length; i++) // Начинаем с 1, чтобы пропустить левый заголовок
                            {
                                newRow.Cells[i - 1].Value = values[i];
                            }

                            dataGridView.Rows.Add(newRow);
                        }
                    }
                }

                // Отключаем сортировку для всех столбцов
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                Prop.LastTableFile = filePath;

                if (!AvtoLoad)
                {
                    MessageBox.Show("File loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void SaveDataGridViewToCsv(DataGridView dataGridView, bool saveHeader, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Сохраняем верхний заголовок (столбцы)
                if (saveHeader)
                {
                    writer.Write(";"); // Пустая ячейка для заголовка строк
                    for (int i = 0; i < dataGridView.Columns.Count; i++)
                    {
                        writer.Write(dataGridView.Columns[i].HeaderText);
                        if (i < dataGridView.Columns.Count - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                }

                // Сохраняем данные строк
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        // Левый заголовок строки
                        writer.Write(row.HeaderCell.Value?.ToString() ?? string.Empty);
                        writer.Write(";");

                        // Данные строки
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            writer.Write(row.Cells[i].Value?.ToString() ?? string.Empty);
                            if (i < dataGridView.Columns.Count - 1)
                                writer.Write(";");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }


        public static void SaveTableWithFileDialog(DataGridView dataGridView, bool saveHeader, SParameterManager Prop)
        {
            string defaultFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tables");
            string defaultFileName = "ParamTable.csv";

            // Убедиться, что папка Debug существует
            if (!Directory.Exists(defaultFolder))
            {
                Directory.CreateDirectory(defaultFolder);
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = defaultFolder; // Устанавливаем папку по умолчанию
                saveFileDialog.FileName = defaultFileName;       // Устанавливаем имя файла по умолчанию
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv"; // Фильтр для файлов

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = saveFileDialog.FileName;

                    try
                    {
                        // Сохранение данных в CSV
                        SaveDataGridViewToCsv(dataGridView, saveHeader, selectedFilePath);

                        // Сохранить имя файла в настройках
                        Prop.LastTableFile = selectedFilePath;

                        MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    }

    #endregion



}
