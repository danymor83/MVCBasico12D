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
            //Trae todas las materias
            var materias = (from m in _context.Materia
                          orderby m.Nombre ascending
                          select m).ToList();
            //Trae todos los profesores
            var profesores = (from p in _context.Profesor
                           orderby p.Nombre ascending
                           select p).ToList();
            //Trae todas las relaciones entre materias y profesores
            var materiaProfesores = (from mp in _context.MateriaProfesor
                                 select mp).ToList();
            //Filtra la lista de profesores, dejando unicamente aquellos que esten relacionados con alguna materia
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
            //Envia todo a la view Index
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
            //Agarra todos los profesores
            var profesores = (from p in _context.Profesor
                           select p).ToList();
            //Filtra la lista de profesores, dejando unicamente aquellos que no tengan relación con ninguna materia
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
            //Agarra todas las materias
            var materias = (from m in _context.Materia
                            orderby m.Nombre ascending
                          select m).ToList();
            //Filtra la lista de materias, dejando unicamente aquellas que no tengan relación con ningun profesor
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
            //Envia todo a la view Create
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
                //Inserto la nueva relación entre materia y profesor en la BD
                _context.Add(materiaProfesor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //Si la relación es invalida vuelve a la view Create con toda la información necesaria y disponibiliza el mensaje de error
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

            //Trae la relación entre materia y profesor que se desea editar
            var materiaProfesor = await _context.MateriaProfesor.FindAsync(id);
            if (materiaProfesor == null)
            {
                return NotFound();
            }
            //Trae al profesor de la relación
            var profesor = _context.Profesor.Where(x => x.Id == materiaProfesor.ProfesorId).FirstOrDefault();
            //Trae todas las materias
            var materias = (from m in _context.Materia
                            select m).ToList();
            //Filtra la lista de materias, dejando unicamente aquellas que no tengan relación con ningun profesor
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
            //Agrega la materia actual de la relación a la lista de materias
            var materiaActual = _context.Materia.Where(x => x.Id == materiaProfesor.MateriaId).FirstOrDefault();
            materiasSinProfe.Add(materiaActual);

            //Envia todo a la view Edit
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
                    //Actualiza la relación en la BD
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
            //En caso de recibir una relación invalida, vuelve a la view Edit y disponibiliza el mensaje de error
            ViewBag.Erro = "display: inline; color:red;";
            return View(materiaProfesor);
        }

        public async Task<IActionResult> Remover(int profesorId, int materiaId)
        {
            //Busco el ID de la relación entre materia y profesor que deseo eliminar, usando el ID de profesor y el ID de materia
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
            //Trae la relación entre materia y profesor que deseo eliminar
            var materiaProfesor = await _context.MateriaProfesor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaProfesor == null)
            {
                return NotFound();
            }
            //Trae al profesor de la relación
            var profesor = _context.Profesor.Where(x => x.Id == materiaProfesor.ProfesorId).FirstOrDefault();
            //Trae la materia de la relación
            var materia = _context.Materia.Where(x => x.Id == materiaProfesor.MateriaId).FirstOrDefault();
            //Envia toda la información a la view Delete
            ViewBag.Profesor = profesor;
            ViewBag.Materia = materia;
            return View(materiaProfesor);
        }

        // POST: MateriaProfesors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Eliminar la relación entre materia y profesor del id correspondiente
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
