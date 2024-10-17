using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using HospitalApplication.Data;
using HospitalApplication.Models;

namespace HospitalApplication.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers/TrainerLogin
        public IActionResult DoctorLogin()
        {
            return View();
        }

        // POST: Trainers/TrainerLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorLogin(Doctor model)
        {
            if (ModelState.IsValid)
            {
                var doctor = await _context.Doctor.FirstOrDefaultAsync(t => t.Email == model.Email);

                if (doctor != null && model.Password == doctor.Password)
                {
                    // Kullanıcı giriş başarılı
                    // Örneğin, authentication işlemlerini gerçekleştirebilirsiniz.
                    // Bu örnekte giriş işlemleri basitleştirilmiştir.

                    return RedirectToAction("DoctorPanel", "Doctors", new { id = doctor.Id }); // Yönlendirilecek sayfa
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong E-mail or password.");
                    return View(model);
                }
            }

            return View(model);
        }

        // Get Trainers/UpdateTrainerProfile/1
        public async Task<IActionResult> UpdateDoctorProfile(int? id)
        {
            if (id == null)
            {
                Console.WriteLine("id null");
                return NotFound();
            }

            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı");
                return NotFound();
            }
            return View(doctor);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorProfile(int id, [Bind("Id,Email,Password,Name,Surname,Hospital,Profession")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DoctorPanel", "Doctors");
            }
            return View(doctor);
        }

        // GET Trainers/TrainerPanel
        public async Task<IActionResult> DoctorPanel(int? id)
        {
            if (id == null || _context.Doctor == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        //Get Appointments
        public async Task<IActionResult> DoctorAppointments(int? id)
        {

            var doctor = await _context.Doctor
        .FirstOrDefaultAsync(m => m.Id == id);
            var appointments = await _context.Appointment
        .Where(a => a.DoctorId == id)
        .Include(a => a.Patient) // İsteğe bağlı: Doktor bilgilerini de dahil edebilirsiniz
        .ToListAsync();

            // Hasta bilgileri ve randevularını bir ViewModel kullanarak View'a gönderebilirsiniz.
            var model = new Doctor
            {

                Appointments = appointments
            };

            return View(model);
        }

        private bool DoctorExists(int id)
        {
            return (_context.Doctor?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
