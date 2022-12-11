$("#datepicker").datepicker(
    { minDate: 0, maxDate: "+1W", dateFormat: "dd/mm/yy" }
);

function listado(arrayHeader, data) {
    let contenido = "";
    contenido += "<table id='tabla-generic' class='container table table-light table-striped table-bordered table-hover'>";
    contenido += "<thead class='table-dark'>";
    contenido += "<tr class='text-center'>";
    for (let i = 0; i < arrayHeader.length; i++) {
        contenido += "<td>";
        contenido += arrayHeader[i];
        contenido += "</td>";
    }
    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";
    let llaves = Object.keys(data[0]);
    for (let i = 0; i < data.length; i++) {
        contenido += "<tr>";
        for (let j = 0; j < llaves.length; j++) {
            let valorLlaves = llaves[j];
            contenido += "<td>";
            contenido += data[i][valorLlaves];
            contenido += "</td>";
        }
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    document.getElementById('tabla-clase').innerHTML = contenido;
    $('#tabla-generic').DataTable(
        {
            searching: false
        }
    )
}

function llenarCombo(data, control, primerElemento) {
    let contenido = "";
    if (primerElemento == true) {
        contenido += "<option value='' disabled selected >--Seleccione una opción--</option>";
    }
    for (let i = 0; i < data.length; i++) {
        contenido += "<option value='" + data[i].ID + "'>";
        contenido += data[i].Nombre;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}

//Agregar clase en el campo a usar
function limpiarCampos() {
    $(".limpiarCampo").val("");
}

function habilitarCampos() {
    $(".habilitarCampo").removeAttr("disabled");
}

function deshabilitarCampos() {
    $(".deshabilitarCampo").attr("disabled", "disabled");
}