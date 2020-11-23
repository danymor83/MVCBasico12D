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
            //Trae la lista de todos los cursos y lo envia a la view Index
            return View(await _context.Curso.ToListAsync());
        }
        

        // GET: Cursoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Agarra el curso que se desea conocer los detalles
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
                           select a).ToList();
            ViewBag.Alumnos = alumnos;

            //Busco las materias asignadas al curso correspondiente y las mando a la vista por el ViewBag
            var materias = (from m in _context.Materia
                           join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                           where cm.CursoId == id
                           orderby m.Nombre ascending
                            select m).ToList();
            ViewBag.Materias = materias;
            return View(curso);
        }

        // GET: Cursoes/Create
        public IActionResult Create()
        {
            //Envia el codigo para dejar el mensaje de error invisible en la view Create
            ViewBag.Erro = "display: none;";
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
                //Valida que la sigla del nuevo curso no exista en la BD
                var curs = _context.Curso.Where(x => x.Sigla == curso.Sigla).FirstOrDefault();
                if (curs == null)
                {
                    _context.Add(curso);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }                
            }
            //En caso de existir un curso con la sigla recibida, se disponibiliza el mensaje de error
            ViewBag.Erro = "display: inline; color:red;";
            return View(curso);
        }

        // GET: Cursoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Agarra el curso que se desea editar
            var curso = await _context.Curso.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            //Envia el codigo para dejar el mensaje de error invisible en la view Edit
            ViewBag.Erro = "display: none;";
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
                    //Valida que la sigla recibida sea del mismo curso y no de otro
                    //En caso de la sigla ser diferente, verifica que no sea una sigla ya existente en la BD
                    //En caso de no existir, se actualiza la sigla del curso
                    var mismoCurso = _context.Curso.Where(x => x.Id == id).FirstOrDefault();
                    var curs = _context.Curso.Where(x => x.Sigla == curso.Sigla).FirstOrDefault();
                    if(curs == null || curso.Sigla == mismoCurso.Sigla)
                    {
                        _context.Update(curso);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }                    
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
            }
            //En el caso de la sigla ser invalida, vuelve a la view Edit y disponibiliza el mensaje de error
            ViewBag.Erro = "display: inline; color:red;";
            return View(curso);
        }

        // GET: Cursoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Agarra el curso que se desea eliminar y lo envia a la view Delete donde se confirma la baja o se cancela
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
            //Busca todos los alumnos del curso y borra sus relaciones
            var cursoAlumnos = _context.CursoAlumno.Where(x => x.CursoId == id).ToList();
            if(cursoAlumnos != null)
            {
                _context.CursoAlumno.RemoveRange(cursoAlumnos);
            }            
            //Busca todas las materias del curso y borra sus relaciones
            var cursoMaterias = _context.CursoMateria.Where(x => x.CursoId == id).ToList();
            if(cursoMaterias != null)
            {
                _context.CursoMateria.RemoveRange(cursoMaterias);
            }            
            //Busca el curso y lo borra
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
            //Recibe el id del alumno que se desea eliminar del curso y el id del curso
            int alumId = Convert.ToInt32(curso.Sigla);
            //Redirecciona al Action Remover del Controller CursoAlumnoes, donde se dará debaja la relación
            if(alumId != 0)
            {
                return RedirectToAction("Remover", "CursoAlumnoes", new { alumnoId = alumId, cursoId = curso.Id });
            }
            else
            {
                return RedirectToAction("Details", "Cursoes", new { id = curso.Id });
            }
                        
        }

        [HttpPost]
        public async Task<IActionResult> RemoverMateria([Bind("Id,Sigla")] Curso curso)
        {
            //Recibe el id de la materia que se desea eliminar del curso y el id del curso
            int matId = Convert.ToInt32(curso.Sigla);
            //Redirecciona al Action Remover del Controller CursoMaterias, donde se dará debaja la relación
            if(matId != 0)
            {
                return RedirectToAction("Remover", "CursoMaterias", new { materiaId = matId, cursoId = curso.Id });
            }
            else
            {
                return RedirectToAction("Details", "Cursoes", new { id = curso.Id });
            }
            
        }
    }
}
//
//