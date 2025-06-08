using ClosedXML.Excel;
using CreateExcelFileWorkerService.Services;
using MDNET.Identity.RabbitMQ.Web.Confugration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Data;
using System.Text;
using System.Text.Json;

namespace CreateExcelFileWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientService _rabbitMQClientService;
        
        private readonly CarService _carService;
        private IModel _channel;
        public string? QueueName { get; set; } = "queue-create-file-excel";
        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, /*IOptions<RabbitMQConfugration> rabbitMQConfugration,*/ CarService carService)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
           // _rabbitMQConfugration = rabbitMQConfugration.Value;
            _carService = carService;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync started...");
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, true);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync started...");
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(QueueName, false, consumer);
            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            _logger.LogInformation("Consumer_Received started...");
            await Task.Delay(5000); 

            var createExcelMessage = JsonSerializer.Deserialize<CreateFileMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            string apiurl = @"https://myfakeapi.com/api/cars/";
            DataTable dataTable = await _carService.GetCarsDataTable(apiurl);

            using var memoryStream = new MemoryStream();
            var wb = new XLWorkbook();
            wb.Worksheets.Add(dataTable, "Cars");
            wb.SaveAs(memoryStream);

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");

            var baseUrl = @"https://localhost:7118/api/files/upload-file";
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"{baseUrl}?userId={createExcelMessage.UserId}&fileId={createExcelMessage.FileId}", multipartFormDataContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"File (Id: {createExcelMessage.FileId}) was created successfully.");
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            else
            {
                _logger.LogError($"File (Id: {createExcelMessage.FileId}) creation failed. StatusCode: {response.StatusCode}");
                _channel.BasicNack(@event.DeliveryTag, false, true); // true: mesajı yenidən sıraya qoy
            }
        }

    }
}
