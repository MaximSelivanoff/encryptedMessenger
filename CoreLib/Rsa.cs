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
            this.p = CryptoAlgorithms.Generate_Random_Prime(generation_size);
            do
            {
                this.q = CryptoAlgorithms.Generate_Random_Prime(generation_size);
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
                e = CryptoAlgorithms.GenerateBigInt(CryptoAlgorithms.CountBits(f));
                e = (e + 1) % f;
            }
            while (!CryptoAlgorithms.MillerRabinTest(e, CryptoAlgorithms.CountBits(e)) || f % e == 0 || e == p || e == q);

            //генерация закрытого ключа
            BigInteger x = 0, y = 11111; // будет нужен только y
            CryptoAlgorithms.ExtendedEuclid(f, e, ref x, ref y);
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
                encodedMessage[i] = CryptoAlgorithms.ModPow(j, e, N);
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
                encodedMessage[i] = CryptoAlgorithms.ModPow(j, e, N);
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
                decodedMessage[i] = CryptoAlgorithms.ModPow(encodedMessage[i], d, N) % Alph.Length;
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
    }

}
