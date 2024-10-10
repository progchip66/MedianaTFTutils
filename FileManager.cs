using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FileCreater
{


    class TResurser
    {
        public int StartRAMadr;
        public int StartFLASHadr;
        public int EndRAMadr;
        public int EndFLASHadr;


        public int UP1down0;


        public int AllignFilesLen = 0;
        public int WriteAddr = 0;

        public string Sextension;
        public int countfiles;
        public string[] NameFiles;
        public string[] FULLnameFiles;
        public int[] LengthFiles;
        public int[] AlignLengthFiles;
        public int[] FLASHAdresFiles;
        public int[] RAMAdresFiles;
        public int[] RAMalignLenFiles;
        public int RAMendAdr;
        public int MaxLengthFile;



        public void initAligner(int StartRAMadr, int StartFLASHadr, int EndRAMadr, int EndFLASHadr, string Sextension)
        {
            this.StartRAMadr = StartRAMadr;
            this.StartFLASHadr = StartFLASHadr;
            this.EndRAMadr = EndRAMadr;
            this.EndFLASHadr = EndFLASHadr;
            this.Sextension = Sextension;
            if (EndRAMadr > StartRAMadr)
                UP1down0 = 0;
            else
                UP1down0 = 1;
            WriteAddr = StartFLASHadr;
        }

        public void InitParams(string SourseDir)
        {

            FULLnameFiles = Directory.GetFiles(SourseDir, Sextension);
            Array.Sort(FULLnameFiles);
            countfiles = FULLnameFiles.Length;
            this.NameFiles = new string[countfiles];
            this.LengthFiles = new int[countfiles];
            this.FLASHAdresFiles = new int[countfiles];
            this.RAMAdresFiles = new int[countfiles];
            this.RAMalignLenFiles = new int[countfiles];

            countfiles = 0;
            foreach (string fileName in FULLnameFiles)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    LengthFiles[countfiles] = Convert.ToInt32(fileInfo.Length);
                    string sExt = fileInfo.Extension;
                    string S = fileInfo.Name;
                    int pos = S.IndexOf('.');
                    NameFiles[countfiles] = S.Substring(0, pos);
                    countfiles++;
                }
            }

        }


        public int GetAlignVol(int FileLength, int align, int UP1down0)
        {
            int ret;
            int tmpmul = FileLength / align;
            ret = align * tmpmul;
            if (UP1down0 != 0)
            {
                if (ret < FileLength)
                    ret += align;
            }

            return ret;
        }

        public int NextAlignAdr(int OldAdr,int FileLength, int align, int UP1down0)
        {
            FileLength = GetAlignVol(FileLength, align, 1);
            if (UP1down0>0)
                OldAdr += FileLength;
            else
                OldAdr -= FileLength;
            return GetAlignVol(FileLength, align, UP1down0);
        }



        void ConvertFileToHEX (string fileName)
        {
            // Замените "FILINAME" на фактическое имя вашего файла
            try
            {
                // Чтение файла в виде байтового массива
                byte[] fileBytes = File.ReadAllBytes(fileName);

                // Создание потока для записи в файл с расширением .bin
                using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(fileName, "bin")))
                {
                    int count = 0;

                    // Преобразование каждого байта в шестнадцатеричное представление
                    foreach (byte b in fileBytes)
                    {
                        if (count == 16)
                        {
                            // Новая строка после каждых 16 значений
                            writer.WriteLine();
                            count = 0;
                        }

                        // Запись шестнадцатеричного представления с разделением запятой
                        writer.Write($"{b:X2}, ");
                        count++;
                    }
                }

    //            Console.WriteLine("Преобразование завершено. Результат записан в файл с расширением .bin.");
            }
            catch (Exception ex)
            {
      //          Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }





        public byte[] GetDataFromFile(string FinputName, int align)
        {
            using (FileStream fstream = new FileStream(FinputName, FileMode.Open))//read file
            {
                // выделяем массив для считывания данных из файла
                int FileLen = Convert.ToInt32(fstream.Length);

                int alignFileLen = GetAlignVol(FileLen, align, 1);//получаем выравненную длину файла, в данном случае с увеличением

                if (WriteAddr + alignFileLen > EndFLASHadr)
                {
                    throw new Exception("Выход за границы диапазона памяти MaxFLASHAdr=" + Convert.ToString(EndFLASHadr) + ", CalculateAdr=" + Convert.ToString(WriteAddr + alignFileLen));
                }
                else
                    WriteAddr += alignFileLen;

                byte[] buffer = new byte[alignFileLen];
                fstream.Read(buffer, 0, FileLen);//read data 
                for (int i = FileLen; i < alignFileLen; i++)
                    buffer[i] = 0xff;
                return buffer;
            }
        }

        public int GRAMaligner(int StartAdr, int align, int UP1down0)
        {
            this.RAMalignLenFiles = new int[countfiles];
            this.RAMAdresFiles = new int[countfiles + 1];
            RAMAdresFiles[0] = StartAdr;
            for (int i = 0; i < countfiles; i++)
            {
                if (UP1down0 > 0)
                {
                    RAMalignLenFiles[i] = GetAlignVol(LengthFiles[i], align, UP1down0);

                    if (i > 0)
                        RAMAdresFiles[i] = RAMAdresFiles[i - 1] + RAMalignLenFiles[i - 1];
                    if (i == (countfiles - 1))
                    {
                        RAMendAdr = RAMAdresFiles[i] + RAMalignLenFiles[i];
                    }
                }
                else
                {
                    int tmpa = StartAdr;
                    StartAdr -= LengthFiles[i];
                    StartAdr /= align;
                    StartAdr *= align;
                    RAMAdresFiles[i] = StartAdr;
                    RAMalignLenFiles[i] = tmpa - StartAdr;
                    RAMendAdr = RAMAdresFiles[i];
                }
            }
            return RAMendAdr;
        }


        public int GetMaxLengthFile()
        {
            MaxLengthFile = 0;
            for (int i = 0; i < countfiles; i++)
                if (LengthFiles[i] > MaxLengthFile)
                    MaxLengthFile = LengthFiles[i];
            return MaxLengthFile;
        }


        public void AppendResursFile(string Outfilename, int align)
        {

            using (FileStream fstream = new FileStream(Outfilename, FileMode.Append))
            {//open file only for added new data
                int tmpi;
                AlignLengthFiles = new int[NameFiles.Length];
                FLASHAdresFiles = new int[NameFiles.Length];
                for (tmpi = 0; tmpi < NameFiles.Length; tmpi++)
                {
                    byte[] fileData = GetDataFromFile(FULLnameFiles[tmpi], align);
                    AlignLengthFiles[tmpi] = fileData.Length;
                    FLASHAdresFiles[tmpi] = Convert.ToInt32(fstream.Position) + StartFLASHadr;

                    fstream.Write(fileData, 0, fileData.Length);
                }
            }

        }

        public string DefValStruct(string Name)
        {
            Name = "par" + Name;
            string tmpS = "S" + Name + "  " + Name + " ={";
            int Count = Convert.ToInt32(LengthFiles.Length);
            int j = 0;
            for (int i = 0; i < Count; i++)
            {
                tmpS += Convert.ToString(LengthFiles[i]) + ',';
                tmpS += Convert.ToString(FLASHAdresFiles[i]) + ',';
                tmpS += Convert.ToString(RAMAdresFiles[i]) + ',';
                tmpS += Convert.ToString(RAMalignLenFiles[i]);
                if (i == Count - 1)
                {
                    tmpS += "};";
                }
                else
                {
                    tmpS += ",";
                    if (j++ >= 3)
                    {
                        j = 0;
                        tmpS += "\r\n";
                    }
                }
            }

            return tmpS += "\r\n";
        }




        public string[] CreatDefStruct(string Name)
        {
            Name = "par" + Name;
            List<string> codeStruct = new List<string>();
            string tmpStartStr = "\tint\t";
            codeStruct.Add("\r\n// ####################### " + Name + " #######################\r\n\r\n");
            codeStruct.Add("#define " + Name + "COUNT " + Convert.ToString(countfiles) + "\r\n");

            int i = 0;

            codeStruct.Add("\r\n\r\ntypedef enum\r\n{");
            foreach (string s in NameFiles)
            {
                codeStruct.Add("\tind" + Name + "_" + s + "\t\t=" + Convert.ToString(i++) + ",\r\n");
            }
            codeStruct.Add("} e" + Name + ";\r\n\r\n");

            codeStruct.Add("\r\ntypedef struct\r\n" + "{\r\n");

            foreach (string s in NameFiles)
            {
                codeStruct.Add(tmpStartStr + "Len_" + s + ";\r\n");
                codeStruct.Add(tmpStartStr + "FLASHadr_" + s + ";\r\n");
                codeStruct.Add(tmpStartStr + "RAMadr_" + s + ";\r\n");
                codeStruct.Add(tmpStartStr + "RAMalignLen_" + s + ";\r\n");
                codeStruct.Add("\r\n");

            }
            codeStruct.Add("} S" + Name + ";\r\n");
            string[] ret = new string[codeStruct.Count];
            codeStruct.CopyTo(ret);
            return ret;
        }


        public string[] CreatMenuButtonStruct(string Name)
        {
            Name = "par" + Name;
            List<string> codeStruct = new List<string>();
            string tmpStartStr = "\tint\t";
            codeStruct.Add("\r\n// ####################### " + Name + " #######################\r\n\r\n");
            codeStruct.Add("#define " + Name + "COUNT " + Convert.ToString(countfiles) + "\r\n");

            int i = 0;

            codeStruct.Add("\r\n\r\ntypedef enum\r\n{");
            foreach (string s in NameFiles)
            {
                codeStruct.Add("\tind" + Name + "_" + s + "\t\t=" + Convert.ToString(i++) + ",\r\n");
            }
            codeStruct.Add("} e" + Name + ";\r\n\r\n");

            codeStruct.Add("\r\ntypedef struct\r\n" + "{\r\n");

            foreach (string s in NameFiles)
            {
                codeStruct.Add(tmpStartStr + "Len_" + s + ";\r\n");
                codeStruct.Add(tmpStartStr + "FLASHadr_" + s + ";\r\n");
                codeStruct.Add(tmpStartStr + "RAMadr_" + s + ";\r\n");
                codeStruct.Add(tmpStartStr + "RAMalignLen_" + s + ";\r\n");
                codeStruct.Add("\r\n");

            }
            codeStruct.Add("} S" + Name + ";\r\n");
            string[] ret = new string[codeStruct.Count];
            codeStruct.CopyTo(ret);
            return ret;
        }



    }






    class TFileManager
    {
        public const int _RAMsize = 0x100000; //1Mbyte
        public const int _FLASHsize = 0x800000; //8Mbyte
        public const int _StartFLASHadr = 0x100000; //1Mbyte
        public const int _StartFLASHadrBLOB = 0; //1Mbyte
        public const int _FLASHalign = 0x1000; //4096 byte
        public const int _RAMalign = 64; //64 byte

        public const int menuGRAMadr = 0x100000; //1Mbyte GetAlignVol

        public int RAMsize { get { return _RAMsize; } }
        public int FLASHsize { get { return _FLASHsize; } }
        public int StartFLASHadr { get { return _StartFLASHadr; } }
        public int StartFLASHadrBLOB { get { return _StartFLASHadrBLOB; } }
        public int FLASHalign { get { return _FLASHalign; } }
        public int RAMalign { get { return _RAMalign; } }



        public TResurser RESjpg;
        public TResurser RESfont;
        public TResurser RESmenu;

        public const string extPict = "*.jpg";
        public const string extFont = "*.font";
        public const string extMenu = "*.menu";




        public string SourseDir;
        public string resultPath;
        public string codefilename;
        public string resultFilename;


        void initFileManager()
        {//start predef initialization

            RESjpg.initAligner(RAMsize, StartFLASHadr, 0, FLASHsize, extPict);
            RESfont.initAligner(0, StartFLASHadr, RAMsize, FLASHsize, extFont);
            RESmenu.initAligner(0, StartFLASHadr, RAMsize, FLASHsize, extMenu);
        }

        public TFileManager()
        {
            RESjpg = new TResurser();
            RESfont = new TResurser();
            RESmenu = new TResurser();
            initFileManager();
        }


        public int ConvHextoInt(string shex)
        {
            int res = 0;
            string s1hex = shex.Replace("0x", "");
            s1hex = s1hex.Replace("0X", "");
            if (string.Compare(shex, s1hex) == 0)
                res = Convert.ToInt32(shex);
            else
                res = Int32.Parse(s1hex, System.Globalization.NumberStyles.HexNumber);
            return res;
        }

        //        Aligner = new TAligner(AllignSize, StartFLASHadr, EndFLASHadr);




        public Boolean GetParameters(string _SourseDir, string ResultDir, string resultFilename)
        {
            initFileManager();

            if (!Directory.Exists(_SourseDir))
            {
                return false;
            }    
               
            if (!Directory.Exists(ResultDir))
                Directory.CreateDirectory(ResultDir);

            RESjpg.InitParams(_SourseDir);
            RESfont.InitParams(_SourseDir);
            RESmenu.InitParams(_SourseDir);

            FileInfo fileInf = new FileInfo(resultPath + '\\' + resultFilename);//if file exist then delete file
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }

            RESjpg.AppendResursFile(resultPath + '\\' + resultFilename, FLASHalign);
            RESfont.AppendResursFile(resultPath + '\\' + resultFilename, FLASHalign);
            RESmenu.AppendResursFile(resultPath + '\\' + resultFilename, FLASHalign);

            int tmpadr = RESfont.GRAMaligner(0, RAMalign, 1);
            for (int i = 0; i < RESmenu.countfiles; i++)
            {
                RESmenu.RAMAdresFiles[i] = tmpadr;
                RESmenu.RAMalignLenFiles[i] = RESmenu.GetAlignVol(RESmenu.LengthFiles[i], RAMalign, 1);
            }
            tmpadr += RESmenu.GetMaxLengthFile();
            int minAdrJPEG = RESjpg.GRAMaligner(RAMsize - RAMalign, RAMalign, 0);
            return true;
        }
    }



    class SGRAF_FILES : TResurser

    {
        private  int  const_sizeTFTFLASHalign = 4096;

        public int NumMenuOneMenuFile=0;
        public int sizeTFTFLASHalign { get { return const_sizeTFTFLASHalign; } }

        public string[] MENUfileNames;
        public string[] PICTfileNames;
        public string[] FONTfileNames;


        public  bool CheckFileExist(string filename, string startDIR, string endSUBDIR)
        {

            // Проверка существует ли файл
            if (!File.Exists(filename))
            {
                MessageBox.Show("Файл не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Проверка, находится ли файл в поддиректории "MENU"
            string menuDirectory = Path.Combine(startDIR, endSUBDIR)+"\\";
            if (!filename.StartsWith(menuDirectory, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Файл не находится в поддиректории "+endSUBDIR+"!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Если все проверки пройдены, возвращаем true
            return true;
        }

        public  int GetFileOrder(string filePath)
        {
            // Получаем директорию из полного имени файла
            string directoryPath = Path.GetDirectoryName(filePath);

            // Получаем имя файла
            string fileName = Path.GetFileName(filePath);

            // Получаем список всех файлов в директории и сортируем их в алфавитном порядке
            var files = Directory.GetFiles(directoryPath).Select(Path.GetFileName).OrderBy(f => f).ToList();

            // Находим порядковый номер файла
            int fileIndex = files.IndexOf(fileName);

            return fileIndex;  // Возвращаем порядковый номер файла
        }



        public int GetAdrInsertFileToFlash(string CATGrafDir, string FullFileName, int StartadrFLASHmenu)
        {

            //проверяет существует ли файл FileName в директории меню,
            //если существует проверяет не превышает ли новая версия допустимую длины выделенную под него в TFT FLASH
            //если место для записи файла во FLASH позволяет, вставляет его на месно прежнего "FileName"
            //если не существует, проверяет допустима ли имя для добавления файла в конец записи FLASH
            //если допустимо, то записывает его в конец FLASH

            FileInfo fileInfo = new FileInfo(FullFileName);
            if (!fileInfo.Exists)
            {
                throw new Exception("Файл для записи не обнаружен");
            }

            string FileName = FullFileName;
            int sStart = FileName.LastIndexOf("\\");
            if (sStart >= 0)
                sStart++;
            else
                sStart = 0;
            int sEnd = FileName.LastIndexOf(".");
            if (sEnd < 0)
                sEnd = FileName.Length;
            FileName = FileName.Substring(sStart, sEnd - sStart);


            if (Directory.Exists(CATGrafDir))
            {
                string[] dirs = Directory.GetDirectories(CATGrafDir);
                foreach (string s in dirs)
                {
                    if (s.EndsWith("MENU"))
                    {
                        MENUfileNames = Directory.GetFiles(s);//получили список файлов директории MENU
                        break;
                    }
                }

                Array.Sort(MENUfileNames, StringComparer.CurrentCultureIgnoreCase);

            }
            else
            {
                throw new Exception("Директория MENU не существует");
            }

 //           string[] MENUfilenames = Init_WRtoFLASHfiles(DIR_FlowPro, "MENU");//получаем список файлов директории c изображениями меню


            int AlignFileLen = 0;
            int curTFTFLASHadr = GetAlignVol(StartadrFLASHmenu, sizeTFTFLASHalign, 1);//на всякий пожарный проводим выравнивание стартового адреса
            for (int i = 0; i < MENUfileNames.Length; i++)
            {
                string s = MENUfileNames[i];

                sStart = s.LastIndexOf("\\");
                if (sStart >= 0)
                    sStart++;
                else
                    sStart = 0;
                sEnd = s.LastIndexOf(".");
                if (sEnd < 0)
                    sEnd = s.Length;
                string curFileName = s.Substring(sStart, sEnd - sStart);
                FileInfo curfileInfo = new FileInfo(MENUfileNames[i]);
                int curAlignFileLen = GetAlignVol(Convert.ToInt32(curfileInfo.Length), sizeTFTFLASHalign, 1);//определяем длину текущего ранее записанного файла
                                                                                                             //файла
                if (FileName == curFileName)
                {
                    AlignFileLen = GetAlignVol(Convert.ToInt32(fileInfo.Length), sizeTFTFLASHalign, 1);//определяем длину файла для замены
                    if (AlignFileLen > curAlignFileLen)
                    {
                        throw new Exception("Файл не может быть вставлен во FLASH поскольку его размер больше файла, который находится на этом месте");
                    }
                    NumMenuOneMenuFile = i;
                    return (curTFTFLASHadr);
                }
                else
                {//определеяем адрес записи следующего файла в TFT FLASH
                    curTFTFLASHadr += curAlignFileLen;
                }
                
            }
            NumMenuOneMenuFile = MENUfileNames.Length;
            return (curTFTFLASHadr);
        }




        public void Init_ArrFLASHaddr_MENU(string CATGrafDir, int StartFLASHadr, int sizeFLASHalign = 4096)
        {
            string GrafDir = CATGrafDir;

            if (!Directory.Exists(GrafDir + "\\DOC"))
            {
                Directory.CreateDirectory(GrafDir + "\\DOC");
            }

            if (Directory.Exists(GrafDir))
            {
                string[] dirs = Directory.GetDirectories(GrafDir);
                foreach (string s in dirs)
                {
                    if (s.EndsWith("MENU"))
                    {
                        MENUfileNames = Directory.GetFiles(s);
                    }
                }

                Array.Sort(MENUfileNames, StringComparer.CurrentCultureIgnoreCase);

                List<string> adrMenuFLASH = new List<string>();

                int FLASHadr = GetAlignVol(StartFLASHadr, sizeFLASHalign, 1);//на всякий пожарный проводим выравнивание стартового адреса
                if (StartFLASHadr != FLASHadr)
                {
                    throw new Exception("Стартовый адрес не выравнен на значение   " + Convert.ToString(sizeFLASHalign));
                }
                int Adr = StartFLASHadr;

                int i10tmp = 0;
                string s10tmp = "unsigned int FLASHadrMENU[] = { ";
                int ipos = 0;
                foreach (string s in MENUfileNames)
                {
                    FileInfo fileInfo = new FileInfo(s);
                    if ((++ipos)== MENUfileNames.Length)
                    {
                        adrMenuFLASH.Add(s10tmp + Convert.ToString(Adr) + "};\r\n\r\n");
                        break;
                    }
                    else
                    { 
                        if (++i10tmp >= 10)
                        {
                            adrMenuFLASH.Add(s10tmp + Convert.ToString(Adr)+",");

                           s10tmp = "";
                            i10tmp = 0;

                        }
                        else
                            s10tmp = s10tmp + Convert.ToString(Adr) + ", ";
                    }
                        
                    int len = Convert.ToInt32(fileInfo.Length);
                    Adr = Adr + GetAlignVol(len, sizeFLASHalign, 1);
                    

                }
               




                using (StreamWriter outputFile = new StreamWriter(Path.Combine(GrafDir + "\\DOC", "defAdrMENU.txt")))
                {//запись адресов в файл

                    string[] lines = new string[adrMenuFLASH.Count];
                    adrMenuFLASH.CopyTo(lines);
                    foreach (string line in lines)
                        outputFile.WriteLine(line);

                }

            }
        }


        public void Init_FONTE_PICT(string CATGrafDir, int StartFLASHadr, int StartGRAMadr, int sizeFLASHalign = 4096, int sizeGRAMHalign = 64)
        {
            string GrafDir = CATGrafDir;

            if (!Directory.Exists(GrafDir + "\\DOC"))
            {
                Directory.CreateDirectory(GrafDir + "\\DOC");
            }

            if (Directory.Exists(GrafDir))
            {
                string[] dirs = Directory.GetDirectories(GrafDir);
                foreach (string s in dirs)
                {
                    if (s.EndsWith("FONT"))
                    {
                        FONTfileNames = Directory.GetFiles(s);
                    }
                    if (s.EndsWith("PICT"))
                    {
                        PICTfileNames = Directory.GetFiles(s);
                    }
                }

                Array.Sort(FONTfileNames, StringComparer.CurrentCultureIgnoreCase);

                List<string> structFONTELASH = new List<string>();


                int FLASHadr = GetAlignVol(StartFLASHadr, sizeFLASHalign, 0);//на всякий пожарный проводим выравнивание стартового адреса
                if (StartFLASHadr != FLASHadr)
                {
                    throw new Exception("Стартовый адрес не выравнен на значение   " + Convert.ToString(sizeFLASHalign));
                }


                structFONTELASH.Add(" #define COUNT_FONT   " + Convert.ToString(FONTfileNames.Length) + "\r\n");
                structFONTELASH.Add(" typedef enum\r\n{");

                foreach (string s in FONTfileNames)
                {
                    FileInfo fileInfo = new FileInfo(s);
                    int sStart = s.Length;
                    sStart = s.LastIndexOf("\\") + 1;
                    int sEnd = s.LastIndexOf(".");
                    structFONTELASH.Add("  cs" + s.Substring(sStart, sEnd - sStart) + ",");
                }
                structFONTELASH.Add(" } EnumFont;\r\n\r\n");

                

                string tmpParFonts = " SparFONT  parFonts[COUNT_FONT]={";

                int FLASHAdr = StartFLASHadr;
                int GRAMAdr = StartGRAMadr;
                int ALignLength = 0;
                int tmpDot = 0;
                foreach (string s in FONTfileNames)
                {
                    FileInfo fileInfo = new FileInfo(s);
                    if (tmpDot == 0)
                        tmpDot++;
                    else
                        tmpParFonts = tmpParFonts + ", ";

                    ALignLength= GetAlignVol(Convert.ToInt32(fileInfo.Length), sizeFLASHalign, 1);
                    FLASHAdr = FLASHAdr - ALignLength;
                    GRAMAdr = GRAMAdr - ALignLength;

                    tmpParFonts = tmpParFonts + "{" + ALignLength + "," + FLASHAdr + "," + Convert.ToInt32(GRAMAdr) + "}";

                }


                structFONTELASH.Add(tmpParFonts + "};\r\n\r\n");


                //****************************************************************************************************************



                Array.Sort(PICTfileNames, StringComparer.CurrentCultureIgnoreCase);

                List<string> structPICTPAR = new List<string>();
                List<string> structPICTLASH = new List<string>();
                

                structPICTLASH.Add(" #define COUNT_PICT   " + Convert.ToString(PICTfileNames.Length) + "\r\n");
                structPICTLASH.Add(" typedef enum\r\n{");

                tmpDot = 0;
                string tmpParPICT = " SbitmapRGB565  parPICTS[COUNT_PICT]={";
                int pictCount = PICTfileNames.Length;
                foreach (string s in PICTfileNames)
                {
                    FileInfo fileInfo = new FileInfo(s);

                    int sStart = s.LastIndexOf("\\") + 1;
                    int sEnd = s.LastIndexOf(".");
                    string tmpS = s.Substring(sStart, sEnd - sStart);
                    
                    int posH = tmpS.LastIndexOf("H") + 1;
                    string H = tmpS.Substring(posH, tmpS.Length - posH);
                    int posL = tmpS.LastIndexOf("L") + 1;
                    string L = tmpS.Substring(posL, posH-posL-1);

    
                    structPICTLASH.Add("  cs" + tmpS + ",");

                    //****************************************
                    tmpDot++;

                    int LENbitmapRGB565 = Convert.ToInt32(H) * Convert.ToInt32(L) * 2;


                    ALignLength = GetAlignVol(Convert.ToInt32(fileInfo.Length), sizeFLASHalign, 1);
                    FLASHAdr = FLASHAdr - ALignLength;//отступ в TFT FLASH
                    int ALignLengthRGB565 = GetAlignVol(LENbitmapRGB565, sizeGRAMHalign, 1);
                    GRAMAdr = GRAMAdr - ALignLengthRGB565;//отступ для скачиваемых в GRAM битмапов


                    if (pictCount == 1 )
                    {
                        structPICTPAR.Add(tmpParPICT + ALignLength + "," + FLASHAdr + "," + GRAMAdr + "," +
                                                        L + "," + H + ",0,0,0  };\r\n\r\n");
                    }
                    else
                    {
                        if (pictCount != tmpDot)
                            structPICTPAR.Add(tmpParPICT + "{" + ALignLength + "," + FLASHAdr + "," + GRAMAdr + "," +
                                L + "," + H + ",0,0,0 },");
                        else
                            structPICTPAR.Add(tmpParPICT + "{" + ALignLength + "," + FLASHAdr + "," + GRAMAdr + "," +
                                L + "," + H + ",0,0,0 } };\r\n\r\n");
                    }

                    tmpParPICT = "    ";
     
                }

                structPICTLASH.Add("}ePICT;\r\n");



                using (StreamWriter outputFile = new StreamWriter(Path.Combine(GrafDir + "\\DOC", "defAdrFLASH.txt")))
                {//запись адресов в файл

                    string[] lines = new string[structFONTELASH.Count+ structPICTPAR.Count+ structPICTLASH.Count];
                    structPICTLASH.CopyTo(lines);
                    structPICTPAR.CopyTo(lines, structPICTLASH.Count);
                    
                    structFONTELASH.CopyTo(lines, structPICTPAR.Count+ structPICTLASH.Count);



                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }


            }
        }







        public void Init_MENU_enum(string CATGrafDir, int StartFLASHadr, int sizeFLASHalign=4096)
        {
            string GrafDir = CATGrafDir;

            if (!Directory.Exists(GrafDir + "\\DOC"))
            {
                Directory.CreateDirectory(GrafDir + "\\DOC");
            }

            if (Directory.Exists(GrafDir))
            {
                string[] dirs = Directory.GetDirectories(GrafDir);
                foreach (string s in dirs)
                {
                    if (s.EndsWith("MENU"))
                    {
                        MENUfileNames = Directory.GetFiles(s);
                    }
                    else
                    {
                        if (s.EndsWith("PICT"))
                        {
                            PICTfileNames = Directory.GetFiles(s);
                        }
                    }
                }

                Array.Sort(MENUfileNames, StringComparer.CurrentCultureIgnoreCase);
                Array.Sort(PICTfileNames, StringComparer.CurrentCultureIgnoreCase);


                List<string> adrMenuFLASH = new List<string>();
                List<string> parMenu = new List<string>();


                parMenu.Add("\r\n #define MENUcount   " + Convert.ToString(MENUfileNames.Length) + ";\r\n\r\n");

                parMenu.Add("\r\n SparamMENU[" + Convert.ToString(MENUfileNames.Length) + "] = paramMENU{");



                int FLASHadr = GetAlignVol(StartFLASHadr, sizeFLASHalign, 1);//на всякий пожарный проводим выравнивание стартового адреса
                if (StartFLASHadr != FLASHadr)
                {
                    throw new Exception("Стартовый адрес не выравнен на значение   " + Convert.ToString(sizeFLASHalign));
                }
                int StartAdr = StartFLASHadr;

                int i10tmp = 0;
                string s10tmp = "";
                foreach (string s in MENUfileNames)
                {
                    FileInfo fileInfo = new FileInfo(s);
                    int len = Convert.ToInt32(fileInfo.Length);
                    len = GetAlignVol(len, sizeFLASHalign, 1);
                    s10tmp = s10tmp + s;
                    if (++i10tmp >= 10)
                    {
                        adrMenuFLASH.Add(s10tmp + ";");
                        s10tmp = "";
                        i10tmp = 0;
                    }
                }
                adrMenuFLASH.Add(s10tmp + "}");

                adrMenuFLASH.Add("}EadrMenuFLASH;\r\n" + "\r\n" + "\r\n");
 //               lenMenuFLASH.Add("}ElenMenuFLASH;\r\n" + "\r\n" + "\r\n");

                //***********************************************************************************



                using (StreamWriter outputFile = new StreamWriter(Path.Combine(GrafDir + "\\DOC", "defAdrLen.txt")))
                {//запись адресов в файл

                    string[] lines = new string[adrMenuFLASH.Count];
                    adrMenuFLASH.CopyTo(lines);
                    foreach (string line in lines)
                        outputFile.WriteLine(line);

                }

            }
            

        }

        public void codeMENUcreater_(string CATGrafDir, int sizeFLASHalign = 4096)
        {
            string GrafDir = CATGrafDir;
            string[] mCS = null;
            List<string> codeStruct = new List<string>();

            if (Directory.Exists(GrafDir))
            {
                string[] dirs = Directory.GetDirectories(GrafDir);
                foreach (string s in dirs)
                {
                    if (s.EndsWith("MENU"))
                    {
                        mCS = Directory.GetFiles(s);

                    }
                }
            }


                for (int i = 0; i < mCS.Length; i++)
                {

                    FileInfo fileInfo = new FileInfo(mCS[i]);

                    int len = Convert.ToInt32(fileInfo.Length);

                    int ind_start = mCS[i].LastIndexOf("\\") + 1;
                    mCS[i] = mCS[i].Substring(ind_start);
                    int ind_end = mCS[i].LastIndexOf(".");
                    mCS[i] = mCS[i].Substring(0, ind_end);
                    mCS[i] = "cs" + mCS[i];
                }


                codeStruct.Add("\r\n typedef enum\r\n" + "{");
                foreach (string ss in mCS)
                {
                    codeStruct.Add(ss + ",");

                }
                codeStruct.Add("csMENUback = 0xfe,");
                codeStruct.Add("csMENUforward = 0xfd,");
                codeStruct.Add("csMENUnone = 0xff,");
                codeStruct.Add("}eMENU;\r\n" + "\r\n" + "\r\n" + "\r\n");



            //*********************************************************************************** codeStruct.Add("");
            codeStruct.Add("void SwitchMenu(eMENU NumMenu)");
            codeStruct.Add("{");
            codeStruct.Add("    switch (NumMenu)");
            codeStruct.Add("    {\r\n");
            foreach (string ss in mCS)
            {
                codeStruct.Add("case " + ss + ":\r\n");

                codeStruct.Add("break;");
            };
            codeStruct.Add("default:");
            codeStruct.Add("    return;");
            codeStruct.Add("}\r\n\r\n\r\n\r\n");

            //*********************************************************************************** codeStruct.Add("");
            codeStruct.Add("void InitMenuButtons(eMENU NumMenu)");
                codeStruct.Add("{");
                codeStruct.Add("    ResetMenuButtons();");
                codeStruct.Add("    switch (NumMenu)");
                codeStruct.Add("    {\r\n");
                foreach (string ss in mCS)
                {
                    codeStruct.Add("case " + ss + ":");
                    for (int ii = 0; ii < 6; ii++)
                    {
                        codeStruct.Add("    SetMenuButton(" + Convert.ToString(ii) + ", );");
                    }
                    codeStruct.Add("break;");
                };
                codeStruct.Add("default:");
                codeStruct.Add("    return;");
                codeStruct.Add("}");
 


            //***********************************************************************************




            using (StreamWriter outputFile = new StreamWriter(Path.Combine(GrafDir + "\\DOC", "defStruct.txt")))
            {

                string[] lines = new string[codeStruct.Count];
                codeStruct.CopyTo(lines);

                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }

        }

        public string[] Init_WRtoFLASHfiles(string CATGrafDir, string NAMEDIR)
        {

            if (Directory.Exists(CATGrafDir))
            {
                string[] dirs = Directory.GetDirectories(CATGrafDir);
                foreach (string s in dirs)
                {
                    if (s.EndsWith(NAMEDIR))
                    {
                        return Directory.GetFiles(s);
                    }
                }

            }
            return null;
        }

    }



}






 
 