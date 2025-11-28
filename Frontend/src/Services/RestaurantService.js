import axiosInstance from "../Filters/AxiosFilters";
import { baseUrl } from "../environments/environment.dev";

// Add a new restaurant
export function addRestaurantAPICall(restaurant) {
  const url = `${baseUrl}/Admin/restaurants`;
  return axiosInstance.post(url, restaurant);
}

// Get all restaurants 
export function getAllRestaurantsAPICall() {
  const url = `${baseUrl}/Admin/restaurants`;
  return axiosInstance.get(url);
}

// Get restaurant by ID
export function getRestaurantByIdAPICall(id) {
  const url = `${baseUrl}/Admin/restaurants/${id}`;
  return axiosInstance.get(url);
}

// Update a restaurant
export function updateRestaurantAPICall(id, restaurant) {
  const url = `${baseUrl}/Admin/restaurants/${id}`;
  return axiosInstance.put(url, restaurant);
}

// Delete a restaurant
export function deleteRestaurantAPICall(id) {
  const url = `${baseUrl}/Admin/restaurants/${id}`;
  return axiosInstance.delete(url);
}