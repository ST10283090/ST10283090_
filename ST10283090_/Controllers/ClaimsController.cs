using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10283090_.Areas.Identity.Data;
using ST10283090_.Models;

namespace ST10283090_.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClaimsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: Claims
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Lecturer"))
            {
                var claims = await _context.Claims
                    .Where(c => c.UserID == user.Id)
                    .ToListAsync();
                return View(claims);
            }
            else
            {
                return View(await _context.Claims.ToListAsync());
            }
        }


        // GET: Claims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claims = await _context.Claims
                .FirstOrDefaultAsync(m => m.ClaimID == id);
            if (claims == null)
            {
                return NotFound();
            }

            return View(claims);
        }

        // GET: Claims/Create
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create
        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claims claim, IFormFile SingleFile)
        {
            const long maxFileSize = 15 * 1024 * 1024; 
                                                      
            var allowedFileTypes = new[] { "application/pdf", "image/png", "image/jpeg" };

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                claim.UserID = user.Id;

                claim.Status = "Pending";
                claim.TotalAmount = claim.HoursWorked * claim.RatePerHour;

                if (SingleFile != null && SingleFile.Length > 0)
                {
                    if (SingleFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("SingleFile", "File size must not exceed 15MB.");
                        return View(claim);
                    }

                    if (!allowedFileTypes.Contains(SingleFile.ContentType))
                    {
                        ModelState.AddModelError("SingleFile", "Invalid file type. Only PDF, PNG, DOCX, and JPEG files are allowed.");
                        return View(claim);
                    }

                    claim.FileName = SingleFile.FileName;
                    claim.ContentType = SingleFile.ContentType;
                    claim.Length = SingleFile.Length;
                    using (var memoryStream = new MemoryStream())
                    {
                        await SingleFile.CopyToAsync(memoryStream);
                        claim.Data = memoryStream.ToArray();
                    }
                }

                _context.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(claim);
        }




        // GET: Claims/Edit/5
        //[Authorize(Roles = "Lecturer")] 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claims = await _context.Claims.FindAsync(id);
            if (claims == null || claims.UserID != (await _userManager.GetUserAsync(User)).Id)
            {
                return NotFound();
            }
            return View(claims);
        }

        // POST: Claims/Edit/5
        [HttpPost]
        //[Authorize(Roles = "Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
        [Bind("ClaimID,UserID,FirstName,Email,LastName,ClaimsPeriodStart,ClaimsPeriodEnd,HoursWorked,RatePerHour,TotalAmount,DescriptionofWork,Status")]
        Claims claims)
        {
            if (id != claims.ClaimID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    claims.TotalAmount = claims.HoursWorked * claims.RatePerHour;

                    _context.Update(claims);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimsExists(claims.ClaimID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(claims);
        }

        // GET: Claims/Delete/5
        [Authorize(Roles = "Academic Manager, Programme Coordinator, Lecturer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claims = await _context.Claims
                .FirstOrDefaultAsync(m => m.ClaimID == id);
            if (claims == null)
            {
                return NotFound();
            }

            return View(claims);
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Academic Manager, Programme Coordinator, Lecturer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claims = await _context.Claims.FindAsync(id);
            if (claims != null)
            {
                _context.Claims.Remove(claims);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            claim.Status = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            claim.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ClaimsExists(int id)
        {
            return _context.Claims.Any(e => e.ClaimID == id);
        }
    }
}
