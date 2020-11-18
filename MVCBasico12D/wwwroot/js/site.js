// Se activa al tocar el boton de salir
function salir() {
    const exit = confirm("Desea salir?");
    const a = document.getElementById("salir");
    if (exit) {
        a.href = "https://localhost:44340/Home/";
    }
}

function asignarAlumno() {
    const alumnoId = document.getElementById("alumnos").value;
    document.getElementById("alumnoId").value = alumnoId;
    const cursoId = document.getElementById("cursos").value;
    document.getElementById("cursoId").value = cursoId;
}

function idAlumno() {
    const alumnoId = document.getElementById("alumnos").value;
    document.getElementById("alumnoId").value = alumnoId;
}

function idMateria() {
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
}

function asignarMateria() {
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
    const cursoId = document.getElementById("cursos").value;
    document.getElementById("cursoId").value = cursoId;
}

function asignarProfesor() {
    const profesorId = document.getElementById("profesores").value;
    document.getElementById("profesorId").value = profesorId;
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
}


function validarFecha() {
    var fecha = document.getElementById("fechaNa").value;
    var partes = fecha.split("-");
    var date = new Date(partes[0] + "-" + partes[1] + "-" + partes[2]);
    var dataHoy = new Date();
    if (dataHoy.getFullYear() - date.getFullYear() < 6 || dataHoy.getFullYear() - date.getFullYear() > 100) {
        alert("Fecha de nacimiento invalida!");
        document.getElementById("fechaNa").focus();
        return false;
    }
    return true;
}

function validateEmail() {
    var email = document.getElementById("email").value;
    if (/\S+@\S+\.\S+/.test(email)) {
        return true;
    }
    alert("Email inválido!")
    return false;
}

function validarDni() {
    var dni = document.getElementById("dni").value;
    if (/^[0-9]{8,8}/.test(dni)) {
        return true;
    }
    alert("DNI inválido!")
    return false;
}

function validarTelefono() {
    var dni = document.getElementById("telefono").value;
    if (/^[0-9]{8,8}/.test(dni)) {
        return true;
    }
    alert("Telefono inválido!")
    return false;
}

function validarFormPersona() {
    if (validarFecha() && validateEmail() && validarDni() && validarTelefono()) {
        return true;
    }
    return false;
}

function asignarCurso() {
    const cursoId = document.getElementById("cursos").value;
    document.getElementById("cursoId").value = cursoId;
}

function agarrarMateria() {
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
}

function pageAlumno() {
    document.getElementById("pageAlumno").submit();
}

function inicio() {
    document.getElementById("inicio").submit();
}

function pageMateria(id) {
    document.getElementById("materiaId").value = id;
    document.getElementById("page").submit();
}