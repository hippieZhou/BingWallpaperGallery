// Copyright (c) hippieZhou. All rights reserved.

using System.Net;
using System.Text;
using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Network.Impl;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Tests.Http.Network;

/// <summary>
/// BingWallpaperClient 契约测试
/// 验证 IBingWallpaperClient 接口的契约行为
/// </summary>
public class BingWallpaperClientContractTests
{
    private readonly Mock<ILogger<BingWallpaperClient>> _mockLogger;

    public BingWallpaperClientContractTests()
    {
        _mockLogger = new Mock<ILogger<BingWallpaperClient>>();
    }

    [Fact]
    public async Task GetWallpapersAsync_WithValidParameters_ShouldReturnWallpapers()
    {
        // Arrange
        var count = 3;
        var marketCode = MarketCode.UnitedStates;
        var resolution = ResolutionCode.UHD4K;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        var expectedResponse = CreateMockBingApiResponse();
        var responseContent = await Json.StringifyAsync(expectedResponse);

        mockHandler.SetupResponse(HttpStatusCode.OK, responseContent);
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        var result = await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedResponse.Images.Count);
    }

    [Fact]
    public async Task GetWallpapersAsync_ShouldSetCorrectHttpHeaders()
    {
        // Arrange
        var count = 1;
        var marketCode = MarketCode.China;
        var resolution = ResolutionCode.FullHD;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        var expectedResponse = CreateMockBingApiResponse();
        var responseContent = await Json.StringifyAsync(expectedResponse);

        mockHandler.SetupResponse(HttpStatusCode.OK, responseContent);
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        var capturedRequest = mockHandler.CapturedRequest;
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Get);
        capturedRequest.Headers.Should().ContainKey("Accept-Language");
        capturedRequest.Headers.Should().ContainKey("User-Agent");
        capturedRequest.Headers.Should().ContainKey("Accept");
        capturedRequest.Headers.Should().ContainKey("Cache-Control");

        var acceptLanguageHeader = string.Join(",", capturedRequest.Headers.GetValues("Accept-Language"));
        acceptLanguageHeader.Should().Contain("zh-CN");
        acceptLanguageHeader.Should().Contain("en");
        acceptLanguageHeader.Should().Contain("q=0.9");

        var userAgentHeader = string.Join(" ", capturedRequest.Headers.GetValues("User-Agent"));
        userAgentHeader.Should().Be(HTTPConstants.HttpHeaders.UserAgent);

        var acceptHeader = string.Join(",", capturedRequest.Headers.GetValues("Accept"));
        acceptHeader.Should().Contain("application/json");
        acceptHeader.Should().Contain("text/html");

        capturedRequest.Headers.GetValues("Cache-Control").First().Should().Be(HTTPConstants.HttpHeaders.CacheControl);
    }

    [Fact]
    public async Task GetWallpapersAsync_ShouldBuildCorrectApiUrl()
    {
        // Arrange
        var count = 2;
        var marketCode = MarketCode.Japan;
        var resolution = ResolutionCode.HD;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        var expectedResponse = CreateMockBingApiResponse();
        var responseContent = await Json.StringifyAsync(expectedResponse);

        mockHandler.SetupResponse(HttpStatusCode.OK, responseContent);
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        var capturedRequest = mockHandler.CapturedRequest;
        capturedRequest.Should().NotBeNull();
        var requestUri = capturedRequest!.RequestUri;
        requestUri.Should().NotBeNull();
        requestUri!.ToString().Should().StartWith("https://global.bing.com/HPImageArchive.aspx");
        requestUri.ToString().Should().Contain("format=js");
        requestUri.ToString().Should().Contain("idx=0");
        requestUri.ToString().Should().Contain("n=2");
        requestUri.ToString().Should().Contain("setmkt=ja-JP");
        requestUri.ToString().Should().Contain("setlang=ja");
        requestUri.ToString().Should().Contain("uhdwidth=1920");
        requestUri.ToString().Should().Contain("uhdheight=1200");
    }

    [Fact]
    public async Task GetWallpapersAsync_WithHttpRequestException_ShouldReturnEmptyCollection()
    {
        // Arrange
        var count = 1;
        var marketCode = MarketCode.UnitedStates;
        var resolution = ResolutionCode.Standard;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        mockHandler.SetupException(new HttpRequestException("Network error"));
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        var result = await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWallpapersAsync_WithInvalidJson_ShouldReturnEmptyCollection()
    {
        // Arrange
        var count = 1;
        var marketCode = MarketCode.UnitedStates;
        var resolution = ResolutionCode.Standard;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        var invalidJson = "{ invalid json }";
        mockHandler.SetupResponse(HttpStatusCode.OK, invalidJson);
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        var result = await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWallpapersAsync_WithHttpErrorResponse_ShouldReturnEmptyCollection()
    {
        // Arrange
        var count = 1;
        var marketCode = MarketCode.UnitedStates;
        var resolution = ResolutionCode.Standard;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        mockHandler.SetupResponse(HttpStatusCode.InternalServerError, "Server Error");
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        var result = await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWallpapersAsync_WithEmptyResponse_ShouldReturnEmptyCollection()
    {
        // Arrange
        var count = 1;
        var marketCode = MarketCode.UnitedStates;
        var resolution = ResolutionCode.Standard;
        var cancellationToken = CancellationToken.None;

        var mockHandler = new MockHttpMessageHandler();
        var emptyResponse = new BingApiResponse { Images = [] };
        var responseContent = await Json.StringifyAsync(emptyResponse);
        mockHandler.SetupResponse(HttpStatusCode.OK, responseContent);
        var httpClient = new HttpClient(mockHandler);
        var client = new BingWallpaperClient(httpClient, _mockLogger.Object);

        // Act
        var result = await client.GetWallpapersAsync(count, marketCode, resolution, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    private static BingApiResponse CreateMockBingApiResponse()
    {
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);
        var tomorrow = today.AddDays(1);

        return new BingApiResponse
        {
            Images = new List<BingWallpaperInfo>
            {
                new()
                {
                    Url = "/th?id=OHR.TestImage1_1920x1080.jpg",
                    UrlBase = "/th?id=OHR.TestImage1",
                    Copyright = "Test Copyright 1",
                    CopyrightOnly = "Test 1",
                    CopyrightLink = "https://test1.com",
                    Title = "Test Title 1",
                    BsTitle = "Test BsTitle 1",
                    Caption = "Test Caption 1",
                    Desc = "Test Description 1",
                    Quiz = "Test Quiz 1",
                    Wp = true,
                    Hash = "test-hash-1",
                    Drk = 0,
                    Top = 0,
                    Bot = 0,
                    Hs = Array.Empty<object>(),
                    StartDate = DateOnly.FromDateTime(today),
                    FullStartDate = today,
                    EndDate = DateOnly.FromDateTime(tomorrow),
                    Date = today.ToString("yyyyMMdd")
                },
                new()
                {
                    Url = "/th?id=OHR.TestImage2_1920x1080.jpg",
                    UrlBase = "/th?id=OHR.TestImage2",
                    Copyright = "Test Copyright 2",
                    CopyrightOnly = "Test 2",
                    CopyrightLink = "https://test2.com",
                    Title = "Test Title 2",
                    BsTitle = "Test BsTitle 2",
                    Caption = "Test Caption 2",
                    Desc = "Test Description 2",
                    Quiz = "Test Quiz 2",
                    Wp = false,
                    Hash = "test-hash-2",
                    Drk = 1,
                    Top = 1,
                    Bot = 1,
                    Hs = Array.Empty<object>(),
                    StartDate = DateOnly.FromDateTime(yesterday),
                    FullStartDate = yesterday,
                    EndDate = DateOnly.FromDateTime(today),
                    Date = yesterday.ToString("yyyyMMdd")
                }
            },
            Tooltips = new { }
        };
    }

    /// <summary>
    /// Mock HTTP message handler for testing
    /// </summary>
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        private Exception? _exception;
        public HttpRequestMessage? CapturedRequest { get; private set; }

        public void SetupResponse(HttpStatusCode statusCode, string content)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }

        public void SetupException(Exception exception)
        {
            _exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CapturedRequest = request;

            if (_exception != null)
            {
                throw _exception;
            }

            return Task.FromResult(_response ?? new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}

