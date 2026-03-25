using AutoMapper;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.Units.Dtos;
using JudgeAPI.Application.Features.Units.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Models.Unit;

namespace JudgeAPI.Application.Features.Units.Services
{
    public class UnitService(
            IMapper mapper,
            IUnitRepository unitRepository,
            IUnitOfWork unitOfWork
            ) : IUnitService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUnitRepository _unitRepository = unitRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        // GET
        public async Task<List<UnitResponseDTO>> GetAllAsync()
        {
            List<Unit> units = await _unitRepository.GetAll();
            List<UnitResponseDTO> responseDTOs = _mapper.Map<List<UnitResponseDTO>>(units);

            return responseDTOs;
        }

        // GET BY ID
        public async Task<UnitResponseDTO> GetByIdAsync(int id)
        {
            Unit unit = await _unitRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"No se encontró la unidad con ID {id}");
            return _mapper.Map<UnitResponseDTO>(unit);
        }

        // GET BY ID WITH PROBLEMS
        public async Task<UnitWithProblemsDTO> GetUnitWithProblemsAsync(int unitId)
        {
            Unit unit = await _unitRepository.GetUnitWithProblemByIdAsync(unitId) ?? throw new KeyNotFoundException($"No se encontró la unidad con ID {unitId}");
            return _mapper.Map<UnitWithProblemsDTO>(unit);
        }

        // CREATE
        public async Task<UnitResponseDTO> CreateAsync(UnitCreateDTO dto)
        {
            Unit unit = _mapper.Map<Unit>(dto);
            _unitRepository.Add(unit);
            _ = await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UnitResponseDTO>(unit);
        }

        // UPDATE
        public async Task<UnitResponseDTO> UpdateAsync(UnitUpdateDTO dto)
        {
            Unit unit = await _unitRepository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"No se encontró la unidad con ID {dto.Id}");
            _ = _mapper.Map(dto, unit);
            _ = await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UnitResponseDTO>(unit);
        }
    }
}
