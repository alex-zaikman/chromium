using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace updater
{
    class CheckSum
    {


        public static bool deserialize(string srcFile, string destFile, string md5String)
        {

            File.Create(destFile);

            //file to string
            //string to byte
            string sfile = File.ReadAllText(srcFile, Encoding.Default);
            //md5
            if (!CheckSum.verifyMd5Hash(sfile, md5String.Trim()))
            {
                return false;
            }

            byte[] buf = Encoding.Default.GetBytes(sfile);
            //byte to file (without last postfix)
            ByteArrayToFile(destFile, buf);
            return true;
        }


        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            using (MD5 md5Hasher = MD5.Create())
            {

                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        // Verify a hash against a string.
        public static bool verifyMd5Hash(string input, string ihash)
        {

            // Hash the input.
            string hashOfInput ;

            using (MD5 md5 = MD5.Create())
            {
                byte[] stream = File.ReadAllBytes(input);

                byte[] hash = md5.ComputeHash(stream);

                hashOfInput = Convert.ToBase64String(hash, 0, hash.Length);
            }
            
            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, ihash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            using (FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                long numBytes = new FileInfo(fileName).Length;
                buff = br.ReadBytes((int)numBytes);
                return buff;
            }
        }

        public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                using (System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {

                    // Writes a block of bytes to this stream using data from a byte array.
                    _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                    // close file stream
                    // _FileStream.Close();
                }
                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

    
    }
}
