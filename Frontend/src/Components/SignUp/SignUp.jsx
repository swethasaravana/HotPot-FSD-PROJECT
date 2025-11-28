import React, { useState } from 'react';
import axios from 'axios';
import { ToastContainer, toast } from 'react-toastify'; // Import ToastContainer and toast
import 'react-toastify/dist/ReactToastify.css'; // Import CSS for react-toastify
import logo from '../../assets/logo.png'; 
import './SignUp.css';

const SignUp = () => {
  const [formData, setFormData] = useState({
    name: '',
    gender: '',
    email: '',
    phone: '',
    password: '',
    addresses: [{
      label: 'Home',
      street: '',
      city: '',
      pincode: ''
    }]
  });

  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value
    });
  };

  const handleAddressChange = (e, index) => {
    const { name, value } = e.target;
    const updatedAddresses = [...formData.addresses];
    updatedAddresses[index] = {
      ...updatedAddresses[index],
      [name]: value
    };
    setFormData({
      ...formData,
      addresses: updatedAddresses
    });
  };

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.name.trim()) newErrors.name = 'Name is required';
    if (!formData.gender) newErrors.gender = 'Gender is required';
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }
    if (!formData.phone.trim()) {
      newErrors.phone = 'Phone is required';
    } else if (!/^\d{10}$/.test(formData.phone)) {
      newErrors.phone = 'Phone must be 10 digits';
    }
    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }
    
    formData.addresses.forEach((address, index) => {
      if (!address.street.trim()) newErrors[`addresses[${index}].street`] = 'Street is required';
      if (!address.city.trim()) newErrors[`addresses[${index}].city`] = 'City is required';
      if (!address.pincode.trim()) newErrors[`addresses[${index}].pincode`] = 'Pincode is required';
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    setIsSubmitting(true);

    try {
      const response = await axios.post('http://localhost:28827/api/Customer/register', formData);
      setSuccessMessage('Registration successful! Redirecting to login...');
      setTimeout(() => {
        window.location.href = '/login';
      }, 2000);
    } catch (error) {
      console.error('Registration error:', error);

      if (error.response) {
        // Check if the error is from the custom exception related to email already being registered
        if (error.response.data.message && error.response.data.message.includes("The email")) {
          const email = error.response.data.message.match(/'(.+)'/)[1];  // Extract the email from the error message
          toast.error(`The email '${email}' is already registered.`); // Show the toast error message
        } else {
          toast.error(error.response.data.message || 'Registration failed');
        }
      } else {
        toast.error('Network error. Please try again.');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="signup-signup-container">
      <div className="signup-signup-card">
        <div className="logo">
          <img src={logo} alt="HotByte Logo" className="signup-logo-image" />
          <h2 className="signup-signup-title">Create Your Account</h2>
        </div>

        {successMessage && <div className="signup-alert signup-alert-success">{successMessage}</div>}

        <form onSubmit={handleSubmit} className="signup-signup-form">
          <div className="signup-form-section">
            <h3 className="signup-section-title">Personal Information</h3>
            <div className="signup-form-row">
              <div className="signup-form-group">
                <label htmlFor="name">Full Name</label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  placeholder="Enter your full name"
                  className={errors.name ? 'signup-input-error' : ''}
                />
                {errors.name && <span className="signup-error-message">{errors.name}</span>}
              </div>

              <div className="signup-form-group">
                <label htmlFor="gender">Gender</label>
                <select
                  id="gender"
                  name="gender"
                  value={formData.gender}
                  onChange={handleChange}
                  className={errors.gender ? 'signup-input-error' : ''}
                >
                  <option value="">Select Gender</option>
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                  <option value="Other">Other</option>
                </select>
                {errors.gender && <span className="signup-error-message">{errors.gender}</span>}
              </div>
            </div>
          </div>

          <div className="signup-form-section">
            <h3 className="signup-section-title">Contact Details</h3>
            <div className="signup-form-row">
              <div className="signup-form-group">
                <label htmlFor="email">Email</label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  placeholder="Enter your email"
                  className={errors.email ? 'signup-input-error' : ''}
                />
                {errors.email && <span className="signup-error-message">{errors.email}</span>}
              </div>

              <div className="signup-form-group">
                <label htmlFor="phone">Phone Number</label>
                <input
                  type="tel"
                  id="phone"
                  name="phone"
                  value={formData.phone}
                  onChange={handleChange}
                  placeholder="Enter your phone number"
                  className={errors.phone ? 'signup-input-error' : ''}
                />
                {errors.phone && <span className="signup-error-message">{errors.phone}</span>}
              </div>
            </div>
          </div>

          <div className="signup-form-section">
            <h3 className="signup-section-title">Security</h3>
            <div className="signup-form-group">
              <label htmlFor="password">Password</label>
              <input
                type="password"
                id="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                placeholder="Create a password"
                className={errors.password ? 'signup-input-error' : ''}
              />
              {errors.password && <span className="signup-error-message">{errors.password}</span>}
            </div>
          </div>

          <div className="signup-form-section">
            <h3 className="signup-section-title">Address Information</h3>
            {formData.addresses.map((address, index) => (
              <div key={index} className="signup-address-card">
                <div className="signup-form-group">
                  <label htmlFor={`address-label-${index}`}>Address Label</label>
                  <input
                    type="text"
                    id={`address-label-${index}`}
                    name="label"
                    value={address.label}
                    onChange={(e) => handleAddressChange(e, index)}
                    placeholder="Home/Work/Other"
                  />
                </div>

                <div className="signup-form-group">
                  <label htmlFor={`address-street-${index}`}>Street Address</label>
                  <input
                    type="text"
                    id={`address-street-${index}`}
                    name="street"
                    value={address.street}
                    onChange={(e) => handleAddressChange(e, index)}
                    placeholder="Street and building number"
                    className={errors[`addresses[${index}].street`] ? 'signup-input-error' : ''}
                  />
                  {errors[`addresses[${index}].street`] && (
                    <span className="signup-error-message">{errors[`addresses[${index}].street`]}</span>
                  )}
                </div>

                <div className="signup-form-row">
                  <div className="signup-form-group">
                    <label htmlFor={`address-city-${index}`}>City</label>
                    <input
                      type="text"
                      id={`address-city-${index}`}
                      name="city"
                      value={address.city}
                      onChange={(e) => handleAddressChange(e, index)}
                      placeholder="City"
                      className={errors[`addresses[${index}].city`] ? 'signup-input-error' : ''}
                    />
                    {errors[`addresses[${index}].city`] && (
                      <span className="signup-error-message">{errors[`addresses[${index}].city`]}</span>
                    )}
                  </div>

                  <div className="signup-form-group">
                    <label htmlFor={`address-pincode-${index}`}>Pincode</label>
                    <input
                      type="text"
                      id={`address-pincode-${index}`}
                      name="pincode"
                      value={address.pincode}
                      onChange={(e) => handleAddressChange(e, index)}
                      placeholder="Postal code"
                      className={errors[`addresses[${index}].pincode`] ? 'signup-input-error' : ''}
                    />
                    {errors[`addresses[${index}].pincode`] && (
                      <span className="signup-error-message">{errors[`addresses[${index}].pincode`]}</span>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>

          <button 
            type="submit" 
            className="signup-submit-button"
            disabled={isSubmitting}
          >
            {isSubmitting ? (
              <>
                <span className="signup-spinner"></span>
                Registering...
              </>
            ) : 'Create Account'}
          </button>
        </form>

        <div className="signup-login-redirect">
          Already have an account? <a href="/login" className="signup-login-link">Log in</a>
        </div>
      </div>
      
      <ToastContainer /> {/* Add the ToastContainer here to show the toast notifications */}
    </div>
  );
};

export default SignUp;
