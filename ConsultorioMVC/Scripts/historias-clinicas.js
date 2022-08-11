$.urlParam = function (name) {
	var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
	return results[1] || 0;
}

listarHC();
$('#txtFecha').datepicker();

function formatDate(fecha) {
    var datePart = fecha.match(/\d+/g),
        year = datePart[0],
        month = datePart[1], day = datePart[2];
    return day + '/' + month + '/' + year;
}

$.get("../Pacientes/getOne/?id=" + $.urlParam('id'), function (data) {
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
    $("#txtOS").val(data[0]['nombreOS']);
});

function listarHC() {
    $.get("../Pacientes/getHistoriasClinicas/?id=" + $.urlParam('id'), function (data) {
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
    $('#txtFecha').datepicker('setDate', 'today');
});

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar historia clínica");
    limpiarCampos();
    habilitarCampos();
    $.get("../Pacientes/getHC/?id=" + id, function (data) {
        $("#txtID").val(data[0]['idHC']);
        $("#txtFecha").val(data[0]['fecha']);
        $("#txtDescripcion").val(data[0]['descripcion']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar historia clínica");
    limpiarCampos();
    deshabilitarCampos();
    $("#btnAceptar").addClass("eliminar");
    $.get("../Pacientes/getHC/?id=" + id, function (data) {
        $("#txtID").val(data[0]['idHC']);
        $("#txtFecha").val(data[0]['fecha']);
        $("#txtDescripcion").val(data[0]['descripcion']);
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

function confirmarCambios() {
    if (campoRequired()) {
        let frm = new FormData();
        let id = $("#txtID").val();
        let fecha = $("#txtFecha").val();
        let descripcion = $("#txtDescripcion").val();
        let paciente_id = $.urlParam('id');
        frm.append("id", id);
        frm.append("fecha", fecha);
        frm.append("descripcion", descripcion);
        frm.append("paciente_id", paciente_id);
        if ($("#btnAceptar").hasClass("eliminar")) {
            if (confirm("¿Seguro que desea eliminar la historia clínica?") == 1) {
                crudHC(frm, "deleteHC");
            }
        } else {
            crudHC(frm, "saveHC");
        }
    }
}

function crudHC(frm, action) {
    $.ajax({
        type: "POST",
        url: "../Pacientes/" + action,
        data: frm,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data != 0) {
                if ($("#btnAceptar").hasClass("eliminar")) {
                    alert("La historia clínica se eliminó correctamente");
                }
                listarHC();
                $("#btnCancelar").click();
            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
        }
    });
}