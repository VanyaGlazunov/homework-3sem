using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Test1;

public class ParallelCheckSum() : ICheckSum
{
    public byte[] GetCheckSum(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        using var md5 = MD5.Create();
        return this.RecursiveCheckSum(path, md5);
    }

    private byte[] RecursiveCheckSum(string path, MD5 md5)
    {
        using var stream = File.Open(path, FileMode.Open);
        var fileAttributes = File.GetAttributes(path);
        if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            var result = System.Text.Encoding.ASCII.GetBytes(path);
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            var subresults = new List<byte[]>(files.Length + directories.Length);
            Parallel.ForEach(directories, item =>
            {
                subresults[Array.IndexOf(directories, item)] = this.RecursiveCheckSum(item, md5);
            });
            Parallel.ForEach(files, item =>
            {
                subresults[Array.IndexOf(files, item)] = this.RecursiveCheckSum(item, md5);
            });
            foreach (var subresult in subresults)
            {
                result = [.. subresult];
            }

            return result;
        }
        else
        {
            return md5.ComputeHash(stream);
        }
    }
}