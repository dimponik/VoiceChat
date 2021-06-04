namespace g711audio
{
    /// <summary>
    /// Превращает 8-битные байты A-law обратно в 16-битные значения PCM.
    /// </summary>
    public static class ALawDecoder
    {
        /// <summary>
        /// Массив, в котором индекс является входным значением закона "A-law",
        /// а значение-16-битным результатом PCM.
        /// </summary>
        private static short[] aLawToPcmMap;

        static ALawDecoder()
        {
            aLawToPcmMap = new short[256];
            for (byte i = 0; i < byte.MaxValue; i++)
                aLawToPcmMap[i] = decode(i);
        }

        /// <summary>
        /// Декодируйте один байт а-закона.
        /// </summary>
        /// <param name="alaw">Закодированный байт а-закона</param>
        /// <returns>Короткое сообщение, содержащее 16-битный результат</returns>
        private static short decode(byte alaw)
        {
            //Инвертируем каждый второй бит и знаковый бит (0xD5 = 1101 0101)
            alaw ^= 0xD5;

            //Получаем значение бита знака
            int sign = alaw & 0x80;
            //Сдвигаем значение экспоненты
            int exponent = (alaw & 0x70) >> 4;
            //Извлекаем четыре бита данных
            int data = alaw & 0x0f;

            //Сдвинаем данные на четыре бита влево
            data <<= 4;
            //Добавьте 8, чтобы поместить результат в середину диапазона
            data += 8;

            //Если показатель не равен 0, то мы знаем, что четыре бита следуют за 1,
            //таким образом, можем добавить неявный 1 с 0x100
            if (exponent != 0)
                data += 0x100;
            if (exponent > 1)
                data <<= (exponent - 1);

            return (short)(sign == 0 ? data : -data);
        }

        /// <summary>
        /// Декодируем один байт A-law
        /// </summary>
        /// <param name="alaw">Закодированный байт а-закона</param>
        /// <returns>Короткое сообщение, содержащее 16-битный результат</returns>
        public static short ALawDecode(byte alaw)
        {
            return aLawToPcmMap[alaw];
        }

        /// <summary>
        /// Декодируем массив байтов, закодированных по закону а
        /// </summary>
        /// <param name="data">Массив байтов, закодированных по закону а</param>
        /// <returns>Массив short, содержащий результаты</returns>
        public static short[] ALawDecode(byte[] data)
        {
            int size = data.Length;
            short[] decoded = new short[size];
            for (int i = 0; i < size; i++)
                decoded[i] = aLawToPcmMap[data[i]];
            return decoded;
        }

        /// <summary>
        /// Декодируем массив байтов, закодированных по закону а
        /// </summary>
        /// <param name="data">Массив байтов, закодированных по закону а</param>
        /// <param name="decoded">Массив short, содержащий результаты</param>
        public static void ALawDecode(byte[] data, out short[] decoded)
        {
            int size = data.Length;
            decoded = new short[size];
            for (int i = 0; i < size; i++)
                decoded[i] = aLawToPcmMap[data[i]];
        }

        /// <summary>
        /// Декодируем массив байтов, закодированных по закону а
        /// </summary>
        /// <param name="data">Массив байтов, закодированных по закону а</param>
        /// <param name="decoded"></param>
        public static void ALawDecode(byte[] data, out byte[] decoded)
        {
            int size = data.Length;
            decoded = new byte[size * 2];
            for (int i = 0; i < size; i++)
            {
                decoded[2 * i] = (byte)(aLawToPcmMap[data[i]] & 0xff);
                decoded[2 * i + 1] = (byte)(aLawToPcmMap[data[i]] >> 8);
            }
        }
    }
}
