import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import HomePage from './Components/HomePage/HomePage';

import Login from './Components/Login/Login';

import SignUp from './Components/SignUp/SignUp';

import AdminDashboard from './Components/AdminDashboard/AdminDashboard';
import AddRestaurant from './Components/AdminDashboard/RestaurantManagement/AddRestaurant';
import ViewRestaurants from './Components/AdminDashboard/RestaurantManagement/ViewRestaurants';
import AddRestaurantManager from './Components/AdminDashboard/RestaurantManagerManagement/AddRestaurantManager';
import ViewAllManagers from './Components/AdminDashboard/RestaurantManagerManagement/ViewAllManagers';
import AddDeliveryPartner from './Components/AdminDashboard/DeliveryPartnerManagement/AddDeliveryPartner'; 
import ViewAllDeliveryPartner from './Components/AdminDashboard/DeliveryPartnerManagement/ViewAllDeliveryPartner';

import DeliveryPartnerDashboard from './Components/DeliveryPartnerDashboard/DeliveryPartnerDashboard';
import DeliveryPartnerProfile from './Components/DeliveryPartnerDashboard/DeliveryPartnerProfile';


import CustomerDashboard from './Components/CustomerDashboard/CustomerDashboard';
import RestaurantMenus from './Components/CustomerDashboard/MenuManagement/RestaurantMenus';
import CartPage from './Components/CustomerDashboard/CartManagement/CartPage';
import CheckoutPage from './Components/CustomerDashboard/CartManagement/CheckoutPage';
import CustomerProfile from './Components/CustomerDashboard/CustomerProfile';
import OrderTracking from './Components/CustomerDashboard/OrderTracking/OrderTracking';

import RestaurantManagerDashboard from './Components/RestaurantManagerDashboard/RestaurantManagerDashboard';
import ViewMenuItems from './Components/RestaurantManagerDashboard/MenuItemManagement/ViewMenuItems';
import AddMenuItemPage from './Components/RestaurantManagerDashboard/MenuItemManagement/AddMenuItemPage';
import RestaurantManagerProfile from './Components/RestaurantManagerDashboard/RestaurantManagerProfile';


function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} />

        <Route path="/login" element={<Login />} />
        <Route path="/SignUp" element={<SignUp />} />

        <Route path="/admindashboard" element={<AdminDashboard />}/>
        <Route path="/admindashboard/add-restaurant" element={<AddRestaurant />} />
        <Route path="/admindashboard/view-restaurants" element={<ViewRestaurants />} />
        <Route path="/admin-dashboard/add-manager" element={<AddRestaurantManager />} />
        <Route path="/admin-dashboard/view-managers" element={<ViewAllManagers />} />
        <Route path="/admindashboard/add-deliverypartner" element={<AddDeliveryPartner />} /> 
        <Route path="/admin-dashboard/view-deliverypartner" element={<ViewAllDeliveryPartner />} />

        <Route path="/deliverydashboard" element={<DeliveryPartnerDashboard  />}/>
        <Route path="/delivery-partner/profile/:partnerId" element={<DeliveryPartnerProfile />} />

        <Route path="/customerdashboard" element={<CustomerDashboard  />}/>
        <Route path="/restaurant-menus/:id" element={<RestaurantMenus />} />
        <Route path="/cart" element={<CartPage />} />
        <Route path="/checkout" element={<CheckoutPage />} />
        <Route path="/customer/profile/:customerId" element={<CustomerProfile />} />
        <Route path="/track-orders" element={<OrderTracking />} />

        <Route path="/restaurantmanager-dashboard" element={<RestaurantManagerDashboard  />}/>
        <Route path="/restaurantmanager-dashboard/view-menu-items" element={<ViewMenuItems />} />
        <Route path="/add-menu-item" element={<AddMenuItemPage />}/>
        <Route path="/restaurant-manager/profile/:managerId" element={<RestaurantManagerProfile />} />


      </Routes>
    </BrowserRouter>
  );
}

export default App;
