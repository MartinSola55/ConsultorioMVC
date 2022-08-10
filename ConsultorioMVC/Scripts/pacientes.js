llenarCombo("filtroOS");
let header = ["Nombre", "Apellido", "Dirección", "Localidad", "Teléfono", "Obra Social", "Fecha Nac."];
$('#filtroNacimiento').datepicker();
$('#txtNacimiento').datepicker();


function listadoPacientes(arrayHeader, data) {
    let contenido = "";
    contenido += "<table id='tabla-generic' class='table table-oscura table-striped table-bordered table-hover'>";
    contenido += "<thead>";
    contenido += "<tr class='fw-bold'>";
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
        let nombre = data[i].nombre ?? "-";
        let apellido = data[i].apellido ?? "-";
        let direccion = data[i].direccion ?? "-";
        let localidad = data[i].localidad ?? "-";
        let telefono = data[i].telefono ?? "-";
        let nombreOS = data[i].nombreOS ?? "-";
        let fecha_nac = data[i].fecha_nac ?? "-";
        if (fecha_nac != "-") {
            fecha_nac = formatDate(fecha_nac);
        }
        contenido += "<tr>";
        contenido += "<td>" + nombre + "</td>";
        contenido += "<td>" + apellido + "</td>";
        contenido += "<td>" + direccion + "</td>";
        contenido += "<td>" + localidad + "</td>";
        contenido += "<td>" + telefono + "</td>";
        contenido += "<td>" + nombreOS + "</td>";
        contenido += "<td>" + fecha_nac + "</td>";
        contenido += "<td class='d-flex justify-content-center'>";
        contenido += "<button class='btn btn-outline-success me-2 py-1' onclick='modalEdit(" + data[i]['id'] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button class='btn btn-outline-danger mx-2' onclick='modalDelete(" + data[i]['id'] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-trash3'></i></button>";
        contenido += "<button class='btn btn-outline-light ms-2' onclick='selectPaciente(" + data[i]['id'] + ")'>Seleccionar</button>";
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

function formatDate(fecha) {
    var datePart = fecha.match(/\d+/g),
        year = datePart[0],
        month = datePart[1], day = datePart[2];
    return day + '/' + month + '/' + year;
}

function llenarCombo(id) {
    $.get("../ObrasSociales/getAll", function (data) {
        let control = $("#" + id);
        let contenido = "";
        contenido += "<option value='0' disabled selected >--Seleccione una opción--</option>";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].id + "'>";
            contenido += data[i].nombre;
            contenido += "</option>";
        }
        control.html(contenido);
    });
}

jQuery('#buscar-filtro').on('click', function () {
    $('#tabla-clase').removeClass('ocultar');
    let nombre = $("#filtroNombre").val();
    let apellido = $("#filtroApellido").val();
    let nacimiento = $("#filtroNacimiento").val();
    let os = $("#filtroOS").val() ?? 0;
    $.get("../Pacientes/filtrarPacientes/?nombre=" + nombre + "&apellido=" + apellido + "&nacimiento=" + nacimiento + "&os=" + os, function (data) {
        listadoPacientes(header, data);
    });
});

jQuery('#limpia-filtro').on('click', function () {
    $("#filtroNombre").val("");
    $('#filtroApellido').val("");
    $('#filtroNacimiento').val("");
    $('#filtroOS').prop('selectedIndex', 0);
    $('#tabla-clase').addClass('ocultar');
});

jQuery('#btnAgregar').on('click', function () {
    limpiarCampos();
    habilitarCampos();
    llenarCombo("comboOS");
    $("#staticBackdropLabel").text("Agregar paciente");
});

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar paciente");
    limpiarCampos();
    habilitarCampos();
    llenarCombo("comboOS");
    $.get("../Pacientes/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#txtApellido").val(data[0]['apellido']);
        $("#txtTelefono").val(data[0]['telefono']);
        $("#txtDireccion").val(data[0]['direccion']);
        $("#txtLocalidad").val(data[0]['localidad']);
        let fecha_nac = '';
        if (data[0]['fecha_nac'] != null) {
            fecha_nac = formatDate(data[0]['fecha_nac'])
        }
        $("#txtNacimiento").val(fecha_nac);
        $("#comboOS").val(data[0]['obra_social_id']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar paciente");
    limpiarCampos();
    deshabilitarCampos();
    llenarCombo("comboOS");
    $("#btnAceptar").addClass("eliminar");
    $.get("../Pacientes/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#txtApellido").val(data[0]['apellido']);
        $("#txtTelefono").val(data[0]['telefono']);
        $("#txtDireccion").val(data[0]['direccion']);
        $("#txtLocalidad").val(data[0]['localidad']);
        let fecha_nac = '';
        if (data[0]['fecha_nac'] != null) {
            fecha_nac = formatDate(data[0]['fecha_nac'])
        }
        $("#txtNacimiento").val(fecha_nac);
        $("#comboOS").val(data[0]['obra_social_id']);
    });
    let frm = new FormData();
    frm.append("id", id);
}

function selectPaciente(id) {
    window.location.href = "Pacientes/DatosPaciente?id=" + id;
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
        let direccion = $("#txtDireccion").val();
        let localidad = $("#txtLocalidad").val();
        let fecha_nac = $("#txtNacimiento").val();
        let obraSocial = $("#comboOS").val();
        frm.append("id", id);
        frm.append("nombre", nombre);
        frm.append("apellido", apellido);
        frm.append("telefono", telefono);
        frm.append("direccion", direccion);
        frm.append("localidad", localidad);
        frm.append("fecha_nacimiento", fecha_nac);
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
                if ($("#btnAceptar").hasClass("eliminar")) {
                    alert("El paciente se eliminó correctamente");
                } else {
                    alert("El paciente se guardó correctamente");
                }
                $('#buscar-filtro').click();
                $("#btnCancelar").click();
            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
        }
    });
}