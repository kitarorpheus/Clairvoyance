public class FolderSizeCalculator
{
    public static long GetDirectorySize(string path)
    {
        long totalSize = 0;

        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
                totalSize += file.Length;

            foreach (DirectoryInfo subDir in dir.GetDirectories())
                totalSize += GetDirectorySize(subDir.FullName);
        }
        catch
        {
            // skip
        }

        return totalSize;
    }

    public static string FormatSize(long size)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double readableSize = size;
        int unitIndex = 0;
        
        while (readableSize >= 1024 && unitIndex < units.Length - 1)
        {
            readableSize /= 1024;
            unitIndex++;
        }

        return $"{readableSize:0.##} {units[unitIndex]}";
    }
}
