using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACryption
{
    /// <summary>
    /// 文件读写类
    /// </summary>
   public class FileHelpTool
    {
        /// <summary>
        /// 程序所在文件目录 X:\xxx\xxx
        /// </summary>
        public static string BaseProgramPath { get; } = System.Environment.CurrentDirectory;

        /// <summary>
        /// 加密流存储文件名称
        /// </summary>
        public const string FileName = "RSAEncryptedIntermediateFile";

        /// <summary>
        /// 整体文件路径
        /// </summary>
        public static string ProgramPath { get; } =BaseProgramPath + "\\" + FileName;


        /// <summary>
        /// 以只读的方式读取加密存储文件
        /// </summary>
        /// <returns></returns>
        public static FileStream RSAEncryptedIntermediateFileReadFile() {
            FileStream file = new FileStream(ProgramPath, FileMode.Open);
            file.Seek(0, SeekOrigin.Begin);

            return file;
        }

        /// <summary>
        /// 以只写的方式写入加密存储文件
        /// </summary>
        /// <returns></returns> 
        public static FileStream RSAEncryptedIntermediateFileWriteFile()
        {
            FileStream file = new FileStream(ProgramPath, FileMode.Create);
            return file;
        }


        public static void CopyToFile(string copyFilePath,string toCopyFilePath)
        {

            //Copy到新文件下
            FileInfo file = new FileInfo(copyFilePath);
            if (file.Exists)
            {
                //true 覆盖已存在的同名文件，false不覆盖
                file.CopyTo(toCopyFilePath, true);
            }
        }

    }
}
