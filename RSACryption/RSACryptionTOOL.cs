using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSACryption
{
    public class RSACryptionTool
    {
        /// <summary>
        /// 生成密钥(xml格式)
        /// </summary>
        /// <param name="privateKey">私钥(公钥)</param>
        /// <param name="publicKey">公钥</param>
        public static void BuildRsaKey(out string privateKey, out string publicKey)
        {
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            privateKey = rsp.ToXmlString(true);
            publicKey = rsp.ToXmlString(false);
        }

        /// <summary>
        /// 获取字符串的hash描述符,返回string
        /// </summary>
        /// <param name="str">原文</param>
        /// <param name="hashDate">hash值</param>
        /// <returns></returns>
        public static bool GetHash(string str, ref string hashDateStr)
        {
            byte[] buffer;
            HashAlgorithm mD5 = HashAlgorithm.Create("MD5");
            buffer = Encoding.GetEncoding("GB2312").GetBytes(str);
            hashDateStr = Convert.ToBase64String(mD5.ComputeHash(buffer));
            return true;
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="inputString">原文</param>
        /// <returns></returns>
        public static string RsaEncrypt(string publicKey, string inputString)
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(inputString);
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(publicKey);
            return Convert.ToBase64String(rsp.Encrypt(bytes, false));
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="bytes">待加密字节数组</param>
        /// <returns></returns>
        public static byte[] RsaEncrypt(string publicKey, byte[] bytes)
        {
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(publicKey);
            return rsp.Encrypt(bytes, false);
        }

        /// <summary>
        /// RSA加密 将数据加密至公用缓存文件中
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="inputStream">待加密数据流对象</param>
        /// <param name="changeProgressBar">更新进度</param>
        /// <returns></returns>
        public async static Task RsaEncrypt(string publicKey, FileStream inputStream,Action<long,long> changeProgressBar)
        {
            await Task.Yield();
            FileStream IntermediateFile = FileHelpTool.RSAEncryptedIntermediateFileWriteFile();
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(publicKey);
            
            long count = inputStream.Length;
            while (count>0)
            {
                byte[] byteDate = new byte[count>117? 117 : count];
                int read = inputStream.Read(byteDate, 0, byteDate.Length);
                byte[] encryptByte = rsp.Encrypt(byteDate, false);
                IntermediateFile.Write(encryptByte, 0, encryptByte.Length);
                IntermediateFile.Flush();
                count -= read;
                changeProgressBar.Invoke(count, inputStream.Length);
            }

            IntermediateFile.Close();
            IntermediateFile.Dispose();
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="inputString">密文</param>
        /// <returns></returns>
        public static string RsaDecrypt(string privateKey, string inputString)
        {
            byte[] bytes = Convert.FromBase64String(inputString);
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(privateKey);
            byte[] resultDate = rsp.Decrypt(bytes, false);
            return Encoding.GetEncoding("GB2312").GetString(resultDate);
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="bytes">密文</param>
        /// <returns></returns>
        public static byte[] RsaDecrypt(string privateKey, byte[] bytes)
        {
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(privateKey);
            byte[] resultDate = rsp.Decrypt(bytes, false);
            return resultDate;
        }

        /// <summary>
        /// RSA解密 将数据解密至公用缓存文件中
        /// </summary>
        /// <param name="publicKey">私钥</param>
        /// <param name="inputStream">加密数据流对象</param>
        /// <param name="changeProgressBar">更新进度</param>
        /// <returns></returns>
        public async static Task RsaDecrypt(string privateKey, FileStream inputStream,Action<long, long> changeProgressBar)
        {
            await Task.Yield();
            FileStream IntermediateFile = FileHelpTool.RSAEncryptedIntermediateFileWriteFile();
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(privateKey);
            long count = inputStream.Length;
            while (count >0)
            {
                byte[] byteDate = new byte[count>128? 128 : count];
                int read = inputStream.Read(byteDate, 0, byteDate.Length);
                byte[] encryptByte = rsp.Decrypt(byteDate, false);
                IntermediateFile.Write(encryptByte, 0, encryptByte.Length);
                IntermediateFile.Flush();
                count -= read;
                changeProgressBar.Invoke(count, inputStream.Length);
            }
            IntermediateFile.Close();
            IntermediateFile.Dispose();
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="key">私钥</param>
        /// <param name="hashDateStr">hash描述符</param>
        /// <param name="returnStr">签名后的字符串</param>
        /// <returns></returns>
        public static bool SignatureFormatter(string privateKey, string hashDateStr, ref string returnStr)
        {
            byte[] hashbyteSignature = Convert.FromBase64String(hashDateStr);
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(privateKey);
            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsp);
            rsaFormatter.SetHashAlgorithm("MD5");
            byte[] resultDate = rsaFormatter.CreateSignature(hashbyteSignature);            //执行签名
            returnStr = Convert.ToBase64String(resultDate);
            return true;
        }

        /// <summary>
        /// RSA签名认证
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="hashDateStr">hash描述符</param>
        /// <param name="inputString">签名后的字符串</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string publicKey, string hashDateStr, string inputString)
        {
            byte[] hashbyteDeformatter = Convert.FromBase64String(hashDateStr);
            byte[] deformatterDate = Convert.FromBase64String(inputString);
            RSACryptoServiceProvider rsp = new RSACryptoServiceProvider();
            rsp.FromXmlString(publicKey);
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsp);
            rsaDeformatter.SetHashAlgorithm("MD5");
            return rsaDeformatter.VerifySignature(hashbyteDeformatter, deformatterDate);
        }
    }
}
