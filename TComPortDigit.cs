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

namespace UART
{
	// Определение перечисления для возможных значений свойства
	public enum CommunicationType
	{
		UART,
		CAN,
		RS485
	}



	public static class CRC
	{
		public static readonly byte[] auchCRCHi =
		{
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
		0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
		0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
		0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
		0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
		0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
		0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
		0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
		0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
		0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
		0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
		0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
		0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
		0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
		0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
		0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
		0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
		0x40
		};


		public static readonly byte[] auchCRCLo =
	{0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
		0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
		0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
		0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
		0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
		0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
		0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
		0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
		0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
		0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
		0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
		0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
		0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
		0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
		0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
		0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
		0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
		0x40
		};

		public static ushort CRC16(byte[] puchMsg, int StartAdr, int usDataLen)
		{
			int i;
			byte uchCRCHi = 0xFF; /* high byte of CRC initialized */
			byte uchCRCLo = 0xFF; /* low byte of CRC initialized */
			byte uIndex; /* will index into CRC lookup table */
			for (i = 0; i < usDataLen; i++) /* pass through message buffer */
			{
				uIndex = (byte)(uchCRCLo ^ puchMsg[i + StartAdr]); /* calculate the CRC */
				uchCRCLo = (byte)(uchCRCHi ^ auchCRCHi[uIndex]);
				uchCRCHi = auchCRCLo[uIndex];
			}
			return (ushort)(uchCRCHi << 8 | uchCRCLo);
		}

		public static bool checkCRC16(byte[] puchMsg, int StartAdr, int usDataLen)
		{
			if (CRC16(puchMsg, StartAdr, usDataLen) == (ushort)(puchMsg[StartAdr + usDataLen] + puchMsg[StartAdr + usDataLen + 1] * 256))
				return true;
			else
				return false;

		}

		public static bool check0CRC16(byte[] puchMsg, int length)
		{
			if (CRC16(puchMsg, 0, length) == 0)
				return true;
			else
				return false;

		}


	}


		public class TComPortDigit : SerialPort
    {


		const int constAnsvTimeOut = 5000;
		const int constAnsvOneByteTimeOut = 200;

		public const int LenServByte = 4;
		public const int ExhLenServByte = 6;

		public const int constMaxLenIO = 256 + ExhLenServByte;
		//		public const int constMaxTimeOUTms =  ;


		public const int MaxLenComand = 255;

		public byte[] Inbuf = new byte[constMaxLenIO];
		public byte[] Outbuf = new byte[constMaxLenIO];

		public CommunicationType Interface { get; set; }
		protected CommunicationType TypeInterface = CommunicationType.UART;

		protected int _CountReadByte = 0;
		int _CountWriteByte = 0;
		public const int startReadTimeout_ms = 100;
		public const int nextReadTimeout_ms = 25;
		//		public string MessageStr;



		protected int _inComDataLen = 0;
		protected int _outComDataLen = 0;

		public int inComLen
		{ 
			get { return _inComDataLen; }
		}


		public int outComLen
		{
			get { return _outComDataLen; }
		}


/*

*/

	public int WriteToUART(byte[] buff, int countByte )
		{


			Write(buff, 0, countByte);
			return countByte;
		}








		public byte[] ReadCommand(int ServByte = 6, int TimeOutStartAnsv = 150, int TimeOutNextByte = 50)
		{//чтение команды 
			/*
			 команда имеет следующие необязательные параметры:
			ServByte - добавочные к байтам данных служебные байты, 
			   для стандартной команды обмена с одним устройством это значение равно расширенной на 4 байта
			   для расширенной 6 байт
			   с CAN хабом обмен идёт по расширенному протоколу
			
			TimeOutStartAnsv - таймаут ожидания первого байта посылки
			TimeOutNextByte - таймаут между приёмом байт в посылке
			 */
			int TimeOutread = 0;
			int _CountReadByte = 0;
			int CountByte = ServByte;
			int TSleep = 25;

			while (_CountReadByte < constMaxLenIO)
			{

				while (BytesToRead > 0)
				{//new byte in buffer

					Inbuf[_CountReadByte] = (byte)(ReadByte());
					if (_CountReadByte == 0)
					{
						CountByte = Inbuf[0] + ServByte;

					}
					_CountReadByte++;
					TimeOutread = 0;//если уже дождались начала чтения уменьшаем таймаут
					if (_CountReadByte >= CountByte)
					{
						byte[] bytes1 = new byte[CountByte];
						Array.Copy(Inbuf, 0, bytes1, 0, CountByte);
						return bytes1;//все данные успешно считаны
					}
				}
				Thread.Sleep(TSleep);
				TimeOutread += TSleep;
				if (_CountReadByte == 0)
				{
					if (TimeOutread >= TimeOutStartAnsv)
					{
						throw new Exception("Превышен таймаут ответа на команду номер " + Convert.ToString(TimeOutStartAnsv) + " в " + Convert.ToString(TimeOutStartAnsv) + "mc");
					}

				}
				else
				{
					if (TimeOutread >= TimeOutNextByte)
						throw new Exception("Неполный ответ на команду " + Convert.ToString(Inbuf[0]) + "считано " + Convert.ToString(_CountReadByte) + "байт вместо " + Convert.ToString(CountByte));
				}


			}
			byte[] bytes = new byte[CountByte];
			Array.Copy(Inbuf, 0, bytes, 0, CountByte);
			return bytes;//все данные успешно считаны
		}

		public byte[] ReadBytesTimeOut(int startDelayMs = startReadTimeout_ms, int nextDelayMs = nextReadTimeout_ms)
		{//чтение байт с выходом по таймауту и проверкой контрольной суммы
			int TimeOutread = startDelayMs;
			int tmp_CountReadByte = 0;
			_CountReadByte = 0;
			while (_CountReadByte < constMaxLenIO)
			{
				Thread.Sleep(TimeOutread);

				while (BytesToRead > 0)
				{//new byte in byffer
					Inbuf[_CountReadByte++] = (byte)(ReadByte());
					TimeOutread = nextDelayMs;
				}

				if (tmp_CountReadByte == _CountReadByte)
					break;
				else
					tmp_CountReadByte = _CountReadByte;
			}
			if (_CountReadByte == 0)
				throw new Exception("Не получен ответ");
			byte[] result = new byte[_CountReadByte];
			Array.Copy(Inbuf, 0, result, 0, _CountReadByte);
			return result;

		}

		public byte[] ReadBytesTimeOutCheckCRC(int startDelayMs = startReadTimeout_ms, int nextDelayMs = nextReadTimeout_ms)
		{//чтение байт с выходом по таймауту и проверкой контрольной суммы
			int TimeOutread = startDelayMs;
			int tmp_CountReadByte = 0;
			_CountReadByte = 0;
			while (_CountReadByte < constMaxLenIO)
			{
				Thread.Sleep(TimeOutread);

				while (BytesToRead > 0)
				{//new byte in byffer
					Inbuf[_CountReadByte++] = (byte)(ReadByte());
					TimeOutread = nextDelayMs;
				}

				if (tmp_CountReadByte == _CountReadByte)
					break;
				else
					tmp_CountReadByte = _CountReadByte;
			}
			if (_CountReadByte == 0)
				throw new Exception("Не получен ответ");

			if (CRC.check0CRC16(Inbuf, _CountReadByte))
			{
				byte[] result = new byte[_CountReadByte - 4];
				Array.Copy(Inbuf, 2, result, 0, _CountReadByte - 4);

				return result;
			}
			else
				throw new Exception("Ошибка контрольной суммы при чтении данных");
		}








		public bool GetOpenComport(string ComPortName, int baudrate, bool ShowMessage)
		{
			DialogResult result;
			try
			{
				if (IsOpen)
					Close();
				PortName = ComPortName;
				BaudRate = baudrate;
				Open();

				//   Properties.Settings.Default.COMportName = ComPortName;
				//   Properties.Settings.Default.Save();

				if (ShowMessage)
				{
					result = MessageBox.Show(
					"Порт :" + ComPortName + " успешно инициализирован.",
					"Сообщение",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information,
					MessageBoxDefaultButton.Button1,
					MessageBoxOptions.DefaultDesktopOnly);
				}

			}
			catch (Exception ex)
			{
				if (ShowMessage)
				{
					MessageBox.Show(ex.Message, "Не удаётся открыть порт" + ComPortName,
					   MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				return false;
			}
			return true;
		}





		public bool IsPortOpen(ListBox.ObjectCollection Items)
        {
            //Создаем переменную для возврата 
            //состояния получения портов.
            //true в случае успешного получения 
            //false если порты не найдены.
            bool available = false;
            Items.Clear();
            //Представляет ресурс последовательного порта.
            SerialPort Port;

            //Выполняем проход по массиву имен 
            //последовательных портов для текущего компьютера
            //которые возвращает функция SerialPort.GetPortNames().
            foreach (string str in System.IO.Ports.SerialPort.GetPortNames())
            {
                try
                {
                    if ((str == PortName) && (IsOpen))
                    {//работаем уже с открытым нами портом
                        Items.Add(str);
                        available = true;
                        continue;
                    }


                    Port = new System.IO.Ports.SerialPort(str);
                    //Открываем новое соединение последовательного порта.
                    Port.Open();

                    //Выполняем проверку полученного порта
                    //true, если последовательный порт открыт, в противном случае — false.
                    //Значение по умолчанию — false.
                    if (Port.IsOpen)
                    {
                        //Если порт открыт то добавляем его в listBox
                        Items.Add(str);

                        //Уничтожаем внутренний объект System.IO.Stream.
                        Port.Close();

                        //возвращаем состояние получения портов
                        available = true;
                    }
                }
                //Ловим все ошибки и отображаем, что открытых портов не найдено               
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Не найден ни один актвный COM порт",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //возвращаем состояние получения портов
            return available;
        }


    }
}
