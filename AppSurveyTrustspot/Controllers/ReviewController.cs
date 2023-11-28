using AppSurveyTrustspot.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AppSurvey.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly TrustspotContext repo;
		//logging
		private readonly ILogger<ReviewController> logger;

		public ReviewController(TrustspotContext repo, ILogger<ReviewController> logger)//dependency injection
		{
			this.repo = repo;
			this.logger = logger;
		}


		// get all the reviews
		[HttpGet("FetchAllReviews")]
		public IActionResult FetchAllReviews()
		{
			List<Review> reviews = repo.Reviews.Include(r => r.App).ToList();
			if (reviews.Count == 0)
			{
				return StatusCode(404, reviews);
			}
			else
			{
				var reviewDetails = reviews.Select(r => new
				{
					ReviewId = r.ReviewId,
					AppName = r.App.AppName,
					AppId = r.AppId,
					Email = r.Email,
					ReviewText = r.ReviewText
				});
				logger.LogInformation("This is from Review Controller");
				return StatusCode(200, reviewDetails);
			}
		}

        [Authorize]
        // get all available unique emails from Users and Admin tables
        [HttpGet("FetchAllEmails")]
        public IActionResult FetchAllEmails()
        {
            try
            {
                List<string> userEmails = repo.Users.Select(u => u.EmailAddress).ToList();
                List<string> adminEmails = repo.Admins.Select(a => a.EmailAddress).ToList();

                List<string> allEmails = userEmails.Concat(adminEmails).ToList();

                if (allEmails.Count == 0)
                {
                    return StatusCode(404, "No emails found.");
                }
                else
                {
                    return StatusCode(200, allEmails);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }






        //To post a new review
        [HttpPost("AddReview")]
		public IActionResult AddReview(Review rev)
		{
			if (rev == null)
			{
				return BadRequest("Invalid data: Review object is null");
			}
			try
			{
				repo.Reviews.Add(rev);
				repo.SaveChanges();
				return Created("New review is added", rev);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while adding the review.");
			}
		}

		////To post a new review
		//[HttpPost("AddReview")]
		//public async Task<IActionResult> AddReview([FromBody] Review rev)
		//{
		//	if(rev==null)
		//	{
		//		return BadRequest("Invalid data: Review object is null.");
		//	}
		//	try
		//	{
		//		repo.Reviews.Add(rev);
		//		await repo.SaveChangesAsync();

		//		return Created("New review is added", rev);
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, "An error occurred while adding the review.");
		//	}
		//	//rev.App = repo.Apps.Find(rev.AppId);
		//repo.Reviews.Add(rev);
			//repo.SaveChanges();
			//logger.LogInformation("This is from Review Controller - AddReview");
			//return Created("New review is Addeded", rev);

		//}


		/* [HttpPost("AddReviewName")]
		 public IActionResult AddReviewName(Review rev)
		 {
			 var app = repo.Apps.FirstOrDefault(a => a.AppName == rev.AppName);

			 if (app == null)
			 {
				 return NotFound("App not found with the specified name.");
			 }

			 rev.App = app;
			 repo.Reviews.Add(rev);
			 repo.SaveChanges();

			 return Created("New review is added", rev);
		 }*/

		[HttpPost("AddReviewAppId")]
		public IActionResult AddReviewAppId(Review rev)
		{
			var app = repo.Apps.FirstOrDefault(a => a.AppId == rev.AppId);

			if (app == null)
			{
				return NotFound("App not found with the specified AppId.");
			}

			rev.App = app;
			repo.Reviews.Add(rev);
			repo.SaveChanges();

			return Created("New review is added", rev);
		}


        // delete review based on review id
        /* [HttpDelete("{ReviewId}")]
         public IActionResult DeleteReview(int ReviewId)
         {
             try
             {
                 Review review = repo.Reviews.Find(ReviewId);

                 if (review == null)
                 {
                     return StatusCode(404, "Review Not Found");
                 }

                 repo.Reviews.Remove(review);
                 repo.SaveChangesAsync();

                 return Ok("Review Deleted Successfully");
             }
             catch (Exception ex)
             {
                 // Log the exception or handle it appropriately
                 return StatusCode(500, "Internal Server Error");
             }
         }*/

        [HttpDelete("{ReviewId}")]
        public async Task<IActionResult> DeleteReview(int ReviewId)
        {
            try
            {
                Review review = await repo.Reviews.FindAsync(ReviewId);

                if (review == null)
                {
                    return StatusCode(404, "Review Not Found");
                }

                repo.Reviews.Remove(review);
                await repo.SaveChangesAsync();

                return Ok("Review Deleted Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal Server Error");
            }
        }


        //update review based on review id.
        [HttpPut("{ReviewId}")]
        public IActionResult UpdateReviewText(int ReviewId, [FromBody] Review updatedReview)
        {
            try
            {
                Review existingReview = repo.Reviews.Find(ReviewId);

                if (existingReview == null)
                {
                    return StatusCode(404, "Review Not Found");
                }

                // Only update the ReviewText property
                existingReview.ReviewText = updatedReview.ReviewText;

                repo.SaveChanges();

                return Ok("Review Text Updated Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal Server Error");
            }
        }

/*        //Review by email
        [HttpGet("AppNameAndReviewByEmail/{email}")]
        public IActionResult AppNameAndReviewByEmail(string email)
        {
            var appReviews = (from r in repo.Reviews
                              join a in repo.Apps on r.AppId equals a.AppId
                              where r.Email == email
                              select new
                              {
                                  AppName = a.AppName,
                                  ReviewContent = r.ReviewText
                              }).ToList();

            return Ok(appReviews);
        }
*/


    }
}