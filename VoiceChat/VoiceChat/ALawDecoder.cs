namespace g711audio
{
    /// <summary>
    /// ���������� 8-������ ����� A-law ������� � 16-������ �������� PCM.
    /// </summary>
    public static class ALawDecoder
    {
        /// <summary>
        /// ������, � ������� ������ �������� ������� ��������� ������ "A-law",
        /// � ��������-16-������ ����������� PCM.
        /// </summary>
        private static short[] aLawToPcmMap;

        static ALawDecoder()
        {
            aLawToPcmMap = new short[256];
            for (byte i = 0; i < byte.MaxValue; i++)
                aLawToPcmMap[i] = decode(i);
        }

        /// <summary>
        /// ����������� ���� ���� �-������.
        /// </summary>
        /// <param name="alaw">�������������� ���� �-������</param>
        /// <returns>�������� ���������, ���������� 16-������ ���������</returns>
        private static short decode(byte alaw)
        {
            //����������� ������ ������ ��� � �������� ��� (0xD5 = 1101 0101)
            alaw ^= 0xD5;

            //�������� �������� ���� �����
            int sign = alaw & 0x80;
            //�������� �������� ����������
            int exponent = (alaw & 0x70) >> 4;
            //��������� ������ ���� ������
            int data = alaw & 0x0f;

            //�������� ������ �� ������ ���� �����
            data <<= 4;
            //�������� 8, ����� ��������� ��������� � �������� ���������
            data += 8;

            //���� ���������� �� ����� 0, �� �� �����, ��� ������ ���� ������� �� 1,
            //����� �������, ����� �������� ������� 1 � 0x100
            if (exponent != 0)
                data += 0x100;
            if (exponent > 1)
                data <<= (exponent - 1);

            return (short)(sign == 0 ? data : -data);
        }

        /// <summary>
        /// ���������� ���� ���� A-law
        /// </summary>
        /// <param name="alaw">�������������� ���� �-������</param>
        /// <returns>�������� ���������, ���������� 16-������ ���������</returns>
        public static short ALawDecode(byte alaw)
        {
            return aLawToPcmMap[alaw];
        }

        /// <summary>
        /// ���������� ������ ������, �������������� �� ������ �
        /// </summary>
        /// <param name="data">������ ������, �������������� �� ������ �</param>
        /// <returns>������ short, ���������� ����������</returns>
        public static short[] ALawDecode(byte[] data)
        {
            int size = data.Length;
            short[] decoded = new short[size];
            for (int i = 0; i < size; i++)
                decoded[i] = aLawToPcmMap[data[i]];
            return decoded;
        }

        /// <summary>
        /// ���������� ������ ������, �������������� �� ������ �
        /// </summary>
        /// <param name="data">������ ������, �������������� �� ������ �</param>
        /// <param name="decoded">������ short, ���������� ����������</param>
        public static void ALawDecode(byte[] data, out short[] decoded)
        {
            int size = data.Length;
            decoded = new short[size];
            for (int i = 0; i < size; i++)
                decoded[i] = aLawToPcmMap[data[i]];
        }

        /// <summary>
        /// ���������� ������ ������, �������������� �� ������ �
        /// </summary>
        /// <param name="data">������ ������, �������������� �� ������ �</param>
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
