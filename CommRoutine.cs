using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UART;

using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace COMMAND
{

	public sealed class CommArgs
	{
		public ECommand Command;
		public Efl_DEV RecDev;
		public byte[] Data;
		public int Timeout;
	}

	public class DataReceivedEventArgs : EventArgs
	{//создаём класс для передачи данных события
		public byte[] Data { get; }

		public DataReceivedEventArgs(byte[] data)
		{
			Data = data;
		}
	}


	public enum Efl_Head { flh_none = 0, flh_set, flh_send, flh_end, flh_stat };
	public enum Efl_DEV { fld_PC = 0, fld_HUB, fld_MainBoard, fld_TFTboard, fld_FEUdetect, fld_none = 0x0f };//тип устройства
	public enum Efl_BoardRej { flr_none = 0, flr_BOOT, flr_WORK };

	public enum ETypeMem { etmem_none, etmem_MCURAM, etmem_MCUFLASH, etmem_ExtFLASH, etmem_GPURAM, etmem_TFTFLASH };//тип памяти
	public enum EMemPRO
	{
		eopmpro_none,
		eopmpro_setpar,//set Header process
		eopmpro_wrGRAM,//процесс записи в буфер GRAM (4096байт)
		eopmpro_ready,//success end folow write to GRAM process	
		eopmpro_checkGRAM,//проверка CRC всего записанного блока
		eopmpro_okGRAM,//записанные в GRAM байты имеют верную CRC
		eopmpro_wrTFTFLASH,//запись из GRAM во внешнюю FLASH панели
		eopmpro_wrFLASHOK,//TFT flash write successful!
		eopmpro_errStart = 0x80,//возникла ошибка при старте оцередного процесса записи в GRAM
		eopmpro_errlen,//counting error
		eopmpro_wrGRAMerr,//ошибка в ходе потоковой записи в GRAM
		eopmpro_errformatcomm,// error format (sample answer)
		eopmpro_errBufCRC,//записанные в промежуточный буфер(например GRAM) данные имеют неверный CRC
		eopmpro_errOptMemCRC,//записанные из промежуточного буфера(например GRAM) в конечную память(например TFTFLASH) данные имеют неверный CRC
		eopmpro_errOptMemWr,//ошибка записи в конечную память (например TFTFLASH)
	}

	public  enum eFLASHresultEnum
	{
		ALL_OK = 0,
		RAMBUF_READTIMEOUT = 1,
		RAMBUF_WRITETIMEOUT = 2,

		START_ERROR = 0x80,

		ERR_BUSY = 0x81,
		ERR_NOT_CLEAR = 0x82,
		ERR_VERIFY_FAILED = 0x83,
		ERR_WRITE_PROTECT = 0x84,
		ERR_ADDRES = 0x85,
		ERR_OVERBUF = 0x86,
		ERR_LENGTH = 0x87,
		ERR_PAGESIZE = 0x89,
		ERR_FLASH_WRITE = 0x8A,
		ERR_READ = 0x8B,
		ERR_BLOB = 0xC8,
		ERR_COMM = 0x8D,
		ERR_COMM_FORMAT = 0x8E,
		ERR_DATA_LENGTH = 0x8F,
		ERR_CRC = 0X90,
		ERR_GRAM_CRC = 0X91,
		ERR_GRAM_READ = 0X92,
		ERR_CANCELPRO = 0X93,
		ERR_TIMEOUT = 0X94,
		ERR_RAM_WRITE = 0X95,
	};


	public enum ECommand
	{
		cmd_abort = 0x00,
		cmd_ReadyRead = 0x01,//команда разрешающая передачу данных от других устройств в качестве мастера

		cmd_wr_connect = 0x10,
		cmd_wr_wakeUp = 0x11,
		cmd_wr_config = 0x14,
		cmd_settime = 0x15,
		cmd_set_gpio = 0x16,
		cmd_wr_strlcd = 0x17,
		cmd_wr_strtft_font = 0x18,
		cmd_wr_disconnect = 0x1F,

		cmd_START_flowMem = 0x20,//Start flowMem Process
		cmd_WR_flowMem = 0x21,//Data write to different types of memory

		cmd_wr_RAM4096buf = 0x31,//запись байт в промежуточный буфер памяти размером 4096 байт
		cmd_rd_RAM4096buf = 0x32,//чтение байт из промежуточного буфера памяти размером 4096 байт
		cmd_wrver_TFTFLASH4096 = 0x33,//запись во FLASH через GRAM
		cmd_rd_TFTFLASH4096 = 0x34,
		cmd_rd_RAM4096bufCRC = 0x35,//getRAMbufCRC

		cmd_start_wrfile = 0x40,
		cmd_startdevpro = 0x41,
		cmd_procent = 0x42,
		cmd_startstopwrFLASH = 0x48,
		cmd_wrFLASH = 0x49,
		cmd_rdFLASH = 0x4A,
		cmd_startboot = 0x50,

		cmd_get_Ver = 0x54,//команда считывания версии программного и аппаратного обеспечения.

		cmd_RWpredef_Conf = 0x55,//передача содержания предустановленных значений конфигурации и запись/чтение конфигурации в RAM
		cmd_RWconf_FLASH = 0x56,//запись/чтение существующей конфигурации из RAM по заданному адресу во FLASH
		cmd_ExhParams = 0x57,//Команда обмена в разных режимах используется по разному
		cmd_Rejim = 0x58,//ВЫБОР РЕЖИМА РАБОТЫ - НОРМАЛЬНЫЙ, СИМУЛЯЦИЯ, ПАУЗА



		cmd_TFTmenu = 0x5B,//вывод меню на TFT панели

		cmd_SetRTCDateTime = 0x5C,////Установка (считывание если параметр равен нулю) нового системного времени и даты путём отправки в плату управления
		cmd_StartTFTcalibr = 0x5E,//Запуск режима калибровки TFT панели
		cmd_USER = 0x70,
		cmd_RDsensWRtimers = 0x71,//чтение сенсоров из таблицы PC в ответ приходит текущий режим работы  и состояние таймеров
		cmd_RdWrSens = 0x72,//Запись и чтение структуры параметров 

		cmd_rd_flow = 0x81,

		cmd_Rd_config = 0x90,
		cmd_rw_DAC = 0x92,
		cmd_get_gpio = 0x93,
		cmd_get_ADC = 0x94,
		cmd_rd_str = 0x95,
		cmd_BackUPdata = 0x96,//сохраненение/чтение данных и настроек в энергонезависимой памяти основной платы

		cmd_test = 0xfe,
		cmd_none = 0xff,

	};


	public enum ERejWork
	{
		ervTFT_master = 0x00,//команды(в основном конфигурация) передаются от TFT контроллера к MainBoard
		ervMB_master = 0x01,//команды передаются(или транслируются) от  MainBoard, TFT контроллер принудительно переходит в режим СТОП
		ervPC_master = 0x02,//команды передаются(или транслируются) от  MainBoard, TFT контроллер принудительно переходит в режим СТОП
		ervPICTupd = 0x03,//происходит закачка новых изображений в FLASH память TFT панели, работа по стандартному алгоритму останавливается обновление экранов происходит только для демонстрации загруженных картинок
		ervNewSetOK = 0x04,//ответ подтверждение прихода команды, никаких установок не происходит
		evrTFTcalibr = 0x05,//переход в режим калибровки TFT панели
		ervPC_SENSemulStart = 0x06, //процедура старта режима симуляции
		ervPC_SENSemulWork = 0x07, //режим симуляции когда данные датчиков извлекаются из компьютера
		ervPC_SemulPause = 0x08, //режим ПАУЗЫ В КОТОРОМ РАБОТА ПРИОСТАНАВЛИВАЕТСЯ В ТЕКУЩЕМ СОСТОЯНИИ(НУЖЕН ДЛЯ СИМУЛЯЦИИ)
	};



	public class SHeadCom
	{
		public int CountReadData=0;//количество считанных байт находящихся в массиве bytesReadData
		public  byte[] bytesReadData = new byte[256];//байты для хранения считанных командой данных
		public uint[] uintReadData = new uint[64];//байты для хранения считанных командой данных
		public float[] floatReadData = new float[64];//байты для хранения считанных командой данных
		private byte[] bytes = new byte[4] { 0, 0, 0, 0 };

		public S_subComOp subCom = new S_subComOp();//Класс установки субкоманды

		public byte[] getHeaderbytes()
        {//получаю байты из которых состоит заголовок, например для формирования новой команды
			bytes[3] = Convert.ToByte(subCom.data);
			return bytes;
		}
		public void getFromCommandArr(byte[] _bytes)
		{//получаю байты заголовка из внешнего массива данных
			bytes[0] = _bytes[0];
			bytes[1] = _bytes[1];
			bytes[2] = _bytes[2];
			bytes[3] = _bytes[3];
			subCom.data = _bytes[3];
		} 

		public bool GetValidHeaderData(byte[] _bytes)
        {
			if (!CRC.check0CRC16(_bytes, _bytes.Length))
				return false;
			getFromCommandArr(_bytes);//извлекаем HEADER из данных
			CountReadData = _bytes.Length - 4;
			Array.Copy(_bytes, 4, bytesReadData, 0, _bytes.Length-4);//копируем данные в буфер данных
			CopyAndReset(bytesReadData, uintReadData, floatReadData, CountReadData);
			return true;
        }

		public void CopyAndReset(byte[] bytesReadData, uint[] uintReadData, float[] floatReadData, int num)
		{
			// Проверяем допустимость num
			if (num > bytesReadData.Length)
			{
				num = bytesReadData.Length;
			}

			// Вычисляем количество байт с округлением вверх до кратного 4
			int paddedNum = (num + 3) / 4 * 4;

			// Обнуляем массивы uintReadData и floatReadData
			Array.Clear(uintReadData, 0, uintReadData.Length);
			Array.Clear(floatReadData, 0, floatReadData.Length);

			// Копируем данные в массив uintReadData
			for (int i = 0; i < paddedNum / 4 && i < uintReadData.Length; i++)
			{
				uintReadData[i] = BitConverter.ToUInt32(bytesReadData, i * 4);
			}

			// Копируем данные в массив floatReadData
			for (int i = 0; i < paddedNum / 4 && i < floatReadData.Length; i++)
			{
				floatReadData[i] = BitConverter.ToSingle(bytesReadData, i * 4);
			}
		}

		public int DataLength
		{
			get
			{
				return Convert.ToInt32(bytes[0]);

			}
			set
			{
				//prevLength = Convert.ToByte(value);
				bytes[0]= Convert.ToByte(value);
			}
		}


		public ECommand Comm
		{
			get
			{
				return (ECommand) (bytes[1]);     // возвращаем значение свойства
			}
			set
			{
				bytes[1] = Convert.ToByte(value);
			}
		}


		public Efl_DEV Rec
		{
			get
			{
				return (Efl_DEV)(bytes[2]&0x0f);     // возвращаем значение свойства
			}
			set
			{
				bytes[2] = Convert.ToByte((bytes[2]&0xf0) | Convert.ToByte(value));
			}
		}


		public Efl_DEV Trans
		{
			get
			{
				return (Efl_DEV)((bytes[2] & 0xf0)>>4);     // возвращаем значение свойства
			}
			set
			{
				bytes[2] = Convert.ToByte((bytes[2] & 0x0f )| (Convert.ToByte(value)<<4));
			}
		}




	}



	public class S_subComOp
	{//описывает субкоманду
		private int prevV_WRflowPro = 0;
		private int prevV_DEV=0;
		private int prevV_fnoAnsv=0;


		public int WRflowPro
		{
			get
			{
				if (prevV_WRflowPro == 0)
					return 0;
				else
					return 1;
			}
			set
			{
				if (value == 0)
					prevV_WRflowPro = 0;
				else
					prevV_WRflowPro = 1;
			}
		}

		public ETypeMem DEV
        {
			get
			{
				return  (ETypeMem)(prevV_DEV);     // возвращаем значение свойства
			}
			set
			{
				prevV_DEV = Convert.ToInt32(value) & 0x3f;
			}
		}

		public int fnoAnsv
        {
			get
			{
				if (prevV_fnoAnsv == 0)
					return 0;
				else
					return 1;
			}
			set
			{
				if (value == 0)
					prevV_fnoAnsv = 0;
				else
					prevV_fnoAnsv = 0x80;
			}
		}




		public byte data
		{
			get
			{			
				return Convert.ToByte(prevV_DEV + (prevV_WRflowPro<<6)+ (prevV_fnoAnsv<<7));    // возвращаем значение свойства
			}
			set
			{
				prevV_fnoAnsv = (value>>7)&1 ;
				prevV_WRflowPro = (value >> 6) & 1;
				prevV_DEV = value  & 0x3f;
			}
		}




	}






	public class SCommStruct : TComPortDigit
	{
		#region crcConstants


		public const Efl_DEV ThisDEV = Efl_DEV.fld_PC;
		public Efl_Head flowStat = Efl_Head.flh_none;
		//адреса полученные в результате приёма очередной команды
		public Efl_DEV incomDEVsend = Efl_DEV.fld_none;
		public Efl_DEV incomDEVrec = ThisDEV;
		//адреса для отправки данных в ходе трансфера или ответа
		public Efl_DEV outcomDEVsend = ThisDEV;
		public Efl_DEV outcomDEVrec = Efl_DEV.fld_none;

		public enum ERxBufStat { rxcom_none, rxcom_readpro, rxcom_ready, rxcom_work, rxcom_err_timeout = 0x80, rxcom_err_format, rxcom_err_numcom, rxcom_err_crc, rxcom_err_abort, err_rxbufoverflow, rxcom_err_DCLeqNULL, err_rxNumUSWEcom };


		#endregion

		public SHeadCom RxHeadCom = new SHeadCom(); //Класс для хранения заголовка(теперь и данных) принятой команды
		 
		public SHeadCom TxHeadCom = new SHeadCom(); //Класс для хранения заголовка(теперь и данных) отправляемой команды



//		public SWR_RAMtoMEM FlowProRAMtoMEM = new SWR_RAMtoMEM();//Класс процесса для записи/чтения данных в память TFT контроллера для хранения заголовка отправляемой команды


		

		public ERxBufStat RxBufStat = ERxBufStat.rxcom_none;



		private byte[] _RxBuff = new byte[constMaxLenIO];

		public SHeadCom SlaveRxHeadCom = new SHeadCom(); //Класс для хранения заголовка принятой в режиме Slave команды


//		public  byte[] _SlaveRxData = new byte[constMaxLenIO];//буфер для приёма данных в режиме Slave



//		private UInt32[] _RxBuffuIntPar = new UInt32[constMaxLenIO / 4];

		private byte[] _TxBuff = new byte[constMaxLenIO];




		#region NewSerialCode

		private readonly Mutex receivingMutex = new Mutex();
		private readonly Queue<byte> tempBuffer = new Queue<byte>();//Очередь в которую переносятся байты и COM порта

		//событие объявляется в классе ИЗДАТЕЛЕ, которым является TComPortDigit
		public event EventHandler<DataReceivedEventArgs> DataReceivedEvent;

		//public TComPortDigit(string portName, int baudRate)
		public SCommStruct()
		{


			// Запускаем фоновый поток для приёма данных
// ПОТОК НЕОБХОДИМО ЗАПУСТИТЬ ПОЗЖЕ /////////////			new Thread(SlaveReceiveLoop) { IsBackground = true }.Start();
		}

		#region SlaveReceiver






		#endregion

		public byte[] SendCommand(byte[] data, int startTimeoutMs = 50, bool ShowException = true  )
		{
			receivingMutex.WaitOne(); //приостанавливаем выполнение команды отправки-приёма до получения мьютекса

			try
			{//isReceiving - флаг приёма по UART в фоновом режиме


				if (!IsOpen)
				{
					throw new InvalidOperationException("Serial port is not open.");
				}

				// Перевести линию RTS в активное состояние
				RtsEnable = true;


//				string Pname = PortName;
//				int bbbb = BaudRate;
				int elapsedTime = 0;


				// Отправка данных
				Write(data, 0, data.Length);
				// Ожидание завершения отправки данных
				BaseStream.Flush();
				RtsEnable = false;


				if (startTimeoutMs > 0)// если startTimeoutMs==0, то ответа на команду не требуется
				{//если за startTimeoutMs так и не поступил первый байт ответа приём заканчивается с ошибкой 
				 //				List<byte> response = new List<byte>();//создаётся список List
					int expectedLength = 0;
					int curLen = 0;

					int interval = 10;
					byte[] buffer = new byte[256 + 6];
					while (elapsedTime <= startTimeoutMs)
					{

						if ((BytesToRead > 0) && (expectedLength == 0))
						{//первое считывание из UART
							curLen = BytesToRead;
							Read(buffer, 0, curLen);
							expectedLength = buffer[0] + 6;//извлекаем ожидаемую длину посылки
							if (curLen >= expectedLength)
							{//вся посылка считана с первого раза полностью!
								byte[] res_buf = new byte[expectedLength];
								Array.Copy(buffer, res_buf, expectedLength);
								return res_buf;
							}

						}
						else
						{//последующие считывания из UART
							if (BytesToRead > 0)
							{//считываем только если пришли новые данные
								int addLen = BytesToRead;
								if ((curLen + addLen) >= expectedLength)
								{//последнее считывание в UART нужное, либо даже большее количество данных!
									addLen = expectedLength - curLen;
									Read(buffer, curLen, addLen);
									byte[] ResultArray = new byte[expectedLength];

									Array.Copy(buffer, ResultArray, expectedLength);
									return ResultArray;//все данные считаны удачно
								}
								else
								{//дочитываем очередной пакет данных
									Read(buffer, curLen, addLen);
									curLen += addLen;
								}
							}
						}
						Thread.Sleep(interval);
						elapsedTime += interval;
					}
					if (!ShowException)
						return new byte[0];
				}
				else
				{
					return new byte[0];
				}
				return new byte[0];
			}
			catch (Exception ex)
			{
				DisplayMessage($"Ошибка отправки команды: {ex.Message}");
				//return Array.Empty<byte>();
				return new byte[0];
			}
			finally
			{
				RtsEnable = false;
				receivingMutex.ReleaseMutex();//данные отправлены и считаны, освобождаем мьютекс

			}
		}


		private void DisplayMessage(string message)
		{
			// Здесь можно использовать MessageBox, если вызывать DisplayMessage в основном потоке,
			// или сделать логирование в текстовый файл, если это фоновый поток
			MessageBox.Show(message);
		}



		#endregion



		public byte[] RxBuff
		{
			get { return _RxBuff; }
		}








		public string ConcatenateStrings(string str1, string str2, string str3)
		{
			/*
				Функция складывает три строки в одну
				автоматически определяет текущую культуру пользователя с помощью CultureInfo.CurrentCulture.
				В зависимости от культуры, если это русскоязычная культура (например, ru-RU, ru-BY), выбирается кодировка Windows-1251.
				Для всех других культур по умолчанию используется кодировка UTF-8.
			 */

			// Обработка пустых строк
			str1 = str1 ?? string.Empty;
			str2 = str2 ?? string.Empty;
			str3 = str3 ?? string.Empty;

			// Определение текущей культуры пользователя
			var currentCulture = CultureInfo.CurrentCulture;

			// Выбор кодировки в зависимости от культуры
			Encoding encoding;
			if (currentCulture.Name.StartsWith("ru")) // Русская культура (ru-RU, ru-BY, и т.д.)
			{
				encoding = Encoding.GetEncoding("Windows-1251");
			}
			else
			{
				encoding = Encoding.UTF8; // По умолчанию UTF-8
			}

			// Конкатенация строк
			string concatenatedString = str1 + str2 + str3;

			// Преобразование строки в массив байт с указанной кодировкой
			byte[] encodedBytes = encoding.GetBytes(concatenatedString);

			// Преобразование массива байт обратно в строку с той же кодировкой
			string result = encoding.GetString(encodedBytes);

			return result;
		}




		public byte[] CommSendAnsv(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, int TimeOutStartAnsv = 50)
		{//команда отправляет данные и принимает отклик, при этом заполняет Структуры хеадеров как приёма, так и ответа
		 //ВНИМАНИЕ!!!! БЕРЁТ ДЛИНУ ДАННЫХ ИЗ МАССИВА data - ЕГО ДЛИНА ДОЛЖНА БЫТЬ РАВНА ДЛИНЕ ДАННЫХ, ТАМ НЕ ДОЛЖНО БЫТЬ ЛИШНИХ ЭЛЕМЕНТОВ!!!
			RtsEnable = true;
			TxHeadCom.Comm = command;
			TxHeadCom.Rec = _RecDev;
			if (data == null)
			{
				TxHeadCom.DataLength = 0;
			}
			else
				TxHeadCom.DataLength = data.Length;
			TxHeadCom.subCom.data = 0;
			int tmpLen = 0;
			if (data != null)
				tmpLen = data.Length;
			byte[] datasend = GetCommFromHeader(data, tmpLen);//формируем посылку для отправки команды

			byte[] bytesAnsv = SendCommand(datasend, TimeOutStartAnsv, true);//отправка данных команды и приём ответа с переключением RTS

			if ((bytesAnsv.Length == 0)||(TimeOutStartAnsv==0))
				return new byte[0] ;//ответа на команду не требуется
			if (!CRC.check0CRC16(bytesAnsv, bytesAnsv.Length))
				throw new Exception("Ошибка контрольной суммы при чтении данных");
			//считывание полученных данных во внутреннюю структуру устройства
			RxHeadCom.getFromCommandArr(bytesAnsv);
			Array.Copy(bytesAnsv, 4, RxBuff, 0, bytesAnsv.Length - 6);
			return bytesAnsv;
		}


		public byte[] CommSendAnsv_NO_showException(ECommand command, Efl_DEV _RecDev = Efl_DEV.fld_none, byte[] data = null, int TimeOutStartAnsv = 50)
		{//ВНИМАНИЕ!!!! команда не вызывает исключительных ситуаций, просто возвращает пустой массив в случае исключительной ситуации
		 //команда отправляет данные и принимает отклик, при этом заполняет Структуры хеадеров как приёма, так и ответа
		 //ВНИМАНИЕ!!!! БЕРЁТ ДЛИНУ ДАННЫХ ИЗ МАССИВА data - ЕГО ДЛИНА ДОЛЖНА БЫТЬ РАВНА ДЛИНЕ ДАННЫХ, ТАМ НЕ ДОЛЖНО БЫТЬ ЛИШНИХ ЭЛЕМЕНТОВ!!!
			RtsEnable = true;
			TxHeadCom.Comm = command;
			TxHeadCom.Rec = _RecDev;
			if (data == null)
			{
				TxHeadCom.DataLength = 0;
			}
			else
				TxHeadCom.DataLength = data.Length;
			TxHeadCom.subCom.data = 0;
			int tmpLen = 0;
			if (data != null)
				tmpLen = data.Length;
			byte[] datasend = GetCommFromHeader(data, tmpLen);//формируем посылку для отправки команды

			byte[] bytesAnsv = SendCommand(datasend, TimeOutStartAnsv, false);//отправка данных команды и приём ответа с переключением RTS

			if ((bytesAnsv.Length == 0) || (TimeOutStartAnsv == 0))
				return new byte[0];//ответа на команду не требуется
			if (!CRC.check0CRC16(bytesAnsv, bytesAnsv.Length))
				throw new Exception("Ошибка контрольной суммы при чтении данных");
			//считывание полученных данных во внутреннюю структуру устройства
			RxHeadCom.getFromCommandArr(bytesAnsv);
			Array.Copy(bytesAnsv, 4, RxBuff, 0, bytesAnsv.Length - 6);
			return bytesAnsv;
		}




		public byte[] GetCommFromHeader(byte[] data, int dataLength)
		{//формируем посылку для отправки команды
			RtsEnable = true;
			TxHeadCom.DataLength = dataLength;
			byte[] tmpb = TxHeadCom.getHeaderbytes();
			Array.Copy(tmpb, Outbuf, 4);
			if (data != null)
				Array.Copy(data, 0, Outbuf, 4, TxHeadCom.DataLength);

			int _crc = CRC.CRC16(Outbuf, 0, Convert.ToUInt16(dataLength + 4));
			Outbuf[dataLength + 4] = (byte)(_crc & 0xff);
			Outbuf[dataLength + 5] = (byte)((_crc >> 8) & 0xff);
			byte[] result = new byte[dataLength + 6];
			Array.Copy(Outbuf, result, dataLength + 6);
			return result;
		}


		public int CommSendFromHeader(byte[] data, int dataLength)
		{//отправка команды на основе существующего Header
			int ret = 0;
			RtsEnable = true;
			TxHeadCom.DataLength = dataLength;
			byte[] tmpb = TxHeadCom.getHeaderbytes();
			Array.Copy(tmpb, Outbuf, 4);
			if (data != null)
				Array.Copy(data, 0, Outbuf, 4, TxHeadCom.DataLength);

			int _crc = CRC.CRC16(Outbuf, 0, Convert.ToUInt16(dataLength + 4));
			Outbuf[dataLength + 4] = (byte)(_crc & 0xff);
			Outbuf[dataLength + 5] = (byte)((_crc >> 8) & 0xff);

			ret = WriteToUART(Outbuf, dataLength + 6);
			RtsEnable = false;
			return ret;
		}


	}
}
