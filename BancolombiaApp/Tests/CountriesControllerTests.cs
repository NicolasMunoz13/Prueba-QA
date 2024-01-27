using BancolombiaApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class CountriesControllerTests
{
    private CountriesController _controller;

    [SetUp]
    public void SetUp()
    {
        _controller = new CountriesController();
    }

    [Test]
    public async Task GetTop5DensestCountriesAsync_Returns_Ok()
    {
        var response = await _controller.GetTop5DensestCountriesAsync();
        Assert.That(response.Result, Is.TypeOf<OkObjectResult>());
    }
}
