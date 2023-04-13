using System;
using System.Collections.Generic;
using System.Numerics;
using CoreLib;

public class DiffieHellman
{
    private readonly int _keySize;
    public BigInteger _prime { get; }
    public BigInteger _generator { get;}
    private readonly Random _rng;
    private readonly BigInteger _privateKey;
    private readonly BigInteger _publicKey;

    public DiffieHellman(int keySize = 48)
    {
        _keySize = keySize;
        _rng = new Random();

        // Генерация простого числа и примитивного корня
        _prime = CryptoAlgorithms.Generate_Random_Prime(keySize);
        _generator = findPrimitive(_prime);
        //_generator = findPrimitive(_prime);
        _privateKey = GeneratePrivateKey();
        _publicKey = GeneratePublicKey();
    }
    public DiffieHellman(BigInteger p, BigInteger g, int keySize = 16)
    {
        _keySize = keySize;
        _rng = new Random();
        _prime = p;
        _generator = g;
        //_generator = findPrimitive(_prime);
        _privateKey = GeneratePrivateKey();
        _publicKey = GeneratePublicKey();
    }
    public BigInteger PublicKey
    {
        get { return _publicKey; }
    }
    public BigInteger GenerateSharedSecret(BigInteger otherPublicKey)
    {
        // Вычисление общего секретного ключа
        BigInteger sharedSecret = BigInteger.ModPow(otherPublicKey, _privateKey, _prime);
        return sharedSecret;
    }
    private BigInteger GeneratePublicKey()
    {
        // Вычисление открытого ключа
        BigInteger publicKey = BigInteger.ModPow(_generator, _privateKey, _prime);
        return publicKey;
    }

    private BigInteger GeneratePrivateKey()
    {
        // Генерация случайного секретного ключа
        byte[] bytes = new byte[_keySize / 8];
        _rng.NextBytes(bytes);
        BigInteger privateKey = new BigInteger(bytes);

        // Убедимся, что секретный ключ меньше, чем простое число
        if (privateKey >= _prime)
        {
            privateKey %= _prime - 1;
        }
        privateKey = BigInteger.Abs(privateKey);
        return privateKey;
    }
    static void findPrimefactors(List<BigInteger> s, BigInteger n)
    {
        for (BigInteger i = 3; i*i <= n; i++)
        {
            while (n % i == 0)
            {
                s.Add(i);
                n = n / i;
            }
        }
        if (n > 2)
        {
            s.Add(n);
        }
    }
    static int findPrimitive(BigInteger n)
    {
        var s = new List<BigInteger>();

        if (CryptoAlgorithms.MillerRabinTest(n, CryptoAlgorithms.CountBits(n)) == false)
        {
            return -1;
        }

        var phi = n - 1;

        findPrimefactors(s, phi);

        for (int r = 2; r <= phi; r++)
        {
            bool flag = false;
            foreach (BigInteger a in s)
            {
                if (BigInteger.ModPow(r, phi / (a), n) == 1)
                {
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                return r;
            }
        }
        return -1;
    }
}