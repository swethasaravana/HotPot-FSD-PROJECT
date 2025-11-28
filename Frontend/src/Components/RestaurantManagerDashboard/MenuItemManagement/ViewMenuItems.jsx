import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  MdEdit,
  MdDelete,
  MdAddCircle,
  MdFastfood,
  MdSearch,
  MdClose,
  MdAccessTime,
  MdAttachMoney,
  MdLocalDining,
  MdRestaurantMenu
} from 'react-icons/md';
import {
  GetRestaurantMenuItemsAPICall,
  updateMenuItemAPICall,
  deleteMenuItemAPICall,
  GetAllCuisinesAPICall,
  GetAllMealTypesAPICall,
  setAvailabilityForAllAPICall
} from "../../../Services/RestaurantManagerService";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './ViewMenuItems.css';

const ViewMenuItems = () => {
  const navigate = useNavigate();
  const [menuItems, setMenuItems] = useState([]);
  const [filteredItems, setFilteredItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedItem, setSelectedItem] = useState(null);
  const [formData, setFormData] = useState({});
  const [showModal, setShowModal] = useState(false);
  const [cuisines, setCuisines] = useState([]);
  const [mealTypes, setMealTypes] = useState([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const user = JSON.parse(sessionStorage.getItem('user'));

  useEffect(() => {
    if (!user?.id) {
      navigate('/login');
      return;
    }

    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        
        const [menuResponse, cuisineResponse, mealTypeResponse] = await Promise.all([
          GetRestaurantMenuItemsAPICall(user.id),
          GetAllCuisinesAPICall(),
          GetAllMealTypesAPICall()
        ]);

        setMenuItems(menuResponse.data);
        setFilteredItems(menuResponse.data);
        setCuisines(cuisineResponse.data);
        setMealTypes(mealTypeResponse.data);
      } catch (error) {
        console.error('Fetch error:', error);
        setError('Failed to load menu data. Please try again.');
        toast.error('Failed to fetch data');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [user.id, navigate]);

  useEffect(() => {
    const results = menuItems.filter(item =>
      item.itemName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (item.cuisine && item.cuisine.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (item.mealType && item.mealType.toLowerCase().includes(searchTerm.toLowerCase()))
    );
    setFilteredItems(results);
  }, [searchTerm, menuItems]);

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this menu item?')) {
      try {
        await deleteMenuItemAPICall(id);
        setMenuItems(prev => prev.filter(item => item.id !== id));
        toast.success('Menu item deleted successfully');
      } catch (error) {
        console.error('Delete error:', error);
        toast.error(error.response?.data?.message || 'Failed to delete item');
      }
    }
  };

  const openEditModal = (item) => {
    setSelectedItem(item);
    setFormData({
      name: item.itemName,
      description: item.description,
      cookingTime: item.cookingTime || "00:30:00",
      price: item.price,
      imagePath: item.imagePath || "",
      availabilityTime: item.availabilityTime || "10:00 AM - 11:00 PM",
      isAvailable: item.isAvailable,
      cuisineId: item.cuisineId,
      mealTypeId: item.mealTypeId,
      calories: item.calories,
      proteins: item.proteins,
      fats: item.fats,
      carbohydrates: item.carbohydrates,
      tasteInfo: item.tasteInfo || ""
    });
    setShowModal(true);
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    if (!selectedItem?.id) return;

    try {
      setIsSubmitting(true);
      const response = await updateMenuItemAPICall(selectedItem.id, formData);
      
      setMenuItems(prev =>
        prev.map(item => 
          item.id === selectedItem.id ? { ...item, ...response.data } : item
        )
      );
      
      toast.success('Menu item updated successfully');
      setShowModal(false);
    } catch (err) {
      console.error('Update error:', err);
      toast.error(err.response?.data?.message || 'Failed to update item');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : 
              type === 'number' ? parseFloat(value) || 0 : 
              value
    }));
  };

  const handleSetAvailabilityForAll = async () => {
    try {
      setLoading(true);
      await setAvailabilityForAllAPICall();
      // Refresh menu items after updating availability
      const menuResponse = await GetRestaurantMenuItemsAPICall(user.id);
      setMenuItems(menuResponse.data);
      setFilteredItems(menuResponse.data);
      toast.success('Availability updated for all menu items');
    } catch (error) {
      console.error('Availability update error:', error);
      toast.error('Failed to update availability for all items');
    } finally {
      setLoading(false);
    }
  };
  

  if (loading) {
    return (
      <div className="RMD-loading-container">
        <div className="RMD-spinner"></div>
        <p>Loading menu items...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="RMD-error-container">
        <p>{error}</p>
        <button className="RMD-retry-btn" onClick={() => window.location.reload()}>Retry</button>
      </div>
    );
  }

  return (
    <div className="RMD-view-menu-container">
      <div className="RMD-menu-header">
        <h1><MdFastfood /> Menu Management</h1>
        <div className="RMD-header-actions">
          <div className="RMD-search-container">
            <input
              type="text"
              placeholder="🔍 Search by menu, cuisine..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="RMD-search-input"
            />
            {searchTerm && (
              <MdClose 
                className="RMD-clear-search" 
                onClick={() => setSearchTerm('')}
              />
            )}
          </div>

          <button 
            className="RMD-add-button" 
            onClick={handleSetAvailabilityForAll}>Set Availability for All 
          </button>

          <button 
            className="RMD-add-button"
            onClick={() => navigate('/add-menu-item')}
          >
            <MdAddCircle /> Add New Item
          </button>
        </div>
      </div>

      {filteredItems.length === 0 ? (
        <div className="RMD-empty-state">
          <MdRestaurantMenu size={48} />
          <p>No menu items found</p>
          {searchTerm && (
            <button 
              className="RMD-clear-search-btn"
              onClick={() => setSearchTerm('')}
            >
              Clear search
            </button>
          )}
        </div>
      ) : (
        <div className="RMD-menu-items-grid">
          {filteredItems.map(item => (
            <div key={item.id} className="RMD-menu-item-card">
              <div className="RMD-card-image-container">
                <img
                  src={item.imagePath || '/images/default-food.png'}
                  alt={item.itemName}
                  onError={(e) => {
                    // e.target.src = '/images/default-food.png';
                  }}
                />
              </div>
              <div className="RMD-card-content">
                <h3>{item.itemName}</h3>
                <p className="RMD-description">{item.description}</p>
                
                <div className="RMD-item-details">
                  <div className="RMD-detail-item">
                    <MdAttachMoney />
                    <span>₹{item.price.toFixed(2)}</span>
                  </div>
                  <div className="RMD-detail-item">
                    <MdAccessTime />
                    <span>{item.cookingTime || 'N/A'}</span>
                  </div>
                  <div className="RMD-detail-item">
                    <MdAccessTime />
                    <span>{item.tasteInfo || 'N/A'}</span>
                  </div>
                  <div className="RMD-detail-item">
                    <MdLocalDining />
                    <span>{item.cuisine || 'No cuisine'}</span>
                  </div>
                  <div className="RMD-detail-item">
                    <MdLocalDining />
                    <span>{item.mealType || 'No mealtype'}</span>
                  </div>
                </div>
                <div className={`RMD-availability ${item.isAvailable ? 'RMD-available' : 'RMD-unavailable'}`}>
                  {item.isAvailable ? 'Available' : 'Unavailable'}
                </div>
              </div>
              
              <div className="RMD-card-actions">
                <button 
                  className="RMD-edit-btn"
                  onClick={() => openEditModal(item)}
                >
                  <MdEdit /> Edit
                </button>
                <button 
                  className="RMD-delete-btn"
                  onClick={() => handleDelete(item.id)}
                >
                  <MdDelete /> Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Edit Modal */}
      {showModal && (
        <div className="RMD-modal-overlay">
          <div className="RMD-modal-content">
            <div className="RMD-modal-header">
              <h2>Edit Menu Item</h2>
              <button 
                className="RMD-close-modal"
                onClick={() => setShowModal(false)}
              >
                <MdClose />
              </button>
            </div>
            <form onSubmit={handleUpdate}>
              <div className="RMD-form-group">
                <label htmlFor="name">Name</label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name || ''}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="RMD-form-group">
                <label htmlFor="description">Description</label>
                <textarea
                  id="description"
                  name="description"
                  value={formData.description || ''}
                  onChange={handleChange}
                  required
                ></textarea>
              </div>

              <div className="RMD-form-group">
                <label htmlFor="availabilityTime">Availability Time</label>
                <input
                  type="text"
                  id="availabilityTime"
                  name="availabilityTime"
                  value={formData.availabilityTime || ''}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="RMD-form-group">
                <label htmlFor="price">Price</label>
                <input
                  type="number"
                  id="price"
                  name="price"
                  value={formData.price || ''}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="RMD-form-group">
                <label htmlFor="cookingTime">Cooking Time</label>
                <input
                  type="text"
                  id="cookingTime"
                  name="cookingTime"
                  value={formData.cookingTime || ''}
                  onChange={handleChange}
                />
              </div>

              <div className="RMD-form-group">
  <label htmlFor="imagePath">Image URL</label>
  <input
    type="text"
    id="imagePath"
    name="imagePath"
    value={formData.imagePath || ''}
    onChange={handleChange}
    placeholder="https://example.com/image.jpg"
  />
  {formData.imagePath && (
    <div className="RMD-url-preview">
      <span className="RMD-url-preview-title"></span>
      <a href={formData.imagePath} className="RMD-url-preview-link" target="_blank" rel="noopener noreferrer">

      </a>
      <img 
        src={formData.imagePath} 
        alt="Preview" 
        className="RMD-url-preview-image" 
        onError={(e) => {
          e.target.style.display = 'none';
        }}
      />
    </div>
  )}
</div>

              <div className="RMD-form-group">
                <label htmlFor="cuisine">Cuisine</label>
                <select
                  id="cuisine"
                  name="cuisineId"
                  value={formData.cuisineId || ''}
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
              </div>

              <div className="RMD-form-group">
                <label htmlFor="mealType">Meal Type</label>
                <select
                  id="mealType"
                  name="mealTypeId"
                  value={formData.mealTypeId || ''}
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
              </div>

              <div className="RMD-form-group">
                <label>Calories</label>
                <input
                  type="number"
                  name="calories"
                  value={formData.calories}
                  onChange={handleChange}
                  min="0"
                />
              </div>

              <div className="RMD-form-group">
                <label>Proteins</label>
                <input
                  type="number"
                  name="proteins"
                  value={formData.proteins}
                  onChange={handleChange}
                  min="0"
                />
              </div>

              <div className="RMD-form-group">
                <label>Fats</label>
                <input
                  type="number"
                  name="fats"
                  value={formData.fats}
                  onChange={handleChange}
                  min="0"
                />
              </div>

              <div className="RMD-form-group">
                <label>Carbohydrates</label>
                <input
                  type="number"
                  name="carbohydrates"
                  value={formData.carbohydrates}
                  onChange={handleChange}
                  min="0"
                />
              </div>

              <div className="RMD-form-group">
                <label htmlFor="tasteInfo">Taste Information</label>
                <textarea
                  id="tasteInfo"
                  name="tasteInfo"
                  value={formData.tasteInfo || ''}
                  onChange={handleChange}
                ></textarea>
              </div>

              <div className="RMD-form-group">
                <label htmlFor="isAvailable">Availability</label>
                <input
                  type="checkbox"
                  id="isAvailable"
                  name="isAvailable"
                  checked={formData.isAvailable || false}
                  onChange={handleChange}
                />
              </div>

              <div className="RMD-form-actions">
                <button 
                  type="submit" 
                  disabled={isSubmitting}
                  className="RMD-submit-btn"
                >
                  {isSubmitting ? 'Updating...' : 'Update Item'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default ViewMenuItems;