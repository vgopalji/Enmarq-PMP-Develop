using System.Collections.Generic;

namespace CareStream.Models
{
    public class UserDropDownModel
    {
        public string AutoPassword { get; set; }
        public List<Country> UserLocation { get; set; }
        public List<UserType> UserTypes { get; set; }
        public List<UserBusinessDepartment> UserBusinessDepartments { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<UserLanguage> UserLanguages { get; set; }
        public Dictionary<string,string> Groups { get; set; }
        public List<UserType> UserAttributes { get; set; }
    }

    public class UserType
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class UserBusinessDepartment
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class UserRole
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class UserLanguage
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
