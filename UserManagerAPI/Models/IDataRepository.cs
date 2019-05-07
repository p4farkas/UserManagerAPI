using System.Collections.Generic;

namespace UserManagerAPI.Models
{
    public interface IUserRepository<User>
    {
        IEnumerable<UserBasicViewModel> GetAllUsers(string routerBase);
        UserViewModel GetUser(int id, string routerBase);
        UserAllViewModel GetMyUser(string routerBase);
        bool UpdateMyUser(UserUpdateViewModel entity);
        byte[] GetProfilePic(int Id);
        bool UpdateMyProfilePic(byte[] userPicBytes);

    }
}
