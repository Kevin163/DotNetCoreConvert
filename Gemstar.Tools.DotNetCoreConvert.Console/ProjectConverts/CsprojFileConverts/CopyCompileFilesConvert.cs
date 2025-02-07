namespace Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile
{
    /// <summary>
    /// Copy compile files
    /// </summary>
    public class CopyCompileFilesConvert : IConvert
    {
        private string _sourceProjectPath, _targetProjectPath;
        private List<string> _needCopyCompileFiles;
        public CopyCompileFilesConvert(string sourceProjectPath, string targetProjectPath, List<string> needCopyCompileFiles)
        {
            _sourceProjectPath = sourceProjectPath;
            _targetProjectPath = targetProjectPath;
            _needCopyCompileFiles = needCopyCompileFiles;
        }
        public Task ConvertAsync()
        {
            this.WriteLine($"一共需要拷贝{_needCopyCompileFiles.Count}个文件");
            /*
             * 注：文件名称的格式是PalmVein\Maxvision\MaxvisionSignHelper.cs
             * 1 拼接文件路径时需要注意\和/的问题
             * 2 拷贝目标文件夹不存在时需要创建
             * 3 如果是特殊目录Service References下的文件，暂时先跳过，后面确定方案后再处理
             * */
            foreach (var fileName in _needCopyCompileFiles)
            {
                var copyResult = "";
                try
                {
                    if (fileName.StartsWith("Service References\\") || fileName.StartsWith("Web References\\"))
                    {
                        copyResult = "跳过";
                    }
                    else
                    {
                        var sourceFilePath = Path.Combine(_sourceProjectPath, fileName);
                        var targetFilePath = Path.Combine(_targetProjectPath, fileName);
                        var targetDirectory = Path.GetDirectoryName(targetFilePath);
                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }
                        File.Copy(sourceFilePath, targetFilePath, true);
                        copyResult = "成功";
                    }
                }
                catch (Exception ex)
                {
                    copyResult = $"失败：{ex.Message}";
                }
                this.WriteLine($"拷贝文件：{fileName}  {copyResult}");
            }
            return Task.CompletedTask;
        }
    }
}
