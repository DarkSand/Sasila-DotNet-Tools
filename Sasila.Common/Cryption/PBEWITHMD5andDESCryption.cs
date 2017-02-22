using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Sasila.Common.Cryption
{
    /// <summary>
    /// Java下PBEWITHMD5andDES加密
    /// </summary>
    public class PBEWITHMD5andDESCryption
    {
        byte[] key = new byte[8], iv = new byte[8];
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        public byte[] Key { get { return key; } }
        public byte[] IV { get { return iv; } }
        /// <summary>
        /// 加密
        /// </summary>
        public ICryptoTransform Encryptor { get { return des.CreateEncryptor(key, iv); } }
        private ICryptoTransform Decryptor { get { return des.CreateDecryptor(key, iv); } } // 多加一个Decryptor用于解密

        /// <summary>
        /// 初始化PBEWITHMD5andDESCryption
        /// </summary>
        /// <param name="pbeKey">pbeKey</param>
        /// <param name="desKey">desKey</param>
        /// <param name="md5iterations">md5迭代次数</param>
        public PBEWITHMD5andDESCryption(string pbeKey, string desKey, int md5iterations)
        {
            Generate(pbeKey, Encoding.UTF8.GetBytes(desKey), md5iterations, 1);
        }

        private ICryptoTransform Generate(string keystring, byte[] salt, int md5iterations, int segments)
        {
            int HASHLENGTH = 16;    //MD5 bytes
            byte[] keymaterial = new byte[HASHLENGTH * segments]; //to store concatenated Mi hashed results

            // --- get secret password bytes ----
            byte[] psbytes;
            psbytes = Encoding.UTF8.GetBytes(keystring);

            // --- concatenate salt and pswd bytes into fixed data array ---
            byte[] data00 = new byte[psbytes.Length + salt.Length];
            Array.Copy(psbytes, data00, psbytes.Length);  //copy the pswd bytes
            Array.Copy(salt, 0, data00, psbytes.Length, salt.Length);//concatenate the salt bytes

            // ---- do multi-hashing and concatenate results  D1, D2 ...  
            // into keymaterial bytes ----
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = null;
            byte[] hashtarget = new byte[HASHLENGTH + data00.Length];   //fixed length initial hashtarget

            for (int j = 0; j < segments; j++)
            {
                // ----  Now hash consecutively for md5iterations times ------
                if (j == 0) result = data00;       //initialize
                else
                {
                    Array.Copy(result, hashtarget, result.Length);
                    Array.Copy(data00, 0, hashtarget, result.Length, data00.Length);
                    result = hashtarget;
                }

                for (int i = 0; i < md5iterations; i++)
                    result = md5.ComputeHash(result);

                Array.Copy(result, 0, keymaterial, j * HASHLENGTH, result.Length);  //concatenate to keymaterial
            }

            Array.Copy(keymaterial, 0, key, 0, 8);
            Array.Copy(keymaterial, 8, iv, 0, 8);
            return Encryptor;
        }

        /// <summary>
        /// PBEWITHMD5andDESCryption
        /// </summary>
        /// <param name="encryptStr">加密字符串</param>
        /// <returns>加密后字符串</returns>
        public string Encrypt(string encryptStr) 
        {
            byte[] src = Encoding.UTF8.GetBytes(encryptStr);
            byte[] result = this.Encryptor.TransformFinalBlock(src, 0, src.Length);
            return Convert.ToBase64String(result);
        }
    }
}
