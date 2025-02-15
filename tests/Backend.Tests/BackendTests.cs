using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Gripe.Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
namespace Backend.Tests;


public class Tests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [OneTimeSetUp]
    public void Setup()
    {
        _factory = new();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _factory.Dispose();
        _client.Dispose();
    }

    [Test]
    public async Task GetAllUsers()
    {
        var endpoint = "/users/";
        var res = await _client.GetAsync(endpoint);
        var jsonStr = await res.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<List<UserDto>>(jsonStr);
        // THIS DOES NOT WORK - ALL FIELDS ARE BLANK BUT AT LEAST WE CAN GET THE COUNT
        // TestContext.Out.WriteLine($"\nContent:\n\t{string.Join("\n\t", json!)}");
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(json!.Count, Is.EqualTo(3));
    }


}
