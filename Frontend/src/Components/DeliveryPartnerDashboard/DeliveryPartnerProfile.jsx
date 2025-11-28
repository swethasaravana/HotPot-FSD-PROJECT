import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getDeliveryPartnerById } from "../../Services/DeliveryPartnerService";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import {
  FaUser,
  FaMotorcycle,
  FaEnvelope,
  FaPhone,
  FaCalendarAlt,
  FaReceipt,
  FaBox,
  FaCheckCircle,
  FaTimesCircle,
  FaIdCard
} from "react-icons/fa";
import "./DeliveryPartnerProfile.css";

const DeliveryPartnerProfile = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("basic");
  const [partner, setPartner] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPartnerData = async () => {
      try {
        setLoading(true);
        const deliveryPartnerId = JSON.parse(sessionStorage.getItem("user"))?.id;
        
        if (!deliveryPartnerId || isNaN(deliveryPartnerId)) {
          throw new Error("Invalid delivery partner ID");
        }

        const response = await getDeliveryPartnerById(deliveryPartnerId);
        setPartner(response.data);
        toast.success("Profile data loaded successfully!");
      } catch (err) {
        console.error("Failed to fetch delivery partner data:", err);
        toast.error(err.response?.data?.message || err.message || "Failed to load profile");
        
        if (err.response?.status === 401) {
          navigate('/login');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchPartnerData();
  }, [navigate]);

  if (loading) {
    return (
      <div className="dpp-loading">
        <div className="dpp-spinner"></div>
        <p>Loading your profile...</p>
      </div>
    );
  }

  if (!partner) {
    return (
      <div className="dpp-no-data">
        <FaUser className="dpp-icon" /> No delivery partner data found
      </div>
    );
  }

  return (
    <div className="dpp-delivery-partner-profile">
      <h2>
        <FaUser className="dpp-icon" /> Delivery Partner Profile
      </h2>
      <div className="dpp-tabs">
        <button
          className={activeTab === "basic" ? "dpp-active" : ""}
          onClick={() => setActiveTab("basic")}
        >
          <FaUser className="dpp-icon" /> Basic Info
        </button>
        <button
          className={activeTab === "vehicle" ? "dpp-active" : ""}
          onClick={() => setActiveTab("vehicle")}
        >
          <FaMotorcycle className="dpp-icon" /> Vehicle Info
        </button>
      </div>

      <div className="dpp-tab-content">
        {activeTab === "basic" && partner && (
          <div className="dpp-info">
            <div className="dpp-info-item">
              <FaUser className="dpp-icon" />
              <p>
                <strong>Full Name:</strong> {partner.fullName}
              </p>
            </div>
            <div className="dpp-info-item">
              <FaIdCard className="dpp-icon" />
              <p>
                <strong>Username:</strong> {partner.username}
              </p>
            </div>
            <div className="dpp-info-item">
              <FaEnvelope className="dpp-icon" />
              <p>
                <strong>Email:</strong> {partner.email}
              </p>
            </div>
            <div className="dpp-info-item">
              <FaPhone className="dpp-icon" />
              <p>
                <strong>Phone:</strong> {partner.phone}
              </p>
            </div>
            <div className="dpp-info-item">
              {partner.isAvailable ? (
                <FaCheckCircle className="dpp-icon" style={{ color: '#27ae60' }} />
              ) : (
                <FaTimesCircle className="dpp-icon" style={{ color: '#e74c3c' }} />
              )}
              <p>
                <strong>Availability:</strong> {partner.isAvailable ? "Available" : "Unavailable"}
              </p>
            </div>
          </div>
        )}

        {activeTab === "vehicle" && partner && (
          <div className="dpp-info">
            <h3>
              <FaMotorcycle className="dpp-icon" /> Vehicle Information
            </h3>
            {partner.vehicleNumber ? (
              <div className="dpp-vehicle-grid">
                <div className="dpp-info-item">
                  <FaMotorcycle className="dpp-icon" />
                  <p>
                    <strong>Vehicle Number:</strong> {partner.vehicleNumber}
                  </p>
                </div>
                <div className="dpp-info-item">
                  <FaIdCard className="dpp-icon" />
                  <p>
                    <strong>License Number:</strong> {partner.drivingLicense || "Not provided"}
                  </p>
                </div>
              </div>
            ) : (
              <p className="dpp-no-data">
                <FaMotorcycle className="dpp-icon" /> No vehicle information available
              </p>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default DeliveryPartnerProfile;