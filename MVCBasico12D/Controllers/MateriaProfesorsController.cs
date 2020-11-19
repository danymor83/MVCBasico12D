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
            var materias = (from m in _context.Materia
                          orderby m.Nombre ascending
                          select m).ToList();
            var profesores = (from p in _context.Profesor
                           orderby p.Nombre ascending
                           select p).ToList();
            var materiaProfesores = (from mp in _context.MateriaProfesor
                                 select mp).ToList();
            List<Profesor> profesorConMateria = new List<Profesor>();
            foreach (Profesor profesor in profesores)
            {
                bool pertenece = false;
                int i = 0;
                while (i < materias.Count && !pertenece)
                {
                    int j = 0;
                    while (j < materiaProfesores.Count && !pertenece)
                    {
                        if (profesor.Id == materiaProfesores.ElementAt(j).ProfesorId && materias.ElementAt(i).Id == materiaProfesores.ElementAt(j).MateriaId)
                        {
                            profesorConMateria.Add(profesor);
                            pertenece = true;
                        }
                        j++;
                    }
                    i++;
                }
            }
            ViewBag.Profesores = profesorConMateria;
            ViewBag.Materias = materias;
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
            ViewBag.Erro = "display: none;";
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
            var profesores = (from p in _context.Profesor
                              select p).ToList();
            List<Profesor> profesSinMateria = new List<Profesor>();
            foreach (Profesor p in profesores)
            {
                MateriaProfesor profe = null;
                profe = _context.MateriaProfesor.Where(m => m.ProfesorId == p.Id)
                 .FirstOrDefault();
                if (profe == null)
                {
                    profesSinMateria.Add(p);
                }
            }
            var materias = (from m in _context.Materia
                            orderby m.Nombre ascending
                            select m).ToList();
            List<Materia> materiasSinProfe = new List<Materia>();
            foreach (Materia m in materias)
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
            ViewBag.Erro = "display: inline; color:red;";
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
            var profesor = _context.Profesor.Where(x => x.Id == materiaProfesor.ProfesorId).FirstOrDefault();
            var materias = (from m in _context.Materia
                            select m).ToList();
            List<Materia> materiasSinProfe = new List<Materia>();
            foreach (Materia m in materias)
            {
                MateriaProfesor mate = null;
                mate = _context.MateriaProfesor.Where(x => x.MateriaId == m.Id)
                 .FirstOrDefault();
                if (mate == null)
                {
                    materiasSinProfe.Add(m);
                }
            }

            var materiaActual = _context.Materia.Where(x => x.Id == materiaProfesor.MateriaId).FirstOrDefault();
            materiasSinProfe.Add(materiaActual);

            ViewBag.Profesor = profesor;
            ViewBag.Materias = materiasSinProfe;
            ViewBag.Erro = "display: none;";
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
            ViewBag.Erro = "display: inline; color:red;";
            return View(materiaProfesor);
        }

        public async Task<IActionResult> Remover(int profesorId, int materiaId)
        {
            //Busco el ID del materiaProfesor que deseo eliminar, usando el ID de profesor y el ID de materia
            var idEliminar = (from mp in _context.MateriaProfesor
                              where mp.ProfesorId == profesorId && mp.MateriaId == materiaId
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
            var profesor = _context.Profesor.Where(x => x.Id == materiaProfesor.ProfesorId).FirstOrDefault();
            var materia = _context.Materia.Where(x => x.Id == materiaProfesor.MateriaId).FirstOrDefault();
            ViewBag.Profesor = profesor;
            ViewBag.Materia = materia;
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
