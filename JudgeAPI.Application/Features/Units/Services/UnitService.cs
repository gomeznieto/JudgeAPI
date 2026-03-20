using AutoMapper;
using JudgeAPI.Application.Features.Units.Dtos;
using JudgeAPI.Application.Features.Units.Interfaces;
using JudgeAPI.Models.Unit;

namespace JudgeAPI.Application.Features.Units.Services
{
    public class UnitService(IMapper mapper) : IUnitService
    {
        private readonly IMapper _mapper = mapper;

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
        
        // GET BY ID WITH PROBLEMS
        public async Task<UnitWithProblemsDTO> GetUnitWithProblemsAsync(int unitId)
        {
            var unit = await _appDbContext.Units
                .Include(u => u.Problems)
                .SingleOrDefaultAsync(u => u.Id == unitId);

            if (unit == null)
                throw new KeyNotFoundException($"No se encontró la unidad con ID {unitId}");

            return _mapper.Map<UnitWithProblemsDTO>(unit);
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
