using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CreateExcelFileWorkerService.Model
{
    public  class CarDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("car")]
        public string? Car { get; set; }

        [JsonProperty("car_model")]
        public string? CarModel { get; set; }

        [JsonProperty("car_color")]
        public string? CarColor { get; set; }

        [JsonProperty("car_model_year")]
        public int CarModelYear { get; set; }

        [JsonProperty("car_vin")]
        public string? CarVin { get; set; }

        [JsonProperty("price")]
        public string? Price { get; set; }

        [JsonProperty("availability")]
        public bool Availability { get; set; }
    }
}

