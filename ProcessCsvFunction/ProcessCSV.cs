using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;

namespace ProcessCsvFunction
{
    public class ProcessCSV
    {
        private readonly ILogger _logger;

        public ProcessCSV(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProcessCSV>();
        }

        [Function("ProcessCSV")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var request = await req.ReadFromJsonAsync<RequestDto>();

            var ConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            BlobContainerClient containerClient = new(ConnectionString, request.ContainerName);

            var blobClient = containerClient.GetBlobClient(request.BlobName);

            var result = blobClient.DownloadContent();

            using var memoryStream = new MemoryStream(result.Value.Content.ToMemory().ToArray());

            memoryStream.Position = 0;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };

            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<CustomerModel>().ToList();

            TableClient client = new TableClient(ConnectionString, "customers");

            await client.CreateIfNotExistsAsync();

            foreach (var record in records)
            {
                record.RowKey = Guid.NewGuid().ToString();
                await client.UpsertEntityAsync(record);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"{records.Count} records proccessed");

            return response;
        }
    }
}
