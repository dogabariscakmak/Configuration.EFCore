namespace Configuration.EFCore.IntegrationTest.Helpers
{
    public class TestSettings
    {
        public const string Position = "TestSettings";
        public bool IsDockerComposeRequired { get; set; }
        public string MssqlConnectionString { get; set; }
        public string DockerComposeFile { get; set; }
        public string DockerWorkingDir { get; set; }
        public string DockerComposeExePath { get; set; }
        public string TestDataPersonFilePath { get; set; }
        public string TestDataSettingsFilePath { get; set; }
        public string TestDataSettingsDevelopmentFilePath { get; set; }
        public string TestDataSettingsMobileFilePath { get; set; }
        public string TestDataSettingsMobileDevelopmentFilePath { get; set; }
        public bool IsGithubAction { get; set; }
    }
}
