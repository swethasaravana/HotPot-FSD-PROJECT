import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import axios from "axios";
import { 
  MdDashboard, 
  MdRestaurant, 
  MdPersonAdd, 
  MdDeliveryDining,
  MdMenu,
  MdClose
} from "react-icons/md";
import { FaPlus, FaEdit, FaTrash, FaList, FaUserShield, FaUtensils } from "react-icons/fa";
import { IoMdLogOut } from "react-icons/io";
import './AdminDashboard.css';


const AdminDashboard = () => {
  const navigate = useNavigate();
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [stats, setStats] = useState({
    customers: 0,
    restaurants: 0,
    managers: 0,
    deliveryPartners: 0
  });
  const [activeItem, setActiveItem] = useState("Dashboard");

  // Simulate fetching stats from API
  useEffect(() => {
    const fetchStats = async () => {
      try {
        const response = await axios.get("http://localhost:28827/api/Admin/stats"); 
        setStats({
          customers: response.data.totalCustomers,
          restaurants: response.data.totalRestaurants,
          managers: response.data.totalRestaurantManagers,
          deliveryPartners: response.data.totalDeliveryPartners
        });
      } catch (error) {
        console.error("Failed to fetch stats", error);
      }
    };
    fetchStats();
  }, []);

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

  return (
    <div className={`admin-container ${isSidebarOpen ? "sidebar-open" : "sidebar-closed"}`}>
      {/* Sidebar */}
      <aside className="admin-sidebar">
        <div className="sidebar-header">
          <h2 className="sidebar-title">
            <FaUserShield className="admin-icon" /> Admin Panel
          </h2>
          <button onClick={toggleSidebar} className="sidebar-toggle">
            {isSidebarOpen ? <MdClose /> : <MdMenu />}
          </button>
        </div>
        
        <div className="sidebar-content" >
          <ul>
            <li 
              className={activeItem === "Dashboard" ? "active" : ""}
              onClick={() => handleNavigation("/admindashboard", "Dashboard")}
            >
              <MdDashboard className="nav-icon" />
              <span>Dashboard</span>
            </li>
            
            <li className="sidebar-heading" style={{ fontSize: '12px' }}>
  <MdRestaurant className="section-icon" />
  Restaurant Management
</li>

            <li 
              className={activeItem === "Add Restaurant" ? "active" : ""}
              onClick={() => handleNavigation("/admindashboard/add-restaurant", "Add Restaurant")}
            >
              <FaPlus className="nav-icon" />
              <span>Add Restaurant</span>
            </li>          
            <li 
              className={activeItem === "View Restaurants" ? "active" : ""}
              onClick={() => handleNavigation("/admindashboard/view-restaurants", "View Restaurants")}
            >
              <FaList className="nav-icon" />
              <span>View All</span>
            </li>

            <li className="sidebar-heading">
              <MdPersonAdd className="section-icon" />
              <span>Restaurant Managers</span>
            </li>
            <li 
              className={activeItem === "Add Manager" ? "active" : ""}
              onClick={() => handleNavigation("/admin-dashboard/add-manager", "Add Manager")} //i changed this line
            >
              <FaPlus className="nav-icon" />
              <span>Add</span>
            </li>
            <li 
              className={activeItem === "View Managers" ? "active" : ""}
              onClick={() => handleNavigation("/admin-dashboard/view-managers", "View Managers")}
            >
              <FaList className="nav-icon" />
              <span>View All</span>
            </li>

            <li className="sidebar-heading">
              <MdDeliveryDining className="section-icon" />
              <span>Delivery Partners</span>
            </li>
            <li 
              className={activeItem === "Add Delivery Partner" ? "active" : ""}
              onClick={() => handleNavigation("/admindashboard/add-deliverypartner", "Add Delivery Partner")}
            >
              <FaPlus className="nav-icon" />
              <span>Add</span>
            </li>
            <li 
              className={activeItem === "View Delivery Partners" ? "active" : ""}
              onClick={() => handleNavigation("/admin-dashboard/view-deliverypartner", "View Delivery Partners")}
            >
              <FaList className="nav-icon" />
              <span>View All</span>
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
      <main className="admin-main">
        <div className="admin-header">
          <h1>Welcome, Admin!</h1>
          <p>Here's what's happening with your platform today</p>
        </div>

        <div className="admin-stats">
          <div className="stat-card">
            <div className="stat-icon customers">
              <MdPersonAdd />
            </div>
            <div className="stat-info">
              <h3>Total Customers</h3>
              <p>{stats.customers.toLocaleString()}</p>
            </div>
          </div>
          
          <div className="stat-card">
            <div className="stat-icon restaurants">
              <MdRestaurant />
            </div>
            <div className="stat-info">
              <h3>Total Restaurants</h3>
              <p>{stats.restaurants}</p>
            </div>
          </div>
          
          <div className="stat-card">
            <div className="stat-icon managers">
              <FaUserShield />
            </div>
            <div className="stat-info">
              <h3>Restaurant Managers</h3>
              <p>{stats.managers}</p>
            </div>
          </div>
          
          <div className="stat-card">
            <div className="stat-icon delivery">
              <MdDeliveryDining />
            </div>
            <div className="stat-info">
              <h3>Delivery Partners</h3>
              <p>{stats.deliveryPartners}</p>
            </div>
          </div>
        </div>

        {/* <div className="recent-activity">
          <h2>Recent Activity</h2>
          <div className="activity-list">
            <div className="activity-item">
              <div className="activity-icon new-restaurant">
                <FaPlus />
              </div>
              <div className="activity-content">
                <p>New restaurant "Saffron Bites" added</p>
                <span className="activity-time">2 hours ago</span>
              </div>
            </div>
            <div className="activity-item">
              <div className="activity-icon new-manager">
                <MdPersonAdd />
              </div>
              <div className="activity-content">
                <p>Manager Priya Sharma registered</p>
                <span className="activity-time">5 hours ago</span>
              </div>
            </div>
            <div className="activity-item">
              <div className="activity-icon delivery-update">
                <MdDeliveryDining />
              </div>
              <div className="activity-content">
                <p>3 new delivery partners joined</p>
                <span className="activity-time">Yesterday</span>
              </div>
            </div>
          </div>
        </div> */}
      </main>
    </div>
  );
};

export default AdminDashboard;