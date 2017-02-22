using System;
using System.Text;
using System.Security.Cryptography;
namespace Sasila.Common.Cryption
{
    /// <summary> 
    /// RSA加解密
    /// </summary> 
    public class RSACryption
    {
        /// <summary> 
        /// RSA加解密
        /// </summary> 
        public RSACryption() { }

        #region RSA 加密

        /// <summary>
        /// RSA 的密钥产生 产生私钥 和公钥 
        /// </summary>
        /// <param name="xmlKeys">私钥</param>
        /// <param name="xmlPublicKey">公钥</param>
        public static void RSAKey(out string xmlKeys, out string xmlPublicKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            xmlKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }

        //############################################################################## 
        //RSA 方式加密 
        //说明KEY必须是XML的行式,返回的是字符串 
        //在有一点需要说明！！该加密方式有 长度 限制的！！ 
        //############################################################################## 

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="strEncrypt">string明文</param>
        /// <returns>暗码</returns>
        public static string RSAEncrypt(string xmlPublicKey, string strEncrypt)
        {
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            PlainTextBArray = (new UnicodeEncoding()).GetBytes(strEncrypt);
            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;

        }

        /// <summary>
        /// 通过pem文件公钥进行加密，一般采用非.net的网站加密都采用此方式
        /// </summary>
        /// <param name="pubKey">公钥</param>
        /// <param name="strEncrypt">明文</param>
        /// <returns>暗码</returns>
        public static string RSAEncryptPemPublicKey(string pubKey, string strEncrypt)
        {
            RSAParameters para = ConvertFromPemPublicKey(pubKey);

            byte[] EncryptedSymmetricData;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = para;
            RSA.ImportParameters(RSAKeyInfo);//导入公钥
            EncryptedSymmetricData = RSA.Encrypt(Encoding.UTF8.GetBytes(strEncrypt), false);//加密
            return Convert.ToBase64String(EncryptedSymmetricData);
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="strEncrypt">byte[]明文</param>
        /// <returns>暗码</returns>
        public static string RSAEncrypt(string xmlPublicKey, byte[] strEncrypt)
        {
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            CypherTextBArray = rsa.Encrypt(strEncrypt, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;

        }

        #endregion

        #region RSA 解密

        public static string RSADecryptPemPrivateKey(string privateKey, string strDncrypt)
        {
            RSAParameters para = ConvertFromPemPrivateKey(privateKey);

            byte[] EncryptedSymmetricData;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = para;
            RSA.ImportParameters(RSAKeyInfo);//导入私钥
            EncryptedSymmetricData = RSA.Decrypt(Convert.FromBase64String(strDncrypt), false);//加密
            return Encoding.UTF8.GetString(EncryptedSymmetricData);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey">xml公钥</param>
        /// <param name="strDncrypt">string暗码</param>
        /// <returns>明文</returns>
        public static string RSADecrypt(string xmlPrivateKey, string strDncrypt)
        {
            byte[] PlainTextBArray;
            byte[] DypherTextBArray;
            string Result;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            PlainTextBArray = Convert.FromBase64String(strDncrypt);
            DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;

        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey">xml公钥</param>
        /// <param name="strDncrypt">byte[]暗码</param>
        /// <returns>明文</returns>
        public static string RSADecrypt(string xmlPrivateKey, byte[] strDncrypt)
        {
            byte[] DypherTextBArray;
            string Result;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            DypherTextBArray = rsa.Decrypt(strDncrypt, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;
        }

        #endregion

        #region RSA PEM格式转化

        /// <summary>
        /// 将pem格式公钥(1024 or 2048)转换为RSAParameters
        /// </summary>
        /// <param name="pemFileConent">pem公钥内容</param>
        /// <returns>转换得到的RSAParamenters</returns>
        public static RSAParameters ConvertFromPemPublicKey(string pemFileConent)
        {
            if (string.IsNullOrEmpty(pemFileConent))
            {
                throw new ArgumentNullException("pemFileConent", "This arg cann't be empty.");
            }
            pemFileConent = pemFileConent.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            bool keySize1024 = (keyData.Length == 162);
            bool keySize2048 = (keyData.Length == 294);
            if (!(keySize1024 || keySize2048))
            {
                throw new ArgumentException("pem file content is incorrect, Only support the key size is 1024 or 2048");
            }
            byte[] pemModulus = (keySize1024 ? new byte[128] : new byte[256]);
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, (keySize1024 ? 29 : 33), pemModulus, 0, (keySize1024 ? 128 : 256));
            Array.Copy(keyData, (keySize1024 ? 159 : 291), pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        /// <summary>
        /// 将pem格式私钥(1024 or 2048)转换为RSAParameters
        /// </summary>
        /// <param name="pemFileConent">pem私钥内容</param>
        /// <returns>转换得到的RSAParamenters</returns>
        public static RSAParameters ConvertFromPemPrivateKey(string pemFileConent)
        {
            if (string.IsNullOrEmpty(pemFileConent))
            {
                throw new ArgumentNullException("pemFileConent", "This arg cann't be empty.");
            }
            pemFileConent = pemFileConent.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "");
            byte[] keyData = Convert.FromBase64String(pemFileConent);

            bool keySize1024 = (keyData.Length == 609 || keyData.Length == 610);
            bool keySize2048 = (keyData.Length == 1190 || keyData.Length == 1192);

            if (!(keySize1024 || keySize2048))
            {
                throw new ArgumentException("pem file content is incorrect, Only support the key size is 1024 or 2048");
            }

            int index = (keySize1024 ? 11 : 12);
            byte[] pemModulus = (keySize1024 ? new byte[128] : new byte[256]);
            Array.Copy(keyData, index, pemModulus, 0, pemModulus.Length);

            index += pemModulus.Length;
            index += 2;
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;
            if ((int)keyData[index] == 0)
            {
                index++;
            }
            byte[] pemPrivateExponent = (keySize1024 ? new byte[128] : new byte[256]);
            Array.Copy(keyData, index, pemPrivateExponent, 0, pemPrivateExponent.Length);

            index += pemPrivateExponent.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemPrime1 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemPrime1, 0, pemPrime1.Length);

            index += pemPrime1.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemPrime2 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemPrime2, 0, pemPrime2.Length);

            index += pemPrime2.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemExponent1 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemExponent1, 0, pemExponent1.Length);

            index += pemExponent1.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemExponent2 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemExponent2, 0, pemExponent2.Length);

            index += pemExponent2.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemCoefficient = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemCoefficient, 0, pemCoefficient.Length);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }

        #endregion
    }
}