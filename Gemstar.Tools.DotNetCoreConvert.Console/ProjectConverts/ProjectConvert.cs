using Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile;
using Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.FileContentConverts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Xml;

namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts;

/// <summary>
/// The class to convert the project
/// </summary>
public class ProjectConvert:IConvert
{
    private string _sourceProjectPath, _targetProjectPath;
    private IServiceProvider _serviceProvider;
    public ProjectConvert(IServiceProvider serviceProvider,string sourceProjectPath, string targetProjectPath)
    {
        _serviceProvider = serviceProvider;
        _sourceProjectPath = sourceProjectPath;
        _targetProjectPath = targetProjectPath;
    }
    /// <summary>
    /// The method to convert the project
    /// </summary>
    public async Task ConvertAsync()
    {
        this.WriteLine($"Convert the project '{_sourceProjectPath}' to '{_targetProjectPath}'");
        var sourceProjectFile = GetSourceProjectFile();
        var frameworkPackageMappingOption = _serviceProvider.GetService<IOptions<FrameworkPackageMappingOption>>().Value;
        var frameworkCsprojFileParser = new FrameworkCsprojFileParser(sourceProjectFile, frameworkPackageMappingOption);
        this.WriteLine("--开始创建项目文件--");
        await CreateCsprojFileAsync(frameworkCsprojFileParser);
        this.WriteLine("--开始拷贝编译文件--");
        await CopyCompileFilesAsync(frameworkCsprojFileParser);
        this.WriteLine("--开始创建其他文件--");
        await CreateOtherFilesAsync();
        this.WriteLine("--开始修改文件内容--");
        await ChangeFileContentsAsync();
        this.WriteLine("Convert the project successfully.");
    }

    /// <summary>
    /// Get the source project file
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private XmlDocument GetSourceProjectFile()
    {
        //find the csproj file in _sourceProjectPath,and load it into XmlDocument
        var csprojFilePath = Directory.GetFiles(_sourceProjectPath, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(csprojFilePath))
        {
            throw new Exception($"The project {_sourceProjectPath} does not contain a .csproj file.");
        }
        var xml = new XmlDocument();
        xml.Load(csprojFilePath);
        return xml;
    }
    private async Task CreateCsprojFileAsync(FrameworkCsprojFileParser frameworkCsprojFileParser)
    {
        var packageReferenceInfos = frameworkCsprojFileParser.ParsePackageReferenceInfos();
        var isWebProject = frameworkCsprojFileParser.ParseOutputType().Equals("Exe", StringComparison.OrdinalIgnoreCase);
        var allowUnsafeBlocks = frameworkCsprojFileParser.ParseAllowUnsafeBlocks();
        var createCsprojFile = new CreateCsprojFileConvert(_targetProjectPath,packageReferenceInfos,isWebProject,allowUnsafeBlocks);
        await createCsprojFile.ConvertAsync();
    }
    private async Task CopyCompileFilesAsync(FrameworkCsprojFileParser frameworkCsprojFileParser)
    {
        var needCopyCompileFiles = frameworkCsprojFileParser.ParseCompileFiles();
        var copyCompileFilesConvert = new CopyCompileFilesConvert(_sourceProjectPath, _targetProjectPath, needCopyCompileFiles);
        await copyCompileFilesConvert.ConvertAsync();
    }
    private async Task CreateOtherFilesAsync()
    {
        var createSystemWebUIConvert = new CreateSystemWebConvert(_targetProjectPath);
        await createSystemWebUIConvert.ConvertAsync();
    }
    private async Task ChangeFileContentsAsync()
    {
        var systemDataSqlClientToMicrosoftDataSqlClientConvert = new SystemDataSqlClientToMicrosoftDataSqlClientConvert(_targetProjectPath,new List<string> { "ADOHelper.cs", "CheckSpaIsPay.cs", "DaJing.cs", "DGuan.cs", "DGuanSql.cs", "GDong.cs", "Genomics.cs", "HDJY.cs", "IstYunos.cs", "JOBO.cs", "NewCapec.cs", "Oealy.cs", "Quickest.cs", "NewCapec.cs", "Test.cs", "XingHuo.cs", "YiEn.cs", "YunAn.cs" });
        await systemDataSqlClientToMicrosoftDataSqlClientConvert.ConvertAsync();
    }
}
