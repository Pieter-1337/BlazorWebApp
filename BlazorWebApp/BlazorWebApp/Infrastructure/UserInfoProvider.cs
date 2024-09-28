using BlazorWebApp.Dtos;
using BlazorWebApp.Infrastructure.Interfaces;

namespace BlazorWebApp.Infrastructure
{
    public class UserInfoProvider: IUserInfoProvider
    {
        private UserInfoDto UserInfo { get; set; }

        public void SetUser(UserInfoDto userInfo)
        {
            UserInfo = userInfo;
        }

        public UserInfoDto GetUser()
        {
            return UserInfo;
        }

        public string GetDisplayName()
        {
            return UserInfo?.DisplayName;
        }

        public string GetEmailAddress()
        {
            return UserInfo?.EmailAddress;
        }

        public IEnumerable<string> GetApplicationRoles()
        {
            return UserInfo != null ? UserInfo.ApplicationRoles : Enumerable.Empty<string>();
        }

        public void ClearUser()
        {
            UserInfo = null;
        }
    }
}
