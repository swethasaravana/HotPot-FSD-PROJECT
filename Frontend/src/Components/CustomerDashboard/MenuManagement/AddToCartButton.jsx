import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { addCartItem } from "../../../Services/CustomerService";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./AddToCartButton.css";

const AddToCartButton = ({ item, customerId, onItemAdded }) => {
  const navigate = useNavigate();
  const [isAdded, setIsAdded] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleAddToCart = async () => {
    try {
      if (!customerId) {
        toast.warn("Please log in to add items to the cart.", { toastId: "login-required" });
        navigate("/login");
        return;
      }

      if (!item.isAvailable) {
        toast.error("This item is currently unavailable.", { toastId: "item-unavailable" });
        return;
      }

      if (isAdded) {
        toast.info("This item is already in your cart.", { toastId: "item-exists" });
        return;
      }

      setIsLoading(true);
      setError(null);

      const cartItemCreate = {
        menuItemId: item.menuItemId || item.id,
        quantity: 1,
      };

      await addCartItem(customerId, cartItemCreate);

      setIsAdded(true);
      if (onItemAdded) onItemAdded(item.menuItemId || item.id);
      toast.success(`${item.name || item.itemName} added to cart!`, {
        toastId: "add-success",
        autoClose: 3000,
      });
    } catch (err) {
      console.error("Add to cart error:", err);

      const errMsg = err?.response?.data?.message || err.message || "";

      if (errMsg.includes("one restaurant")) {
        const restrictionMsg = "You can only order from one restaurant at a time";
        setError(restrictionMsg);
        toast.warning(restrictionMsg, { 
          toastId: "restaurant-restriction",
          autoClose: 5000,
        });
      } else {
        const defaultMsg = "Failed to add item to cart. Please try again.";
        setError(defaultMsg);
        toast.error(defaultMsg, { 
          toastId: "add-error",
          autoClose: 5000,
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  if (!item.isAvailable) {
    return <span className="unavailable-text">Currently Unavailable</span>;
  }

  return (
    <div className="add-to-cart-container">
      <button
        className={`add-to-cart-btn ${isAdded ? "added" : ""} ${isLoading ? "loading" : ""}`}
        onClick={handleAddToCart}
        disabled={isAdded || isLoading}
        aria-label={`Add ${item.name || item.itemName} to cart`}
      >
        {isLoading ? (
          <span className="loading-indicator">Adding...</span>
        ) : isAdded ? (
          <span className="added-indicator">✓ Added</span>
        ) : (
          "Add to Cart"
        )}
      </button>
      {error && <div className="error-message">{error}</div>}
    </div>
  );
};

export default AddToCartButton;