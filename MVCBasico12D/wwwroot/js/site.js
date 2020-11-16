// Se activa al tocar el boton de salir
function salir() {
    const exit = confirm("Desea salir?");
    const a = document.getElementById("salir");
    if (exit) {
        a.href = "https://localhost:44340/Home/";
    }
}

//Agarra el valor seleccionado de la lista de alumnos y asgina el DNI como valor del Input que lo envia al controller
function dniAlumno() {
    const dni = document.getElementById("alumnos").value.substring(8,16);
    document.getElementById("dni").value = dni;
    console.log(dni);
}

//Agarra el valor seleccionado de la lista de profesores y asgina el DNI como valor del Input que lo envia al controller
function dniProfesor() {
    const dni = document.getElementById("profesores").value.substring(8, 16);
    document.getElementById("dni").value = dni;
    console.log(dni);
}

//Agarra el valor seleccionado de la lista de cursos y asgina el ID como valor del Input que lo envia al controller
function siglaCurso() {
    const sigla = document.getElementById("cursos").value.substring(7, 8);
    if (!(document.getElementById("cursos").value.substring(8, 9) === ",")) {
        sigla = document.getElementById("cursos").value.substring(7, 9);
    }
    document.getElementById("sigla").value = sigla;
    console.log(sigla);
}

//Agarra el valor seleccionado de la lista de materias y asgina el ID como valor del Input que lo envia al controller
function idMateria() {
    const id = document.getElementById("materias").value.substring(7, 8);
    if (!(document.getElementById("materias").value.substring(8, 9) === ",")) {
        id = document.getElementById("materias").value.substring(7, 9);
    }
    document.getElementById("materiaId").value = id;
    console.log(id);
}

function asignarAlumno() {
    dniAlumno();
    siglaCurso();
}

function asignarMateria() {
    idMateria();
    siglaCurso();
}

function asignarProfesor() {
    dniProfesor();
    idMateria();
}


//Line 20: , new { onchange = "dniAlumno()" }
//Line 26: , new { onchange = "siglaCurso()" }
//, new { onchange = "idMateria()" }
//, new { onchange = "siglaCurso()" }
//, new { onchange = "dniProfesor()" }
//, new { onchange = "idMateria()" }