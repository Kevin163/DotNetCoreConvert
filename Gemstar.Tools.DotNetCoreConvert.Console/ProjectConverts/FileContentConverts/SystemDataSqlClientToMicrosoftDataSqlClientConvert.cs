namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.FileContentConverts;

public class SystemDataSqlClientToMicrosoftDataSqlClientConvert : IConvert
{
    private string _targetProjectPath;
    private List<string> _needChangeFiles;
    public SystemDataSqlClientToMicrosoftDataSqlClientConvert(string targetProjectPath, List<string> needChangeFiles)
    {
        _targetProjectPath = targetProjectPath;
        _needChangeFiles = needChangeFiles;
    }
    public async Task ConvertAsync()
    {
        //递归遍历文件夹，如果文件在_needChangeFiles中，则替换其中的System.Data.SqlClient为Microsoft.Data.SqlClient
        await FindFiles(_targetProjectPath);
    }
    private async Task FindFiles(string parent)
    {
        var files = Directory.GetFiles(parent);
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            if (_needChangeFiles.Contains(fileName))
            {
                await ChangeFileContentAsync(file);
            }
        }
        var subDires = Directory.GetDirectories(parent);
        foreach (var subDire in subDires)
        {
            await FindFiles(subDire);
        }
    }
    private async Task ChangeFileContentAsync(string filePath)
    {        
        var content = await File.ReadAllTextAsync(filePath);
        content = content.Replace("System.Data.SqlClient", "Microsoft.Data.SqlClient");
        await File.WriteAllTextAsync(filePath, content);
        this.WriteLine($"修改文件内容：{filePath}  成功");
    }
}
