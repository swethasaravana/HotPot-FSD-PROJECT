import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  MdRestaurant, 
  MdLocationOn, 
  MdPhone, 
  MdEmail,
  MdImage 
} from 'react-icons/md';
import { addRestaurantAPICall } from '../../../Services/RestaurantService';
import './AddRestaurant.css';

const AddRestaurant = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    contact: '',
    email: '',
    restaurantlogo: ''
  });

  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [imagePreview, setImagePreview] = useState(null);

  useEffect(() => {
    // Generate image preview when URL changes
    if (formData.restaurantlogo) {
      setImagePreview(formData.restaurantlogo);
    } else {
      setImagePreview(null);
    }
  }, [formData.restaurantlogo]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value
    }));
  };

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.name.trim()) newErrors.name = 'Restaurant name is required';
    if (!formData.address.trim()) newErrors.address = 'Address is required';
    
    if (!formData.contact.trim()) {
      newErrors.contact = 'Contact number is required';
    } else if (!/^[0-9]{10,15}$/.test(formData.contact)) {
      newErrors.contact = 'Please enter a valid contact number';
    }
    
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^\S+@\S+\.\S+$/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email address';
    }
    
    if (!formData.restaurantlogo.trim()) {
      newErrors.restaurantlogo = 'Image URL is required';
    } else if (!isValidImageUrl(formData.restaurantlogo)) {
      newErrors.restaurantlogo = 'Please enter a valid image URL';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const isValidImageUrl = (url) => {
    // Basic URL validation for common image extensions
    return /\.(jpeg|jpg|gif|png|webp)$/i.test(url);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    setIsSubmitting(true);
    try {
      await addRestaurantAPICall(formData);
      alert('Restaurant added successfully!');
      navigate('/admindashboard/view-restaurants');
    } catch (error) {
      console.error('Add restaurant failed:', error);
      let errorMessage = error.response?.data?.message || 'Error adding restaurant';
      
      // Handle specific error cases
      if (error.response?.status === 409) {
        errorMessage = 'A restaurant with this email or name already exists';
      } else if (error.response?.status === 400) {
        errorMessage = 'Invalid data provided';
      }
      
      alert(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="add-restaurant-container">
      <h2><MdRestaurant /> Add New Restaurant</h2>
      <form className="add-restaurant-form" onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="name"><MdRestaurant /> Restaurant Name:</label>
          <input
            type="text"
            id="name"
            name="name"
            className="form-control"
            value={formData.name}
            onChange={handleChange}
            placeholder="Enter restaurant name"
          />
          {errors.name && <span className="error">{errors.name}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="address"><MdLocationOn /> Address:</label>
          <input
            type="text"
            id="address"
            name="address"
            className="form-control"
            value={formData.address}
            onChange={handleChange}
            placeholder="Enter full address"
          />
          {errors.address && <span className="error">{errors.address}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="contact"><MdPhone /> Contact Number:</label>
          <input
            type="tel"
            id="contact"
            name="contact"
            className="form-control"
            value={formData.contact}
            onChange={handleChange}
            placeholder="Enter contact number"
          />
          {errors.contact && <span className="error">{errors.contact}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="email"><MdEmail /> Email:</label>
          <input
            type="email"
            id="email"
            name="email"
            className="form-control"
            value={formData.email}
            onChange={handleChange}
            placeholder="Enter email address"
          />
          {errors.email && <span className="error">{errors.email}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="restaurantlogo"><MdImage /> Restaurant Logo URL:</label>
          <input
            type="url"
            id="restaurantlogo"
            name="restaurantlogo"
            className="form-control"
            value={formData.restaurantlogo}
            onChange={handleChange}
            placeholder="Enter image URL (jpg, png, etc.)"
          />
          {errors.restaurantlogo && <span className="error">{errors.restaurantlogo}</span>}
          
          <div className="image-preview-container">
            {imagePreview ? (
              <img 
                src={imagePreview} 
                alt="Restaurant preview" 
                className="image-preview"
                onError={() => setImagePreview(null)}
              />
            ) : (
              <div className="preview-placeholder">
                Image preview will appear here
              </div>
            )}
          </div>
        </div>

        <button 
          type="submit" 
          className="submit-btn"
          disabled={isSubmitting}
        >
          {isSubmitting ? 'Adding Restaurant...' : 'Add Restaurant'}
        </button>
      </form>
    </div>
  );
};

export default AddRestaurant;