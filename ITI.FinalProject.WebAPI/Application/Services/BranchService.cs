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
using Azure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Domain.Services
{
    public class BranchService : IPaginationService<Branch,BranchDisplayDTO,BranchInsertDTO,BranchUpdateDTO,int>, IDropDownOptionsService<Branch, int>
    {
        public IPaginationRepository<Branch> branchRepo;
        public IUnitOfWork unit;
        public BranchService( IUnitOfWork _unit)
        {
            branchRepo= _unit.GetPaginationRepository<Branch>(); 
            unit = _unit;
        }

        public async Task<List<OptionDTO<int>>> GetOptions(params Expression<Func<Branch, object>>[] includes)
        {
            var options = await branchRepo.GetAllElements(includes);
            return options.Select(o => new OptionDTO<int>() { Id = o.id, Name = o.name, DependentId = o.cityId }).ToList();
        }

        public async Task<List<OptionDTO<int>>> GetOptions(Expression<Func<Branch, bool>> filter, params Expression<Func<Branch, object>>[] includes)
        {
            var options = await branchRepo.GetAllElements(filter, includes);
            return options.Select(o => new OptionDTO<int>() { Id = o.id, Name = o.name, DependentId = o.cityId }).ToList();
        }
       
        public async Task<List<BranchDisplayDTO>> GetAllObjects()
        {
            List<Branch>? branches= await branchRepo.GetAllElements();
            if(branches == null)
            {
                return null;
            }
            List<BranchDisplayDTO> branchsDTO = new List<BranchDisplayDTO>();
            foreach (var item in branches)
            {
                branchsDTO.Add(
                    new BranchDisplayDTO
                    {
                        id= item.id,
                        name = item.name,
                        addingDate = item.addingDate,
                        cityId = item.cityId,
                        status = item.status
                    }
                );    
            }
            return branchsDTO;
        }

        public  async Task<List<BranchDisplayDTO>> GetAllObjects(params Expression<Func<Branch, object>>[] includes)
        {
            List<Branch>? branches = await branchRepo.GetAllElements(includes);
            if (branches == null)
            {
                return null;
            }
            List<BranchDisplayDTO> branchsDTO = new List<BranchDisplayDTO>();
            foreach (var item in branches)
            {
                branchsDTO.Add(
                    new BranchDisplayDTO
                    {
                        name = item.name,
                        addingDate = item.addingDate,
                        cityId = item.cityId,
                        status = item.status
                    }
                );
            }
            return branchsDTO;
        }

        public async Task<BranchDisplayDTO?> GetObject(Expression<Func<Branch, bool>> filter)
        {
            Branch?  branch = await branchRepo.GetElement(filter);
            if (branch == null)
            {
                return null;
            }
            BranchDisplayDTO branchDTO = new BranchDisplayDTO(){
                name = branch.name,
                addingDate = branch.addingDate,
                cityId = branch.cityId,
                status = branch.status
            };
            
            return branchDTO;
        }

        public async Task<BranchDisplayDTO?> GetObject(Expression<Func<Branch, bool>> filter, params Expression<Func<Branch, object>>[] includes)
        {
            Branch? branch = await branchRepo.GetElement(filter, includes);
            if (branch == null)
            {
                return null;
            }
            BranchDisplayDTO branchDTO = new BranchDisplayDTO()
            {
                name = branch.name,
                addingDate = branch.addingDate,
                cityId = branch.cityId,
                status = branch.status
            };

            return branchDTO;
        }

        public async Task<BranchDisplayDTO?> GetObjectWithoutTracking(Expression<Func<Branch, bool>> filter)
        {
            Branch? branch = await branchRepo.GetElementWithoutTracking(filter);
            if (branch == null)
            {
                return null;
            }
            BranchDisplayDTO branchDTO = new BranchDisplayDTO()
            {
                name = branch.name,
                addingDate = branch.addingDate,
                cityId = branch.cityId,
                status = branch.status
            };

            return branchDTO;
        }


        public async Task<BranchDisplayDTO?> GetObjectWithoutTracking(Expression<Func<Branch, bool>> filter, params Expression<Func<Branch, object>>[] includes)
        {
            Branch? branch = await branchRepo.GetElementWithoutTracking(filter,includes);
            if (branch == null)
            {
                return null;
            }
            BranchDisplayDTO branchDTO = new BranchDisplayDTO()
            {
                name = branch.name,
                addingDate = branch.addingDate,
                cityId = branch.cityId,
                status = branch.status
            };

            return branchDTO;
        }

        public async Task<ModificationResultDTO> InsertObject(BranchInsertDTO ObjectDTO)
        {
            Branch branch = new Branch() {
                name = ObjectDTO.name,
                addingDate = DateTime.Now,
                cityId = ObjectDTO.cityId,
                status = ObjectDTO.status
            };

            bool result = branchRepo.Add(branch);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the branch"
                };
            }

            result = await unit.SaveChanges();

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
            var result = await unit.SaveChanges();

            return result;
        }

        public async Task<ModificationResultDTO> UpdateObject(BranchUpdateDTO ObjectDTO)
        {
            Branch? branch = await branchRepo.GetElement(b => b.id == ObjectDTO.id);

            if (branch == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Branch doesn't exist in the db"
                };
            }

            branch.id = ObjectDTO.id;
            branch.name = ObjectDTO.name;
            branch.cityId = ObjectDTO.cityId;
            branch.status = ObjectDTO.status;

            var result = branchRepo.Edit(branch);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the branch"
                };
            }

            result = await unit.SaveChanges();

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
            Branch? branch = await branchRepo.GetElement(b => b.id == ObjectId);

            if (branch == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Branch doesn't exist in the db"
                };
            }

            var result = branchRepo.Delete(branch);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the branch"
                };
            }

            result = await unit.SaveChanges();

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

        public async Task<PaginationDTO<BranchDisplayDTO>> GetPaginatedOrders(int pageNumber, int pageSize, Expression<Func<Branch, bool>> filter)
        {
            var totalCount = await branchRepo.Count(filter);
            var totalPages = await branchRepo.Pages(pageSize);
            var objectList = await branchRepo.GetPaginatedElements(pageNumber, pageSize, filter);
            List<BranchDisplayDTO> branchsDTO = new List<BranchDisplayDTO>();

            foreach (var item in objectList)
            {
                branchsDTO.Add(
                    new BranchDisplayDTO
                    {
                        id = item.id,
                        name = item.name,
                        addingDate = item.addingDate,
                        cityId = item.cityId,
                        status = item.status
                    }
                );
            }
            return new PaginationDTO<BranchDisplayDTO>()
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = branchsDTO
            };
        }
    }
}
