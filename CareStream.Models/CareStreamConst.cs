namespace CareStream.Models
{
    public static class CareStreamConst
    {
        public const string Owner = "Owner";
        public const string Member = "Member";
        public const string GroupMember = "Group_Member";
        public const string DealerProductFamily = "Dealer_ProductFammily";
        public const string GroupOwner = "Group_Owner";
        public const string UserSignInType_UserPrincipalName = "UserPrincipalName";
        public const string UserSignInType_EmailAddress = "EmailAddress";
        public const string Extension = "extension";
        public const string Application = "application";
        public const string Applications = "applications";
        public const string ExtensionProperties = "extensionProperties";
        public const string Bearer = "Bearer";
        public const string Json = "json";
        public const string Application_Json = "application/json";
        public const string Dealer = "Dealer";

        public const string Dash = "-";
        public const string Underscore = "_";
        public const string ForwardSlash = "/";
        public const string Pipe = "|";

        public const string O365 = "O365";
        public const string Security = "Security";
        public const string Unified = "Unified";
        public const string DynamicMembership = "DynamicMembership";

        public const string Member_GroupId = "MemberGroupId";
        public const string Owner_GroupId = "OwnerGroupId";
        public const string Public = "Public";

        //User Custom Attributes
        public const string Roles_C = "Roles_C";
        public const string UserType_C = "UserType_C";
        public const string UserBusinessDepartment_C = "UserBusinessDepartment_C";
        public const string Language_C = "Language_C";
        //URLS
        public const string Base_Url = "https://localhost:44366/";
        public const string Base_API = "api/";

        //URLS for User
        public const string User_Url = "User";
        public const string Users_Detail_Url = "User/users";
        public const string Users_DropDown_Url = "User/userdropdown";


        //URLS for Group
        public const string Group_Url = "Group";
        public const string GroupsDetail_Url = "group/groupdetails";
        public const string Group_By_Id_Url = "group/groups/";

        //URLS for Group Members
        public const string GroupMember_Url = "GroupMembers";
        public const string GroupMember_By_Id_Url = "groupmembers/getgroupmembers/";

        //URLS for Group Owners
        public const string GroupOwner_Url = "GroupOwners";
        public const string GroupOwner_By_Id_Url = "groupowners/getgroupowners/";

        //Bulk File Action
        public const string Bulk_Action = "Action";
        public const string Bulk_User_Create = "Create";
        public const string Bulk_User_Invite = "Invite";
        public const string Bulk_User_Update = "Update";
        public const string Bulk_User_Delete = "Delete";

        public const string Bulk_User_UploadedBy = "Admin";

        //Bulk User Header
        public const string BU_ObjectID = "objectid";
        public const string BU_UserPrincipalName = "userprincipalname";
        public const string BU_DisplayName = "displayname";
        public const string BU_BlockSignIn = "blocksignin";
        public const string BU_InitialPassword = "initialpassword";
        public const string BU_FirstName = "firstname";
        public const string BU_LastName = "lastname";
        public const string BU_JobTitle = "jobtitle";
        public const string BU_Department = "department";
        public const string BU_Usagelocation = "usagelocation";
        public const string BU_StreetAddress = "streetaddress";
        public const string BU_State = "state";
        public const string BU_Country = "country";
        public const string BU_Office = "office";
        public const string BU_City = "city";
        public const string BU_ZIP = "zip";
        public const string BU_OfficePhone = "officephone";
        public const string BU_MobilePhone = "mobilephone";
        public const string BU_CustomizedMessageBody = "customizedmessagebody";
        public const string BU_InviteeEmail = "inviteeemail";
        public const string BU_InviteRedirectURL = "inviteredirecturl";
        public const string BU_SendEmail = "sendemail";

        //Scheduler Status
        public const string Bulk_User_Loaded_Status = "Loaded";
        public const string Bulk_User_Started_Status = "Started";
        public const string Bulk_User_InProgress_Status = "InProgress";
        public const string Bulk_User_Failed_Status = "Failed";
        public const string Bulk_User_Success_Status = "Success";
        public const string Bulk_User_Completed_Status = "Completed";

        public static string CareStreamConnectionString { get; set; }
    }
}
