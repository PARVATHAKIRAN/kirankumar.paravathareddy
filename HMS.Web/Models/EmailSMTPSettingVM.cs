using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Web.Models
{
    public class EmailSMTPSettingVM
    {
        public Guid Id { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public bool IsEnableSSL { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }
}
