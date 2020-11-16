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
    public class CursoesController : Controller
    {
        private readonly SchoolDBContext _context;

        public CursoesController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: Cursoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Curso.ToListAsync());
        }
        

        // GET: Cursoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Curso
                .FirstOrDefaultAsync(m => m.Id == id);
           
            if (curso == null)
            {
                return NotFound();
            }

            //Busco los alumnos asignados al curso correspondiente y los mando a la vista por el ViewBag
            var alumnos = (from a in _context.Alumno
                           join ca in _context.CursoAlumno on a.Id equals ca.AlumnoId
                           where ca.CursoId == id
                           orderby a.Apellido ascending
                           select new { a.Dni, a.Nombre, a.Apellido }).ToList();
            ViewBag.Alumnos = alumnos;

            //Busco las materias asignadas al curso correspondiente y las mando a la vista por el ViewBag
            var materias = (from m in _context.Materia
                           join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                           where cm.CursoId == id
                           orderby m.Nombre ascending
                            select new { m.Id, m.Nombre, m.Anio }).ToList();
            ViewBag.Materias = materias;
            return View(curso);
        }

        // GET: Cursoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cursoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sigla")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // GET: Cursoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Curso.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            return View(curso);
        }

        // POST: Cursoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sigla")] Curso curso)
        {
            if (id != curso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id))
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
            return View(curso);
        }

        // GET: Cursoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Curso
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Cursoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Curso.FindAsync(id);
            _context.Curso.Remove(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
            return _context.Curso.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> RemoverAlumno([Bind("Id,Sigla")] Curso curso)
        {
            int alumId = Convert.ToInt32(curso.Sigla);
            return RedirectToAction("Remover", "CursoAlumnoes", new{ alumnoId = alumId, cursoId = curso.Id });            
        }

        [HttpPost]
        public async Task<IActionResult> RemoverMateria([Bind("Id,Sigla")] Curso curso)
        {
            int matId = Convert.ToInt32(curso.Sigla);
            return RedirectToAction("Remover", "CursoMaterias", new { materiaId = matId, cursoId = curso.Id });
        }
    }
}
//
//