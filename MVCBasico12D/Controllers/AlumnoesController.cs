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
            //Trae la lista de todos los alumnos y lo envia a la view Index
            return View(await _context.Alumno.ToListAsync());
        }

        //Redirecciona el usuario a la pagina de calificaciones, enviando el ID del alumno correspondiente
        public async Task<IActionResult> Notas(int id)
        {
            return RedirectToAction("Alumno", "Notas", new { alumnoId = id});
        }

        //Recibe el DNI del alumno, trae toda su información de la BD y las materias de su curso
        //Luego envia todo a la View Inicio
        public async Task<IActionResult> Inicio(String dni)
        {
            //Trae al alumno de la BD con su DNI
            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Dni == dni);
            //Trae las materias del alumno de la BD 
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
            //Llama a la vista inicial de alumno y pasa al respectivo alumno como parametro
            return View(alumno);
        }

        //Recibe el id del alumno y el id de la materia que se desea acceder
        //Se envia todo a la view paginaMateria
        [HttpPost]
        public async Task<IActionResult> paginaMateria(int id, String nombre)
        {
            //Trae al alumno de la BD con su ID
            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Id == id);
            //Trae las materias del alumno
            int materiaId = Convert.ToInt32(nombre);
            var materias = (from m in _context.Materia
                            join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                            join c in _context.Curso on cm.CursoId equals c.Id
                            join ca in _context.CursoAlumno on c.Id equals ca.CursoId
                            join a in _context.Alumno on ca.AlumnoId equals id
                            where a.Id == id
                            orderby m.Nombre ascending
                            select m).ToList();
            //Trae la materia que se desea acceder
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

            //Trae el alumno que se desea conocer los detalles y se lo envia a la view Details
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
            //Envia el codigo para dejar el mensaje de error invisible en la view Create
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
                //Busca en la BD un alumno que ya exista con el DNI que se desea crear el nuevo alumno                
                var alum = _context.Usuarios.Where(x => x.Login == alumno.Dni).FirstOrDefault();
                //En caso de no existir, se agrega el nuevo alumno y se crea un nuevo usuario de alumno
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
            //En caso de existir un alumno con el DNI recibido, se disponibiliza el mensaje de error
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
            //Trae el alumno que se desea editar y se lo envia a la view Edit
            var alumno = await _context.Alumno.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }
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
                    //Hace un update en la tabla con los nuevos datos del profesor
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }
            return View(alumno);
        }

        // GET: Alumnoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Agarra el alumno que se desea eliminar y lo envia a la view Delete donde se confirma la baja o se cancela
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
