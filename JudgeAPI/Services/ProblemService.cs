using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace JudgeAPI.Services
{
    public class ProblemService : IProblemService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ProblemService(
            AppDbContext appDbContext,
            IMapper mapper
            )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<UnitWithProblemsDTO> GetUnitWithProblemsAsync(int unitId)
        {
            var unit = await _appDbContext.Units
                .Include(u => u.Problems)
                .SingleOrDefaultAsync(u => u.Id == unitId);

            if (unit == null)
                throw new KeyNotFoundException($"No se encontró la unidad con ID {unitId}");

            return _mapper.Map<UnitWithProblemsDTO>(unit);
        }

        public async Task<ProblemResponseDTO> CreateAsync(ProblemCreateDTO dto)
        {
            var problem = _mapper.Map<Problem>(dto);

            _appDbContext.Add(problem);
            await _appDbContext.SaveChangesAsync();
            var mapping = _mapper.Map<ProblemResponseDTO>(problem);
            return mapping;
        }

        public async Task<ProblemResponseDTO> GetById(int id)
        {
            var problem = await _appDbContext.Problems.FindAsync(id);

            if (problem is null)
                throw new KeyNotFoundException($"No se encontró el problema con ID {id}");

            return _mapper.Map<ProblemResponseDTO>(problem);
        }

        public async Task<ProblemResponseDTO> UpdateAsync(ProblemUpdateDTO dto)
        {
            var problem = await _appDbContext.Problems.FindAsync(dto.Id);

            if (problem is null)
                throw new KeyNotFoundException($"No se encontró el problema con ID {dto.Id}");

            _mapper.Map(dto, problem);

            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<ProblemResponseDTO>(problem);

        }

        public async Task DeleteProblemAsync(int id)
        {
            var exist = await _appDbContext.Problems.AnyAsync(x => x.Id == id);

            if(!exist)
                throw new KeyNotFoundException($"No se encontró el problema con ID {id}");

            var affectedRows = await _appDbContext.Problems.Where(x => x.Id == id).ExecuteDeleteAsync();

            if(affectedRows == 0)
                throw new InvalidOperationException($"No se pudo eliminar el problema con ID {id}.");
        }
    }
}
