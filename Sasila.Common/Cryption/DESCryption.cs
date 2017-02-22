using System;
using System.Security.Cryptography;
using System.Text;
namespace Sasila.Common.DEncrypt
{
    /// <summary>
    /// DES�ӽ��ܣ�KEYһ����8λ��
    /// </summary>
    public class DESCryption
    {
        /// <summary>
        /// DES�ӽ���
        /// </summary>
        public DESCryption() { }

        #region DES Ȥ����ܣ�CBCģʽ��

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="Text">����</param>
        /// <returns>����</returns>
        public static string Encrypt(string Text)
        {
            //return Encrypt(Text,"~c^e-s_c#");
            return Encrypt(Text, "cesc");
        }

        /// <summary> 
        /// DES����
        /// </summary> 
        /// <param name="Text">����</param> 
        /// <param name="sKey">�ܳ�</param> 
        /// <returns>����</returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// DES����
        /// </summary>
        /// <param name="Text">����</param>
        /// <returns>����</returns>
        public static string Decrypt(string Text)
        {
            //return Decrypt(Text, "~c^e-s_c#");
            return Decrypt(Text, "cesc");
        }

        /// <summary> 
        /// DES����
        /// </summary> 
        /// <param name="Text">����</param> 
        /// <param name="sKey">�ܳ�</param> 
        /// <returns>����</returns> 
        public static string Decrypt(string Text, string sKey)
        {
            if (string.IsNullOrEmpty(Text)) return "";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region DES IV(CBC)ģʽ����

        /// <summary>
        /// DES8λ��ԿCBC����
        /// </summary>
        /// <param name="encryptStr">���ܵ�����</param>
        /// <param name="encryptKey">8λ����key</param>
        /// <param name="encryptIv">8λ����iv</param>
        /// <returns>����base64�ַ���</returns>
        public static string CBCEncrypt(string encryptStr, byte[] encryptKey, byte[] encryptIv)
        {
            if (string.IsNullOrEmpty(encryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = encryptKey;
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = Encoding.ASCII.GetBytes(encryptStr);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.IV = encryptIv;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DES8λ��ԿCBC����
        /// </summary>
        /// <param name="decryptStr">��Ҫ���ܵ��ַ�����base64��</param>
        /// <param name="decryptKey">8λ����key</param>
        /// <param name="decryptIv">8λ����iv</param>
        /// <returns>����</returns>
        public static string CBCDecrypt(string decryptStr, byte[] decryptKey, byte[] decryptIv)
        {
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = decryptKey;
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.IV = decryptIv;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            byte[] inputByteArray = Convert.FromBase64String(decryptStr);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.ASCII.GetString(ms.ToArray());
        }

        #endregion

        #region DES8λECBģʽͨ�ü���

        /// <summary>
        /// DES8λ��ԿECB����
        /// </summary>
        /// <param name="encryptStr">���ܵ�����</param>
        /// <param name="encryptKey">8λ����key</param>
        /// <returns>����base64�ַ���</returns>
        public static string ECBEncrypt(string encryptStr, string encryptKey)
        {
            if (null == encryptStr || encryptStr.Length == 0)
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(encryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = Encoding.ASCII.GetBytes(encryptStr);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DES8λ��ԿECB����
        /// </summary>
        /// <param name="encryptBts">���ܵ�����</param>
        /// <param name="encryptKey">8λ����key</param>
        /// <returns>����base64�ַ���</returns>
        public static string ECBEncrypt(byte[] encryptBts, string encryptKey)
        {
            if (null == encryptBts || encryptBts.Length == 0)
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(encryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = encryptBts;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DES8λ��ԿECB����
        /// </summary>
        /// <param name="decryptStr">��Ҫ���ܵ�base64�ַ�</param>
        /// <param name="decryptKey">8λ����key</param>
        /// <returns>���ܺ���ֽ�����</returns>
        public static byte[] ECBDecrypt(string decryptStr, string decryptKey)
        {
            if (null == decryptStr || decryptStr.Length == 0)
            {
                return new byte[0];
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(decryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            byte[] inputByteArray = Convert.FromBase64String(decryptStr);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        #endregion
    }
}
