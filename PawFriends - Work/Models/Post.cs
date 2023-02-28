using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; // to enable the Key Attribute
using System.ComponentModel.DataAnnotations.Schema;

namespace PawFriends___Work.Models
{
    public class Post
    {
        [Key]
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title's maxlen is 100 chars!")]
        [MinLength(5, ErrorMessage = "Title's minlen is 5 chars!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required!")]
        public string Content { get; set; }

        public DateTime Date { get; set;}

        [Required(ErrorMessage = "Category is required!")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }  

        //PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

        public string? UserId {get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

		[DefaultValue("http://cdn.onlinewebfonts.com/svg/img_275062.png")]
		public string? Icon { get; set; }

	}
}
