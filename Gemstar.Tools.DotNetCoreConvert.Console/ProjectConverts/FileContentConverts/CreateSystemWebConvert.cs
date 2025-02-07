namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.FileContentConverts;

/// <summary>
/// 创建兼容System.Web.UI的转换器
/// 由于新的.net 8不再引用 System.Web.UI，所以需要实现一些新类，以兼容原有的代码
/// </summary>
public class CreateSystemWebConvert : IConvert
{
    private string _targetProjectPath;
    public CreateSystemWebConvert(string targetProjectPath)
    {
        _targetProjectPath = targetProjectPath;
    }
    public async Task ConvertAsync()
    {
        await CreateListItemAsync();
        this.WriteLine("创建System.Web.UI.WebControls.ListItem  成功");
        await CreateJavaScriptSerializerAsync();
        this.WriteLine("创建System.Web.Script.Serialization.JavaScriptSerializer  成功");
    }
    /// <summary>
    /// 创建ListItem类，原来的皮卡丘接口程序中有使用到此类
    /// </summary>
    /// <returns></returns>
    private async Task CreateListItemAsync()
    {
        var content = @"
namespace System.Web.UI.WebControls
{
    public class ListItem
    {
        public ListItem()
        {
        }
        public ListItem(string text, string value)
        {
            Text = text;
            Value = value;
        }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
";
        await CreateFile("System/Web/UI/ListItem.cs", content);
    }
    private async Task CreateJavaScriptSerializerAsync()
    {
        var content = @"
using System.Text.Json;
namespace System.Web.Script.Serialization
{
    public class JavaScriptSerializer
    {
        public JavaScriptSerializer()
        {
        }
        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
";
        await CreateFile("System/Web/Script/Serialization/JavaScriptSerializer.cs", content);
    }
    private async Task CreateFile(string filePath, string content)
    {
        filePath = Path.Combine(_targetProjectPath, filePath);
        //检查目录是否存在，不存在则创建
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        await File.WriteAllTextAsync(filePath, content);
    }
}
