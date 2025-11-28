import React, { useEffect, useState } from "react";
import {
  getCartByCustomerId,
  addCartItem,
  updateCartItem,
  removeCartItem,
} from "../../../Services/CustomerService";
import { useNavigate } from "react-router-dom";
import logo from '../../../assets/logo.png';
import "./CartPage.css";

function CartPage() {
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  // Get the logged-in user's ID from localStorage
  const user = JSON.parse(sessionStorage.getItem("user"));
  const customerId = user?.id;

  const fetchCart = async () => {
    setLoading(true);
    try {
      const response = await getCartByCustomerId(customerId);
      setCart(response.data);
    } catch (error) {
      console.error("Error fetching cart", error);
    } finally {
      setLoading(false);
    }
  };

  // Fetch cart when component mounts
  useEffect(() => {
    if (customerId) {
      fetchCart();
    }
  }, [customerId]);

  const handleAddItem = async (menuItemId, quantity) => {
    try {
      await addCartItem(customerId, {
        MenuItemId: menuItemId,
        Quantity: quantity,
      });
      fetchCart();
    } catch (error) {
      console.error("Error adding cart item", error);
    }
  };

  const handleUpdateItem = async (cartItemId, quantity) => {
    if (quantity <= 0) {
      await handleRemoveItem(cartItemId);
      return;
    }

    try {
      await updateCartItem(customerId, cartItemId, { Quantity: quantity });
      fetchCart();
    } catch (error) {
      console.error("Error updating cart item", error);
    }
  };

  const handleRemoveItem = async (cartItemId) => {
    try {
      await removeCartItem(customerId, cartItemId);
      fetchCart();
    } catch (error) {
      console.error("Error removing cart item", error);
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div className="cart-page">
      <header className="header">
        <div className="logo">
                  <img src={logo} alt="HotByte Logo" 
                  onError={(e) => {e.target.onerror = null; e.target.src = '/default-logo.png';}}/>
                </div>
        <div className="header-icons">
        <button onClick={() => navigate(`/customer/profile/${customerId}`)}>Profile</button>
          <button onClick={() => navigate("/checkout")}>Checkout</button>
        </div>
      </header>

      <h2>Your Cart</h2>

      <div className="cart-items">
        {cart?.items?.length > 0 ? (
          cart.items.map((item) => (
            <div className="cart-item" key={item.cartItemId}>
              <div className="cart-item-info">
                <h4>{item.menuItemName}</h4>
                <p>₹{item.priceAtPurchase}</p>
              </div>
              <div className="cart-item-actions">
                <button onClick={() => handleUpdateItem(item.cartItemId, item.quantity + 1)}>+</button>
                <span>{item.quantity}</span>
                <button onClick={() => handleUpdateItem(item.cartItemId, item.quantity - 1)}>-</button>
                <button onClick={() => handleRemoveItem(item.cartItemId)}>Remove</button>
              </div>
            </div>
          ))
        ) : (
          <p>Your cart is empty.</p>
        )}
      </div>
      {cart?.totalPrice > 0 && (
        <div className="cart-total">
          <h3>Total: <span>₹{cart.totalPrice}</span></h3>
        </div>
      )}
    </div>
  );
}

export default CartPage;
