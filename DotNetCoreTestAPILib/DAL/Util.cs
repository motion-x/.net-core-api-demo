using System;
using System.IO;

namespace DotNetCoreTestAPILib.DAL
{
    public static class Util
    {
        /// <summary>
        /// Loads a file into an expandable memory stream.
        /// </summary>
        /// <param name="path">Relative or absolute file path</param>
        /// <returns>Expandable memory stream with the contents of the given file.</returns>
        public static MemoryStream ReadFileToStream(string path)
        {
            if (File.Exists(path))
            {
                using (var temp = new MemoryStream(File.ReadAllBytes(path)))
                {
                    // copy to a new ms to make expandable.
                    var ms = new MemoryStream();
                    temp.CopyTo(ms);

                    return ms;
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
