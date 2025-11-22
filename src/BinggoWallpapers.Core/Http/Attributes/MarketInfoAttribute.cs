// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Http.Attributes;

/// <summary>
/// 市场信息特性
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="code">市场代码</param>
/// <param name="cn">市场名称</param>
/// <param name="en">市场描述</param>
/// <param name="flag">国旗表情符号</param>
[AttributeUsage(AttributeTargets.Field)]
public class MarketInfoAttribute(string code, string cn, string en, string flag) : Attribute
{
    /// <summary>
    /// 市场代码
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// 市场名称
    /// </summary>
    public string CN { get; } = cn;

    /// <summary>
    /// 市场描述
    /// </summary>
    public string EN { get; } = en;

    /// <summary>
    /// 国旗表情符号
    /// </summary>
    public string Flag { get; } = flag;
}
