using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    internal class DirectoryInfoHelper
    {
        private DirectoryInfo? _di;
        private long _totalSize;
        private void GetSize(string path)
        {
            DirectoryInfo directoryInfo = new(path);
            // Add files size:
            IEnumerable<FileInfo> files = directoryInfo.GetFiles();
            foreach (FileInfo fi in files)
                _totalSize += fi.Length;
            // Check inner folders:
            IEnumerable<DirectoryInfo> folders = directoryInfo.GetDirectories();
            foreach (DirectoryInfo di in folders)
            {
                try { GetSize(di.FullName); }
                catch (Exception e)
                {
                    Console.WriteLine("Error with " + di.FullName + ": " + e.Message);
                    continue;
                }
            }
        }
        public DirectoryInfoHelper(string path)
        {
            try
            {
                _di = new(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public long GetSize()
        {
            _totalSize = 0;
            if (_di is not null)
                GetSize(_di.FullName);
            return _totalSize;
        }
    }
}
