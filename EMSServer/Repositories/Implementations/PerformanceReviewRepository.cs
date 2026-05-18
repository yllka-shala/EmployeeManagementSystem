using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class PerformanceReviewRepository(AppDbContext db) : IGenericRepositoryInterface<PerformanceReview>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var review = await db.PerformanceReviews.FindAsync(id);
            if (review == null) 
                return NotFound();

            db.PerformanceReviews.Remove(review);
            await Commit();
            return Success();
        }

        public async Task<List<PerformanceReview>> GetAll() 
        {
            var reviews = await db.PerformanceReviews
                                          .Include(e => e.Employee)
                                          .Include(r => r.Reviewer)
                                          .ToListAsync(); 
            return reviews;
        }

        public async Task<PerformanceReview> GetById(int id)
        {
            var review = await db.PerformanceReviews
                                         .Include(e => e.Employee)
                                         .Include(r => r.Reviewer)
                                         .FirstOrDefaultAsync(r => r.Id == id);
            return review;
        }

        public async Task<GeneralResponse> Insert(PerformanceReview item)
        {
           var checkIfExists = await CheckEmployee(item.EmployeeId);
           if(checkIfExists) 
                return new GeneralResponse(false, "Performance Review already exists!");

           if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            db.PerformanceReviews.Add(item);
           await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(PerformanceReview item)
        {
            var review = await db.PerformanceReviews.FindAsync(item.Id);
            if (review == null)
                return NotFound();

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            review.EmployeeId = item.EmployeeId;
            review.ReviewerId = item.ReviewerId;
            review.ReviewDate = item.ReviewDate;
            review.Rating = item.Rating;
            review.Comments = item.Comments;

            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry, Performance Review not Found!");
        private static GeneralResponse Success() => new(true, "Process Completed!");
        private async Task Commit()  => await db.SaveChangesAsync();

        private async Task<bool> CheckEmployee(int? Id)
        {
            var deduction = await db.PerformanceReviews.AnyAsync(x => x.EmployeeId == Id && x.ReviewDate == DateTime.Today);
            return deduction;
        }
    }
}
