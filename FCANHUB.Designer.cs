namespace TFTprog
{
    partial class FormHUB
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lHEX2 = new System.Windows.Forms.Label();
            this.TBSourseDir = new System.Windows.Forms.TextBox();
            this.bSEL2 = new System.Windows.Forms.Button();
            this.lHEXres = new System.Windows.Forms.Label();
            this.tBresultPath = new System.Windows.Forms.TextBox();
            this.bRES = new System.Windows.Forms.Button();
            this.tBCode = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPagePack = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SLprocess = new System.Windows.Forms.ToolStripStatusLabel();
            this.PBFileToMem = new System.Windows.Forms.ToolStripProgressBar();
            this.SLfileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.PBwrFilePro = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bwr_tft_flash = new System.Windows.Forms.Button();
            this.bCreatCode = new System.Windows.Forms.Button();
            this.tBendAdr = new System.Windows.Forms.TextBox();
            this.tBstartAdr = new System.Windows.Forms.TextBox();
            this.lBendFLASH = new System.Windows.Forms.Label();
            this.lbStartFLASH = new System.Windows.Forms.Label();
            this.tBcodefilename = new System.Windows.Forms.TextBox();
            this.bCreatFiles = new System.Windows.Forms.Button();
            this.bSaveConf = new System.Windows.Forms.Button();
            this.tBresultFilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labCODE = new System.Windows.Forms.Label();
            this.bClear = new System.Windows.Forms.Button();
            this.gBSaveCode = new System.Windows.Forms.GroupBox();
            this.gBsaveOneFile = new System.Windows.Forms.GroupBox();
            this.tBNameOneFile = new System.Windows.Forms.TextBox();
            this.lbNameOneFile = new System.Windows.Forms.Label();
            this.bSaveOneFile = new System.Windows.Forms.Button();
            this.bStopTest = new System.Windows.Forms.Button();
            this.bProcessCreater = new System.Windows.Forms.Button();
            this.bTestFileToFLASH = new System.Windows.Forms.Button();
            this.cBmenu = new System.Windows.Forms.CheckBox();
            this.cBfontPict = new System.Windows.Forms.CheckBox();
            this.lCreater = new System.Windows.Forms.Label();
            this.bGRAFdir = new System.Windows.Forms.Button();
            this.tBgrafDIR = new System.Windows.Forms.TextBox();
            this.lSOURCE = new System.Windows.Forms.Label();
            this.tabPageBTAddr = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridTimers = new System.Windows.Forms.DataGridView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tP_TFT_WR_RD = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.gBtestWRgraf = new System.Windows.Forms.GroupBox();
            this.lbNum_BAD = new System.Windows.Forms.Label();
            this.lbNumGOOD = new System.Windows.Forms.Label();
            this.lbCountBad = new System.Windows.Forms.Label();
            this.lbCountGood = new System.Windows.Forms.Label();
            this.bStopTestWrPict = new System.Windows.Forms.Button();
            this.bStartTestWrPict = new System.Windows.Forms.Button();
            this.cBLoadPict = new System.Windows.Forms.CheckBox();
            this.buttestSw = new System.Windows.Forms.Button();
            this.cBoxEnMess = new System.Windows.Forms.CheckBox();
            this.DevLabel = new System.Windows.Forms.Label();
            this.bTestMainBoardRS485 = new System.Windows.Forms.Button();
            this.LBoxInterface = new System.Windows.Forms.ListBox();
            this.bTestProWRFLASH = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listComPort = new System.Windows.Forms.ListBox();
            this.bCOMselect = new System.Windows.Forms.Button();
            this.bfindPort = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gBchangePar = new System.Windows.Forms.GroupBox();
            this.cBrejSimulator = new System.Windows.Forms.ComboBox();
            this.bCalibr = new System.Windows.Forms.Button();
            this.gBdateTime = new System.Windows.Forms.GroupBox();
            this.lspeederTIME = new System.Windows.Forms.Label();
            this.cBtimeSpeed = new System.Windows.Forms.ComboBox();
            this.bSetBOARDwinDT = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.bSetBoardDT = new System.Windows.Forms.Button();
            this.LBDTread = new System.Windows.Forms.Label();
            this.lBDT = new System.Windows.Forms.Label();
            this.bParamRead = new System.Windows.Forms.Button();
            this.labRej = new System.Windows.Forms.Label();
            this.bParamWrite = new System.Windows.Forms.Button();
            this.cBrej = new System.Windows.Forms.ComboBox();
            this.dGtimers = new System.Windows.Forms.DataGridView();
            this.b_PARtableLoad = new System.Windows.Forms.Button();
            this.b_PARtableSave = new System.Windows.Forms.Button();
            this.b_newPARfilename = new System.Windows.Forms.Button();
            this.lbFT1divFT0mult100 = new System.Windows.Forms.Label();
            this.lbFT0minusFT1 = new System.Windows.Forms.Label();
            this.tB_PARfileMan = new System.Windows.Forms.TextBox();
            this.dGparam = new System.Windows.Forms.DataGridView();
            this.WRpro = new System.ComponentModel.BackgroundWorker();
            this.labRejSimulator = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPagePack.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gBSaveCode.SuspendLayout();
            this.gBsaveOneFile.SuspendLayout();
            this.tabPageBTAddr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTimers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tP_TFT_WR_RD.SuspendLayout();
            this.gBtestWRgraf.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gBchangePar.SuspendLayout();
            this.gBdateTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGtimers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGparam)).BeginInit();
            this.SuspendLayout();
            // 
            // lHEX2
            // 
            this.lHEX2.AutoSize = true;
            this.lHEX2.Location = new System.Drawing.Point(810, 394);
            this.lHEX2.Name = "lHEX2";
            this.lHEX2.Size = new System.Drawing.Size(72, 13);
            this.lHEX2.TabIndex = 1;
            this.lHEX2.Text = "Sourse PATH";
            // 
            // TBSourseDir
            // 
            this.TBSourseDir.Location = new System.Drawing.Point(813, 315);
            this.TBSourseDir.Name = "TBSourseDir";
            this.TBSourseDir.Size = new System.Drawing.Size(55, 20);
            this.TBSourseDir.TabIndex = 3;
            // 
            // bSEL2
            // 
            this.bSEL2.Location = new System.Drawing.Point(797, 32);
            this.bSEL2.Name = "bSEL2";
            this.bSEL2.Size = new System.Drawing.Size(197, 23);
            this.bSEL2.TabIndex = 5;
            this.bSEL2.Text = "Создать файл фонта";
            this.bSEL2.UseVisualStyleBackColor = true;
            this.bSEL2.Click += new System.EventHandler(this.bSEL2_Click);
            // 
            // lHEXres
            // 
            this.lHEXres.AutoSize = true;
            this.lHEXres.Location = new System.Drawing.Point(722, 394);
            this.lHEXres.Name = "lHEXres";
            this.lHEXres.Size = new System.Drawing.Size(82, 13);
            this.lHEXres.TabIndex = 6;
            this.lHEXres.Text = "RESULT PATH";
            // 
            // tBresultPath
            // 
            this.tBresultPath.Location = new System.Drawing.Point(813, 359);
            this.tBresultPath.Name = "tBresultPath";
            this.tBresultPath.Size = new System.Drawing.Size(55, 20);
            this.tBresultPath.TabIndex = 7;
            // 
            // bRES
            // 
            this.bRES.Location = new System.Drawing.Point(735, 359);
            this.bRES.Name = "bRES";
            this.bRES.Size = new System.Drawing.Size(72, 23);
            this.bRES.TabIndex = 8;
            this.bRES.Text = "SELECT";
            this.bRES.UseVisualStyleBackColor = true;
            this.bRES.Click += new System.EventHandler(this.bRES_Click);
            // 
            // tBCode
            // 
            this.tBCode.Location = new System.Drawing.Point(21, 267);
            this.tBCode.Multiline = true;
            this.tBCode.Name = "tBCode";
            this.tBCode.Size = new System.Drawing.Size(657, 181);
            this.tBCode.TabIndex = 12;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPagePack);
            this.tabControl1.Controls.Add(this.tabPageBTAddr);
            this.tabControl1.Controls.Add(this.tP_TFT_WR_RD);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1027, 691);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 17;
            // 
            // tabPagePack
            // 
            this.tabPagePack.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPagePack.Controls.Add(this.button3);
            this.tabPagePack.Controls.Add(this.statusStrip1);
            this.tabPagePack.Controls.Add(this.groupBox2);
            this.tabPagePack.Controls.Add(this.gBSaveCode);
            this.tabPagePack.Controls.Add(this.tBCode);
            this.tabPagePack.Controls.Add(this.TBSourseDir);
            this.tabPagePack.Controls.Add(this.tBresultPath);
            this.tabPagePack.Controls.Add(this.bRES);
            this.tabPagePack.Controls.Add(this.bSEL2);
            this.tabPagePack.Controls.Add(this.lHEX2);
            this.tabPagePack.Controls.Add(this.lHEXres);
            this.tabPagePack.Location = new System.Drawing.Point(4, 22);
            this.tabPagePack.Name = "tabPagePack";
            this.tabPagePack.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePack.Size = new System.Drawing.Size(1019, 665);
            this.tabPagePack.TabIndex = 0;
            this.tabPagePack.Text = "Запись изображений";
            this.tabPagePack.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(797, 63);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(197, 23);
            this.button3.TabIndex = 24;
            this.button3.Text = "Сравнить файлы фонтов";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SLprocess,
            this.PBFileToMem,
            this.SLfileName,
            this.PBwrFilePro});
            this.statusStrip1.Location = new System.Drawing.Point(3, 636);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1009, 22);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SLprocess
            // 
            this.SLprocess.Name = "SLprocess";
            this.SLprocess.Size = new System.Drawing.Size(37, 17);
            this.SLprocess.Text = "SLpro";
            // 
            // PBFileToMem
            // 
            this.PBFileToMem.Name = "PBFileToMem";
            this.PBFileToMem.Size = new System.Drawing.Size(100, 16);
            this.PBFileToMem.Step = 1;
            this.PBFileToMem.Click += new System.EventHandler(this.PBFileToMem_Click);
            // 
            // SLfileName
            // 
            this.SLfileName.Name = "SLfileName";
            this.SLfileName.Size = new System.Drawing.Size(57, 17);
            this.SLfileName.Text = "FileName";
            // 
            // PBwrFilePro
            // 
            this.PBwrFilePro.Name = "PBwrFilePro";
            this.PBwrFilePro.Size = new System.Drawing.Size(100, 16);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bwr_tft_flash);
            this.groupBox2.Controls.Add(this.bCreatCode);
            this.groupBox2.Controls.Add(this.tBendAdr);
            this.groupBox2.Controls.Add(this.tBstartAdr);
            this.groupBox2.Controls.Add(this.lBendFLASH);
            this.groupBox2.Controls.Add(this.lbStartFLASH);
            this.groupBox2.Controls.Add(this.tBcodefilename);
            this.groupBox2.Controls.Add(this.bCreatFiles);
            this.groupBox2.Controls.Add(this.bSaveConf);
            this.groupBox2.Controls.Add(this.tBresultFilename);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.labCODE);
            this.groupBox2.Controls.Add(this.bClear);
            this.groupBox2.Location = new System.Drawing.Point(21, 453);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(774, 139);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            // 
            // bwr_tft_flash
            // 
            this.bwr_tft_flash.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bwr_tft_flash.ForeColor = System.Drawing.Color.Red;
            this.bwr_tft_flash.Location = new System.Drawing.Point(591, 93);
            this.bwr_tft_flash.Name = "bwr_tft_flash";
            this.bwr_tft_flash.Size = new System.Drawing.Size(161, 31);
            this.bwr_tft_flash.TabIndex = 22;
            this.bwr_tft_flash.Text = "WRITE TFT FLASH";
            this.bwr_tft_flash.UseVisualStyleBackColor = true;
            this.bwr_tft_flash.Click += new System.EventHandler(this.bwr_tft_flash_Click);
            // 
            // bCreatCode
            // 
            this.bCreatCode.Enabled = false;
            this.bCreatCode.Location = new System.Drawing.Point(626, 54);
            this.bCreatCode.Name = "bCreatCode";
            this.bCreatCode.Size = new System.Drawing.Size(126, 27);
            this.bCreatCode.TabIndex = 21;
            this.bCreatCode.Text = "Creat CODE files";
            this.bCreatCode.UseVisualStyleBackColor = true;
            this.bCreatCode.Click += new System.EventHandler(this.bCreatCode_Click);
            // 
            // tBendAdr
            // 
            this.tBendAdr.Location = new System.Drawing.Point(457, 90);
            this.tBendAdr.Name = "tBendAdr";
            this.tBendAdr.Size = new System.Drawing.Size(96, 20);
            this.tBendAdr.TabIndex = 20;
            // 
            // tBstartAdr
            // 
            this.tBstartAdr.Location = new System.Drawing.Point(239, 90);
            this.tBstartAdr.Name = "tBstartAdr";
            this.tBstartAdr.Size = new System.Drawing.Size(96, 20);
            this.tBstartAdr.TabIndex = 19;
            // 
            // lBendFLASH
            // 
            this.lBendFLASH.AutoSize = true;
            this.lBendFLASH.Location = new System.Drawing.Point(375, 96);
            this.lBendFLASH.Name = "lBendFLASH";
            this.lBendFLASH.Size = new System.Drawing.Size(76, 13);
            this.lBendFLASH.TabIndex = 18;
            this.lBendFLASH.Text = "EndAdrFLASH";
            // 
            // lbStartFLASH
            // 
            this.lbStartFLASH.AutoSize = true;
            this.lbStartFLASH.Location = new System.Drawing.Point(148, 96);
            this.lbStartFLASH.Name = "lbStartFLASH";
            this.lbStartFLASH.Size = new System.Drawing.Size(85, 13);
            this.lbStartFLASH.TabIndex = 17;
            this.lbStartFLASH.Text = "StartAddrFLASH";
            // 
            // tBcodefilename
            // 
            this.tBcodefilename.Location = new System.Drawing.Point(174, 19);
            this.tBcodefilename.Name = "tBcodefilename";
            this.tBcodefilename.Size = new System.Drawing.Size(259, 20);
            this.tBcodefilename.TabIndex = 13;
            // 
            // bCreatFiles
            // 
            this.bCreatFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bCreatFiles.Location = new System.Drawing.Point(626, 19);
            this.bCreatFiles.Name = "bCreatFiles";
            this.bCreatFiles.Size = new System.Drawing.Size(126, 27);
            this.bCreatFiles.TabIndex = 11;
            this.bCreatFiles.Text = "CREAT FILES";
            this.bCreatFiles.UseVisualStyleBackColor = true;
            this.bCreatFiles.Click += new System.EventHandler(this.bRUN_Click);
            // 
            // bSaveConf
            // 
            this.bSaveConf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bSaveConf.Location = new System.Drawing.Point(454, 51);
            this.bSaveConf.Name = "bSaveConf";
            this.bSaveConf.Size = new System.Drawing.Size(99, 25);
            this.bSaveConf.TabIndex = 16;
            this.bSaveConf.Text = "Save config";
            this.bSaveConf.UseVisualStyleBackColor = true;
            this.bSaveConf.Click += new System.EventHandler(this.bSaveConf_Click);
            // 
            // tBresultFilename
            // 
            this.tBresultFilename.Location = new System.Drawing.Point(128, 56);
            this.tBresultFilename.Name = "tBresultFilename";
            this.tBresultFilename.Size = new System.Drawing.Size(305, 20);
            this.tBresultFilename.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "RESULT FILENAME";
            // 
            // labCODE
            // 
            this.labCODE.AutoSize = true;
            this.labCODE.Location = new System.Drawing.Point(16, 22);
            this.labCODE.Name = "labCODE";
            this.labCODE.Size = new System.Drawing.Size(139, 13);
            this.labCODE.TabIndex = 14;
            this.labCODE.Text = "RESULT CODE FILENAME";
            // 
            // bClear
            // 
            this.bClear.Location = new System.Drawing.Point(480, 12);
            this.bClear.Name = "bClear";
            this.bClear.Size = new System.Drawing.Size(73, 23);
            this.bClear.TabIndex = 15;
            this.bClear.Text = "Clear text";
            this.bClear.UseVisualStyleBackColor = true;
            this.bClear.Click += new System.EventHandler(this.bClear_Click);
            // 
            // gBSaveCode
            // 
            this.gBSaveCode.Controls.Add(this.gBsaveOneFile);
            this.gBSaveCode.Controls.Add(this.bStopTest);
            this.gBSaveCode.Controls.Add(this.bProcessCreater);
            this.gBSaveCode.Controls.Add(this.bTestFileToFLASH);
            this.gBSaveCode.Controls.Add(this.cBmenu);
            this.gBSaveCode.Controls.Add(this.cBfontPict);
            this.gBSaveCode.Controls.Add(this.lCreater);
            this.gBSaveCode.Controls.Add(this.bGRAFdir);
            this.gBSaveCode.Controls.Add(this.tBgrafDIR);
            this.gBSaveCode.Controls.Add(this.lSOURCE);
            this.gBSaveCode.Location = new System.Drawing.Point(21, 6);
            this.gBSaveCode.Name = "gBSaveCode";
            this.gBSaveCode.Size = new System.Drawing.Size(752, 239);
            this.gBSaveCode.TabIndex = 18;
            this.gBSaveCode.TabStop = false;
            // 
            // gBsaveOneFile
            // 
            this.gBsaveOneFile.Controls.Add(this.tBNameOneFile);
            this.gBsaveOneFile.Controls.Add(this.lbNameOneFile);
            this.gBsaveOneFile.Controls.Add(this.bSaveOneFile);
            this.gBsaveOneFile.Location = new System.Drawing.Point(19, 140);
            this.gBsaveOneFile.Name = "gBsaveOneFile";
            this.gBsaveOneFile.Size = new System.Drawing.Size(716, 71);
            this.gBsaveOneFile.TabIndex = 27;
            this.gBsaveOneFile.TabStop = false;
            this.gBsaveOneFile.Text = "Перезаписать или добавить в конец области записи (номер файла должен быть больше " +
    "последнего записанного) один файл";
            // 
            // tBNameOneFile
            // 
            this.tBNameOneFile.Location = new System.Drawing.Point(12, 32);
            this.tBNameOneFile.Name = "tBNameOneFile";
            this.tBNameOneFile.Size = new System.Drawing.Size(284, 20);
            this.tBNameOneFile.TabIndex = 32;
            // 
            // lbNameOneFile
            // 
            this.lbNameOneFile.AutoSize = true;
            this.lbNameOneFile.Location = new System.Drawing.Point(435, 35);
            this.lbNameOneFile.Name = "lbNameOneFile";
            this.lbNameOneFile.Size = new System.Drawing.Size(177, 13);
            this.lbNameOneFile.TabIndex = 31;
            this.lbNameOneFile.Text = "Полное имя файла, включая путь";
            // 
            // bSaveOneFile
            // 
            this.bSaveOneFile.Location = new System.Drawing.Point(629, 27);
            this.bSaveOneFile.Name = "bSaveOneFile";
            this.bSaveOneFile.Size = new System.Drawing.Size(72, 28);
            this.bSaveOneFile.TabIndex = 26;
            this.bSaveOneFile.Text = "Записать";
            this.bSaveOneFile.UseVisualStyleBackColor = true;
            this.bSaveOneFile.Click += new System.EventHandler(this.button2_Click);
            // 
            // bStopTest
            // 
            this.bStopTest.Location = new System.Drawing.Point(638, 67);
            this.bStopTest.Name = "bStopTest";
            this.bStopTest.Size = new System.Drawing.Size(97, 57);
            this.bStopTest.TabIndex = 25;
            this.bStopTest.Text = "BLOBrepair BT815";
            this.bStopTest.UseVisualStyleBackColor = true;
            this.bStopTest.Click += new System.EventHandler(this.bStopTest_Click);
            // 
            // bProcessCreater
            // 
            this.bProcessCreater.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bProcessCreater.Location = new System.Drawing.Point(268, 63);
            this.bProcessCreater.Name = "bProcessCreater";
            this.bProcessCreater.Size = new System.Drawing.Size(129, 23);
            this.bProcessCreater.TabIndex = 16;
            this.bProcessCreater.Text = "Cоздать";
            this.bProcessCreater.UseVisualStyleBackColor = true;
            this.bProcessCreater.Click += new System.EventHandler(this.bProcessCreater_Click);
            // 
            // bTestFileToFLASH
            // 
            this.bTestFileToFLASH.Location = new System.Drawing.Point(19, 101);
            this.bTestFileToFLASH.Name = "bTestFileToFLASH";
            this.bTestFileToFLASH.Size = new System.Drawing.Size(252, 23);
            this.bTestFileToFLASH.TabIndex = 24;
            this.bTestFileToFLASH.Text = "Процесс записи файлов в TFT FLASH";
            this.bTestFileToFLASH.UseVisualStyleBackColor = true;
            this.bTestFileToFLASH.Click += new System.EventHandler(this.bTestFileToFLASH_Click);
            // 
            // cBmenu
            // 
            this.cBmenu.AutoSize = true;
            this.cBmenu.Location = new System.Drawing.Point(466, 105);
            this.cBmenu.Name = "cBmenu";
            this.cBmenu.Size = new System.Drawing.Size(132, 17);
            this.cBmenu.TabIndex = 15;
            this.cBmenu.Text = "Изображения МЕНЮ";
            this.cBmenu.UseVisualStyleBackColor = true;
            this.cBmenu.CheckedChanged += new System.EventHandler(this.cBmenu_CheckedChanged);
            // 
            // cBfontPict
            // 
            this.cBfontPict.AutoSize = true;
            this.cBfontPict.Location = new System.Drawing.Point(294, 105);
            this.cBfontPict.Name = "cBfontPict";
            this.cBfontPict.Size = new System.Drawing.Size(127, 17);
            this.cBfontPict.TabIndex = 13;
            this.cBfontPict.Text = "Шрифты и картинки";
            this.cBfontPict.UseVisualStyleBackColor = true;
            this.cBfontPict.CheckedChanged += new System.EventHandler(this.cBCreatBEN_CheckedChanged);
            // 
            // lCreater
            // 
            this.lCreater.AutoSize = true;
            this.lCreater.Location = new System.Drawing.Point(12, 68);
            this.lCreater.Name = "lCreater";
            this.lCreater.Size = new System.Drawing.Size(227, 13);
            this.lCreater.TabIndex = 12;
            this.lCreater.Text = "Создание заголовочных файлов и структур";
            // 
            // bGRAFdir
            // 
            this.bGRAFdir.Location = new System.Drawing.Point(638, 25);
            this.bGRAFdir.Name = "bGRAFdir";
            this.bGRAFdir.Size = new System.Drawing.Size(99, 24);
            this.bGRAFdir.TabIndex = 11;
            this.bGRAFdir.UseVisualStyleBackColor = true;
            this.bGRAFdir.Click += new System.EventHandler(this.bGRAFdir_Click);
            // 
            // tBgrafDIR
            // 
            this.tBgrafDIR.Location = new System.Drawing.Point(116, 25);
            this.tBgrafDIR.Name = "tBgrafDIR";
            this.tBgrafDIR.Size = new System.Drawing.Size(516, 20);
            this.tBgrafDIR.TabIndex = 10;
            // 
            // lSOURCE
            // 
            this.lSOURCE.AutoSize = true;
            this.lSOURCE.Location = new System.Drawing.Point(12, 28);
            this.lSOURCE.Name = "lSOURCE";
            this.lSOURCE.Size = new System.Drawing.Size(87, 13);
            this.lSOURCE.TabIndex = 9;
            this.lSOURCE.Text = "GRAFsourceDIR";
            // 
            // tabPageBTAddr
            // 
            this.tabPageBTAddr.Controls.Add(this.button2);
            this.tabPageBTAddr.Controls.Add(this.dataGridTimers);
            this.tabPageBTAddr.Controls.Add(this.dataGridView1);
            this.tabPageBTAddr.Location = new System.Drawing.Point(4, 22);
            this.tabPageBTAddr.Name = "tabPageBTAddr";
            this.tabPageBTAddr.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBTAddr.Size = new System.Drawing.Size(1019, 665);
            this.tabPageBTAddr.TabIndex = 1;
            this.tabPageBTAddr.Text = "Тест";
            this.tabPageBTAddr.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(339, 560);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(158, 43);
            this.button2.TabIndex = 8;
            this.button2.Text = "TableRdWr Test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // dataGridTimers
            // 
            this.dataGridTimers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridTimers.Location = new System.Drawing.Point(8, 380);
            this.dataGridTimers.Name = "dataGridTimers";
            this.dataGridTimers.Size = new System.Drawing.Size(645, 86);
            this.dataGridTimers.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 37);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(750, 316);
            this.dataGridView1.TabIndex = 0;
            // 
            // tP_TFT_WR_RD
            // 
            this.tP_TFT_WR_RD.Controls.Add(this.textBox1);
            this.tP_TFT_WR_RD.Controls.Add(this.gBtestWRgraf);
            this.tP_TFT_WR_RD.Controls.Add(this.cBLoadPict);
            this.tP_TFT_WR_RD.Controls.Add(this.buttestSw);
            this.tP_TFT_WR_RD.Controls.Add(this.cBoxEnMess);
            this.tP_TFT_WR_RD.Controls.Add(this.DevLabel);
            this.tP_TFT_WR_RD.Controls.Add(this.bTestMainBoardRS485);
            this.tP_TFT_WR_RD.Controls.Add(this.LBoxInterface);
            this.tP_TFT_WR_RD.Controls.Add(this.bTestProWRFLASH);
            this.tP_TFT_WR_RD.Controls.Add(this.button1);
            this.tP_TFT_WR_RD.Controls.Add(this.listComPort);
            this.tP_TFT_WR_RD.Controls.Add(this.bCOMselect);
            this.tP_TFT_WR_RD.Controls.Add(this.bfindPort);
            this.tP_TFT_WR_RD.Location = new System.Drawing.Point(4, 22);
            this.tP_TFT_WR_RD.Name = "tP_TFT_WR_RD";
            this.tP_TFT_WR_RD.Padding = new System.Windows.Forms.Padding(3);
            this.tP_TFT_WR_RD.Size = new System.Drawing.Size(1019, 665);
            this.tP_TFT_WR_RD.TabIndex = 2;
            this.tP_TFT_WR_RD.Text = "Настройки";
            this.tP_TFT_WR_RD.UseVisualStyleBackColor = true;
            this.tP_TFT_WR_RD.Click += new System.EventHandler(this.tP_TFT_WR_RD_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(652, 325);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(174, 20);
            this.textBox1.TabIndex = 36;
            // 
            // gBtestWRgraf
            // 
            this.gBtestWRgraf.Controls.Add(this.lbNum_BAD);
            this.gBtestWRgraf.Controls.Add(this.lbNumGOOD);
            this.gBtestWRgraf.Controls.Add(this.lbCountBad);
            this.gBtestWRgraf.Controls.Add(this.lbCountGood);
            this.gBtestWRgraf.Controls.Add(this.bStopTestWrPict);
            this.gBtestWRgraf.Controls.Add(this.bStartTestWrPict);
            this.gBtestWRgraf.Location = new System.Drawing.Point(581, 13);
            this.gBtestWRgraf.Name = "gBtestWRgraf";
            this.gBtestWRgraf.Size = new System.Drawing.Size(283, 132);
            this.gBtestWRgraf.TabIndex = 35;
            this.gBtestWRgraf.TabStop = false;
            this.gBtestWRgraf.Text = "Тест записи графики в память панели";
            // 
            // lbNum_BAD
            // 
            this.lbNum_BAD.AutoSize = true;
            this.lbNum_BAD.Location = new System.Drawing.Point(180, 94);
            this.lbNum_BAD.Name = "lbNum_BAD";
            this.lbNum_BAD.Size = new System.Drawing.Size(13, 13);
            this.lbNum_BAD.TabIndex = 5;
            this.lbNum_BAD.Text = "0";
            // 
            // lbNumGOOD
            // 
            this.lbNumGOOD.AutoSize = true;
            this.lbNumGOOD.Location = new System.Drawing.Point(58, 95);
            this.lbNumGOOD.Name = "lbNumGOOD";
            this.lbNumGOOD.Size = new System.Drawing.Size(13, 13);
            this.lbNumGOOD.TabIndex = 4;
            this.lbNumGOOD.Text = "0";
            // 
            // lbCountBad
            // 
            this.lbCountBad.AutoSize = true;
            this.lbCountBad.Location = new System.Drawing.Point(164, 67);
            this.lbCountBad.Name = "lbCountBad";
            this.lbCountBad.Size = new System.Drawing.Size(54, 13);
            this.lbCountBad.TabIndex = 3;
            this.lbCountBad.Text = "CountBad";
            // 
            // lbCountGood
            // 
            this.lbCountGood.AutoSize = true;
            this.lbCountGood.Location = new System.Drawing.Point(36, 67);
            this.lbCountGood.Name = "lbCountGood";
            this.lbCountGood.Size = new System.Drawing.Size(61, 13);
            this.lbCountGood.TabIndex = 2;
            this.lbCountGood.Text = "CountGood";
            // 
            // bStopTestWrPict
            // 
            this.bStopTestWrPict.Location = new System.Drawing.Point(138, 32);
            this.bStopTestWrPict.Name = "bStopTestWrPict";
            this.bStopTestWrPict.Size = new System.Drawing.Size(107, 23);
            this.bStopTestWrPict.TabIndex = 1;
            this.bStopTestWrPict.Text = "StopTestWrPict";
            this.bStopTestWrPict.UseVisualStyleBackColor = true;
            this.bStopTestWrPict.Click += new System.EventHandler(this.bStopTestWrPict_Click);
            // 
            // bStartTestWrPict
            // 
            this.bStartTestWrPict.Location = new System.Drawing.Point(18, 32);
            this.bStartTestWrPict.Name = "bStartTestWrPict";
            this.bStartTestWrPict.Size = new System.Drawing.Size(101, 23);
            this.bStartTestWrPict.TabIndex = 0;
            this.bStartTestWrPict.Text = "StartTestWrPict";
            this.bStartTestWrPict.UseVisualStyleBackColor = true;
            this.bStartTestWrPict.Click += new System.EventHandler(this.bStartTestWrPict_Click);
            // 
            // cBLoadPict
            // 
            this.cBLoadPict.AutoSize = true;
            this.cBLoadPict.Location = new System.Drawing.Point(20, 209);
            this.cBLoadPict.Name = "cBLoadPict";
            this.cBLoadPict.Size = new System.Drawing.Size(114, 17);
            this.cBLoadPict.TabIndex = 34;
            this.cBLoadPict.Text = "StopExhTFTBoard";
            this.cBLoadPict.UseVisualStyleBackColor = true;
            this.cBLoadPict.CheckedChanged += new System.EventHandler(this.cBLoadPict_CheckedChanged);
            // 
            // buttestSw
            // 
            this.buttestSw.Location = new System.Drawing.Point(581, 152);
            this.buttestSw.Name = "buttestSw";
            this.buttestSw.Size = new System.Drawing.Size(283, 33);
            this.buttestSw.TabIndex = 33;
            this.buttestSw.Text = "Запустить калибровку TFT панели";
            this.buttestSw.UseVisualStyleBackColor = true;
            this.buttestSw.Click += new System.EventHandler(this.buttestSw_Click);
            // 
            // cBoxEnMess
            // 
            this.cBoxEnMess.AutoSize = true;
            this.cBoxEnMess.Location = new System.Drawing.Point(20, 185);
            this.cBoxEnMess.Name = "cBoxEnMess";
            this.cBoxEnMess.Size = new System.Drawing.Size(208, 17);
            this.cBoxEnMess.TabIndex = 32;
            this.cBoxEnMess.Text = "Отображать служебные сообщения";
            this.cBoxEnMess.UseVisualStyleBackColor = true;
            this.cBoxEnMess.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // DevLabel
            // 
            this.DevLabel.AutoSize = true;
            this.DevLabel.Location = new System.Drawing.Point(82, 13);
            this.DevLabel.Name = "DevLabel";
            this.DevLabel.Size = new System.Drawing.Size(231, 13);
            this.DevLabel.TabIndex = 31;
            this.DevLabel.Text = "Обнаруженные и подключенные устройства";
            // 
            // bTestMainBoardRS485
            // 
            this.bTestMainBoardRS485.Location = new System.Drawing.Point(331, 355);
            this.bTestMainBoardRS485.Name = "bTestMainBoardRS485";
            this.bTestMainBoardRS485.Size = new System.Drawing.Size(163, 42);
            this.bTestMainBoardRS485.TabIndex = 30;
            this.bTestMainBoardRS485.Text = "ConnectMainBoardRS485";
            this.bTestMainBoardRS485.UseVisualStyleBackColor = true;
            // 
            // LBoxInterface
            // 
            this.LBoxInterface.FormattingEnabled = true;
            this.LBoxInterface.Location = new System.Drawing.Point(20, 45);
            this.LBoxInterface.Name = "LBoxInterface";
            this.LBoxInterface.Size = new System.Drawing.Size(344, 134);
            this.LBoxInterface.TabIndex = 28;
            this.LBoxInterface.SelectedIndexChanged += new System.EventHandler(this.LBoxInterface_SelectedIndexChanged);
            // 
            // bTestProWRFLASH
            // 
            this.bTestProWRFLASH.Location = new System.Drawing.Point(326, 466);
            this.bTestProWRFLASH.Name = "bTestProWRFLASH";
            this.bTestProWRFLASH.Size = new System.Drawing.Size(169, 43);
            this.bTestProWRFLASH.TabIndex = 27;
            this.bTestProWRFLASH.Text = "TestProWRFLASH";
            this.bTestProWRFLASH.UseVisualStyleBackColor = true;
            this.bTestProWRFLASH.Click += new System.EventHandler(this.bTestProWRFLASH_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(326, 419);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 41);
            this.button1.TabIndex = 26;
            this.button1.Text = "TestCANansv";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listComPort
            // 
            this.listComPort.FormattingEnabled = true;
            this.listComPort.Location = new System.Drawing.Point(387, 50);
            this.listComPort.Name = "listComPort";
            this.listComPort.Size = new System.Drawing.Size(97, 95);
            this.listComPort.TabIndex = 25;
            this.listComPort.SelectedIndexChanged += new System.EventHandler(this.listComPort_SelectedIndexChanged_1);
            // 
            // bCOMselect
            // 
            this.bCOMselect.Location = new System.Drawing.Point(387, 152);
            this.bCOMselect.Name = "bCOMselect";
            this.bCOMselect.Size = new System.Drawing.Size(96, 27);
            this.bCOMselect.TabIndex = 24;
            this.bCOMselect.Text = "Подключить устройство";
            this.bCOMselect.UseVisualStyleBackColor = true;
            this.bCOMselect.Click += new System.EventHandler(this.bCOMselect_Click_1);
            // 
            // bfindPort
            // 
            this.bfindPort.Location = new System.Drawing.Point(387, 13);
            this.bfindPort.Name = "bfindPort";
            this.bfindPort.Size = new System.Drawing.Size(96, 30);
            this.bfindPort.TabIndex = 23;
            this.bfindPort.Text = "Найти Порт";
            this.bfindPort.UseVisualStyleBackColor = true;
            this.bfindPort.Click += new System.EventHandler(this.bfindPort_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gBchangePar);
            this.tabPage1.Controls.Add(this.dGtimers);
            this.tabPage1.Controls.Add(this.b_PARtableLoad);
            this.tabPage1.Controls.Add(this.b_PARtableSave);
            this.tabPage1.Controls.Add(this.b_newPARfilename);
            this.tabPage1.Controls.Add(this.lbFT1divFT0mult100);
            this.tabPage1.Controls.Add(this.lbFT0minusFT1);
            this.tabPage1.Controls.Add(this.tB_PARfileMan);
            this.tabPage1.Controls.Add(this.dGparam);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1019, 665);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Тест Режимов Работы";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gBchangePar
            // 
            this.gBchangePar.Controls.Add(this.labRejSimulator);
            this.gBchangePar.Controls.Add(this.cBrejSimulator);
            this.gBchangePar.Controls.Add(this.bCalibr);
            this.gBchangePar.Controls.Add(this.gBdateTime);
            this.gBchangePar.Controls.Add(this.bParamRead);
            this.gBchangePar.Controls.Add(this.labRej);
            this.gBchangePar.Controls.Add(this.bParamWrite);
            this.gBchangePar.Controls.Add(this.cBrej);
            this.gBchangePar.Location = new System.Drawing.Point(790, 6);
            this.gBchangePar.Name = "gBchangePar";
            this.gBchangePar.Size = new System.Drawing.Size(223, 544);
            this.gBchangePar.TabIndex = 8;
            this.gBchangePar.TabStop = false;
            this.gBchangePar.Text = "Действия";
            // 
            // cBrejSimulator
            // 
            this.cBrejSimulator.FormattingEnabled = true;
            this.cBrejSimulator.Items.AddRange(new object[] {
            "Симуляция отключена",
            "Симуляция работы таймеров"});
            this.cBrejSimulator.Location = new System.Drawing.Point(16, 41);
            this.cBrejSimulator.Name = "cBrejSimulator";
            this.cBrejSimulator.Size = new System.Drawing.Size(192, 21);
            this.cBrejSimulator.TabIndex = 9;
            this.cBrejSimulator.SelectedIndexChanged += new System.EventHandler(this.cBrejSimulator_SelectedIndexChanged);
            // 
            // bCalibr
            // 
            this.bCalibr.Location = new System.Drawing.Point(47, 437);
            this.bCalibr.Name = "bCalibr";
            this.bCalibr.Size = new System.Drawing.Size(110, 23);
            this.bCalibr.TabIndex = 8;
            this.bCalibr.Text = "Калибровка TFT";
            this.bCalibr.UseVisualStyleBackColor = true;
            this.bCalibr.Click += new System.EventHandler(this.bCalibr_Click);
            // 
            // gBdateTime
            // 
            this.gBdateTime.Controls.Add(this.lspeederTIME);
            this.gBdateTime.Controls.Add(this.cBtimeSpeed);
            this.gBdateTime.Controls.Add(this.bSetBOARDwinDT);
            this.gBdateTime.Controls.Add(this.textBox2);
            this.gBdateTime.Controls.Add(this.bSetBoardDT);
            this.gBdateTime.Controls.Add(this.LBDTread);
            this.gBdateTime.Controls.Add(this.lBDT);
            this.gBdateTime.Location = new System.Drawing.Point(16, 208);
            this.gBdateTime.Name = "gBdateTime";
            this.gBdateTime.Size = new System.Drawing.Size(192, 214);
            this.gBdateTime.TabIndex = 7;
            this.gBdateTime.TabStop = false;
            this.gBdateTime.Text = "ДатаВремя";
            // 
            // lspeederTIME
            // 
            this.lspeederTIME.AutoSize = true;
            this.lspeederTIME.Location = new System.Drawing.Point(28, 160);
            this.lspeederTIME.Name = "lspeederTIME";
            this.lspeederTIME.Size = new System.Drawing.Size(117, 13);
            this.lspeederTIME.TabIndex = 10;
            this.lspeederTIME.Text = "Ускорение Таймеров";
            // 
            // cBtimeSpeed
            // 
            this.cBtimeSpeed.FormattingEnabled = true;
            this.cBtimeSpeed.Items.AddRange(new object[] {
            "x1",
            "x2",
            "x10",
            "x60",
            "x600"});
            this.cBtimeSpeed.Location = new System.Drawing.Point(31, 176);
            this.cBtimeSpeed.Name = "cBtimeSpeed";
            this.cBtimeSpeed.Size = new System.Drawing.Size(110, 21);
            this.cBtimeSpeed.TabIndex = 9;
            this.cBtimeSpeed.SelectedIndexChanged += new System.EventHandler(this.cBtimeSpeed_SelectedIndexChanged);
            // 
            // bSetBOARDwinDT
            // 
            this.bSetBOARDwinDT.Location = new System.Drawing.Point(31, 128);
            this.bSetBOARDwinDT.Name = "bSetBOARDwinDT";
            this.bSetBOARDwinDT.Size = new System.Drawing.Size(110, 23);
            this.bSetBOARDwinDT.TabIndex = 4;
            this.bSetBOARDwinDT.Text = "GetBoardDT";
            this.bSetBOARDwinDT.UseVisualStyleBackColor = true;
            this.bSetBOARDwinDT.Click += new System.EventHandler(this.bSetBOARDwinDT_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(31, 102);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(110, 20);
            this.textBox2.TabIndex = 3;
            // 
            // bSetBoardDT
            // 
            this.bSetBoardDT.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bSetBoardDT.ForeColor = System.Drawing.Color.Red;
            this.bSetBoardDT.Location = new System.Drawing.Point(31, 73);
            this.bSetBoardDT.Name = "bSetBoardDT";
            this.bSetBoardDT.Size = new System.Drawing.Size(110, 23);
            this.bSetBoardDT.TabIndex = 2;
            this.bSetBoardDT.Text = "WinDTtoBoard";
            this.bSetBoardDT.UseVisualStyleBackColor = true;
            this.bSetBoardDT.Click += new System.EventHandler(this.bSetBoardDT_Click);
            // 
            // LBDTread
            // 
            this.LBDTread.AutoSize = true;
            this.LBDTread.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LBDTread.Location = new System.Drawing.Point(28, 46);
            this.LBDTread.Name = "LBDTread";
            this.LBDTread.Size = new System.Drawing.Size(51, 13);
            this.LBDTread.TabIndex = 1;
            this.LBDTread.Text = "noRead";
            // 
            // lBDT
            // 
            this.lBDT.AutoSize = true;
            this.lBDT.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lBDT.Location = new System.Drawing.Point(28, 16);
            this.lBDT.Name = "lBDT";
            this.lBDT.Size = new System.Drawing.Size(94, 13);
            this.lBDT.TabIndex = 0;
            this.lBDT.Text = "BoardDateTime";
            // 
            // bParamRead
            // 
            this.bParamRead.Location = new System.Drawing.Point(16, 79);
            this.bParamRead.Name = "bParamRead";
            this.bParamRead.Size = new System.Drawing.Size(192, 28);
            this.bParamRead.TabIndex = 2;
            this.bParamRead.Text = "Считать параметры";
            this.bParamRead.UseVisualStyleBackColor = true;
            this.bParamRead.Click += new System.EventHandler(this.bParamRead_Click);
            // 
            // labRej
            // 
            this.labRej.AutoSize = true;
            this.labRej.Location = new System.Drawing.Point(44, 152);
            this.labRej.Name = "labRej";
            this.labRej.Size = new System.Drawing.Size(131, 13);
            this.labRej.TabIndex = 5;
            this.labRej.Text = "Режим работы Фильтра";
            // 
            // bParamWrite
            // 
            this.bParamWrite.Location = new System.Drawing.Point(16, 113);
            this.bParamWrite.Name = "bParamWrite";
            this.bParamWrite.Size = new System.Drawing.Size(192, 27);
            this.bParamWrite.TabIndex = 3;
            this.bParamWrite.Text = "Записать параметры";
            this.bParamWrite.UseVisualStyleBackColor = true;
            this.bParamWrite.Click += new System.EventHandler(this.bParamWrite_Click);
            // 
            // cBrej
            // 
            this.cBrej.FormattingEnabled = true;
            this.cBrej.Items.AddRange(new object[] {
            "Ожидание Wait 1",
            "Промывка Wash 2",
            "Производство Fabric 3",
            "Предподготовка prepWash 4",
            "Новые Мембраны newWash 5",
            "Авария Damage 6",
            "Санитарная обработкаSanitar 7",
            "Первое включение FirstStart 8",
            "Быстраяпромывка speedWash 9",
            "Останов Stop 10"});
            this.cBrej.Location = new System.Drawing.Point(16, 168);
            this.cBrej.Name = "cBrej";
            this.cBrej.Size = new System.Drawing.Size(192, 21);
            this.cBrej.TabIndex = 4;
            this.cBrej.SelectedIndexChanged += new System.EventHandler(this.cBrej_SelectedIndexChanged);
            // 
            // dGtimers
            // 
            this.dGtimers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGtimers.Location = new System.Drawing.Point(8, 390);
            this.dGtimers.Name = "dGtimers";
            this.dGtimers.Size = new System.Drawing.Size(776, 126);
            this.dGtimers.TabIndex = 5;
            // 
            // b_PARtableLoad
            // 
            this.b_PARtableLoad.Location = new System.Drawing.Point(510, 637);
            this.b_PARtableLoad.Name = "b_PARtableLoad";
            this.b_PARtableLoad.Size = new System.Drawing.Size(127, 23);
            this.b_PARtableLoad.TabIndex = 1;
            this.b_PARtableLoad.Text = "Загрузить параметры";
            this.b_PARtableLoad.UseVisualStyleBackColor = true;
            this.b_PARtableLoad.Click += new System.EventHandler(this.b_PARtableLoad_Click);
            // 
            // b_PARtableSave
            // 
            this.b_PARtableSave.Location = new System.Drawing.Point(643, 637);
            this.b_PARtableSave.Name = "b_PARtableSave";
            this.b_PARtableSave.Size = new System.Drawing.Size(130, 23);
            this.b_PARtableSave.TabIndex = 2;
            this.b_PARtableSave.Text = "Сохранить параметры";
            this.b_PARtableSave.UseVisualStyleBackColor = true;
            // 
            // b_newPARfilename
            // 
            this.b_newPARfilename.Location = new System.Drawing.Point(783, 639);
            this.b_newPARfilename.Name = "b_newPARfilename";
            this.b_newPARfilename.Size = new System.Drawing.Size(91, 23);
            this.b_newPARfilename.TabIndex = 3;
            this.b_newPARfilename.Text = "Выбор файла";
            this.b_newPARfilename.UseVisualStyleBackColor = true;
            this.b_newPARfilename.Click += new System.EventHandler(this.b_newPARfilename_Click);
            // 
            // lbFT1divFT0mult100
            // 
            this.lbFT1divFT0mult100.AutoSize = true;
            this.lbFT1divFT0mult100.Location = new System.Drawing.Point(803, 579);
            this.lbFT1divFT0mult100.Name = "lbFT1divFT0mult100";
            this.lbFT1divFT0mult100.Size = new System.Drawing.Size(84, 13);
            this.lbFT1divFT0mult100.TabIndex = 4;
            this.lbFT1divFT0mult100.Text = "FT1/FT0*100 = ";
            // 
            // lbFT0minusFT1
            // 
            this.lbFT0minusFT1.AutoSize = true;
            this.lbFT0minusFT1.Location = new System.Drawing.Point(803, 553);
            this.lbFT0minusFT1.Name = "lbFT0minusFT1";
            this.lbFT0minusFT1.Size = new System.Drawing.Size(60, 13);
            this.lbFT0minusFT1.TabIndex = 3;
            this.lbFT0minusFT1.Text = "FT0-FT1 = ";
            // 
            // tB_PARfileMan
            // 
            this.tB_PARfileMan.Location = new System.Drawing.Point(-4, 637);
            this.tB_PARfileMan.Name = "tB_PARfileMan";
            this.tB_PARfileMan.Size = new System.Drawing.Size(497, 20);
            this.tB_PARfileMan.TabIndex = 0;
            // 
            // dGparam
            // 
            this.dGparam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGparam.Location = new System.Drawing.Point(8, 6);
            this.dGparam.Name = "dGparam";
            this.dGparam.Size = new System.Drawing.Size(776, 345);
            this.dGparam.TabIndex = 1;
            // 
            // WRpro
            // 
            this.WRpro.WorkerReportsProgress = true;
            this.WRpro.WorkerSupportsCancellation = true;
            this.WRpro.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WRpro_DoWork);
            this.WRpro.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.WRpro_ProgressChanged);
            this.WRpro.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WRpro_RunWorkerCompleted);
            // 
            // labRejSimulator
            // 
            this.labRejSimulator.AutoSize = true;
            this.labRejSimulator.Location = new System.Drawing.Point(44, 25);
            this.labRejSimulator.Name = "labRejSimulator";
            this.labRejSimulator.Size = new System.Drawing.Size(141, 13);
            this.labRejSimulator.TabIndex = 10;
            this.labRejSimulator.Text = "Выбор режима симуляции";
            // 
            // FormHUB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 691);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormHUB";
            this.Text = "RIVERDI GRAF FILES MAKER";
            this.Load += new System.EventHandler(this.FormHUB_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPagePack.ResumeLayout(false);
            this.tabPagePack.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gBSaveCode.ResumeLayout(false);
            this.gBSaveCode.PerformLayout();
            this.gBsaveOneFile.ResumeLayout(false);
            this.gBsaveOneFile.PerformLayout();
            this.tabPageBTAddr.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTimers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tP_TFT_WR_RD.ResumeLayout(false);
            this.tP_TFT_WR_RD.PerformLayout();
            this.gBtestWRgraf.ResumeLayout(false);
            this.gBtestWRgraf.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.gBchangePar.ResumeLayout(false);
            this.gBchangePar.PerformLayout();
            this.gBdateTime.ResumeLayout(false);
            this.gBdateTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGtimers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGparam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lHEX2;
        private System.Windows.Forms.TextBox TBSourseDir;
        private System.Windows.Forms.Button bSEL2;
        private System.Windows.Forms.Label lHEXres;
        private System.Windows.Forms.TextBox tBresultPath;
        private System.Windows.Forms.Button bRES;
        private System.Windows.Forms.TextBox tBCode;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPagePack;
        private System.Windows.Forms.TabPage tabPageBTAddr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBresultFilename;
        private System.Windows.Forms.Button bCreatFiles;
        private System.Windows.Forms.TextBox tBcodefilename;
        private System.Windows.Forms.Label labCODE;
        private System.Windows.Forms.Button bClear;
        private System.Windows.Forms.Button bSaveConf;
        private System.Windows.Forms.GroupBox gBSaveCode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tBendAdr;
        private System.Windows.Forms.TextBox tBstartAdr;
        private System.Windows.Forms.Label lBendFLASH;
        private System.Windows.Forms.Label lbStartFLASH;
        private System.Windows.Forms.Button bCreatCode;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel SLprocess;
        private System.Windows.Forms.TabPage tP_TFT_WR_RD;
        private System.Windows.Forms.ListBox listComPort;
        private System.Windows.Forms.Button bCOMselect;
        private System.Windows.Forms.Button bfindPort;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bTestProWRFLASH;
        private System.ComponentModel.BackgroundWorker WRpro;
        private System.Windows.Forms.Button bwr_tft_flash;
        private System.Windows.Forms.Button bTestFileToFLASH;
        private System.Windows.Forms.ToolStripProgressBar PBFileToMem;
        private System.Windows.Forms.Button bStopTest;
        private System.Windows.Forms.Button bGRAFdir;
        private System.Windows.Forms.TextBox tBgrafDIR;
        private System.Windows.Forms.Label lSOURCE;
        private System.Windows.Forms.ToolStripStatusLabel SLfileName;
        private System.Windows.Forms.ToolStripProgressBar PBwrFilePro;
        private System.Windows.Forms.CheckBox cBfontPict;
        private System.Windows.Forms.Label lCreater;
        private System.Windows.Forms.Button bProcessCreater;
        private System.Windows.Forms.CheckBox cBmenu;
        private System.Windows.Forms.Button bSaveOneFile;
        private System.Windows.Forms.GroupBox gBsaveOneFile;
        private System.Windows.Forms.TextBox tBNameOneFile;
        private System.Windows.Forms.Label lbNameOneFile;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridTimers;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox LBoxInterface;
        private System.Windows.Forms.Button bTestMainBoardRS485;
        private System.Windows.Forms.Label DevLabel;
        private System.Windows.Forms.CheckBox cBoxEnMess;
        private System.Windows.Forms.Button buttestSw;
        private System.Windows.Forms.CheckBox cBLoadPict;
        private System.Windows.Forms.GroupBox gBtestWRgraf;
        private System.Windows.Forms.Label lbNum_BAD;
        private System.Windows.Forms.Label lbNumGOOD;
        private System.Windows.Forms.Label lbCountBad;
        private System.Windows.Forms.Label lbCountGood;
        private System.Windows.Forms.Button bStopTestWrPict;
        private System.Windows.Forms.Button bStartTestWrPict;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dGparam;
        private System.Windows.Forms.Button b_newPARfilename;
        private System.Windows.Forms.Button b_PARtableSave;
        private System.Windows.Forms.Button b_PARtableLoad;
        private System.Windows.Forms.TextBox tB_PARfileMan;
        private System.Windows.Forms.Label lbFT1divFT0mult100;
        private System.Windows.Forms.Label lbFT0minusFT1;
        private System.Windows.Forms.DataGridView dGtimers;
        private System.Windows.Forms.GroupBox gBchangePar;
        private System.Windows.Forms.Button bCalibr;
        private System.Windows.Forms.GroupBox gBdateTime;
        private System.Windows.Forms.Button bSetBOARDwinDT;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button bSetBoardDT;
        private System.Windows.Forms.Label LBDTread;
        private System.Windows.Forms.Label lBDT;
        private System.Windows.Forms.Button bParamRead;
        private System.Windows.Forms.Label labRej;
        private System.Windows.Forms.ComboBox cBtimeSpeed;
        private System.Windows.Forms.Button bParamWrite;
        private System.Windows.Forms.ComboBox cBrej;
        private System.Windows.Forms.Label lspeederTIME;
        private System.Windows.Forms.ComboBox cBrejSimulator;
        private System.Windows.Forms.Label labRejSimulator;
    }
}

