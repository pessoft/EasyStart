function Login() {
    let login = $("#login").val();
    let password = $("#password").val();
    let successFunc = (result) => {
        if (result.Success) {
            window.location.href = result.URL;
        } else {
            alert(result.ErrMessage);
        }
    }

    if (!login || !password) {
        return;
    }

    let loginData = {
        Login: login,
        Password: password
    }

    $.post("/Home/Login", loginData, successCallBack(successFunc, null));
 }
