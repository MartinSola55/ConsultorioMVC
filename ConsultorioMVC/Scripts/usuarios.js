listar();
let header = ["Email"];

function listar() {
    $.get("../Usuarios/getAll", function (data) {
        listadoUsuarios(header, data);
    });
}

function listadoUsuarios(arrayHeader, data) {
    let contenido = "";
    contenido += "<table id='tabla-generic' class='table table-oscura table-striped table-bordered table-hover'>";
    contenido += "<thead>";
    contenido += "<tr>";
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
        contenido += "<tr>";
        contenido += "<td>" + data[i].email + "</td>";
        contenido += "<td class='d-flex justify-content-center'>";
        contenido += "<button class='btn btn-outline-success me-4' onclick='modalEdit(" + data[i]["id"] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button class='btn btn-outline-danger ms-4' onclick='modalDelete(" + data[i]["id"] + ")' data-bs-toggle='modal' data-bs-target='#staticBackdrop'><i class='bi bi-trash3'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    $("#tabla-usuarios").html(contenido);
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
                emptyTable: "No existen usuarios que coincidan con la búsqueda",
                info: "Mostrando _START_ a _END_ de _TOTAL_ usuarios",
                infoEmpty: "Mostrando 0 usuarios",
                lengthMenu: "Mostrar _MENU_ usuarios",
            },
            columnDefs: [{
                orderable: false,
                targets: "no-sort"
            }]
        }
    )
    $("#tabla-generic").removeAttr("style");
}

jQuery('#filtroEmail').on('input', function () {
    let email = $("#filtroEmail").val();
    $.get("../Usuarios/filtraUsuarios/?email=" + email, function (data) {
            listadoUsuarios(header, data);
        });
});


function modalEdit(id) {
    $("#staticBackdropLabel").text("Editar usuario");
    limpiarCampos();
    habilitarCampos();
    $.get("../Usuarios/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtEmail").val(data[0]['email']);
        $("#txtPass").val(data[0]['password']);
    });
}

function modalDelete(id) {
    $("#staticBackdropLabel").text("Eliminar usuario");
    limpiarCampos();
    deshabilitarCampos();
    $("#btnAceptar").addClass("eliminar");
    $.get("../Usuarios/getOne/?id=" + id, function (data) {
        $("#txtID").val(data[0]['id']);
        $("#txtEmail").val(data[0]['email']);
        $("#txtPass").val(data[0]['password']);
    });
    let frm = new FormData();
    frm.append("id", id);
}

$("#btnVerClave").click(function () {
    if ($("#btnVerClave").hasClass("oculto")) {
        $("#txtPass").attr("type", "text");
        $("#txtRepitePass").attr("type", "text");
        $("#btnVerClave").removeClass("oculto");
        $("#iconVerClave").removeClass("bi-eye");
        $("#iconVerClave").addClass("bi-eye-slash");
    } else {
        $("#txtPass").attr("type", "password");
        $("#txtRepitePass").attr("type", "password");
        $("#btnVerClave").addClass("oculto");
        $("#iconVerClave").addClass("bi-eye");
        $("#iconVerClave").removeClass("bi-eye-slash");
    }
});


$("#btnAgregar").click(function () {
    limpiarCampos();
    habilitarCampos();
    $("#staticBackdropLabel").text("Agregar usuario");
});

jQuery('#limpia-filtro').on('click', function () {
    listar();
    $("#filtroEmail").val("");
});

function limpiarCampos() {
    $(".limpiarCampo").val("");
    campos = $(".required");
    for (let i = 0; i < campos.length; i++) {
        $("#campo" + i).removeClass("error");
    }
    $("#btnAceptar").removeClass("eliminar");
    $("#txtPass").attr("type", "password");
    $("#txtRepitePass").attr("type", "password");
    $("#btnVerClave").addClass("oculto");
    $("#iconVerClave").addClass("bi-eye");
    $("#iconVerClave").removeClass("bi-eye-slash");
}

function habilitarCampos() {
    $(".habilitarCampo").removeAttr("disabled");
    $("#txtPass").removeAttr("hidden");
    $(".lblPass").removeAttr("hidden");
    $("#txtRepitePass").removeAttr("hidden");
    $(".lblRepitePass").removeAttr("hidden");
    $("#btnVerClave").removeAttr("hidden");
}

function deshabilitarCampos() {
    $(".deshabilitarCampo").attr("disabled", "disabled");
    $("#txtPass").attr("hidden", "hidden");
    $(".lblPass").attr("hidden", "hidden");
    $("#txtRepitePass").attr("hidden", "hidden");
    $(".lblRepitePass").attr("hidden", "hidden");
    $("#btnVerClave").attr("hidden", "hidden");
}

function validaDatos() {
    let valido = true;
    campos = $(".required");
    for (let i = 0; i < campos.length; i++) {
        if (campos[i].value == "") {
            valido = false;
            $("#campo" + i).addClass("error");
        } else {
            $("#campo" + i).removeClass("error");
        }
    }
    if ($("#txtPass").val() != $("#txtRepitePass").val() || $("#txtPass").val() == "" || $("#txtRepitePass").val() == "") {
        valido = false;
        $(".lblPass").addClass("error");
        $(".lblRepitePass").addClass("error");
    } else {
        $(".lblPass").removeClass("error");
        $(".lblRepitePass").removeClass("error");
    }
    return valido;
}

function confirmarCambios() {
    if (validaDatos()) {
        let frm = new FormData();
        let id = $("#txtID").val();
        let email = $("#txtEmail").val();
        let pass = $("#txtPass").val();
        frm.append("id", id);
        frm.append("email", email);
        frm.append("password", pass);
        if ($("#btnAceptar").hasClass("eliminar")) {
            if (confirm("¿Seguro que desea eliminar el usuario?") == 1) {
                crudUsuarios(frm, "delete");
            }
        } else {
            crudUsuarios(frm, "save");
        }
    }
}

function crudUsuarios(frm, action) {
    $.ajax({
        type: "POST",
        url: "../Usuarios/" + action,
        data: frm,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data == 1) {
                listar();
                if ($("#btnAceptar").hasClass("eliminar")) {
                    alert("El usuario se eliminó correctamente");
                } else {
                    alert("El usuario se guardó correctamente");
                }
                $("#btnCancelar").click();
            } else if (data == -1) {
                alert("El email ingresado ya pertenece a otro usuario");
            } else {
                alert("Los cambios no se guardaron. Error en la base de datos");
            }
        }
    });
}