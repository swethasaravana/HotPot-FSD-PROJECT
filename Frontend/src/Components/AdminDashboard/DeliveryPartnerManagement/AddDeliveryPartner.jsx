import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { addDeliveryPartnerAPICall } from "../../../Services/DeliveryPartnerService";
import './AddDeliveryPartner.css';

const AddDeliveryPartner = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    password: "",
    phone: "",
    vehicleNumber: "",
  });

  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState(""); // New state for success message

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await addDeliveryPartnerAPICall(formData);
      setSuccessMessage("Delivery partner added successfully!"); // Set success message
      setError(""); // Clear any previous error
      setTimeout(() => {
        navigate("/admin-dashboard/view-deliverypartner"); // Redirect to view all delivery partners page
      }, 2000); // Redirect after 2 seconds to allow success message to be seen
    } catch (err) {
      setError("Failed to add delivery partner.");
      setSuccessMessage(""); // Clear success message in case of error
    }
  };

  return (
    <div className="add-delivery-partner">
      <h2>Add Delivery Partner</h2>
      {error && <p className="error-message">{error}</p>}
      {successMessage && <p className="success-message">{successMessage}</p>} {/* Display success message */}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="fullName">Full Name</label>
          <input
            type="text"
            id="fullName"
            name="fullName"
            value={formData.fullName}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="password">Password</label>
          <input
            type="password"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="phone">Phone</label>
          <input
            type="text"
            id="phone"
            name="phone"
            value={formData.phone}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="vehicleNumber">Vehicle Number</label>
          <input
            type="text"
            id="vehicleNumber"
            name="vehicleNumber"
            value={formData.vehicleNumber}
            onChange={handleChange}
            required
          />
        </div>
        <button type="submit" className="submit-btn">
          Add Delivery Partner
        </button>
      </form>
    </div>
  );
};

export default AddDeliveryPartner;
