using System.Collections.Generic;

namespace TCU.English.Models
{
    public class UserType : BaseEntity
    {
        #region Default Role
        public const string ROLE_NORMAL_USER = nameof(ROLE_NORMAL_USER);
        public const string ROLE_MANAGER_USER = nameof(ROLE_MANAGER_USER);
        public const string ROLE_ALL = nameof(ROLE_ALL);
        public const string ROLE_MANAGER_LIBRARY = nameof(ROLE_MANAGER_LIBRARY);
        #endregion
        public UserType()
        {
            UserTypeUser = new HashSet<UserTypeUser>();
        }
        public string UserTypeName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<UserTypeUser> UserTypeUser { get; set; }

        //==================================================//
        public static UserType NormalUser = new UserType
        {
            UserTypeName = nameof(ROLE_NORMAL_USER),
            Description = "Learner"
        };
        public static UserType ManagerUser = new UserType
        {
            UserTypeName = nameof(ROLE_MANAGER_USER),
            Description = "User Manager"
        };
        public static UserType All = new UserType
        {
            UserTypeName = nameof(ROLE_ALL),
            Description = "System Manager"
        };
        public static UserType ManagerLibrary = new UserType
        {
            UserTypeName = nameof(ROLE_MANAGER_LIBRARY),
            Description = "Question Library Manager"
        };
        //==================================================//

        public static UserType[] Roles = new UserType[]
        {
            NormalUser,
            ManagerUser,
            All,
            ManagerLibrary
        };
    }
}
