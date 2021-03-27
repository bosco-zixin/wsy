using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WSY.Common
{
    /// <summary>
    /// DES加密/解密类。
    /// </summary>
    public class DESEncrypt
    {
        /// <summary>
        /// 加密报文数据
        /// </summary>
        /// <param name="content">待加密的报文</param>
        /// <returns>密文</returns>
        public static string EncryptDEC(string content, string key)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("明文为空!");
            }

            byte[] BKey = new byte[8];
            byte[] BIV = new byte[8];
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            byte[] bytes = Convert.FromBase64String(key);
            Buffer.BlockCopy(bytes, 0, BKey, 0, 8);
            Buffer.BlockCopy(bytes, 8, BIV, 0, 8);
            byte[] encrypt = Encoding.UTF8.GetBytes(content);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5Hash = md5.ComputeHash(encrypt, 0, encrypt.Length);
            byte[] totalByte = CombineBytes(md5Hash, encrypt);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(BKey, BIV), CryptoStreamMode.Write))
                {
                    cs.Write(totalByte, 0, totalByte.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密报文数据
        /// </summary>
        /// <param name="ciphertext">待解密的报文</param>
        /// <returns>明文</returns>
        public static string DecryptDEC(string ciphertext, string key)
        {
            if (string.IsNullOrEmpty(ciphertext))
            {
                throw new Exception("密文为空!");
            }

            ciphertext = ciphertext.Replace(" ", "+");
            byte[] BKey = new byte[8];
            byte[] BIV = new byte[8];
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            byte[] bytes = Convert.FromBase64String(key);
            Buffer.BlockCopy(bytes, 0, BKey, 0, 8);
            Buffer.BlockCopy(bytes, 8, BIV, 0, 8);

            byte[] totalByte = null;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(ciphertext);
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(BKey, BIV), CryptoStreamMode.Write))
                {
                    cs.Write(inData, 0, inData.Length);
                    cs.FlushFinalBlock();
                    totalByte = ms.ToArray();
                }
            }

            if (totalByte.Length <= 16)
            {
                throw new Exception("密文格式错误!");
            }

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5Hash = md5.ComputeHash(totalByte, 16, totalByte.Length - 16);

            for (int i = 0; i < md5Hash.Length; i++)
            {
                if (md5Hash[i] != totalByte[i])
                {
                    throw new Exception("Md5校验失败!");
                }
            }

            return Encoding.UTF8.GetString(totalByte, 16, totalByte.Length - 16);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes1"></param>
        /// <param name="bytes2"></param>
        /// <returns></returns>
        private static byte[] CombineBytes(byte[] bytes1, byte[] bytes2)
        {
            int len = bytes1.Length + bytes2.Length;
            byte[] lenArr = new byte[len];
            bytes1.CopyTo(lenArr, 0);
            bytes2.CopyTo(lenArr, bytes1.Length);
            return lenArr;
        }

        /// <summary>
        /// MD5普通加密
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetMd5Code(string code)
        {
            var hash = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(code);
            var MD5str = hash.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (var c in MD5str)
            {
                sb.Append(c.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
