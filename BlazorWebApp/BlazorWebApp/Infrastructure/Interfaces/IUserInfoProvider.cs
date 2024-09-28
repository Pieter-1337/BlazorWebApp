using BlazorWebApp.Dtos;

namespace BlazorWebApp.Infrastructure.Interfaces
{
    public interface IUserInfoProvider
    {
        void SetUser(UserInfoDto userInfo);
        UserInfoDto GetUser();
        string GetDisplayName();
        string GetEmailAddress();
        IEnumerable<string> GetApplicationRoles();
        void ClearUser();
    }
}
