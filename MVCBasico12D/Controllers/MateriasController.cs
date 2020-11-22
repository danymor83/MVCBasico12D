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
    public class MateriasController : Controller
    {
        private readonly SchoolDBContext _context;

        public MateriasController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: Materias
        public async Task<IActionResult> Index()
        {
            //Envia una lista con todas las materias a la view Index
            return View(await _context.Materia.ToListAsync());
        }

        // GET: Materias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Trae la materia que se desea ver los detalles
            var materia = await _context.Materia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null)
            {
                return NotFound();
            }

            //Busco los profesores asignados a la materia correspondiente y los mando a la vista por el ViewBag
            var profesores = (from p in _context.Profesor
                           join mp in _context.MateriaProfesor on p.Id equals mp.ProfesorId
                           where mp.MateriaId == id
                           orderby p.Apellido ascending
                           select p).ToList();
            ViewBag.Profesores = profesores;
            return View(materia);
        }

        // GET: Materias/Create
        public IActionResult Create()
        {
            ViewBag.Erro = "display: none;";
            return View();
        }

        // POST: Materias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Anio")] Materia materia)
        {            
            if (ModelState.IsValid)
            {
                //Valido que la materia creada no exista previamente
                var mat = _context.Materia.Where(x => x.Nombre == materia.Nombre && x.Anio == materia.Anio).FirstOrDefault();
                if (mat == null)
                {
                    _context.Add(materia);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }                
            }
            //En caso de ser invalido, disponibilizo el mensaje de error
            ViewBag.Erro = "display: inline; color:red;";
            return View(materia);
        }

        // GET: Materias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Trae la materia que se desea editar
            var materia = await _context.Materia.FindAsync(id);
            if (materia == null)
            {
                return NotFound();
            }
            ViewBag.Erro = "display: none;";
            return View(materia);
        }

        // POST: Materias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Anio")] Materia materia)
        {
            if (id != materia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Valido que los nuevos datos de la materia no coinciadn con otra materia
                    //En caso de ser valido, actualizo sus valores
                    var mismaMat = _context.Materia.Where(x => x.Id == id).FirstOrDefault();
                    var mat = _context.Materia.Where(x => x.Nombre == materia.Nombre && x.Anio == materia.Anio).FirstOrDefault();
                    if(mat == null || mat == mismaMat)
                    {
                        _context.Update(materia);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriaExists(materia.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            //En caso de ser invalido, disponibilizo el mensaje de error
            ViewBag.Erro = "display: inline; color:red;";
            return View(materia);
        }

        // GET: Materias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Agarro la materia que deseo eliminar
            var materia = await _context.Materia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null)
            {
                return NotFound();
            }

            return View(materia);
        }

        // POST: Materias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Busca las materias del curso y borra sus relaciones
            var cursoMaterias = _context.CursoMateria.Where(x => x.MateriaId == id).ToList();
            if(cursoMaterias != null)
            {
                _context.CursoMateria.RemoveRange(cursoMaterias);
            }           
            //Busca la relacion entre profesor y materia y lo borra
            var materiasProfesor = _context.MateriaProfesor.Where(x => x.MateriaId == id).FirstOrDefault();
            if(materiasProfesor != null)
            {
                _context.MateriaProfesor.Remove(materiasProfesor);
            }            
            //Busca la materia y la borra
            var materia = await _context.Materia.FindAsync(id);
            _context.Materia.Remove(materia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriaExists(int id)
        {
            return _context.Materia.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> RemoverProfesor([Bind("Id,Nombre,Anio")] Materia materia)
        {
            //Recibe el ID del profesor de la relacion materia profesor que deseo eliminar
            int profId = Convert.ToInt32(materia.Nombre);
            //Envio el ID del profesor y el ID de la materia de la relación materia profesor que deseo eliminar al Action Remover de MateriaProfesorsController
            return RedirectToAction("Remover", "MateriaProfesors", new { profesorId = profId, materiaId = materia.Id });
        }
    }
}
