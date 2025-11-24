using System.Text;
using BinggoWallpapers.Collector;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Network;
using BinggoWallpapers.Core.Http.Network.Impl;
using BinggoWallpapers.Core.Http.Options;
using BinggoWallpapers.Core.Http.Services;
using BinggoWallpapers.Core.Http.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// 创建服务容器
var services = new ServiceCollection();

// 配置应用设置
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<IValidateOptions<CollectionOptions>, CollectionOptionsValidator>();
services
    .AddOptionsWithValidateOnStart<CollectionOptions>()
    .BindConfiguration(nameof(CollectionOptions))
    .ValidateDataAnnotations();

// 配置HttpClient
services.AddHttpClient<IBingWallpaperClient, BingWallpaperClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(HTTPConstants.HttpTimeoutSeconds);
    client.DefaultRequestHeaders.Add("User-Agent", HTTPConstants.HttpHeaders.UserAgent);
}).AddStandardResilienceHandler(ResilienceConfiguration.ConfigureStandardResilience);

// 配置日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册服务
services.AddTransient<IBingWallpaperService, BingWallpaperService>();

// 注册应用服务
services.AddTransient<BingWallpaperApp>();

// 构建服务提供者
using var serviceProvider = services.BuildServiceProvider();

// 获取日志器
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("=== 必应壁纸信息收集器启动 ===");
    logger.LogInformation("程序版本: 2.2.0");
    logger.LogInformation("目标框架: .NET 9.0");
    logger.LogInformation("启动时间: {StartTime}", DateTime.Now);
    logger.LogInformation("=============================");

    // 获取应用实例并运行
    var app = serviceProvider.GetRequiredService<BingWallpaperApp>();
    await app.RunAsync();

    logger.LogInformation("程序执行完成，按任意键退出...");

    // 等待用户输入以便查看结果（检测是否为重定向输入）
    if (Console.IsInputRedirected)
    {
        logger.LogInformation("检测到输入重定向，自动退出");
    }
    else
    {
        Console.ReadKey();
    }
}
catch (Exception ex)
{
    logger.LogCritical(ex, "程序发生致命错误: {Message}", ex.Message);
    Console.WriteLine("\n程序异常退出，按任意键关闭...");
    if (!Console.IsInputRedirected)
    {
        Console.ReadKey();
    }

    Environment.Exit(1);
}
finally
{
    logger.LogInformation("程序退出");
}
