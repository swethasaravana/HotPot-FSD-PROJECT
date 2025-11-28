import axiosInstance from "../Filters/AxiosFilters";
import { baseUrl } from "../environments/environment.dev";

//AdminDashBoard
// Add a new restaurant manager
export function addRestaurantManagerAPICall(manager) {
  const url = `${baseUrl}/RestaurantManager/register`;
  return axiosInstance.post(url, manager);
}

// Get all restaurant managers
export function getAllManagersAPICall() {
    const url = `${baseUrl}/RestaurantManager/GetAllManagers`;
    return axiosInstance.get(url);
}

// Update restaurant manager by ID
export function updateRestaurantManagerAPICall(managerId, managerData) {
  const url = `${baseUrl}/RestaurantManager/UpdateManagerById/${managerId}`;  
  return axiosInstance.put(url, managerData);
}

// Delete restaurant manager by ID
export function deleteRestaurantManagerByIdAPICall(managerId) {
  const url = `${baseUrl}/RestaurantManager/manager/${managerId}`;
  return axiosInstance.delete(url);
}


//RestaurantManagerDashBoard
// Get restaurant manager by ID
export function getRestaurantManagerByIdAPICall(managerId) {
  const url = `${baseUrl}/RestaurantManager/GetManagerById/${managerId}`;
  return axiosInstance.get(url);
}

// Get Menu item by Manager
export function GetRestaurantMenuItemsAPICall(managerId) {
  const url = `${baseUrl}/RestaurantManager/GetAllMenuItems/${managerId}`;
  return axiosInstance.get(url);
}

// Add new menu item
export const AddMenuItemAPICall = (managerId, menuItem) => {
  const url = `${baseUrl}/RestaurantManager/manager/${managerId}/menuitems`;
  return axiosInstance.post(url, menuItem);
};

// Get all cuisines
export function GetAllCuisinesAPICall() {
  return axiosInstance.get(`${baseUrl}/Customer/cuisines`);
}

// Get all meal types
export function GetAllMealTypesAPICall() {
  return axiosInstance.get(`${baseUrl}/Customer/mealtypes`);
}

// Update Menu Item by ID
export function updateMenuItemAPICall(menuItemId, updatedData) {
  const url = `${baseUrl}/RestaurantManager/update/${menuItemId}`;
  return axiosInstance.put(url, updatedData);
}

// Delete Menu Item by ID
export function deleteMenuItemAPICall(menuItemId) {
  const url = `${baseUrl}/RestaurantManager/${menuItemId}`;
  return axiosInstance.delete(url);
}

// Add this function to get restaurant orders
export function GetRestaurantOrdersAPICall(managerId) {
  const url = `${baseUrl}/RestaurantManager/${managerId}/orders`;
  return axiosInstance.get(url);
}

// Add to RestaurantManagerService.js
export function UpdateOrderStatusAPICall(orderId, statusId) {
  const managerId = JSON.parse(sessionStorage.getItem("user")).id;
  const url = `${baseUrl}/RestaurantManager/${managerId}/orders/${orderId}/status/${statusId}`;
  return axiosInstance.put(url);
}

// Set availability for all menu items
export function setAvailabilityForAllAPICall() {
  const url = `${baseUrl}/RestaurantManager/availability/auto`;
  return axiosInstance.put(url);
}

// Get restaurant managers by id
export function getManagerById(managerId) {
  const url = `${baseUrl}/RestaurantManager/GetManagerById/${managerId}`;
  return axiosInstance.get(url);
}