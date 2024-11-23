using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using COMMAND;
using UART;

/*
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using COMMAND;
 */

namespace RS485
{

    // Определение перечислений
    public enum DevRS485Type
    {
        dev_AllWideAddr = 1,//широковещательный адрес, используется для подсоединения и конфигурирования адреса новых устройств
        dev_PC = 2,//управляющий компьютер, выступает мастером для тпелоконтроллера
        dev_slaveTEPLOCONTROL = 3,//теплоконтроллер как SLAVE для компьютера
        dev_masterTEPLOCONTROL = 4,//теплоконтроллер как MASTER
        dev_PULT1 = 5,//выносной пульт номер 1
        dev_PULT2 = 6,//выносной пульт номер 1
        dev_PULT3 = 7,//выносной пульт номер 1
        dev_PULT4 = 8,//выносной пульт номер 1
        dev_TEPLOCOUNT = 9,//теплосчётчик ультразвуковой
        dev_IMPCOUNT1 = 10,//импульсный счётчик расхода
        dev_IMPCOUNT2 = 11,//импульсный счётчик расхода
        dev_IMPCOUNT3 = 12,//импульсный счётчик расхода
        dev_IMPCOUNT4 = 13,//импульсный счётчик расхода

        dev_MainBoarMediana = 66,//Main Board Mediana
    }

    /*
    // список доступных команд
    public enum funСRS485
    {
        getVer = 1,//Read Teplocontrol Information
        readVol = 2,//read data
        readCounter1
    }
    */
    

    public enum comRS485
    {
        DevInfo =2,//считывание информации об устройстве
        setmV0_10000 = 3,//запись милливольт в теплоконтроллер в диапазоне 0-10 вольт
        readImpCount = 4,//считывание данных из импульсногосчётчика

        readCurTemp  = 5,//считывание текущей температуры помещения с пульта
 
    }




    //            ******************** Counter *********************** в дальнейшем надо выделить в отдельный класс

    public enum funPulsar
    {//доступные для счётчика функции
        getVer = 0x01,//Значение счётчика в кубических дециметрах 
        readM3 = 0x0A,//чтение всего потряд
    }





    public class TRS485 : TComPortDigit
    {
        /*
                [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
                private static extern uint TimeBeginPeriod(uint uMilliseconds);

                [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
                private static extern uint TimeEndPeriod(uint uMilliseconds); */


    DevRS485Type DEV = DevRS485Type.dev_PC;//номер устройства в сети
        DevRS485Type readDEV;//номер устройства от которого было принято последнее сообщение



        public string parBoardVerSoft = "1.00";
        public string parBoardVerHard = "2.00";
        public string BoardString = "TEPLOCONTROL";
        public string SerNum = "1";

        public int DUMMY_COUNT = 250;



        //            ---------------------------------------------


        public int GetFromStringSensSerialNum(string input)
        {//
            if (input.Length != 8 || !int.TryParse(input, out int result))
            {
                return 0;
            }

            if (result >= 0 && result <= 99999999)
                return result;
            else
                return 0;
        }


        public void SendData(byte[] data)
        {
            if ( !IsOpen)
            {
                throw new InvalidOperationException("Serial port is not open.");
            }

            // Перевести линию RTS в активное состояние
            RtsEnable = true;
            // Отправка данных
            Write(data, 0, data.Length);
            // Ожидание завершения отправки данных
            BaseStream.Flush();
            // Проверка готовности передатчика
            while (BytesToWrite > 0)
            {
                Thread.Sleep(1);
            }
            // Перевести линию RTS в неактивное состояние
            RtsEnable = false;
        }


        public void SendtoDEV( DevRS485Type recDEV, byte[] data, comRS485 command)
        {//отправка сообщения по UART для получателя recDEV

            // Проверка текущего состояния handshake
            //Handshake currentHandshake = Handshake;
            //Console.WriteLine($"Текущее состояние Handshake: {currentHandshake}");

            Handshake = Handshake.None;

            Outbuf[0] = Convert.ToByte(recDEV);
            Outbuf[1] = Convert.ToByte(Convert.ToInt32(command) & 0xff);

            Array.Copy(data, 0, Outbuf, 2, data.Length);

            int _crc = CRC.CRC16(Outbuf, 0, Convert.ToUInt16(data.Length + 2));
            Outbuf[data.Length + 2] = (byte)(_crc & 0xff);
            Outbuf[data.Length + 3] = (byte)((_crc >> 8) & 0xff);
;

            if (TypeInterface == CommunicationType.RS485)
            {// Переключение в режим передачи в случае применения интерфейса RS485
                RtsEnable = true;
                // Добавление задержки после установки RtsEnable = true
                Thread.Sleep(15);
            }
            Write(Outbuf,0, data.Length + 4);
            // Убедиться, что все данные отправлены
            BaseStream.Flush();

            RtsEnable = false;
        }






        public byte[] SendtoDEVwithANSV(DevRS485Type recDEV, byte[] data, comRS485 command, int startDelayMs = startReadTimeout_ms * 5, int nextDelayMs = nextReadTimeout_ms)
        {
            DiscardInBuffer();//очистим входной буфер COM порта от возможного мусора
            SendtoDEV(recDEV, data, command);
            return ReadBytesTimeOutCheckCRC(startDelayMs, nextReadTimeout_ms);
        }

        public String GetDevInfo(DevRS485Type Dev)
        {// в параметре Dev передаётся информацию о каком устройстве необходимо получить информацию
            int intDev = (int)Dev;
            byte[] buf = BitConverter.GetBytes(intDev);
            byte[] inbuf = SendtoDEVwithANSV(DevRS485Type.dev_slaveTEPLOCONTROL, buf, comRS485.DevInfo, startReadTimeout_ms * 2);
            return (Encoding.UTF8.GetString(inbuf));//получаем в ответ стринг, который можно внести в заголовок
        }











        public byte[] SendTestPulsCountwithANSV( byte[] data,  int startDelayMs = startReadTimeout_ms * 5, int nextDelayMs = nextReadTimeout_ms)
        {
            DiscardInBuffer();//очистим входной буфер COM порта от возможного мусора
                              // SendtoDEV(recDEV, data, command);


            if (TypeInterface == CommunicationType.RS485)
            {// Переключение в режим передачи в случае применения интерфейса RS485
                RtsEnable = true;
                // Добавление задержки после установки RtsEnable = true
                Thread.Sleep(15);
            }
            Write(data, 0, data.Length);
            // Убедиться, что все данные отправлены
            BaseStream.Flush();
            RtsEnable = false;
            return ReadBytesTimeOutCheckCRC(startDelayMs, nextReadTimeout_ms);
        }

        public void  WR_0_10V(int mV)
        {
            byte[] buf = new byte[4];
            if (mV > 10000)
                mV = 10000;
            buf[0] = Convert.ToByte(mV & 0xff);
            buf[1] = Convert.ToByte((mV >> 8) & 0xff);
            buf[2] = 0;
            buf[3] = 0;
            SendtoDEV(DevRS485Type.dev_slaveTEPLOCONTROL, buf, comRS485.setmV0_10000);
 //           return SendtoDEVwithANSV(DevRS485Type.dev_slaveTEPLOCONTROL, buf, comRS485.setmV0_10000 ,  startReadTimeout_ms * 2);
        }

        /*
        ******************************************  чтение счётчика                                                     .TVf.........,
      08 54 56 66  : Серийный номер устройства
      01           : Код функции чтения   
      0E           : Длина посылки - 14 байт
      A0 00 00 00  : Получается здесь зашифровано 160 и это 0.16 метров кубических, тоесть значнеие нужно делить на 1000
      01 00        : Идентификатор какой то
      C6 2C        : CRC16
      *********************************************************************
         */

        public int readCountPulsarMini(int serialNum)
        {// чтение количества кубических дециметров с ответом в качестве int

/*
            DUMMY_COUNT = DUMMY_COUNT + 50;
            return (DUMMY_COUNT);
*/

            byte[] template = { 0x00, 0x00, 0x00, 0x00, 0x01, 0x0E, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
            byte[] buf =  BitConverter.GetBytes(serialNum);
            byte[] bserialNum = new byte[4];
            bserialNum[0]= buf[3];
            bserialNum[1] = buf[2];
            bserialNum[2] = buf[1];
            bserialNum[3] = buf[0];
            Array.Copy(bserialNum, 0, template, 0, 4);//вставляем серийный номер счётчика в массив
            //добавим CRC в конец массива шаблона запроса чтения количества кубических дециметров
            int _crc = CRC.CRC16(template, 0, Convert.ToUInt16(template.Length - 2));
            template[template.Length - 2] = (byte)(_crc & 0xff);
            template[template.Length -1] = (byte)((_crc >> 8) & 0xff);
            DiscardInBuffer();//очистим входной буфер COM порта от возможного мусора
            SendtoDEV(DevRS485Type.dev_slaveTEPLOCONTROL, template, comRS485.readImpCount);

            byte[] tmparr = ReadBytesTimeOut(1000,250);

            int Count_dM3 = Convert.ToInt32(tmparr[6]) + Convert.ToInt32(tmparr[7]) * 0x100 + Convert.ToInt32(tmparr[8]) * 0x10000 + Convert.ToInt32(tmparr[9]) * 0x1000000;

            return (Count_dM3);//возвращаем значение
        }

        static int DecimalToHex(int decimalNumber)
        {
            string hexString = decimalNumber.ToString("X");
            return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        static int HexToDecimal(int hexNumber)
        {
            string decimalString = hexNumber.ToString();
            return int.Parse(decimalString);
        }

    }
}
