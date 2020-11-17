﻿using System;
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
            return View(await _context.Nota.ToListAsync());
        }
        public async Task<IActionResult> Alumno(int alumnoId)
        {
            var nota = (from n in _context.Nota
                        join a in _context.Alumno on n.AlumnoId equals a.Id
                        where a.Id == alumnoId
                        select n).FirstOrDefault();

            var materias = (from m in _context.Materia
                            join cm in _context.CursoMateria on m.Id equals cm.MateriaId
                            join c in _context.Curso on cm.CursoId equals c.Id
                            join ca in _context.CursoAlumno on c.Id equals ca.CursoId
                            join a in _context.Alumno on ca.AlumnoId equals alumnoId
                            where a.Id == alumnoId
                            orderby m.Id ascending
                            select m).ToList();
            ViewBag.Materias = materias;

            var notas = (from n in _context.Nota
                         join a in _context.Alumno on n.AlumnoId equals a.Id
                         where a.Id == alumnoId
                         orderby n.MateriaId ascending
                         select n).ToList();
            ViewBag.Notas = notas;
            ViewBag.Context = _context;
            return View(nota);
        }

        public async Task<IActionResult> InicioAlumno(int alumnoId)
        {
            var alumno = await _context.Alumno
                .FirstOrDefaultAsync(m => m.Id == alumnoId);
            return RedirectToAction("Inicio", "Alumnoes", new { dni = alumno.Dni });
        }

        public async Task<IActionResult> Profesor(int alumnoId)
        {
            var profe = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == alumnoId);
            ViewBag.Profesor = profe;

            var materia = (from m in _context.Materia
                           join mp in _context.MateriaProfesor on m.Id equals mp.MateriaId
                           join p in _context.Profesor on mp.ProfesorId equals profe.Id
                           where p.Id == profe.Id
                           select m).FirstOrDefault();
            ViewBag.Materia = materia;

            var cursos = (from c in _context.Curso
                          join cm in _context.CursoMateria on c.Id equals cm.CursoId
                          join mp in _context.MateriaProfesor on cm.MateriaId equals mp.MateriaId
                          join p in _context.Profesor on mp.ProfesorId equals profe.Id
                          where p.Id == profe.Id
                          select c).ToList();
            ViewBag.Cursos = cursos;


            var alumnos = (from a in _context.Alumno
                                join ca in _context.CursoAlumno on a.Id equals ca.AlumnoId
                                join cm in _context.CursoMateria on ca.CursoId equals cm.CursoId
                                join m in _context.Materia on cm.MateriaId equals m.Id
                                where m.Id == materia.Id
                                orderby a.Id ascending
                                select a).ToList();
            ViewBag.Alumnos = alumnos;

            var cursoAlumnos = (from ca in _context.CursoAlumno
                                 select ca).ToList();
            ViewBag.CursoAlumnos = cursoAlumnos;

            var notas = (from n in _context.Nota
                                join al in _context.Alumno on n.AlumnoId equals al.Id
                                where n.MateriaId == materia.Id
                                 orderby n.AlumnoId ascending
                                 orderby n.Cuatrimestre ascending
                                 select n).ToList();
            ViewBag.Notas = notas;
            return View();
        }

        public async Task<IActionResult> InicioProfesor(int alumnoId)
        {
            var profesor = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == alumnoId);
            return RedirectToAction("Inicio", "Profesors", new { dni = profesor.Dni });
        }

        public async Task<IActionResult> ActualizarNota(int id, int nota1, int cuatrimestre, int alumnoId, int materiaId)
        {
            var nota = await _context.Nota
                .FirstOrDefaultAsync(m => m.AlumnoId == alumnoId && m.MateriaId == materiaId && m.Cuatrimestre == cuatrimestre);
            if(nota == null)
            {
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
