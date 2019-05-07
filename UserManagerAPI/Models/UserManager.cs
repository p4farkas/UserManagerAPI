using System.Collections.Generic;
using System.Linq;

namespace UserManagerAPI.Models
{
    public class UserManager : IUserRepository<User>
    {
        readonly UserServiceContext _userServiceContext;

        public UserManager(UserServiceContext userServiceContext)
        {
            _userServiceContext = userServiceContext;
        }

        public UserAllViewModel GetMyUser(string routerBase)
        {
            int minId = _userServiceContext.Users.Min(x => x.Id);

            var myUser = from user in _userServiceContext.Users
                         where user.Id == minId
                         select new UserAllViewModel()
                         {
                             Id = "myself",
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             Email = user.Email,
                             TelNumber = user.TelNumber,
                             IsEmailVisible = user.IsEmailVisible,
                             IsTelNumberVisible = user.IsTelNumberVisible,
                             ProfilePicUrl = UrlCombine(routerBase, minId.ToString())
                         };

            return myUser.FirstOrDefault();
        }

        public UserViewModel GetUser(int id, string routerBase)
        {
            var userView = from user in _userServiceContext.Users
                           where user.Id == id
                           select new UserViewModel()
                           {
                               Id = user.Id,
                               FirstName = user.FirstName,
                               LastName = user.LastName,
                               Email = user.IsEmailVisible ? user.Email : null,
                               TelNumber = user.IsTelNumberVisible ? user.TelNumber : null,
                               ProfilePicUrl = UrlCombine(routerBase, id.ToString())
                           };

            return userView.FirstOrDefault();
        }

        public IEnumerable<UserBasicViewModel> GetAllUsers(string routerBase)
        {
            var users = from user in _userServiceContext.Users
                        select new UserBasicViewModel()
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            ProfilePicUrl = UrlCombine(routerBase, user.Id.ToString())
                        };

            return users.ToList();
        }

        public byte[] GetProfilePic(int Id)
        {
            var userPic = from user in _userServiceContext.Users
                          where user.Id == Id
                          select user.ProfilePic;

            return userPic.FirstOrDefault();
        }

        public bool UpdateMyUser(UserUpdateViewModel entity)
        {
            var myUser = FindMyUser();

            myUser.FirstName = entity.FirstName;
            myUser.LastName = entity.LastName;
            myUser.Email = entity.Email;
            myUser.TelNumber = entity.TelNumber;
            myUser.IsEmailVisible = entity.IsEmailVisible;
            myUser.IsTelNumberVisible = entity.IsTelNumberVisible;

            int recordUpdates = _userServiceContext.SaveChanges();

            return recordUpdates > 0 ? true : false;
        }

        public bool UpdateMyProfilePic(byte[] userPicBytes)
        {
            var myUser = FindMyUser();

            myUser.ProfilePic = userPicBytes;

            int recordUpdates = _userServiceContext.SaveChanges();

            return recordUpdates > 0 ? true : false;
        }

        #region Helper Methods

        private string UrlCombine(string url1, string url2)
        {
            if (url1.Length == 0)
            {
                return url2;
            }

            if (url2.Length == 0)
            {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return $"{url1}/{url2}";
        }

        private User FindMyUser()
        {
            int minId = _userServiceContext.Users.Min(x => x.Id);

            var myUser = (from user in _userServiceContext.Users
                          where user.Id == minId
                          select user).FirstOrDefault();

            return myUser;
        }

        #endregion
    }
}
