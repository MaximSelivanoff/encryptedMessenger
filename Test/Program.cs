namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Alice = new DiffieHellman(64);
            var Bob = new DiffieHellman(Alice._prime, Alice._generator);
            var b_shared = Bob.GenerateSharedSecret(Alice.PublicKey);
            var a_shared = Alice.GenerateSharedSecret(Bob.PublicKey);
            Console.WriteLine(Alice.PublicKey.ToString());
        }
    }
}