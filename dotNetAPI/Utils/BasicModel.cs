using dotNetAPI.DTO;
using dotNetAPI.DTO.VehicleResponse;
using dotNetAPI.DTO.Vehicle.Response;
using dotNetAPI.DTO.Vehicle.Request;
using Microsoft.Net.Http.Headers;
using RestSharp;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using dotNetAPI.Exceptions;

namespace dotNetAPI.Utils
{
    public class BasicModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RestClient _client;

        public BasicModel(IHttpClientFactory httpClientFactory, string url)
        {
            _httpClientFactory = httpClientFactory;
            _client = new RestClient(url);
        }


        public async Task getVehiclesAsync()
        {

            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                "https://mini.rentcar.api.snet.com.pl/vehicles")
            {
                Headers =
            {
                { HeaderNames.Accept, "text/plain" }
            }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.SendAsync(httpRequestMessage);

            httpResponse.EnsureSuccessStatusCode();

            if (httpResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(httpResponse.Content.ToString());
                using var contentStream =
                    await httpResponse.Content.ReadAsStreamAsync();
                Console.WriteLine();
            }
        }

        public VehiclesResponse getVehicles()
        {
            var request = new RestRequest("vehicles", Method.GET);
            request.AddHeader("accept", "text/plain");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            IRestResponse response = _client.Execute(request);

            try
            {
                return JsonSerializer.Deserialize<VehiclesResponse>(response.Content);
            }
            catch (Exception ex)
            {
                return new VehiclesResponse();
            }
        }

        public VehiclePriceResponse getPriceById(VehiclePriceRequest vehiclePriceRequest, string id)
        {
            var request = new RestRequest("vehicle/" + id + "/GetPrice");

            AddPriceBodyToRequest(vehiclePriceRequest, request);

            AddHeadersToRequest(request);

            IRestResponse response = _client.Post(request);
            try
            {
                return JsonSerializer.Deserialize<VehiclePriceResponse>(response.Content);
            }
            catch(Exception ex)
            {
                throw new CarDoesntExistException();
                return new VehiclePriceResponse();
            }
        }

        public VehiclePriceResponse getPriceByBrandAndModel(VehiclePriceRequest vehiclePriceRequest, string brand, string model)
        {
            var request = new RestRequest("vehicle/"+brand+"/"+model+"/GetPrice");

            AddPriceBodyToRequest(vehiclePriceRequest, request);

            AddHeadersToRequest(request);

            IRestResponse response = _client.Post(request);

            try
            {
                return JsonSerializer.Deserialize<VehiclePriceResponse>(response.Content);
            }
            catch(Exception e)
            {
                return new VehiclePriceResponse();
            }
        }

        private void AddPriceBodyToRequest(VehiclePriceRequest vehiclePriceRequest, RestRequest request)
        {
            request.AddJsonBody(new
            {
                age = vehiclePriceRequest.age,
                yearsOfHavingDriverLicense = vehiclePriceRequest.yearsOfHavingDriverLicense,
                rentDuration = vehiclePriceRequest.rentDuration,
                location = vehiclePriceRequest.city + " " + vehiclePriceRequest.country,
                currentlyRentedCount = vehiclePriceRequest.currentlyRentedCount,
                overallRentedCount = vehiclePriceRequest.overallRenetdCount
            });
        }

        public RentVehicleResponse RentCar(RentVehicleRequest rentVehicleRequest, string quoteId)
        {
            var request = new RestRequest("vehicles/Rent/" + quoteId);

            request.AddJsonBody(new
            {
                startDate = rentVehicleRequest.startDate
            });

            AddHeadersToRequest(request);

            IRestResponse response = _client.Post(request);

            try
            {
                return JsonSerializer.Deserialize<RentVehicleResponse>(response.Content);
            }
            catch (Exception e)
            {
                throw new CarDoesntExistException();
            }
        }

        public void ReturnCar(string rentId)
        {
            var request = new RestRequest("vehicle/Return/" + rentId);

            AddHeadersToRequest(request);
            IRestResponse response = _client.Post(request);
        }

        private void AddHeadersToRequest(RestRequest request)
        {
            var tokenString = getToken().access_token;

            request.AddHeader("accept", "text/plain");
            request.AddHeader("Authorization", "Bearer " + tokenString);
            request.AddHeader("Content-Type", "application/json");
        }

        private AuthToken getToken()
        {
            var authClient = new RestClient("https://indentitymanager.snet.com.pl/connect/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=team1b&client_secret=490f00bb-467b-43e4-a3bd-7b2b85c3ddb2", ParameterType.RequestBody);
            IRestResponse response = authClient.Execute(request);
            return JsonSerializer.Deserialize<AuthToken>(response.Content);
        }
    }
}