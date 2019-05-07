using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(200)]
        public string LastName { get; set; }
        [Required]
        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(200)]
        public string TelNumber { get; set; }
        [DefaultValue(false)]
        public bool IsEmailVisible { get; set; }
        [DefaultValue(false)]
        public bool IsTelNumberVisible { get; set; }
        public byte[] ProfilePic { get; set; }
    }

    public class UserBasicViewModel
    {
        [JsonProperty(PropertyName = "userID")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicUrl { get; set; }
    }

    public class UserViewModel
    {
        [JsonProperty(PropertyName = "userID")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Email { get; set; }
        [JsonProperty(PropertyName = "telephoneNumber")]
        public string TelNumber { get; set; }
    }

    public class UserAllViewModel
    {
        [JsonProperty(PropertyName = "userID")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [JsonProperty(PropertyName = "telephoneNumber")]
        public string TelNumber { get; set; }
        public bool IsEmailVisible { get; set; }
        public bool IsTelNumberVisible { get; set; }
        public string ProfilePicUrl { get; set; }
    }

    public class UserUpdateViewModel
    {
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(200)]
        public string LastName { get; set; }
        [Required]
        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(200)]
        [JsonProperty(PropertyName = "telephoneNumber")]
        public string TelNumber { get; set; }
        [DefaultValue(false)]
        public bool IsEmailVisible { get; set; }
        [DefaultValue(false)]
        public bool IsTelNumberVisible { get; set; }
    }
}
