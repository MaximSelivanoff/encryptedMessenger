using System.Text;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Alice = new DiffieHellman(8);
            var Bob = new DiffieHellman(Alice._prime, Alice._generator);
            var b_shared = Bob.GenerateSharedSecret(Alice.PublicKey);
            var a_shared = Alice.GenerateSharedSecret(Bob.PublicKey);


            //var k = CoreLib.PhoPollarDiscrLog.DiskLog(Alice._prime, Alice._generator, Alice.PublicKey);

            //var k = CoreLib.PhoPollarDiscrLog.DiskLog(3,15, 43);
            var k = CoreLib.PhoPollarDiscrLog3.Solve(916, 307, 179);

            //var k = CoreLib.PhoPollarDiscrLog3.Solve(3, 5, 7);

            //var coder = new CoreLib.RC4(new byte[] {1, 2 ,3 ,4, 5, 6});
            //var encoded = coder.Encrypt(new byte[] { 23, 234,4, 23 });
            //Console.WriteLine(string.Join(' ', encoded));
            //var decoded = coder.Decrypt(encoded);
            //Console.WriteLine(string.Join(' ', decoded));
        }
    }
}