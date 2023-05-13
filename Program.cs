using System.Diagnostics.Metrics;
using System.IO;
// В задании требуется вывод на русском, потому весь вывод сделан на русском.
namespace ConsoleApp5
{
    internal class Program
    {
        public static string InputString(string prompt, bool allowEmptyInput = false)
        {
            Console.Write(prompt);
            string? result;
            if (allowEmptyInput)
                result = Console.ReadLine();
            else
                while ((result = Console.ReadLine()) is null || result == string.Empty)
                    Console.Write(prompt);
            return (result is not null ? result : string.Empty);
        }
        public static int fileCounter = 0;
        public static int folderCounter = 0;
        public static void ProcessFolder(DirectoryInfo directoryInfo)
        {
            IEnumerable<DirectoryInfo> folders = directoryInfo.GetDirectories();
            if (folders.Any())
            {
                foreach (var di in folders)
                {
                    if (!di.Exists)
                    {
                        Console.WriteLine("Ошибка: папка " + di.FullName + " не существует");
                        continue;
                    }
                    try { ProcessFolder(di); }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Ошибка: нет доступа к папке" + di.FullName);
                    }
                    finally { }
                }
            }
            else
            {
                IEnumerable<FileInfo> files = directoryInfo.GetFiles();
                foreach (var fi in files)
                {
                    if (!fi.Exists)
                    {
                        Console.WriteLine("Ошибка: файл " + fi.FullName + " не существует");
                        continue;
                    }
                    try
                    {
                        DateTime fileCreatedDate = File.GetLastWriteTime(fi.FullName);
                        TimeSpan difference = DateTime.Now.Subtract(fileCreatedDate);
                        if (difference.TotalMinutes > 30)
                        {
                            File.Delete(fi.FullName);
                            fileCounter++;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Ошикба: нет доступа к файлу " + fi.FullName);
                    }
                    finally { }
                }
                files = directoryInfo.GetFiles();
                if (!files.Any())
                {
                    DateTime folderCreatedDate = Directory.GetLastWriteTime(directoryInfo.FullName);
                    TimeSpan difference = DateTime.Now.Subtract(folderCreatedDate);
                    if (difference.TotalMinutes > 30)
                    {
                        Directory.Delete(directoryInfo.FullName);
                        folderCounter++;
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            string path = (args.Length == 0 ?
                        InputString("Выберите папку для удаления всего, что не использовалось больше 30 минут: ")
                        : args[0]);
            DirectoryInfoHelper helper = new(path);
            long initialSize = helper.GetSize();
            DirectoryInfo directoryInfo = new(path);
            try
            {
                if (directoryInfo.Exists)
                {
                    ProcessFolder(directoryInfo);
                }
                else
                    throw new Exception("Ошибка: папка не существует");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e.ToString());
            }
            finally { }
            long finalSize = helper.GetSize();
            Console.WriteLine("Исходный размер папки: " + initialSize.ToString() + " байт\n" +
                              "Освобождено: " + (initialSize - finalSize).ToString() + " байт\n" +
                              "Удалено " + fileCounter.ToString() + " файлов и " +
                              folderCounter.ToString() + " папок\n" +
                              "Текущий размер папки: " + finalSize.ToString() + " байт");
        }
    }
}