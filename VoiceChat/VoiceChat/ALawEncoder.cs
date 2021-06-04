namespace g711audio
{
    /// <summary>
    /// Превращает 16-битные линейные значения PCM в 8-битные байты A-law.
    /// </summary>
    public class ALawEncoder
    {
        public const int MAX = 0x7fff; //максимум, который может храниться в 15 битах

        /// <summary>
        /// Массив, в котором индекс-это 16-битный вход PCM, 
        /// а значение-результат А-закона.
        /// </summary>
        private static byte[] pcmToALawMap;

        static ALawEncoder()
        {
            pcmToALawMap = new byte[65536];
            for (int i = short.MinValue; i <= short.MaxValue; i++)
                pcmToALawMap[(i & 0xffff)] = encode(i);
        }

        /// <summary>
        /// Кодирует один байт a-law из 16-битного целого числа
        /// </summary>
        /// <param name="pcm">16-разрядное значение pcm со знаком</param>
        /// <returns>Байт в кодировке a-law</returns>
        private static byte encode(int pcm)
        {
            //Получаем знак бита.
            int sign = (pcm & 0x8000) >> 8;
            //Если число отрицательное, делаем его положительным (теперь это величина)
            if (sign != 0)
                pcm = -pcm;
            //Величина должна соответствовать 15 битам, чтобы избежать переполнения
            if (pcm > MAX) pcm = MAX;

            /* Поиск "экспоненты"
             * Биты:
             * 1 2 3 4 5 6 7 8 9 A B C D E F G
             * S 7 6 5 4 3 2 1 0 0 0 0 0 0 0 0
             * Мы хотим найти, где находится первая 1 после бита знака.
             * Мы берем соответствующее значение из второй строки в качестве значения экспоненты.
             * (т. е. если первая 1 в позиции 7 -> показатель степени = 2)
             * Показатель степени равен 0, если 1 не найдена в битах со 2 по 8.
             * Это означает, что показатель равен 0, даже если первая 1 не существует.
             */
            int exponent = 7;
            //Двигаемся вправо и уменьшаем показатель до тех пор, пока мы не достигнем 1 или показатель не достигнет 0
            for (int expMask = 0x4000; (pcm & expMask) == 0 && exponent>0; exponent--, expMask >>= 1) { }

            /* Последняя часть - "мантисса"
             * Нам нужно взять четыре бита после 1, которую мы только что нашли.
             * Чтобы получить их, мы сдвигаем 0x0f :
             * 1 2 3 4 5 6 7 8 9 A B C D E F G
             * S 0 0 0 0 0 1 . . . . . . . . . (например, показатель степени 2)
             * . . . . . . . . . . . . 1 1 1 1
             * Мы сдвигаем его 5 раз для экспоненты, равной двум, что означает
             * мы сдвинем наши четыре бита (экспонента + 3) бита.
             * Для удобства мы на самом деле просто сдвинем число, а затем и 0x0f.
             * 
             * Если показатель степени равен 0:
             * 1 2 3 4 5 6 7 8 9 A B C D E F G
             * S 0 0 0 0 0 0 0 Z Y X W V U T S (мы ничего не знаем о 9 бите)
             * . . . . . . . . . . . . 1 1 1 1
             * Мы хотим получить ZYXW, что означает сдвиг на 4 вместо 3
             */
            int mantissa = (pcm >> ((exponent == 0) ? 4 : (exponent + 3))) & 0x0f;

            //Расположение битов по закону A-Law это SEM (Знак, показатель степени и Мантисса.)
            byte alaw = (byte)(sign | exponent << 4 | mantissa);

            //переворачиваем каждый второй бит и знаковый бит (0xD5 = 1101 0101)
            return (byte)(alaw^0xD5);
        }

        /// <summary>
        /// Кодируем значение pcm в байт a-law
        /// </summary>
        /// <param name="pcm">16-битное значение pcm</param>
        /// <returns>Байт в кодировке a-law</returns>
        public static byte ALawEncode(int pcm)
        {
            return pcmToALawMap[pcm & 0xffff];
        }

        /// <summary>
        /// Кодируйте значение pcm в байт a-law
        /// </summary>
        /// <param name="pcm">16-битное значение pcm</param>
        /// <returns>Байт в кодировке a-law</returns>
        public static byte ALawEncode(short pcm)
        {
            return pcmToALawMap[pcm & 0xffff];
        }

        /// <summary>
        /// Кодирование массива значений pcm
        /// </summary>
        /// <param name="data">Массив 16-битных значений pcm</param>
        /// <returns>Массив байтов a-law, содержащий результаты</returns>
        public static byte[] ALawEncode(int[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = ALawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// Кодирование массива значений pcm
        /// </summary>
        /// <param name="data">Массив 16-битных значений pcm</param>
        /// <returns>Массив байтов a-law, содержащий результаты</returns>
        public static byte[] ALawEncode(short[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = ALawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// Кодирование массива значений pcm
        /// </summary>
        /// <param name="data">Массив байтов</param>
        /// <returns>Массив байтов a-law, содержащий результаты</returns>
        public static byte[] ALawEncode(byte[] data)
        {
            int size = data.Length / 2;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = ALawEncode((data[2 * i + 1] << 8) | data[2 * i]);
            return encoded;
        }

        /// <summary>
        /// Кодируем массив значений pcm в предварительно выделенный массив
        /// </summary>
        /// <param name="data">Массив байтов</param>
        /// <param name="target">Предварительно выделенный массив для приема байтов A-law.  
        /// Этот массив должен быть не менее половины размера источника.</param>
        public static void ALawEncode(byte[] data, byte[] target)
        {
            int size = data.Length / 2;
            for (int i = 0; i < size; i++)
                target[i] = ALawEncode((data[2 * i + 1] << 8) | data[2 * i]);
        }
    }
}
