using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;
using Application.Interfaces.ApplicationServices;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IGenericService<Product, DisplayProductDTO, InsertProductDTO, UpdateProductDTO, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product> _repository;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = _unitOfWork.GetGenericRepository<Product>();
        }
        public async Task<List<DisplayProductDTO>> GetAllObjects()
        {
            var products = await _repository.GetAllElements();
            return _mapper.Map<List<DisplayProductDTO>>(products);
        }

        public async Task<List<DisplayProductDTO>> GetAllObjects(params System.Linq.Expressions.Expression<Func<Product, object>>[] includes)
        {
            var products = await _repository.GetAllElements(includes);
            return _mapper.Map<List<DisplayProductDTO>>(products);
        }

        public async Task<DisplayProductDTO?> GetObject(System.Linq.Expressions.Expression<Func<Product, bool>> filter)
        {
            var product = await _repository.GetElement(filter);
            return _mapper.Map<DisplayProductDTO>(product);
        }

        public async Task<DisplayProductDTO?> GetObject(System.Linq.Expressions.Expression<Func<Product, bool>> filter, params System.Linq.Expressions.Expression<Func<Product, object>>[] includes)
        {
            var product = await _repository.GetElement(filter, includes);
            return _mapper.Map<DisplayProductDTO>(product);
        }

        public async Task<DisplayProductDTO?> GetObjectWithoutTracking(System.Linq.Expressions.Expression<Func<Product, bool>> filter)
        {
            var product = await _repository.GetElementWithoutTracking(filter);
            return _mapper.Map<DisplayProductDTO>(product);
        }

        public async Task<DisplayProductDTO?> GetObjectWithoutTracking(System.Linq.Expressions.Expression<Func<Product, bool>> filter, params System.Linq.Expressions.Expression<Func<Product, object>>[] includes)
        {
            var product = await _repository.GetElementWithoutTracking(filter, includes);
            return _mapper.Map<DisplayProductDTO>(product);
        }

        public  Task<ModificationResultDTO> InsertObject(InsertProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            var result = _repository.Add(product);

            if (result == false)
            {
                return Task.FromResult(new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the product"
                });
            }

            return Task.FromResult(new ModificationResultDTO()
            {
                Succeeded = true
            });
        }

        public Task<ModificationResultDTO> UpdateObject(UpdateProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            var result = _repository.Edit(product);

            if (result == false)
            {
                return Task.FromResult(new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the product"
                });
            }

            return Task.FromResult(new ModificationResultDTO()
            {
                Succeeded = true
            });
        }

        public async Task<ModificationResultDTO> DeleteObject(int productId)
        {
            var product = await _repository.GetElement(x => x.Id == productId);

            if (product == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Product doesn't exist in the db"
                };
            }

            var result = _repository.Delete(product);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the product"
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
    }
}
