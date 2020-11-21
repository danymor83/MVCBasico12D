// Se activa al tocar el boton de salir
//Pregunta si el usuario desea salir y en caso positivo, lo redirecciona a la página inicial de login
function salir() {
    const exit = confirm("Desea salir?");
    const a = document.getElementById("salir");
    if (exit) {
        a.href = "https://localhost:44340/Home/";
    }
}

//Agarra el alumno seleccionado y manda su ID al input
//Agarra el curso seleccionado y manda su ID al input
function asignarAlumno() {
    const alumnoId = document.getElementById("alumnos").value;
    document.getElementById("alumnoId").value = alumnoId;
    const cursoId = document.getElementById("cursos").value;
    document.getElementById("cursoId").value = cursoId;
}

//Agarra el alumno seleccionado y manda su ID al input
function idAlumno() {
    const alumnoId = document.getElementById("alumnos").value;
    document.getElementById("alumnoId").value = alumnoId;
}

//Agarra la materia seleccionada y manda su ID al input
function idMateria() {
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
}

//Agarra la materia seleccionada y manda su ID al input
//Agarra el curso seleccionado y manda su ID al input
function asignarMateria() {
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
    const cursoId = document.getElementById("cursos").value;
    document.getElementById("cursoId").value = cursoId;
}

//Agarra el profesor seleccionado y manda su ID al input
//Agarra la materia seleccionada y manda su ID al input
function asignarProfesor() {
    const profesorId = document.getElementById("profesores").value;
    document.getElementById("profesorId").value = profesorId;
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
}

//Agarra el valor de la fecha y valida que la persona no tenga menos de 6 años ni mas de 100
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

//Valida que el email tenga el formato correspondiente
function validateEmail() {
    var email = document.getElementById("email").value;
    if (/\S+@\S+\.\S+/.test(email)) {
        return true;
    }
    alert("Email inválido!")
    return false;
}

//Valida que el DNI tenga el formato correspondiente
function validarDni() {
    var dni = document.getElementById("dni").value;
    if (/^[0-9]{8,8}/.test(dni)) {
        return true;
    }
    alert("DNI inválido!")
    return false;
}

//Valida que el telefono tenga el formato correspondiente
function validarTelefono() {
    var dni = document.getElementById("telefono").value;
    if (/^[0-9]{8,8}/.test(dni)) {
        return true;
    }
    alert("Telefono inválido!")
    return false;
}

//Valida el Form, y en caso de ser invalido, manda un alert y devuelve "false" para impedir que se haga el submit del form
function validarFormPersona() {
    if (validarFecha() && validateEmail() && validarDni() && validarTelefono()) {
        return true;
    }
    return false;
}

//Agarra el curso seleccionado y manda su ID al input
function asignarCurso() {
    const cursoId = document.getElementById("cursos").value;
    document.getElementById("cursoId").value = cursoId;
}

//Agarra la materia seleccionada y manda su ID al input
function agarrarMateria() {
    const materiaId = document.getElementById("materias").value;
    document.getElementById("materiaId").value = materiaId;
}

//Agarra el profesor seleccionado y manda su ID al input
function agarrarProfesor() {
    const profesorId = document.getElementById("profesores").value;
    document.getElementById("profesorId").value = profesorId;
}

//Hace el submit del formulario correspondiente
function pageAlumno() {
    document.getElementById("pageAlumno").submit();
}

//Hace el submit del formulario correspondiente
function pageProfesor() {
    document.getElementById("pageProfesor").submit();
}

//Hace el submit del formulario correspondiente
function inicio() {
    document.getElementById("inicio").submit();
}

//Pasa el ID de la materia a la cual se desea acceder y luego hace el submit
function pageMateria(id) {
    document.getElementById("materiaId").value = id;
    document.getElementById("page").submit();
}

//Pasa el ID del curso al cual se desea acceder y luego hace el submit
function pageCurso(id) {
    document.getElementById("cursoId").value = id;
    document.getElementById("page").submit();
}