let minDia = $('#datepicker').attr('min');
let maxDia = $('#datepicker').attr('max');
let hoy = new Date();
moment.locale('es');
llenarComboOS();

$(function () {
    $('#datepicker').daterangepicker({
        "autoApply": true,
        "locale": {
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "fromLabel": "Hasta",
            "toLabel": "Desde",
        },
        singleDatePicker: true,
        minDate: minDia,
        maxDate: maxDia,
        opens: 'right',
        isInvalidDate: function (date) {
            if (date.day() == 1 || date.day() == 2 || date.day() == 3 || date.day() == 5)
                return false;
            return true;
        }
    })
});

jQuery('#datepicker').on('change', function () {
    $("#comboHoras").html("<option value='0' disabled selected>--Seleccione una hora--</option>");
    let dia = $("#datepicker").val();
    llenarComboH(dia);
});

function llenarComboOS() {
    $.get("../Main/getOSParticular", function (data) {
        let control = $("#comboOS");
        let contenido = "";
        contenido += "<option value='0' disabled selected >--Seleccione una obra social--</option>";
        contenido += "<option value='" + data[0]['id'] + "'>";
        contenido += data[0]['nombre'];
        contenido += "</option>";
        control.html(contenido);
    });
    $.get("../Main/getOSHabilitadas", function (data) {
        let control = $("#comboOS");
        let contenido = "";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].id + "'>";
            contenido += data[i].nombre;
            contenido += "</option>";
        }
        control.append(contenido);
    });
}

function llenarComboH() {
    let dia = $("#datepicker").val();
    $.get("../Main/getHoras/?dia=" + dia, function (data) {
        let control = $("#comboHoras");
        let contenido = "";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].idHora + "'>";
            contenido += data[i].hora;
            contenido += "</option>";
        }
        control.append(contenido);
    });
}

function campoRequired() {
    campos = $("input[required]");
    for (let i = 0; i < campos.length; i++) {
        if (campos[i].value == "") {
            alert("Por favor, completa todos los campos requeridos")
            return false;
        }
    }
    return true;
}

function confirmarCambios() {
    if (campoRequired()) {
        let frm = new FormData();
        let nombre = $("#txtNombre").val();
        let apellido = $("#txtApellido").val();
        let telefono = $("#txtTelefono").val();
        let obraSocial = $("#comboOS").val();
        let dia = $("#datepicker").val();
        let hora = $("#comboHoras").val();
        let correo = $("#txtEmail").val();
        frm.append("Persona.nombre", nombre);
        frm.append("Persona.apellido", apellido);
        frm.append("Persona.telefono", telefono);
        frm.append("Persona.obra_social_id", obraSocial);
        frm.append("DiaHorario.dia", dia);
        frm.append("DiaHorario.horario_id", hora);
        frm.append("Persona.correo", correo);
        saveTurno(frm);
    }
}

function saveTurno(frm) {
    $.ajax({
        type: "POST",
        url: "../Main/saveTurno",
        data: frm,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (data) {
            if (data == 1) {
                alert("El turno se guardó correctamente");
            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
        }
    });
}