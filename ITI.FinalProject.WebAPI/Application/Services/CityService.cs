using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;
using Application.Interfaces.ApplicationServices;
using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Domain.Services
{
    public class CityService : IPaginationService<City,CityDisplayDTO,CityInsertDTO,CityUpdateDTO,int>, IDropDownOptionsService<City, int>
    {
        public IPaginationRepository<City> CityRepo;
        public IUnitOfWork Unit;
        public CityService( IUnitOfWork unit)
        {
            CityRepo= unit.GetPaginationRepository<City>(); 
            Unit = unit;
        }

        public async Task<List<OptionDTO<int>>> GetOptions(params Expression<Func<City, object>>[] includes)
        {
            var options = await CityRepo.GetAllElements(includes);
            return options.Select(o => new OptionDTO<int>() { Id = o.id, Name = o.name, DependentId = o.governorateId }).ToList();
        }

        public async Task<List<OptionDTO<int>>> GetOptions(Expression<Func<City, bool>> filter, params Expression<Func<City, object>>[] includes)
        {
            var options = await CityRepo.GetAllElements(filter, includes);
            return options.Select(o => new OptionDTO<int>() { Id = o.id, Name = o.name, DependentId = o.governorateId }).ToList();
        }


        public async Task<List<CityDisplayDTO>> GetAllObjects()
        {
            List<City>? Cities = await CityRepo.GetAllElements();
            if(Cities == null)
            {
                return null;
            }
            List<CityDisplayDTO> CitiesDTO = new List<CityDisplayDTO>();
            foreach (var item in Cities)
            {
                CitiesDTO.Add(
                    new CityDisplayDTO
                    {
                        id = item.id,
                        name = item.name,
                        status = item.status,
                        normalShippingCost = item.normalShippingCost,
                        pickupShippingCost = item.pickupShippingCost,
                        governorateId = item.governorateId
                    }
                );    
            }
            return CitiesDTO;
        }

        public  async Task<List<CityDisplayDTO>> GetAllObjects(params Expression<Func<City, object>>[] includes)
        {
            List<City>? Cities = await CityRepo.GetAllElements(includes);
            if (Cities == null)
            {
                return null;
            }
            List<CityDisplayDTO> CitysDTO = new List<CityDisplayDTO>();
            foreach (var item in Cities)
            {
                CitysDTO.Add(
                    new CityDisplayDTO
                    {
                        name = item.name,
                        status = item.status,
                        normalShippingCost = item.normalShippingCost,
                        pickupShippingCost = item.pickupShippingCost,
                        governorateId = item.governorateId
                    }
                );
            }
            return CitysDTO;
        }

        public async Task<CityDisplayDTO?> GetObject(Expression<Func<City, bool>> filter)
        {
            City?  City = await CityRepo.GetElement(filter);
            if (City == null)
            {
                return null;
            }
            CityDisplayDTO CityDTO = new CityDisplayDTO(){
                name = City.name,
                status = City.status,                
                normalShippingCost = City.normalShippingCost,
                pickupShippingCost = City.pickupShippingCost,
                governorateId = City.governorateId
            };
            
            return CityDTO;
        }

       

        public async Task<CityDisplayDTO?> GetObject(Expression<Func<City, bool>> filter, params Expression<Func<City, object>>[] includes)
        {
            City? City = await CityRepo.GetElement(filter, includes);
            if (City == null)
            {
                return null;
            }
            CityDisplayDTO CityDTO = new CityDisplayDTO()
            {
                name = City.name,
                status = City.status,
                normalShippingCost = City.normalShippingCost,
                pickupShippingCost = City.pickupShippingCost,
                governorateId = City.governorateId
            };

            return CityDTO;
        }

        public async Task<CityDisplayDTO?> GetObjectWithoutTracking(Expression<Func<City, bool>> filter)
        {
            City? City = await CityRepo.GetElementWithoutTracking(filter);
            if (City == null)
            {
                return null;
            }
            CityDisplayDTO CityDTO = new CityDisplayDTO()
            {
                name = City.name,
                status = City.status,
                normalShippingCost = City.normalShippingCost,
                pickupShippingCost = City.pickupShippingCost,
                governorateId = City.governorateId
            };

            return CityDTO;
        }


        public async Task<CityDisplayDTO?> GetObjectWithoutTracking(Expression<Func<City, bool>> filter, params Expression<Func<City, object>>[] includes)
        {
            City? City = await CityRepo.GetElementWithoutTracking(filter,includes);
            if (City == null)
            {
                return null;
            }
            CityDisplayDTO CityDTO = new CityDisplayDTO()
            {
                name = City.name,
                status = City.status,
                normalShippingCost = City.normalShippingCost,
                pickupShippingCost = City.pickupShippingCost,
                governorateId = City.governorateId
            };

            return CityDTO;
        }

        public async Task<ModificationResultDTO> InsertObject(CityInsertDTO ObjectDTO)
        {
            City City = new City() {
                id = 0,
                name = ObjectDTO.name,
                status = ObjectDTO.status,
                normalShippingCost = ObjectDTO.normalShippingCost,
                pickupShippingCost = ObjectDTO.pickupShippingCost,
                governorateId = ObjectDTO.governorateId
            };

            var result = CityRepo.Add(City);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the city"
                };
            }

            result = await Unit.SaveChanges();

            if (result == false)
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

        public async Task<bool> SaveChangesForObject()
        {
            var result = await Unit.SaveChanges();

            return result;
        }

        public async Task<ModificationResultDTO> UpdateObject(CityUpdateDTO ObjectDTO)
        {
            City? City =await CityRepo.GetElement(b => b.id == ObjectDTO.id);

            if (City == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "City doesn't exist in the db"
                };
            }
            
            City.name = ObjectDTO.name;
            City.normalShippingCost = ObjectDTO.normalShippingCost;
            City.pickupShippingCost = ObjectDTO.pickupShippingCost;
            City.status = ObjectDTO.status;
            City.governorateId = ObjectDTO.governorateId;

            var result = CityRepo.Edit(City);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the city"
                };
            }

            result = await Unit.SaveChanges();

            if (result == false)
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

        public async Task<ModificationResultDTO> DeleteObject(int ObjectId)
        {
            City? City = await CityRepo.GetElement(b => b.id == ObjectId);

            if (City == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "City doesn't exist in the db"
                };
            }

            var result = CityRepo.Delete(City);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the city"
                };
            }

            result = await Unit.SaveChanges();

            if (result == false)
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

        public async Task<PaginationDTO<CityDisplayDTO>> GetPaginatedOrders(int pageNumber, int pageSize, Expression<Func<City, bool>> filter)
        {
            var totalCount = await CityRepo.Count(filter);
            var totalPages = await CityRepo.Pages(pageSize);
            var objectList = await CityRepo.GetPaginatedElements(pageNumber, pageSize, filter);
            List<CityDisplayDTO> CityDisplayDTO = new List<CityDisplayDTO>();

            foreach (var item in objectList)
            {
                CityDisplayDTO.Add(
                    new CityDisplayDTO
                    {
                        id = item.id,
                        name = item.name,
                        status = item.status,
                        normalShippingCost = item.normalShippingCost,
                        pickupShippingCost = item.pickupShippingCost,
                        governorateId = item.governorateId
                    }
                );
            }
            return new PaginationDTO<CityDisplayDTO>()
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = CityDisplayDTO
            };
        }
    }
}
