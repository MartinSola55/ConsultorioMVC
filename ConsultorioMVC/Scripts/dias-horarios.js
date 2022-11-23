let horas = ["09:00", "09:20", "09:40", "10:00", "10:20", "10:40", "11:00", "11:20", "11:40", "12:00", "12:20", "12:40", "13:00"];
let hoy = new Date();
var daylist;

$(document).ready(function () {
    if ($("#txtNotification").html() !== "") {
        $("#btnModal").click();
        setTimeout(function () {
            $("#btnCerrar").click();
        }, 6000)
    }
});


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

$(document).ready(function () {
    daylist = hoy.toLocaleDateString();
});

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
        minDate: hoy,
        startDate: hoy,
        endDate: hoy,
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

limpiarCampos();

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

$('#rangoFechas').on('apply.daterangepicker', function (ev, picker) {
    let desde = picker.startDate.format('YYYY-MM-DD');
    let hasta = picker.endDate.format('YYYY-MM-DD');
    daylist = getDaysArray(new Date(desde), new Date(hasta));
    daylist = daylist.map((v) => v.toLocaleDateString());
});

function campoRequired() {
    campos = $(".required");
    for (let i = 0; i < campos.length; i++) {
        if (campos[i].value == "") {
            $("#campo" + i).addClass("error");
            return false;
        } else {
            $("#campo" + i).removeClass("error");
        }
    }
    let selectedHoras = $.map($('input[name="horas[]"]:checked'), function (c) { return c.value; })
    if (selectedHoras.length == 0) {
        alert("Debes seleccionar al menos una hora");
        return false;
    }
    return true;
}

$('#btnAceptar').on('click', function (e) {
    e.preventDefault();
    if (campoRequired()) {
        let dias = daylist;
        let selectedHoras = $.map($('input[name="horas[]"]:checked'), function (c) { return c.value; })
        $("#arrayHorarios").val(selectedHoras);
        $("#arrayDias").val(dias);
        console.log(dias);
        console.log(selectedHoras);
        $('#formHorarios').submit();
    }
});

