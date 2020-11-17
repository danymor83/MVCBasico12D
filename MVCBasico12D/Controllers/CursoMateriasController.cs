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
    public class CursoMateriasController : Controller
    {
        private readonly SchoolDBContext _context;

        public CursoMateriasController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: CursoMaterias
        public async Task<IActionResult> Index()
        {
            return View(await _context.CursoMateria.ToListAsync());
        }

        // GET: CursoMaterias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursoMateria = await _context.CursoMateria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cursoMateria == null)
            {
                return NotFound();
            }

            return View(cursoMateria);
        }

        // GET: CursoMaterias/Create
        public IActionResult Create()
        {
            var materias = (from m in _context.Materia
                            orderby m.Nombre ascending
                            select m).ToList();
            var cursos = (from c in _context.Curso
                          orderby c.Sigla ascending
                          select c).ToList();
            ViewBag.Materias = materias;
            ViewBag.Cursos = cursos;
            return View();
        }

        // POST: CursoMaterias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CursoId,MateriaId")] CursoMateria cursoMateria)
        {
           if (ModelState.IsValid)
            {
                var cursoMat = _context.CursoMateria.Where(x => x.CursoId == cursoMateria.CursoId && x.MateriaId == cursoMateria.MateriaId).FirstOrDefault();
                if(cursoMat == null)
                {
                    _context.Add(cursoMateria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }               
            }
            return View(cursoMateria);
        }

        // GET: CursoMaterias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursoMateria = await _context.CursoMateria.FindAsync(id);
            if (cursoMateria == null)
            {
                return NotFound();
            }
            return View(cursoMateria);
        }

        // POST: CursoMaterias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CursoId,MateriaId")] CursoMateria cursoMateria)
        {
            if (id != cursoMateria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cursoMateria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoMateriaExists(cursoMateria.Id))
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
            return View(cursoMateria);
        }

        public async Task<IActionResult> Remover(int materiaId, int cursoId)
        {
            //Busco el ID del cursoMateria que deseo eliminar, usando el ID de la materia y el ID del curso
            var idEliminar = (from cm in _context.CursoMateria
                              where cm.MateriaId == materiaId && cm.CursoId == cursoId
                              select cm.Id).FirstOrDefault();
            //Agarro el cursoMateria que deseo eliminar y lo elimino
            var cursoMateria = await _context.CursoMateria.FindAsync(idEliminar);
            _context.CursoMateria.Remove(cursoMateria);
            await _context.SaveChangesAsync();
            //Vuelvo a la vista de Details de Cursos
            return RedirectToAction("Details", "Cursoes", new { id = cursoId });
        }

        // GET: CursoMaterias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cursoMateria = await _context.CursoMateria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cursoMateria == null)
            {
                return NotFound();
            }

            return View(cursoMateria);
        }

        // POST: CursoMaterias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cursoMateria = await _context.CursoMateria.FindAsync(id);
            _context.CursoMateria.Remove(cursoMateria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoMateriaExists(int id)
        {
            return _context.CursoMateria.Any(e => e.Id == id);
        }
    }
}
