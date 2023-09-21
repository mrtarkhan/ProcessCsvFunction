using Azure.Storage.Blobs;

namespace ProcessCSVFunction.Test;

public class UploadCSVFile
{
    [Fact]
    public void upload_csv_to_azuriti()
    {
        var ConnectionString = "UseDevelopmentStorage=true";

        BlobContainerClient containerClient = new(ConnectionString, "testcontainer");

        var blobName = "testblob";

        var blobClient = containerClient.GetBlobClient(blobName);

        blobClient.Delete();

        // data from : https://www.kaggle.com/datasets/sakshigoyal7/credit-card-customers

        Stream fileStream = new FileStream(Path.Combine("Files", "BankChurners.csv"), FileMode.Open, FileAccess.Read);

        try
        {
            blobClient.Upload(fileStream);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }


    }
}