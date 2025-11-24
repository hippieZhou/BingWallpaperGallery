// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Http.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ResolutionInfoAttribute(string name, string suffix, int width, int height) : Attribute
{
    public string Name { get; set; } = name;
    public string Suffix { get; set; } = suffix;
    public int Width { get; } = width;
    public int Height { get; } = height;
}
