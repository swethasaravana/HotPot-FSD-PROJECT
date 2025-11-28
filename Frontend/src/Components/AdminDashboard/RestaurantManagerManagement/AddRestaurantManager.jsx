import { useState } from "react";
import { addRestaurantManagerAPICall } from "../../../Services/RestaurantManagerService";
import "./AddRestaurantManager.css";

const AddRestaurantManager = () => {
  const [manager, setManager] = useState({
    fullName: "",
    email: "",
    password: "",
    phoneNumber: "",
    restaurantId: ""
  });

  const [message, setMessage] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;
    setManager({ ...manager, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await addRestaurantManagerAPICall(manager);
      setMessage("Manager added successfully!");
      setManager({
        fullName: "",
        email: "",
        password: "",
        phoneNumber: "",
        restaurantId: ""
      });
    } catch (error) {
      console.error(error);
      setMessage("Failed to add manager. Please try again.");
    }
  };

  return (
    <div className="add-manager-container">
      <h2>Add Restaurant Manager</h2>
      {message && <p className="message">{message}</p>}
      <form onSubmit={handleSubmit} className="add-manager-form">
        <input
          type="text"
          name="fullName"
          placeholder="Full Name"
          value={manager.fullName}
          onChange={handleChange}
          required
        />
        <input
          type="email"
          name="email"
          placeholder="Email"
          value={manager.email}
          onChange={handleChange}
          required
        />
        <input
          type="password"
          name="password"
          placeholder="Password"
          value={manager.password}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="phoneNumber"
          placeholder="Phone Number"
          value={manager.phoneNumber}
          onChange={handleChange}
          required
        />
        <input
          type="number"
          name="restaurantId"
          placeholder="Restaurant ID"
          value={manager.restaurantId}
          onChange={handleChange}
          required
        />
        <button type="submit" className="submit-btn">Add Manager</button>
      </form>
    </div>
  );
};

export default AddRestaurantManager;
