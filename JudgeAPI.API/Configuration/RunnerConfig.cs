namespace JudgeAPI.Configuration
{
    public class RunnerConfig
    {
        public int Cpus { get; set; } = 1;
        public int MemoryMb { get; set; } = 256;
        public int PerTestTimeoutSeconds { get; set; } = 2;
        public string ImageName { get; set; } = "judge-cpp-runner";
    }
}
