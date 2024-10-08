using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;

namespace TFTprog
{

    public enum ErejAKVA
    {

    rejak_NULL = 0,
    rejak_Wait   	= 		1,//ожидание
	rejak_Wash  	 = 		2,//промывка
	rejak_Fabric   	= 		3,//производство
	rejak_prepWash	= 		4,//промывка преподготовки
	rejak_newWash  	= 		5,//промывка новых мембран
	rejak_Damage   	= 		6,//Авария
	rejak_Sanitar   = 		7,//санитарная обработка
	rejak_FirstStart  = 	8,//первое включение
	rejak_speedWash  = 		9,//быстрая промывка

	rejak_Stop		=		10,
	rejak_none		=		0xff,
    };


    //    структура для хранения параметров - min,Vol, max
    public struct SminmaxPar
    {
        private float _min;
        private float _vol;
        private float _max;
        private int _numvol;

        public float min
        {
            get { return _min; }
            set { _min = value; }
        }
        public float vol
        {
            get { return _vol; }
            set { _vol = value; }
        }
        public float max
        {
            get { return _max; }
            set { _max = value; }
        }

        public int numvol
        {
            get { return _numvol; }
            set { _numvol = value; }
        }

        #region tableUtils

        public void SetDIMstr(int rowIndex, int columnIndex, float value, DataTable tablePAR)
        {
            string PARdim = "";
        }

        public static float GetFloatFromBytes(byte[] byteArray, int startIndex)
        {
            // Проверяем, чтобы в массиве было достаточно байт для извлечения float
            if (byteArray == null || byteArray.Length < startIndex + 4)
            {
                throw new ArgumentException("Array does not contain enough bytes starting from the given index.");
            }

            // Извлекаем 4 байта, начиная с указанного индекса
            byte[] floatBytes = new byte[4];
            Array.Copy(byteArray, startIndex, floatBytes, 0, 4);

            // Преобразуем байты в float
            return BitConverter.ToSingle(floatBytes, 0);
        }

        /*
        Условия возникновения аварий и предупреждений 

        float		minPin;         PT0<P0 низкое входное давление
		float		maxQT0;         QT0>Q0 высокая электропроводность исходной воды
		float		minFT1;         
		float		minFT0minusFT1; 
		float		maxQT1;         QT1>Q1 высокая электропроводность пермиата
		float		maxQT2;         QT2>Q2 высокая электропроводность фильтрата

		unsigned int T1stop;
		unsigned int T2Wash;
		unsigned int T3wait;
		unsigned int T4preprod;
		unsigned int TNEWWash;

		float		minFT1divFT0;   FT1/
		float		maxFT1divFT0; 
         */
        public void GetTableFromPARarr(byte[] PARarr,int CountPAR, DataTable tablePAR)
        {// Метод для извлечения параметра из массива и занесения в соответствующую ему ячейку (кроме первой колонки)
            int numInArr = 0;
            float fpar_min_err;
            float fpar_cur_err;
            float fpar_max_err;
            float fpar_warn;
            
            ;
            for (int rowIndex=1; rowIndex<= CountPAR; rowIndex++)
            {//проходим по всем строкам не считая заголовка
                fpar_min_err = GetFloatFromBytes(PARarr, numInArr);
                numInArr += 4;
                fpar_cur_err = GetFloatFromBytes(PARarr, numInArr);
                numInArr += 4;
                fpar_max_err = GetFloatFromBytes(PARarr, numInArr);
                numInArr += 4;

                switch (rowIndex)
                {
                    case 1:
                    case 2:
                    case 5:
                    case 6:
                        fpar_warn = fpar_cur_err * (float)1.2; //LevelWarn for < 
                        break;
                    default:
                        fpar_warn = fpar_cur_err * (float)0.8;// LevelWarn for  >
                        break;
                }

                SetFloatInCell(rowIndex, 1, fpar_min_err, tablePAR);
                SetFloatInCell(rowIndex, 1, fpar_warn, tablePAR);
                SetFloatInCell(rowIndex, 1, fpar_cur_err, tablePAR);
                SetFloatInCell(rowIndex, 1, fpar_max_err, tablePAR);

                switch (rowIndex)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 12:
                        break;
                    case 13:
                        break;
                    default:

                        break;
                }

            }

        }


        public void SetFloatInCell(int rowIndex, int columnIndex, float value, DataTable tablePAR)
        {// Метод для занесения числа в произвольную ячейку (кроме первой колонки)
            if (rowIndex >= 0 && rowIndex < tablePAR.Rows.Count && columnIndex > 0 && columnIndex < tablePAR.Columns.Count)
            {
                tablePAR.Rows[rowIndex][columnIndex] = value;
            }
        }

        public float GetFloatFromCell(int rowIndex, int columnIndex, DataTable tablePAR)
        {// Метод для чтения числа из произвольной ячейки (кроме первой колонки)
            if (rowIndex >= 0 && rowIndex < tablePAR.Rows.Count && columnIndex > 0 && columnIndex < tablePAR.Columns.Count)
            {
                return Convert.ToSingle(tablePAR.Rows[rowIndex][columnIndex]);
            }
            return 0.0f;
        }

        public byte[] GetColumnValuesAsBytes(int columnIndex, int Ystart, int Ystop, DataTable tablePAR)
        {// Метод для считывания значений из столбца в массив байт
            if (columnIndex > 0 && columnIndex < tablePAR.Columns.Count && Ystart >= 0 && Ystop < tablePAR.Rows.Count && Ystart <= Ystop)
            {
                int count = Ystop - Ystart + 1;
                byte[] byteArray = new byte[count * 4];
                for (int i = 0; i < count; i++)
                {
                    if (i==6)
                    {
                        i = i + 5;
                    }
                    float value = Convert.ToSingle(tablePAR.Rows[Ystart + i][columnIndex]);
                    byte[] floatBytes = BitConverter.GetBytes(value);
                    Array.Copy(floatBytes, 0, byteArray, i * 4, 4);
                }
                return byteArray;
            }
            return new byte[0];
        }

        public void SetStringInTable(string inputString, DataTable tablePAR)
        {
            // Разбиваем строку на элементы по разделителю ";"
            string[] elements = inputString.Split(';');

            int elementIndex = 0;
            // Заполняем таблицу элементами из строки
            for (int i = 0; i < tablePAR.Rows.Count; i++)
            {
                for (int j = 0; j < tablePAR.Columns.Count; j++)
                {
                    if (elementIndex < elements.Length)
                    {
                        tablePAR.Rows[i][j] = elements[elementIndex];
                        elementIndex++;
                    }
                }
            }
        }

        #endregion


        public string typeVol
        {
            get {
                switch (numvol)
                {
                    case 0:
                    case 1:
                    case 2:
                        return "мкСм/см";
                    case 4:
                        return "Бар";
                    case 5:
                    case 6:
                        return "%";
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        return "мин";
                    case 12:
                        return "PSET";
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        return "л/мин";
                    default:
                        return "";
                }


                 }
        }

        public void SetParams(float _vmin, float _vvol, float _vmax)
        {
            min = _vmin;
            vol = _vvol;
            max = _vmax;
        }

        public byte[] GetArr()
        {//возвращает содержание структуры параметров в виде массива байт
            byte[] vminb = BitConverter.GetBytes(min);
            byte[] vvolb = BitConverter.GetBytes(vol);
            byte[] vmaxb = BitConverter.GetBytes(max);

            byte[] retbarr = new byte[12];
            vminb.CopyTo(retbarr, 0);
            vvolb.CopyTo(retbarr, 4);
            vmaxb.CopyTo(retbarr, 8);
            return retbarr;
        }

        public void SetFromArr(byte[] ArrPar,int NumPar)
        {//устанавливает значение параметров структуры из массива байт

            min = BitConverter.ToSingle(ArrPar, 0 + NumPar*12);
            vol = BitConverter.ToSingle(ArrPar, 4 + NumPar * 12);
            max = BitConverter.ToSingle(ArrPar, 8 + NumPar * 12);
        }


    }



    public class SminmaxParams
    {
        public SminmaxPar[] vsrazm;

        public SminmaxParams(int count_AkvaPar)
        {
            _countAkvaPar = count_AkvaPar;
            vsrazm = new SminmaxPar[_countAkvaPar];
        }

        private int _countAkvaPar;
        public float countAkvaPar
        {
            get { return _countAkvaPar; }

        }


        public void setminmaxPar(float _vmin, float _vvol, float _vmax, int NumPar)
        {//установка параметра
            if (NumPar < countAkvaPar)
            {
                vsrazm[NumPar].SetParams(_vmin, _vvol, _vmax);
                vsrazm[NumPar].numvol = NumPar;
            }

        }

        public void getParamsFromArr(byte[] arr)
        {
            for (int i = 0; i < countAkvaPar; i++)
            {
                vsrazm[i].SetFromArr(arr, i);
            }
        }



        public byte[] setArrFromParams()
        {
            // формируем массив из параметров
            // int countAkvaPar = 5;  // Это просто пример, замените на ваш реальный countAkvaPar

            // Создаём список для хранения всех массивов
            List<byte> resultList = new List<byte>();

            for (int i = 0; i < countAkvaPar; i++)
            {
                // Получаем очередной массив байтов
                byte[] arr = vsrazm[i].GetArr();

                // Добавляем его в результирующий список
                resultList.AddRange(arr);
            }

            // Преобразуем список обратно в массив
            return resultList.ToArray();
        }




    }
    }



    //******************   ЧАТ GPT
    /*
     * 
     * 
     * enum
    public class SParFlow
    {
        public float ImpFreq { get; set; }
        public float AllLitr { get; set; }
        public float LitrPerHour { get; set; }
        public float LitrPerMinute { get; set; }
    }

    public class SParAnal
    {
        public float Val { get; set; }
        public float ValFilt { get; set; }
    }

    public enum ErejAKVA
    {
        // Enum values for ErejAKVA
    }

    public class SAKVApar
    {
        public ErejAKVA Rej { get; set; }

        public SParFlow[] FM { get; } = new SParFlow[3];
        public SParAnal[] PT { get; } = new SParAnal[2];
        public SParAnal[] QT { get; } = new SParAnal[3];

        public struct Flags
        {
            public bool K1, K2, K3, K4, K5, K6, H1, H2, LED1, RELE;
            public int Unuse;
        }

        public Flags OUTs;

        public struct Inputs
        {
            public int Unus0;
            public bool InONOFF, InESC, InPressRazd;
            public int InFlowMeters;
            public int Unus1;
        }

        public Inputs INs;

        public uint TimeStampSec { get; set; }



        public byte[] PackData()
        {
            int size = Marshal.SizeOf(typeof(SAKVApar));
            byte[] data = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);

            return data;
        }

        public static SAKVApar UnpackData(byte[] data)
        {
            int size = Marshal.SizeOf(typeof(SAKVApar));

            if (data.Length != size)
            {
                throw new ArgumentException("Invalid data length");
            }

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);

            try
            {
                return (SAKVApar)Marshal.PtrToStructure(ptr, typeof(SAKVApar));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }

    */


    public class SoutsClass
{
    public bool K1 { get; set; }
    public bool K2 { get; set; }
    public bool K3 { get; set; }
    public bool K4 { get; set; }
    public bool K5 { get; set; }
    public bool K6 { get; set; }
    public bool H1 { get; set; }
    public bool H2 { get; set; }
    public bool LED1 { get; set; }
    public bool RELE { get; set; }



    public void ReadData(byte[] data)
    {
        if (data.Length < 2)
        {
            throw new ArgumentException("Invalid data length");
        }

        K1 = (data[0] & 0x01) != 0;
        K2 = (data[0] & 0x02) != 0;
        K3 = (data[0] & 0x04) != 0;
        K4 = (data[0] & 0x08) != 0;
        K5 = (data[0] & 0x10) != 0;
        K6 = (data[0] & 0x20) != 0;
        H1 = (data[0] & 0x40) != 0;
        H2 = (data[0] & 0x80) != 0;
        LED1 = (data[1] & 0x01) != 0;
        RELE = (data[1] & 0x02) != 0;
    }

    public int GetData()
    {
        int result = 0;

        result |= K1 ? 0x01 : 0;
        result |= K2 ? 0x02 : 0;
        result |= K3 ? 0x04 : 0;
        result |= K4 ? 0x08 : 0;
        result |= K5 ? 0x10 : 0;
        result |= K6 ? 0x20 : 0;
        result |= H1 ? 0x40 : 0;
        result |= H2 ? 0x80 : 0;

        result |= LED1 ? 0x100 : 0;
        result |= RELE ? 0x200 : 0;

        return result;
    }



    public class SinClass
    {
        public bool inONOFF { get; set; }
        public bool inESC { get; set; }
        public bool inPressRazd { get; set; }

        public int inFlowMeters { get; set; }

        public void ReadData(byte[] data)
        {
            if (data.Length < 2)
            {
                throw new ArgumentException("Invalid data length");
            }

            inONOFF = (data[0] & 0x20) != 0;
            inESC = (data[0] & 0x40) != 0;
            inPressRazd = (data[0] & 0x80) != 0;
            inFlowMeters = data[1] & 0x07;
        }

        public int GetData()
        {
            int result = 0;

            result |= inONOFF ? 0x20 : 0;
            result |= inESC ? 0x40 : 0;
            result |= inPressRazd ? 0x80 : 0;
            result |= (inFlowMeters << 8) & 0x700;

            return result;
        }


    }


};
