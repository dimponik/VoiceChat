namespace g711audio
{
    /// <summary>
    /// ���������� 16-������ �������� �������� PCM � 8-������ ����� A-law.
    /// </summary>
    public class ALawEncoder
    {
        public const int MAX = 0x7fff; //��������, ������� ����� ��������� � 15 �����

        /// <summary>
        /// ������, � ������� ������-��� 16-������ ���� PCM, 
        /// � ��������-��������� �-������.
        /// </summary>
        private static byte[] pcmToALawMap;

        static ALawEncoder()
        {
            pcmToALawMap = new byte[65536];
            for (int i = short.MinValue; i <= short.MaxValue; i++)
                pcmToALawMap[(i & 0xffff)] = encode(i);
        }

        /// <summary>
        /// �������� ���� ���� a-law �� 16-������� ������ �����
        /// </summary>
        /// <param name="pcm">16-��������� �������� pcm �� ������</param>
        /// <returns>���� � ��������� a-law</returns>
        private static byte encode(int pcm)
        {
            //�������� ���� ����.
            int sign = (pcm & 0x8000) >> 8;
            //���� ����� �������������, ������ ��� ������������� (������ ��� ��������)
            if (sign != 0)
                pcm = -pcm;
            //�������� ������ ��������������� 15 �����, ����� �������� ������������
            if (pcm > MAX) pcm = MAX;

            /* ����� "����������"
             * ����:
             * 1 2 3 4 5 6 7 8 9 A B C D E F G
             * S 7 6 5 4 3 2 1 0 0 0 0 0 0 0 0
             * �� ����� �����, ��� ��������� ������ 1 ����� ���� �����.
             * �� ����� ��������������� �������� �� ������ ������ � �������� �������� ����������.
             * (�. �. ���� ������ 1 � ������� 7 -> ���������� ������� = 2)
             * ���������� ������� ����� 0, ���� 1 �� ������� � ����� �� 2 �� 8.
             * ��� ��������, ��� ���������� ����� 0, ���� ���� ������ 1 �� ����������.
             */
            int exponent = 7;
            //��������� ������ � ��������� ���������� �� ��� ���, ���� �� �� ��������� 1 ��� ���������� �� ��������� 0
            for (int expMask = 0x4000; (pcm & expMask) == 0 && exponent>0; exponent--, expMask >>= 1) { }

            /* ��������� ����� - "��������"
             * ��� ����� ����� ������ ���� ����� 1, ������� �� ������ ��� �����.
             * ����� �������� ��, �� �������� 0x0f :
             * 1 2 3 4 5 6 7 8 9 A B C D E F G
             * S 0 0 0 0 0 1 . . . . . . . . . (��������, ���������� ������� 2)
             * . . . . . . . . . . . . 1 1 1 1
             * �� �������� ��� 5 ��� ��� ����������, ������ ����, ��� ��������
             * �� ������� ���� ������ ���� (���������� + 3) ����.
             * ��� �������� �� �� ����� ���� ������ ������� �����, � ����� � 0x0f.
             * 
             * ���� ���������� ������� ����� 0:
             * 1 2 3 4 5 6 7 8 9 A B C D E F G
             * S 0 0 0 0 0 0 0 Z Y X W V U T S (�� ������ �� ����� � 9 ����)
             * . . . . . . . . . . . . 1 1 1 1
             * �� ����� �������� ZYXW, ��� �������� ����� �� 4 ������ 3
             */
            int mantissa = (pcm >> ((exponent == 0) ? 4 : (exponent + 3))) & 0x0f;

            //������������ ����� �� ������ A-Law ��� SEM (����, ���������� ������� � ��������.)
            byte alaw = (byte)(sign | exponent << 4 | mantissa);

            //�������������� ������ ������ ��� � �������� ��� (0xD5 = 1101 0101)
            return (byte)(alaw^0xD5);
        }

        /// <summary>
        /// �������� �������� pcm � ���� a-law
        /// </summary>
        /// <param name="pcm">16-������ �������� pcm</param>
        /// <returns>���� � ��������� a-law</returns>
        public static byte ALawEncode(int pcm)
        {
            return pcmToALawMap[pcm & 0xffff];
        }

        /// <summary>
        /// ��������� �������� pcm � ���� a-law
        /// </summary>
        /// <param name="pcm">16-������ �������� pcm</param>
        /// <returns>���� � ��������� a-law</returns>
        public static byte ALawEncode(short pcm)
        {
            return pcmToALawMap[pcm & 0xffff];
        }

        /// <summary>
        /// ����������� ������� �������� pcm
        /// </summary>
        /// <param name="data">������ 16-������ �������� pcm</param>
        /// <returns>������ ������ a-law, ���������� ����������</returns>
        public static byte[] ALawEncode(int[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = ALawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// ����������� ������� �������� pcm
        /// </summary>
        /// <param name="data">������ 16-������ �������� pcm</param>
        /// <returns>������ ������ a-law, ���������� ����������</returns>
        public static byte[] ALawEncode(short[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = ALawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// ����������� ������� �������� pcm
        /// </summary>
        /// <param name="data">������ ������</param>
        /// <returns>������ ������ a-law, ���������� ����������</returns>
        public static byte[] ALawEncode(byte[] data)
        {
            int size = data.Length / 2;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = ALawEncode((data[2 * i + 1] << 8) | data[2 * i]);
            return encoded;
        }

        /// <summary>
        /// �������� ������ �������� pcm � �������������� ���������� ������
        /// </summary>
        /// <param name="data">������ ������</param>
        /// <param name="target">�������������� ���������� ������ ��� ������ ������ A-law.  
        /// ���� ������ ������ ���� �� ����� �������� ������� ���������.</param>
        public static void ALawEncode(byte[] data, byte[] target)
        {
            int size = data.Length / 2;
            for (int i = 0; i < size; i++)
                target[i] = ALawEncode((data[2 * i + 1] << 8) | data[2 * i]);
        }
    }
}
