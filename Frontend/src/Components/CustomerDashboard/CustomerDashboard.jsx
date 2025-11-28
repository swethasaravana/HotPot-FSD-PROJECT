import React, { useEffect, useState } from "react";
import "./CustomerDashboard.css";
import { useNavigate } from "react-router-dom";
import { 
  getAllCuisines, 
  getAllRestaurants, 
  getAllMenuItems, 
  filterMenuItems,

} from '../../Services/CustomerService';
import AddToCartButton from "../CustomerDashboard/MenuManagement/AddToCartButton";
import logo from '../../assets/logo.png';
import { IoMdLogOut } from "react-icons/io"; // Logout icon
import { FaShoppingCart, FaUser, FaMapMarkerAlt, FaPhone, FaEnvelope, FaFilter } from "react-icons/fa";
import { FiFilter, FiSearch  } from 'react-icons/fi'; // Various Font Awesome icons

const cuisineImages = {
  "South Indian": "https://png.pngtree.com/png-clipart/20220813/ourmid/pngtree-onam-simple-sadhya-served-in-banana-leaf-png-image_6109188.png",
  "North Indian": "https://images.rawpixel.com/image_png_800/cHJpdmF0ZS9sci9pbWFnZXMvd2Vic2l0ZS8yMDI0LTEwL3Jhd3BpeGVsX29mZmljZV8zM190aGVfYmVzdF9zaG90X29mX2FuX2dyYXBoaWNfZWxlbWVudF9vZl9hX3Npbl9lYmE2OTQ2NC1kOGExLTQwOGEtYjVlYi0zZWFiMzkyMDViZDQucG5n.png",
  "Tandoori": "https://png.pngtree.com/png-clipart/20240506/original/pngtree-tandoori-chicken-indian-food-design-png-image_15018479.png",
  "Biryani": "https://images.rawpixel.com/image_png_800/cHJpdmF0ZS9sci9pbWFnZXMvd2Vic2l0ZS8yMDI0LTEwL3Jhd3BpeGVsb2ZmaWNlNF90b3Bfdmlld19vZl9hX3Bha2lzdGFuX2NoaWNrZW5fYmlyeWFuaV9mb29kX2Rpc19kMjQzYjY1Zi1iN2U4LTQwMmItOGY5Yi1mMzQ3YTliYTE0ZTYucG5n.png",
  "Parotta": "https://static.vecteezy.com/system/resources/previews/045/360/036/non_2x/makki-ki-roti-isolated-on-a-transparent-background-png.png",
  "Pizza": "https://img.freepik.com/free-psd/delicious-veggie-pizza-freshly-baked-toppings-cheese-mushrooms-peppers-olives_84443-37364.jpg",
  "Burger": "https://png.pngtree.com/png-clipart/20221001/original/pngtree-fast-food-big-ham-burger-png-image_8648590.png",
  "Rolls": "https://static.vecteezy.com/system/resources/previews/032/325/388/non_2x/spring-rolls-isolated-on-transparent-background-file-cut-out-ai-generated-png.png",
  "Shawarma": "https://static.vecteezy.com/system/resources/previews/025/222/157/non_2x/shawarma-sandwich-isolated-on-transparent-background-png.png",
  "Momos": "https://png.pngtree.com/png-clipart/20230429/original/pngtree-delicious-momos-with-chutney-free-psd-png-image_9121929.png",
  "Kebab": "https://png.pngtree.com/png-vector/20240524/ourmid/pngtree-healthy-and-delicious-modern-takes-on-seekh-kababs-grill-like-a-png-image_12485842.png",
  "Chinese": "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJcTpnIU2zo54Rygkz_WtrA48EJVvEyK8UVA&s",
  "Pasta": "https://png.pngtree.com/png-vector/20240913/ourmid/pngtree-pasta-with-tomato-in-red-sauce-on-a-white-plate-wooden-png-image_13793012.png",
  "Cake": "https://png.pngtree.com/png-vector/20240923/ourmid/pngtree-angel-cake-isolated-on-white-transparent-background-png-image_13886063.png",
  "Ice Cream": "https://static.vecteezy.com/system/resources/previews/047/818/120/non_2x/ice-cream-in-a-bowl-on-a-transparent-background-free-png.png",
  "Seafood": "https://static.vecteezy.com/system/resources/thumbnails/050/277/245/small/seafood-platter-with-shrimp-mussels-and-clams-free-png.png"
};

function CustomerDashboard() {
  const [cuisines, setCuisines] = useState([]);
  const [restaurants, setRestaurants] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [sortOption, setSortOption] = useState("");
  const [showFilterModal, setShowFilterModal] = useState(false);
  const [filterOptions, setFilterOptions] = useState({
    minPrice: '',
    maxPrice: '',
    isAvailable: true,
    cuisineName: '',
    mealTypeName: '',
    sortBy: '',
    sortOrder: 'asc'
  });
  const [addedItems, setAddedItems] = useState([]);
  const navigate = useNavigate();

  const customerId = JSON.parse(sessionStorage.getItem("user"))?.id;

  useEffect(() => {
    fetchCuisines();
    fetchRestaurants();
    fetchMenuItems();
  }, []);

  const handleLogout = () => {
    sessionStorage.removeItem("user");
    navigate("/login");
  };

  const fetchCuisines = async () => {
    try {
      const response = await getAllCuisines();
      setCuisines(response.data);
    } catch (error) {
      console.error("Error fetching cuisines", error);
    }
  };

  const fetchRestaurants = async () => {
    try {
      const response = await getAllRestaurants();
      setRestaurants(response.data);
    } catch (error) {
      console.error("Error fetching restaurants", error);
    }
  };

  const fetchMenuItems = async () => {
    try {
      const response = await getAllMenuItems();
      setMenuItems(response.data);
    } catch (error) {
      console.error("Error fetching menu items", error);
      setMenuItems([]);
    }
  };

  const fetchFilteredMenuItems = async () => {
    try {
      const response = await filterMenuItems({
        minPrice: filterOptions.minPrice || null,
        maxPrice: filterOptions.maxPrice || null,
        isAvailable: filterOptions.isAvailable,
        cuisineName: filterOptions.cuisineName || null,
        mealTypeName: filterOptions.mealTypeName || null,
        sortBy: filterOptions.sortBy || null,
        sortOrder: filterOptions.sortOrder || null
      });
      setMenuItems(response.data || []);
    } catch (error) {
      console.error("Error filtering menu items", error);
      setMenuItems([]);
    }
  };

  const handleFilterChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFilterOptions(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const applyFilters = () => {
    fetchFilteredMenuItems();
    setShowFilterModal(false);
  };

  const resetFilters = () => {
    setFilterOptions({
      minPrice: '',
      maxPrice: '',
      isAvailable: true,
      cuisineName: '',
      mealTypeName: '',
      sortBy: '',
      sortOrder: 'asc'
    });
    fetchMenuItems();
    setShowFilterModal(false);
  };

  const handleItemAdded = (itemId) => {
    setAddedItems((prevItems) => [...prevItems, itemId]);
  };

  const filteredMenuItems = menuItems.filter(item => {
    if (!item) return false;
    return (
      item.itemName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      // item.description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      item.cuisine?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      item.mealType?.toLowerCase().includes(searchTerm.toLowerCase())
    );
  });

  const sortedMenuItems = [...filteredMenuItems].sort((a, b) => {
    if (sortOption === "Price Low to High") return a.price - b.price;
    if (sortOption === "Price High to Low") return b.price - a.price;
    if (sortOption === "Rating") return b.rating - a.rating;
    return 0;
  });

  const cuisineOptions = [...new Set(menuItems.map(item => item.cuisine).filter(Boolean))];
  const mealTypeOptions = [...new Set(menuItems.map(item => item.mealType).filter(Boolean))];

  return (
    <div className="customer-dashboard">
      <header className="header">
        <div className="CD-logo">
          <img src={logo} alt="HotByte Logo" 
          onError={(e) => {e.target.onerror = null; e.target.src = '/default-logo.png';}}/>
        </div>
        <div className="search-bar">
        {/* <FiSearch className="search-icon" /> */}
          <input 
            type="text" 
            placeholder="🔍 Search for dishes..." 
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        
        <div className="header-icons">
          <button className="track-orders" onClick={() => navigate('/track-orders')}>
            <FaMapMarkerAlt className="header-icon" />
              <span>Track Orders</span>
          </button>

          <button className="cart" onClick={() => navigate('/cart')}>
            {/* You can enable the cart count like below */}
            {/* <i className="fa fa-shopping-cart"></i> */}
            <FaShoppingCart className="header-icon" />
            <span>Cart</span>
          </button>

          <button className="profile" onClick={() => navigate(`/customer/profile/${customerId}`)}>
          <FaUser className="header-icon" />
            <span>Profile</span>
          </button>

          <button className="logout" onClick={handleLogout}>
            <IoMdLogOut className="logout-icon" />
              <span>Logout</span>
          </button>

        </div>
      </header>

      <section className="whats-on-mind">
        <h2>What's on your mind?</h2>
        <div className="cuisine-list">
          {cuisines.map((cuisine, index) => (
            <div className="cuisine-card" key={cuisine.id || cuisine.name || index}>
              <img
                src={cuisineImages[cuisine.name] || cuisineImages["South Indian"]}
                alt={cuisine.name}
                className="cuisine-image"
                onError={(e) => {
                  e.target.onerror = null;
                  e.target.src = cuisineImages["South Indian"];
                }}
              />
              <p>{cuisine.name}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="top-restaurants">
        <h2>Top restaurants in Chennai</h2>
        <div className="restaurant-list">
          {restaurants.map((restaurant, index) => (
            <div 
              className="restaurant-card" 
              key={restaurant.restaurantId || restaurant.restaurantName || index}
              onClick={() => navigate(`/restaurant-menus/${restaurant.restaurantId}`)}
            >  
              <img
                src={restaurant.restaurantlogo || "https://via.placeholder.com/150"}
                alt={`${restaurant.restaurantName} Logo`}
                className="restaurant-logo"
                onError={(e) => {
                  e.target.onerror = null;
                  e.target.src = "https://via.placeholder.com/150";
                }}
              />
              <div className="restaurant-info">
                <h3>{restaurant.restaurantName}</h3>
                <p><i className="location-icon"></i> {restaurant.location}</p>
                <p><i className="phone-icon"></i> {restaurant.contactNumber}</p>
                <p><i className="email-icon"></i> {restaurant.email}</p>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="explore-menu">
        <div className="filters-header">
          <h2>Menu Items across various parts of Chennai</h2>
          
          <div className="filter-controls">
            <select 
              value={sortOption}
              onChange={(e) => setSortOption(e.target.value)}>
                <option value="">Sort by (Default)</option>
                <option value="Price Low to High">Price Low to High</option>
                <option value="Price High to Low">Price High to Low</option>
                {/* <option value="Rating">Rating</option> */}
            </select>

            <button className="filter-button" onClick={() => setShowFilterModal(true)}>
              <FiFilter className="filter-icon" /> Filter
            </button>
          </div>
        </div>

        {showFilterModal && (
          <div className="filter-modal">
            <div className="modal-content">
              <h3>Filter Menu Items</h3>
              <button 
                className="close-modal"
                onClick={() => setShowFilterModal(false)}
              >
                &times;
              </button>

              <div className="filter-group">
                <label>Price Range</label>
                <div className="price-range">
                  <input
                    type="number"
                    placeholder="Min"
                    name="minPrice"
                    value={filterOptions.minPrice}
                    onChange={handleFilterChange}
                  />
                  <span>to</span>
                  <input
                    type="number"
                    placeholder="Max"
                    name="maxPrice"
                    value={filterOptions.maxPrice}
                    onChange={handleFilterChange}
                  />
                </div>
              </div>

              <div className="filter-group">
                <label>
                  <input
                    type="checkbox"
                    name="isAvailable"
                    checked={filterOptions.isAvailable}
                    onChange={handleFilterChange}
                  />
                  Available Only
                </label>
              </div>

              <div className="filter-group">
                <label>Cuisine</label>
                <select
                  name="cuisineName"
                  value={filterOptions.cuisineName}
                  onChange={handleFilterChange}
                >
                  <option value="">All Cuisines</option>
                  {cuisineOptions.map((cuisine, index) => (
                    <option key={index} value={cuisine}>{cuisine}</option>
                  ))}
                </select>
              </div>

              <div className="filter-group">
                <label>Meal Type</label>
                <select
                  name="mealTypeName"
                  value={filterOptions.mealTypeName}
                  onChange={handleFilterChange}
                >
                  <option value="">All Meal Types</option>
                  {mealTypeOptions.map((mealType, index) => (
                    <option key={index} value={mealType}>{mealType}</option>
                  ))}
                </select>
              </div>

              <div className="filter-group">
                <label>Sort By</label>
                <select
                  name="sortBy"
                  value={filterOptions.sortBy}
                  onChange={handleFilterChange}
                >
                  <option value="">Default</option>
                  <option value="price">Price</option>
                  <option value="name">Name</option>
                </select>
                <select
                  name="sortOrder"
                  value={filterOptions.sortOrder}
                  onChange={handleFilterChange}
                >
                  <option value="asc">Ascending</option>
                  <option value="desc">Descending</option>
                </select>
              </div>

              <div className="modal-actions">
                <button className="apply-btn" onClick={applyFilters}>Apply Filters</button>
                <button className="reset-btn" onClick={resetFilters}>Reset</button>
              </div>
            </div>
          </div>
        )}

        <div className="menu-item-list">
          {sortedMenuItems.length > 0 ? (
            sortedMenuItems.map((item, index) => (
              <div className="menu-card" key={item.id || item.name || index}>
                <div className="menu-item-image">
                  <img
                    src={item.imagePath} 
                    alt={item.itemName || "Menu Item"}
                    onError={(e) => {
                      e.target.onerror = null;
                      // e.target.src = "https://via.placeholder.com/150";
                    }}
                  />
                </div>
                <div className="menu-item-details">
                  <h4>{item.itemName || "Menu Item"}</h4>
                  <p className="description">{item.description || "Delicious food item"}</p>
                  {item.cookingTime && <p className="cookingTime">Ready in: {item.cookingTime}</p>}
                  <p className="price">₹{item.price || "0"}</p>
                  <AddToCartButton 
                    item={item} 
                    customerId={customerId}
                    onItemAdded={() => handleItemAdded(item.id)}
                  />
                  <p className="Restaurant Name">{item.restaurantName || "Restaurant Name Not Found "}</p>
                </div>
              </div>
            ))
          ) : (
            <div className="no-items-message">
              <img src="https://cdn-icons-png.flaticon.com/512/4076/4076478.png" alt="No items" width="100" />
              <p>No menu items found matching your criteria</p>
              <button className="clear-filters-btn" onClick={resetFilters}> Clear Filters </button>
            </div>
          )}
        </div>
      </section>

      <footer className="footer">
        <p>© 2025 HotPot. All rights reserved.</p>
      </footer>
    </div>
  );
}

export default CustomerDashboard;