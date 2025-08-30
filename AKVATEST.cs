using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Globalization;
using COMMAND;
using ExtHubComm;
using FileCreater;
using UART;
using RS485;
using TFTprog;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;






namespace TESTAKVA
{

    //описание структуры таймеров

    public enum ErejTimer : uint
    {
        rejt_off = 0x00U,

        rejt_on = 0x01U,
        rejt_over = 0x02U
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SATIMER
    {
        public ErejTimer Rej;
        public uint CountSec;
        public uint DamageSec;
    }


    public enum ErejAKVA
    {
        rejak_Wait = 1,           // ожидание
        rejak_Wash = 2,           // промывка
        rejak_Fabric = 3,         // производство
        rejak_prepWash = 4,       // Промывка преподготовки СТАРТ
        rejak_newWash = 5,        // промывка новых мембран Ожидание нажатия кнопки "Да"
        rejak_Damage = 6,         // Авария
        rejak_Sanitar = 7,        // санитарная обработка
        rejak_FirstStart = 8,     // первое включение
        rejak_speedWash = 9,      // быстрая промывка
        rejak_Stop = 10,
        rejak_none = 0xff,
        rejak_test = 0x1ff,

        rejak_WaitRazd = 0x100 + rejak_Wait,
        rejak_WashRazd = 0x100 + rejak_Wash,
        rejak_FabricRazd = 0x100 + rejak_Fabric,

        rejak_prepWashSteep1 = 0x400 + 1,
        rejak_prepWashSteep2 = 0x400 + 2,
        rejak_prepWashSteep3 = 0x400 + 3,
        rejak_prepWashSteep4 = 0x400 + 4,

        rejak_prepnewWashSteep1 = 0x500 + 1,
        rejak_prepnewWashSteep2 = 0x500 + 2,

        rejak_SanitarSteep1 = 0x700 + 1,
        rejak_SanitarSteep2 = 0x700 + 2,
        rejak_SanitarSteep3 = 0x700 + 3,
        rejak_SanitarSteep4 = 0x700 + 4,
        rejak_SanitarSteep5 = 0x700 + 5,
        rejak_SanitarSteep6 = 0x700 + 6,
        rejak_SanitarSteep7 = 0x700 + 7,
        rejak_SanitarSteep8 = 0x700 + 8,

        rejak_FirstStartSteep1 = 0x800 + 1,
        rejak_FirstStatrSteep2 = 0x800 + 2,
        rejak_FirstStartSteep3 = 0x800 + 3,
        rejak_FirstStatrSteep4 = 0x800 + 4
    }




    public class SAKVApar
    {
        public ErejAKVA Rej { get; set; }
        public float[] FM { get; set; } = new float[3];
        public float[] PT { get; set; } = new float[2];
        public float[] QT { get; set; } = new float[3];

        private uint _ins;
        public uint INs
        {
            get => _ins;
            set
            {
                _ins = value;
                Unus0 = (value & 0xF);
                InONOFF = (value >> 4 & 1) != 0;
                InESC = (value >> 5 & 1) != 0;
                InPressRazd = (value >> 6 & 1) != 0;
                InFlowMeters = (value >> 7 & 0x7);
            }
        }

        public uint Unus0 { get; private set; }
        public bool InONOFF { get; private set; }
        public bool InESC { get; private set; }
        public bool InPressRazd { get; private set; }
        public uint InFlowMeters { get; private set; }
        public uint TimeStampSec { get; set; }

        // Метод для загрузки структуры из массива байт
        public void LoadFromByteArray(byte[] data)
        {
            if (data.Length != Marshal.SizeOf(typeof(SAKVAparRaw)))
                throw new ArgumentException("Размер массива не соответствует размеру структуры SAKVApar");

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var raw = (SAKVAparRaw)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SAKVAparRaw));
                LoadFromRaw(raw);
            }
            finally
            {
                handle.Free();
            }
        }

        // Метод для преобразования структуры в массив байт
        public byte[] AKVAPARtoByteArray()
        {
            var raw = ConvertToRaw();
            int size = Marshal.SizeOf(raw);
            byte[] result = new byte[size];
            var handle = GCHandle.Alloc(result, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(raw, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
                //В финале добавим извлечённое из Windows количество секунд
                long WinNowTime = TimeConverter.WindowsTimeTosec("00:00:00 01.01.2000");
                // Преобразуем long в uint
                uint uintValue = (uint)WinNowTime;

                // Получаем байты uint в формате little-endian
                byte[] uintBytes = BitConverter.GetBytes(uintValue);

                // Копируем байты uint в последние 4 байта массива result
                Array.Copy(uintBytes, 0, result, result.Length - 4, 4);

            }
            return result;
        }

        // Метод для передачи данных структуры в таблицу
        public void PutGridViewColumn(DataGridView paramGridView, int columnIndex)
        {
            if (paramGridView == null || columnIndex < 0 || columnIndex >= paramGridView.ColumnCount)
                throw new ArgumentException("Invalid DataGridView or column index.");

            var values = new List<string>
        {
            FM[0].ToString(CultureInfo.InvariantCulture),
            FM[1].ToString(CultureInfo.InvariantCulture),
            FM[2].ToString(CultureInfo.InvariantCulture),
            PT[0].ToString(CultureInfo.InvariantCulture),
            PT[1].ToString(CultureInfo.InvariantCulture),
            QT[0].ToString(CultureInfo.InvariantCulture),
            QT[1].ToString(CultureInfo.InvariantCulture),
            QT[2].ToString(CultureInfo.InvariantCulture),
            InONOFF ? "1" : "0",
            InESC ? "1" : "0",
            InPressRazd ? "1" : "0",
            InFlowMeters.ToString()
        };

            if (paramGridView.RowCount < values.Count)
                paramGridView.RowCount = values.Count;

            for (int i = 0; i < values.Count; i++)
            {
                paramGridView.Rows[i].Cells[columnIndex].Value = values[i];
            }
        }

        // Метод для загрузки данных структуры из таблицы
        public void LoadFromDataGridViewColumn(DataGridView paramGridView, int columnIndex)
        {
            // старая версия ограничений         if (paramGridView == null || columnIndex < 0 || columnIndex >= paramGridView.ColumnCount)
            if ((paramGridView == null) || (columnIndex < 0) || ((columnIndex > 4) && (columnIndex != 8)))
                throw new ArgumentException("Invalid DataGridView or column index.");

            var values = new List<string>();
            for (int i = 0; i < paramGridView.RowCount; i++)
            {
                var cellValue = paramGridView.Rows[i].Cells[columnIndex].Value?.ToString() ?? string.Empty;
                values.Add(cellValue);
            }

            Rej = GetErejAKVA(columnIndex);

            FM[0] = float.Parse(values[0], CultureInfo.InvariantCulture);
            FM[1] = float.Parse(values[1], CultureInfo.InvariantCulture);
            FM[2] = float.Parse(values[2], CultureInfo.InvariantCulture);
            PT[0] = float.Parse(values[3], CultureInfo.InvariantCulture);
            PT[1] = float.Parse(values[4], CultureInfo.InvariantCulture);
            QT[0] = float.Parse(values[5], CultureInfo.InvariantCulture);
            QT[1] = float.Parse(values[6], CultureInfo.InvariantCulture);
            QT[2] = float.Parse(values[7], CultureInfo.InvariantCulture);
            INs = (uint)((values[8] == "1" ? 1U << 4 : 0U) |
                         (values[9] == "1" ? 1U << 5 : 0U) |
                         (values[10] == "1" ? 1U << 6 : 0U) |
                         (uint.Parse(values[11]) << 7));
        }

        private void LoadFromRaw(SAKVAparRaw raw)
        {
            Rej = (ErejAKVA)raw.Rej;
            FM = raw.FM;
            PT = raw.PT;
            QT = raw.QT;
            INs = raw.INs;
            TimeStampSec = raw.TimeStampSec;
        }

        public  int GetSelectedColumnIndex(DataGridView dgv)
        { //определения столбца таблицы с выделенной ячейкой
            if (dgv == null)
                return -1;

            if (dgv.SelectedCells.Count == 0)
            {
                MessageBox.Show("Выделите ячейку в столбце устанавливаемого таймера",
                                "Нет выделения",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return -1;
            }

            // Возвращаем номер столбца первой выделенной ячейки
            return dgv.SelectedCells[0].ColumnIndex;
        }
        //***********************************************************************************************
        // Метод преобразования ErejAKVA в номер выделенного столбца ему соответствующее
        public  int GetNumVol(ErejAKVA rej)
        {
            var values = Enum.GetValues(typeof(ErejAKVA));
            int index = 0;
            foreach (var value in values)
            {
                if ((ErejAKVA)value == rej)
                {
                    return index;
                }
                index++;
            }
            throw new ArgumentException($"Value {rej} not found in enumeration.");
        }


        // Метод преобразования номера выделенного столбца в ErejAKVA ему соотуетствующее
        public  ErejAKVA GetErejAKVA(int num)
        {
            var values = Enum.GetValues(typeof(ErejAKVA));
            if (num >= 0 && num < values.Length)
            {
                return (ErejAKVA)values.GetValue(num);
            }
            throw new ArgumentOutOfRangeException($"Number {num} is out of range for ErejAKVA.");
        }

        // Преобразование массива байтов в значение перечисления ErejAKVA
        public  ErejAKVA ByteArrayToEnum(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length != 4)
            {
                throw new ArgumentException("Массив должен содержать ровно 4 байта.");
            }

            // Конвертируем массив байтов в 32-битное целое число
            int value = BitConverter.ToInt32(byteArray, 0);

            // Проверяем, существует ли такое значение в перечислении
            if (Enum.IsDefined(typeof(ErejAKVA), value))
            {
                return (ErejAKVA)value;
            }

            throw new ArgumentException("Значение массива не соответствует допустимому значению ErejAKVA.");
        }

        // Преобразование значения перечисления ErejAKVA в массив байтов
        public  byte[] EnumToByteArray(ErejAKVA enumValue)
        {
            // Преобразуем значение перечисления в 32-битное целое число
            int value = (int)enumValue;

            // Конвертируем число в массив из 4 байтов
            return BitConverter.GetBytes(value);
        }



        private SAKVAparRaw ConvertToRaw()
        {
            return new SAKVAparRaw
            {
                Rej = (int)Rej,
                FM = FM,
                PT = PT,
                QT = QT,
                INs = INs,
                TimeStampSec = TimeStampSec
            };
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SAKVAparRaw
        {
            public int Rej;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] FM;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public float[] PT;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] QT;
            public uint INs;
            public uint TimeStampSec;
        }
    }






    public struct GridViewParams
    {
        public int numCol { get; set; }//общее количество колонок в таблице
        public int numRow { get; set; }//общее количество строк в таблице
        public int X { get; set; }//координата X левого верхнего угла
        public int Y { get; set; }//координата Y левого верхнего угла
        public int Width { get; set; }//ширина всех колонок, кроме левой
        public int Height { get; set; }//высота всех строк
        public int L1 { get; set; }//номер колонки которая отображается следующей за зафиксированной нулевой
        public int WidthLeftCol { get; set; }//ширина крайне левой колонки
        public int ViewCol { get; set; }//количество колонок, которые должны умещаться в промежутке видимой зоны от крайне левой зафиксированной колонки до крайне правой
    }






    public class SWORKAKVATEST : SCommStruct
    {
        public DataGridView TimersGridView;
        public DataGridView ParamGridView;
        public int AKVAint = 0;// номер столбца таблицы соответствующий текущему значению режима работы устройства
        public int NewAKVAint = -1;//номер столбца таблицы соответствующий новому значению режима работы устройства
        public int HandlAKVAchange = 0;//исходный режим
        public bool isUpdating = false; // Флаг для предотвращения самоблокировки
        public ErejAKVA selectedMode = ErejAKVA.rejak_Stop;//Выбранный режим работы
        public SAKVApar AKVApar;

        public SWORKAKVATEST()
        {
            AKVApar = new SAKVApar();
        }
       

        public void SelTableRowHead(DataGridView Grid, int numCol, string allowedCols)
        {//метод выделения заголовка выделенного столбца
            var allowed = allowedCols.Split(',').Select(s => int.Parse(s.Trim())).ToArray();
            int numPAR = numCol + 1;
            if (!allowed.Contains(numPAR))
            {
                MessageBox.Show($"Столбец {numCol} не входит в список допустимых: {allowedCols}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Grid == null || numCol < 0 || numCol >= Grid.Columns.Count)
            {
                MessageBox.Show("Недопустимый номер столбца.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Grid.EnableHeadersVisualStyles = false;
            var def = Grid.ColumnHeadersDefaultCellStyle;

            for (int i = 0; i < Grid.Columns.Count; i++)
            {
                var hc = Grid.Columns[i].HeaderCell;
                if (i == numCol)
                {
                    hc.Style.BackColor = Color.Blue;
                    hc.Style.ForeColor = Color.White;
                }
                else
                {
                    hc.Style.BackColor = def.BackColor;
                    hc.Style.ForeColor = def.ForeColor;
                    hc.Style.Font = def.Font;
                    hc.Style.SelectionBackColor = def.SelectionBackColor;
                    hc.Style.SelectionForeColor = def.SelectionForeColor;
                    hc.Style.Alignment = def.Alignment;
                    hc.Style.WrapMode = def.WrapMode;
                }
            }
        }




        public SWORKAKVATEST(DataGridView _TimersGridView, DataGridView _ParamGridView)
        {//конструктор в котором происходит формирование таблиц таймеров и параметров
            TimersGridView = _TimersGridView ?? throw new ArgumentNullException(nameof(_TimersGridView));

            ParamGridView = _ParamGridView ?? throw new ArgumentNullException(nameof(_ParamGridView));
            FormatParamsGridView();//установка параметров таблицы параметров
            DrawAKVAtable(ParamGridView, FormatParamsGV);//установка основных параметров таблицы и заполнение значений нулями
            ParamSetHeaders();//форматирование таблицы параметров
            //форматирование таблицы таймеров
            FormatTimersGridView(120, 30, new string[] { "Rej", "CountSec", "LastStamp_mSec", "MaxCountSec", "DamageSec" }, GetTextHead(0, 7));
            TimersParPerminEdit();




            TimersGridView.CellMouseDoubleClick += (s, e) =>
            {//подключаемся к событию двойного клика - событие изменения состояния таймера on/off
                // Проверяем, что двойной клик произошел в области первой строки
                if (e.RowIndex == 0 && e.ColumnIndex >= 0)
                {
                    // Определяем номер колонки
                    int NumCol = e.ColumnIndex;
                    int value;
    //                return int.TryParse(text, out value);

                    // Получаем текст ячейки, на которую был произведён двойной клик
                    string text = TimersGridView.Rows[e.RowIndex].Cells[NumCol].Value?.ToString();

                    // Проверяем текст и при необходимости изменяем его


                    if ((text == "rejt_over")|| (text == "rejt_off") )
                    { 
                        TimersGridView.Rows[1].Cells[NumCol].Value = "0";
                        TimersGridView.Rows[e.RowIndex].Cells[NumCol].Value = "rejt_on";
                        return;
                    }
                    else
                    {
                        if (text == "rejt_on" || (text == "") || (text == null))
                        {
                            TimersGridView.Rows[e.RowIndex].Cells[NumCol].Value = "rejt_over";
                            return;
                        }
                    }

                    return;



   //видимо необходимо вместо булевой переменной вносить тип возвращаемой посылке 
                    boolChangeTimersVol = READoneTimerFromDataGridView(ref TIMS[NumCol],NumCol);
                    if (boolChangeTimersVol)
                    {
                        xChangeTimersVol = NumCol;
                        boolChangeTimersVol = false;
                    }
                        

                }
            };


            // Подключаемся к событию окончания редактирования
            TimersGridView.CellEndEdit += (s, e) =>
            {
                try
                {
                    // Получаем значение отредактированной ячейки
                    var cellValue = TimersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    // Пытаемся преобразовать значение в uint
                    if (uint.TryParse(cellValue, out uint parsedValue))
                    {
                        // Если получилось, присваиваем значение переменной ChangeTimersVol
                        int NumCol = e.ColumnIndex;

                        return;

                         boolChangeTimersVol = READoneTimerFromDataGridView(ref TIMS[NumCol],NumCol);//попытка обновить значения таймера введёнными данными
                        if (boolChangeTimersVol)
                        {
                            xChangeTimersVol = NumCol;
                            boolChangeTimersVol = false;
                        }
                        // Запоминаем координаты изменённой вручную ячейки

                    }

                }
                catch (Exception ex)
                {
                    // Обработка исключения (например, вывод сообщения об ошибке)
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            floatParamsGridView = new float[rejheaders.Length, rowHeaders.Length] ;
        }

        public GridViewParams FormatParamsGV;//структура хранения параметров форматирования таблицы ParamGridView
        public SATIMER[] TIMS = new SATIMER[ArraySize];





        public  string[] rejheaders = { "Wait", "Wash", "Fabric", "prepWash", "newWash", "Damage", "Sanitar", "FirstStart", "speedWash", "Stop",
                         "WaitRazd", "WashRazd", "FabricRazd", "prepWashSteep1", "prepWashSteep2", "prepWashSteep3", "prepWashSteep4",
                         "newWashSteep1", "newWashSteep2", "SanitarSteep1", "SanitarSteep2", "SanitarSteep3", "SanitarSteep4", "SanitarSteep5", "SanitarSteep6",
                         "SanitarSteep7", "SanitarSteep8", "FirstStartSteep1", "FirstStartSteep2", "FirstStartSteep3", "FirstStartSteep4" };

        public  string[] rowHeaders = { "FM[0]", "FM[1]", "FM[2]", "PT[0]", "PT[1]", "QT[0]", "QT[1]", "QT[2]",
                            "InONOFF", "InESC", "InPressRazd", "InFlowMeters" };

        public float[,] floatParamsGridView;


        public void SetNewRej(int ColumnIndex)
        {
            // Проходим по всем ячейкам таблицы
            foreach (DataGridViewRow row in ParamGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // Если ячейка в том же столбце, что и заголовок, на который кликнули
                    if (cell.ColumnIndex == ColumnIndex)
                    {
                        // Устанавливаем цвет на DefaultCellStyle.BackColor
                        cell.Style.BackColor = ParamGridView.DefaultCellStyle.BackColor;
                    }
                    else
                    {
                        // Устанавливаем цвет на LightGray
                        cell.Style.BackColor = Color.LightGray;
                    }
                }
            }
            NewAKVAint = ColumnIndex + 1;//выбран новый режим в таблице параметров
        }


        #region Timers
        public bool boolChangeTimersVol = false;//вручную изменили параметры работы таймера
        public int xChangeTimersVol = -1;
        private const int ArraySize = 8;

        private byte[] tmpbufTimersData = new byte[Marshal.SizeOf(typeof(SATIMER)) * ArraySize];//массив в который копируются считанные данные таймеров

        //  преобразование СТРУКТУРЫ SATIMER в МАССИВ байт
        public  byte[] SATIMER_ToBytes(SATIMER oneTIM)
        {
            int size = Marshal.SizeOf(typeof(SATIMER));
            byte[] buf = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(oneTIM, ptr, false);
                Marshal.Copy(ptr, buf, 0, size);
                return buf;
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        //  преобразование  МАССИВА байт в СТРУКТУРУ SATIMER
        public  SATIMER Bytes_ToSATIMER(byte[] data)
        {
            int size = Marshal.SizeOf(typeof(SATIMER));
            if (data == null || data.Length != size)
                throw new ArgumentException("Invalid data size");
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(data, 0, ptr, size);
                return (SATIMER)Marshal.PtrToStructure(ptr, typeof(SATIMER));
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        //  преобразование МАССИВА структур SATIMER в МАССИВ байт
        public  byte[] SATIMER_ArrayToBytes(SATIMER[] arrTIM)
        {
            int itemSize = Marshal.SizeOf(typeof(SATIMER));
            int total = itemSize * arrTIM.Length;
            var buf = new byte[total];
            IntPtr basePtr = Marshal.AllocHGlobal(total);
            try
            {
                for (int i = 0; i < arrTIM.Length; i++)
                    Marshal.StructureToPtr(arrTIM[i], IntPtr.Add(basePtr, i * itemSize), false);
                Marshal.Copy(basePtr, buf, 0, total);
                return buf;
            }
            finally { Marshal.FreeHGlobal(basePtr); }
        }

        //  преобразование МАССИВА байт в МАССИВ структур SATIMER
        public void Bytes_ToSATIMER_Array(byte[] data, SATIMER[] arrTIM)
        {
            int itemSize = Marshal.SizeOf(typeof(SATIMER));
            int expected = itemSize * arrTIM.Length;
            IntPtr basePtr = Marshal.AllocHGlobal(expected);
            try
            {
                Marshal.Copy(data, 0, basePtr, expected);
                for (int i = 0; i < arrTIM.Length; i++)
                    arrTIM[i] = (SATIMER)Marshal.PtrToStructure(IntPtr.Add(basePtr, i * itemSize), typeof(SATIMER));
            }
            finally { Marshal.FreeHGlobal(basePtr); }
        }

    public void DisplayInTimersGridView()
        {

            // Заполняем таблицу данными из массива таймеров SATIMER[]
            for (int i = 0; i < ArraySize; i++)
            {
                // Проверяем каждую ячейку перед записью
                UpdateCellIfChanged(TimersGridView.Rows[0].Cells[i], TIMS[i].Rej);
                UpdateCellIfChanged(TimersGridView.Rows[1].Cells[i], TIMS[i].CountSec);
                //              UpdateCellIfChanged(TimersGridView.Rows[2].Cells[i], T[i].LastStamp_mSec);
                //              UpdateCellIfChanged(TimersGridView.Rows[3].Cells[i], T[i].MaxCountSec);
                UpdateCellIfChanged(TimersGridView.Rows[4].Cells[i], TIMS[i].DamageSec);
            }
        }

        public void ReadTIMsendPAR(DataGridView paramGridView)
        {

            // извлечение данных структуры AKVAPAR из таблицы и отправка в TFT контроллер
            AKVApar.LoadFromDataGridViewColumn(paramGridView, HandlAKVAchange);// извлекаем  данные из таблицы в структуру AKVAPAR 
            byte[] arrAKVAPAR = AKVApar.AKVAPARtoByteArray();//копируем данные в массив

            HandlAKVAchange = -1;
            //отправляем структуру в TFT контроллер без требования ответа
            CommSendAnsv(ECommand.cmd_RdWrSens, Efl_DEV.fld_MainBoard, arrAKVAPAR, 0);


            //считываем данные о таймерах из TFT контроллера путём отсылки команды без данных
            byte[] comm_data = CommSendAnsv(ECommand.cmd_ExhParams, Efl_DEV.fld_TFTboard, null, 200);
            //записываем считанные байты в массив таймеров

            byte[] byteArray = new byte[comm_data.Length - 6];
            Array.Copy(comm_data, 4, byteArray, 0, byteArray.Length);


            Bytes_ToSATIMER_Array(byteArray, TIMS);
            //загружаем параметры таймеров в таблицу и отображаем её
            DisplayInTimersGridView();
        }


        private void TimersParPerminEdit()
        {//разрешает редактировать только две строки - номер 1(Count) и 4(Damage) содержащие текущее значением таймеров
            int editableRowIndex = 1; // Номер строки, которая остаётся редактируемой
            int editableRowIndex2 = 4; // Номер строки, которая остаётся редактируемой
            foreach (DataGridViewRow row in TimersGridView.Rows)
            {
                if ((row.Index != editableRowIndex) && (row.Index != editableRowIndex2))
                {
                    // Запрет редактирования
                    row.ReadOnly = true;

                    // Установка серого фона для нередактируемых строк
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    // Разрешение редактирования для одной строки
                    row.ReadOnly = false;

                    // Сброс цвета для редактируемой строки (по умолчанию)
                    row.DefaultCellStyle.BackColor = TimersGridView.DefaultCellStyle.BackColor;
                }
            }
        }

        public void UpdateCellIfChanged(DataGridViewCell cell, object newValue)
        {
            // Проверяем, что ячейка нередактируемая и новое значение отличается от текущего
            if (/*!cell.ReadOnly &&*/ !Equals(cell.Value, newValue))
            {
                cell.Value = newValue;
            }
        }






        public bool READoneTimerFromDataGridView(ref SATIMER TIM, int TimerNum)
        {//считывание данных об одном таймере из соответствующего столбца таблицы
            // Проверка выхода за пределы массива
            if (TimerNum >= TIMS.Length)
                return false;

            // Создание временного массива для проверки корректности данных
            SATIMER Ttmp = new SATIMER();

                // Попытка преобразовать данные во временный массив
                try
                {
                    Ttmp.CountSec = uint.Parse(TimersGridView.Rows[1].Cells[TimerNum].Value.ToString());
                    Ttmp.DamageSec = uint.Parse(TimersGridView.Rows[4].Cells[TimerNum].Value.ToString());

                }
                catch 
                {
                    // Обработка исключения (например, вывод сообщения об ошибке)
                    MessageBox.Show("введите корректные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false; // Ошибка преобразования
            }

                try
                {
                    Ttmp.Rej = (ErejTimer)Enum.Parse(typeof(ErejTimer), TimersGridView.Rows[0].Cells[TimerNum].Value.ToString());
                }
                catch //(FormatException)
                {
                    if (Ttmp.CountSec>= Ttmp.DamageSec)
                    Ttmp.Rej = ErejTimer.rejt_over;
                }
               


                // Если все преобразования прошли успешно, копируем данные в структуру таймера
                TIM = Ttmp;
                return true; // Успешное преобразование

        }

        public void UpdateAllTimersFromDataGridView()
        {// считывание всех данных из таблицы
            for (int i = 0; i < ArraySize; i++)
            {
                READoneTimerFromDataGridView(ref TIMS[i], i);
            }
        }

        public  byte[] OneTimerToBytes<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T)); // Получаем размер структуры в байтах
            byte[] buffer = new byte[size]; // Создаем массив байт для хранения данных

            IntPtr ptr = Marshal.AllocHGlobal(size); // Выделяем память в неуправляемой области
            try
            {
                Marshal.StructureToPtr(structure, ptr, false); // Копируем данные из структуры в неуправляемую память
                Marshal.Copy(ptr, buffer, 0, size); // Копируем данные из неуправляемой памяти в массив байт
            }
            finally
            {
                Marshal.FreeHGlobal(ptr); // Освобождаем неуправляемую память
            }

            return buffer;
        }


        #endregion



        #region FormatTIMERtible
        //ТАБЛИЦА ПАРАМЕТРОВ ТАЙМЕРОВ

        //выдаёт последовательность строк соедержащих числа увеличивающиеся на 1 для формирования заголовков
        public string[] GetTextHead(int Start, int End)
        {
            // Вычисляем количество элементов в массиве
            int length = End - Start + 1;

            // Создаем массив строк
            string[] result = new string[length];

            // Заполняем массив числами, преобразованными в строки
            for (int i = 0; i < length; i++)
            {
                result[i] = (Start + i).ToString();
            }

            return result;
        }


        //      Функция форматёр таблицы не содержащей полосы прокрутки под  заголовки
        //      в виде набора строк вводятся верхний заголовок и нижний заголовок

        public void FormatTimersGridView(int rowHeaderWidth, int columnHeaderHeight, string[] rowHeaders, string[] columnHeaders)
        {
            TimersGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            int rowCount = rowHeaders.Length;
            int columnCount = columnHeaders.Length;

            // Настройка DataGridView
            TimersGridView.ScrollBars = ScrollBars.None; // Отключение полос прокрутки
            TimersGridView.AllowUserToAddRows = false;  // Отключение возможности добавления новой строки

            // Очистка столбцов и строк
            TimersGridView.Columns.Clear();
            TimersGridView.Rows.Clear();

            // Установка размеров заголовков с учетом остатков
            int totalWidth = TimersGridView.ClientSize.Width;
            int totalHeight = TimersGridView.ClientSize.Height;

            // Вычисление остатков
            int widthRemainder = (totalWidth - rowHeaderWidth) % columnCount;
            int heightRemainder = (totalHeight - columnHeaderHeight) % rowCount;

            // Увеличение размеров заголовков на остатки
            TimersGridView.RowHeadersWidth = rowHeaderWidth + widthRemainder;
            TimersGridView.ColumnHeadersHeight = columnHeaderHeight + heightRemainder;

            // Установка столбцов
            for (int i = 0; i < columnCount; i++)
            {
                TimersGridView.Columns.Add($"C{i}", columnHeaders[i]);
            }

            // Добавление строк и установка заголовков строк
            for (int i = 0; i < rowCount; i++)
            {
                TimersGridView.Rows.Add();
                TimersGridView.Rows[i].HeaderCell.Value = rowHeaders[i];
            }

            // Вычисление ширины столбцов и высоты строк
            int columnWidth = (totalWidth - TimersGridView.RowHeadersWidth) / columnCount;
            int rowHeight = (totalHeight - TimersGridView.ColumnHeadersHeight) / rowCount;

            // Установка ширины столбцов
            foreach (DataGridViewColumn column in TimersGridView.Columns)
            {
                column.Width = columnWidth;
            }

            // Установка высоты строк
            foreach (DataGridViewRow row in TimersGridView.Rows)
            {
                row.Height = rowHeight;
            }

            // Включение отображения заголовков строк
            TimersGridView.RowHeadersVisible = true;

            foreach (DataGridViewColumn column in TimersGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }


        }


        #endregion


        #region DateTime
        // Константы для расчётов
        private const int SecondsInMinute = 60;
        private const int SecondsInHour = 3600;
        private const int SecondsInDay = 86400;
        private static readonly int[] DaysInMonthNormal = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int[] DaysInMonthLeap = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        // Проверка, является ли год високосным
        public static bool IsLeapYear(int year)
        {
            return (year % 400 == 0) || (year % 4 == 0 && year % 100 != 0);
        }

        // Функция для преобразования системного времени в количество секунд с 01.01.2000
        public static uint DateTimeToSeconds(DateTime dt)
        {
            uint seconds = 0;

            // Добавляем секунды за полные годы
            for (int year = 2000; year < dt.Year; year++)
            {
                seconds += (uint)(IsLeapYear(year) ? 366 : 365) * SecondsInDay;
            }

            // Добавляем секунды за полные месяцы текущего года
            int[] daysInMonth = IsLeapYear(dt.Year) ? DaysInMonthLeap : DaysInMonthNormal;
            for (int month = 1; month < dt.Month; month++)
            {
                seconds += (uint)daysInMonth[month - 1] * SecondsInDay;
            }

            // Добавляем секунды за полные дни
            seconds += (uint)(dt.Day - 1) * SecondsInDay;

            // Добавляем часы, минуты и секунды
            seconds += (uint)dt.Hour * SecondsInHour;
            seconds += (uint)dt.Minute * SecondsInMinute;
            seconds += (uint)dt.Second;

            return seconds;
        }

        // Функция для получения количества секунд с 01.01.2000 на основе строки
        public static uint StringToSeconds(string dateTimeString)
        {
            // Пытаемся распарсить строку в формате "HH:mm dd.MM.yyyy"
            DateTime parsedDateTime = DateTime.ParseExact(dateTimeString, "HH:mm dd.MM.yyyy", CultureInfo.InvariantCulture);
            return DateTimeToSeconds(parsedDateTime);
        }

        // Функция для получения количества секунд с текущего времени Windows
        public static uint GetSystemTimeInSeconds()
        {
            // Получаем текущее системное время
            DateTime now = DateTime.Now;

            // Преобразуем текущее время в количество секунд с 01.01.2000
            return DateTimeToSeconds(now);
        }

        static void TEST_DTconv()
        {
            // Пример использования: строка "11:45 13.10.2024"
            string dateTimeString = "11:45 13.10.2024";
            uint secFromString = StringToSeconds(dateTimeString);
            Console.WriteLine($"Секунды с 01.01.2000 для строки \"{dateTimeString}\": " + secFromString);

            // Получаем количество секунд с 01.01.2000 для текущего времени Windows
            uint sec = GetSystemTimeInSeconds();
            Console.WriteLine("Секунды с 01.01.2000 для текущего времени: " + sec);
        }

        #endregion



        #region FORMATparamsScrollAKVAtable

        

        public void FormatParamsGridView()
        {//форматируем таблицу структуры GVstruct

            FormatParamsGV.numRow = rowHeaders.Length; //общее количество колонок в таблице на одну больше чем количество режимов из за крайне левого столбца
            FormatParamsGV.numCol = rejheaders.Length; //общее количество строк в таблице
            FormatParamsGV.L1 = 1;//сразу после инициализации номер колонки которая отображается следующей за зафиксированной нулевой
            FormatParamsGV.WidthLeftCol = 75;  // //ширина крайне левой колонки
            FormatParamsGV.ViewCol = 7;           //количество колонок, которые должны умещаться в промежутке видимой зоны от крайне левой зафиксированной колонки до крайне правой
            // Чтение текущих размеров компонента в структуру (ширина и высота)
            FormatParamsGV.Width = ParamGridView.Size.Width;
            FormatParamsGV.Height = ParamGridView.Size.Height;
        }

        public void ParamSetHeaders()
        {//формирование таблицы параметров
            //выравнивание текстов верхнего заголовка по центру
            ParamGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // Установка ширины левого заголовка (RowHeadersWidth)
            ParamGridView.RowHeadersWidth = FormatParamsGV.WidthLeftCol;

            // Установка заголовков строк (левый хеадер)
            for (int i = 0; i < rowHeaders.Length; i++)
            {
                if (i < ParamGridView.Rows.Count)
                {
                    ParamGridView.Rows[i].HeaderCell.Value = rowHeaders[i];
                }
                else
                {
                    // Если строк недостаточно, добавляем строки
                    ParamGridView.Rows.Add();
                    ParamGridView.Rows[i].HeaderCell.Value = rowHeaders[i];
                }
            }

            // Расчёт ширины каждого столбца
            int totalWidth = ParamGridView.ClientSize.Width; // Общая ширина видимой зоны
            int columnSeparatorWidth = ParamGridView.Columns[0].DividerWidth; // Ширина разделителя столбцов

            // Вычисляем доступное место для колонок, вычитая ширину левого заголовка и разделители столбцов
            int availableWidth = totalWidth - ParamGridView.RowHeadersWidth - (columnSeparatorWidth * FormatParamsGV.ViewCol);

            // Рассчитываем ширину одной колонки
            int columnWidth = availableWidth / FormatParamsGV.ViewCol;

            // Установка заголовков столбцов (верхний хеадер)
            ParamGridView.ColumnCount = rejheaders.Length;
            for (int i = 0; i < rejheaders.Length; i++)
            {
                ParamGridView.Columns[i].HeaderText = rejheaders[i];
                ParamGridView.Columns[i].Width = columnWidth; // Устанавливаем ширину для  столбцов
            }
 
            // Отключение автосортировки для всех столбцов
            foreach (DataGridViewColumn column in ParamGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Установка фона всех ячеек таблицы в Color.LightGray
            foreach (DataGridViewRow row in ParamGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.LightGray;
                }
            }
        }


        public void AdjustRowHeights(DataGridView grid)
        {//подстройка высоты строк для их корректного отображения
            // Убедимся, что у нас есть необходимость в горизонтальном скролле
            bool horizontalScrollRequired = grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) > grid.ClientSize.Width;

            // Устанавливаем ширину столбцов, если это необходимо
            if (horizontalScrollRequired)
            {
                grid.HorizontalScrollingOffset = 0; // Обновляем положение скролла, если требуется
            }

            // Получаем высоту видимой области таблицы (включая скроллбар)
            int visibleHeight = grid.ClientSize.Height - grid.ColumnHeadersHeight;

            // Проверяем, виден ли горизонтальный скроллбар, используя Controls

            // Получаем высоту горизонтального скроллбара, если он виден
            visibleHeight -= SystemInformation.HorizontalScrollBarHeight;


            // Количество строк
            int rowCount = grid.RowCount;

            // Учитываем высоту разделителей между строками
            int totalDividerHeight = (rowCount - 1);// * grid.RowTemplate.DividerHeight;

            // Вычитаем высоту всех разделителей из общей высоты
            visibleHeight -= totalDividerHeight;

            // Вычисляем базовую высоту строки
            int baseRowHeight = visibleHeight / rowCount;

            // Вычисляем остаток высоты, который нужно добавить к первой строке
            int extraHeight = visibleHeight % rowCount;

            // Устанавливаем высоту для всех строк одинаково
            for (int i = 0; i < rowCount; i++)
            {
                grid.Rows[i].Height = baseRowHeight;
            }

            // Прибавляем остаток к высоте верхнего заголовка
            if (rowCount > 0)
            {

                grid.ColumnHeadersHeight += extraHeight;
            }

            // Перерисовка DataGridView для применения изменений
            grid.ScrollBars = ScrollBars.Horizontal;
            grid.Refresh();
        }







        public GridViewParams DrawAKVAtable(DataGridView AKVAparGridView, GridViewParams gridParams)
        {
            // Устанавливаем локальную переменную равную текущему количеству строк в таблице
            int Numrow = rowHeaders.Length;

            // Проверяем количество столбцов и корректируем, если необходимо
            if (AKVAparGridView.ColumnCount < gridParams.numCol)
            {
                // Добавляем недостающие столбцы
                for (int i = AKVAparGridView.ColumnCount; i < gridParams.numCol; i++)
                {
                    AKVAparGridView.Columns.Add($"Column{i}", $"Header {i}");
                }
            }
            else if (AKVAparGridView.ColumnCount > gridParams.numCol)
            {
                // Удаляем лишние столбцы
                for (int i = AKVAparGridView.ColumnCount - 1; i >= gridParams.numCol; i--)
                {
                    AKVAparGridView.Columns.RemoveAt(i);
                }
            }

            // Разрешаем отображение полосы прокрутки
            AKVAparGridView.ScrollBars = ScrollBars.Horizontal;

            // Устанавливаем количество строк
            if (AKVAparGridView.RowCount != Numrow)
            {
                AKVAparGridView.RowCount = Numrow;
            }

            // Устанавливаем содержимое заголовков строк
            for (int i = 0; i < Math.Min(Numrow, rowHeaders.Length); i++)
            {
                AKVAparGridView.Rows[i].HeaderCell.Value = rowHeaders[i];
            }

            // Заполняем остальные ячейки "0"
            for (int row = 0; row < AKVAparGridView.RowCount; row++)
            {
                for (int col = 0; col < AKVAparGridView.ColumnCount; col++)
                {
                    AKVAparGridView.Rows[row].Cells[col].Value = "0";
                }
            }

            // Сделать редактируемыми все ячейки, кроме первого столбца и первой строки
            for (int row = 0; row < AKVAparGridView.RowCount; row++)
            {
                for (int col = 0; col < AKVAparGridView.ColumnCount; col++)
                {
                    AKVAparGridView.Rows[row].Cells[col].ReadOnly = false;
                }
            }

            AdjustRowHeights(AKVAparGridView);

            return gridParams;
        }


        // Метод для выделения колонки по индексу
        public void SelectColumn(DataGridView DGV, int columnIndex)
        {

            // Очистим все предыдущие выделения
            DGV.ClearSelection();

            // Убедимся, что индекс колонки допустимый
            if (columnIndex >= 0 && columnIndex < DGV.Columns.Count)
            {
                // Получим прямоугольник колонки для проверки её видимости
                Rectangle columnRect = DGV.GetColumnDisplayRectangle(columnIndex, true);

                // Если ширина прямоугольника равна 0, это значит, что колонка скрыта
                if (columnRect.Width == 0)
                {
                    // Если колонка скрыта, установим прокрутку на эту колонку
                    DGV.FirstDisplayedScrollingColumnIndex = columnIndex;
                }
                else
                {
                    // Проверка, находится ли колонка в видимой области
                    if (columnRect.Right > DGV.DisplayRectangle.Right || columnRect.Left < 0)
                    {
                        // Если колонка не видима, прокручиваем к ней
                        DGV.FirstDisplayedScrollingColumnIndex = columnIndex;
                    }
                }

                // Программно выделим всю колонку вместе с заголовком
                DGV.Columns[columnIndex].Selected = true;
            }
            else
            {
                MessageBox.Show("Неправильный индекс колонки.");
            }
        }

        #endregion

        #region WORKwithScrollAKVAtable







public void saveAKVAparTable(string fileName, DataGridView AKVAparGridView)
        {//сохранение таблицы в файле
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Сохраняем заголовки столбцов
                string[] headers = AKVAparGridView.Columns.Cast<DataGridViewColumn>()
                                       .Select(column => column.HeaderText).ToArray();
                writer.WriteLine(string.Join(";", headers));

                // Сохраняем строки таблицы
                foreach (DataGridViewRow row in AKVAparGridView.Rows)
                {
                    if (!row.IsNewRow) // Пропустить новую строку
                    {
                        string[] cells = row.Cells.Cast<DataGridViewCell>()
                                          .Select(cell => cell.Value?.ToString() ?? "").ToArray();
                        writer.WriteLine(string.Join(";", cells));
                    }
                }
            }
        }

        //Загрузка таблицы из файла
        public void LoadAKVAparTable(string fileName, DataGridView AKVAparGridView)
        {
            using (System.IO.StreamReader reader = new StreamReader(fileName))
            {
                // Чтение заголовков столбцов
                string[] headers = reader.ReadLine().Split(';');
                AKVAparGridView.ColumnCount = headers.Length;

                for (int i = 0; i < headers.Length; i++)
                {
                    AKVAparGridView.Columns[i].HeaderText = headers[i];
                }

                // Чтение строк
                AKVAparGridView.Rows.Clear();
                while (!reader.EndOfStream)
                {
                    string[] cells = reader.ReadLine().Split(';');
                    AKVAparGridView.Rows.Add(cells);
                }
            }
        }

        #endregion


            // Метод для считывания столбца в массив float
            public  float[] GetColumnValues(DataGridView gridView, int column)
            {
                if (column < 0 || column >= gridView.ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(column), "Указан недопустимый номер столбца.");

                float[] result = new float[gridView.RowCount];
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    var cellValue = gridView[column, i].Value?.ToString()?.Trim();
                    try
                    {
                        result[i] = string.IsNullOrEmpty(cellValue)
                            ? 0
                            : float.Parse(cellValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        string rowHeader = gridView.Rows[i].HeaderCell.Value?.ToString() ?? $"Row {i + 1}";
                        string columnHeader = gridView.Columns[column].HeaderText ?? $"Column {column + 1}";
                        throw new FormatException($"Не удалось конвертировать значение '{cellValue}' в ячейке ({rowHeader}, {columnHeader}).");
                    }
                }
                return result;
            }

        // Метод для считывания всей таблицы в массив float[][]
        public float[][] GetTableValues(DataGridView gridView)
        {
            float[][] result = new float[gridView.RowCount][];
            for (int i = 0; i < gridView.RowCount; i++)
            {
                result[i] = new float[gridView.ColumnCount];
                for (int j = 0; j < gridView.ColumnCount; j++)
                {
                    var cellValue = gridView[j, i].Value?.ToString()?.Trim();
                    try
                    {
                        result[i][j] = string.IsNullOrEmpty(cellValue)
                            ? 0
                            : float.Parse(cellValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        string rowHeader = gridView.Rows[i].HeaderCell.Value?.ToString() ?? $"Row {i + 1}";
                        string columnHeader = gridView.Columns[j].HeaderText ?? $"Column {j + 1}";
                        throw new FormatException($"Не удалось конвертировать значение '{cellValue}' в ячейке ({rowHeader}, {columnHeader}).");
                    }
                }
            }
            return result;
        }
            // Метод для заполнения столбца таблицы из массива float[]
            public  void SetColumnValues(DataGridView gridView, int column, float[] values)
            {
                if (column < 0 || column >= gridView.ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(column), "Указан недопустимый номер столбца.");
                if (values.Length != gridView.RowCount)
                    throw new ArgumentException("Размер массива значений не соответствует числу строк в таблице.");

                for (int i = 0; i < gridView.RowCount; i++)
                {
                    gridView[column, i].Value = values[i].ToString(CultureInfo.InvariantCulture);
                }
            }

            // Метод для заполнения массива DataFloat из столбца таблицы
            public  void UpdateDataFloatFromColumn(DataGridView gridView, int column, float[][] dataFloat)
            {
                if (column < 0 || column >= gridView.ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(column), "Указан недопустимый номер столбца.");
                if (dataFloat.Length != gridView.RowCount)
                    throw new ArgumentException("Количество строк в DataFloat не соответствует числу строк в таблице.");

                for (int i = 0; i < gridView.RowCount; i++)
                {
                    dataFloat[i][column] = float.Parse(gridView[column, i].Value?.ToString() ?? "0", CultureInfo.InvariantCulture);
                }
            }

            // Метод для конвертации одномерного массива float в массив byte
            public  byte[] FloatArrayToByteArray(float[] array)
            {
                byte[] result = new byte[array.Length * sizeof(float)];
                Buffer.BlockCopy(array, 0, result, 0, result.Length);
                return result;
            }

            // Метод для конвертации массива byte в массив float
            public  float[] ByteArrayToFloatArray(byte[] byteArray)
            {
                if (byteArray.Length % sizeof(float) != 0)
                    throw new ArgumentException("Размер массива byte некорректен для конвертации в массив float.");

                float[] result = new float[byteArray.Length / sizeof(float)];
                Buffer.BlockCopy(byteArray, 0, result, 0, byteArray.Length);
                return result;
            }

        #region ComboBoxRejak

        public void UpdateComboBoxRejak(ComboBox comboBox)
        {
            // Очищаем старое содержимое ComboBox
            comboBox.Items.Clear();

            // Массив строк, которые будут добавлены в ComboBox
            string[] modes = new string[]
            {
        "Wait",
        "Wash",
        "Fabric",
        "prepWash",
        "newWash",
        "Damage",
        "Sanitar",
        "FirstStart",
        "speedWash",
        "Stop",
        "WaitRazd",
        "WashRazd",
        "FabricRazd",
        "prepWashSteep1",
        "prepWashSteep2",
        "prepWashSteep3",
        "prepWashSteep4",
        "prepnewWashSteep1",
        "prepnewWashSteep2",
        "SanitarSteep1",
        "SanitarSteep2",
        "SanitarSteep3",
        "SanitarSteep4",
        "SanitarSteep5",
        "SanitarSteep6",
        "SanitarSteep7",
        "SanitarSteep8",
        "FirstStartSteep1",
        "FirstStatrSteep2",
        "FirstStartSteep3",
        "FirstStatrSteep4"
            };

            // Добавляем строки в ComboBox
            foreach (string mode in modes)
            {
                comboBox.Items.Add(mode);
            }
        }



        public void SetComboBoxFromRejak(ComboBox comboBox, ErejAKVA mode)
        {
            string modeText = "";

            // Определяем строку на основе значения ErejAKVA
            if (mode == ErejAKVA.rejak_Wait) modeText = "Wait";
            else if (mode == ErejAKVA.rejak_Wash) modeText = "Wash";
            else if (mode == ErejAKVA.rejak_Fabric) modeText = "Fabric";
            else if (mode == ErejAKVA.rejak_prepWash) modeText = "prepWash";
            else if (mode == ErejAKVA.rejak_newWash) modeText = "newWash";
            else if (mode == ErejAKVA.rejak_Damage) modeText = "Damage";
            else if (mode == ErejAKVA.rejak_Sanitar) modeText = "Sanitar";
            else if (mode == ErejAKVA.rejak_FirstStart) modeText = "FirstStart";
            else if (mode == ErejAKVA.rejak_speedWash) modeText = "speedWash";
            else if (mode == ErejAKVA.rejak_Stop) modeText = "Stop";
            else if (mode == ErejAKVA.rejak_WaitRazd) modeText = "WaitRazd";
            else if (mode == ErejAKVA.rejak_WashRazd) modeText = "WashRazd";
            else if (mode == ErejAKVA.rejak_FabricRazd) modeText = "FabricRazd";
            else if (mode == ErejAKVA.rejak_prepWashSteep1) modeText = "prepWashSteep1";
            else if (mode == ErejAKVA.rejak_prepWashSteep2) modeText = "prepWashSteep2";
            else if (mode == ErejAKVA.rejak_prepWashSteep3) modeText = "prepWashSteep3";
            else if (mode == ErejAKVA.rejak_prepWashSteep4) modeText = "prepWashSteep4";
            else if (mode == ErejAKVA.rejak_prepnewWashSteep1) modeText = "prepnewWashSteep1";
            else if (mode == ErejAKVA.rejak_prepnewWashSteep2) modeText = "prepnewWashSteep2";
            else if (mode == ErejAKVA.rejak_SanitarSteep1) modeText = "SanitarSteep1";
            else if (mode == ErejAKVA.rejak_SanitarSteep2) modeText = "SanitarSteep2";
            else if (mode == ErejAKVA.rejak_SanitarSteep3) modeText = "SanitarSteep3";
            else if (mode == ErejAKVA.rejak_SanitarSteep4) modeText = "SanitarSteep4";
            else if (mode == ErejAKVA.rejak_SanitarSteep5) modeText = "SanitarSteep5";
            else if (mode == ErejAKVA.rejak_SanitarSteep6) modeText = "SanitarSteep6";
            else if (mode == ErejAKVA.rejak_SanitarSteep7) modeText = "SanitarSteep7";
            else if (mode == ErejAKVA.rejak_SanitarSteep8) modeText = "SanitarSteep8";
            else if (mode == ErejAKVA.rejak_FirstStartSteep1) modeText = "FirstStartSteep1";
            else if (mode == ErejAKVA.rejak_FirstStatrSteep2) modeText = "FirstStatrSteep2";
            else if (mode == ErejAKVA.rejak_FirstStartSteep3) modeText = "FirstStartSteep3";
            else if (mode == ErejAKVA.rejak_FirstStatrSteep4) modeText = "FirstStatrSteep4";
            else throw new ArgumentException("Неизвестное значение ErejAKVA.");

            // Найдем индекс строки в ComboBox
            int itemIndex = comboBox.Items.IndexOf(modeText);

            if (itemIndex >= 0)
            {
                // Устанавливаем выбранный элемент
                comboBox.SelectedIndex = itemIndex;
            }
            else
            {
                throw new ArgumentException("Режим не найден в списке.");
            }
        }

        public int SetTimersMult(ComboBox comboBox)
        {//возвращает коэффициент ускорения таймеров

            switch (comboBox.SelectedIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 10;
                case 3:
                    return 60;
                default:
                    return -1;
            }
        }


        public ErejAKVA SetRejakFromComboBox(ComboBox comboBox)
        {
            // Получаем активный индекс выбранной строки
            int selectedIndex = comboBox.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= comboBox.Items.Count)
            {
                throw new ArgumentOutOfRangeException("Неверный индекс строки.");
            }

            string selectedLine = comboBox.Items[selectedIndex].ToString();

            // Сопоставляем строку с элементами перечисления ErejAKVA
            if (selectedLine == "Wait") return ErejAKVA.rejak_Wait;
            else if (selectedLine == "Wash") return ErejAKVA.rejak_Wash;
            else if (selectedLine == "Fabric") return ErejAKVA.rejak_Fabric;
            else if (selectedLine == "prepWash") return ErejAKVA.rejak_prepWash;
            else if (selectedLine == "newWash") return ErejAKVA.rejak_newWash;
            else if (selectedLine == "Damage") return ErejAKVA.rejak_Damage;
            else if (selectedLine == "Sanitar") return ErejAKVA.rejak_Sanitar;
            else if (selectedLine == "FirstStart") return ErejAKVA.rejak_FirstStart;
            else if (selectedLine == "speedWash") return ErejAKVA.rejak_speedWash;
            else if (selectedLine == "Stop") return ErejAKVA.rejak_Stop;
            else if (selectedLine == "WaitRazd") return ErejAKVA.rejak_WaitRazd;
            else if (selectedLine == "WashRazd") return ErejAKVA.rejak_WashRazd;
            else if (selectedLine == "FabricRazd") return ErejAKVA.rejak_FabricRazd;
            else if (selectedLine == "prepWashSteep1") return ErejAKVA.rejak_prepWashSteep1;
            else if (selectedLine == "prepWashSteep2") return ErejAKVA.rejak_prepWashSteep2;
            else if (selectedLine == "prepWashSteep3") return ErejAKVA.rejak_prepWashSteep3;
            else if (selectedLine == "prepWashSteep4") return ErejAKVA.rejak_prepWashSteep4;
            else if (selectedLine == "prepnewWashSteep1") return ErejAKVA.rejak_prepnewWashSteep1;
            else if (selectedLine == "prepnewWashSteep2") return ErejAKVA.rejak_prepnewWashSteep2;
            else if (selectedLine == "SanitarSteep1") return ErejAKVA.rejak_SanitarSteep1;
            else if (selectedLine == "SanitarSteep2") return ErejAKVA.rejak_SanitarSteep2;
            else if (selectedLine == "SanitarSteep3") return ErejAKVA.rejak_SanitarSteep3;
            else if (selectedLine == "SanitarSteep4") return ErejAKVA.rejak_SanitarSteep4;
            else if (selectedLine == "SanitarSteep5") return ErejAKVA.rejak_SanitarSteep5;
            else if (selectedLine == "SanitarSteep6") return ErejAKVA.rejak_SanitarSteep6;
            else if (selectedLine == "SanitarSteep7") return ErejAKVA.rejak_SanitarSteep7;
            else if (selectedLine == "SanitarSteep8") return ErejAKVA.rejak_SanitarSteep8;
            else if (selectedLine == "FirstStartSteep1") return ErejAKVA.rejak_FirstStartSteep1;
            else if (selectedLine == "FirstStatrSteep2") return ErejAKVA.rejak_FirstStatrSteep2;
            else if (selectedLine == "FirstStartSteep3") return ErejAKVA.rejak_FirstStartSteep3;
            else if (selectedLine == "FirstStatrSteep4") return ErejAKVA.rejak_FirstStatrSteep4;
            else throw new ArgumentException("Неизвестный режим.");
        }

    }



    #endregion



 






}


