namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile;

/// <summary>
/// .net framework项目引用包映射选项
/// </summary>
public class FrameworkPackageMappingOption
{
    /// <summary>
    /// 引用包映射
    /// </summary>
    public Dictionary<string, List<PackageReferenceInfo>> PackageMappings { get; set; }
    /// <summary>
    /// 必须引用的包
    /// </summary>
    public List<PackageReferenceInfo> RequiredPackages { get; set; }
}
