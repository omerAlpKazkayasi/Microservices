using CourseServicesCatalog.Dtos;
using CourseServicesCatalog.Model;
using CourseShared.Dto;

namespace CourseServicesCatalog.Services
{
    public interface ICategoryService
    {
      Task<ResponseDto<List<CategoryDto>>> GetAllAsync();
        Task<ResponseDto<CategoryDto>> CreateAsync(CategoryDto category);
        Task<ResponseDto<CategoryDto>> GetByIdAsync(string id);
    }
}
