namespace g711audio
{
    /// <summary>
    /// Превращает 16-битные линейные значения PCM в 8-битные байты µ-закона.
    /// </summary>
    public class MuLawEncoder
    {
        public const int BIAS = 0x84; //он же 132, или 1000 0100
        public const int MAX = 32635; //32767 (максимальное 15-битное целое число) минус смещение

        /// <summary>
        /// Задает, будут ли все нулевые байты закодированы как 2 вместо этого.
        /// Значения pcm, о которых идет речь, находятся в диапазоне [32768,33924] (без знака).
        /// </summary>
        public bool ZeroTrap
        {
            get { return (pcmToMuLawMap[33000] != 0); }
            set
            {
                byte val = (byte)(value ? 2 : 0);
                for (int i = 32768; i <= 33924; i++)
                    pcmToMuLawMap[i] = val;
            }
        }

        /// <summary>
        /// Массив, в котором индекс представляет собой 16-битный вход PCM, а значение
        /// результат закона Mu-law.
        /// </summary>
        private static byte[] pcmToMuLawMap;

        static MuLawEncoder()
        {
            pcmToMuLawMap = new byte[65536];
            for (int i = short.MinValue; i <= short.MaxValue; i++)
                pcmToMuLawMap[(i & 0xffff)] = encode(i);
        }

        /// <summary>
        /// Закодируем один байт mu-закона из 16-битного целого числа со знаком.
        /// </summary>
        /// <param name="pcm">16-разрядное значение pcm со знаком</param>
        /// <returns>Байт, закодированный по закону Mu-law</returns>
        private static byte encode(int pcm)
        {
            //Получаем бит знака.  Переносим его для последующего использования без дальнейших изменений
            int sign = (pcm & 0x8000) >> 8;
            //Если число отрицательное, сделаем его положительным
            if (sign != 0)
                pcm = -pcm;
            //Величина должна быть меньше 32635, чтобы избежать переполнения
            if (pcm > MAX) pcm = MAX;
            //Добавим 132, чтобы гарантировать 1 в восьми битах после бита знака
            pcm += BIAS;

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
            for (int expMask = 0x4000; (pcm & expMask) == 0; exponent--, expMask >>= 1) { }

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
             */
            int mantissa = (pcm >> (exponent + 3)) & 0x0f;

            //Расположение битов по закону Mu-Law это SEM (Знак, показатель степени и Мантисса.)
            byte mulaw = (byte)(sign | exponent << 4 | mantissa);

            //Переворачиваем биты
            return (byte)~mulaw;
        }

        /// <summary>
        /// Кодируем значение pcm в байт mu-law
        /// </summary>
        /// <param name="pcm">16-битное значение pcm</param>
        /// <returns>Байт в кодировке mu-law</returns>
        public static byte MuLawEncode(int pcm)
        {
            return pcmToMuLawMap[pcm & 0xffff];
        }

        /// <summary>
        /// Кодируем значение pcm в байт mu-law
        /// </summary>
        /// <param name="pcm">16-битное значение pcm</param>
        /// <returns>Байт в кодировке mu-law</returns>
        public static byte MuLawEncode(short pcm)
        {
            return pcmToMuLawMap[pcm & 0xffff];
        }

        /// <summary>
        /// Кодирование массива значений pcm
        /// </summary>
        /// <param name="data">Массив 16-битных значений pcm</param>
        /// <returns>Массив байтов mu-law, содержащий результаты</returns>
        public static byte[] MuLawEncode(int[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = MuLawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// Кодирование массива значений pcm
        /// </summary>
        /// <param name="data">Массив 16-битных значений pcm</param>
        /// <returns>Массив байтов mu-law, содержащий результаты</returns>
        public static byte[] MuLawEncode(short[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = MuLawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// Кодирование массива значений pcm
        /// </summary>
        /// <param name="data">Массив 16-битных значений pcm</param>
        /// <returns>Массив байтов mu-law, содержащий результаты</returns>
        public static byte[] MuLawEncode(byte[] data)
        {
            int size = data.Length / 2;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = MuLawEncode((data[2 * i + 1] << 8) | data[2 * i]);
            return encoded;
        }

        /// <summary>
        /// Кодируем массив значений pcm в предварительно выделенный массив
        /// </summary>
        /// <param name="data">Массив байтов</param>
        /// <param name="target">Предварительно выделенный массив для приема байтов mu-law. 
        /// Этот массив должен быть не менее половины размера источника.</param>
        public static void MuLawEncode(byte[] data, byte[] target)
        {
            int size = data.Length / 2;
            for (int i = 0; i < size; i++)
                target[i] = MuLawEncode((data[2 * i + 1] << 8) | data[2 * i]);
        }
    }
}
