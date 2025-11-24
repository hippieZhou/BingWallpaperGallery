// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Attributes;

namespace BinggoWallpapers.Core.Http.Enums;

/// <summary>
/// 支持的国家/地区市场代码
/// </summary>
[Flags]
public enum MarketCode
{
    [MarketInfo("zh-CN", "中国", "China (Simplified)", "🇨🇳")]
    China,

    [MarketInfo("en-US", "美国", "United States", "🇺🇸")]
    UnitedStates,

    [MarketInfo("en-GB", "英国", "United Kingdom", "🇬🇧")]
    UnitedKingdom,

    [MarketInfo("ja-JP", "日本", "Japan", "🇯🇵")]
    Japan,

    [MarketInfo("de-DE", "德国", "Germany", "🇩🇪")]
    Germany,

    [MarketInfo("fr-FR", "法国", "France", "🇫🇷")]
    France,

    [MarketInfo("es-ES", "西班牙", "Spain", "🇪🇸")]
    Spain,

    [MarketInfo("it-IT", "意大利", "Italy", "🇮🇹")]
    Italy,

    [MarketInfo("ru-RU", "俄罗斯", "Russia", "🇷🇺")]
    Russia,

    [MarketInfo("ko-KR", "韩国", "South Korea", "🇰🇷")]
    SouthKorea,

    [MarketInfo("pt-BR", "巴西", "Brazil", "🇧🇷")]
    Brazil,

    [MarketInfo("en-AU", "澳大利亚", "Australia", "🇦🇺")]
    Australia,

    [MarketInfo("en-CA", "加拿大", "Canada", "🇨🇦")]
    Canada,

    [MarketInfo("en-IN", "印度", "India", "🇮🇳")]
    India
}
