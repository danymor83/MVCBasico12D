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
            //Agarra todos los cursos
            var cursos = (from c in _context.Curso
                          orderby c.Sigla ascending
                          select c).ToList();
            //Agarra todos los alumnos
            var alumnos = (from a in _context.Alumno
                           orderby a.Nombre ascending
                           select a).ToList();
            //Agarra todas las relaciones entre cursos y alumnos
            var cursosAlumnos = (from ca in _context.CursoAlumno
                                 select ca).ToList();
            //Filtra la lista de alumnos, dejando unicamente aquellos que tienen un curso
            List<Alumno> alumnosConCurso = new List<Alumno>();
            foreach(Alumno alumno in alumnos)
            {
                bool pertenece = false;
                int i = 0;
                while (i < cursos.Count && !pertenece)
                {
                    int j = 0;
                    while(j < cursosAlumnos.Count && !pertenece)
                    {
                        if(alumno.Id == cursosAlumnos.ElementAt(j).AlumnoId && cursos.ElementAt(i).Id == cursosAlumnos.ElementAt(j).CursoId){
                            alumnosConCurso.Add(alumno);
                            pertenece = true;
                        }
                        j++;
                    }
                    i++;                    
                }                                
            }
            //Envia todo a la view Index
            ViewBag.Alumnos = alumnosConCurso;
            ViewBag.Cursos = cursos;
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
            //Trae todos los alumnos
            var alumnos = (from a in _context.Alumno
                              select a).ToList();
            List<Alumno> alumnosSinCurso = new List<Alumno>();
            //Trae todos los cursos
            var cursos = (from c in _context.Curso
                             select c).ToList();
            //Filtra la lista de alumnos, dejando unicamente aquellos que no tienen un curso asignado
            foreach (Alumno a in alumnos) {
                var cursoAlumno = _context.CursoAlumno.Where(m => m.AlumnoId == a.Id)
                 .FirstOrDefault();
                if(cursoAlumno == null)
                {
                    alumnosSinCurso.Add(a);
                }
            }

            //Se envia todo a la view Create
            ViewBag.Alumnos = alumnosSinCurso;
            ViewBag.Cursos = cursos;
            ViewBag.Erro = "display: none;";
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
            //En caso de ser invalido, disponibiliza el mensaje de error
            ViewBag.Erro = "display: inline; color:red;";
            return View(cursoAlumno);
        }

        // GET: CursoAlumnoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Trae la relacion entre curso y alumno que se desea editar
            var cursoAlumno = await _context.CursoAlumno.FindAsync(id);
            if (cursoAlumno == null)
            {
                return NotFound();
            }
            //Trae al alumno de la relación
            var alumno = _context.Alumno.Where(x => x.Id == cursoAlumno.AlumnoId).FirstOrDefault();
            //Trae todos los cursos
            var cursos = (from c in _context.Curso
                         select c).ToList();
            //Se envia todo a la view Edit
            ViewBag.Alumno = alumno;
            ViewBag.Cursos = cursos;            
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
                    //Actualiza la relación entre curso y alumno, asignando el alumno al nuevo curso
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
        public async Task<IActionResult> Remover(int alumnoId, int cursoId)
        {
            //Agarro la relación de curso con alumno que deseo eliminar y lo elimino
            var cursoAlumno = _context.CursoAlumno.Where(x => x.AlumnoId == alumnoId && x.CursoId == cursoId).FirstOrDefault();
            if(cursoAlumno == null)
            {
                return NotFound();
            }
            _context.CursoAlumno.Remove(cursoAlumno);
            await _context.SaveChangesAsync();
            //Vuelvo a la view Details de Cursos
            return RedirectToAction("Details", "Cursoes", new { id = cursoId});
        }
        // GET: CursoAlumnoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Busca la relación de curso con alumno que se desea eliminar
            var cursoAlumno = await _context.CursoAlumno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cursoAlumno == null)
            {
                return NotFound();
            }
            //Trae el alumno de la relación
            var alumno = _context.Alumno.Where(x => x.Id == cursoAlumno.AlumnoId).FirstOrDefault();
            //Trae el curso de la relación
            var curso = _context.Curso.Where(x => x.Id == cursoAlumno.CursoId).FirstOrDefault();
            //Se envia todo a la view Delete
            ViewBag.Alumno = alumno;
            ViewBag.Curso = curso;
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
