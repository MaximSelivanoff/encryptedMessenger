using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class RC4
    {
        private byte[] S;
        const int sSize = 16;
        public RC4(byte[] key)
        {
            S = new byte[sSize];
            for (int i = 0; i < S.Length; i++)
            {
                S[i] = (byte)i;
            }

            int j = 0;
            for (int i = 0; i < S.Length; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % S.Length;
                Swap(i, j, S);
            }
        }

        private void Swap(int i, int j, byte[] arr)
        {
            byte temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        public byte[] Encrypt(byte[] data)
        {
            byte[] encryptedData = new byte[data.Length];
            var s = new byte[S.Length];
            Array.Copy(S, s, S.Length);
            int i = 0, j = 0;
            for (int k = 0; k < data.Length; k++)
            {
                i = (i + 1) % s.Length;
                j = (j + s[i]) % s.Length;
                Swap(i, j, s);
                var Sa = s[(S[i] + s[j]) % s.Length];
                Console.Write(Sa.ToString() + " ");
                encryptedData[k] = (byte)(data[k] ^ Sa);
            }
            Console.Write("\n");
            return encryptedData;
        }

        public byte[] Decrypt(byte[] data)
        {
            return Encrypt(data);
        }
    }
}
