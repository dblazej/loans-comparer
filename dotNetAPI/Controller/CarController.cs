using dotNetAPI.DTO;
using dotNetAPI.DTO.Vehicle.Request;
using dotNetAPI.DTO.Vehicle.Response;
using dotNetAPI.DTO.VehicleResponse;
using dotNetAPI.Service;
using Microsoft.AspNetCore.Mvc;
using dotNetAPI.Exceptions;

namespace dotNetAPI.Controllers
{
    [ApiController]
    [Route("car")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ICarStatusService _carStatusService;

        public CarController(ICarService carService, ICarStatusService carHistoryService)
        {
            _carService = carService;
            _carStatusService = carHistoryService;
        }

        [HttpGet("list")]
        public ActionResult<VehiclesResponse> GetAllCars()
        {
            return Ok(_carService.GetAllCars());
        }

        [HttpPost("add")]
        public IActionResult AddCar(AddCarDTO addCarDTO)
        {
            _carService.AddCar(addCarDTO);

            return StatusCode(200);
        }

        [HttpPost("{id}/price")]
        public ActionResult<VehiclePriceResponse> GetCarPriceById(VehiclePriceFrontendRequest request, string id)
        {
            try
            {
                return Ok(_carService.GetCarPriceById(request, id));
            }
            catch(Exception ex)
            {
                return StatusCode(404);
            }
        }

        [HttpPost("{brand}/{model}/price")]
        public ActionResult<VehiclePriceResponse> GetCarPriceByBrandAndModel(VehiclePriceFrontendRequest request, string brand, string model)
        {
            try
            {
                return Ok(_carService.GetCarPriceByBrandAndModel(request, brand, model));
            }
            catch (Exception ex)
            {
                return StatusCode(404);
            }
        }

        [HttpPost("rent/{quoteId}")]
        public ActionResult<RentVehicleResponse> RentCar(RentVehicleRequest rentVehicleRequest, string quoteId, [FromHeader(Name = "id")] string MicrosoftID)
        {
            try
            {
                var result = _carService.RentCar(rentVehicleRequest, quoteId, MicrosoftID);
                return Ok(result);
            }
            catch(CarDoesntExistException ex)
            {
                return StatusCode(404);
            }
            catch(CarIsRentedException ex)
            {
                return StatusCode(404);
            }                
            

        }

        [HttpPost("return/{rentId}")]
        public IActionResult ReturnCar(string rentId)
        {
            try
            {
                _carService.ReturnCar(rentId);
            }
            catch (Exception ex) { return StatusCode(500);}

            return StatusCode(200);
        }

        [HttpGet("rented/history")]
        public List<CarRentalDTO> GetHistoryOfRentedCars([FromHeader(Name = "id")] string MicrosoftID)
        {
            return _carStatusService.GetHistoryOfCarRentals(MicrosoftID);
        }

        [HttpGet("rented")]
        public List<CarRentalDTO> GetRentedCars([FromHeader(Name = "id")] string MicrosoftID)
        {
            return _carStatusService.GetCarRentals(MicrosoftID);
        }

        [HttpGet("rented/all/history")]
        public List<CarRentalAdminDTO> GetHistoryOfAllRentedCars()
        {
            return _carStatusService.GetHistoryOfAllCarRentals();
        }

        [HttpGet("rented/all/rented")]
        public List<CarRentalAdminDTO> GetAllRentedCars()
        {
            return _carStatusService.GetAllCarRentals();
        }

    }
}