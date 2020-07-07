using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CareStream.Models.User;
using Newtonsoft.Json;


namespace CareStream.Models
{
    /// <summary>
    /// User model 
    /// </summary>
    public class UserModel
    {
        public UserModel()
        {
            PasswordOptions = new List<PasswordOption>
            {
                new PasswordOption
                {
                    Id=1,
                    Type=PasswordOptionType.AutoGeneratePassword
                },
                new PasswordOption
                {
                    Id=2,
                    Type=PasswordOptionType.ManualCreatedPassword
                }
            };
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isselected", Required = Required.Default)]
        public bool IsSelected { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id", Required = Required.Default)]
        public string Id { get; set; }

        /*--Additional Attributes--*/
        [JsonProperty(PropertyName = "customAttributes", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> CustomAttributes { get; set; }

        [JsonProperty(PropertyName = "groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Groups { get; set; }

        [JsonProperty(PropertyName = "rolesaa", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RolesAA { get; set; }

        [JsonProperty(PropertyName = "usertypeaa", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> UserTypeAA { get; set; }

        [JsonProperty(PropertyName = "userbusinessdepartmentaa", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> UserBusinessDepartmentAA { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "usageLocation", Required = Required.Default)]
        public string UsageLocation { get; set; }

        /*----*/

        /*--Custom Attributes--*/

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "roles_c", Required = Required.Default)]
        public string Roles_C { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userType_c", Required = Required.Default)]
        public string UserType_C { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "language_c", Required = Required.Default)]
        public string Language_C { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userBusinessDepartment_c", Required = Required.Default)]
        public string UserBusinessDepartment_C { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "source", Required = Required.Default)]
        public string Source { get; set; }
        /*----*/


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "accountenabled", Required = Required.Default)]
        public bool? AccountEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "givenName", Required = Required.Default)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid First name")]
        public string GivenName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "preferredName", Required = Required.Default)]
        public string PreferredName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "surname", Required = Required.Default)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid surname")]
        public string Surname { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "displayName", Required = Required.Default)]
        public string DisplayName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userType", Required = Required.Default)]
        public string UserType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userPrincipalName", Required = Required.Default)]
        public string UserPrincipalName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "creationType", Required = Required.Default)]
        public string CreationType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "country", Required = Required.Default)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid Country")]
        public string Country { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "companyName", Required = Required.Default)]
        public string CompanyName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mail", Required = Required.Default)]
        public string Mail { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mobilePhone", Required = Required.Default)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{2})[-. ]?([0-9]{4})[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Please enter valid Phone number")]
        public string MobilePhone { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "streetAddress", Required = Required.Default)]
        public string StreetAddress { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "state", Required = Required.Default)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid State")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "delegateadministration", NullValueHandling = NullValueHandling.Ignore)]
        public bool DelegateAdministration { get; set; }

        [JsonProperty(PropertyName = "forcechangepasswordnextsignin", NullValueHandling = NullValueHandling.Ignore)]
        public bool ForceChangePasswordNextSignIn { get; set; }

        [JsonProperty(PropertyName = "autogeneratepassword", NullValueHandling = NullValueHandling.Ignore)]
        public bool AutoGeneratePassword { get; set; }

        [JsonProperty(PropertyName = "manualcreatedpassword", NullValueHandling = NullValueHandling.Ignore)]
        public bool ManualCreatedPassword { get; set; }

        [JsonProperty(PropertyName = "password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "postalCode", Required = Required.Default)]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Please enter valid Zip Code")]
        public string PostalCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "officeLocation", Required = Required.Default)]
        public string OfficeLocation { get; set; }

        [JsonProperty(PropertyName = "signInName", NullValueHandling = NullValueHandling.Ignore)]
        [Required(ErrorMessage = "Email ID is Required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Only 50 chars allowed.")]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessage = "Please enter valid Email")]
        public string SignInName { get; set; }

        [JsonProperty(PropertyName = "businessemail", NullValueHandling = NullValueHandling.Ignore)]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Only 50 chars allowed.")]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessage = "Please enter valid Email")]
        public string BusinessEmail { get; set; }

        [JsonProperty(PropertyName = "region", NullValueHandling = NullValueHandling.Ignore)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid Region")]
        public string Region { get; set; }

        [JsonProperty(PropertyName = "address", NullValueHandling = NullValueHandling.Ignore)]
        [MaxLength(500, ErrorMessage = "Only 500 chars allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-_#()]+$", ErrorMessage = "Please enter valid address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "address2", NullValueHandling = NullValueHandling.Ignore)]
        [MaxLength(500, ErrorMessage = "Only 500 chars allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-_#()]+$", ErrorMessage = "Please enter valid address 2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "address3", NullValueHandling = NullValueHandling.Ignore)]
        [MaxLength(500, ErrorMessage = "Only 500 chars allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-_#()]+$", ErrorMessage = "Please enter valid address 3")]
        public string Address3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "department", Required = Required.Default)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid Department")]
        public string Department { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jobTitle", Required = Required.Default)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please enter valid Job Title")]
        public string JobTitle { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "businessPhones", Required = Required.Default)]
        public IEnumerable<string> BusinessPhones { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "businessPhone", Required = Required.Default)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{2})[-. ]?([0-9]{4})[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Please enter valid Business Phone")]
        public string BusinessPhone { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mySite", Required = Required.Default)]
        [MaxLength(250, ErrorMessage = "Only 250 chars allowed")]
        [RegularExpression(@"^((https?|ftp|smtp):\/\/)?(www.)?[a-z0-9]+\.[a-z]+(\/[a-zA-Z0-9#]+\/?)*$", ErrorMessage = "Please enter valid website")]
        public string WebSite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "signintype", Required = Required.Default)]
        public string SignInType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userName", Required = Required.Default)]
        public string UserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userDomain", Required = Required.Default)]
        public string UserDomain { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "city", Required = Required.Default)]
        public string City { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "otherMails", Required = Required.Default)]
        public IEnumerable<string> OtherMails { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "passwordoptiontype", Required = Required.Default)]
        public PasswordOptionType PasswordOptionType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "passwordoptions", Required = Required.Default)]
        public List<PasswordOption> PasswordOptions { get; set; }

        public List<UserType> AdditionalData { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

   public class UserViewModel
    {
        public UserViewModel()
        {
            UserModel = new UserModel();
        }

        public UserModel UserModel { get; set; }
        public InviteUser InviteUser { get; set; }
    }
}
