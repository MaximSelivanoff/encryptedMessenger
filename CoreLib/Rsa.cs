using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CoreLib
{
    public class Rsa
    {
        Random rnd;
        BigInteger p, q;
        BigInteger N; // = p * q
        BigInteger f; // = (p-1) * (q - 1)
        BigInteger e;
        BigInteger d;
        string Alph;
        static string RuAlph = "АБВГДЕЖЗИЙКЛМНОПРСТУФЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя0123456789/*-+=";
        static string EnAplh = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789/*-+=";
        int[] AlphNumbs;
        public Rsa()
        {
            Alph = EnAplh;
            rnd = new Random();
            AlphNumbs = new int[Alph.Length];
            for (int i = 0; i < Alph.Length; i++)
            {
                AlphNumbs[i] = 10 + i;
            }
        }

        public Rsa(BigInteger p, BigInteger q): this()
        {
            this.p = p;
            this.q = q;
            GenerateKeys(p, q, out this.e, out this.d);
        }
        public Rsa(int generation_size): this()
        {
            this.p = Generate_Random_Prime(generation_size);
            do
            {
                this.q = Generate_Random_Prime(generation_size);
            }
            while (p == q);
            GenerateKeys(p, q, out this.e, out this.d);
        }
        private void GenerateKeys(BigInteger p, BigInteger q, out BigInteger e, out BigInteger d)
        {
            this.N = p * q;
            this.f = (p - 1) * (q - 1);

            // генерация открытого ключа
            do
            {
                e = GenerateBigInt(CountBits(f));
                e = (e + 1) % f;
            }
            while (!MillerRabinTest(e, CountBits(e)) || f % e == 0 || e == p || e == q);

            //генерация закрытого ключа
            BigInteger x = 0, y = 11111; // будет нужен только y
            ExtendedEuclid(f, e, ref x, ref y);
            d = (y + f) % f;
        }
        public BigInteger[] Encode(string message)
        {
            //перевели буквы в числа
            BigInteger[] encodedMessage = new BigInteger[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                int j = 0;
                while (message[i] != Alph[j])
                    j++;
                encodedMessage[i] = ModPow(j, e, N);
            }
            return encodedMessage;
        }
        static public BigInteger[] EncodeWithEnAlph(string message, BigInteger e, BigInteger N)
        {
            //перевели буквы в числа
            BigInteger[] encodedMessage = new BigInteger[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                int j = 0;
                while (message[i] != EnAplh[j])
                    j++;
                encodedMessage[i] = ModPow(j, e, N);
            }
            return encodedMessage;
        }
        static public string EncodeToString(BigInteger[] encodedMessage, string split_symb = "")
        {
            string stringMessage = "";
            for (int i = 0; i < encodedMessage.Length; i++)
            {
                stringMessage += encodedMessage[i].ToString() + split_symb;
            }
            return stringMessage;
        }
        public BigInteger[] Decode(BigInteger[] encodedMessage)
        {
            BigInteger[] decodedMessage = new BigInteger[encodedMessage.Length];
            for (int i = 0; i < encodedMessage.Length; i++)
            {
                decodedMessage[i] = ModPow(encodedMessage[i], d, N) % Alph.Length;
            }
            return decodedMessage;
        }
        public string DecodeToSymb(BigInteger[] decodedMessage)
        {
            string stringMessage = "";
            for (int i = 0; i < decodedMessage.Length; i++)
            {
                if (decodedMessage[i] < Alph.Length)
                    stringMessage += Alph[(int)decodedMessage[i]].ToString();
            }
            return stringMessage;
        }

        public BigInteger Get_p()
        {
            return p;
        }
        public BigInteger Get_q()
        {
            return q;
        }
        public (BigInteger N, BigInteger e) GetPublicKey()
        {
            return (N, e);
        }
        public (BigInteger N, BigInteger d) GetPrivateKey()
        {
            return (N, d);
        }

        public BigInteger Generate_Random_Prime(int generation_size)
        {
            BigInteger new_prime;
            do
            {
                new_prime = GenerateBigInt(generation_size);
            }
            while (!MillerRabinTest(new_prime, generation_size));
            return new_prime;
        }

        static public bool MillerRabinTest(BigInteger n, int k)
        {
            Random rnd = new Random();
            if (n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;
            // представим n − 1 в виде (2^s)·t
            BigInteger t = n - 1;
            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }
            for (int i = 0; i < k; i++)
            {
                byte[] _a = new byte[n.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rnd.NextBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= n - 2);
                BigInteger x = ModPow(a, t, n);
                if (x == 1 || x == n - 1)
                    continue;
                for (int r = 1; r < s; r++)
                {
                    x = ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;
        }
        static public BigInteger GenerateBigInt(int size = 512)
        {
            Random rnd = new Random();
            BigInteger Decimalvalue = 0;
            while (Decimalvalue < 1)
            {
                int lenght = size;
                List<string> raw = Enumerable.Repeat("0", lenght / 2).ToList();
                raw.AddRange(Enumerable.Repeat("1", lenght - raw.Count));

                string result = "";
                for (int i = raw.Count; i > 0; --i)
                {
                    int index = rnd.Next(0, raw.Count);
                    result += raw[index];
                    raw.RemoveAt(index);
                }
                Decimalvalue = 0;
                foreach (char c in result)
                {
                    Decimalvalue <<= 1;
                    Decimalvalue += c == '1' ? 1 : 0;
                }
            }
            return Decimalvalue;
        }
        static public int CountBits(BigInteger number)
        {
            int count = 0;
            while (number != 0)
            {
                number >>= 1;
                count++;
            }
            return count;
        }
        static public BigInteger ExtendedEuclid(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0; y = 1;
                return b;
            }
            BigInteger x1 = 0, y1 = 0;
            BigInteger d = ExtendedEuclid(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }
        static public BigInteger ModPow(BigInteger number, BigInteger exponent, BigInteger modulus)
        {
            BigInteger B, D;
            B = number;
            B %= modulus;
            D = 1;
            if ((exponent & 1) == 1)
            {
                D = B;
            }
            while (exponent > 1)
            {
                exponent >>= 1;
                B = (B * B) % modulus;
                if ((exponent & 1) == 1)
                {
                    D = (D * B) % modulus;
                }
            }
            return D;
        }

    }

}
