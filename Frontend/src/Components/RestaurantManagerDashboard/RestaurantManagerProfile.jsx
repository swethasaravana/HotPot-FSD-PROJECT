import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getManagerById } from "../../Services/RestaurantManagerService";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import {
  FaUser,
  FaUtensils,
  FaEnvelope,
  FaPhone,
  FaStore,
  FaMapMarkerAlt,
  FaIdCard
} from "react-icons/fa";
import "./RestaurantManagerProfile.css";

const RestaurantManagerProfile = () => {
  const navigate = useNavigate();
  const [manager, setManager] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchManagerData = async () => {
      try {
        setLoading(true);
        const managerIdFromSession = JSON.parse(sessionStorage.getItem("user"))?.id;
        
        if (!managerIdFromSession || isNaN(managerIdFromSession)) {
          throw new Error("Invalid manager ID");
        }

        const response = await getManagerById(managerIdFromSession);
        setManager(response.data);
        toast.success("Profile data loaded successfully!");
      } catch (err) {
        console.error("Failed to fetch manager data:", err);
        toast.error(err.response?.data?.message || err.message || "Failed to load profile");
        
        if (err.response?.status === 401) {
          navigate('/login');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchManagerData();
  }, [navigate]);

  if (loading) {
    return (
      <div className="rmp-loading">
        <div className="rmp-spinner"></div>
        <p>Loading your profile...</p>
      </div>
    );
  }

  if (!manager) {
    return (
      <div className="rmp-no-data">
        <FaUser className="rmp-icon" /> No manager data found
      </div>
    );
  }

  return (
    <div className="rmp-restaurant-manager-profile">
      <h2>
        <FaUser className="rmp-icon" /> Restaurant Manager Profile
      </h2>

      <div className="rmp-info">
        <div className="rmp-info-item">
          <FaUser className="rmp-icon" />
          <p>
            <strong>Full Name:</strong> {manager.fullName}
          </p>
        </div>
        <div className="rmp-info-item">
          <FaEnvelope className="rmp-icon" />
          <p>
            <strong>Email:</strong> {manager.email}
          </p>
        </div>
        <div className="rmp-info-item">
          <FaPhone className="rmp-icon" />
          <p>
            <strong>Phone:</strong> {manager.phone}
          </p>
        </div>
        <div className="rmp-info-item">
          <FaIdCard className="rmp-icon" />
          <p>
            <strong>Username:</strong> {manager.email}
          </p>
        </div>

        <h3 className="rmp-restaurant-heading">
          <FaStore className="rmp-icon" /> Restaurant Information
        </h3>
        
        {manager.restaurantName ? (
          <div className="rmp-restaurant-grid">
            <div className="rmp-info-item">
              <FaStore className="rmp-icon" />
              <p>
                <strong>Name:</strong> {manager.restaurantName}
              </p>
            </div>
            <div className="rmp-info-item">
              <FaMapMarkerAlt className="rmp-icon" />
              <p>
                <strong>Address:</strong> {manager.Location} {manager.restaurantName}
              </p>
            </div>
            <div className="rmp-info-item">
              <FaIdCard className="rmp-icon" />
              <p>
                <strong>License Number: N/A</strong> {manager.restaurantLicenseNumber}
              </p>
            </div>
            {manager.restaurantCuisineType && (
              <div className="rmp-info-item">
                <FaUtensils className="rmp-icon" />
                <p>
                  <strong>Cuisine Type:</strong> {manager.restaurantCuisineType}
                </p>
              </div>
            )}
          </div>
        ) : (
          <p className="rmp-no-data">
            <FaStore className="rmp-icon" /> No restaurant information available
          </p>
        )}
      </div>
    </div>
  );
};

export default RestaurantManagerProfile;