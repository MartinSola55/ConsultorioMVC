$('#btnIngresar').on('click', function () {
    let email = $('#txtEmail').val();
    let pass = $('#txtPass').val();
    if (email == "") {
        alert("Debes ingresar un email");
        return;
    }
    if (pass == "") {
        alert("Debes ingresar una contraseña");
        return;
    }

    $.get("../Login/validar/?email=" + email + "&pass=" + pass, function (data) {
        if (data == 1) {
            document.location.href = "../Turnos";
        } else {
            alert("Usuario y/o contraseña incorrectos");
        }
    });
});