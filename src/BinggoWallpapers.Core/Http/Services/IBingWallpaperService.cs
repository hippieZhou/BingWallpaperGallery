// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Http.Services;

/// <summary>
/// 必应壁纸信息收集服务接口
/// </summary>
public interface IBingWallpaperService
{
    /// <summary>
    /// 运行壁纸收集应用
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>收集结果</returns>
    Task<IEnumerable<CollectedWallpaperInfo>> CollectAsync(CancellationToken cancellationToken = default);
}
