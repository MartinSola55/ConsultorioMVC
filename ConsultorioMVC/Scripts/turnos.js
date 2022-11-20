const header = ["Nombre", "Apellido", "Obra Social", "Teléfono", "Hora", "Disponible"];
const horas = ["09:00", "09:20", "09:40", "10:00", "10:20", "10:40", "11:00", "11:20", "11:40", "12:00", "12:20", "12:40", "13:00"];
var hoy = new Date();
let dd = String(hoy.getDate()).padStart(2, '0');
let mm = String(hoy.getMonth() + 1).padStart(2, '0');
let yyyy = hoy.getFullYear();
hoy = dd + '/' + mm + '/' + yyyy;
$("#datepicker").removeAttr("data-val-date");

listarInicial(hoy);

function listarInicial(dia) {
    $.get("../Turnos/getAll/?dia=" + dia, function (data) {
        listadoTurnos(header, data);
    });
    llenarComboOS();
}

moment.locale('es');
$(function () {
    $("#datepicker").daterangepicker({
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

jQuery('#datepicker').on('change', function () {
    let dia = $("#datepicker").val();
    $.get("../Turnos/getAll/?dia=" + dia, function (data) {
        listadoTurnos(header, data);
    });
});

function listadoTurnos(arrayHeader, data) {
    let contenido = "";
    contenido += "<table id='tabla-generic' class='container table table-oscura table-striped table-bordered table-hover'>";
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
    for (let p = 0; p < horas.length; p++) {
    contenido += "<tr class='align-items-center'>";
        let k = 0;
        do {
            if (data.length != 0) {
                for (let i = 0; i < data.length; i++) {
                    if (data[i].DiaHorario.Horario.HoraString == horas[p]) {
                        contenido += "<td style='vertical-align: middle'>" + data[i].Persona.Nombre + "</td>";
                        contenido += "<td style='vertical-align: middle'>" + data[i].Persona.Apellido + "</td>";
                        contenido += "<td style='vertical-align: middle'>" + data[i].Persona.ObraSocial.Nombre + "</td>";
                        contenido += "<td style='vertical-align: middle'>" + data[i].Persona.Telefono + "</td>";
                        contenido += "<td style='vertical-align: middle' class='text-center'>" + horas[p] + "</td>";
                        if (data[i].DiaHorario.Disponible == true) {
                            contenido += "<td style='vertical-align: middle' class='text-center'>Sí</td>";
                            contenido += "<td style='vertical-align: middle' class='d-flex justify-content-center'>";
                            contenido += "<button class='btn btn-outline-danger' onclick='turnoDelete(" + data[i].DiaHorario.ID + ")'><i class='bi bi-trash3'></i></button>";
                            contenido += "</td>";
                        }
                        else {
                            contenido += "<td style='vertical-align: middle' class='text-center'>No</td>";
                            contenido += "<td style='vertical-align: middle' class='d-flex justify-content-center'>";
                            contenido += "<button class='btn btn-outline-success me-4' onclick='modalEdit(" + data[i].ID + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-pencil-square'></i></button>";
                            contenido += "<button class='btn btn-outline-danger ms-4' onclick='modalDelete(" + data[i].ID + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-trash3'></i></button>";
                            contenido += "</td>";
                        }
                        contenido += "</tr>";
                        break;
                    } else if (i == data.length - 1) {
                        contenido += "<td></td>";
                        contenido += "<td></td>";
                        contenido += "<td></td>";
                        contenido += "<td></td>";
                        contenido += "<td style='vertical-align: middle' class='text-center'>" + horas[p] + "</td>";
                        contenido += "<td style='vertical-align: middle' class='text-center'>No</td>";
                        contenido += "<td></td>";
                        contenido += "</tr>";
                    }
                }
            } else {
                contenido += "<td></td>";
                contenido += "<td></td>";
                contenido += "<td></td>";
                contenido += "<td></td>";
                contenido += "<td style='vertical-align: middle' class='text-center'>" + horas[p] + "</td>";
                contenido += "<td style='vertical-align: middle' class='text-center'>No</td>";
                contenido += "<td></td>";
                contenido += "</tr>";
            }
        } while (k == horas.length-1)
    }
    contenido += "</tbody>";
    contenido += "</table>";
    $("#tabla-clase").html(contenido);
}

jQuery('#btnAgregar').on('click', function () {
    $("#staticBackdropLabel").text("Agregar turno");
    limpiarCampos();
    habilitarCampos();
    llenarComboH(0);
    $("#comboOS").val("0");
    $("#txtID").prop("disabled", "disabled");
});

function llenarComboOS() {
    $.get("../ObrasSociales/getHabilitadas", function (data) {
        let control = $("#comboOS");
        let contenido = "";
        contenido += "<option value='0' disabled selected >--Seleccione una obra social--</option>";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].ID + "'>";
            contenido += data[i].Nombre;
            contenido += "</option>";
        }
        control.html(contenido);
    });
}

function llenarComboH(hora) {
    let dia = $("#datepicker").val();
    $.get("../Turnos/getHoras/?dia=" + dia + "&hora=" + hora, function (data) {
        let control = $("#comboHoras");
        let contenido = "";
        contenido += "<option value='' disabled selected >--Seleccione una hora--</option>";
        for (let i = 0; i < data.length; i++) {
            contenido += "<option value='" + data[i].idHora + "'>";
            contenido += data[i].hora;
            contenido += "</option>";
        }
        control.html(contenido);
        $("#comboHoras option[value = " + hora + "]").attr('selected', 'selected');
    });
}

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar turno");
    limpiarCampos();
    habilitarCampos();
    $.get("../Turnos/getOne/?id=" + id, function (data) {
        llenarComboH(data[0]['idHorario']);
        $("#IDPersona").val(data[0]['idPersona']);
        $("#IDDiaHorario").val(data[0]['idDiaHorario']);
        $("#DiaHorario").val($("#datepicker").val());
        $("#txtID").val(data[0]['idTurno']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#txtApellido").val(data[0]['apellido']);
        $("#comboOS").val(data[0]['idOS']);
        $("#txtTelefono").val(data[0]['telefono']);
        $("#txtCorreo").val(data[0]['correo']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar turno");
    limpiarCampos();
    deshabilitarCampos();
    $("#btnAceptar").addClass("eliminar");
    $.get("../Turnos/getOne/?id=" + id, function (data) {
        llenarComboH(data[0]['idHorario']);
        $("#IDPersona").val(data[0]['idPersona']);
        $("#IDDiaHorario").val(data[0]['idDiaHorario']);
        $("#DiaHorario").val($("#datepicker").val());
        $("#txtID").val(data[0]['idTurno']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#txtApellido").val(data[0]['apellido']);
        $("#comboOS").val(data[0]['idOS']);
        $("#txtTelefono").val(data[0]['telefono']);
        $("#txtCorreo").val(data[0]['correo']);
    });
}

function turnoDelete(id) {
    $.get("../DiasHorarios/getOne/?id=" + id, function (data) {
        if (confirm("¿Seguro desea eliminar el horario?\n\nDía: " + data[0].dia + "\nHora: " + data[0].hora)) {
            let frm = new FormData();
            let id = data[0].id;
            frm.append("id", id);
            deleteDH(frm, data[0].dia);
        }
    });
}

function limpiarCampos() {
    $(".limpiarCampo").val("");
    $("#comboHoras").prop("selectedIndex", 1);
    $("#btnAceptar").removeClass("eliminar");
}

function habilitarCampos() {
    $(".habilitarCampo").removeAttr("disabled");
}

function deshabilitarCampos() {
    $(".deshabilitarCampo").attr("disabled", "disabled");
}

$('#formTurno').on('submit', function (e) {
    e.preventDefault();
    if ($("#txtNombre").hasClass("valid") &&
        $("#txtApellido").hasClass("valid") &&
        $("#comboOS").hasClass("valid") &&
        $("#txtTelefono").hasClass("valid") &&
        $("#txtCorreo").hasClass("valid") &&
        $("#comboHoras").hasClass("valid"))
    {
        confirmarCambios();
    }
});

function confirmarCambios() {
    let frm = new FormData();
    let id = $("#txtID").val();
    let idPersona = $("#IDPersona").val();
    let idDiaHorario = $("#IDDiaHorario").val();
    let nombre = $("#txtNombre").val();
    let apellido = $("#txtApellido").val();
    let obraSocial = $("#comboOS").val();
    let telefono = $("#txtTelefono").val();
    let correo = $("#txtCorreo").val();
    let hora = $("#comboHoras").val();
    let dia = $("#datepicker").val();
    frm.append("id", id);
    frm.append("Persona.ID", idPersona);
    frm.append("Persona.Nombre", nombre);
    frm.append("Persona.Apellido", apellido);
    frm.append("Persona.ObraSocial.ID", obraSocial);
    frm.append("Persona.Telefono", telefono);
    frm.append("Persona.Correo", correo);
    frm.append("DiaHorario.ID", idDiaHorario);
    frm.append("DiaHorario.Horario.ID", hora);
    frm.append("DiaHorario.Dia", dia);
    if ($("#btnAceptar").hasClass("eliminar")) {
        if (confirm("¿Seguro que desea eliminar el turno?") == 1) {
            crudTurno(frm, "Delete", dia);
        }
    } else {
        crudTurno(frm, "Save", dia);
    }
}

function crudTurno(frm, action, dia) {
    $.ajax({
        type: "POST",
        url: "../Turnos/" + action,
        data: frm,
        contentType: false,
        processData: false,
        //dataType: 'json',
        success: function (data) {
            console.log(data);
            if (data != 0) {
                if ($("#btnAceptar").hasClass("eliminar")) {
                    alert("El turno se eliminó correctamente");
                }

            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
            $("#btnCancelar").click();
            listarInicial(dia);
        }
    });
}


function deleteDH(frm, dia) {
    $.ajax({
        type: "POST",
        url: "../DiasHorarios/delete",
        data: frm,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (data) {
            if (data != 0) {
                if ($("#btnAceptar").hasClass("eliminar")) {
                    alert("El horario se eliminó correctamente");
                }
                $("#btnCancelar").click();

            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
            listarInicial(dia);
        }
    });
}