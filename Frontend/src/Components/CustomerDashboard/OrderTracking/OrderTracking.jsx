import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCustomerOrders } from '../../../Services/CustomerService';
import './OrderTracking.css';

const OrderTracking = () => {
  const user = JSON.parse(sessionStorage.getItem("user"));
  const customerId = user?.id;
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const response = await getCustomerOrders(customerId);
        const validatedOrders = Array.isArray(response?.data)
          ? response.data.map(order => ({
              ...order,
              totalAmount: Number(order.totalAmount) || 0,
              orderItems: Array.isArray(order.orderItems)
                ? order.orderItems.map(item => ({
                    ...item,
                    priceAtPurchase: Number(item.priceAtPurchase) || 0
                  }))
                : []
            }))
          : [];
        setOrders(validatedOrders);
      } catch (err) {
        console.error("Error fetching orders:", err);
        setError("Failed to load orders. Please try again.");
      } finally {
        setLoading(false);
      }
    };

    if (customerId) {
      fetchOrders();
    } else {
      navigate('/login');
    }
  }, [customerId, navigate]);

  const getStatusBadge = (status) => {
    const statusMap = {
      'Pending': 'badge-warning',
      'Processing': 'badge-info',
      'Shipped': 'badge-primary',
      'Delivered': 'badge-success',
      'Cancelled': 'badge-danger'
    };
    return statusMap[status] || 'badge-secondary';
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const options = { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' };
      return new Date(dateString).toLocaleDateString(undefined, options);
    } catch {
      return 'Invalid date';
    }
  };

  const formatPrice = (amount) => {
    return typeof amount === 'number' 
      ? `₹${amount.toFixed(2)}` 
      : '₹0.00';
  };

  if (loading) {
    return <div className="loading-spinner">Loading...</div>;
  }

  if (error) {
    return <div className="error-message">{error}</div>;
  }

  return (
    <div className="order-tracking-container">
      <h2>Your Orders</h2>
      
      {orders.length === 0 ? (
        <div className="no-orders">
          <p>You haven't placed any orders yet.</p>
          <button onClick={() => navigate('/')} className="btn-primary">
            Browse Menu
          </button>
        </div>
      ) : (
        // <div className="orders-list">
        //   {orders.map((order) => (
        //     <div key={order.orderId || Math.random()} className="order-card">
        //       <div className="order-header">
        //         <div>
        //           {/* <h3>Order #{order.orderId || 'N/A'}</h3> */}
        //           <h3>Order Details:</h3>
        //           <p className="order-date">Placed on: {formatDate(order.orderDate)}</p>
        //         </div>
        //         <span className={`status-badge ${getStatusBadge(order.orderStatus)}`}>
        //           {order.orderStatus || 'Status unknown'}
        //         </span>
        //       </div>
              
        //       <div className="order-details">
        //         <div className="order-summary">
        //           <p><strong>Total:</strong> {formatPrice(order.totalAmount)}</p>
        //           <p><strong>Payment:</strong> {order.paymentMethod || 'N/A'}</p>
        //           <p><strong>Delivery Address:</strong> {order.customerAddress || 'N/A'}</p>
        //           <p><strong>Delivery Partner:</strong> {order.deliveryPartnerName || 'Not Assigned'}</p>
        //           <p><strong>Phone:</strong> {order.deliveryPartnerPhone || 'N/A'}</p>
        //         </div>
                
        //         <div className="order-items">
        //           <h4>Items:</h4>
        //           <ul>
        //             {(order.orderItems || []).map((item) => (
        //               <li key={item.orderItemId || Math.random()}>
        //                 {item.quantity || 0} × {item.menuItemName || 'Unknown item'} - {formatPrice(item.price)}
        //               </li>
        //             ))}
        //           </ul>
        //         </div>
        //       </div>

        //       {order.orderStatus === 'Shipped' && (
        //         <div className="tracking-info">
        //           <h4>Tracking Information</h4>
        //           <p><strong>Estimated Delivery:</strong> {formatDate(order.estimatedDeliveryDate)}</p>
        //         </div>
        //       )}
        //     </div>
        //   ))}
        // </div>    (If the below lines doest work use the above code)

        <div className="orders-list">
          {orders.map((order, index) => (
            <div key={order.orderId || index} className="order-card">
              <div className="order-header">
                <div>
                  <h3>Order {index + 1} Details:</h3> {/* Dynamically show order number */}
                  <p className="order-date">Placed on: {formatDate(order.orderDate)}</p>
                </div>
                <span className={`status-badge ${getStatusBadge(order.orderStatus)}`}>
                  {order.orderStatus || 'Status unknown'}
                </span>
              </div>

              <div className="order-details">
                <div className="order-summary">
                  <p><strong>Total:</strong> {formatPrice(order.totalAmount)}</p>
                  <p><strong>Payment:</strong> {order.paymentMethod || 'N/A'}</p>
                  <p><strong>Delivery Address:</strong> {order.customerAddress || 'N/A'}</p>
                  <p><strong>Delivery Partner:</strong> {order.deliveryPartnerName || 'Not Assigned'}</p>
                  <p><strong>Phone:</strong> {order.deliveryPartnerPhone || 'N/A'}</p>
                </div>

                <div className="order-items">
                  <h4>Items:</h4>
                  {order.orderItems && order.orderItems.length > 0 ? (
                    <ul>
                      {order.orderItems.map((item, itemIndex) => (
                      <li key={item.orderItemId || itemIndex}>
                        {item.quantity || 0} × {item.menuItemName || 'Unknown item'} - {formatPrice(item.price)}
                      </li>
                        ))}
                    </ul>
                    ) : (
                    <p>No items in this order.</p> // Handling empty items case
                  )}
                </div>
              </div>

              {order.orderStatus === 'Shipped' && (
                <div className="tracking-info">
                  <h4>Tracking Information</h4>
                  <p><strong>Estimated Delivery:</strong> {formatDate(order.estimatedDeliveryDate)}</p>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default OrderTracking;