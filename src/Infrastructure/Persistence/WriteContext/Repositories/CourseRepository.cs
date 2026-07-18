using Domain.Entities.CourseAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.WriteContext.Context;

namespace Infrastructure.Persistence.WriteContext.Repositories;

internal sealed class CourseRepository(WriteDbContext context)
    : BaseRepository<Course>(context), ICourseRepository
{
    
}
