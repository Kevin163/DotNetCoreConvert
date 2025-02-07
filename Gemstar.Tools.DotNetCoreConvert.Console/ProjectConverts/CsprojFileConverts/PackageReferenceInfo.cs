namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile;

/// <summary>
/// 新项目引用包信息
/// </summary>
public class PackageReferenceInfo
{
    /// <summary>
    /// 包名称
    /// </summary>
    public string PackageName { get; set; }
    /// <summary>
    /// 包版本
    /// </summary>
    public string PackageVersion { get; set; }
    /// <summary>
    /// 引用的文件路径
    /// </summary>
    public string HintPath { get; set; }
}
