using System;
using System.Text;
using System.IO;


public static class FileTools
{
    /// <summary>
    /// 读取文件
    /// </summary>
    public static string ReadFile(string filePath)
    {
        if (File.Exists(filePath) == false)
            return string.Empty;
        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    public static void CreateFile(string filePath, string content)
    {
        // 删除旧文件
        if (File.Exists(filePath))
            File.Delete(filePath);

        // 创建文件夹路径
        CreateFileDirectory(filePath);

        // 创建新文件
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        using (FileStream fs = File.Create(filePath))
        {
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
        }
    }

    /// <summary>
    /// 创建文件的文件夹路径
    /// </summary>
    public static void CreateFileDirectory(string filePath)
    {
        // 获取文件的文件夹路径
        string directory = Path.GetDirectoryName(filePath);
        CreateDirectory(directory);
    }

    /// <summary>
    /// 创建文件夹路径
    /// </summary>
    public static void CreateDirectory(string directory)
    {
        // If the directory doesn't exist, create it.
        if (Directory.Exists(directory) == false)
            Directory.CreateDirectory(directory);
    }

    /// <summary>
    /// 获取文件大小（字节数）
    /// </summary>
    public static long GetFileSize(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return 0;
        }
        FileInfo fileInfo = new FileInfo(filePath);
        return fileInfo.Length;
    }

    /// <summary>
    /// 从路径的末尾向前截取指定级别的目录
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="levels"></param>
    /// <returns></returns>
    public static string TruncatePath(string fullPath, int levels)
    {
        for (int i = 0; i < levels; i++)
        {
            fullPath = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(fullPath))
                break;
        }

        return fullPath;
    }

    public static string FormatToUnityPath(string path)
    {
        return path.Replace("\\", "/");
    }

    public static string FormatToSysFilePath(string path)
    {
        return path.Replace("/", "\\");
    }
}
