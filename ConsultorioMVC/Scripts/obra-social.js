listar();
let header = ["Nombre", "Habilitada"];

$(document).ready(function () {
    if ($("#txtNotification").html() !== "") {
        $("#btnModal").click();
        setTimeout(function () {
            $("#btnCerrar").click();
        }, 4000)
    }
});

function listar() {
    $.get("../ObrasSociales/getAll", function (data) {
        listadoOS(header, data);
    });
}

function listadoOS(arrayHeader, data) {
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
        let keyID = Object.keys(data[0])[0];
        contenido += "<tr>";
        contenido += "<td>" + data[i].nombre + "</td>";
        let habilitado = data[i].habilitada == 1 ? "Sí" : "No";
        contenido += "<td class='text-center'>" + habilitado + "</td>";
        contenido += "<td class='d-flex justify-content-center'>";
        contenido += "<button class='btn btn-outline-success me-4' onclick='modalEdit(" + data[i][keyID] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button class='btn btn-outline-danger ms-4' onclick='modalDelete(" + data[i][keyID] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-trash3'></i></button>";
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
                emptyTable: "No existen obras sociales que coincidan con la búsqueda",
                info: "Mostrando _START_ a _END_ de _TOTAL_ obras sociales",
                infoEmpty: "Mostrando 0 obras sociales",
                lengthMenu: "Mostrar _MENU_ obras sociales",
            },
            columnDefs: [{
            orderable: false,
            targets: "no-sort"
        }]
        }
    )
    $("#tabla-generic").removeAttr("style");
}

jQuery('#filtroNombre').on('input', function () {
    let nombre = $("#filtroNombre").val();
    let habilitada = $("#filto-habilitado").val();
    if (habilitada == null) {
        $.get("../ObrasSociales/buscarObraSocialNombre/?nombre=" + nombre, function (data) {
            listadoOS(header, data);
        });
    } else if (habilitada == 0 || habilitada == 1) {
        $.get("../ObrasSociales/filtrarObrasSocialesHabilitadas/?nombre=" + nombre + "&habilitada=" + habilitada, function (data) {
            listadoOS(header, data);
        });
    }
});

jQuery('#filto-habilitado').on('change', function () {
    let nombre = $("#filtroNombre").val();
    let habilitada = $("#filto-habilitado").val();
    $.get("../ObrasSociales/filtrarObrasSocialesHabilitadas/?nombre=" + nombre + "&habilitada=" + habilitada, function (data) {
        listadoOS(header, data);
    });
});

jQuery('#limpia-filtro').on('click', function () {
    listar();
    $("#filtroNombre").val("");
    $('#filto-habilitado').prop('selectedIndex', 0);
});

jQuery('#btnAgregar').on('click', function () {
    limpiarCampos();
    habilitarCampos();
    $("#staticBackdropLabel").text("Agregar obra social");
    $("#checkHabilitada").prop('checked', false);
    $("#txtID").prop("disabled", "disabled");
});

function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar obra social");
    limpiarCampos();
    habilitarCampos();
    $("#txtNombre").addClass("valid");
    $.get("../ObrasSociales/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#checkHabilitada").prop('checked', data[0]['habilitada']);
        $("#checkHabilitada").attr('value', data[0]['habilitada']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar obra social");
    limpiarCampos();
    deshabilitarCampos();
    $("#txtNombre").addClass("valid");
    $("#btnAceptar").addClass("eliminar");
    $.get("../ObrasSociales/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtNombre").val(data[0]['nombre']);
        $("#checkHabilitada").prop('checked', data[0]['habilitada']);
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

function confirmarCambios() {
    if ($("#btnAceptar").hasClass("eliminar")) {
        if (confirm("¿Seguro que desea eliminar la obra social?") == 1) {
            $("#formOS").attr("action", "/ObrasSociales/Delete");
            $("#formOS").submit();
        }
    } else {
        $("#formOS").attr("action", "/ObrasSociales/Save");
        $("#formOS").submit();
    }
}

$("#checkHabilitada").on('change', function () {
    if ($(this).is(':checked')) {
        $(this).attr('value', 'true');
    } else {
        $(this).attr('value', 'false');
    }
});

$('#btnAceptar').on('click', function (e) {
    e.preventDefault();
    if ($("#txtNombre").hasClass("valid")) {
        confirmarCambios();
        return;
    } else {
        $("#formOS").attr("action", "/ObrasSociales/Save");
        $("#formOS").submit();
    }
});