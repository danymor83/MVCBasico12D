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
    public class MateriaProfesorsController : Controller
    {
        private readonly SchoolDBContext _context;

        public MateriaProfesorsController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: MateriaProfesors
        public async Task<IActionResult> Index()
        {
            return View(await _context.MateriaProfesor.ToListAsync());
        }

        // GET: MateriaProfesors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaProfesor = await _context.MateriaProfesor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaProfesor == null)
            {
                return NotFound();
            }

            return View(materiaProfesor);
        }

        // GET: MateriaProfesors/Create
        public IActionResult Create()
        {
            var profesores = (from p in _context.Profesor
                           select p).ToList();
            List<Profesor> profesSinMateria = new List<Profesor>();
            foreach(Profesor p in profesores)
            {
                MateriaProfesor profe = null;
                profe = _context.MateriaProfesor.Where(m => m.ProfesorId == p.Id)
                 .FirstOrDefault();
                if(profe == null)
                {
                    profesSinMateria.Add(p);
                }
            }
            var materias = (from m in _context.Materia
                            orderby m.Nombre ascending
                          select m).ToList();
            List<Materia> materiasSinProfe = new List<Materia>();
            foreach(Materia m in materias)
            {
                MateriaProfesor mate = null;
                mate = _context.MateriaProfesor.Where(x => x.MateriaId == m.Id)
                 .FirstOrDefault();
                if (mate == null)
                {
                    materiasSinProfe.Add(m);
                }
            }
            ViewBag.Profesores = profesSinMateria;
            ViewBag.Materias = materiasSinProfe;
            return View();
        }

        // POST: MateriaProfesors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProfesorId,MateriaId")] MateriaProfesor materiaProfesor)
        {
            if (ModelState.IsValid)
            {
                //Inserto el nuevo materiaProfesor en la BD
                _context.Add(materiaProfesor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materiaProfesor);
        }

        // GET: MateriaProfesors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaProfesor = await _context.MateriaProfesor.FindAsync(id);
            if (materiaProfesor == null)
            {
                return NotFound();
            }
            return View(materiaProfesor);
        }

        // POST: MateriaProfesors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProfesorId,MateriaId")] MateriaProfesor materiaProfesor)
        {
            if (id != materiaProfesor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materiaProfesor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriaProfesorExists(materiaProfesor.Id))
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
            return View(materiaProfesor);
        }

        public async Task<IActionResult> Remover(String profesorId, int materiaId)
        {
            //Con el DNI del profesor, busco su ID
            var idProfesor = (from p in _context.Profesor
                            where p.Dni == profesorId
                              select p.Id).FirstOrDefault();
            //Busco el ID del materiaProfesor que deseo eliminar, usando el ID de profesor y el ID de materia
            var idEliminar = (from mp in _context.MateriaProfesor
                              where mp.ProfesorId == idProfesor && mp.MateriaId == materiaId
                              select mp.Id).FirstOrDefault();
            //Agarro el materiaProfesor que deseo eliminar y lo elimino
            var materiaProfesor = await _context.MateriaProfesor.FindAsync(idEliminar);
            _context.MateriaProfesor.Remove(materiaProfesor);
            await _context.SaveChangesAsync();
            //Vuelvo a la vista de Details de Materias
            return RedirectToAction("Details", "Materias", new { id = materiaId });
        }

        // GET: MateriaProfesors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaProfesor = await _context.MateriaProfesor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaProfesor == null)
            {
                return NotFound();
            }

            return View(materiaProfesor);
        }

        // POST: MateriaProfesors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materiaProfesor = await _context.MateriaProfesor.FindAsync(id);
            _context.MateriaProfesor.Remove(materiaProfesor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriaProfesorExists(int id)
        {
            return _context.MateriaProfesor.Any(e => e.Id == id);
        }
    }
}
