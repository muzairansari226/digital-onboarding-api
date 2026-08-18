namespace CustomerPortal.Application.Settings
{
    public class HashingSettings
    {
        public int Iterations { get; set; }

        public int MemorySizeKb { get; set; }

        public int DegreeOfParallelism { get; set; }

        public int HashLengthBytes { get; set; }

        public int SaltLengthBytes { get; set; }
    }
}
