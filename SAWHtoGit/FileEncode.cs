using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SAWHtoGit
{
    internal class FileEncode
    {
        public static String ConvertFileEncoding(String sourcePath, Encoding destEncoding)
        {

            Encoding sourceEncoding = GetEncoding(sourcePath);
            String fileName = Program.WorkingDir + '\\' + sourcePath.Replace('/', '\\');

            if (File.Exists(fileName + ".ASCII"))
            {
                File.Delete(fileName + ".ASCII");
            }

            // If the source and destination encodings are the same, don't do anything.
            if (sourceEncoding.ToString() == destEncoding.ToString())
            {
                //Don't do anything just return
                System.Console.WriteLine("Encoding is Same. Returning..");
                return fileName;
            }


            // Convert the file.
            String tempName = null;
            try
            {
                tempName = Path.GetTempFileName();
                using (StreamReader sr = new StreamReader(fileName, sourceEncoding, false))
                {
                    using (StreamWriter sw = new StreamWriter(tempName, false, destEncoding))
                    {
                        int charsRead;
                        char[] buffer = new char[128 * 1024];
                        while ((charsRead = sr.ReadBlock(buffer, 0, buffer.Length)) > 0)
                        {
                            sw.Write(buffer, 0, charsRead);
                        }
                    }
                }
               // File.Delete(Program.WorkingDir + '\\' + sourcePath.Replace('/', '\\'));
                File.Move(tempName, fileName + ".ASCII");
                fileName = fileName + ".ASCII";
                return fileName;
            }
            finally
            {
                File.Delete(tempName);
            }
        }


        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(Program.WorkingDir + '\\' + filename.Replace('/', '\\'), FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}