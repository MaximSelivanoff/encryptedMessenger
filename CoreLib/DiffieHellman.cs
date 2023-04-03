using System;
using System.Security.Cryptography;
using System.Numerics;
using CoreLib;

class DiffieHellman
{
    private readonly int _keySize;
    private readonly BigInteger _prime;
    private readonly BigInteger _generator;
    private readonly RNGCryptoServiceProvider _rng;

    public DiffieHellman(int keySize = 256)
    {
        _keySize = keySize;
        _rng = new RNGCryptoServiceProvider();

        // Генерация простого числа и примитивного корня
        _prime = GeneratePrime();
        _generator = GenerateGenerator();
    }

    public BigInteger GeneratePrivateKey()
    {
        // Генерация случайного секретного ключа
        byte[] bytes = new byte[_keySize / 8];
        _rng.GetBytes(bytes);
        BigInteger privateKey = new BigInteger(bytes);

        // Убедимся, что секретный ключ меньше, чем простое число
        if (privateKey >= _prime)
        {
            privateKey %= _prime - 1;
        }

        return privateKey;
    }

    public BigInteger GeneratePublicKey(BigInteger privateKey)
    {
        // Вычисление открытого ключа
        BigInteger publicKey = BigInteger.ModPow(_generator, privateKey, _prime);
        return publicKey;
    }

    public BigInteger GenerateSharedSecret(BigInteger privateKey, BigInteger otherPublicKey)
    {
        // Вычисление общего секретного ключа
        BigInteger sharedSecret = BigInteger.ModPow(otherPublicKey, privateKey, _prime);
        return sharedSecret;
    }

    private BigInteger GeneratePrime()
    {
        // Генерация случайного простого числа длиной _keySize бит
        BigInteger prime;
        do
        {
            byte[] bytes = new byte[_keySize / 8];
            _rng.GetBytes(bytes);
            prime = new BigInteger(bytes);
            prime = BigInteger.Abs(prime);
            prime = BigIntegerExtensions.NextPrime(prime);
        } while (CryptoAlgorithms.MillerRabinTest(prime, CryptoAlgorithms.CountBits(prime)));

        return prime;
    }

    private BigInteger GenerateGenerator()
    {
        // Генерация примитивного корня
        BigInteger generator;
        do
        {
            byte[] bytes = new byte[_keySize / 8];
            _rng.GetBytes(bytes);
            generator = new BigInteger(bytes);
            generator = BigInteger.Abs(generator);
        } while (!IsPrimitiveRoot(generator));

        return generator;
    }

    private bool IsPrimitiveRoot(BigInteger candidate)
    {
        // Проверка, является ли число candidate примитивным корнем
        for (int i = 2; i < _prime - 1; i++)
        {
            if (BigInteger.ModPow(candidate, i, _prime) == 1)
            {
                return false;
            }
        }

        return true;
    }
}

// Вспомогательный класс для поиска следующего простого числа
public static class BigIntegerExtensions
{
    public static BigInteger NextPrime(BigInteger number)
    {
        while (!CryptoAlgorithms.MillerRabinTest(number, CryptoAlgorithms.CountBits(number)))
        {
            number++;
        }
        return number;
    }
}