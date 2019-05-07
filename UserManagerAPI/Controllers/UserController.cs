using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UserManagerAPI.Models;

namespace UserManagerAPI.Controllers
{
    [Route("users/")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserRepository<User> _dataRepository;

        public UserController(IUserRepository<User> dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public ActionResult<IEnumerable<UserBasicViewModel>> GetAllUsers()
        {
            var users = _dataRepository.GetAllUsers(Url.Link("ProfilePicRoute", null));

            if (users == null)
            {
                return NotFound();
            }

            return users.ToList();
        }

        [HttpGet]
        [Route("GetUser/{Id}")]
        public ActionResult<UserViewModel> GetUser(int Id)
        {
            UserViewModel user = _dataRepository.GetUser(Id, Url.Link("ProfilePicRoute", null));

            if (user == null)
            {
                return NotFound("User not found with the specified Id");
            }

            return user;
        }

        [HttpGet]
        [Route("GetMyUser")]
        public ActionResult<UserAllViewModel> GetMyUser()
        {
            UserAllViewModel user = _dataRepository.GetMyUser(Url.Link("ProfilePicRoute", null));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet]
        [Route("GetProfilePic/{Id?}", Name = "ProfilePicRoute")]
        public ActionResult GetProfilePic(int Id)
        {
            byte[] userPicBytes = _dataRepository.GetProfilePic(Id);

            if (userPicBytes == null)
            {
                return NotFound();
            }

            return File(userPicBytes, "image/jpeg");
        }

        [HttpPost]
        [Route("UpdateMyUser")]
        public ActionResult<UserAllViewModel> UpdateMyUser(UserUpdateViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUpdated = _dataRepository.UpdateMyUser(user);

            if (!isUpdated)
            {
                return BadRequest("Invalid data");
            }

            UserAllViewModel myUser = _dataRepository.GetMyUser(Url.Link("ProfilePicRoute", null));

            if (myUser == null)
            {
                return NotFound();
            }

            return myUser;
        }

        [Route("UpdateMyProfilePic")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public ActionResult<string> UpdateMyProfilePic([FromForm] IFormFile fileToUpload)
        {
            byte[] userPicBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                fileToUpload.OpenReadStream().CopyTo(ms);
                userPicBytes = ms.ToArray();
            }

            string validateErrorMessage = ValidatePicture(fileToUpload, userPicBytes.Length);

            if (userPicBytes == null)
            {
                throw new Exception("Invalid profile picture");
            }
            else if (validateErrorMessage != string.Empty)
            {
                throw new Exception(validateErrorMessage);
            }

            byte[] userPicBytesResized = ResizeImage(userPicBytes, 1024, 1024, fileToUpload.ContentType);

            bool isUpdated = _dataRepository.UpdateMyProfilePic(userPicBytesResized);

            if (isUpdated)
            {
                return Ok("Profile picture updated successfully");
            }
            else
            {
                throw new Exception("Unexpected error occured during profile picture update");
            }
        }

        #region Helper Methods

        [NonAction]
        private string ValidatePicture(IFormFile profilePicFile, int fileSize)
        {
            string[] allowedImageTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp" };

            const int MAX_FILESIZEBYTES = 10 * 1024 * 1024;

            if (!allowedImageTypes.Contains(profilePicFile.ContentType))
            {
                return "Invalid content found, please upload only image type";
            }
            else if (fileSize > MAX_FILESIZEBYTES)
            {
                return "The size of the image should not exceed 10 MB";
            }

            return string.Empty;
        }

        [NonAction]
        private byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight, string imageMimeType)
        {
            using (Image<Rgba32> image = Image.Load(imageBytes))
            {
                double xRatio = (double)image.Width / maxWidth;
                double yRatio = (double)image.Height / maxHeight;
                double ratio = Math.Max(xRatio, yRatio);

                int nnx = (int)Math.Floor(image.Width / ratio);
                int nny = (int)Math.Floor(image.Height / ratio);

                image.Mutate(x => x
                     .Resize(nnx, nny));

                IImageEncoder imageEncoder = CreateImageEncoder(imageMimeType);

                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, imageEncoder);
                    return ms.ToArray();
                }
            }
        }

        [NonAction]
        private IImageEncoder CreateImageEncoder(string mimeType)
        {
            IImageEncoder imageEncoder = null;
            switch (mimeType)
            {
                case "image/jpeg":
                    {
                        imageEncoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
                        break;
                    }
                case "image/png":
                    {
                        imageEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
                        break;
                    }
                case "image/gif":
                    {
                        imageEncoder = new SixLabors.ImageSharp.Formats.Gif.GifEncoder();
                        break;
                    }
                case "image/bmp":
                    {
                        imageEncoder = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();
                        break;
                    }
            }

            return imageEncoder;
        }

        #endregion
    }
}