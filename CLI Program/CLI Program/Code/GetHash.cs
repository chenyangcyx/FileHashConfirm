using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CLI_Program
{
    class GetHash
    {
        public string GetSHA256Hash(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine(path + "不存在！");
                return null;
            }
            FileInfo fi = new FileInfo(path);
            using(SHA256 sha256 = SHA256.Create())
            {
                FileStream fileStream = fi.OpenRead();
                fileStream.Position = 0;
                byte[] hashValue = sha256.ComputeHash(fileStream);
                fileStream.Close();
                return ConvertHashCodeBytes(hashValue);
            }
        }

        public string GetSHA256Hash(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (SHA256 sha256 = SHA256.Create())
            {
                FileStream fileStream = fi.OpenRead();
                fileStream.Position = 0;
                byte[] hashValue = sha256.ComputeHash(fileStream);
                fileStream.Close();
                return ConvertHashCodeBytes(hashValue);
            }
        }

        public string ConvertHashCodeBytes(byte[] hashvalue)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashvalue.Length; i++)
                builder.Append(hashvalue[i].ToString("X2"));
            return builder.ToString();
        }
    }
}
