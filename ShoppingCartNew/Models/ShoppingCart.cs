﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartNew.Models
{
    public class ShoppingCart
    {
        ApplicationDbContext storeDB = new ApplicationDbContext();
       
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;

        }
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        public void AddToCart(Product product)
        {
            // Get the matching cart and album instances
            var cartItem = storeDB.CartItems.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == product.ProductID);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartItem
                {
                    ProductId = product.ProductID,
                    CartId = ShoppingCartId,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                storeDB.CartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Quantity++;
            }
         
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDB.CartItems.Single(cart => cart.CartId == ShoppingCartId && cart.CartItemId == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    itemCount = cartItem.Quantity;
                }
                else
                {
                    storeDB.CartItems.Remove(cartItem);
                }

                // Save changes
                storeDB.SaveChanges();
            }

            return itemCount;
        }
        public void EmptyCart()
        {
            var cartItems = storeDB.CartItems.Where(cart => cart.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                storeDB.CartItems.Remove(cartItem);
                
            }
            //Save Changes
            storeDB.SaveChanges();

        }
        public List<CartItem> GetCartItems()
        {
            return storeDB.CartItems.Where(cart => cart.CartId == ShoppingCartId).ToList();
        }
        public int GetCount()
        {
            //Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.CartItems
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Quantity).Sum();
            //Return 0 if all entries are null
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            decimal? total = (from cartItems in storeDB.CartItems
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Quantity * cartItems.Product.UnitPrice).Sum();
            return total ?? decimal.Zero;
        }
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            var cartItems = GetCartItems();
            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = order.OrderId,
                    UnitPrice = (decimal)item.Product.UnitPrice,
                    Quantity = item.Quantity
                };
                //Set the order's total to the ordertotal count
                orderTotal += (item.Quantity * (decimal)item.Product.UnitPrice);
                storeDB.OrderDetails.Add(orderDetail);
            }
            order.Total = orderTotal;
            storeDB.SaveChanges();
            EmptyCart();
            return order.OrderId;
        }

        public string GetCartId(HttpContextBase context)
        {
            if(context.Session[CartSessionKey]==null)
            {
                if(!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    //Random GUID Generation
                    Guid tempCartId = Guid.NewGuid();
                    //Send tempCartId back to client as a client
                    context.Session[CartSessionKey] = tempCartId.ToString();

                }
            }
            return context.Session[CartSessionKey].ToString();
        }
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.CartItems.Where(c => c.CartId == ShoppingCartId);
            foreach (CartItem item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();

        }



    }
}