using System;
using System.Numerics;
using CoreLib;

public class DiffieHellman
{
    private readonly int _keySize;
    private readonly BigInteger _prime;
    private readonly BigInteger _generator;
    private readonly Random _rng;
    private readonly BigInteger _privateKey;
    private readonly BigInteger _publicKey;

    public DiffieHellman(int keySize = 16)
    {
        _keySize = keySize;
        _rng = new Random();

        // Генерация простого числа и примитивного корня
        _prime = CryptoAlgorithms.Generate_Random_Prime(keySize);
        _generator = GenerateGenerator();

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

    private BigInteger GenerateGenerator()
    {
        // Генерация примитивного корня
        BigInteger generator;
        do
        {
            byte[] bytes = new byte[_keySize / 8];
            _rng.NextBytes(bytes);
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