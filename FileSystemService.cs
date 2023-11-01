using CHFA.OnBaseBackend.Business.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CHFA.OnBaseBackend.Business.Implementations
{
    public class FileSystemService : IFileSystemService
    {
        public async Task WriteFileToPath(string directory, string fileName, byte[] contents)
        {
            // combine path and file name
            string fileNameWithPath = Path.Combine(directory, Path.GetFileName(fileName));

            if (!File.Exists(fileNameWithPath))
            {
                try
                {
                    //write the contents of the byte array to the file
                    using (FileStream _fileStream = new FileStream(fileNameWithPath, FileMode.Create, FileAccess.Write))
                    {
                        _fileStream.Write(contents, 0, contents.Length);
                        _fileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public async Task<byte[]> ReadFileFromPath(string directory, string fileName)
        {
            string _filePath = Path.Combine(directory, fileName);
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Unable to find file at path {_filePath}");
            }

            var _result = File.ReadAllBytes(_filePath);

            return _result;
        }

        public async Task<byte[]> ReadFileFromPath(string fullyJustifiedPath)
        {
           
            if (!File.Exists(fullyJustifiedPath))
            {
                throw new FileNotFoundException($"Unable to find file at path {fullyJustifiedPath}");
            }

            var _result = File.ReadAllBytes(fullyJustifiedPath);

            return _result;
        }

        public async Task DeleteFileAtPath(string directory, string fileName)
        {
            string _filePath = Path.Combine(directory, fileName);

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException(_filePath);
            }
        }

        public async Task<List<string>> GetFileNamesInDirectory(string directoryPath, bool filenameOnly = false)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Unable to find {directoryPath}");
            }

            if (filenameOnly)
            {
                List<string> _raw = Directory.EnumerateFiles(directoryPath).ToList();

                List<string> _trimmed = new List<string>();

                foreach (string aString in _raw)
                {
                    _trimmed.Add(aString.Substring(aString.LastIndexOf("\\")+1));
                }

                return _trimmed;
            }
            else
            {
                return Directory.EnumerateFiles(directoryPath).ToList();
            }
        }


        public async Task DeleteFilesOlderThanDate(string directory, DateTime expireDate)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Unable to find directory {directory}");
            }

            IEnumerable<string> _filesInDirectory = Directory.EnumerateFiles(directory);

            foreach (string _aFile in _filesInDirectory)
            {
                string _fullFilePath = Path.Combine(directory, _aFile);
                DateTime _aFileCreated = File.GetCreationTime(_fullFilePath);
                if(_aFileCreated < expireDate)
                {
                    File.Delete(_fullFilePath);
                }
            }
        }

        public async Task MoveFilesToAnotherFolderWithTimeStamp(string sourcePath, string targetPath)
        {

            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"Unable to find directory {sourcePath}");
            }

            if (!Directory.Exists(targetPath))
            {
                throw new DirectoryNotFoundException($"Unable to find directory {targetPath}");
            }

            //Get all file names
            string[] sourcefiles = Directory.GetFiles(sourcePath);

            //Loop it through to rame and move 
            foreach (string sourcefile in sourcefiles)
            {
                string fileName = Path.GetFileName(sourcefile);

                if (!fileName.Contains("Thumbs"))
                {
                    //Added timestamp to avoid the same file name existing in the target folder.
                    string destFile = Path.Combine(targetPath, DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + fileName);

                    //Move
                    File.Move(sourcefile, destFile);
                }
            }
        }

        public async Task MoveFilesToAnotherFolderWithoutTimeStamp(string sourcePath, string targetPath)
        {

            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"Unable to find directory {sourcePath}");
            }

            if (!Directory.Exists(targetPath))
            {
                throw new DirectoryNotFoundException($"Unable to find directory {targetPath}");
            }

            //Get all file names
            string[] sourcefiles = Directory.GetFiles(sourcePath);

            //Loop it through to rame and move 
            foreach (string sourcefile in sourcefiles)
            {
                string fileName = Path.GetFileName(sourcefile);

                //Added timestamp to avoid the same file name existing in the target folder.
                string destFile = Path.Combine(targetPath, fileName);

                //Move
                File.Move(sourcefile, destFile);
            }
        }


        public async Task <List<string>> SearchForSpreadsheetFileInDirecotry(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Unable to find {directory}");
            }

            return Directory.EnumerateFiles(directory).ToList();
        }

        //public async Task<Dictionary<string, byte[]>> GetLoanNumAndFileBytes(string directory)
        //{
        //    Dictionary<string, byte[]> keyValuePairs = new Dictionary<string, byte[]>();    

        //}
    }
}
