using dotNetAPI.DTO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace dotNetAPI.IntegrationTests
{
    public class UserControllerIntegrationTest : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserControllerIntegrationTest(TestingWebAppFactory<Program> factory)
            => _client = factory.CreateClient();

        [Fact]
        public async Task SuccessfulAddUserAndGetAllUsers()
        {
            var requestUser = new RegisterUserDTO()
            {
                MicrosoftID = "newuser",
                YearOfGettingDriverLicence = 2020,
                YearOfBirth = 2000,
                City = "test City",
                Country = "test Country",
                Email = "test@test.com"
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/user/register");
            postRequest.Content = content;

            var response = await _client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();

            var responseGetAll = await _client.GetAsync("/user/all");

            responseGetAll.EnsureSuccessStatusCode();

            var responseString = await responseGetAll.Content.ReadAsStringAsync();
            var users = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(responseString);

            Assert.Contains(users, u => u.MicrosoftID == requestUser.MicrosoftID);

            //System.Diagnostics.Trace.WriteLine($"wynik: {responseString}");
        }

        [Fact]
        public async Task AddUsedMicrosoftIDAndGetAllUsers()
        {
            var requestUser1 = new RegisterUserDTO()
            {
                MicrosoftID = "newuser",
                YearOfGettingDriverLicence = 2020,
                YearOfBirth = 2000,
                City = "test City",
                Country = "test Country",
                Email = "test@test.com"
            };

            var requestUser2 = new RegisterUserDTO()
            {
                MicrosoftID = "newuser",
                YearOfGettingDriverLicence = 2020,
                YearOfBirth = 2000,
                City = "test City2",
                Country = "test Country2",
                Email = "test2@test.com"
            };

            var json1 = Newtonsoft.Json.JsonConvert.SerializeObject(requestUser1);
            var content1 = new StringContent(json1, Encoding.UTF8, "application/json");
            var postRequest1 = new HttpRequestMessage(HttpMethod.Post, "/user/register");
            postRequest1.Content = content1;

            var response1 = await _client.SendAsync(postRequest1);

            //response.EnsureSuccessStatusCode();


            var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(requestUser1);
            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");
            var postRequest2 = new HttpRequestMessage(HttpMethod.Post, "/user/register");

            var response2 = await _client.SendAsync(postRequest2);

            Assert.False(response2.IsSuccessStatusCode);

            var responseGetAll = await _client.GetAsync("/user/all");

            responseGetAll.EnsureSuccessStatusCode();

            var responseString = await responseGetAll.Content.ReadAsStringAsync();
            var users = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(responseString);

            Assert.Contains(users, u => u.MicrosoftID == requestUser1.MicrosoftID);

            int count = 0;

            foreach (var user in users)
                count++;
            Assert.Equal(1, count);

            //System.Diagnostics.Trace.WriteLine($"wynik: {responseString}");
        }
    }
}