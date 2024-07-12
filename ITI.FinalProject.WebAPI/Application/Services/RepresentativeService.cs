using Application.DTOs.UpdateDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.ApplicationServices;
using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Microsoft.AspNetCore.Identity;
using Application.DTOs;



namespace Application.Services
{
    public class RepresentativeService:IPaginationService<Representative,RepresentativeDisplayDTO,RepresentativeInsertDTO,RepresentativeUpdateDTO,string>, IDropDownOptionsService<Representative, string>
    {
        private readonly IUnitOfWork unit;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaginationRepository<Representative> repository;
        private readonly IPaginationRepository<GovernorateRepresentatives> GovRepo;

        public RepresentativeService(IUnitOfWork unit,UserManager<ApplicationUser> userManager)
        {
            this.unit = unit;
            this._userManager = userManager;
            this.repository = unit.GetPaginationRepository<Representative>();
            this.GovRepo = unit.GetPaginationRepository<GovernorateRepresentatives>();
        }

        public async Task<List<OptionDTO<string>>> GetOptions(params Expression<Func<Representative, object>>[] includes)
        {
            var options = await repository.GetAllElements(includes);

            var governoratesReps = await GovRepo.GetAllElements(g => g.governorate);

            var governorates = governoratesReps.Select(g => g.governorate).ToList();

            var optionList = new List<OptionDTO<string>>();

            return options.Select(o => new OptionDTO<string>() { Id = o.userId, Name = o.user.FullName, DependentIds = o.governorates.Select(gr => gr.governorateId).ToList() }).ToList();
        }

        public async Task<List<OptionDTO<string>>> GetOptions(Expression<Func<Representative, bool>> filter, params Expression<Func<Representative, object>>[] includes)
        {
            var options = await repository.GetAllElements(filter, includes);
            return options.Select(o => new OptionDTO<string>() { Id = o.userId, Name = o.user.FullName, DependentIds = o.governorates.Select(g => g.governorateId).ToList() }).ToList();
        }

        public async Task<List<RepresentativeDisplayDTO>> GetAllObjects()
        {
            var representatives = await repository.GetAllElements();
            return MapRepresentatives(representatives);
        }
        public async Task<List<RepresentativeDisplayDTO>> GetAllObjects(params Expression<Func<Representative, object>>[] includes)
        {
            var representatives = await repository.GetAllElements(includes);
            return MapRepresentatives(representatives);
        }
        public async Task<RepresentativeDisplayDTO?> GetObject(Expression<Func<Representative, bool>> filter)
        {
            var representative = await repository.GetElement(filter);
            if (representative == null)
            {
                return null;
            }
            return MapRepresentative(representative);
        }
        public async Task<RepresentativeDisplayDTO?> GetObject(Expression<Func<Representative, bool>> filter, params Expression<Func<Representative, object>>[] includes)
        {
            var representative = await repository.GetElement(filter, includes);
            if (representative == null)
            {
                return null;
            }
            return MapRepresentative(representative);
        }
        public async Task<RepresentativeDisplayDTO?> GetObjectWithoutTracking(Expression<Func<Representative, bool>> filter)
        {
            var representative = await repository.GetElementWithoutTracking(filter);
            if (representative == null)
            {
                return null;
            }
            return MapRepresentative(representative);
        }
        public async Task<RepresentativeDisplayDTO?> GetObjectWithoutTracking(Expression<Func<Representative, bool>> filter, params Expression<Func<Representative, object>>[] includes)
        {
            var representative = await repository.GetElementWithoutTracking(filter, includes);
            if (representative == null)
            {
                return null;
            }
            return MapRepresentative(representative);
        }

        public async Task<ModificationResultDTO> InsertObject(RepresentativeInsertDTO ObjectDTO)
        {
            var userAdded = new UserDto()
            {
                Email = ObjectDTO.Email,
                Password = ObjectDTO.Password,
                Address = ObjectDTO.UserAddress,
                FullName = ObjectDTO.UserFullName,
                PhoneNo = ObjectDTO.UserPhoneNo,
                BranchId = ObjectDTO.UserBranchId,
                Status = Domain.Enums.Status.Active,
                UserType = Domain.Enums.UserType.Representative,

            };

            var resultUser = await AddUser(userAdded);

            if(resultUser.Message != null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = resultUser.Message
                };
            }

            var representive = new Representative()
            {
                CompanyPercentage = ObjectDTO.CompanyPercentage,
                DiscountType = ObjectDTO.DiscountType,
                userId = resultUser.UserId,
            };

            var representativeResult = repository.Add(representive);

            if (representativeResult == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the representative"
                };
            }

            var saveResult = await unit.SaveChanges();

            if (saveResult == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error saving the changes"
                };
            }

            var govRepresentativeResult = false;

            foreach (var govId in ObjectDTO.GovernorateIds)
            {
                var governrateRepresentative = new GovernorateRepresentatives()
                {
                    representativeId = resultUser.UserId,
                    governorateId = govId
                };

                govRepresentativeResult = GovRepo.Add(governrateRepresentative);

                if (govRepresentativeResult == false)
                {
                    return new ModificationResultDTO()
                    {
                        Succeeded = false,
                        Message = "Error inserting the governorate representative"
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
        public async Task<ModificationResultDTO> UpdateObject(RepresentativeUpdateDTO ObjectDTO)
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

            user.FullName = ObjectDTO.UserFullName;

            user.BranchId = ObjectDTO.UserBranchId;

            var identityResult = new IdentityResult();

            if (ObjectDTO.OldPassword != null && ObjectDTO.OldPassword != "" && ObjectDTO.NewPassword != null && ObjectDTO.NewPassword != "")
            {
                identityResult = await _userManager.ChangePasswordAsync(user, ObjectDTO.OldPassword, ObjectDTO.NewPassword);
                if (!identityResult.Succeeded)
                {
                    return new ModificationResultDTO()
                    {
                        Succeeded = false,
                        Message = "Error changing user password"
                    };
                }
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, ObjectDTO.Email);

            identityResult = await _userManager.ChangeEmailAsync(user, ObjectDTO.Email, token);

            if (!identityResult.Succeeded)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error changing user email"
                };
            }

            token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, ObjectDTO.UserPhoneNo);

            identityResult = await _userManager.ChangePhoneNumberAsync(user, ObjectDTO.UserPhoneNo, token);

            if (!identityResult.Succeeded)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error changing user phone number"
                };
            }

            user.Status = ObjectDTO.UserStatus;

            identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the user"
                };
            }

            var representative = await repository.GetElement(r => r.userId == ObjectDTO.Id);

            if (representative == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Representative doesn't exist in the db"
                };
            }

            representative.CompanyPercentage = ObjectDTO.CompanyPercentage;
            representative.DiscountType = ObjectDTO.DiscountType;
            var result = repository.Edit(representative);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the representative"
                };
            }

            var representativeGovernorates = await GovRepo.GetAllElements(gr => gr.representativeId == ObjectDTO.Id);

            foreach (var governorateId in ObjectDTO.GovernorateIds)
            {
                if (representativeGovernorates.FirstOrDefault(rg => rg.governorateId == governorateId) == null)
                {
                    result = GovRepo.Add(new GovernorateRepresentatives()
                    {
                        governorateId = governorateId,
                        representativeId = ObjectDTO.Id
                    });

                    if (result == false)
                    {
                        return new ModificationResultDTO()
                        {
                            Succeeded = false,
                            Message = "Error updating the governorate representative"
                        };
                    }
                }
            }

            foreach (var representativeGovernorate in representativeGovernorates)
            {
                if (ObjectDTO.GovernorateIds.FirstOrDefault(gid => gid == representativeGovernorate.governorateId) == 0)
                {
                    result = GovRepo.Delete(representativeGovernorate);

                    if (result == false)
                    {
                        return new ModificationResultDTO()
                        {
                            Succeeded = false,
                            Message = "Error updating the governorate representative"
                        };
                    }
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

            return new ModificationResultDTO()
            {
                Succeeded = true
            };
        }
        public async Task<ModificationResultDTO> DeleteObject(string ObjectId)
        {

            var representative = await repository.GetElement(r => r.userId == ObjectId);

            if (representative == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Representative doesn't exist in the db"
                };
            }

            var representativeGovernorates = await GovRepo.GetAllElements(rg => rg.representativeId == ObjectId);

            var result = false;

            foreach (var representativeGovernorate in representativeGovernorates)
            {
                result = GovRepo.Delete(representativeGovernorate);

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

            result = repository.Delete(representative);
            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the representative"
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
        private RepresentativeDisplayDTO MapRepresentative(Representative representative)
        {
            var RepresentativeDTO = new RepresentativeDisplayDTO()
            {
                Id = representative.userId,
                DiscountType = representative.DiscountType,
                CompanyPercentage = representative.CompanyPercentage,
                UserFullName = representative.user.FullName,
                UserAddress = representative.user.Address,
                Email = representative.user.Email,
                UserPhoneNo = representative.user.PhoneNumber,
                UserStatus = representative.user.Status,
                UserBranchId = (int)representative.user.BranchId!,
                UserType = representative.user.UserType,
                GovernorateIds = representative.governorates.Select(x => x.governorateId).ToList()
            };

            return RepresentativeDTO;
        }
        private List<RepresentativeDisplayDTO> MapRepresentatives(List<Representative> representatives)
        {
            var RepresentativesDTO = representatives.Select(r => new RepresentativeDisplayDTO()
            {
                Id = r.userId,
                DiscountType = r.DiscountType,
                CompanyPercentage = r.CompanyPercentage,
                UserFullName = r.user.FullName,
                UserAddress = r.user.Address,
                Email = r.user.Email,
                UserPhoneNo = r.user.PhoneNumber,
                UserStatus = r.user.Status,
                UserBranchId = (int)r.user.BranchId!,
                UserType = r.user.UserType,
                GovernorateIds = r.governorates.Select(x => x.governorateId).ToList()

            }).ToList();

            return RepresentativesDTO;
        }

        public async Task<ResultUser> AddUser(UserDto userDto)
        {
            if (await _userManager.FindByEmailAsync(userDto.Email) != null)
                return new ResultUser { Message = "Email is Already registered!" };

            if (await _userManager.FindByNameAsync(userDto.FullName.Trim().Replace(' ', '_')) != null)
                return new ResultUser { Message = "UserName is Already registered!" };

            var user = new ApplicationUser
            {
                UserName = userDto.FullName.Trim().Replace(' ', '_'), //userDto.Email,
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

            result = await _userManager.AddToRoleAsync(user, "Representative");

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

        public async Task<PaginationDTO<RepresentativeDisplayDTO>> GetPaginatedOrders(int pageNumber, int pageSize, Expression<Func<Representative, bool>> filter)
        {
            var totalCount = await repository.Count(filter);
            var totalPages = await repository.Pages(pageSize);
            var objectList = await repository.GetPaginatedElements(pageNumber, pageSize, filter, r => r.user, r => r.governorates);

            return new PaginationDTO<RepresentativeDisplayDTO>()
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = MapRepresentatives(objectList.ToList())
            };
        }
    }
}
