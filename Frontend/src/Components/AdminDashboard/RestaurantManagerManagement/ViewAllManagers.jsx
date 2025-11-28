import React, { useEffect, useState } from "react";
import {
  getAllManagersAPICall,
  getRestaurantManagerByIdAPICall,
  updateRestaurantManagerAPICall,
  deleteRestaurantManagerByIdAPICall
} from "../../../Services/RestaurantManagerService";
import "./ViewAllManagers.css";

const ViewAllManagers = () => {
  const [managers, setManagers] = useState([]);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [currentManager, setCurrentManager] = useState({
    id: '',
    fullName: '',
    password: '',
    email: '',
    phone: ''
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchManagers();
  }, []);

  const fetchManagers = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await getAllManagersAPICall();
      setManagers(response.data);
    } catch (error) {
      console.error("Error fetching managers:", error);
      setError("Failed to fetch managers. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleEditClick = (manager) => {
    setCurrentManager({
      id: manager.id,
      fullName: manager.fullName,
      password: '',
      email: manager.email,
      phone: manager.phone
    });
    setIsEditModalOpen(true);
  };

  const handleEditSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const payload = {
        Username: currentManager.fullName,
        Password: currentManager.password,
        Email: currentManager.email,
        PhoneNumber: currentManager.phone,
      };
      await updateRestaurantManagerAPICall(currentManager.id, payload);
      setIsEditModalOpen(false);
      fetchManagers();
    } catch (error) {
      console.error("Error updating manager:", error);
      setError(error.response?.data?.message || "Failed to update manager.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteClick = async (managerId) => {
    if (!window.confirm("Are you sure you want to delete this manager?")) return;
    setIsLoading(true);
    try {
      await deleteRestaurantManagerByIdAPICall(managerId);
      fetchManagers();
    } catch (error) {
      console.error("Error deleting manager:", error);
      setError("Failed to delete manager.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setCurrentManager({ ...currentManager, [name]: value });
  };

  return (
    <div className="viewRM-managers-container">
      <h2>All Restaurant Managers</h2>

      {error && <div className="viewRM-error-message">{error}</div>}
      {isLoading && <div className="viewRM-loading-indicator">Loading...</div>}

      <div className="viewRM-managers-table-wrapper">
        <table className="viewRM-managers-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Full Name</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Restaurant</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {managers.length > 0 ? (
              managers.map((manager) => (
                <tr key={manager.id}>
                  <td>{manager.id}</td>
                  <td>{manager.fullName}</td>
                  <td>{manager.email}</td>
                  <td>{manager.phone}</td>
                  <td>{manager.restaurantName}</td>
                  <td>
                    <button
                      className="viewRM-edit-btn"
                      onClick={() => handleEditClick(manager)}
                      disabled={isLoading}
                    >
                      Edit
                    </button>
                    <button
                      className="viewRM-delete-btn"
                      onClick={() => handleDeleteClick(manager.id)}
                      disabled={isLoading}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6">No managers found.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Edit Modal */}
      {isEditModalOpen && (
        <div className="viewRM-modal-overlay">
          <div className="viewRM-modal-content">
            <h3>Edit Manager</h3>
            <form onSubmit={handleEditSubmit}>
              <div className="viewRM-form-group">
                <label>ID:</label>
                <input type="text" name="id" value={currentManager.id} disabled />
              </div>
              <div className="viewRM-form-group">
                <label>Full Name:</label>
                <input
                  type="text"
                  name="fullName"
                  value={currentManager.fullName}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="viewRM-form-group">
                <label>Password:</label>
                <input
                  type="password"
                  name="password"
                  value={currentManager.password}
                  onChange={handleInputChange}
                  placeholder="Enter new password"
                />
              </div>
              <div className="viewRM-form-group">
                <label>Email:</label>
                <input
                  type="email"
                  name="email"
                  value={currentManager.email}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="viewRM-form-group">
                <label>Phone:</label>
                <input
                  type="tel"
                  name="phone"
                  value={currentManager.phone}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="viewRM-modal-actions">
                <button type="submit" disabled={isLoading}>
                  {isLoading ? 'Saving...' : 'Save Changes'}
                </button>
                <button
                  type="button"
                  onClick={() => setIsEditModalOpen(false)}
                  disabled={isLoading}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default ViewAllManagers;
