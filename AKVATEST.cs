using System;
using System.Windows.Forms;
using System.Collections.Generic;
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
        public uint LastStamp_mSec;
        public uint MaxCountSec;
        public uint DamageSec;
    }


    public class TableManager
    {
        private int x = -1; // Координата строки изменённой ячейки
        private int y = -1; // Координата столбца изменённой ячейки
        private bool Change = false; // Флаг изменения вручную

        // Конструктор класса
        public TableManager(DataGridView dataGridView)
        {
            // Отключаем сортировку столбцов
            dataGridView.ColumnHeaderMouseClick += (s, e) =>
            {
                dataGridView.Sort(dataGridView.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
            };

            // Подключаемся к событию окончания редактирования
            dataGridView.CellEndEdit += (s, e) =>
            {
                // Запоминаем координаты изменённой вручную ячейки
                x = e.RowIndex;
                y = e.ColumnIndex;
                Change = true;
            };
        }

        // Метод обновления ячеек
        public void DisplayInDataGridView(DataGridView dataGridView, SATIMER[] T)
        {
            for (int i = 0; i < T.Length; i++)
            {
                UpdateCellIfNeeded(dataGridView.Rows[0].Cells[i], T[i].Rej, 0, i);
                UpdateCellIfNeeded(dataGridView.Rows[1].Cells[i], T[i].CountSec, 1, i);
                UpdateCellIfNeeded(dataGridView.Rows[2].Cells[i], T[i].LastStamp_mSec, 2, i);
                UpdateCellIfNeeded(dataGridView.Rows[3].Cells[i], T[i].MaxCountSec, 3, i);
                UpdateCellIfNeeded(dataGridView.Rows[4].Cells[i], T[i].DamageSec, 4, i);
            }
        }

        private void UpdateCellIfNeeded(DataGridViewCell cell, object newValue, int row, int col)
        {
            // Если ячейка была изменена вручную, пропускаем её обновление
            if (Change && x == row && y == col)
            {
                Change = false; // Сбрасываем флаг после пропуска обновления
                return;
            }

            // Обновляем ячейку, если новое значение отличается
            UpdateCellIfChanged(cell, newValue);
        }

        public void UpdateCellIfChanged(DataGridViewCell cell, object newValue)
        {
            // Проверяем, что ячейка нередактируемая и новое значение отличается от текущего
            if (!cell.ReadOnly && !Equals(cell.Value, newValue))
            {
                cell.Value = newValue;
            }
        }
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


    public struct SParFlow
    {
        public float AllLitr { get; set; }
        public float LitrPerHour { get; set; }
        public float LitrPerMinute { get; set; }
    }

    public class SAKVApar
    {
        public ErejAKVA Rej { get; set; }
        public SParFlow[] FM { get; set; } = new SParFlow[3];
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






    public class SWORKAKVATEST
    {
        private DataGridView TimersGridView;
        private DataGridView ParamGridView;
        public SWORKAKVATEST(DataGridView _TimersGridView, DataGridView _ParamGridView)
        {//конструктор в котором происходит формирование таблиц таймеров и параметров
            TimersGridView = _TimersGridView ?? throw new ArgumentNullException(nameof(_TimersGridView));
            ParamGridView = _ParamGridView ?? throw new ArgumentNullException(nameof(_ParamGridView));
            FormatParamsGridView();//установка параметров таблицы параметров
            DrawAKVAtable(ParamGridView, FormatParamsGV);            
            ParamSetHeaders();//форматирование таблицы параметров
            //форматирование таблицы таймеров
            FormatTimersGridView(120, 30, new string[] { "Rej", "CountSec", "LastStamp_mSec", "MaxCountSec", "DamageSec" }, GetTextHead(0, 7));
            TimersParPerminEdit();
        }

        public GridViewParams FormatParamsGV;//структура хранения параметров форматирования таблицы ParamGridView
        public SATIMER[] T = new SATIMER[ArraySize];
        public ErejAKVA selectedMode = ErejAKVA.rejak_Stop;




        public  readonly string[] rejheaders = { "Wait", "Wash", "Fabric", "prepWash", "newWash", "Damage", "Sanitar", "FirstStart", "speedWash", "Stop",
                         "WaitRazd", "WashRazd", "FabricRazd", "prepWashSteep1", "prepWashSteep2", "prepWashSteep3", "prepWashSteep4",
                         "newWashSteep1", "newWashSteep2", "SanitarSteep1", "SanitarSteep2", "SanitarSteep3", "SanitarSteep4", "SanitarSteep5", "SanitarSteep6",
                         "SanitarSteep7", "SanitarSteep8", "FirstStartSteep1", "FirstStartSteep2", "FirstStartSteep3", "FirstStartSteep4" };

        public readonly string[] rowHeaders = { "FM[0]", "FM[1]", "FM[2]", "PT[0]", "PT[1]", "QT[0]", "QT[1]", "QT[2]",
                            "InONOFF", "InESC", "InPressRazd", "InFlowMeters" };




     #region Timers

        private const int ArraySize = 8;


        public void TimersParFromByteArray(byte[] data)
        {//преобразования из массива байт в структуру
            if (data.Length != Marshal.SizeOf(typeof(SATIMER)) * ArraySize)
                throw new ArgumentException("Invalid data length");

            for (int i = 0; i < ArraySize; i++)
            {
                int offset = i * Marshal.SizeOf(typeof(SATIMER));
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SATIMER)));
                Marshal.Copy(data, offset, ptr, Marshal.SizeOf(typeof(SATIMER)));
                T[i] = (SATIMER)Marshal.PtrToStructure(ptr, typeof(SATIMER));
                Marshal.FreeHGlobal(ptr);
            }
        }


        

        private void TimersParPerminEdit()
        {//разрешает редактировать только одну строку номер 1 содержащую текущее значением таймеров
            int editableRowIndex = 1; // Номер строки, которая остаётся редактируемой
            foreach (DataGridViewRow row in TimersGridView.Rows)
            {
                if (row.Index != editableRowIndex)
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

        public void DisplayInDataGridView()
        {
            //Запись должна проводиться только в нередактируемую ячейку и только если её значение изменилось.
            // Заполняем таблицу данными из SATIMER[]
            for (int i = 0; i < ArraySize; i++)
            {
                // Проверяем каждую ячейку перед записью
                UpdateCellIfChanged(TimersGridView.Rows[0].Cells[i], T[i].Rej);
                UpdateCellIfChanged(TimersGridView.Rows[1].Cells[i], T[i].CountSec);
                UpdateCellIfChanged(TimersGridView.Rows[2].Cells[i], T[i].LastStamp_mSec);
                UpdateCellIfChanged(TimersGridView.Rows[3].Cells[i], T[i].MaxCountSec);
                UpdateCellIfChanged(TimersGridView.Rows[4].Cells[i], T[i].DamageSec);
            }
        }

        public void UpdateCellIfChanged(DataGridViewCell cell, object newValue)
        {
            // Проверяем, что ячейка нередактируемая и новое значение отличается от текущего
            if (!cell.ReadOnly && !Equals(cell.Value, newValue))
            {
                cell.Value = newValue;
            }
        }

        public void UpdateFromDataGridView()
        {// считывание всех данных из таблицы
            for (int i = 0; i < ArraySize; i++)
            {
                T[i].Rej = (ErejTimer)Enum.Parse(typeof(ErejTimer), TimersGridView.Rows[0].Cells[i + 1].Value.ToString());
                T[i].CountSec = uint.Parse(TimersGridView.Rows[1].Cells[i + 1].Value.ToString());
                T[i].LastStamp_mSec = uint.Parse(TimersGridView.Rows[2].Cells[i + 1].Value.ToString());
                T[i].MaxCountSec = uint.Parse(TimersGridView.Rows[3].Cells[i + 1].Value.ToString());
                T[i].DamageSec = uint.Parse(TimersGridView.Rows[4].Cells[i + 1].Value.ToString());
            }
        }

     #endregion



     #region SimplFormatTibles
//ТАБЛИЦА ПАРАМЕТРОВ

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

        public void FormatTimersGridView( int rowHeaderWidth, int columnHeaderHeight, string[] rowHeaders, string[] columnHeaders)
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



     #region ScrollTable

        /*
            public struct GridViewParams
            {
                public int numCol { get; set; }//общее количество колонок в таблице
                public int numRow { get; set; }//общее количество строк в таблице
                public int X { get; set; }//координала X левого верхнего угла
                public int Y { get; set; }//координала Y левого верхнего угла
                public int Width { get; set; }//ширина всех колонок, кроме левой
                public int Height { get; set; }//высота всех строк
                public int L1 { get; set; }//номер колонки которая отображается следующей за зафиксированной нулевой
                public int WidthLeftCol { get; set; }//ширина крайне левой колонки
                public int Vcol { get; set; }//количество колонок, которые должны умещаться в промежутке видимой зоны от крайне левой зафиксированной колонки до крайне правой
            }

                        dGparam.Location.X,
                dGparam.Location.Y,
                dGparam.Size.Width,
                dGparam.Size.Height,

         */
        public void FormatParamsGridView()
        {//форматируем таблицу структуры GVstruct

            FormatParamsGV.numRow =  rowHeaders.Length; //общее количество колонок в таблице на одну больше чем количество режимов из за крайне левого столбца
            FormatParamsGV.numCol = rejheaders.Length; //общее количество строк в таблице
            FormatParamsGV.L1 = 1;//сразу после инициализации номер колонки которая отображается следующей за зафиксированной нулевой
            FormatParamsGV.WidthLeftCol = 75;  // //ширина крайне левой колонки
            FormatParamsGV.ViewCol = 7;           //количество колонок, которые должны умещаться в промежутке видимой зоны от крайне левой зафиксированной колонки до крайне правой
            // Чтение текущих размеров компонента в структуру (ширина и высота)
            FormatParamsGV.Width = ParamGridView.Size.Width;
            FormatParamsGV.Height = ParamGridView.Size.Height;

/*

            // Настройка первой фиксированной колонки (ширина крайней левой колонки)
            if (ParamGridView.Columns.Count > 0)
            {
                ParamGridView.Columns[0].Width = FormatParamsGV.WidthLeftCol;
            }

            // Настройка видимых колонок (если нужно вручную настроить количество видимых колонок)
            for (int i = 1; i < Math.Min(ParamGridView.Columns.Count, FormatParamsGV.ViewCol + 1); i++)
            {
                // Устанавливаем ширину каждой видимой колонки (кроме первой фиксированной)
                ParamGridView.Columns[i].Width = (FormatParamsGV.Width - FormatParamsGV.WidthLeftCol) / FormatParamsGV.ViewCol;
            }

            // Настройка для номера колонки, следующей за первой фиксированной (если требуется)
            ParamGridView.FirstDisplayedScrollingColumnIndex = FormatParamsGV.L1;
*/
        }

        /*
                public void ApplyGridViewParams(DataGridView gridView)
                {//gridView - сама таблица, gridParams - структура параметров таблицы

                    // Чтение текущих размеров компонента в структуру (ширина и высота)
                    FormatParamsGV.Width = gridView.Size.Width;
                    FormatParamsGV.Height = gridView.Size.Height;

                    // Настройка первой фиксированной колонки (ширина крайней левой колонки)
                    if (gridView.Columns.Count > 0)
                    {
                        gridView.Columns[0].Width = FormatParamsGV.WidthLeftCol;
                    }

                    // Настройка видимых колонок (если нужно вручную настроить количество видимых колонок)
                    for (int i = 1; i < Math.Min(gridView.Columns.Count, FormatParamsGV.ViewCol + 1); i++)
                    {
                        // Устанавливаем ширину каждой видимой колонки (кроме первой фиксированной)
                        gridView.Columns[i].Width = (FormatParamsGV.Width - FormatParamsGV.WidthLeftCol) / FormatParamsGV.ViewCol;
                    }

                    // Настройка для номера колонки, следующей за первой фиксированной (если требуется)
                    gridView.FirstDisplayedScrollingColumnIndex = FormatParamsGV.L1;
                }
        */


  

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
        {//отрисовка таблицы на основе параметров GridViewParams
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

            // Задаём размеры и расположение таблицы
            // Не надо метять эти параметры!         AKVAparGridView.Location = new System.Drawing.Point(x, y);

            // Разрешаем отображение полосы прокрутки
            AKVAparGridView.ScrollBars = ScrollBars.Horizontal;


            // Устанавливаем количество строк
            if (AKVAparGridView.RowCount != Numrow)
            {
                AKVAparGridView.RowCount = Numrow;
            }

            // Устанавливаем содержимое нулевого столбца (заголовки строк)


            for (int i = 0; i < Math.Min(Numrow, rowHeaders.Length); i++)
            {
                AKVAparGridView.Rows[i].Cells[0].Value = rowHeaders[i];
            }

            // Заполняем остальные ячейки "0"
            for (int row = 0; row < AKVAparGridView.RowCount; row++)
            {
                for (int col = 1; col < AKVAparGridView.ColumnCount; col++)
                {
                    AKVAparGridView.Rows[row].Cells[col].Value = "0";
                }
            }

            // Сделать редактируемыми все ячейки, кроме первого столбца и первой строки
            for (int row = 0; row < AKVAparGridView.RowCount; row++)
            {
                for (int col = 1; col < AKVAparGridView.ColumnCount; col++)
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


     #endregion

            /*            // Очистим все предыдущие выделения
                        DGV.ClearSelection();

                        // Убедимся, что индекс колонки допустимый
                        if (columnIndex >= 0 && columnIndex < DGV.Columns.Count)
                        {
                            // Проверим, находится ли колонка в видимой области
                             Rectangle columnRect = DGV.GetColumnDisplayRectangle(columnIndex, true);

                            if (columnRect.Right > DGV.DisplayRectangle.Right)
                            {
                                // Прокручиваем влево, если колонка выходит за пределы видимой области
                                DGV.FirstDisplayedScrollingColumnIndex = columnIndex;
                            }
                            else if (columnRect.Left < 0)
                            {
                                // Прокручиваем вправо, если колонка находится слева видимой области
                                DGV.FirstDisplayedScrollingColumnIndex = columnIndex;
                            }

                            // Программно выделим всю колонку вместе с заголовком
                            DGV.Columns[columnIndex].Selected = true;
                        }
                        else
                        {
                            MessageBox.Show("Неправильный индекс колонки.");
                        }*/
        }




        public void saveAKVAparTable(string fileName, DataGridView AKVAparGridView)
        {
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





        #endregion












    }
}


