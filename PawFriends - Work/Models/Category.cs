using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; // to enable the Key Attribute

namespace PawFriends___Work.Models
{
    public class Category
    {
        [Key]

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Descriere { get; set; }

        [DefaultValue("http://cdn.onlinewebfonts.com/svg/img_275062.png")]
        public string? Icon { get; set; }


		public virtual ICollection<Post>? Posts { get; set; }
    }
}
