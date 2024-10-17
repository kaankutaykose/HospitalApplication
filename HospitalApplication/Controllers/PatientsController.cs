using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using HospitalApplication.Data;
using HospitalApplication.Models;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace HospitalApplication.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Email,Password,Name,Surname,BirthDate,Gender,PhoneNumber")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                // Örneğin, şifreyi hashleyerek güvenli bir şekilde saklamak için uygun bir yöntem kullanabilirsiniz.
                // Ancak burada basitleştirmek için doğrudan şifreyi kullanıyoruz.
                patient.Password = BCrypt.Net.BCrypt.HashPassword(patient.Password);

                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Patients"); // Yönlendirilecek sayfa
            }
            return View(patient);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Patient model)
        {
            if (ModelState.IsValid)
            {
                var patient = await _context.Patient.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (patient != null && BCrypt.Net.BCrypt.Verify(model.Password, patient.Password))
                {
                    // Kullanıcı giriş başarılı
                    // Örneğin, authentication işlemlerini gerçekleştirebilirsiniz.
                    // Bu örnekte giriş işlemleri basitleştirilmiştir.

                    return RedirectToAction("PatientPanel", "Patients", new { id = patient.Id }); // Yönlendirilecek sayfa
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong E-mail or password.");
                    return View(model);
                }
            }

            return View(model);
        }
        
        // GET Users/UserPanel
        public async Task<IActionResult> PatientPanel(int? id)
        {
            if (id == null || _context.Patient == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        //Get Appointments
        public async Task<IActionResult> Appointments(int? id)
        {
            
            var patient = await _context.Patient
        .FirstOrDefaultAsync(m => m.Id == id);
            var appointments = await _context.Appointment
        .Where(a => a.PatientId == id)
        .Include(a => a.Doctor) // İsteğe bağlı: Doktor bilgilerini de dahil edebilirsiniz
        .ToListAsync();

            // Hasta bilgileri ve randevularını bir ViewModel kullanarak View'a gönderebilirsiniz.
            var model = new Patient
            {
                
                Appointments = appointments
            };

            return View(model);
        }

        public async Task<IActionResult> BookAppointment(int? id)
        {
            var professions = await _context.Doctor
                .Select(d => d.Profession)
                .Distinct()
                .ToListAsync();
            var patient = await _context.Patient
        .FindAsync(id);
            var model = new BookAppointmentViewModel
            {
                PatientId = patient.Id,
                Professions = professions.Select(p => new SelectListItem { Value = p, Text = p }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate
                };

                _context.Appointment.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("PatientPanel", new { id = model.PatientId });
            }

            return View(model);
        }

        public async Task<JsonResult> GetDoctorsByProfession(string profession)
        {
            var doctors = await _context.Doctor
                .Where(d => d.Profession == profession)
                .Select(d => new { d.Id, d.Name, d.Surname })
                .ToListAsync();

            return Json(doctors);
        }
        

        // Get Users/UpdateProfile/1
        public async Task<IActionResult> UpdateProfile(int? id)
        {
            if (id == null)
            {
                // Hata ayıklama için günlükleme ekle
                Console.WriteLine("id null");
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                // Hata ayıklama için günlükleme ekle
                Console.WriteLine("Kullanıcı bulunamadı");
                return NotFound();
            }
            return View(patient);
        }



        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int id, [Bind("Id,Email,Name,Surname,BirthDate,Gender,PhoneNumber,Password")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("PatientPanel", "Patients");
            }
            return View(patient);
        }


        private bool PatientExists(int id)
        {
            return (_context.Patient?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}