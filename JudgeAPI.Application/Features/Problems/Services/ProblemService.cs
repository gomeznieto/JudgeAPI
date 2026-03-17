using AutoMapper;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain;

namespace JudgeAPI.Application.Features
{
    public class ProblemService(IMapper mapper,
                                IProblemRepository problemRepository,
                                IUnitOfWork unitOfWork) : IProblemService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProblemRepository _problemRepository = problemRepository;

        // CREATE
        public async Task<ProblemResponseDTO> CreateAsync(ProblemCreateDTO dto)
        {
            Problem problem = _mapper.Map<Problem>(dto);

            _problemRepository.Add(problem); 
            _ = await unitOfWork.SaveChangesAsync();
            ProblemResponseDTO mapping = _mapper.Map<ProblemResponseDTO>(problem);

            return mapping;
        }

        public async Task<ProblemResponseDTO> GetById(int id)
        {
            Problem? problem = await _problemRepository.GetByIdAsync(id);

            return problem is null
                ? throw new KeyNotFoundException($"No se encontró el problema con ID {id}")
                : _mapper.Map<ProblemResponseDTO>(problem);
        }

        public async Task<ProblemResponseDTO> UpdateAsync(ProblemUpdateDTO dto)
        {
            Problem? problem = await _problemRepository.GetByIdAsync(dto.Id);

            if (problem is not null)
            {
                _ = _mapper.Map(dto, problem);

               _ = await unitOfWork.SaveChangesAsync();

                return _mapper.Map<ProblemResponseDTO>(problem);
            }

            throw new KeyNotFoundException($"No se encontró el problema con ID {dto.Id}");
        }

        public async Task DeleteProblemAsync(int id)
        {
            Problem? problem = await _problemRepository.GetByIdAsync(id);

            if (problem is not null)
            {
                _problemRepository.Delete(problem);
                _ = await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"No se encontró el problema con ID {id}");
            }
        }
    }
}
