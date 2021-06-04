namespace g711audio
{
    /// <summary>
    /// ���������� 16-������ �������� �������� PCM � 8-������ ����� �-������.
    /// </summary>
    public class MuLawEncoder
    {
        public const int BIAS = 0x84; //�� �� 132, ��� 1000 0100
        public const int MAX = 32635; //32767 (������������ 15-������ ����� �����) ����� ��������

        /// <summary>
        /// ������, ����� �� ��� ������� ����� ������������ ��� 2 ������ �����.
        /// �������� pcm, � ������� ���� ����, ��������� � ��������� [32768,33924] (��� �����).
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
        /// ������, � ������� ������ ������������ ����� 16-������ ���� PCM, � ��������
        /// ��������� ������ Mu-law.
        /// </summary>
        private static byte[] pcmToMuLawMap;

        static MuLawEncoder()
        {
            pcmToMuLawMap = new byte[65536];
            for (int i = short.MinValue; i <= short.MaxValue; i++)
                pcmToMuLawMap[(i & 0xffff)] = encode(i);
        }

        /// <summary>
        /// ���������� ���� ���� mu-������ �� 16-������� ������ ����� �� ������.
        /// </summary>
        /// <param name="pcm">16-��������� �������� pcm �� ������</param>
        /// <returns>����, �������������� �� ������ Mu-law</returns>
        private static byte encode(int pcm)
        {
            //�������� ��� �����.  ��������� ��� ��� ������������ ������������� ��� ���������� ���������
            int sign = (pcm & 0x8000) >> 8;
            //���� ����� �������������, ������� ��� �������������
            if (sign != 0)
                pcm = -pcm;
            //�������� ������ ���� ������ 32635, ����� �������� ������������
            if (pcm > MAX) pcm = MAX;
            //������� 132, ����� ������������� 1 � ������ ����� ����� ���� �����
            pcm += BIAS;

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
            for (int expMask = 0x4000; (pcm & expMask) == 0; exponent--, expMask >>= 1) { }

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
             */
            int mantissa = (pcm >> (exponent + 3)) & 0x0f;

            //������������ ����� �� ������ Mu-Law ��� SEM (����, ���������� ������� � ��������.)
            byte mulaw = (byte)(sign | exponent << 4 | mantissa);

            //�������������� ����
            return (byte)~mulaw;
        }

        /// <summary>
        /// �������� �������� pcm � ���� mu-law
        /// </summary>
        /// <param name="pcm">16-������ �������� pcm</param>
        /// <returns>���� � ��������� mu-law</returns>
        public static byte MuLawEncode(int pcm)
        {
            return pcmToMuLawMap[pcm & 0xffff];
        }

        /// <summary>
        /// �������� �������� pcm � ���� mu-law
        /// </summary>
        /// <param name="pcm">16-������ �������� pcm</param>
        /// <returns>���� � ��������� mu-law</returns>
        public static byte MuLawEncode(short pcm)
        {
            return pcmToMuLawMap[pcm & 0xffff];
        }

        /// <summary>
        /// ����������� ������� �������� pcm
        /// </summary>
        /// <param name="data">������ 16-������ �������� pcm</param>
        /// <returns>������ ������ mu-law, ���������� ����������</returns>
        public static byte[] MuLawEncode(int[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = MuLawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// ����������� ������� �������� pcm
        /// </summary>
        /// <param name="data">������ 16-������ �������� pcm</param>
        /// <returns>������ ������ mu-law, ���������� ����������</returns>
        public static byte[] MuLawEncode(short[] data)
        {
            int size = data.Length;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = MuLawEncode(data[i]);
            return encoded;
        }

        /// <summary>
        /// ����������� ������� �������� pcm
        /// </summary>
        /// <param name="data">������ 16-������ �������� pcm</param>
        /// <returns>������ ������ mu-law, ���������� ����������</returns>
        public static byte[] MuLawEncode(byte[] data)
        {
            int size = data.Length / 2;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = MuLawEncode((data[2 * i + 1] << 8) | data[2 * i]);
            return encoded;
        }

        /// <summary>
        /// �������� ������ �������� pcm � �������������� ���������� ������
        /// </summary>
        /// <param name="data">������ ������</param>
        /// <param name="target">�������������� ���������� ������ ��� ������ ������ mu-law. 
        /// ���� ������ ������ ���� �� ����� �������� ������� ���������.</param>
        public static void MuLawEncode(byte[] data, byte[] target)
        {
            int size = data.Length / 2;
            for (int i = 0; i < size; i++)
                target[i] = MuLawEncode((data[2 * i + 1] << 8) | data[2 * i]);
        }
    }
}
