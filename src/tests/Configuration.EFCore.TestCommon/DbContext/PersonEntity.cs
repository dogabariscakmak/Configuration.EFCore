using System;

namespace Configuration.EFCore.TestCommon
{
    public class PersonEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderEnum Gender { get; set; }
    }

    public enum GenderEnum
    {
        NotSet = 0,
        Female = 1,
        Male = 2
    }
}
