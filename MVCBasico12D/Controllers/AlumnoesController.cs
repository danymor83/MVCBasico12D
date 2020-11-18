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
    public class AlumnoesController : Controller
    {
        private readonly SchoolDBContext _context;

        public AlumnoesController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: Alumnoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Alumno.ToListAsync());
        }

        public async Task<IActionResult> Notas(int id)
        {
            return RedirectToAction("Alumno", "Notas", new { alumnoId = id});
        }

        public async Task<IActionResult> Inicio(String dni)
        {
            // 7 - Trae al alumno de la BD con el dni correspondiente
            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Dni == dni);
            var materias = (from m in _context.Materia
                            join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                            join c in _context.Curso on cm.CursoId equals c.Id
                            join ca in _context.CursoAlumno on c.Id equals ca.CursoId
                            join a in _context.Alumno on ca.AlumnoId equals alumno.Id
                            where a.Id == alumno.Id
                            orderby m.Nombre ascending
                            select m).ToList();
            ViewBag.Materias = materias;
           
            if (alumno == null)
            {
                return NotFound();
            }
            // 8 - Llama a la vista inicial de alumno y pasa al respectivo alumno como parametro
            return View(alumno);
        }

        [HttpPost]
        public async Task<IActionResult> paginaMateria(int id, String nombre)
        {
            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Id == id);
            //traer materias del alumno
            int materiaId = Convert.ToInt32(nombre);
            var materias = (from m in _context.Materia
                            join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                            join c in _context.Curso on cm.CursoId equals c.Id
                            join ca in _context.CursoAlumno on c.Id equals ca.CursoId
                            join a in _context.Alumno on ca.AlumnoId equals id
                            where a.Id == id
                            orderby m.Nombre ascending
                            select m).ToList();
            ViewBag.Materia = (from m in _context.Materia
                               where m.Id == materiaId
                               select m).FirstOrDefault();
            ViewBag.Materias = materias;
            return View(alumno);
        }

        // GET: Alumnoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumnoes/Create
        public IActionResult Create()
        {
            ViewBag.Erro = "display: none;";
            return View();
        }

        // POST: Alumnoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Dni,FechaNacimiento,Email,Telefono")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                var alum = _context.Usuarios.Where(x => x.Login == alumno.Dni).FirstOrDefault();
                if(alum == null)
                {
                    _context.Add(alumno);
                    await _context.SaveChangesAsync();
                    Usuario user = new Usuario();
                    user.Tipo = 1;
                    user.Login = alumno.Dni;
                    user.Password = alumno.Nombre.ToLower();
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Erro = "display: inline; color:red;";
            return View(alumno);
        }


        // GET: Alumnoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumno.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }
            ViewBag.Erro = "display: none;";
            return View(alumno);
        }

        // POST: Alumnoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Dni,FechaNacimiento,Email,Telefono")] Alumno alumno)
        {
            if (id != alumno.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mismoAlumno = _context.Alumno.Where(x => x.Id == id).FirstOrDefault();
                    var alum = _context.Usuarios.Where(x => x.Login == alumno.Dni).FirstOrDefault();                    
                    if(alum == null || alum.Login == mismoAlumno.Dni)
                    {
                        _context.Update(alumno);
                        _context.Update(alum);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumnoExists(alumno.Id))
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
            return View(alumno);
        }

        // GET: Alumnoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumnoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Busca las notas del alumno y las borra
            var notas = _context.Nota.Where(x => x.AlumnoId == id).ToList();
            if(notas != null)
            {
                _context.Nota.RemoveRange(notas);
            }            
            //Busca la relacion del alumno con su curso y lo borra
            var cursoAlumno = _context.CursoAlumno.Where(x => x.AlumnoId == id).FirstOrDefault();
            if(cursoAlumno != null)
            {
                _context.CursoAlumno.Remove(cursoAlumno);
            }            
            //Busca al alumno y lo borra de la tabla de alumnos y de la tabla de usuarios
            var alumno = await _context.Alumno.FindAsync(id);
            var alumnoUser = _context.Usuarios.Where(x => x.Login == alumno.Dni).FirstOrDefault();
            _context.Usuarios.Remove(alumnoUser);
            _context.Alumno.Remove(alumno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumno.Any(e => e.Id == id);
        }
    }
}
