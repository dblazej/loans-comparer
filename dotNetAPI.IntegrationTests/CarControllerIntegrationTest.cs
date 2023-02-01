using dotNetAPI.DTO;
using dotNetAPI.DTO.VehicleResponse;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace dotNetAPI.IntegrationTests
{
    public class CarControllerIntegrationTest : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;

        public CarControllerIntegrationTest(TestingWebAppFactory<Program> factory)
            => _client = factory.CreateClient();

        [Fact]
        public async Task SuccessfulAddCarAndGetAllCars()
        {
            var requestCar = new AddCarDTO()
            {
                ImageUrl = "image",
                brandName = "Tesla",
                modelName = "Model S",
                year = 2020,
                enginePower = 670,
                enginePowerType = "HP",
                capacity = 5,
                description = "wow",
                price = 50000,
                currency = "PLN"
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestCar);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/car/add");
            postRequest.Content = content;

            var response = await _client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();

            var responseGetAll = await _client.GetAsync("/car/list");

            responseGetAll.EnsureSuccessStatusCode();

            var responseString = await responseGetAll.Content.ReadAsStringAsync();
            var vehiclesResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<VehiclesResponse>(responseString);
            var cars = vehiclesResponse.vehicles;

            Assert.Contains(cars, c => c.brandName == requestCar.brandName && c.modelName == requestCar.modelName);
            Assert.Equal(27, cars.Count);

            System.Diagnostics.Trace.WriteLine($"wynik: {responseString}");

            var requestCar2 = new AddCarDTO()
            {
                ImageUrl = "image",
                brandName = "Tesla",
                modelName = "Model S",
                year = 2021,
                enginePower = 1000,
                enginePowerType = "HP",
                capacity = 5,
                description = "even faster",
                price = 100000,
                currency = "PLN"
            };

            var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(requestCar2);
            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");
            var postRequest2 = new HttpRequestMessage(HttpMethod.Post, "/car/add");
            postRequest2.Content = content2;

            var response2 = await _client.SendAsync(postRequest2);

            response2.EnsureSuccessStatusCode();

            var responseGetAll2 = await _client.GetAsync("/car/list");

            responseGetAll2.EnsureSuccessStatusCode();

            var responseString2 = await responseGetAll2.Content.ReadAsStringAsync();
            var vehiclesResponse2 = Newtonsoft.Json.JsonConvert.DeserializeObject<VehiclesResponse>(responseString2);
            var cars2 = vehiclesResponse2.vehicles;

            Assert.Contains(cars, c => c.brandName == requestCar.brandName && c.modelName == requestCar.modelName);
            Assert.Contains(cars, c => c.brandName == requestCar2.brandName && c.modelName == requestCar2.modelName);
            Assert.Equal(28, cars2.Count);

            System.Diagnostics.Trace.WriteLine($"wynik2: {responseString2}");
        }

    }
}
