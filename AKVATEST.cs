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

namespace TESTAKVA
{

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

    class AKVATEST
    {

        SAKVApar AKVApar;
        ErejAKVA rejAKVA;



    }

    public class DateTimeConverter
    {
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


        public  void DrawAKVAtable(DataGridView AKVAparGridView, int x, int y, int L, int H, int L1, int widthleftCol, int Vcol)
        {
            // Устанавливаем содержимое первой строки (заголовки столбцов)
            string[] headers = { "Wait", "Wash", "Fabric", "prepWash", "newWash", "Damage", "Sanitar", "FirstStart", "speedWash", "Stop",
                         "WaitRazd", "WashRazd", "FabricRazd", "prepWash1", "prepWash2", "prepWash3", "prepWash4",
                         "newWash1", "newWash2", "Sanitar1", "Sanitar2", "Sanitar3", "Sanitar4", "Sanitar5", "Sanitar6",
                         "Sanitar7", "Sanitar8", "FirstStart1", "FirstStart2", "FirstStart3", "FirstStart4" };

            // Задаём размеры и расположение таблицы
            AKVAparGridView.Location = new System.Drawing.Point(x, y);

            // Настраиваем полосу прокрутки
            AKVAparGridView.ScrollBars = ScrollBars.Horizontal;

            // Задаём параметры высоты и ширины таблицы
            AKVAparGridView.Size = new System.Drawing.Size(L, H - SystemInformation.HorizontalScrollBarHeight); // Учтём высоту скроллбара

            // Устанавливаем количество строк и столбцов
            AKVAparGridView.RowCount = 13;
            AKVAparGridView.ColumnCount = headers.Length; // Общее количество столбцов, но будет видно только Vcol

            // Настраиваем ширину столбцов
            AKVAparGridView.Columns[0].Width = widthleftCol; // Левый столбец фиксированной ширины
            int remainingWidth = L - widthleftCol;
            int columnWidth = remainingWidth / Vcol; // Ширина видимых столбцов

            for (int i = 1; i < AKVAparGridView.ColumnCount; i++)
            {
                AKVAparGridView.Columns[i].Width = columnWidth;
            }



            for (int i = 1; i < AKVAparGridView.ColumnCount; i++)
            {
                AKVAparGridView.Columns[i].HeaderText = headers[i - 1];
            }

            // Устанавливаем содержимое нулевого столбца
            string[] rowHeaders = { "Rej", "FM[0]", "FM[1]", "FM[2]", "PT[0]", "PT[1]", "QT[0]", "QT[1]", "QT[2]",
                            "InONOFF", "InESC", "InPressRazd", "InFlowMeters" };

            for (int i = 0; i < AKVAparGridView.RowCount; i++)
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

            // Включаем горизонтальный скроллинг
            AKVAparGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            AKVAparGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            AKVAparGridView.AllowUserToResizeColumns = false;
            AKVAparGridView.AllowUserToResizeRows = false;
            AKVAparGridView.ScrollBars = ScrollBars.Horizontal;
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

    }
}
