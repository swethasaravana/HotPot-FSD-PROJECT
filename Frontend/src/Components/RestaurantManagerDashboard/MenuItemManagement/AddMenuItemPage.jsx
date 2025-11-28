import React, { useState, useEffect } from "react";
import {
  AddMenuItemAPICall,
  GetAllCuisinesAPICall,
  GetAllMealTypesAPICall,
} from "../../../Services/RestaurantManagerService";
import "./AddMenuItemPage.css";

const AddMenuItemPage = () => {
  const restaurantManagerId = JSON.parse(sessionStorage.getItem("user"))?.id;

  const [menuItem, setMenuItem] = useState({
    name: "",
    description: "",
    cookingTime: "",
    price: 0,
    imagePath: "",
    availabilityTime: "",
    isAvailable: true,
    cuisineId: 0,
    mealTypeId: 0,
    calories: 0,
    proteins: 0,
    fats: 0,
    carbohydrates: 0,
    tasteInfo: "",
  });

  const [cuisines, setCuisines] = useState([]);
  const [mealTypes, setMealTypes] = useState([]);
  const [imageLoaded, setImageLoaded] = useState(false);
  const [imageError, setImageError] = useState(false);
  const [submitSuccess, setSubmitSuccess] = useState(false);

  useEffect(() => {
    GetAllCuisinesAPICall()
      .then((res) => setCuisines(res.data))
      .catch((err) => console.error("Error fetching cuisines:", err));

    GetAllMealTypesAPICall()
      .then((res) => setMealTypes(res.data))
      .catch((err) => console.error("Error fetching meal types:", err));
  }, []);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    const val = type === "checkbox" ? checked : value;
    setMenuItem((prev) => ({ ...prev, [name]: val }));
    
    if (name === "imagePath") {
      setImageLoaded(false);
      setImageError(false);
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    AddMenuItemAPICall(restaurantManagerId, menuItem)
      .then(() => {
        setSubmitSuccess(true);
        setTimeout(() => setSubmitSuccess(false), 3000);
        
        setMenuItem({
          name: "",
          description: "",
          cookingTime: "",
          price: 0,
          imagePath: "",
          availabilityTime: "",
          isAvailable: true,
          cuisineId: 0,
          mealTypeId: 0,
          calories: 0,
          proteins: 0,
          fats: 0,
          carbohydrates: 0,
          tasteInfo: "",
        });
        setImageLoaded(false);
        setImageError(false);
      })
      .catch((err) => {
        console.error("Error adding menu item", err);
        alert("Failed to add menu item.");
      });
  };

  return (
    <div className="fullscreen-container">
      <div className="RMD-add-menu-item-container">
        {/* Decorative floating food icons */}
        <div className="food-icon">🍕</div>
        <div className="food-icon">🍔</div>
        <div className="food-icon">🍣</div>
        
        <h2>Add Menu Item</h2>
        
        {submitSuccess && (
          <div className="success-message">
            Menu item added successfully! 🎉
          </div>
        )}
        
        <form onSubmit={handleSubmit} className="add-menu-form">
          <div className="form-section">
            <div className="form-column">
              <input 
                name="name" 
                value={menuItem.name} 
                onChange={handleChange} 
                placeholder="Name" 
                required 
              />
              
              <textarea 
                name="description" 
                value={menuItem.description} 
                onChange={handleChange} 
                placeholder="Description of Menu Item!" 
                required 
              />
              
              <input 
                name="cookingTime" 
                value={menuItem.cookingTime} 
                onChange={handleChange} 
                placeholder="Cooking Time (00:30:00)" 
              />
              
              <input 
                type="number" 
                name="price" 
                // value={menuItem.price} 
                onChange={handleChange} 
                placeholder="₹ Price" 
                min="0"
                step="0.01"
              />
            </div>
            
            <div className="form-column">
              <input 
                name="imagePath" 
                value={menuItem.imagePath} 
                onChange={handleChange} 
                placeholder="Image URL" 
              />
              
              {menuItem.imagePath && (
                <div className="image-preview">
                  <img 
                    src={menuItem.imagePath} 
                    alt="Preview"
                    className={!imageLoaded ? 'loading' : ''}
                    onLoad={() => setImageLoaded(true)}
                    onError={(e) => {
                      e.target.style.display = 'none';
                      setImageError(true);
                    }}
                  />
                  {imageError && (
                    <div className="error-message">
                      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      Could not load image
                    </div>
                  )}
                </div>
              )}
              
              <input 
                name="availabilityTime" 
                value={menuItem.availabilityTime} 
                onChange={handleChange} 
                placeholder="Availability Time (12:00 PM - 10:00 PM)" 
              />
              
              <label className="availability-label">
                Available:
                <input 
                  type="checkbox" 
                  name="isAvailable" 
                  checked={menuItem.isAvailable} 
                  onChange={handleChange} 
                />
              </label>
            </div>
            
            <div className="form-column">
              <select 
                name="cuisineId" 
                value={menuItem.cuisineId} 
                onChange={handleChange} 
                required
              >
                <option value="">Select Cuisine</option>
                {cuisines.map((cuisine) => (
                  <option key={cuisine.id} value={cuisine.id}>
                    {cuisine.name}
                  </option>
                ))}
              </select>

              <select 
                name="mealTypeId" 
                value={menuItem.mealTypeId} 
                onChange={handleChange} 
                required
              >
                <option value="">Select Meal Type</option>
                {mealTypes.map((mealType) => (
                  <option key={mealType.id} value={mealType.id}>
                    {mealType.name}
                  </option>
                ))}
              </select>

              <div className="nutrition-grid">
                <input 
                  type="number" 
                  name="calories" 
                  // value={menuItem.calories} 
                  onChange={handleChange} 
                  placeholder="Calories" 
                  min="0"
                />
                
                <input 
                  type="number" 
                  name="proteins" 
                  // value={menuItem.proteins} 
                  onChange={handleChange} 
                  placeholder="Proteins" 
                  min="0"
                  step="0.1"
                />
                
                <input 
                  type="number" 
                  name="fats" 
                  // value={menuItem.fats} 
                  onChange={handleChange} 
                  placeholder="Fats" 
                  min="0"
                  step="0.1"
                />
                
                <input 
                  type="number" 
                  name="carbohydrates" 
                  // value={menuItem.carbohydrates} 
                  onChange={handleChange} 
                  placeholder="Carbs" 
                  min="0"
                  step="0.1"
                />
              </div>
            </div>
          </div>

          <div className="full-width-section">
            <input 
              name="tasteInfo" 
              value={menuItem.tasteInfo} 
              onChange={handleChange} 
              placeholder="Taste Info (e.g., Sweet, Spicy, Savory)" 
              className="full-width-input"
            />
            
            <button type="submit" className="submit-button">
              Add Menu Item
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddMenuItemPage;