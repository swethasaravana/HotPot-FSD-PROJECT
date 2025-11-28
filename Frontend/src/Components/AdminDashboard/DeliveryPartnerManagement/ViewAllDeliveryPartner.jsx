import React, { useState, useEffect } from 'react';
import { 
  MdEdit, 
  MdDelete, 
  MdCheckCircle, 
  MdCancel, 
  MdDirectionsBike,
  MdLocalShipping,
  MdTwoWheeler,
  MdDirectionsCar
} from 'react-icons/md';
import { 
  getAllDeliveryPartners,
  updateDeliveryPartner,
  deleteDeliveryPartner,
} from "../../../Services/DeliveryPartnerService";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './ViewAllDeliveryPartner.css';

const ViewAllDeliveryPartner = () => {
  const [deliveryPartners, setDeliveryPartners] = useState([]);
  const [selectedPartner, setSelectedPartner] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    fetchPartners();
  }, []);

  const fetchPartners = async () => {
    try {
      setLoading(true);
      const response = await getAllDeliveryPartners();
      setDeliveryPartners(response.data);
    } catch (error) {
      console.error("Error fetching partners:", error);
      toast.error('Failed to load delivery partners');
    } finally {
      setLoading(false);
    }
  };

  const handleEditClick = (partner) => {
    setSelectedPartner({ ...partner });
    setIsModalOpen(true);
  };

  const handleDeleteClick = async (id) => {
    if (window.confirm("Are you sure you want to delete this partner?")) {
      try {
        await deleteDeliveryPartner(id);
        toast.success('Delivery partner deleted successfully');
        fetchPartners();
      } catch (error) {
        console.error("Delete failed:", error);
        toast.error('Failed to delete delivery partner');
      }
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setSelectedPartner((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleUpdate = async () => {
    try {
      await updateDeliveryPartner(selectedPartner.deliveryPartnerId, selectedPartner);
      toast.success('Delivery partner updated successfully');
      setIsModalOpen(false);
      fetchPartners();
    } catch (error) {
      console.error("Update failed:", error);
      toast.error('Failed to update delivery partner');
    }
  };

  const getVehicleIcon = (vehicleType) => {
    switch(vehicleType?.toLowerCase()) {
      case 'bike': return <MdDirectionsBike className="ViewDP-vehicle-icon" />;
      case 'truck': return <MdLocalShipping className="ViewDP-vehicle-icon" />;
      case 'car': return <MdDirectionsCar className="ViewDP-vehicle-icon" />;
      default: return <MdTwoWheeler className="ViewDP-vehicle-icon" />;
    }
  };

  const filteredPartners = deliveryPartners.filter(partner =>
    partner.fullName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    partner.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    partner.vehicleNumber?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="ViewDP-view-all-delivery-partners">
      <h1>Delivery Partners Management</h1>
      
      <div className="ViewDP-header-actions">
        <div className="ViewDP-search-container">
          <input
            type="text"
            placeholder="Search partners..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="ViewDP-search-input"
          />
          {searchTerm && (
            <button 
              className="ViewDP-clear-search" 
              onClick={() => setSearchTerm('')}
            >
              Clear
            </button>
          )}
        </div>
      </div>

      {loading ? (
        <div className="ViewDP-loading-container">
          <div className="ViewDP-spinner"></div>
          <p>Loading delivery partners...</p>
        </div>
      ) : (
        <table className="ViewDP-partner-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Contact</th>
              <th>Vehicle</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredPartners.length === 0 ? (
              <tr>
                <td colSpan="5" className="ViewDP-no-results">
                  {searchTerm ? 'No matching partners found' : 'No delivery partners available'}
                </td>
              </tr>
            ) : (
              filteredPartners.map((partner) => (
                <tr key={partner.deliveryPartnerId}>
                  <td>
                    <div className="ViewDP-partner-name">
                      {partner.fullName}
                      <span className="ViewDP-username">@{partner.username}</span>
                    </div>
                  </td>
                  <td>
                    <div className="ViewDP-contact-info">
                      <div>{partner.email}</div>
                      <div>{partner.phone}</div>
                    </div>
                  </td>
                  <td>
                    <div className="ViewDP-vehicle-info">
                      {getVehicleIcon(partner.vehicleType)}
                      <span>{partner.vehicleNumber}</span>
                    </div>
                  </td>
                  <td>
                    <span className={`ViewDP-status-badge ${partner.isAvailable ? 'ViewDP-status-available' : 'ViewDP-status-unavailable'}`}>
                      {partner.isAvailable ? 'Available' : 'Unavailable'}
                      {partner.isAvailable ? <MdCheckCircle /> : <MdCancel />}
                    </span>
                  </td>
                  <td>
                    <div className="ViewDP-action-buttons">
                      <button 
                        className="ViewDP-edit-button"
                        onClick={() => handleEditClick(partner)}
                      >
                        <MdEdit /> Edit
                      </button>
                      <button 
                        className="ViewDP-delete-button"
                        onClick={() => handleDeleteClick(partner.deliveryPartnerId)}
                      >
                        <MdDelete /> Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      )}

      {/* Modal Form */}
      {isModalOpen && selectedPartner && (
        <div className="ViewDP-modal">
          <div className="ViewDP-modal-content">
            <h2>Edit Delivery Partner</h2>
            
            <div className="ViewDP-form-group">
              <label>Full Name</label>
              <input 
                type="text" 
                name="fullName" 
                value={selectedPartner.fullName || ''} 
                onChange={handleChange} 
                className="ViewDP-form-input"
              />
            </div>
            
            <div className="ViewDP-form-group">
              <label>Email</label>
              <input 
                type="email" 
                name="email" 
                value={selectedPartner.email || ''} 
                onChange={handleChange} 
                className="ViewDP-form-input"
              />
            </div>
            
            <div className="ViewDP-form-group">
              <label>Phone</label>
              <input 
                type="text" 
                name="phone" 
                value={selectedPartner.phone || ''} 
                onChange={handleChange} 
                className="ViewDP-form-input"
              />
            </div>
            
            <div className="ViewDP-form-group">
              <label>Vehicle Number</label>
              <input 
                type="text" 
                name="vehicleNumber" 
                value={selectedPartner.vehicleNumber || ''} 
                onChange={handleChange} 
                className="ViewDP-form-input"
              />
            </div>
            
            <div className="ViewDP-checkbox-container">
              <input 
                type="checkbox" 
                name="isAvailable" 
                id="isAvailable"
                checked={selectedPartner.isAvailable || false} 
                onChange={handleChange} 
              />
              <label htmlFor="isAvailable">Available for deliveries</label>
            </div>
            
            <div className="ViewDP-modal-buttons">
              <button 
                className="ViewDP-update-button"
                onClick={handleUpdate}
              >
                Update Partner
              </button>
              <button 
                className="ViewDP-cancel-button"
                onClick={() => setIsModalOpen(false)}
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ViewAllDeliveryPartner;