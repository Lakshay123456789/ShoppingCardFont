using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelfrontEnd.Model
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "please enter Product Name ")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "please enter Product Name ")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "please enter price")]
        public double ProductPrice { get; set; }

        [Required(ErrorMessage = "please enter Product Quality")]
        public int ProductQuality { get; set; }

        [Required(ErrorMessage = "please enter Product Image")]
        public string ProductImage { get; set; }

        [Required(ErrorMessage = "please enter ProductCategory ")]
        public string ProductCategory { get; set; }

    }
}
