using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class CryptoAlgorithms
    {
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

        public static BigInteger Generate_Random_Prime(int generation_size)
        {
            BigInteger new_prime;
            do
            {
                new_prime = GenerateBigInt(generation_size);
            }
            while (!MillerRabinTest(new_prime, generation_size));
            return new_prime;
        }


    }
}
