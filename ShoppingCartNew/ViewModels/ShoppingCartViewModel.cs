using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ShoppingCartNew.Models;

namespace ShoppingCartNew.ViewModels
{
    public class ShoppingCartViewModel
    {
        [Key]
        public int ID { get; set; }
        public List<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}