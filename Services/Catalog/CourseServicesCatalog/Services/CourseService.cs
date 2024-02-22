using AutoMapper;
using CourseServicesCatalog.Dtos;
using CourseServicesCatalog.Model;
using CourseServicesCatalog.Settings;
using CourseShared.Dto;
using MongoDB.Driver;

namespace CourseServicesCatalog.Services
{
    public class CourseService:ICourseService
    {
        private readonly IMongoCollection<Course> courseCollection;
        private readonly IMongoCollection<Category> categoryCollection;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            this.mapper = mapper;
            this.courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
        }

        private readonly IMapper mapper;

        public async Task<ResponseDto<List<CourseDto>>> GetAllAsync()
        {
            var courses =await courseCollection.Find(course=>true).ToListAsync();


            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category=await categoryCollection.Find<Category>(x=>x.Id==course.Id).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }
            return ResponseDto<List<CourseDto>>.Success(mapper.Map<List<CourseDto>>(courses), 200);
        }
        public async Task<ResponseDto<CourseDto>> GetByIdAsync(string id)
        {
            var course = await courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return ResponseDto<CourseDto>.Fail("Course not Found", 404);
            }
            course.Category = await categoryCollection.Find(x => x.Id == course.CategoryId).FirstAsync();


            return ResponseDto<CourseDto>.Success(mapper.Map<CourseDto>(course), 200);
        }
        public async Task<ResponseDto<List<CourseDto>>> GetAllByUserIdAsync(string id)
        {
            var courses=await courseCollection.Find<Course>(x=>x.UserId==id).ToListAsync();

           
            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await categoryCollection.Find<Category>(x => x.Id == course.Id).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }
            return ResponseDto<List<CourseDto>>.Success(mapper.Map<List<CourseDto>>(courses), 200);
        }


        public async Task<ResponseDto<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = mapper.Map<Course>(courseCreateDto);
            await courseCollection.InsertOneAsync(newCourse);

            return ResponseDto<CourseDto>.Success(mapper.Map<CourseDto>(newCourse), 200);
        }
        public async Task<ResponseDto<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = mapper.Map<Course>(courseUpdateDto);
            var result = await courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);
            if (result==null)
            {
                return ResponseDto<NoContent>.Fail("Course not Found", 404);
            }
            return ResponseDto<NoContent>.Success(204);

        }
        public async Task<ResponseDto<NoContent>> DeleteAsync(string id)
        {
            var result = await courseCollection.DeleteOneAsync(id);
            if (result.DeletedCount>0) 
            {
                return ResponseDto<NoContent>.Success(204);
            }
            else
            {
                return ResponseDto<NoContent>.Fail("Course not Found", 404);
            }
        }

    }
}
