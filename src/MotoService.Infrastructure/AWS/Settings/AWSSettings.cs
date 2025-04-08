namespace MotoService.Infrastructure.AWS.Settings
{
    public class AWSSettings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string EndpointUrl { get; set; } = string.Empty;
        public string ServiceURL { get; set; } = string.Empty;
        public bool UseHttp { get; set; }
        public bool ForcePathStyle { get; set; }
    }
}
