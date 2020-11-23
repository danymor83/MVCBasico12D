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
    public class NotasController : Controller
    {
        private readonly SchoolDBContext _context;

        public NotasController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: Notas
        public async Task<IActionResult> Index()
        {
            //Trae todos los alumnos
            var alumnos = (from a in _context.Alumno
                           select a).ToList();
            //Trae todas las materias
            var materias = (from m in _context.Materia
                           select m).ToList();
            //Envia todo a la view Index
            ViewBag.Alumnos = alumnos;
            ViewBag.Materias = materias;
            return View(await _context.Nota.ToListAsync());
        }
        public async Task<IActionResult> Alumno(int alumnoId)
        {
            var nota = (from n in _context.Nota
                        join a in _context.Alumno on n.AlumnoId equals a.Id
                        where a.Id == alumnoId
                        select n).FirstOrDefault();
            //Agarra las materias relacionadas al curso del alumno
            var materias = (from m in _context.Materia
                            join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                            join c in _context.Curso on cm.CursoId equals c.Id
                            join ca in _context.CursoAlumno on c.Id equals ca.CursoId
                            join a in _context.Alumno on ca.AlumnoId equals alumnoId
                            where a.Id == alumnoId
                            orderby m.Id ascending
                            select m).ToList();
            ViewBag.Materias = materias;
            //Agarra las notas del alumno
            var notas = (from n in _context.Nota
                         join a in _context.Alumno on n.AlumnoId equals a.Id
                         where a.Id == alumnoId
                         orderby n.MateriaId ascending
                         select n).ToList();
            //Envia todo al view Alumno, donde muestra todas las calificaciones del alumno
            ViewBag.Notas = notas;
            ViewBag.Context = _context;
            if(materias.Count > 0)
            {
                return View(nota);
            }
            else
            {
                //En caso del alumno no tener niguna materia, vuelve a su view inicial
                var dni = (from a in _context.Alumno
                           where a.Id == alumnoId
                           select a.Dni).FirstOrDefault();
                return RedirectToAction("Inicio", "Alumnoes", new { dni = dni });
            }
            
        }

        public async Task<IActionResult> InicioAlumno(int alumnoId)
        {
            //Recibe el ID del alumno y lo manda a la pagina inicial del alumno
            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Id == alumnoId);
            return RedirectToAction("Inicio", "Alumnoes", new { dni = alumno.Dni });
        }

        public async Task<IActionResult> Profesor(int alumnoId)
        {
            //Agarra el profesor
            var profe = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == alumnoId);
            ViewBag.Profesor = profe;
            //Agarra la materia asignada al profesor
            var materia = (from m in _context.Materia
                           join mp in _context.MateriaProfesor on m.Id equals mp.MateriaId
                           join p in _context.Profesor on mp.ProfesorId equals profe.Id
                           where p.Id == profe.Id
                           select m).FirstOrDefault();
            ViewBag.Materia = materia;
            //Agarra los cursos del profesor
            var cursos = (from c in _context.Curso
                          join cm in _context.CursoMateria on c.Id equals cm.CursoId
                          join mp in _context.MateriaProfesor on cm.MateriaId equals mp.MateriaId
                          join p in _context.Profesor on mp.ProfesorId equals profe.Id
                          where p.Id == profe.Id
                          select c).ToList();
            ViewBag.Cursos = cursos;

            //Agarra los alumnos de los cursos del profesor
            var alumnos = (from a in _context.Alumno
                                join ca in _context.CursoAlumno on a.Id equals ca.AlumnoId
                                join cm in _context.CursoMateria on ca.CursoId equals cm.CursoId
                                join m in _context.Materia on cm.MateriaId equals m.Id
                                where m.Id == materia.Id
                                orderby a.Id ascending
                                select a).ToList();
            ViewBag.Alumnos = alumnos;
            //Agarra todas las relaciones entre curso y alumno
            var cursoAlumnos = (from ca in _context.CursoAlumno
                                 select ca).ToList();
            ViewBag.CursoAlumnos = cursoAlumnos;
            //Agarra todas las notas de los alumnos que cursan la materia en cuestion
            var notas = (from n in _context.Nota
                                join al in _context.Alumno on n.AlumnoId equals al.Id
                                where n.MateriaId == materia.Id
                                 orderby n.AlumnoId ascending
                                 orderby n.Cuatrimestre ascending
                                 select n).ToList();
            ViewBag.Notas = notas;
            //Se envia todo a la View Profesor = Calificaciones, donde el profesor sube las notas de sus alumnos y las modifica
            if(cursos.Count > 0)
            {
                return View();
            }
            else
            {
                //En el caso del profesor no tener ningun curso, vuelve a su view inicial
                var dni = (from p in _context.Profesor
                           where p.Id == alumnoId
                           select p.Dni).FirstOrDefault();
                return RedirectToAction("Inicio", "Profesors", new { dni = dni });
            }
            
        }

        public async Task<IActionResult> InicioProfesor(int alumnoId)
        {
            //Agarra al profesor que se desea y vuelve a la pantalla inicial del mismo
            var profesor = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == alumnoId);
            return RedirectToAction("Inicio", "Profesors", new { dni = profesor.Dni });
        }

        public async Task<IActionResult> ActualizarNota(int id, int nota1, int cuatrimestre, int alumnoId, int materiaId)
        {
            //Busca la nota que se desea actualizar
            var nota = await _context.Nota
                .FirstOrDefaultAsync(m => m.AlumnoId == alumnoId && m.MateriaId == materiaId && m.Cuatrimestre == cuatrimestre);
            if(nota == null)
            {
                //En caso de no existir, se crea una nueva nota y se la inserta en la BD
                Nota nuevaNota = new Nota();
                nuevaNota.AlumnoId = alumnoId;
                nuevaNota.MateriaId = materiaId;
                nuevaNota.Cuatrimestre = cuatrimestre;
                nuevaNota.Nota1 = nota1;
                _context.Add(nuevaNota);
                await _context.SaveChangesAsync();                
            }
            else
            {
                try
                {
                    //En caso de existir, se actualiza la nota en cuestion
                    nota.Nota1 = nota1;
                    _context.Update(nota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotaExists(nota.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            //Vuelve a la view Profesor
            return RedirectToAction("Profesor", "Notas", new { alumnoId = id });
        }

        // GET: Notas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Nota
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // GET: Notas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AlumnoId,MateriaId,Nota1,Cuatrimestre")] Nota nota)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nota);
        }

        // GET: Notas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Nota.FindAsync(id);
            if (nota == null)
            {
                return NotFound();
            }
            return View(nota);
        }

        // POST: Notas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlumnoId,MateriaId,Nota1,Cuatrimestre")] Nota nota)
        {
            if (id != nota.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotaExists(nota.Id))
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
            return View(nota);
        }

        // GET: Notas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Agarra la nota que se desea eliminar y manda sus datos a la pagina de confirmacion
            var nota = await _context.Nota
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // POST: Notas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Se borra la nota del respectivo ID
            var nota = await _context.Nota.FindAsync(id);
            _context.Nota.Remove(nota);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotaExists(int id)
        {
            return _context.Nota.Any(e => e.Id == id);
        }
    }
}
