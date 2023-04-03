using CoreLib;
using static Server.core.Server;

namespace Server.core

{
    public class RsaKeyExchange
    {
        public static string GenKeyMessForClient(LogHandler logHandler)
        {
            var nonce = CryptoAlgorithms.GenerateBigInt(256);
            var nonceHash = Account.GetHashMD5(nonce.ToString());

            var rsa = new Rsa(256);
            (var N, var e) = rsa.GetPublicKey();
            var encodedNonceHash = Rsa.EncodeWithEnAlph(nonceHash, e, N);
            var encodedNonceHashString = Rsa.EncodeToString(encodedNonceHash);


            var resultString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.RsaKeyExchange,
                                                        nonce.ToString(),
                                                        encodedNonceHashString,
                                                        N.ToString(),
                                                        e.ToString());

            logHandler(ServerLogMessages.RsaKeyExchangeDataSended(nonce.ToString(), 
                                                                    encodedNonceHashString, 
                                                                    N.ToString(), 
                                                                    e.ToString()));
            return resultString;
        }
    }
}
