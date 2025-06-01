using CreateExcelFileWorkerService.Model;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateExcelFileWorkerService.Services
{
    public class CarService
    {

        public class CarApiResponse
        {
            [JsonProperty("cars")]
            public List<CarDTO> Cars { get; set; }
        }

        public async Task<DataTable> GetCarsDataTable(string url)
        {
            using HttpClient _httpClient = new HttpClient();
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var carResponse =  JsonConvert.DeserializeObject<CarApiResponse>(json);
           
            var dataTable =  ConvertToDataTable(carResponse.Cars);
            return dataTable;

        }

        private DataTable ConvertToDataTable(List<CarDTO> carDTOs) 
        { 

            var table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Car", typeof(string));
            table.Columns.Add("Model", typeof(string));
            table.Columns.Add("Color", typeof(string));
            table.Columns.Add("Year", typeof(int));
            table.Columns.Add("VIN", typeof(string));
            table.Columns.Add("Price", typeof(string));
            table.Columns.Add("Availability", typeof(bool));

            foreach (var car in carDTOs)
            {
                table.Rows.Add(car.Id, car.Car, car.CarModel, car.CarColor,
                               car.CarModelYear, car.CarVin, car.Price, car.Availability);
            }

            return table;
        }
    }
}
