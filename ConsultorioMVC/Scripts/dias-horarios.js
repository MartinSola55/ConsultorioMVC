let header = ["Horario", "Atiende"];
let horas = ["09:00", "09:20", "09:40", "10:00", "10:20", "10:40", "11:00", "11:20", "11:40", "12:00", "12:20", "12:40", "13:00"];

moment.locale('es');
$(function () {
    $('#rangoFechas').daterangepicker({
        "autoApply": true,
        "locale": {
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "fromLabel": "Hasta",
            "toLabel": "Desde",
        },
        opens: 'right',
        isInvalidDate: function (date) {
            if (date.day() == 1 || date.day() == 2 || date.day() == 3 || date.day() == 5)
                return false;
            return true;
        }
    },
    function (start, end) {
        startDate = start;
        endDate = end;
    })
});

listadoDH(header);
limpiarCampos();

function listadoDH(arrayHeader) {
    let contenido = "";
    contenido += "<table class='container table table-oscura table-striped table-bordered table-hover'>";
    contenido += "<thead>";
    contenido += "<tr class='fw-bold'>";
    for (let i = 0; i < arrayHeader.length; i++) {
        contenido += "<td class='text-center'>";
        contenido += arrayHeader[i];
        contenido += "</td>";
    }
    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";
    for (let i = 0; i < 13; i++) {
        contenido += "<tr class='text-center'>";
        contenido += "<td>" + horas[i] + "</td>";
        contenido += "<td>"
        contenido += "<div class='d-flex justify-content-center'>"
        contenido += "<input value='" + (i+1) + "' type='checkbox' name='horas[]' class='form-check' style='transform:scale(1.5)' />";
        contenido += "</div>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    $("#tabla-dh").html(contenido);
}

function selectAll(source) {
    checkboxes = $('input[name="horas[]"]');
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        checkboxes[i].checked = source.checked;
    }
}

function limpiarCampos() {
    $('input[type="checkbox"]').prop('checked', false);
    $('#datepicker').val("");
}

var daylist;
$('#rangoFechas').on('apply.daterangepicker', function (ev, picker) {
    let desde = picker.startDate.format('YYYY-MM-DD');
    let hasta = picker.endDate.format('YYYY-MM-DD');
    daylist = getDaysArray(new Date(desde), new Date(hasta));
    daylist = daylist.map((v) => v.toLocaleDateString());
});

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
        let dias = daylist;
        let selectedHoras = $.map($('input[name="horas[]"]:checked'), function (c) { return c.value; })
        frm.append("dias", dias);
        frm.append("horas", selectedHoras);
        insertarDH(dias, selectedHoras);
    }
}

function insertarDH(dias, selectedHoras) {
    $.ajax({
        type: "POST",
        url: "../DiasHorarios/insert",
        data: { dias: dias, horas: selectedHoras },
        success: function (data) {
            if (data != 0) {
                limpiarCampos();
                alert("Se han guardado " + data + " horarios correctamente");
            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
        },
    });
}

var getDaysArray = function (start, end) {
    for (var arr = [], dt = new Date(start); dt <= new Date(end); dt.setDate(dt.getDate() + 1)) {
        var dia = new Date(dt);
        dia.setDate(dia.getDate() + 1);
        if (dia.getDay() == 1 || dia.getDay() == 2 || dia.getDay() == 3 || dia.getDay() == 5) {
            arr.push(dia);
        }
    }
    return arr;
};

