document.getElementById("Register").addEventListener("click", Register);

function Register() {
    //取到input框中的数据
    let userName = document.getElementById("UserName").value;
    let password = document.getElementById("Password").value;
    let email = document.getElementById("Email").value;
    let phone = document.getElementById("Phone").value;
    let nickName = document.getElementById("NickName").value;

    password1 = hex_md5(password)

    let para = {
        UserName: userName,
        Password: password1,
        Email: email,
        Phone: phone,
        NickName: nickName
    }

    fetch("/API?APICommand=DemoRegister2", {
        method: "POST",
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(para)
    })
        .then(Response => Response.json())
        .then(data => {
            alert(data);
            if (data && data > 0) {
                alert("注册成功!");
                window.location = "/Home?";
            } else {
                alert("请重新注册");
            }
        })
        .catch(err => {
            alert(err);
        })

}
