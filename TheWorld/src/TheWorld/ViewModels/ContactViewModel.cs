using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// Model Binding to allow us to accept data from that form Contact.cshtml directly inside of a method of a controller.
/// </summary>
namespace TheWorld.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 10)]
        public string Message { get; set; }
    }
}
