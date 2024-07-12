using Application.DTOs;
using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;
using Application.Interfaces.ApplicationServices;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MerchantService : IPaginationService<Merchant, MerchantResponseDto, MerchantAddDto, MerchantUpdateDto, string>
    {
        private readonly IUnitOfWork unit;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaginationRepository<Merchant> repository;
        private readonly IGenericRepository<Branch> branchRepository;
        private readonly IGenericRepository<SpecialPackages> spRepository;
        private readonly IMapper _mapper;

        public MerchantService(IMapper mapper, IUnitOfWork unit, UserManager<ApplicationUser> userManager)
        {
            this.unit = unit;
            this._userManager = userManager;
            this.repository = unit.GetPaginationRepository<Merchant>();
            this.branchRepository = unit.GetPaginationRepository<Branch>();
            this.spRepository = unit.GetPaginationRepository<SpecialPackages>();
            _mapper = mapper;
        }

        public async Task<List<MerchantResponseDto>> GetAllObjects()
        {
            var merchants = await repository.GetAllElements();
            List<MerchantResponseDto> result = new List<MerchantResponseDto>();
            foreach (var merchant in merchants)
                result.Add( await MapMerchant(merchant));
            return result;
        }
        public async Task<List<MerchantResponseDto>> GetAllObjects(params Expression<Func<Merchant, object>>[] includes)
        {
            var merchants = await repository.GetAllElements(includes);
            List<MerchantResponseDto> result = new List<MerchantResponseDto>();
            foreach (var merchant in merchants)
                result.Add( await MapMerchantWithIncludes(merchant));
            return result;
        }
        public async Task<MerchantResponseDto?> GetObject(Expression<Func<Merchant, bool>> filter)
        {
            var merchant = await repository.GetElement(filter);
            if (merchant == null)
            {
                return null;
            }
            return await MapMerchant(merchant);
        }
        public async Task<MerchantResponseDto?> GetObject(Expression<Func<Merchant, bool>> filter, params Expression<Func<Merchant, object>>[] includes)
        {
            var representative = await repository.GetElement(filter, includes);
            if (representative == null)
            {
                return null;
            }
            return await MapMerchantWithIncludes(representative);
        }
        public async Task<MerchantResponseDto?> GetObjectWithoutTracking(Expression<Func<Merchant, bool>> filter)
        {
            var merhcant = await repository.GetElementWithoutTracking(filter);
            if (merhcant == null)
            {
                return null;
            }
            return await MapMerchant(merhcant);
        }
        public async Task<MerchantResponseDto?> GetObjectWithoutTracking(Expression<Func<Merchant, bool>> filter, params Expression<Func<Merchant, object>>[] includes)
        {
            var merhcant = await repository.GetElementWithoutTracking(filter, includes);
            if (merhcant == null)
            {
                return null;
            }
            return await MapMerchant(merhcant);
        }

        public async Task<ModificationResultDTO> InsertObject(MerchantAddDto ObjectDTO)
        {
            var userAdded = new UserDto()
            {
                Email = ObjectDTO.Email,
                Password = ObjectDTO.PasswordHash,
                Address = ObjectDTO.Address,
                FullName = ObjectDTO.UserName,
                PhoneNo = ObjectDTO.PhoneNumber,
                Status = Domain.Enums.Status.Active,
                UserType = Domain.Enums.UserType.Merchant,
                BranchId = ObjectDTO.branchId
            };

            var resultUser = await AddUser(userAdded);

            if (resultUser.Message != null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = resultUser.Message
                };
            }

            var merchant = new Merchant()
            {
                GovernorateId = ObjectDTO.governorateID,
                CityId = ObjectDTO.cityID,
                StoreName = ObjectDTO.StoreName,
                userId = resultUser.UserId,
                MerchantPayingPercentageForRejectedOrders = ObjectDTO.MerchantPayingPercentageForRejectedOrders,
                SpecialPickupShippingCost = ObjectDTO.SpecialPickupShippingCost
            };

            var merchantResult = repository.Add(merchant);

            if (merchantResult == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the representative"
                };
            }

            bool saveResult;

            saveResult = await unit.SaveChanges();

            if (saveResult == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving the changes"
                };
            }



            foreach (var specialPackage in ObjectDTO.SpecialPackages)
            {                
                saveResult = spRepository.Add(new SpecialPackages() {
                    ShippingPrice = specialPackage.ShippingPrice,
                    cityId = specialPackage.cityId,
                    governorateId = specialPackage.governorateId,
                    MerchantId = merchant.userId
                });

                if (saveResult == false)
                {
                    return new ModificationResultDTO()
                    {
                        Succeeded = false,
                        Message = "Error inserting special package"
                    };
                }
            }
            
            saveResult = await unit.SaveChanges();

            if (saveResult == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving the changes"
                };
            }

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }
        public async Task<ModificationResultDTO> UpdateObject(MerchantUpdateDto ObjectDTO)
        {
            var user = await _userManager.FindByIdAsync(ObjectDTO.Id);

            if (user == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "User doesn't exist in the db"
                };
            }

            user.Status = ObjectDTO.Status;
            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the user"
                };
            }

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }
        public async Task<ModificationResultDTO> DeleteObject(string ObjectId)
        {

            var merchant = await repository.GetElement(r => r.userId == ObjectId);

            if (merchant == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Merchant doesn't exist in the db"
                };
            }

            var merchants = await repository.GetAllElements(rg => rg.userId == ObjectId);

            var result = false;

            foreach (var mer in merchants)
            {
                result = repository.Delete(mer);

                if (result == false)
                {
                    return new ModificationResultDTO()
                    {
                        Succeeded = false,
                        Message = "Error deleting the governorate representative"
                    };
                }
            }


            result = await SaveChangesForObject();

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving changes"
                };
            }

            result = repository.Delete(merchant);
            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the Merchant"
                };
            }

            result = await SaveChangesForObject();

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving changes"
                };
            }

            var user = await _userManager.FindByIdAsync(ObjectId);

            if (user == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "User doesn't exist in the db"
                };
            }

            var identityResult = await _userManager.DeleteAsync(user);

            if (identityResult.Succeeded)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = true
                };
            }


            return new ModificationResultDTO()
            {
                Succeeded = false,
                Message = "Error deleting user"
            };
        }

        public async Task<bool> SaveChangesForObject()
        {
            var result = await unit.SaveChanges();

            return result;
        }
        private async Task<MerchantResponseDto> MapMerchant(Merchant merchant)
        {
            var ordersAfterMapper = _mapper.Map<List<DisplayOrderDTO>>(merchant.orders);
            var branch = await branchRepository.GetElement(b => b.id == merchant.user.BranchId);
            var MerchantResponseDto = new MerchantResponseDto()
            {
                MerchantPayingPercentageForRejectedOrders = merchant.MerchantPayingPercentageForRejectedOrders,
                orders = ordersAfterMapper,
                SpecialPackages = MapSpecialPackages(merchant.SpecialPackages, merchant),
                SpecialPickupShippingCost = merchant.SpecialPickupShippingCost,
                StoreName = merchant.StoreName,
                BranchName = branch?.name
            };

            return MerchantResponseDto;
        }

        private async Task<MerchantResponseDto> MapMerchantWithIncludes(Merchant merchant)
        {
            var ordersAfterMapper = _mapper.Map<List<DisplayOrderDTO>>(merchant.orders);
            var branch = await branchRepository.GetElement(b => b.id == merchant.user.BranchId);
            var MerchantResponseDto = new MerchantResponseDto()
            {
                Id = merchant.user.Id,
                Address = merchant.user.Address,
                CityName = merchant.city.name,
                Email = merchant.user.Email,
                GovernorateName = merchant.governorate.name,
                BranchName = branch?.name,
                MerchantPayingPercentageForRejectedOrders = merchant.MerchantPayingPercentageForRejectedOrders,
                orders = ordersAfterMapper,
                PhoneNumber = merchant.user.PhoneNumber,
                SpecialPackages = MapSpecialPackages(merchant.SpecialPackages, merchant),
                SpecialPickupShippingCost = merchant.SpecialPickupShippingCost,
                StoreName = merchant.StoreName,
                UserName = merchant.user.UserName,
                Status = merchant.user.Status
            };

            return MerchantResponseDto;
        }

        private List<SpecialPackageDTO> MapSpecialPackages(List<SpecialPackages> specialPackages, Merchant merchant)
        {
            return  specialPackages.Select(sp => new SpecialPackageDTO()
                    {
                        cityName = merchant.city.name,
                        governorateName = merchant.governorate.name,
                        MerchantName = merchant.user.FullName,
                        ShippingPrice = sp.ShippingPrice
                    }).ToList();
        }

        public async Task<ResultUser> AddUser(UserDto userDto)
        {
            if (await _userManager.FindByEmailAsync(userDto.Email) != null)
                return new ResultUser { Message = "Email is Already registered!" };

            if (await _userManager.FindByNameAsync(userDto.FullName.Trim().Replace(' ', '_')) != null)
                return new ResultUser { Message = "UserName is Already registered!" };

            var user = new ApplicationUser
            {
                UserName = userDto.FullName.Trim().Replace(' ', '_'), 
                Email = userDto.Email,
                FullName = userDto.FullName,
                UserType = userDto.UserType,
                Status = userDto.Status,
                PhoneNumber = userDto.PhoneNo,
                Address = userDto.Address,
                BranchId = userDto.BranchId
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                string errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new ResultUser { Message = errors };
            }

            result = await _userManager.AddToRoleAsync(user, "Merchant");

            if (!result.Succeeded)
            {
                string errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new ResultUser { Message = errors };
            }

            return new ResultUser
            {
                Email = user.Email,
                IsAuthenticated = true,
                Username = user.Email,
                UserId = user.Id

            };

        }

        public async Task<PaginationDTO<MerchantResponseDto>> GetPaginatedOrders(int pageNumber, int pageSize, Expression<Func<Merchant, bool>> filter)
        {
            var totalCount = await repository.Count(filter);
            var totalPages = await repository.Pages(pageSize);
            var objectList = await repository.GetPaginatedElements(pageNumber, pageSize, filter, m => m.governorate, m => m.city, m => m.user, m => m.SpecialPackages);
            var list = new List<MerchantResponseDto>();

            foreach (var item in objectList)
            {
                list.Add(await MapMerchantWithIncludes(item));
            }

            return new PaginationDTO<MerchantResponseDto>()
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = list
            };
        }
    }
}