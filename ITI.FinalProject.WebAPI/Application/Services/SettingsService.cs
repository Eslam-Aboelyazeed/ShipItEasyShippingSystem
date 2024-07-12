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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SettingsService : IGenericService<Settings, SettingsDTO, SettingsInsertDTO, SettingsUpdateDTO, int>
    {
        private readonly IGenericRepository<Settings> repository;
        private readonly IUnitOfWork unit;
        private readonly IMapper mapper;

        public SettingsService(IUnitOfWork unit, IMapper mapper)
        {
            this.repository = unit.GetGenericRepository<Settings>();
            this.unit = unit;
            this.mapper = mapper;
        }
        public async Task<List<SettingsDTO>> GetAllObjects()
        {
            var settingsList = await repository.GetAllElements();

            return mapper.Map<List<SettingsDTO>>(settingsList);
        }

        public async Task<List<SettingsDTO>> GetAllObjects(params Expression<Func<Settings, object>>[] includes)
        {
            var settingsList = await repository.GetAllElements(includes);

            return mapper.Map<List<SettingsDTO>>(settingsList);
        }

        public async Task<SettingsDTO?> GetObject(Expression<Func<Settings, bool>> filter)
        {
            var settings = await repository.GetElement(filter);

            return mapper.Map<SettingsDTO>(settings);
        }

        public async Task<SettingsDTO?> GetObject(Expression<Func<Settings, bool>> filter, params Expression<Func<Settings, object>>[] includes)
        {
            var settings = await repository.GetElement(filter, includes);

            return mapper.Map<SettingsDTO>(settings);
        }

        public async Task<SettingsDTO?> GetObjectWithoutTracking(Expression<Func<Settings, bool>> filter)
        {
            var settings = await repository.GetElementWithoutTracking(filter);

            return mapper.Map<SettingsDTO>(settings);
        }

        public async Task<SettingsDTO?> GetObjectWithoutTracking(Expression<Func<Settings, bool>> filter, params Expression<Func<Settings, object>>[] includes)
        {
            var settings = await repository.GetElementWithoutTracking(filter, includes);

            return mapper.Map<SettingsDTO>(settings);
        }

        public Task<ModificationResultDTO> InsertObject(SettingsInsertDTO ObjectDTO)
        {
            var settings = mapper.Map<Settings>(ObjectDTO);

            var result = repository.Add(settings);

            if (result == false)
            {
                return Task.FromResult(new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error inserting the settings"
                });
            }

            return Task.FromResult(new ModificationResultDTO()
            {
                Succeeded = true
            });
        }


        public Task<ModificationResultDTO> UpdateObject(SettingsUpdateDTO ObjectDTO)
        {
            var settings = mapper.Map<Settings>(ObjectDTO);

            var result = repository.Edit(settings);

            if (result == false)
            {
                return Task.FromResult(new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error updating the settings"
                });
            }

            return Task.FromResult(new ModificationResultDTO()
            {
                Succeeded = true
            });
        }

        public async Task<ModificationResultDTO> DeleteObject(int ObjectId)
        {
            var settings = await repository.GetElement(s => s.Id == ObjectId);

            if (settings == null)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "settings doesn't exist in the db"
                };
            }

            var result = repository.Delete(settings);

            if (result == false)
            {
                return new ModificationResultDTO()
                {
                    Succeeded = false,
                    Message = "Error deleting the settings"
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
    }
}
