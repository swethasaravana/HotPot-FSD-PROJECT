import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  MdDashboard,
  MdRestaurant,
  MdPerson,
  MdListAlt,
  MdPendingActions,
  MdDoneAll,
  MdMenu,
  MdClose,
  MdAddCircle,
  MdFastfood
} from 'react-icons/md';
import { IoMdLogOut } from 'react-icons/io';
import { 
  GetRestaurantMenuItemsAPICall,
  GetRestaurantOrdersAPICall,
  UpdateOrderStatusAPICall,
  AddMenuItemAPICall
} from '../../Services/RestaurantManagerService';
import './RestaurantManagerDashboard.css';

const RestaurantManagerDashboard = () => {
  const navigate = useNavigate();
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [activeItem, setActiveItem] = useState("Dashboard");
  const [orders, setOrders] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState({
    orders: false,
    menuItems: false,
    statusUpdate: false,
    addMenuItem: false
  });
  const [restaurantManager, setRestaurantManager] = useState(null);
  const [error, setError] = useState(null);
  const [newMenuItem, setNewMenuItem] = useState({
    name: '',
    description: '',
    price: '',
    category: ''
  });

  useEffect(() => {
    const user = JSON.parse(sessionStorage.getItem("user"));
    if (!user || user.role.toLowerCase() !== "restaurantmanager") {
      navigate("/login");
      return;
    }
    setRestaurantManager(user);
    fetchOrders(user.id);
    fetchMenuItems(user.id);
  }, [navigate]);

  const fetchOrders = async (restaurantManagerId) => {
    try {
      setLoading(prev => ({ ...prev, orders: true }));
      setError(null);
      const response = await GetRestaurantOrdersAPICall(restaurantManagerId);
  
      const formattedOrders = response.data.map(order => ({
        ...order,
        customerName: order.customer?.name || 'Unknown Customer',
        deliveryAddress: order.deliveryAddress || 'Address not specified',
        orderItems: order.orderItems?.map(item => ({
          itemName: item.menuItem?.name || 'Unknown Item',
          quantity: item.quantity,
          price: item.price
        })) || [],
        statusId: order.orderStatusId || 1
      }));
  
      // Sort by orderId in descending order
      const sortedOrders = formattedOrders.sort((a, b) => b.orderId - a.orderId);
      
      setOrders(sortedOrders);
    } catch (error) {
      console.error("Error fetching orders:", error);
      setError("Failed to load orders. Please try again.");
      setOrders([]);
    } finally {
      setLoading(prev => ({ ...prev, orders: false }));
    }
  };
  

  const fetchMenuItems = async (restaurantManagerId) => {
    try {
      setLoading(prev => ({ ...prev, menuItems: true }));
      const response = await GetRestaurantMenuItemsAPICall(restaurantManagerId);
      setMenuItems(response.data);
    } catch (error) {
      console.error("Error fetching menu items:", error);
      setError("Failed to load menu items. Please try again.");
      setMenuItems([]);
    } finally {
      setLoading(prev => ({ ...prev, menuItems: false }));
    }
  };

  const toggleSidebar = () => {
    setIsSidebarOpen(!isSidebarOpen);
  };

  const handleNavigation = (path, itemName) => {
    navigate(path);
    setActiveItem(itemName);
  };

  const handleLogout = () => {
    sessionStorage.removeItem("user");
    navigate("/login");
  };

  const updateOrderStatus = async (orderId, newStatusId) => {
    try {
      setLoading(prev => ({ ...prev, statusUpdate: true }));
      
      // Validate status transition (restaurant can only manage statuses 1-4)
      const validStatuses = [2, 3, 4, 7]; // Confirm, Preparing, Ready for Pickup, Cancel
      if (!validStatuses.includes(newStatusId)) {
        throw new Error("Invalid status transition for restaurant");
      }
  
      await UpdateOrderStatusAPICall(orderId, newStatusId);
      
      setOrders(prevOrders => 
        prevOrders.map(order => 
          order.orderId === orderId ? { 
            ...order, 
            statusId: newStatusId,
            isComplete: [4, 6, 7].includes(newStatusId) // Complete if Ready, Delivered or Cancelled
          } : order
        )
      );
      
      alert(`Order status updated to ${getStatusText(newStatusId)}`);
    } catch (error) {
      console.error("Update error:", error);
      alert(error.response?.data || error.message || "Failed to update order status");
    } finally {
      setLoading(prev => ({ ...prev, statusUpdate: false }));
    }
  };

  const handleAddMenuItem = async (e) => {
    e.preventDefault();
    try {
      setLoading(prev => ({ ...prev, addMenuItem: true }));
      await AddMenuItemAPICall(restaurantManager.id, newMenuItem);
      await fetchMenuItems(restaurantManager.id);
      setNewMenuItem({
        name: '',
        description: '',
        price: '',
        category: ''
      });
      alert("Menu item added successfully!");
    } catch (error) {
      console.error("Error adding menu item:", error);
      alert("Failed to add menu item. Please try again.");
    } finally {
      setLoading(prev => ({ ...prev, addMenuItem: false }));
    }
  };

  const handleMenuItemChange = (e) => {
    const { name, value } = e.target;
    setNewMenuItem(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const getStatusText = (statusId) => {
    switch(statusId) {
      case 1: return "Order Placed";
      case 2: return "Order Confirmed";
      case 3: return "Preparing";
      case 4: return "Ready for Pick Up";
      case 5: return "Out for Delivery";
      case 6: return "Delivered";
      case 7: return "Cancelled";
      default: return "Unknown Status";
    }
  };

  const getNextStatus = (currentStatusId) => {
    switch(currentStatusId) {
      case 1: return { id: 2, text: "Confirm Order" }; // Placed -> Confirmed
      case 2: return { id: 3, text: "Start Preparing" }; // Confirmed -> Preparing
      case 3: return { id: 4, text: "Mark as Ready" }; // Preparing -> Ready
      default: return null;
    }
  };

  const canCancelOrder = (statusId) => {
    // Can cancel orders that are not yet ready or already completed
    return [1, 2, 3].includes(statusId);
  };

  return (
    <div className={`restaurant-container ${isSidebarOpen ? "sidebar-open" : "sidebar-closed"}`}>
      {/* Sidebar */}
      <aside className="restaurant-sidebar">
        <div className="sidebar-header">
          <h2 className="sidebar-title">
            <MdRestaurant className="restaurant-icon" /> Restaurant Panel
          </h2>
          <button onClick={toggleSidebar} className="sidebar-toggle">
            {isSidebarOpen ? <MdClose /> : <MdMenu />}
          </button>
        </div>
        
        <div className="sidebar-content">
          <ul>
            <li 
              className={activeItem === "Dashboard" ? "active" : ""}
              onClick={() => handleNavigation("/restaurant-dashboard", "Dashboard")}
            >
              <MdDashboard className="nav-icon" />
              <span>Dashboard</span>
            </li>
            
            <li 
              className={activeItem === "Profile" ? "active" : ""}
              onClick={() => handleNavigation("/restaurant-manager/profile/:managerId", "Profile")}
            >
              <MdPerson className="nav-icon" />
              <span>Profile</span>
            </li>
            
            <li 
              className={activeItem === "AddMenuItem" ? "active" : ""}
              onClick={() => handleNavigation("/add-menu-item", "AddMenuItem")}
            >
              <MdAddCircle className="nav-icon" />
              <span>Add Menu Item</span>
            </li>
            
            <li 
              className={activeItem === "MenuItems" ? "active" : ""}
              onClick={() => handleNavigation("/restaurantmanager-dashboard/view-menu-items", "MenuItems")}
            >
              <MdFastfood className="nav-icon" />
              <span>Menu Items</span>
            </li>
          </ul>
        </div>

        <div className="sidebar-footer">
          <button onClick={handleLogout} className="logout-btn">
            <IoMdLogOut className="logout-icon" />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="restaurant-main">
        {activeItem === "Profile" && restaurantManager && (
          <div className="profile-section">
            <div className="profile-header">
              <h1>Your Profile</h1>
            </div>
            <div className="profile-details">
              <div className="detail-row">
                <span className="detail-label">Name:</span>
                <span className="detail-value">{restaurantManager.name}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Email:</span>
                <span className="detail-value">{restaurantManager.email}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Phone:</span>
                <span className="detail-value">{restaurantManager.phoneNumber}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Restaurant:</span>
                <span className="detail-value">{restaurantManager.restaurantName}</span>
              </div>
            </div>
          </div>
        )}

        {activeItem === "AddMenuItem" && (
          <div className="add-menu-item-section">
            <div className="section-header">
              <h1>Add New Menu Item</h1>
            </div>
            <form onSubmit={handleAddMenuItem} className="menu-item-form">
              <div className="form-group">
                <label>Item Name</label>
                <input
                  type="text"
                  name="name"
                  value={newMenuItem.name}
                  onChange={handleMenuItemChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Description</label>
                <textarea
                  name="description"
                  value={newMenuItem.description}
                  onChange={handleMenuItemChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Price (₹)</label>
                <input
                  type="number"
                  name="price"
                  value={newMenuItem.price}
                  onChange={handleMenuItemChange}
                  min="0"
                  step="0.01"
                  required
                />
              </div>
              <div className="form-group">
                <label>Category</label>
                <input
                  type="text"
                  name="category"
                  value={newMenuItem.category}
                  onChange={handleMenuItemChange}
                  required
                />
              </div>
              <button 
                type="submit" 
                className="submit-btn" 
                disabled={loading.addMenuItem}
              >
                {loading.addMenuItem ? "Adding..." : "Add Menu Item"}
              </button>
            </form>
          </div>
        )}

        {activeItem === "MenuItems" && (
          <div className="menu-items-section">
            <div className="section-header">
              <h1>Menu Items</h1>
            </div>
            {loading.menuItems ? (
              <div className="loading-spinner"></div>
            ) : (
              <div className="menu-items-list">
                {menuItems.length === 0 ? (
                  <p className="no-items">No menu items found.</p>
                ) : (
                  <table className="menu-items-table">
                    <thead>
                      <tr>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Price (₹)</th>
                        <th>Category</th>
                      </tr>
                    </thead>
                    <tbody>
                      {menuItems.map(item => (
                        <tr key={item.menuItemId}>
                          <td>{item.name}</td>
                          <td>{item.description}</td>
                          <td>{item.price.toFixed(2)}</td>
                          <td>{item.category}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            )}
          </div>
        )}

        {activeItem === "Dashboard" && (
          <>
            <div className="restaurant-header">
              <h1>Welcome, {restaurantManager?.name || 'Restaurant Manager'}!</h1>
              <p>Manage your restaurant orders</p>
            </div>

            {error && (
              <div className="error-message">
                {error}
              </div>
            )}

            {loading.orders ? (
              <div className="loading-spinner"></div>
            ) : (
              <div className="order-list">
                {orders.length === 0 ? (
                  <p className="no-orders">No orders received yet.</p>
                ) : (
                  orders.map(order => (
                    <div className={`order-card status-${order.statusId}`} key={order.orderId}>
                      <div className="order-header">
                        <h3>Order #{order.orderId}</h3>
                        <span className={`status-badge status-${order.statusId}`}>
                          {getStatusText(order.statusId)}
                        </span>
                      </div>
                      
                      <div className="order-details">
                        <p><strong>Customer:</strong> {order.customerName}</p>
                        {/* <p><strong>Delivery Address:</strong> {order.deliveryAddress}</p> */}
                        <p><strong>Total Amount:</strong> ₹{order.totalAmount || '0.00'}</p>
                        
                        <div className="order-items">
                          <strong>Items:</strong>
                          <ul>
                            {order.orderItems.map((item, index) => (
                              <li key={index}>
                                {item.quantity}x {item.itemName} (₹{item.price || '0.00'})
                              </li>
                            ))}
                          </ul>
                        </div>
                      </div>
                      
                      <div className="order-actions">
                        {getNextStatus(order.statusId) && (
                          <button 
                            className="action-btn"
                            onClick={() => updateOrderStatus(order.orderId, getNextStatus(order.statusId).id)}
                            disabled={loading.statusUpdate}
                          >
                            {loading.statusUpdate ? "Processing..." : getNextStatus(order.statusId).text}
                          </button>
                        )}
                        
                        {canCancelOrder(order.statusId) && (
                          <button 
                            className="action-btn cancel"
                            onClick={() => updateOrderStatus(order.orderId, 7)}
                            disabled={loading.statusUpdate}
                          >
                            {loading.statusUpdate ? "Processing..." : "Cancel Order"}
                          </button>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
};

export default RestaurantManagerDashboard;