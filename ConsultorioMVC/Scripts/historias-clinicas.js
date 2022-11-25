var idPaciente = window.location.pathname.split("/").pop();
listarHC();
$("#txtFecha").removeAttr("data-val-date");
var hoy = new Date();
let dd = String(hoy.getDate()).padStart(2, '0');
let mm = String(hoy.getMonth() + 1).padStart(2, '0');
let yyyy = hoy.getFullYear();
hoy = dd + '/' + mm + '/' + yyyy;

function formatDate(fecha) {
    var datePart = fecha.match(/\d+/g),
        year = datePart[0],
        month = datePart[1], day = datePart[2];
    return day + '/' + month + '/' + year;
}

$(document).ready(function () {
    if ($("#txtNotification").html() !== "") {
        $("#btnModal").click();
        setTimeout(function () {
            $("#btnCerrar").click();
        }, 4000)
    }
});

moment.locale('es');
$(function () {
    $("#txtFecha").daterangepicker({
        "autoApply": true,
        "locale": {
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "fromLabel": "Hasta",
            "toLabel": "Desde",
        },
        singleDatePicker: true,
        opens: 'right',
        autoUpdateInput: true,
        autoApply: true
    })
});

$.get("/Pacientes/getOne/?id=" + idPaciente, function (data) {
    $("#txtNombre").val(data[0]['nombre']);
    $("#txtApellido").val(data[0]['apellido']);
    $("#txtTelefono").val(data[0]['telefono']);
    $("#txtDireccion").val(data[0]['direccion']);
    $("#txtLocalidad").val(data[0]['localidad']);
    let fecha_nac = '';
    if (data[0]['fecha_nac'] != null) {
        fecha_nac = formatDate(data[0]['fecha_nac']);
        $("#txtNacimiento").val(fecha_nac + " - " + calculateAge(fecha_nac) + " años");
    }
    $("#txtOS").val(data[0]['nombreOS']);
});

function calculateAge(birthday) { // birthday is a date
    var dateParts = birthday.split("/");
    // month is 0-based, that's why we need dataParts[1] - 1
    var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
    var ageDifMs = Date.now() - dateObject;
    var ageDate = new Date(ageDifMs); // miliseconds from epoch
    return Math.abs(ageDate.getUTCFullYear() - 1970);
}

function listarHC() {
    $.get("/Pacientes/getHistoriasClinicas/?id=" + idPaciente, function (data) {
        let contenedor = $('#tarjetasHC');
        let tarjeta = "";
        for (let i = 0; i < data.length; i++) {
            tarjeta += "<div class='card me-4 col-3 mb-4' style='width: 18rem;'>";
            tarjeta += "<div class='card-body d-flex flex-column'>"
            tarjeta += "<h5 class='card-title'>Fecha: " + formatDate(data[i].fecha) + "</h5>"
            tarjeta += "<p class='card-text'>" + data[i].descripcion + "</p>"
            tarjeta += "<div class='d-flex justify-content-between mt-auto'>"
            tarjeta += "<button class='btn btn-outline-dark' onclick='modalEdit(" + data[i].idHC + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop' >Ver</button>"
            tarjeta += "<button class='btn btn-danger' onclick='modalDelete(" + data[i].idHC + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop' >Eliminar</button>"
            tarjeta += "</div>"
            tarjeta += "</div>"
            tarjeta += "</div>"
            tarjeta += "<br/>"
            contenedor.html(tarjeta);
        }
    });
}

jQuery('#btnAgregar').on('click', function () {
    limpiarCampos();
    habilitarCampos();
    $("#staticBackdropLabel").text("Agregar historia clínica");
    $('#txtFecha').val(hoy);
    $('#txtID').prop("disabled", "disabled");
});

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar historia clínica");
    limpiarCampos();
    habilitarCampos();
    $("#txtFecha").addClass("valid");
    $("#txtDescripcion").addClass("valid");
    $.get("/Pacientes/getHC/?id=" + id, function (data) {
        $("#txtID").val(data[0]['idHC']);
        $("#txtFecha").val(formatDate(data[0]['fecha']));
        $("#txtDescripcion").val(data[0]['descripcion']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar historia clínica");
    limpiarCampos();
    deshabilitarCampos();
    $("#btnAceptar").addClass("eliminar");
    $("#txtFecha").addClass("valid");
    $("#txtDescripcion").addClass("valid");
    $.get("/Pacientes/getHC/?id=" + id, function (data) {
        $("#txtID").val(data[0]['idHC']);
        $("#txtFecha").val(formatDate(data[0]['fecha']));
        $("#txtDescripcion").val(data[0]['descripcion']);
    });
    let frm = new FormData();
    frm.append("id", id);
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
    $("#txtPacienteID").val(idPaciente);
    if ($("#txtFecha").hasClass("valid") && $("#txtDescripcion").hasClass("valid")) {
        confirmarCambios();
    } else {
        $("#formHC").attr("action", "/Pacientes/SaveHC");
        $("#formHC").submit();
    }
});

function confirmarCambios() {
    if ($("#btnAceptar").hasClass("eliminar")) {
        if (confirm("¿Seguro que desea eliminar la historia clínica?") == 1) {
            $("#formHC").attr("action", "/Pacientes/DeleteHC");
            $("#formHC").submit();
        }
    } else {
        $("#formHC").attr("action", "/Pacientes/SaveHC");
        $("#formHC").submit();
    }
}