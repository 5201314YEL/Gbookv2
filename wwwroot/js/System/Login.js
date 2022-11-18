document.querySelector("#Submit").addEventListener("click", LoginIn);

function LoginIn() {
    let userName = document.querySelector("#login-username").value;
    let userPwd = document.querySelector("#login-password").value;
    userPwd = hex_md5(userPwd)
    if (userName == "" || userPwd == "") {
        alert("用户名和密码不能为空");
        return;
    }

    // 构建一个请求参数对象
    let param = { UserName: userName, Password: userPwd };

    // 通过fetch发送并接收前台传递的json数据
    fetch("/API?APICommand=Login", {
        method: "post",
        // body:formData,
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(param)
    }).then(res => res.json())
        .then(data => {
            // alert(data);
            // alert(data.UserName);
            if (data.retCode == -1) {
                alert("用户名或密码错误,请重新输入!");
            } else {
                document.cookie = `UserName=${data.data.UserName}; UserID=${data.data.UserID}`;
                window.location = "/Home?DashboardID=-1"; //前端js跳转不够安全,后期需要优化
            }
        })
        .catch(err => {
            alert(err);
        });
};
