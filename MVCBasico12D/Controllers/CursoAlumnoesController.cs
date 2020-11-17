using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBasico12D.Context;
using MVCBasico12D.Models;

namespace MVCBasico12D.Controllers
{
    public class CursoAlumnoesController : Controller
    {
        private readonly SchoolDBContext _context;

        public CursoAlumnoesController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: CursoAlumnoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CursoAlumno.ToListAsync());
        }

        // GET: CursoAlumnoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursoAlumno = await _context.CursoAlumno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cursoAlumno == null)
            {
                return NotFound();
            }

            return View(cursoAlumno);
        }

        // GET: CursoAlumnoes/Create
        public IActionResult Create()
        {
            var alumnos = (from a in _context.Alumno
                              select a).ToList();
            List<Alumno> alumnosSinCurso = new List<Alumno>();
            var cursos = (from c in _context.Curso
                             select c).ToList();

            foreach (Alumno a in alumnos) {
                var cursoAlumno = _context.CursoAlumno.Where(m => m.AlumnoId == a.Id)
                 .FirstOrDefault();
                if(cursoAlumno == null)
                {
                    alumnosSinCurso.Add(a);
                }
            }

            ViewBag.Alumnos = alumnosSinCurso;
            ViewBag.Cursos = cursos;
            return View();
        }

        // POST: CursoAlumnoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AlumnoId,CursoId")] CursoAlumno cursoAlumno)
        {
            if (ModelState.IsValid)
            {
                //Inserto el nuevo cursoAlumno en la BD
                _context.Add(cursoAlumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cursoAlumno);
        }

        // GET: CursoAlumnoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursoAlumno = await _context.CursoAlumno.FindAsync(id);
            if (cursoAlumno == null)
            {
                return NotFound();
            }
            return View(cursoAlumno);
        }

        // POST: CursoAlumnoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlumnoId,CursoId")] CursoAlumno cursoAlumno)
        {
            if (id != cursoAlumno.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cursoAlumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoAlumnoExists(cursoAlumno.Id))
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
            return View(cursoAlumno);
        }
        public async Task<IActionResult> Remover(String alumnoId, int cursoId)
        {
            //Con el DNI del alumno, busco su ID
            var idAlumno = (from a in _context.Alumno
                            where a.Dni == alumnoId
                            select a.Id).FirstOrDefault();
            //Busco el ID del cursoAlumno que deseo eliminar, usando el ID de alumno y el ID de curso
            var idEliminar = (from ca in _context.CursoAlumno
                              where ca.AlumnoId == idAlumno && ca.CursoId == cursoId
                              select ca.Id).FirstOrDefault();
            //Agarro el cursoAlumno que deseo eliminar y lo elimino
            var cursoAlumno = await _context.CursoAlumno.FindAsync(idEliminar);
            _context.CursoAlumno.Remove(cursoAlumno);
            await _context.SaveChangesAsync();
            //Vuelvo a la vista de Details de Cursos
            return RedirectToAction("Details", "Cursoes", new { id = cursoId});
        }
        // GET: CursoAlumnoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursoAlumno = await _context.CursoAlumno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cursoAlumno == null)
            {
                return NotFound();
            }

            return View(cursoAlumno);
        }

        // POST: CursoAlumnoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cursoAlumno = await _context.CursoAlumno.FindAsync(id);
            _context.CursoAlumno.Remove(cursoAlumno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoAlumnoExists(int id)
        {
            return _context.CursoAlumno.Any(e => e.Id == id);
        }
    }
}
