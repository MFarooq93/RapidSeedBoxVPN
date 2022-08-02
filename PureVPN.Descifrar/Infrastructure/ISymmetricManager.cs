using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Descifrar.Infrastructure
{
    public interface ISymmetricManager
    {
        /// <summary>
        /// Encrypts <paramref name="textToEncrypt"/>
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <param name="key">Key to encrypt</param>
        /// <returns></returns>
        string Encrypt(string textToEncrypt, string key);

        /// <summary>
        /// Decrypts <paramref name="textToDecrypt"/>
        /// </summary>
        /// <param name="textToDecrypt"></param>
        /// <param name="key">Key to use for decryption</param>
        /// <returns></returns>
        string Decrypt(string textToDecrypt, string key);
    }
}
