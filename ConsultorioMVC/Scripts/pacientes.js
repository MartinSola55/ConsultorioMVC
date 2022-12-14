llenarCombo();
const header = ["Nombre", "Apellido", "Dirección", "Localidad", "Teléfono", "Obra Social", "Fecha Nac."];
let maxDia = $('#txtNacimiento').attr('max');
moment.locale('es');
$("#txtNacimiento").removeAttr("data-val-date");

$(document).ready(function () {
    if ($("#txtNotification").html() !== "") {
        $("#btnModal").click();
        setTimeout(function () {
            $("#btnCerrar").click();
        }, 1500)
    }
});

$('#filtroNacimiento').daterangepicker({
    "locale": {
        "applyLabel": "Aplicar",
        "cancelLabel": "Cancelar",
        "fromLabel": "Hasta",
        "toLabel": "Desde",
    },
    singleDatePicker: true,
    opens: 'right',
    autoUpdateInput: false,
    autoApply: true,
});

$('#filtroNacimiento').on('apply.daterangepicker', function (ev, picker) {
    $('#filtroNacimiento').val(picker.startDate.format('DD/MM/YYYY'));
});

$('#txtNacimiento').daterangepicker({
    "locale": {
        "applyLabel": "Aplicar",
        "cancelLabel": "Cancelar",
        "fromLabel": "Hasta",
        "toLabel": "Desde",
    },
    singleDatePicker: true,
    maxDate: maxDia,
    minDate: "01/01/1900",
    opens: 'right',
    autoUpdateInput: true,
    autoApply: true,
});


function listadoPacientes(arrayHeader, data) {
    let contenido = "";
    contenido += "<table id='tabla-generic' class='container table table-light table-striped table-bordered table-hover'>";
    contenido += "<thead class='table-dark'>";
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
        contenido += "<button class='btn btn-success me-2 py-1' onclick='modalEdit(" + data[i]['id'] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button class='btn btn-danger mx-2' onclick='modalDelete(" + data[i]['id'] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-trash3'></i></button>";
        contenido += "<button class='btn btn-dark ms-2' onclick='selectPaciente(" + data[i]['id'] + ")'>Seleccionar</button>";
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

function llenarCombo() {
    let control = $("#filtroOS");
    $.get("../ObrasSociales/getHabilitadas", function (data) {
        let contenido = "";
        contenido += "<option value='0' disabled selected >--Seleccione una opción--</option>";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].ID + "'>";
            contenido += data[i].Nombre;
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
    $("#staticBackdropLabel").text("Agregar paciente");
    $('#txtID').prop("disabled", "disabled");
});

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar paciente");
    limpiarCampos();
    habilitarCampos();
    addValid();
    $.get("../Pacientes/getOne/?id=" + id, function (data) {
        console.log(data);
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
    addValid();
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
    window.location.href = "/Pacientes/DatosPaciente/" + id;
}

function addValid() {
    $("#txtNombre").addClass("valid");
    $("#txtApellido").addClass("valid");
    $("#txtTelefono").addClass("valid");
    $("#txtDireccion").addClass("valid");
    $("#txtLocalidad").addClass("valid");
    $("#txtNacimiento").addClass("valid");
    $("#comboOS").addClass("valid");
}

function limpiarCampos() {
    $(".limpiarCampo").val("");
    $("#btnAceptar").removeClass("eliminar");
}

function habilitarCampos() {
    $(".habilitarCampo").removeAttr("disabled");
}

function deshabilitarCampos() {
    $(".deshabilitarCampo").attr("disabled", "disabled");
}

$('#btnAceptar').on('click', function (e) {
    e.preventDefault();
    if ($("#txtNombre").hasClass("valid") &&
        $("#txtApellido").hasClass("valid") &&
        $("#txtTelefono").hasClass("valid") &&
        $("#txtDireccion").hasClass("valid") &&
        $("#txtLocalidad").hasClass("valid") &&
        $("#txtNacimiento").hasClass("valid") &&
        $("#comboOS").hasClass("valid")) {
        confirmarCambios();
    } else {
        $("#formPaciente").attr("action", "/Pacientes/Save");
        $("#formPaciente").submit();
    }
});

function confirmarCambios() {
    if ($("#btnAceptar").hasClass("eliminar")) {
        if (confirm("¿Seguro que desea eliminar el paciente?") == 1) {
            $("#formPaciente").attr("action", "/Pacientes/Delete");
            $("#formPaciente").submit();
        }
    } else {
        $("#formPaciente").attr("action", "/Pacientes/Save");
        $("#formPaciente").submit();
    }
}