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
    public class ProfesorsController : Controller
    {
        private readonly SchoolDBContext _context;

        public ProfesorsController(SchoolDBContext context)
        {
            _context = context;
        }

        // GET: Profesors
        public async Task<IActionResult> Index()
        {
            //Trae la lista de todos los profesores y lo manda a la view Index
            return View(await _context.Profesor.ToListAsync());
        }

        public async Task<IActionResult> Notas(int id)
        {
            //Redirecciona al Action Profesor de NotasController enviando el ID del respectivo profesor
            return RedirectToAction("Profesor", "Notas", new { alumnoId = id });
        }
        public async Task<IActionResult> Inicio(String dni)
        {
            //Trae al profe de la BD con el dni correspondiente
            var profe = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Dni == dni);
            //Trae todos los cursos deste profesor
            var cursos = (from c in _context.Curso
                          join cm in _context.CursoMateria on c.Id equals cm.CursoId
                          join mp in _context.MateriaProfesor on cm.MateriaId equals mp.MateriaId                            
                          join p in _context.Profesor on mp.ProfesorId equals profe.Id
                          where p.Id == profe.Id
                          orderby c.Sigla ascending
                          select c).ToList();
            ViewBag.Cursos = cursos;

            if (profe == null)
            {
                return NotFound();
            }
            //Llama a la vista inicial de profe y envia toda la información
            return View(profe);
        }

        [HttpPost]
        public async Task<IActionResult> paginaCurso(int id, String nombre)
        {
            //Busca el profesor
            var profe = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == id);
            //Busca los cursos del profe
            var cursos = (from c in _context.Curso
                          join cm in _context.CursoMateria on c.Id equals cm.CursoId
                          join mp in _context.MateriaProfesor on cm.MateriaId equals mp.MateriaId
                          join p in _context.Profesor on mp.ProfesorId equals profe.Id
                          where p.Id == profe.Id
                          orderby c.Sigla ascending
                          select c).ToList();
            ViewBag.Cursos = cursos;

            //Busca la sigla del curso actual
            var cursoId = Convert.ToInt32(nombre);
            ViewBag.Curso = (from c in _context.Curso
                             where c.Id == cursoId
                             select c.Sigla).FirstOrDefault();
            //Busca el ID de la materia relacionada al profesor
            int materiaId = (from m in _context.Materia
                             join mp in _context.MateriaProfesor on m.Id equals mp.MateriaId
                             join p in _context.Profesor on mp.ProfesorId equals profe.Id
                             where p.Id == profe.Id
                             select m.Id).FirstOrDefault();
            //Busca la materia
            ViewBag.Materia = (from m in _context.Materia
                               where m.Id == materiaId
                               select m.Nombre).FirstOrDefault();
            //Envia todo a la view paginaCurso
            return View(profe);
        }

        // GET: Profesors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Busca el profesor que se desea conocer sus detalles
            var profesor = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profesor == null)
            {
                return NotFound();
            }

            return View(profesor);
        }

        // GET: Profesors/Create
        public IActionResult Create()
        {
            //Envia el codigo que hace invisible el mensaje de error en la view Create
            ViewBag.Erro = "display: none;";
            return View();
        }

        // POST: Profesors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Dni,FechaNacimiento,Email,Telefono")] Profesor profesor)
        {
            if (ModelState.IsValid)
            {
                //Verifica que el DNI del profesor que se quiere crear no exista previamente
                var profe = _context.Usuarios.Where(x => x.Login == profesor.Dni).FirstOrDefault();
                if(profe == null)
                {
                    //En caso de no existir, se crea el nuevo profesor y un usuario para el mismo
                    _context.Add(profesor);
                    await _context.SaveChangesAsync();
                    Usuario user = new Usuario();
                    user.Tipo = 2;
                    user.Login = profesor.Dni;
                    user.Password = profesor.Nombre.ToLower();
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }                
            }
            //En caso de ya existir, disponibiliza un mensaje de error y vuelve a la view Create
            ViewBag.Erro = "display: inline; color:red;";
            return View(profesor);
        }

        // GET: Profesors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Busca al profesor que se desea editar
            var profesor = await _context.Profesor.FindAsync(id);
            if (profesor == null)
            {
                return NotFound();
            }
            return View(profesor);
        }

        // POST: Profesors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Dni,FechaNacimiento,Email,Telefono")] Profesor profesor)
        {
            if (id != profesor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Hace un update en la tabla con los nuevos datos del profesor
                    _context.Update(profesor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));                 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesorExists(profesor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(profesor);
        }

        // GET: Profesors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Busca el profesor que se desea eliminar y manda su información a la página de confirmación
            var profesor = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profesor == null)
            {
                return NotFound();
            }

            return View(profesor);
        }

        // POST: Profesors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Busca la relacion del profesor con su materia y lo borra
            var materiaProfesor = _context.MateriaProfesor.Where(x => x.ProfesorId == id).FirstOrDefault();
            if(materiaProfesor != null)
            {
                _context.MateriaProfesor.Remove(materiaProfesor);
            }                
            //Buscar al profesor y lo borra de la tabla de profesores y de la tabla de usuarios
            var profesor = await _context.Profesor.FindAsync(id);
            var profesorUser = _context.Usuarios.Where(x => x.Login == profesor.Dni).FirstOrDefault();
            _context.Usuarios.Remove(profesorUser);
            _context.Profesor.Remove(profesor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfesorExists(int id)
        {
            return _context.Profesor.Any(e => e.Id == id);
        }
    }
}
