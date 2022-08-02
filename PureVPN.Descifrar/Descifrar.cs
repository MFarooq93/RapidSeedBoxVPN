using System;
using System.Security.Cryptography;
using System.Text;

namespace PureVPN.Descifrar
{
    public class Manager
    {
        private static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        public static string Decrypt(string base64string, bool IsFromLogin = true)
        {
            try
            {
                if (IsFromLogin)
                    LoadPrivateFromXml();
                else
                    LoadPrivateKeyFromXmlForAuthorization();

                byte[] decMessage = Convert.FromBase64String(base64string);
                byte[] message = PrivateDecryption(decMessage);
                string sMsg = Encoding.UTF8.GetString(message);

                return sMsg;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("An Error occurred while trying to decrypt the data,\nMessage: " + ex.Message);
                throw ex;
            }
        }

        public static string Encrypt(string base64string, bool IsFromLogin = true)
        {
            try
            {
                if (IsFromLogin)
                    LoadPrivateFromXml();
                else
                    LoadPrivateKeyFromXmlForAuthorization();

                byte[] decMessage = Convert.FromBase64String(base64string);
                byte[] message = PrivateEncryption(decMessage);
                string sMsg = Encoding.UTF8.GetString(message);

                return sMsg;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("An Error occurred while trying to decrypt the data,\nMessage: " + ex.Message);
                throw ex;
            }
        }

        private static byte[] PrivateDecryption(byte[] encryptedData)
        {
            var decryptedbyte = rsa.Decrypt(encryptedData, false);
            return decryptedbyte;
        }

        private static byte[] PrivateEncryption(byte[] plainData)
        {
            var decryptedbyte = rsa.Encrypt(plainData, false);
            return decryptedbyte;
        }

        private static void LoadPrivateFromXml()
        {
            try
            {
                rsa.FromXmlString("<RSAKeyValue><Modulus>SAPiP5lgWiOPnGOOuwG9CCYNLYqc+hlXevOdlZuHgKF+bOc7ZXphD1oTbLrrhZUXBplLn0WA7lupvGFqzHUFPkuask5wQbA22m+5R59Q4ofBzsi/R/xfbOfFKQR9MAXje0XJXk7OR92fZjmIqk7g9f1732Lg9bYD9cmRCYMp01M=</Modulus><Exponent>AQAB</Exponent><P>j9UR/LSLhBDCzujMhx848eqJ+Eb8jedztHSALOURoeVIVP/P9Xmfv0URzEsB/Fx//cDvDmfhrNf7WrkbBlC2RQ==</P><Q>gC0d50mnvKAmnbdrN0bmMr4gFtqSb9gOy+1wmNj8igyn2gbq08G95BQsLT+lzabkBidhNG8Z1UwF33HthjXotw==</Q><DP>GmL1bM3WdoD4rCrLMtLIiKEFdLXZKVNXx7hDt3jCtlew4F8Z0KfiZZ2POVJdZ+W0WWImuRbXsPTimO8yhQMTnQ==</DP><DQ>MadIqkySOjQgQYDIfAVvL8EPVUse50zfjohQ0iVcz23PqFp9pcKr+SsHYhAB/wTj2K5wrYtMiqCEtebAiPlhSQ==</DQ><InverseQ>i4nIrk4RDYKpQOuNNakG088vFd/NClM0Og12CawvRV3rvCV5XEkNAkj2nitr+mkBGTYp31EET6nEIw5mG6RsGA==</InverseQ><D>IocqLRAesFkoe/UH9AWrWoLUAqfZB9iqptqzDY4ac8P6V8CuK8N3UeMAOTuNvOh0t8c8CMtMO/xa72VNzWW181vljXH36Y/zClFP7NsL9E5JOeB9DLuYdMzJN6ht/479TpDFFZ3viLoT9SxpX0/Q5DJjW1YjMoE4uYsffwrByVE=</D></RSAKeyValue>");
                RSAParameters rsaParams = rsa.ExportParameters(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception occurred at LoadPrivateFromXml()\nMessage: " + ex.Message);
                throw ex;
            }
        }

        private static void LoadPrivateKeyFromXmlForAuthorization()
        {
            try
            {
                rsa.FromXmlString("<RSAKeyValue><Modulus>zom0Py8fqUKByEzAutCwuYZMMrMtEV09G2c6CXzlke7A/MM0nvOI+z7bNIsB5k5MW9d7w7tddvLdXvcMd3Om2K3bvVI3LotMGmsxnlv/op6Kzaf7KaUCV5FL8GIBiesbV2fNCNx9MWzp4bpCor0vDk7owIj91hAQuesRwNZQYU8=</Modulus><Exponent>AQAB</Exponent><P>/jmPFr67f+OwjJvv78/7ETvN5+Z5jOdkikIwp7fkvelRRg5XiZuxRBR9ivIe4kBN8GbgviI/9n68MYEQsoNrTw==</P><Q>z/rm8AIvFYX/dISx7Fql/AmItsRYDMYrh4TI+BnfGcnyn1xpNscInc5fwX22h2Jl9l5BdeBlODzB4OHe9C4qAQ==</Q><DP>YgVlc3qlJuFusDhr4gZKItHxSWnnEfoiHOD6i6Bu9P6iFKXxAKDkT4CrC0jhuZDbvEaefxELooopG9lMy9e6Jw==</DP><DQ>pno03wswey8OY0tQTFvnH07WY03ZGSiLcWfFomq7HZ58GjnTcmhxKjgmh/BD6izGZyXdiih7fT+NBaBnYdeOAQ==</DQ><InverseQ>zOwVAcEsa8dTianxdR/7jl5L1z/jHg85pNMX2hzcVLXkScxOPSJlMWG7fm0sIvBCQldsSt3RWGGkbWQkgtSs2Q==</InverseQ><D>X+SXh259qLx0PWdFZHdYVgsAfdmq5xD/OiXnUFhoziSn/bM0T0iLJPu+EtsneqKH2Wxmzi3D62I/XcQeSaesor8cj4F923fiM74jPNitMRkdw3LTls1MA4Q1W8w1GiYQgF5c+1XxA+fMzkWfRi2wUJlhUDXsmnlEdZDZOc5+UAE=</D></RSAKeyValue>");
                RSAParameters rsaParams = rsa.ExportParameters(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception occurred at LoadPrivateFromXml()\nMessage: " + ex.Message);
                throw ex;
            }
        }


    }
}
