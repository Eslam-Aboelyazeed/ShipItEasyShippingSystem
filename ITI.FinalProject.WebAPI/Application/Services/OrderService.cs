using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;
using Application.Interfaces.ApplicationServices;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService : IPaginationService<Order, DisplayOrderDTO, InsertOrderDTO, NewOrderUpdateDTO, int>, IUpdateOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginationRepository<Order> _repository;
        private readonly IGenericRepository<City> _cityRepository;
        private readonly IGenericRepository<Settings> _settingsRepository;
        private readonly IGenericRepository<Merchant> _merchantRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Representative> _representativeRepository;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = _unitOfWork.GetPaginationRepository<Order>();
            _cityRepository = _unitOfWork.GetGenericRepository<City>();
            _settingsRepository = _unitOfWork.GetGenericRepository<Settings>();
            _merchantRepository = _unitOfWork.GetGenericRepository<Merchant>();
            _productRepository = _unitOfWork.GetGenericRepository<Product>();
            _representativeRepository = _unitOfWork.GetGenericRepository<Representative>();
        }

        public async Task<List<DisplayOrderDTO>> GetAllObjects()
        {
            var orders = await _repository.GetAllElements();
            var mappedOrders = _mapper.Map<List<DisplayOrderDTO>>(orders);
            for (var i = 0; i < orders.Count; i++)
            {
                var mechant = await _merchantRepository.GetElement(m => m.userId == orders[i].MerchantId, m => m.user);
                mappedOrders[i].MerchantName = mechant?.user.FullName ?? "";

                var representative = await _representativeRepository.GetElement(r => r.userId == orders[i].RepresentativeId, r => r.user);
                mappedOrders[i].RepresentativeName = representative?.user.FullName ?? "";

                mappedOrders[i].CompanyProfit = CalculateCompanyProfit(orders[i]);
            }

            return mappedOrders;
        }

        public async Task<List<DisplayOrderDTO>> GetAllObjects(params System.Linq.Expressions.Expression<Func<Order, object>>[] includes)
        {
            var orders = await _repository.GetAllElements(includes);
            var mappedOrders = _mapper.Map<List<DisplayOrderDTO>>(orders);
            for (var i = 0; i < orders.Count; i++)
            {
                var mechant = await _merchantRepository.GetElement(m => m.userId == orders[i].MerchantId, m => m.user);
                mappedOrders[i].MerchantName = mechant?.user.FullName ?? "";

                var representative = await _representativeRepository.GetElement(r => r.userId == orders[i].RepresentativeId, r => r.user);
                mappedOrders[i].RepresentativeName = representative?.user.FullName ?? "";

                mappedOrders[i].CompanyProfit = CalculateCompanyProfit(orders[i]);
            }

            return mappedOrders;
        }

        public async Task<DisplayOrderDTO?> GetObject(System.Linq.Expressions.Expression<Func<Order, bool>> filter)
        {
            var order = await _repository.GetElement(filter);
            var mappedOrder = _mapper.Map<DisplayOrderDTO>(order);

            if (order != null)
            {                
                var mechant = await _merchantRepository.GetElement(m => m.userId == order.MerchantId, m => m.user);
                mappedOrder.MerchantName = mechant?.user.FullName ?? "";

                var representative = await _representativeRepository.GetElement(r => r.userId == order.RepresentativeId, r => r.user);
                mappedOrder.RepresentativeName = representative?.user.FullName ?? "";

                mappedOrder.CompanyProfit = CalculateCompanyProfit(order);
            }
        
            return mappedOrder;
        }

        public async Task<DisplayOrderDTO?> GetObject(System.Linq.Expressions.Expression<Func<Order, bool>> filter, params System.Linq.Expressions.Expression<Func<Order, object>>[] includes)
        {
            var order = await _repository.GetElement(filter, includes);
            var mappedOrder = _mapper.Map<DisplayOrderDTO>(order);

            if (order != null)
            {
                var mechant = await _merchantRepository.GetElement(m => m.userId == order.MerchantId, m => m.user);
                mappedOrder.MerchantName = mechant?.user.FullName ?? "";

                var representative = await _representativeRepository.GetElement(r => r.userId == order.RepresentativeId, r => r.user);
                mappedOrder.RepresentativeName = representative?.user.FullName ?? "";

                mappedOrder.CompanyProfit = CalculateCompanyProfit(order);
            }

            return mappedOrder;
        }

        public async Task<DisplayOrderDTO?> GetObjectWithoutTracking(System.Linq.Expressions.Expression<Func<Order, bool>> filter)
        {
            var order = await _repository.GetElementWithoutTracking(filter);
            return _mapper.Map<DisplayOrderDTO>(order);
        }

        public async Task<DisplayOrderDTO?> GetObjectWithoutTracking(System.Linq.Expressions.Expression<Func<Order, bool>> filter, params System.Linq.Expressions.Expression<Func<Order, object>>[] includes)
        {
            var order = await _repository.GetElementWithoutTracking(filter, includes);
            return _mapper.Map<DisplayOrderDTO>(order);
        }

        public async Task<ModificationResultDTO> InsertObject(InsertOrderDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            
            order.ShippingCost = await CalculateShipmentCost(order);
            order.Date = DateTime.Now;
            var result =  _repository.Add(order);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the order"
                };
            }

            result = await _unitOfWork.SaveChanges();

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving changes"
                };
            }

            foreach (var productDTO in orderDTO.Products)
            {
                var product = _mapper.Map<Product>(productDTO);
                product.OrderId = order.Id;
                result = _productRepository.Add(product);

                if (result == false)
                {
                    return new ModificationResultDTO()
                    {
                        Succeeded = false,
                        Message = "Error inserting the product"
                    };
                }
            }

            result = await _unitOfWork.SaveChanges();

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving changes"
                };
            }

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }

        public async Task<ModificationResultDTO> UpdateObject(NewOrderUpdateDTO orderDTO)
        {

            var order = await _repository.GetElement(o => o.Id == orderDTO.Id, o => o.merchant, o => o.city, o => o.branch, o => o.governorate, o => o.representative, o => o.Products);

            order!.Status = orderDTO.OrderStatus;

            if (orderDTO.RepresentativeId != null && orderDTO.RepresentativeId != "")
            {
                order!.RepresentativeId = orderDTO.RepresentativeId;
            }

            var result = _repository.Edit(order!);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the order"
                };
            }

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }

        public async Task<ModificationResultDTO> UpdateOrder(DeliveredOrderUpdateDTO orderDTO)
        {
            var order = await _repository.GetElement(o => o.Id == orderDTO.Id, o => o.merchant, o => o.city, o => o.branch, o => o.governorate, o => o.representative, o => o.Products);

            order!.Status = orderDTO.OrderStatus;

            if (orderDTO.OrderMoneyReceived != null) order.OrderMoneyReceived = orderDTO.OrderMoneyReceived;
            if (orderDTO.ShippingMoneyReceived != null) order.ShippingMoneyReceived = orderDTO.ShippingMoneyReceived;
            if (orderDTO.Notes != null) order.Notes = orderDTO.Notes;

            var result = _repository.Edit(order!);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the order"
                };
            }

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }

        public async Task<ModificationResultDTO> DeleteObject(int orderId)
        {
            var order = await _repository.GetElement(x => x.Id == orderId);

            if (order == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Order doesn't exist in the db"
                };
            }

            var result = _repository.Delete(order);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the order"
                };
            }

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }

        public async Task<bool> SaveChangesForObject()
        {
            return await _unitOfWork.SaveChanges();
        }

        public async Task<PaginationDTO<DisplayOrderDTO>> GetPaginatedOrders(int pageNumber, int pageSize, Expression<Func<Order, bool>> filter)
        {
            var totalOrders = await _repository.Count(filter); 
            var totalPages = await _repository.Pages(pageSize);
            var orderList = await _repository.GetPaginatedElements(pageNumber, pageSize, filter, o => o.merchant, o => o.city, o => o.branch, o => o.governorate, o => o.representative, o => o.Products);
            var orders = orderList.ToList();
            var mappedOrders = _mapper.Map<List<DisplayOrderDTO>>(orders);

            for (var i = 0; i < orders.Count; i++)
            {
                var mechant = await _merchantRepository.GetElement(m => m.userId == orders[i].MerchantId, m => m.user);
                mappedOrders[i].MerchantName = mechant?.user.FullName ?? "";

                var representative = await _representativeRepository.GetElement(r => r.userId == orders[i].RepresentativeId, r => r.user);
                mappedOrders[i].RepresentativeName = representative?.user.FullName ?? "";

                mappedOrders[i].CompanyProfit = CalculateCompanyProfit(orders[i]);
            }

            return new PaginationDTO<DisplayOrderDTO>()
            {
                TotalCount = totalOrders,
                TotalPages = totalPages,
                List = mappedOrders
            };
        }

        public async Task<decimal> CalculateShipmentCost(Order order)
        {
            var settings = (await _settingsRepository.GetAllElements())[0];
            var city = await _cityRepository.GetElement(c => c.id == order.CityId);
            var merchant = await _merchantRepository.GetElement(m => m.userId == order.MerchantId, c => c.SpecialPackages);
            var orderType = order.Type;
            var shippingType = order.ShippingType;
            var totalWeight = order.TotalWeight;
            decimal shippingCost = 0;

            switch (orderType)
            {
                case OrderTypes.BranchDelivery:

                    var merchantSpecialPackage = merchant?.SpecialPackages.FirstOrDefault(sp => sp.cityId == order.CityId);
                    
                    if (merchantSpecialPackage != null)
                    {
                        shippingCost += merchantSpecialPackage.ShippingPrice;
                    }
                    else
                    {
                        shippingCost += city!.normalShippingCost;
                    }

                break;

                case OrderTypes.HomeDelivery:

                    if (merchant?.SpecialPickupShippingCost != null)
                    {
                        shippingCost += (decimal)merchant.SpecialPickupShippingCost;
                    }
                    else
                    {
                        shippingCost += city!.pickupShippingCost;
                    }

                break;
            }

            switch (shippingType)
            {
                case ShippingTypes.Ordinary:
                    shippingCost += settings.OrdinaryShippingCost;
                break;

                case ShippingTypes.Within24Hours:
                    shippingCost += settings.TwentyFourHoursShippingCost;
                break;

                case ShippingTypes.Within15Days:
                    shippingCost += settings.FifteenDayShippingCost;
                break;
            }

            if (totalWeight > settings.BaseWeight)
            {
                decimal additionalWeight = totalWeight - settings.BaseWeight;
                decimal totalAdditionalCost = additionalWeight * settings.AdditionalFeePerKg;
                shippingCost += totalAdditionalCost;
            }

            if (order.ShippingToVillage)
            {
                shippingCost += settings.VillageDeliveryFee;
            }

            return shippingCost;
        }

        public decimal? CalculateCompanyProfit(Order order)
        {
            decimal? companyProfit = null; 

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.RepresentativeDelivered || order.Status == OrderStatus.PartiallyDelivered || order.Status == OrderStatus.RejectedWithPartiallyPayment || order.Status == OrderStatus.RejectedWithPayment)
            {
                var representative = order.representative;

                if (representative != null)
                {
                    if (representative.DiscountType == DeductionType.Amount)
                    {
                        companyProfit = (decimal)representative.CompanyPercentage;
                    }
                    else
                    {
                        decimal? relevantAmount = order.ShippingMoneyReceived ?? (order.TotalPrice - order.OrderMoneyReceived);
                        companyProfit = relevantAmount * (decimal)(representative.CompanyPercentage / 100);
                    }
                }
                else
                {
                    companyProfit = order.ShippingMoneyReceived;
                }
            }
            else if (order.Status == OrderStatus.RejectedWithoutPayment)
            {
                companyProfit = (order.merchant.MerchantPayingPercentageForRejectedOrders/100) * order.ShippingCost;
            }

            return companyProfit;
        }
    }
}
