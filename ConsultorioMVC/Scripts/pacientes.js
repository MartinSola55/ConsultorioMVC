listar();
llenarCombo();
let header = ["ID", "Nombre", "Apellido", "Teléfono", "Correo", "Obra Social"];

function listar() {
    $.get("../Pacientes/getAll", function (data) {
        listadoPacientes(header, data);
    });
}

function listadoPacientes(arrayHeader, data) {
    let contenido = "";
    contenido += "<table id='tabla-generic' class='table table-dark table-striped table-bordered table-hover'>";
    contenido += "<thead>";
    contenido += "<tr>";
    for (let i = 0; i < arrayHeader.length; i++) {
        contenido += "<td class='text-center'>";
        contenido += arrayHeader[i];
        contenido += "</td>";
    }
    contenido += "<td class='no-sort text-center'>Acción</td>";
    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";
    for (let i = 0; i < data.length; i++) {
        let keyID = Object.keys(data[0])[0];
        contenido += "<tr>";
        contenido += "<td class='text-center'>" + data[i].id + "</td>";
        contenido += "<td>" + data[i].nombre + "</td>";
        contenido += "<td>" + data[i].apellido + "</td>";
        contenido += "<td>" + data[i].telefono + "</td>";
        let email = (data[i].correo == null) ? "-" : data[i].correo;
        contenido += "<td>" + email + "</td>";
        contenido += "<td>" + data[i].nombreOS + "</td>";
        contenido += "<td class='d-flex justify-content-center'>";
        contenido += "<button class='btn btn-outline-success me-4' onclick='modalEdit(" + data[i][keyID] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button class='btn btn-outline-danger ms-4' onclick='modalDelete(" + data[i][keyID] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-trash3'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    $("#tabla-clase").html(contenido);
    $('#tabla-generic').DataTable(
        {
            searching: false,
            "language": {
                paginate: {
                    "first": "Primero",
                    "last": "Último",
                    "next": "Siguiente",
                    "previous": "Anterior"
                },
                emptyTable: "No existen pacientes que coincidan con la búsqueda",
                info: "Mostrando _START_ a _END_ de _TOTAL_ pacientes",
                infoEmpty: "Mostrando 0 pacientes",
                lengthMenu: "Mostrar _MENU_ pacientes",
            },
            columnDefs: [{
                orderable: false,
                targets: "no-sort"
            }]
        }
    )
    $("#tabla-generic").removeAttr("style");
}


function llenarCombo() {
    $.get("../ObrasSociales/getAll", function (data) {
        let control = $("#filtroOS");
        let contenido = "";
        contenido += "<option value='' disabled selected >--Seleccione una opción--</option>";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].id + "'>";
            contenido += data[i].nombre;
            contenido += "</option>";
        }
        control.html(contenido);
    });
}

jQuery('#filtroNombre').on('input', function () {
    let nombre = $("#filtroNombre").val();
    let apellido = $("#filtroApellido").val();
    let os = $("#filtroOS").val();
    if (os == null) {
        $.get("../Pacientes/buscarPacientesNombreApellido/?nombre=" + nombre + "&apellido=" + apellido, function (data) {
            listadoPacientes(header, data);
        });
    } else {
        $.get("../Pacientes/buscarPacientesNombApOS/?nombre=" + nombre + "&apellido=" + apellido + "&os=" + os, function (data) {
            listadoPacientes(header, data);
        });
    }
});

jQuery('#filtroApellido').on('input', function () {
    let nombre = $("#filtroNombre").val();
    let apellido = $("#filtroApellido").val();
    let os = $("#filtroOS").val();
    if (os == null) {
        $.get("../Pacientes/buscarPacientesNombreApellido/?nombre=" + nombre + "&apellido=" + apellido, function (data) {
            listadoPacientes(header, data);
        });
    } else {
        $.get("../Pacientes/buscarPacientesNombApOS/?nombre=" + nombre + "&apellido=" + apellido + "&os=" + os, function (data) {
            listadoPacientes(header, data);
        });
    }
});

jQuery('#filtroOS').on('change', function () {
    let nombre = $("#filtroNombre").val();
    let apellido = $("#filtroApellido").val();
    let os = $("#filtroOS").val();
    $.get("../Pacientes/buscarPacientesNombApOS/?nombre=" + nombre + "&apellido=" + apellido + "&os=" + os, function (data) {
        listadoPacientes(header, data);
    });
});

jQuery('#limpia-filtro').on('click', function () {
    listar();
    $("#filtroNombre").val("");
    $('#filtroApellido').val("");
    $('#filtroOS').val("");
});

jQuery('#btnAgregar').on('click', function () {
    limpiarCampos();
    habilitarCampos();
    $("#staticBackdropLabel").text("Agregar paciente");
});

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar paciente");
    limpiarCampos();
    habilitarCampos();
    $.get("../Pacientes/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#txtApellido").val(data[0]['apellido']);
        $("#txtTelefono").val(data[0]['telefono']);
        $("#txtCorreo").val(data[0]['correo']);
        $("#txtObraSocial").val(data[0]['obra_social_id']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar obra social");
    limpiarCampos();
    deshabilitarCampos();
    $("#btnAceptar").addClass("eliminar");
    $.get("../Pacientes/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#txtApellido").val(data[0]['apellido']);
        $("#txtTelefono").val(data[0]['telefono']);
        $("#txtCorreo").val(data[0]['correo']);
        $("#txtObraSocial").val(data[0]['obra_social_id']);
    });
    let frm = new FormData();
    frm.append("id", id);
}

function limpiarCampos() {
    $(".limpiarCampo").val("");
    campos = $(".required");
    for (let i = 0; i < campos.length; i++) {
        $(".campo" + i).removeClass("error");
    }
    $("#btnAceptar").removeClass("eliminar");
}

function habilitarCampos() {
    $(".habilitarCampo").removeAttr("disabled");
}

function deshabilitarCampos() {
    $(".deshabilitarCampo").attr("disabled", "disabled");
}

function confirmarCambios() {
    if (campoRequired()) {
        let frm = new FormData();
        let id = $("#txtID").val();
        let nombre = $("#txtNombre").val();
        let apellido = $("#txtApellido").val();
        let telefono = $("#txtTelefono").val();
        let correo = $("#txtCorreo").val();
        let obraSocial = $("#txtObraSocial").val();
        frm.append("id", id);
        frm.append("nombre", nombre);
        frm.append("apellido", apellido);
        frm.append("telefono", telefono);
        frm.append("correo", correo);
        frm.append("obra_social_id", obraSocial);
        if ($("#btnAceptar").hasClass("eliminar")) {
            if (confirm("¿Seguro que desea eliminar el paciente?") == 1) {
                crudPaciente(frm, "delete");
            }
        } else {
            crudPaciente(frm, "save");
        }
    }
}

function campoRequired() {
    let valido = true;
    campos = $(".required");
    for (let i = 0; i < campos.length; i++) {
        if (campos[i].value == "") {
            valido = false;
            $(".campo" + i).addClass("error");
        } else {
            $(".campo" + i).removeClass("error");
        }
    }
    return valido;
}

function crudPaciente(frm, action) {
    $.ajax({
        type: "POST",
        url: "../Pacientes/" + action,
        data: frm,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data != 0) {
                listar();
                if ($("#btnAceptar").hasClass("eliminar")) {
                    alert("El paciente se eliminó correctamente");
                } else {
                    alert("El paciente se guardó correctamente");
                }
                $("#btnCancelar").click();

            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
        }
    });
}