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

const formulario = $("#formTurno");
const inputs = $("#formTurno input[type!=button], select").toArray();

const expresiones = {
    nombre: /^[a-zA-ZÀ-ÿ\s]{1,40}$/, // Letras y espacios, pueden llevar acentos.
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
            if (validaCampos(expresiones.nombre, e.target, 'Apellido')) {
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
        $(`#container${campo} i`).removeClass("bi-x-circle-fill");
        $(`#container${campo} i`).addClass("bi-check-circle-fill");
        $(`#container${campo} .informaError`).css("display", "none");
        return true;
    }
    if (expresion.test(input.value)) {
        $(`#container${campo}`).removeClass("formContainer-incorrecto");
        $(`#container${campo}`).addClass("formContainer-correcto");
        $(`#container${campo} i`).removeClass("bi-x-circle-fill");
        $(`#container${campo} i`).addClass("bi-check-circle-fill");
        $(`#container${campo} .informaError`).css("display", "none");
        return true;
    } else {
        $(`#container${campo}`).removeClass("formContainer-correcto");
        $(`#container${campo}`).addClass("formContainer-incorrecto");
        $(`#container${campo} i`).removeClass("bi-check-circle-fill");
        $(`#container${campo} i`).addClass("bi-x-circle-fill");
        $(`#container${campo} .informaError`).css("display", "block");
        return false;
    }
};

inputs.forEach((input) => {
    input.addEventListener('keyup', validaForm );
    input.addEventListener('blur', validaForm );
    input.addEventListener('change', validaForm );
});

$("#comboOS").on('change', function () {
    campos['os'] = $("#comboOS").val() == "0" ? false : true;
});

$("#comboHoras").on('change', function () {
    campos['hora'] = $("#comboHoras").val() == "0" ? false : true;
});


function campoRequired() {
    if (campos.nombre && campos.apellido && campos.telefono && campos.os && campos.fecha && campos.hora && campos.email) {
        $("#errorMessage").removeClass("errorMessage-activo");
        return true;
    }
    $("#errorMessage").addClass("errorMessage-activo");
    return false;
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
                alert("El turno no se pudo asignar. Hubo un error en la base de datos");
            }
        }
    });
}