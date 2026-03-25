namespace JudgeAPI.Application.Common.Configuration{
    public class CompilerOptions
    {
        public const string SectionName = "CompilerSettings";
        public string CppStandard { get; set; } = string.Empty;
        public string Flags { get; set; } = string.Empty;
    }
}
