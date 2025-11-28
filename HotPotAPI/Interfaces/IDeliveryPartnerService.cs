using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;

namespace HotPotAPI.Interfaces
{
    public interface IDeliveryPartnerService
    {
        Task<CreateDeliveryPartnerResponse> AddDeliveryPartner(CreateDeliveryPartnerRequest request);
        Task<DeliveryPartner?> UpdateDeliveryPartner(int partnerId, DeliveryPartnerUpdate updatedData);
        Task<bool> DeleteDeliveryPartner(int partnerId);
        Task<DeliveryPartner> GetById(int id);
        Task<DeliveryPartner> GetDeliveryPartnerById(int partnerId);

        Task<DeliveryPartner> Update(int id, DeliveryPartner entity);
        Task<List<DeliveryPartner>> GetAllDeliveryPartners();
        Task<int> GetTotalDeliveryPartners();
    }
}
