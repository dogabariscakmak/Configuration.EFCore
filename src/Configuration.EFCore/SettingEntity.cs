namespace Configuration.EFCore
{
    public class SettingEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Environment { get; set; }
        public string Application { get; set; }
    }
}
