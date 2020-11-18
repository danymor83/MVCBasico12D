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
            var cursos = (from c in _context.Curso
                          orderby c.Sigla ascending
                          select c).ToList();
            var materias = (from m in _context.Materia
                           orderby m.Nombre ascending
                           select m).ToList();
            var cursosMaterias = (from cm in _context.CursoMateria
                                 select cm).ToList();
            List<Materia> materiasConCurso = new List<Materia>();
            foreach (Materia materia in materias)
            {
                bool pertenece = false;
                int i = 0;
                while (i < cursos.Count && !pertenece)
                {
                    int j = 0;
                    while (j < cursosMaterias.Count && !pertenece)
                    {
                        if (materia.Id == cursosMaterias.ElementAt(j).MateriaId && cursos.ElementAt(i).Id == cursosMaterias.ElementAt(j).CursoId)
                        {
                            materiasConCurso.Add(materia);
                            pertenece = true;
                        }
                        j++;
                    }
                    i++;
                }
            }
            ViewBag.Materias = materiasConCurso;
            ViewBag.Cursos = cursos;
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
            ViewBag.Erro = "display: none;";
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
            var materias = (from m in _context.Materia
                            orderby m.Nombre ascending
                            select m).ToList();
            var cursos = (from c in _context.Curso
                          orderby c.Sigla ascending
                          select c).ToList();
            ViewBag.Materias = materias;
            ViewBag.Cursos = cursos;
            ViewBag.Erro = "display: inline; color:red;";
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
            var materia = _context.Materia.Where(x => x.Id == cursoMateria.MateriaId).FirstOrDefault();
            var cursos = (from c in _context.Curso
                          select c).ToList();
            ViewBag.Materia = materia;
            ViewBag.Curso = cursos;
            ViewBag.Erro = "display: none;";
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
                    var cursoMat = _context.CursoMateria.Where(x => x.CursoId == cursoMateria.CursoId && x.MateriaId == cursoMateria.MateriaId).FirstOrDefault();
                    if(cursoMat == null)
                    {
                        _context.Update(cursoMateria);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }                    
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
            }
            var materia = _context.Materia.Where(x => x.Id == cursoMateria.MateriaId).FirstOrDefault();
            var cursos = (from c in _context.Curso
                          select c).ToList();
            ViewBag.Materia = materia;
            ViewBag.Curso = cursos;
            ViewBag.Erro = "display: inline; color:red;";
            return View(cursoMateria);
        }

        public async Task<IActionResult> Remover(int materiaId, int cursoId)
        {
            //Agarro el cursoMateria que deseo eliminar y lo elimino
            var cursoMateria = _context.CursoMateria.Where(x => x.MateriaId == materiaId && x.CursoId == cursoId).FirstOrDefault();
            if(cursoMateria == null)
            {
                return NotFound();
            }
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
            var materia = _context.Materia.Where(x => x.Id == cursoMateria.MateriaId).FirstOrDefault();
            var curso = _context.Curso.Where(x => x.Id == cursoMateria.CursoId).FirstOrDefault();
            ViewBag.Materia = materia;
            ViewBag.Curso = curso;
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
