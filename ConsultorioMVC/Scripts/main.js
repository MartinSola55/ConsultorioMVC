let minDia = $('#datepicker').attr('min');
let maxDia = $('#datepicker').attr('max');
let hoy = new Date();
moment.locale('es');
$("#datepicker").removeAttr("data-val-date");
limpiarCampos();

$(document).ready(function () {
    if ($("#txtNotification").html() !== "") {
        $("#btnModal").click();
        setTimeout(function () {
            $("#btnCerrar").click();
        }, 8000)
    }
});

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
        autoUpdateInput: true,
        autoApply: true,
        isInvalidDate: function (date) {
            if (date.day() == 1 || date.day() == 2 || date.day() == 3 || date.day() == 5)
                return false;
            return true;
        }
    })
});

jQuery('#datepicker').on('change', function () {
    $("#comboHoras").html("<option value='0' disabled selected>--Seleccione una hora--</option>");
    llenarComboH();
    if (expresiones.fecha.test($('#datepicker').val())) {
        $("#containerFecha").removeClass("formContainer-incorrecto");
        $("#containerFecha").addClass("formContainer-correcto");
        $("#containerFecha i").removeClass("bi-x-circle-fill");
        $("#containerFecha i").addClass("bi-check-circle-fill");
        campos['fecha'] = true;
    } else {
        $("#containerFecha").removeClass("formContainer-correcto");
        $("#containerFecha").addClass("formContainer-incorrecto");
        $("#containerFecha i").removeClass("bi-check-circle-fill");
        $("#containerFecha i").addClass("bi-x-circle-fill");
        campos['fecha'] = false;
    }
});

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

const formulario = $("#formTurno");
const inputs = $("#formTurno input[type!=button], select").toArray();

const expresiones = {
    nombre: /^[a-zA-ZÀ-ÿ\s]{1,40}$/, // Letras y espacios, pueden llevar acentos.
    apellido: /^[a-zA-ZÀ-ÿ\s']{1,40}$/, // Letras y espacios, pueden llevar acentos.
    email: /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$/,
    telefono: /^\d{7,14}$/, // 7 a 14 numeros.
    numero: /^[0-9]{1,2}$/, // Numeros.
    fecha: /^[0-9]{2}[/][0-9]{2}[/][0-9]{4}$/, // fecha con formato dd/mm/aaaa.
}

const campos = {
    nombre: false,
    apellido: false,
    telefono: false,
    os: false,
    fecha: false,
    hora: false,
    email: true
}

const validaForm = (e) => {
    switch (e.target.id) {
        case "txtNombre": {
            if (validaCampos(expresiones.nombre, e.target, 'Nombre')) {
                campos['nombre'] = true;
            } else {
                campos['nombre'] = false;
            };
            break;
        }
        case "txtApellido": {
            if (validaCampos(expresiones.apellido, e.target, 'Apellido')) {
                campos['apellido'] = true;
            } else {
                campos['apellido'] = false;
            };
            break;
        }
        case "txtTelefono": {
            if (validaCampos(expresiones.telefono, e.target, 'Telefono')) {
                campos['telefono'] = true;
            } else {
                campos['telefono'] = false;
            };
            break;
        }
        case "datepicker": {
            if (validaCampos(expresiones.fecha, e.target, 'Fecha')) {
                campos['fecha'] = true;
            } else {
                campos['fecha'] = false;
            };
            break;
        }
        case "txtEmail": {
            if (validaCampos(expresiones.email, e.target, 'Email')) {
                campos['email'] = true;
            } else {
                campos['email'] = false;
            };
            break;
        }
    }
}

let validaCampos = (expresion, input, campo) => {
    if (campo == "Email" && $("#txtEmail").val() == "") {
        $(`#container${campo}`).removeClass("formContainer-incorrecto");
        $(`#container${campo}`).removeClass("formContainer-correcto");
        $(`#container${campo} .informaError`).css("display", "none");
        return true;
    }
    if (input.value === "") {
        $(`#container${campo}`).removeClass("formContainer-incorrecto");
        $(`#container${campo}`).removeClass("formContainer-correcto");
        return false;
    }
    if (expresion.test(input.value)) {
        $(`#container${campo}`).removeClass("formContainer-incorrecto");
        $(`#container${campo}`).addClass("formContainer-correcto");
        $(`#container${campo} i`).removeClass("bi-x-circle-fill");
        $(`#container${campo} i`).addClass("bi-check-circle-fill");
        return true;
    } else {
        $(`#container${campo}`).removeClass("formContainer-correcto");
        $(`#container${campo}`).addClass("formContainer-incorrecto");
        $(`#container${campo} i`).removeClass("bi-check-circle-fill");
        $(`#container${campo} i`).addClass("bi-x-circle-fill");
    }
    return false;
};

inputs.forEach((input) => {
    input.addEventListener('keyup', validaForm);
    input.addEventListener('change', validaForm);
});

$("#comboOS").on('change', function () {
    campos['os'] = $("#comboOS").val() == "" ? false : true;
});

$("#comboHoras").on('change', function () {
    campos['hora'] = $("#comboHoras").val() == "" ? false : true;
});

$("#btnEnviar").on("click", function () {
    if (campos.nombre && campos.apellido && campos.telefono && campos.os && campos.fecha && campos.hora && campos.email) {
    } else {
        $("#errorMessage").show();
        setTimeout(function () {
            $("#errorMessage").fadeOut(1500)
        }, 4000);
    }
});

function limpiarCampos() {
    $("#txtNombre").val("");
    $("#txtApellido").val("");
    $("#txtTelefono").val("");
    $("#comboOS").val("");
    $("#comboHoras").val("");
    $("#txtEmail").val("");
}