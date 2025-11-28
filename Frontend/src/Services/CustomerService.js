import axiosInstance from "../Filters/AxiosFilters";
import { baseUrl } from "../environments/environment.dev";

// Fetch all cuisines
export function getAllCuisines() {
  const url = `${baseUrl}/Customer/cuisines`;
  return axiosInstance.get(url);
}

// Fetch all restaurants
export function getAllRestaurants() {
  const url = `${baseUrl}/Customer/restaurants`;
  return axiosInstance.get(url);
}

// Fetch all menu items
export function getAllMenuItems() {
  const url = `${baseUrl}/Customer/GetAllMenuItems`;
  return axiosInstance.get(url);
}

// Filter
export const filterMenuItems = async (filters) => {
  try {
    const params = new URLSearchParams();
    
    // Only append parameters that have values
    if (filters.minPrice) params.append('minPrice', filters.minPrice);
    if (filters.maxPrice) params.append('maxPrice', filters.maxPrice);
    if (filters.isAvailable) params.append('isAvailable', filters.isAvailable);
    if (filters.cuisineName) params.append('cuisineName', filters.cuisineName);
    if (filters.mealTypeName) params.append('mealTypeName', filters.mealTypeName);
    if (filters.sortBy) params.append('sortBy', filters.sortBy);
    if (filters.sortOrder) params.append('sortOrder', filters.sortOrder);
    
    const response = await axiosInstance.get(`${baseUrl}/Customer/filter?${params.toString()}`);
    return response;
  } catch (error) {
    console.error("Error filtering menu items", error);
    throw error;
  }
};

// Fetch menu items by restaurant ID
export function getMenuByRestaurant(id) {
  return axiosInstance.get(`${baseUrl}/Customer/menus/by-restaurant-id/${id}`);
}

// Fetch a single restaurant by ID
export function getRestaurantById(id) {
  return axiosInstance.get(`${baseUrl}/Admin/restaurants/${id}`);
}

// Fetch cart by customer ID
export function getCartByCustomerId(customerId) {
  const url = `${baseUrl}/Customer/GetCart/${customerId}`;
  return axiosInstance.get(url);
}

// Add a new item to the cart
export function addCartItem(customerId, cartItemCreate) {
  const url = `${baseUrl}/Customer/AddCartItems/${customerId}/items`;
  return axiosInstance.post(url, cartItemCreate);
}

// Update an item in the cart
export function updateCartItem(customerId, cartItemId, cartItemUpdate) {
  const url = `${baseUrl}/Customer/UpdateQuantity/${customerId}/items/${cartItemId}`;
  return axiosInstance.put(url, cartItemUpdate);
}

// Remove an item from the cart
export function removeCartItem(customerId, cartItemId) {
  const url = `${baseUrl}/Customer/DeleteCartItems/${customerId}/items/${cartItemId}`;
  return axiosInstance.delete(url);
}

// Fetch customer addresses
export function getCustomerAddresses(customerId) {
  const url = `${baseUrl}/Customer/customer/${customerId}/addresses`;
  return axiosInstance.get(url);
}

// Add a new address
export function addCustomerAddress(customerId, addressData) {
  const url = `${baseUrl}/Customer/customer/${customerId}/add-address`;
  return axiosInstance.post(url, addressData);
}

// Get customer with address
export function getCustomerWithAddress(customerId) {
  return axiosInstance.get(`${baseUrl}/Customer/with-address/${customerId}`);
}

// // Get customer order history
// export function getCustomerOrderHistory(customerId) {
//   return axiosInstance.get(`${baseUrl}/Customer/history/${customerId}`);
// }

// Place an order
export function placeOrder(orderRequest) {
  const url = `${baseUrl}/Orders/place-order`;
  return axiosInstance.post(url, orderRequest);
}

export function getCustomerOrders(customerId) {
  const url = `${baseUrl}/Orders/customer/${customerId}`;
  return axiosInstance.get(url);
}
