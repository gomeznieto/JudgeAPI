using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Models.Unit;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Services.Unit
{
    public class UnitService : IUnitService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public UnitService(
            AppDbContext appDbContext,
            IMapper mapper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        // GET
        public async Task<List<UnitResponseDTO>> GetAllAsync()
        {
            var units = await _appDbContext.Units.ToListAsync();
            var responseDTOs = _mapper.Map<List<UnitResponseDTO>>(units);

            return responseDTOs;
        }

        // GET BY ID
        public async Task<UnitResponseDTO> GetByIdAsync(int id)
        {
            var unit = await _appDbContext.Units.FindAsync(id);

            if (unit is null)
                throw new KeyNotFoundException($"No se encontró la unidad con ID {id}");

            return _mapper.Map<UnitResponseDTO>(unit);
        }

        // CREATE
        public async Task<UnitResponseDTO> CreateAsync(UnitCreateDTO dto)
        {
            var unit = _mapper.Map<Entities.Unit>(dto);
            _appDbContext.Add(unit);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<UnitResponseDTO>(unit);
        }

        // UPDATE
        public async Task<UnitResponseDTO> UpdateAsync(UnitUpdateDTO dto)
        {
            var unit = await _appDbContext.Units.FindAsync(dto.Id);

            if (unit is null)
                throw new KeyNotFoundException($"No se encontró la unidad con ID {dto.Id}");

            _mapper.Map(dto, unit);

            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<UnitResponseDTO>(unit);
        }
    }
}
