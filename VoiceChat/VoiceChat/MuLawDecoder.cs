namespace g711audio
{
    /// <summary>
    /// Превращает 16-битные линейные значения PCM в 8-битные байты µ-law.
    /// </summary>
    public static class MuLawDecoder
    {

        private static short[] muLawToPcmMap;

        static MuLawDecoder()
        {
            muLawToPcmMap = new short[256];
            for (byte i = 0; i < byte.MaxValue; i++)
                muLawToPcmMap[i] = decode(i);
        }

        private static short decode(byte mulaw)
        {
            mulaw = (byte)~mulaw;

            int sign = mulaw & 0x80;
            int exponent = (mulaw & 0x70) >> 4;
            int data = mulaw & 0x0f;

            data |= 0x10;
            data <<= 1;
            data += 1;
            data <<= exponent + 2;
            data -= MuLawEncoder.BIAS;
            //Если знаковый бит равен 0, число является положительным. В противном случае-отрицательно.
            return (short)(sign == 0 ? data : -data);
        }

        public static short MuLawDecode(byte mulaw)
        {
            return muLawToPcmMap[mulaw];
        }

        public static short[] MuLawDecode(byte[] data)
        {
            int size = data.Length;
            short[] decoded = new short[size];
            for (int i = 0; i < size; i++)
                decoded[i] = muLawToPcmMap[data[i]];
            return decoded;
        }

        public static void MuLawDecode(byte[] data, out short[] decoded)
        {
            int size = data.Length;
            decoded = new short[size];
            for (int i = 0; i < size; i++)
                decoded[i] = muLawToPcmMap[data[i]];
        }


        public static void MuLawDecode(byte[] data, out byte[] decoded)
        {
            int size = data.Length;
            decoded = new byte[size * 2];
            for (int i = 0; i < size; i++)
            {
                decoded[2 * i] = (byte)(muLawToPcmMap[data[i]] & 0xff);
                decoded[2 * i + 1] = (byte)(muLawToPcmMap[data[i]] >> 8);
            }
        }
    }
}
