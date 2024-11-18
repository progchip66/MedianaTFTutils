using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UART;
using COMMAND;
using FileCreater;
using System.Threading;
using TFTprog;
using System.ComponentModel;
using System.IO;

namespace ExtHubComm
{//хаб использует расширенный вариант команд для передачи данных внешним платам и приёма данных от них

    // public enum Efb_Head { fb_PC = 0, fb_CANHUB, fb_MBmediana, fb_TFTmediana, fb_feudetect }




    public class BoardVer
    {
        private Efl_BoardRej BoardRej = Efl_BoardRej.flr_none;
        private Efl_DEV TypeBoard;

        private int Ver=0;
        private int Subver = 0;
        private int progVer = 0;
        private int progSubver = 0;
        private string _StrVer = "Bord not defined";
 //       private string Comment="";


        public void GetBoardVer(byte[] Data)
        {
            TypeBoard = (Efl_DEV)Data[0];
            BoardRej = (Efl_BoardRej)Data[1];
            Ver = Data[2];
            Subver = Data[3];
            progVer = Data[4];
            progSubver = Data[5];
        }




        public string Version
        {
            get
            {

                _StrVer = "   Board ";
                string tmpStr = "";
                switch (TypeBoard)
                    {
                    // public enum Efl_DEV { fld_none = 0, fld_PC, fld_HUB, fld_MainBoard, fld_TFTboard, fld_FEUdetect };//тип устройства
                    case Efl_DEV.fld_PC:
                        tmpStr = "PC";
                        break;
                    case Efl_DEV.fld_HUB:
                        tmpStr = "HUB";
                        break;
                    case Efl_DEV.fld_MainBoard:
                        tmpStr = "MainBoar";
                        break;
                    case Efl_DEV.fld_TFTboard:
                        tmpStr = "TFTboard";
                        break;
                    case Efl_DEV.fld_FEUdetect:
                        tmpStr = "FEUdetect";
                        break;
                    default:
                        _StrVer = _StrVer + ":UnknounBoard";
                        return _StrVer;
                }
                _StrVer = _StrVer +  tmpStr + ": ";

                switch (BoardRej)
                {
                    case Efl_BoardRej.flr_BOOT:
                        tmpStr = "BootRej";
                        break;
                    case Efl_BoardRej.flr_WORK:
                        tmpStr = "WorkRej";
                        break;
                    default:
                        return "_StrVer" + ":UnknounRej";
                }
                _StrVer= _StrVer  + tmpStr + "  | ";


                _StrVer = _StrVer + "  HardVer: " + Convert.ToString(Ver) + "." + Convert.ToString(Subver) + "  ";
                _StrVer = _StrVer + "/ SoftVer: " + Convert.ToString(progVer) + "." + Convert.ToString(progSubver) + "  ";
                return _StrVer; 
            }

        }
    }
    //get { return (ECommand)(RxBuff[1]); }




    class SCANHUB : SCommStruct
    {

       
        public int adrBASEMenuGPUpict
        {
            get { return Resurs.GetAlignVol(0x100000 - (480 * 272 * 2), 64, 0); }
        }


        
        public int RAMsize { get { return FM.RAMsize; } }
        public int FLASHsize { get { return FM.FLASHsize; } }
        public int StartFLASHadr { get { return FM.StartFLASHadr; } }
        public int StartFLASHadrBLOB { get { return FM.StartFLASHadrBLOB; } }
        public int FLASHalign { get { return FM.FLASHalign; } }
        public int RAMalign { get { return FM.RAMalign; } }

        public string DirFlowPro;
        public int StartFLASHadrFlowPro;
        public int Up1Doun0adrFLASH;

        public int enProMenu = 0;
        public int enProFontPict = 0;

        public int enProAddOneMenuFile = 0;
        public string OneFileName="";
        public int OneFileNum = 0;

        public int maxAttempt = 3;//максимальное количество попыток записи
        public int countAttempt = 0;//cчётчик попыток записи



        public ERejWork ExhRejWork = ERejWork.ervTFT_master;//режим работы обмена данными

        public BoardVer CAN_HUB = new BoardVer();
        //      BoardVer Device = new BoardVer();

        public BoardVer MAIN_Board = new BoardVer();
        //      BoardVer Device = new BoardVer();

        public BoardVer TFT_Board = new BoardVer();
        //      BoardVer Device = new BoardVer();     

        public TResurser Resurs = new TResurser();
        public TFileManager FM = new TFileManager();

        public SminmaxParams MinmaxParams = new SminmaxParams(13);

        public int BlockSize = 4096;
        public int DataSize = 252;
        

        /*   class TFileManager
           {
               public const int RAMsize = 0x100000; //1Mbyte
               public const int FLASHsize = 0x800000; //8Mbyte
               public const int StartFLASHadr = 0x100000; //1Mbyte

               public const int FLASHalign = 0x1000; //4096 byte
               public const int RAMalign = 64; //64 byte*/



        public string GetVerDev(BoardVer Dev, Efl_DEV DevType)
        {
            CommSendAnsv(ECommand.cmd_get_Ver, DevType,null,500);
            switch (DevType)
            {
                case Efl_DEV.fld_HUB:
                    CAN_HUB.GetBoardVer(RxBuff);
                    return CAN_HUB.Version;
                case Efl_DEV.fld_MainBoard:
                    MAIN_Board.GetBoardVer(RxBuff);
                    return CAN_HUB.Version;
                case Efl_DEV.fld_TFTboard:
                    TFT_Board.GetBoardVer(RxBuff);
                    return TFT_Board.Version;
                default:
                    return (":UnknounBoard");
            }
        }

        //Установка (считывание если параметр равен нулю) нового системного времени и даты путём отправки в плату управления

        public  long GetRTCDateTime( Efl_DEV DevType)
        {
            byte[] comm_data = new byte[4] { 0, 0, 0, 0 };
            CommSendAnsv(ECommand.cmd_SetRTCDateTime, DevType, comm_data, 100);
            long result = BitConverter.ToUInt32(RxBuff, 0);
            return result;
        }
        public void SetRTCDateTime(long number, Efl_DEV DevType)
        {
            byte[] comm_data = new byte[4] { 0, 0, 0, 0 };
            comm_data = BitConverter.GetBytes((uint)(number & 0xFFFFFFFF));
            CommSendAnsv(ECommand.cmd_SetRTCDateTime, DevType, comm_data, 100);
            long result = BitConverter.ToUInt32(RxBuff, 0);
            if (result != number)
            {
                throw new Exception("Ошибка операции записи-чтения в RTC MainBoard");
            }
        }

        // Функция SetSpeedFromStr
        public void SetSpeedFromStr(string xSpeed)
        {
            int ret=1;
            // Удаление всех символов, кроме цифр
            string numericString = new string(xSpeed.Where(char.IsDigit).ToArray());

            // Преобразование в целое число
            if (int.TryParse(numericString, out int result))
            {
                ret = result;
            }
            byte[] byteArray = BitConverter.GetBytes(ret);
            CommSendAnsv(ECommand.cmd_TimeAccelerat, Efl_DEV.fld_MainBoard, byteArray, 500);
        }


        //	public enum Efl_DEV { fld_PC = 0, fld_HUB, fld_MainBoard, fld_TFTboard, fld_FEUdetect, fld_none = 0x0f };//тип устройства
        public ERejWork ChangeDEVExhRejWork(ERejWork REJ, Efl_DEV DevType)
        {
            
            byte[] comm_data = new byte[4] { 0, 0, 0, 0 };
            comm_data[0]= Convert.ToByte(REJ);
            //CommSendAnsv(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, byte SubCom = 0, int TimeOutStartAnsv = 500, int TimeOutNextByte = 100)
            CommSendAnsv(ECommand.cmd_Rejim, DevType, comm_data,1500);
            return (ERejWork)(RxBuff[0]);
        }

        public byte[] GenerateByteArray(int NumBytes)
        {
            byte[] byteArray = new byte[NumBytes];
            for (int i = 0; i < NumBytes; i++)
            {
                byteArray[i] = (byte)i;
            }
            return byteArray;
        }

        public void TestPictureWrite(int num, Efl_DEV DevType)
        {
            byte[] bTest = GenerateByteArray(num);
            CommSendAnsv(ECommand.cmd_test, DevType, bTest,  1500);
            Thread.Sleep(50);
        }
        

        //     cmd_GetAKVAparFromPC = 0x60,//PC как мастер считывает структуру допустимых параметров
        //		cmd_SendAKVAparToPC = 0x61, //PC как мастер Записывает структуру допустимых параметров

        public void GetAKVAparFromMainBoard()
        {//формируем параметры из считанного массива
            CommSendAnsv(ECommand.cmd_MaxminAKVAparMCUtoPC, Efl_DEV.fld_MainBoard);
            MinmaxParams.getParamsFromArr(RxBuff);

        }

        #region SendDataTestPro

        public byte[] fileDataFLASH;

        public byte[] CreatDataTestPro( int CountByte)
        {
            byte[] Arrtmpb = new byte[CountByte];

            for (int i = 0; i < CountByte / 4; i++)
            {
                int tmpi = i;
                Arrtmpb[i * 4] = Convert.ToByte(tmpi & 0xff);
                tmpi = tmpi >> 8;
                Arrtmpb[i * 4 + 1] = Convert.ToByte(tmpi & 0xff);
                tmpi = tmpi >> 8;
                Arrtmpb[i * 4 + 2] = Convert.ToByte(tmpi & 0xff);
                tmpi = tmpi >> 8;
                Arrtmpb[i * 4 + 3] = Convert.ToByte(tmpi & 0xff);
            }
            return Arrtmpb;
        }

        public string GetSringtEnumEMemPRO(EMemPRO memPRO)
        {
            string ret="";
            switch (memPRO)
            {
                case EMemPRO.eopmpro_none:
                    ret ="eopmpro_none";
                    break;

                case EMemPRO.eopmpro_setpar:
                    ret = "eopmpro_setpar";
                    break;
                case EMemPRO.eopmpro_wrGRAM:
                    ret = "eopmpro_wrGRAM";
                    break;
                case EMemPRO.eopmpro_ready:
                    ret = "eopmpro_ready";
                    break;
                case EMemPRO.eopmpro_wrTFTFLASH:
                    ret = "eopmpro_wrTFTFLASH";
                    break;
                case EMemPRO.eopmpro_wrFLASHOK:
                    ret = "eopmpro_wrFLASHOK";
                    break;
                case EMemPRO.eopmpro_errStart:
                    ret = "eopmpro_errStart";
                    break;
                case EMemPRO.eopmpro_errlen:
                    ret = "eopmpro_errlen";
                    break;

                case EMemPRO.eopmpro_wrGRAMerr:
                    ret = "eopmpro_err";
                    break;
                case EMemPRO.eopmpro_errformatcomm:
                    ret = "eopmpro_errformatcomm";
                    break;
                case EMemPRO.eopmpro_errBufCRC:
                    ret = "eopmpro_errBufCRC";
                    break;
                case EMemPRO.eopmpro_errOptMemCRC:
                    ret = "eopmpro_errOptMemCRC";
                    break;
                case EMemPRO.eopmpro_errOptMemWr:
                    ret = "eopmpro_errOptMemWr";
                    break;
            };
            return ret;
        }


        public  EMemPRO WrTFTFLASHFlowPro( Efl_DEV _RecDev)
        {
            byte[] comm_data = new byte[1];
            comm_data[0] = Convert.ToByte( EMemPRO.eopmpro_wrTFTFLASH);
            CommSendAnsv(ECommand.cmd_WR_flowMem, _RecDev, comm_data,2500);
            return (EMemPRO)(RxBuff[0]);
        }


        public void ReportError(string functionName, Exception ex, int processedBytes, BackgroundWorker worker)
        {
            string errorMessage = $"Ошибка в {functionName}. Обработано байт: {processedBytes}. ";
            if (ex != null)
            {
                errorMessage += $"Сообщение: {ex.Message}";
            }
            worker.ReportProgress(0, errorMessage); // Передаем сообщение через ProgressChanged
        }

        public eFLASHresultEnum Wr_RAMbuf_max252(byte[] data, int adr, Efl_DEV _RecDev)
        {//запись в промежуточный буфер памяти
            int dataSize;
            try
            {
                // Определяем количество байт для копирования
                dataSize = data.Length > 252 ? 252 : data.Length;

                // Создаем массив sendData: 2 байта для adr + dataSize байт данных
                byte[] sendData = new byte[2 + dataSize];

                // Извлекаем младшие байты adr
                sendData[0] = (byte)(adr & 0xFF);        // Младший байт
                sendData[1] = (byte)((adr >> 8) & 0xFF); // Старший байт

                // Копируем данные из массива data в массив sendData (только первые 252 байта, если их больше)
                Array.Copy(data, 0, sendData, 2, dataSize);

                CommSendAnsv(ECommand.cmd_wr_RAM4096buf, _RecDev, sendData, 500);
            
            }
            catch (Exception)
            {
                return eFLASHresultEnum.RAMBUF_READTIMEOUT;
            }
            return eFLASHresultEnum.ALL_OK;
        }

        //      cmd_wrver_TFTFLASH4096 = 0x33,//запись во FLASH через GRAM

        public eFLASHresultEnum   WrVer_TFTFLASH4096(int StartGPUadr, int StartTFTFLASHadr, int length_data, int _CRC, Efl_DEV _RecDev)
        {//запись c верификацией из промежуточного буфера памяти во FLASH
            byte[] sendData = new byte[14];
            try
            {
                // Помещаем StartGPUadr
                Array.Copy(BitConverter.GetBytes(StartGPUadr), 0, sendData, 0, 4);
                // Помещаем StartTFTFLASHad
                Array.Copy(BitConverter.GetBytes(StartTFTFLASHadr), 0, sendData, 4, 4);
                // Помещаем length_data
                Array.Copy(BitConverter.GetBytes(length_data), 0, sendData, 8, 4);
                // Помещаем два младших байта _CRC через Array.Copy
                Array.Copy(BitConverter.GetBytes(_CRC), 0, sendData, 12, 2);
                CommSendAnsv(ECommand.cmd_wrver_TFTFLASH4096, _RecDev, sendData,  1000);
                return (eFLASHresultEnum)(RxBuff[0]);//если равен нулю, процедура записи-верификации прошла успешно
            }
            catch (Exception)
            {
                return eFLASHresultEnum.ERR_TIMEOUT;
            }

        }


        //success = WriteBlock(BlockBytes,  Flashaddr, BlockSize, ref totalProcessedBytes, worker);

        public eFLASHresultEnum WriteBlock(byte[] BlockBytes, int validBytes,  int totalProcessedBytes, BackgroundWorker worker)
        {
            try
            {
                int totalBytes = validBytes;
                int offset = 0;

                while (totalBytes > 0)
                {
                    int bytesToWrite = Math.Min(DataSize, totalBytes);
                    byte[] dataChunk = new byte[bytesToWrite];
                    Array.Copy(BlockBytes, offset, dataChunk, 0, bytesToWrite);

                    eFLASHresultEnum success = eFLASHresultEnum.ERR_RAM_WRITE;
                    int attempt = 0;

                    while (attempt < 3 && (success!= eFLASHresultEnum.ALL_OK))
                    {
                        if (worker.CancellationPending)
                        {
                            return eFLASHresultEnum.ERR_RAM_WRITE; // Затем вручную установить флаг отмены
                        }

                        try
                        {
                            success = Wr_RAMbuf_max252(dataChunk, offset, Efl_DEV.fld_TFTboard);


                        }
                        catch (Exception ex)
                        {
                            attempt++;
                            ReportError("WriteData", ex, totalProcessedBytes, worker);
                            if (attempt == 3)
                            {
                                return eFLASHresultEnum.ERR_RAM_WRITE;
                            }
                        }

                        if (success != eFLASHresultEnum.ALL_OK)
                        {
                            attempt++;
                            ReportError("WriteData", null, totalProcessedBytes, worker);
                            if (attempt == 3)
                            {
                                return eFLASHresultEnum.ERR_RAM_WRITE;//Все три попытки оказались неудачными
                            }
                        }
                    }

                    offset += bytesToWrite;
                    totalBytes -= bytesToWrite;
                    totalProcessedBytes += bytesToWrite;
                }




                return eFLASHresultEnum.ALL_OK; // Успешная запись всех данных в блоке
            }
            catch (Exception ex)
            {
                ReportError("WriteBlock", ex, totalProcessedBytes, worker);
                return eFLASHresultEnum.ERR_RAM_WRITE;
            }
        }

        public eFLASHresultEnum WritePict(string Filename, int Flashaddr, BackgroundWorker worker)
        {
            int totalProcessedBytes = 0;
            int procentFile = 0;

            try
            {
                using (FileStream fileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read))
                {
                    byte[] BlockBytes = new byte[BlockSize];
                    int bytesRead;
                    long fileSize = fileStream.Length;


                    while ((bytesRead = fileStream.Read(BlockBytes, 0, BlockSize)) > 0)
                    {
                        if (bytesRead < BlockSize)
                        {
                            for (int i = bytesRead; i < BlockSize; i++)
                            {
                                BlockBytes[i] = 0xFF;
                            }
                        }

                        eFLASHresultEnum success = eFLASHresultEnum.ERR_FLASH_WRITE;
                        int attempt = 0;

                        while (attempt < 3 && (success != eFLASHresultEnum.ALL_OK))
                        {
                            try
                            {// public bool WriteBlock(byte[] BlockBytes, int Flashaddr, int validBytes,  ref int totalProcessedBytes, BackgroundWorker worker)
                                success = WriteBlock(BlockBytes,  BlockSize,  totalProcessedBytes, worker);
                                if (success == eFLASHresultEnum.ALL_OK)
                                {//если данные успешно записаны в RAM буфер делаем попытку записи их во FLASH память TFT панели
                                    int _crc = CRC16(BlockBytes, 0, BlockSize);
                                    success = WrVer_TFTFLASH4096(0, Flashaddr, BlockSize, _crc, Efl_DEV.fld_TFTboard);

                                }
                                

                                if (worker.CancellationPending)
                                {
                                    return eFLASHresultEnum.ERR_FLASH_WRITE; // Затем вручную установить флаг отмены
                                }
                            }
                            catch (Exception ex)
                            {
                                attempt++;
                                ReportError("WriteBlock", ex, totalProcessedBytes, worker);
                                if (attempt == 3)
                                {
                                    return eFLASHresultEnum.ERR_CANCELPRO;
                                }
                            }

                            if (success != eFLASHresultEnum.ALL_OK)
                            {
                                attempt++;
                                ReportError("WriteBlock", null, totalProcessedBytes, worker);
                                if (attempt == 3)
                                {
                                    return eFLASHresultEnum.ERR_FLASH_WRITE;
                                }
                            }
                        }
                        totalProcessedBytes += BlockSize;
                        Flashaddr += BlockSize;
                        int currentProcentFile = (int)(((double)totalProcessedBytes / fileSize) * 100);
                        if (currentProcentFile > 100)
                            currentProcentFile = 100;

                        if (currentProcentFile >= procentFile + 5)
                        {
                            procentFile = currentProcentFile;
                            worker.ReportProgress(procentFile);
                        }
                    }
                }

                return eFLASHresultEnum.ALL_OK; // Успешная запись всех блоков
            }
            catch (Exception ex)
            {
                ReportError("WritePict", ex, totalProcessedBytes, worker);
                return eFLASHresultEnum.ERR_FLASH_WRITE;
            }
        }



        public int[] GetNumbersFromFile(string filePath)
        {
            // извлечение из файла числовых данных в массив
            string fileContent = File.ReadAllText(filePath);
            List<int> numbers = new List<int>();

            string currentNumber = "";

            // Проход по всем символам файла
            foreach (char c in fileContent)
            {
                // Если текущий символ цифра, добавляем его к числу
                if (char.IsDigit(c))
                {
                    currentNumber += c;
                }
                else
                {
                    // Если мы встретили не цифру и число накопилось, добавляем его в список
                    if (currentNumber != "")
                    {
                        numbers.Add(int.Parse(currentNumber));
                        currentNumber = ""; // Сбрасываем строку для следующего числа
                    }
                }
            }

            // Добавляем последнее число, если оно есть
            if (currentNumber != "")
            {
                numbers.Add(int.Parse(currentNumber));
            }

            // Возвращаем массив чисел
            return numbers.ToArray();
        }



        #endregion






    }
}
