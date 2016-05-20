using System.IO;
using System.IO.Compression;

namespace DBRestorer
{
    public static class UnZipHelper
    {
        /// <summary>
        /// Used to specify what our overwrite policy
        /// is for files we are extracting.
        /// </summary>
        public enum Overwrite
        {
            Always,
            IfNewer,
            Never
        }

        /// <summary>
        /// Unzips the specified file to the given folder in a safe
        /// manner.  This plans for missing paths and existing files
        /// and handles them gracefully.
        /// </summary>
        /// <param name="sourceArchiveFileName">
        /// The name of the zip file to be extracted
        /// </param>
        /// <param name="destinationDirectoryName">
        /// The directory to extract the zip file to
        /// </param>
        /// <param name="overwriteMethod">
        /// Specifies how we are going to handle an existing file.
        /// The default is IfNewer.
        /// </param>
        public static void ExtractToDirectory(string sourceArchiveFileName,
                                                      string destinationDirectoryName,
                                                      Overwrite overwriteMethod = Overwrite.IfNewer)
        {
            //Opens the zip file up to be read
            using (ZipArchive archive = ZipFile.OpenRead(sourceArchiveFileName))
            {
                //Loops through each file in the zip file
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    ExtractToFile(file, destinationDirectoryName, overwriteMethod);
                }
            }
        }

        /// <summary>
        /// Safely extracts a single file from a zip file
        /// </summary>
        /// <param name="file">
        /// The zip entry we are pulling the file from
        /// </param>
        /// <param name="destinationPath">
        /// The root of where the file is going
        /// </param>
        /// <param name="overwriteMethod">
        /// Specifies how we are going to handle an existing file.
        /// The default is Overwrite.IfNewer.
        /// </param>
        public static void ExtractToFile(ZipArchiveEntry file,
                                                 string destinationPath,
                                                 Overwrite overwriteMethod = Overwrite.IfNewer)
        {
            //Gets the complete path for the destination file, including any
            //relative paths that were in the zip file
            string destinationFileName = Path.Combine(destinationPath, file.FullName);

            //Gets just the new path, minus the file name so we can create the
            //directory if it does not exist
            string destinationFilePath = Path.GetDirectoryName(destinationFileName);

            //Creates the directory (if it doesn't exist) for the new path
            Directory.CreateDirectory(destinationFilePath);

            //Determines what to do with the file based upon the
            //method of overwriting chosen
            switch (overwriteMethod)
            {
                case Overwrite.Always:
                    //Just put the file in and overwrite anything that is found
                    file.ExtractToFile(destinationFileName, true);
                    break;
                case Overwrite.IfNewer:
                    //Checks to see if the file exists, and if so, if it should
                    //be overwritten
                    if (!File.Exists(destinationFileName) || File.GetLastWriteTime(destinationFileName) < file.LastWriteTime)
                    {
                        //Either the file didn't exist or this file is newer, so
                        //we will extract it and overwrite any existing file
                        file.ExtractToFile(destinationFileName, true);
                    }
                    break;
                case Overwrite.Never:
                    //Put the file in if it is new but ignores the 
                    //file if it already exists
                    if (!File.Exists(destinationFileName))
                    {
                        file.ExtractToFile(destinationFileName);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
