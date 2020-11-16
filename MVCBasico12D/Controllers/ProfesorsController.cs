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
            return View(await _context.Profesor.ToListAsync());
        }

        public async Task<IActionResult> Notas(int id)
        {
            return RedirectToAction("Profesor", "Notas", new { alumnoId = id });
        }
        public async Task<IActionResult> Inicio(String dni)
        {
            // 7 - Trae al profe de la BD con el dni correspondiente
            var profe = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Dni == dni);
            var cursos = (from c in _context.Curso
                          join cm in _context.CursoMateria on c.Id equals cm.CursoId
                          join mp in _context.MateriaProfesor on cm.MateriaId equals mp.MateriaId                            
                          join p in _context.Profesor on mp.ProfesorId equals profe.Id
                          where p.Id == profe.Id
                          orderby c.Sigla ascending
                          select c.Sigla).ToList();
            ViewBag.Cursos = cursos;

            if (profe == null)
            {
                return NotFound();
            }
            // 8 - Llama a la vista inicial de profe y pasa al respectivo profe como parametro
            return View(profe);
        }

        [HttpPost]
        public async Task<IActionResult> paginaCurso(int id, String nombre)
        {
            var profe = await _context.Profesor
                .FirstOrDefaultAsync(m => m.Id == id);
            //traer cursos del profe
            var cursos = (from c in _context.Curso
                          join cm in _context.CursoMateria on c.Id equals cm.CursoId
                          join mp in _context.MateriaProfesor on cm.MateriaId equals mp.MateriaId
                          join p in _context.Profesor on mp.ProfesorId equals profe.Id
                          where p.Id == profe.Id
                          orderby c.Sigla ascending
                          select c.Sigla).ToList();
            ViewBag.Cursos = cursos;

            //Sigla del curso actual
            ViewBag.Curso = nombre;

            int materiaId = (from m in _context.Materia
                             join mp in _context.MateriaProfesor on m.Id equals mp.MateriaId
                             join p in _context.Profesor on mp.ProfesorId equals profe.Id
                             where p.Id == profe.Id
                             select m.Id).FirstOrDefault();
            //Materia actual
            ViewBag.Materia = (from m in _context.Materia
                               where m.Id == materiaId
                               select m.Nombre).FirstOrDefault();
            return View(profe);
        }

        // GET: Profesors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
            return View(profesor);
        }

        // GET: Profesors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
                    _context.Update(profesor);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
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
            var profesor = await _context.Profesor.FindAsync(id);
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