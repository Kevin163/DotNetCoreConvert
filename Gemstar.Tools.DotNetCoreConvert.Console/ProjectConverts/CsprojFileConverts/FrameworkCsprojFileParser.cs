using System.Xml;

namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile;
/// <summary>
/// .net framework项目文件解析器
/// </summary>
public class FrameworkCsprojFileParser
{
    private XmlDocument _frameworkCsprojFileXml;
    private FrameworkPackageMappingOption _frameworkPackageMappingOption;
    public FrameworkCsprojFileParser(XmlDocument frameworkCsprojFileXml, FrameworkPackageMappingOption frameworkPackageMappingOption)
    {
        _frameworkCsprojFileXml = frameworkCsprojFileXml;
        _frameworkPackageMappingOption = frameworkPackageMappingOption;
    }
    /// <summary>
    /// 解析项目文件中的包引用信息
    /// </summary>
    /// <param name="csprojFilePath">项目文件路径</param>
    /// <returns>包引用信息</returns>
    public List<PackageReferenceInfo> ParsePackageReferenceInfos()
    {
        var packageReferenceInfos = new List<PackageReferenceInfo>();
        //add required packages
        packageReferenceInfos.AddRange(_frameworkPackageMappingOption.RequiredPackages);
        //add mapping packages
        var packageReferenceNodes = SelectNodes("ItemGroup", "Reference");
        foreach (XmlNode packageReferenceNode in packageReferenceNodes)
        {
            var packageNameInfos = packageReferenceNode?.Attributes?["Include"]?.Value?.Split(',');
            var packageName = packageNameInfos?[0];
            if (!string.IsNullOrWhiteSpace(packageName) && _frameworkPackageMappingOption.PackageMappings.ContainsKey(packageName))
            {
                var mappingPackages = _frameworkPackageMappingOption.PackageMappings[packageName];
                foreach (var mappingPackage in mappingPackages)
                {
                    packageReferenceInfos.Add(mappingPackage);
                }
            }
        }
        return packageReferenceInfos;
    }
    public string ParseOutputType()
    {
        var outputTypeNode = SelectNodes("PropertyGroup", "OutputType");
        if(outputTypeNode != null && outputTypeNode.Count > 0)
        {
            return outputTypeNode[0].InnerText;
        }
        return "Library";
    }
    public bool ParseAllowUnsafeBlocks()
    {
        var allowUnsafeBlocksNode = SelectNodes("PropertyGroup", "AllowUnsafeBlocks");
        return allowUnsafeBlocksNode != null && allowUnsafeBlocksNode.Count > 0 && bool.TryParse(allowUnsafeBlocksNode[0].InnerText, out var allowUnsafeBlocks) && allowUnsafeBlocks;
    }
    public List<string> ParseCompileFiles()
    {
        var compileFiles = new List<string>();
        var compileNodes = SelectNodes("ItemGroup", "Compile");
        foreach (XmlNode compileNode in compileNodes)
        {
            var include = compileNode?.Attributes?["Include"]?.Value;
            if (!string.IsNullOrWhiteSpace(include))
            {
                compileFiles.Add(include);
            }
        }
        return compileFiles;
    }
    private XmlNodeList? SelectNodes(params string[] nodeNames)
    {
        var xpath = $"//{string.Join("/", nodeNames.Select(n => $"msb:{n}"))}";
        var namespaceManager = new XmlNamespaceManager(_frameworkCsprojFileXml.NameTable);
        namespaceManager.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
        return _frameworkCsprojFileXml.SelectNodes(xpath, namespaceManager);
    }
}
