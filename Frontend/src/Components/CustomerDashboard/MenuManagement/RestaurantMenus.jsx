import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import {
  getMenuByRestaurant,
  getRestaurantById,
} from "../../../Services/CustomerService";
import AddToCartButton from "./AddToCartButton";
import "./RestaurantMenus.css";
import { FaShoppingCart } from "react-icons/fa";

function RestaurantMenus() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [menuItems, setMenuItems] = useState([]);
  const [restaurant, setRestaurant] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [addedItems, setAddedItems] = useState([]);

  const customerId = JSON.parse(sessionStorage.getItem("user"))?.id;

  const itemsPerRow = Math.ceil(menuItems.length / 3);
  const row1 = menuItems.slice(0, itemsPerRow);
  const row2 = menuItems.slice(itemsPerRow, itemsPerRow * 2);
  const row3 = menuItems.slice(itemsPerRow * 2);


  useEffect(() => {
    fetchRestaurantAndMenu(id);
  }, [id]);

  const fetchRestaurantAndMenu = async (restaurantId) => {
    try {
      setLoading(true);
      setError(null);
      const [menuRes, restaurantRes] = await Promise.all([
        getMenuByRestaurant(restaurantId),
        getRestaurantById(restaurantId),
      ]);

      setMenuItems(menuRes.data);
      setRestaurant(restaurantRes.data);
      toast.success("Menu loaded successfully!");
    } catch (err) {
      console.error("Error loading data:", err);
      setError("Failed to load restaurant information. Please try again later.");
      toast.error("Failed to load menu. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const handleItemAdded = (itemId, itemName) => {
    setAddedItems((prevItems) => [...prevItems, itemId]);
    toast.success(`${itemName} added to cart!`, {
      position: "bottom-right",
      autoClose: 2000,
      hideProgressBar: false,
      closeOnClick: true,
      pauseOnHover: true,
      draggable: true,
    });
  };

  if (loading) return (
    <div className="loading-container">
      <div className="loading-spinner"></div>
      <p>Loading menu...</p>
    </div>
  );
  
  if (error) return (
    <div className="error-container">
      <div className="error-icon">!</div>
      <p className="error-message">{error}</p>
      <button 
        className="retry-btn" 
        onClick={() => fetchRestaurantAndMenu(id)}
      >
        Retry
      </button>
    </div>
  );

  return (
    <div className="restaurant-menu">
      <button
        className="view-cart-btn pulse" 
        onClick={() => navigate("/cart")}
        aria-label="View your shopping cart"
      ><FaShoppingCart />
         View Cart
      </button>

      {restaurant && (
        <div className="restaurant-header slide-in">
          <h1>{restaurant.restaurantName} Menu</h1>
          <div className="restaurant-info">
            <p><span className="info-label">📍 Address:</span> {restaurant.location}</p>
            <p><span className="info-label">📞 Contact:</span> {restaurant.contactNumber}</p>
            <p><span className="info-label">✉️ Email:</span> {restaurant.email}</p>
          </div>
        </div>
      )}

      <section className="menu-items" aria-labelledby="menu-items-heading">
        <h2 id="menu-items-heading" className="section-title">Our Dishes</h2>
        <div className="menu-list">
          {menuItems.length > 0 ? (
            menuItems.map((item) => (
              <div className="menu-card pop-in" key={item.menuItemId}>
                <div className="menu-details">
                  <h3 className="item-name">{item.name}</h3>
                  <p className="item-description">{item.description}</p>
                  
                  <div className="nutrition-grid">
                    <div className="nutrition-item">
                      <span className="nutrition-value">₹{item.price}</span>
                      <span className="nutrition-label">Price</span>
                    </div>
                    <div className="nutrition-item">
                      <span className="nutrition-value">{item.calories}</span>
                      <span className="nutrition-label">Calories</span>
                    </div>
                    <div className="nutrition-item">
                      <span className="nutrition-value">{item.proteins}g</span>
                      <span className="nutrition-label">Protein</span>
                    </div>
                    <div className="nutrition-item">
                      <span className="nutrition-value">{item.carbohydrates}g</span>
                      <span className="nutrition-label">Carbs</span>
                    </div>
                    <div className="nutrition-item">
                      <span className="nutrition-value">{item.fats}g</span>
                      <span className="nutrition-label">Fat</span>
                    </div>
                    <div className="nutrition-item">
                      <span className="nutrition-value">{item.tasteInfo}</span>
                      <span className="nutrition-label">Taste</span>
                    </div>
                  </div>

                  <div className="item-meta">
                    <span className={`availability ${item.isAvailable ? "available" : "unavailable"}`}>
                      {item.isAvailable ? "Available" : "Unavailable"}
                    </span>
                    <span className="meal-type">{item.mealType?.name || 'N/A'}</span>
                    <span className="cuisine">{item.cuisine?.name || 'N/A'}</span>
                  </div>

                  <AddToCartButton 
                    item={item} 
                    customerId={customerId}
                    onItemAdded={() => handleItemAdded(item.menuItemId, item.name)}
                    disabled={!item.isAvailable}
                  />
                </div>
              </div>
            ))
          ) : (
            <div className="no-items-container">
              <div className="empty-icon">🍽️</div>
              <p className="no-items">No menu items available at this time.</p>
            </div>
          )}
        </div>
      </section>
    </div>
  );
}

export default RestaurantMenus;