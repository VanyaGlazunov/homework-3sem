using System.Security.Cryptography;
using System.Text;

namespace Test1;

public class NonParallelCheckSum : ICheckSum
{
    public byte[] GetCheckSum(string path)
    {
       using var md5 = MD5.Create();
       return RecursiveCheckSum(path, md5);
    }

    private static byte[] RecursiveCheckSum(string path, MD5 md5)
    {
        using var stream = File.Open(path, FileMode.Open);
        var fileAttributes = File.GetAttributes(path);
        if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            var result = Encoding.ASCII.GetBytes(path);
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            Array.Sort(directories);
            Array.Sort(files);
            foreach (var dir in directories)
            {
                result = [.. RecursiveCheckSum(dir, md5)];
            }

            foreach (var file in files)
            {
                result = [.. RecursiveCheckSum(file, md5)];
            }

            return result;
        }
        else
        {
            return md5.ComputeHash(stream);
        }
    }
}