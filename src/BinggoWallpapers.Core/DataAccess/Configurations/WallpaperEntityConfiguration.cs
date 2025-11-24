// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BinggoWallpapers.Core.DataAccess.Configurations;

public class WallpaperEntityConfiguration : IEntityTypeConfiguration<WallpaperEntity>
{
    public void Configure(EntityTypeBuilder<WallpaperEntity> builder)
    {
        // 表名和架构
        builder.ToTable("wallpapers", "gallery");

        // 主键配置
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        // 属性配置
        builder.Property(e => e.Hash)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(e => e.MarketCode)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(e => e.ResolutionCode)
            .IsRequired()
            .HasConversion<string>();

        // 日期属性配置
        builder.Property(e => e.ActualDate)
            .IsRequired()
            .HasColumnType("DATE");

        // 信息属性配置
        builder.Property(e => e.InfoJson).IsRequired();

        // 使用 Fluent API 配置 Info 属性为不映射到数据库
        builder.Ignore(e => e.Info);

        // 添加复合索引优化查询性能
        builder.HasIndex(e => new { e.MarketCode, e.ResolutionCode, e.Hash })
            .IsUnique()
            .HasDatabaseName("IX_Wallpapers_MarketCode_ResolutionCode_Hash");

        // 添加单独的日期索引用于按日期排序查询
        builder.HasIndex(e => e.ActualDate)
            .HasDatabaseName("IX_Wallpapers_ActualDate");

        // 添加市场代码索引用于按国家筛选
        builder.HasIndex(e => e.MarketCode)
            .HasDatabaseName("IX_Wallpapers_MarketCode");

        // 添加市场代码索引用于按国家筛选
        builder.HasIndex(e => e.ResolutionCode)
            .HasDatabaseName("IX_Wallpapers_ResolutionCode");
    }
}
