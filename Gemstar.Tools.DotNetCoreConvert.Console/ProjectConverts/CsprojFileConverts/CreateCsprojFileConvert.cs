using System.Xml;

namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile;

/// <summary>
/// 创建.net8.0项目文件转换器
/// </summary>
public class CreateCsprojFileConvert : IConvert
{
    private string _targetProjectPath;
    private string _targetProjectFileName;
    private bool _isWebProject;
    private bool _allowUnsafeBlocks;
    private List<PackageReferenceInfo> _packageReferenceInfos;
    /// <summary>
    /// 创建项目文件转换器实例
    /// </summary>
    /// <param name="targetProjectPath">目标项目存放路径</param>
    /// <param name="isWebProject">是否web项目，如果不是，则认为是dll动态库项目，暂不考虑winform项目，因为不满足信创要求</param>
    /// <param name="allowUnsafeBlocks">是否允许不安全代码，默认为不允许，如果项目有特定的硬件调用要求，需要启用时，可设置</param>
    public CreateCsprojFileConvert(string targetProjectPath, List<PackageReferenceInfo> packageReferenceInfos, bool isWebProject = false, bool allowUnsafeBlocks = false)
    {
        _targetProjectPath = targetProjectPath;
        _targetProjectFileName = Path.Combine(targetProjectPath, $"{Path.GetFileName(targetProjectPath)}.csproj");
        _packageReferenceInfos = packageReferenceInfos;
        _isWebProject = isWebProject;
        _allowUnsafeBlocks = allowUnsafeBlocks;
    }
    public async Task ConvertAsync()
    {
        this.WriteLine($"Create the csproj file: {_targetProjectFileName}");
        //if target project path is not exist, create it
        if (!Directory.Exists(_targetProjectPath))
        {
            Directory.CreateDirectory(_targetProjectPath);
        }
        //create the csproj file, rewrite it if it is exist
        var xml = new XmlDocument();
        {
            var root = xml.CreateElement("Project");
            {
                root.SetAttribute("Sdk", _isWebProject ? "Microsoft.NET.Sdk.Web" : "Microsoft.NET.Sdk");

                var propertyGroup = xml.CreateElement("PropertyGroup");
                {
                    var targetFramework = xml.CreateElement("TargetFramework");
                    targetFramework.InnerText = "net8.0";
                    propertyGroup.AppendChild(targetFramework);

                    var implicitUsings = xml.CreateElement("ImplicitUsings");
                    implicitUsings.InnerText = "enable";
                    propertyGroup.AppendChild(implicitUsings);

                    var nullable = xml.CreateElement("Nullable");
                    nullable.InnerText = "enable";
                    propertyGroup.AppendChild(nullable);

                    var allowUnsafeBlocks = xml.CreateElement("AllowUnsafeBlocks");
                    allowUnsafeBlocks.InnerText = _allowUnsafeBlocks.ToString();
                    propertyGroup.AppendChild(allowUnsafeBlocks);

                    root.AppendChild(propertyGroup);
                }

                var itemGroup = xml.CreateElement("ItemGroup");
                {
                    foreach (var packageReferenceInfo in _packageReferenceInfos)
                    {
                        if (string.IsNullOrWhiteSpace(packageReferenceInfo.HintPath))
                        {
                            var packageReference = xml.CreateElement("PackageReference");
                            packageReference.SetAttribute("Include", packageReferenceInfo.PackageName);
                            if (!string.IsNullOrWhiteSpace(packageReferenceInfo.PackageVersion))
                            {
                                packageReference.SetAttribute("Version", packageReferenceInfo.PackageVersion);
                            }
                            itemGroup.AppendChild(packageReference);
                        }
                        else
                        {
                            var packageReference = xml.CreateElement("Reference");
                            packageReference.SetAttribute("Include", packageReferenceInfo.PackageName);
                            itemGroup.AppendChild(packageReference);
                            var hintPath = xml.CreateElement("HintPath");
                            hintPath.InnerText = packageReferenceInfo.HintPath;
                            packageReference.AppendChild(hintPath);
                        }
                    }
                    root.AppendChild(itemGroup);
                }
                xml.AppendChild(root);
            }
        }
        await File.WriteAllTextAsync(_targetProjectFileName, xml.OuterXml);
    }
}
