//1.button绑定点击事件
document.getElementById("AddBtn").addEventListener("click", function(){
    // 2.获取输入框的值
    const task = document.getElementById("Task").value;
    const userID = document.getElementById("UserID").value;
    const projectID = document.getElementById("ProjectID").value;

    // 3.构造js对象
    const param = {
        Task: task,
        UserID: userID,
        ProjectID: projectID
    }

    // 4.发送请求
    fetch("/API?APICommand=AddTask",{
        method:"POST",
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body:JSON.stringify(param),
    })
    .then(Response => Response.json())
    .then(data => {
        // alert(data);
        if (data > 0) {
            alert("添加Task成功!");
        } else {
            alert("添加Task失败!");
        }
    })
    .catch(error => {
        alert(error);
    })
});