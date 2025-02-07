namespace Gemstar.Tools.DotNetCoreConvert.Console;

/// <summary>
/// 转换器接口扩展类
/// </summary>
public static class IConvertExtension
{
    /// <summary>
    /// 转换器输出信息扩展方法
    /// </summary>
    /// <param name="convert">转换器实例</param>
    /// <param name="message">要输出的信息</param>
    public static void WriteLine(this IConvert convert, string message)
    {
        System.Console.WriteLine(message);
    }
}
