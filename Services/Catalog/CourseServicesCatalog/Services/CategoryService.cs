using AutoMapper;
using CourseServicesCatalog.Dtos;
using CourseServicesCatalog.Model;
using CourseServicesCatalog.Settings;
using CourseShared.Dto;
using MongoDB.Driver;

namespace CourseServicesCatalog.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly IMongoCollection<Category> categoryCollection;
        private readonly IMapper mapper;

        public CategoryService(IMapper mapper,IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            this.mapper = mapper;
            this.categoryCollection = database.GetCollection<Category>(databaseSettings.CourseCollectionName);
        }
        public async Task<ResponseDto<List<CategoryDto>>> GetAllAsync()
        {
            var categories=await categoryCollection.Find(category=>true).ToListAsync();
            return ResponseDto<List<CategoryDto>>.Success(mapper.Map<List<CategoryDto>>(categories),200);
        }
        public async Task<ResponseDto<CategoryDto>> CreateAsync(CategoryDto category)
        {
            var categories = mapper.Map<Category>(category);
            await categoryCollection.InsertOneAsync(categories);
            return ResponseDto<CategoryDto>.Success(mapper.Map<CategoryDto>(category),200);
        }
        public async Task<ResponseDto<CategoryDto>> GetByIdAsync(string id)
        {
            var category =await categoryCollection.Find<Category>(x=>x.Id==id).FirstOrDefaultAsync();
            if (category == null)
                return ResponseDto<CategoryDto>.Fail("Category not found", 404);

            return ResponseDto<CategoryDto>.Success(mapper.Map<CategoryDto>(category), 200);
        }
    }
}
