import axiosInstance from "../Filters/AxiosFilters";
import { baseUrl } from "../environments/environment.dev";

// AdminDashBoard
// Add a new delivery partner
export function addDeliveryPartnerAPICall(deliveryPartner) {
  const url = `${baseUrl}/DeliveryPartner/register`;
  return axiosInstance.post(url, deliveryPartner);
}

// Update delivery partners
export function updateDeliveryPartner(id, partnerData) {
  const url = `${baseUrl}/DeliveryPartner/update-partner/${id}`;
  return axiosInstance.put(url, partnerData);
}

// Delete delivery partners
export function deleteDeliveryPartner(id) {
  const url = `${baseUrl}/DeliveryPartner/delete-partner/${id}`;
  return axiosInstance.delete(url);
}

// Fetch all delivery partners
export function getAllDeliveryPartners() {
    const url = `${baseUrl}/DeliveryPartner/Getall`;
    return axiosInstance.get(url);
}


// DeliveryPartnerDashboard
// Get orders for delivery partner
export function GetDeliveryPartnerOrdersAPICall(deliveryPartnerId) {
  const url = `${baseUrl}/Orders/delivery-partner/${deliveryPartnerId}/orders`;
  return axiosInstance.get(url);
}

// Update order delivery status
export const UpdateOrderDeliveryStatusAPICall = async (orderId, statusId) => {
  try {
    const response = await axiosInstance.put(
      `${baseUrl}/DeliveryPartner/delivery/update-status/${orderId}`,
      statusId,  // Send as raw integer value
      {
        headers: {
          'Content-Type': 'application/json'  // Explicitly set content type
        }
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error updating order status:", error);
    throw error;
  }
};

// Get delivery partner by ID
export function getDeliveryPartnerById(partnerId) {
  const url = `${baseUrl}/DeliveryPartner/GetById/${partnerId}`;
  return axiosInstance.get(url);
}
