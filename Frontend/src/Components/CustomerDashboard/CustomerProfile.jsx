import React, { useEffect, useState } from "react";
import {
  getCustomerWithAddress,
  getCustomerOrders,
} from "../../Services/CustomerService";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { 
  FaUser, FaHome, FaShoppingBag, 
  FaMapMarkerAlt, FaCity, FaTag,
  FaCalendarAlt, FaReceipt, FaBox,
  FaPhone, FaEnvelope, FaVenusMars,
  FaRupeeSign, FaHashtag
} from "react-icons/fa";
import "./CustomerProfile.css";

const CustomerProfile = () => {
  const customerId = JSON.parse(sessionStorage.getItem("user"))?.id;

  const [activeTab, setActiveTab] = useState("basic");
  const [customer, setCustomer] = useState(null);
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchData() {
      if (!customerId) return;

      try {
        setLoading(true);
        const [customerRes, orderRes] = await Promise.all([
          getCustomerWithAddress(customerId),
          getCustomerOrders(customerId),
        ]);

        setCustomer(customerRes.data);

        const sortedOrders = orderRes.data.sort(
          (a, b) => new Date(b.orderDate) - new Date(a.orderDate)
        );
        setOrders(sortedOrders);
        toast.success("Profile data loaded successfully!");
      } catch (error) {
        console.error("Failed to fetch customer data:", error);
        toast.error("Failed to load profile data. Please try again.");
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, [customerId]);

  if (loading) {
    return (
      <div className="cp-loading">
        <div className="cp-spinner"></div>
        <p>Loading your profile...</p>
      </div>
    );
  }

  return (
    <div className="cp-customer-profile">
      <h2>
        <FaUser className="cp-icon" /> Customer Profile
      </h2>
      <div className="cp-tabs">
        <button
          className={activeTab === "basic" ? "cp-active" : ""}
          onClick={() => setActiveTab("basic")}
        >
          <FaUser className="cp-icon" /> Basic Info
        </button>
        <button
          className={activeTab === "address" ? "cp-active" : ""}
          onClick={() => setActiveTab("address")}
        >
          <FaHome className="cp-icon" /> Addresses
        </button>
        <button
          className={activeTab === "orders" ? "cp-active" : ""}
          onClick={() => setActiveTab("orders")}
        >
          <FaShoppingBag className="cp-icon" /> Orders
        </button>
      </div>

      <div className="cp-tab-content">
        {activeTab === "basic" && customer && (
          <div className="cp-info">
            <div className="cp-info-item">
              <FaUser className="cp-icon" />
              <p>
                <strong>Name:</strong> {customer.name}
              </p>
            </div>
            <div className="cp-info-item">
              <FaVenusMars className="cp-icon" />
              <p>
                <strong>Gender:</strong> {customer.gender}
              </p>
            </div>
            <div className="cp-info-item">
              <FaEnvelope className="cp-icon" />
              <p>
                <strong>Email:</strong> {customer.email}
              </p>
            </div>
            <div className="cp-info-item">
              <FaPhone className="cp-icon" />
              <p>
                <strong>Phone:</strong> {customer.phone}
              </p>
            </div>
          </div>
        )}

        {activeTab === "address" && customer?.addresses && (
          <div className="cp-info">
            <h3>
              <FaHome className="cp-icon" /> Addresses
            </h3>
            {customer.addresses.length > 0 ? (
              <div className="cp-address-grid">
                {customer.addresses.map((address, index) => (
                  <div key={index} className="cp-address-card">
                    <div className="cp-info-item">
                      <FaTag className="cp-icon" />
                      <p>
                        <strong>Label:</strong> {address.label}
                      </p>
                    </div>
                    <div className="cp-info-item">
                      <FaMapMarkerAlt className="cp-icon" />
                      <p>
                        <strong>Street:</strong> {address.street}
                      </p>
                    </div>
                    <div className="cp-info-item">
                      <FaCity className="cp-icon" />
                      <p>
                        <strong>City:</strong> {address.city}
                      </p>
                    </div>
                    <div className="cp-info-item">
                      <FaHashtag className="cp-icon" />
                      <p>
                        <strong>Pincode:</strong> {address.pincode}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="cp-no-data">
                <FaMapMarkerAlt className="cp-icon" /> No address found.
              </p>
            )}
          </div>
        )}

        {activeTab === "orders" && (
          <div className="cp-orders">
            {orders.length > 0 ? (
              orders.map((order) => (
                <div key={order.orderId} className="cp-order-card">
                  <div className="cp-order-header">
                    <div className="cp-info-item">
                      <FaHashtag className="cp-icon" />
                      <p>
                        <strong>Order ID:</strong> {order.orderId}
                      </p>
                    </div>
                    <div className="cp-info-item">
                      <FaCalendarAlt className="cp-icon" />
                      <p>
                        <strong>Date:</strong>{" "}
                        {new Date(order.orderDate).toLocaleDateString()}
                      </p>
                    </div>
                    <div className="cp-info-item">
                      <FaBox className="cp-icon" />
                      <p>
                        <strong>Status:</strong> {order.orderStatus}
                      </p>
                    </div>
                    <div className="cp-info-item">
                      <FaRupeeSign className="cp-icon" />
                      <p>
                        <strong>Total:</strong> ₹{order.totalAmount}
                      </p>
                    </div>
                  </div>

                  <div className="cp-order-items">
                    <h3>
                      <FaReceipt className="cp-icon" /> Items:
                    </h3>
                    {Array.isArray(order.orderItems) &&
                    order.orderItems.length > 0 ? (
                      <div className="cp-items-grid">
                        {order.orderItems.map((item, itemIndex) => (
                          <div key={itemIndex} className="cp-order-item">
                            <div className="cp-info-item">
                              <p>
                                <strong>Item:</strong> {item.menuItemName}
                              </p>
                            </div>
                            <div className="cp-info-item">
                              <p>
                                <strong>Quantity:</strong> {item.quantity}
                              </p>
                            </div>
                            <div className="cp-info-item">
                              <FaRupeeSign className="cp-icon" />
                              <p>
                                <strong>Price:</strong> ₹{item.price}
                              </p>
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <p className="cp-no-data">
                        <FaBox className="cp-icon" /> No items found.
                      </p>
                    )}
                  </div>
                </div>
              ))
            ) : (
              <p className="cp-no-data">
                <FaShoppingBag className="cp-icon" /> No orders found.
              </p>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default CustomerProfile;