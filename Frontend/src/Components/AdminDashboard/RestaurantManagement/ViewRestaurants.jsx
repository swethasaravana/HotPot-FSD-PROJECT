import React, { useEffect, useState } from 'react';
import {
  getAllRestaurantsAPICall,
  deleteRestaurantAPICall,
  updateRestaurantAPICall,
} from '../../../Services/RestaurantService';
import './ViewRestaurants.css';

const ViewRestaurants = () => {
  const [restaurants, setRestaurants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editData, setEditData] = useState({
    restaurantId: '',
    restaurantName: '',
    location: '',
    contactNumber: '',
    email: '',
    restaurantlogo: ''
  });

  useEffect(() => {
    fetchRestaurants();
  }, []);

  const fetchRestaurants = async () => {
    try {
      const response = await getAllRestaurantsAPICall();
      setRestaurants(response.data);
    } catch (error) {
      console.error('Error fetching restaurants:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this restaurant?")) {
      try {
        await deleteRestaurantAPICall(id);
        fetchRestaurants();
      } catch (error) {
        console.error('Delete error:', error);
      }
    }
  };

  const handleEditClick = (restaurant) => {
    setEditData({ ...restaurant });
    setShowModal(true);
  };

  const handleChange = (e) => {
    setEditData({
      ...editData,
      [e.target.name]: e.target.value,
    });
  };

  const handleUpdate = async () => {
    try {
      const dto = {
        name: editData.restaurantName,
        address: editData.location,
        contact: editData.contactNumber,
        email: editData.email,
        restaurantlogo: editData.restaurantlogo,
      };
      await updateRestaurantAPICall(editData.restaurantId, dto);
      setShowModal(false);
      fetchRestaurants();
    } catch (error) {
      console.error('Update error:', error);
    }
  };

  return (
    <div className="view-restaurants-container ViewR-container">
      <h2>All Restaurants</h2>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="restaurant-list ViewR-list">
          {restaurants.length === 0 ? (
            <p>No restaurants found.</p>
          ) : (
            restaurants.map((restaurant, index) => (
              <div className="restaurant-card ViewR-card" key={restaurant.restaurantId || index}>
                <img
                  src={restaurant.restaurantlogo}
                  alt="Logo"
                  className="restaurant-logo ViewR-logo"
                />
                <h3>{restaurant.restaurantName}</h3>
                <p><strong>Address:</strong> {restaurant.location}</p>
                <p><strong>Contact:</strong> {restaurant.contactNumber}</p>
                <p><strong>Email:</strong> {restaurant.email}</p>
                <div className="card-buttons ViewR-buttons">
                  <button onClick={() => handleEditClick(restaurant)} className="ViewR-edit-btn">Edit</button>
                  <button className="delete ViewR-delete-btn" onClick={() => handleDelete(restaurant.restaurantId)}>Delete</button>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {showModal && (
        <div className="modal ViewR-modal">
          <div className="modal-content ViewR-modal-content">
            <h3>Edit Restaurant</h3>
            <label>Name:</label>
            <input name="restaurantName" value={editData.restaurantName} onChange={handleChange} />
            <label>Location:</label>
            <input name="location" value={editData.location} onChange={handleChange} />
            <label>Contact Number:</label>
            <input name="contactNumber" value={editData.contactNumber} onChange={handleChange} />
            <label>Email:</label>
            <input name="email" value={editData.email} onChange={handleChange} />
            <label>Logo URL:</label>
            <input name="restaurantlogo" value={editData.restaurantlogo} onChange={handleChange} />
            <div className="modal-buttons ViewR-modal-buttons">
              <button onClick={handleUpdate} className="ViewR-update-btn">Update</button>
              <button className="close ViewR-close-btn" onClick={() => setShowModal(false)}>Cancel</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ViewRestaurants;
