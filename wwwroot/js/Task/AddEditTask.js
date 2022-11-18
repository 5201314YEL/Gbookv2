// 加载窗体
window.onload = function() {
    fetch("/API?APICommand=GetUsersToAssign", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            // body:
        })
        .then(Response => Response.json())
        .then(data => {
            if (data.length > 0) {
                InitAssign(data);
            }
        })
        .catch(error => {
            console.log(error);
        })
}

function InitAssign(data) {
    let html = "<option></option>";
    for (let i = 0; i < data.length; i++) {
        let item = data[i];
        html += `<option value="${item.UserID}">${item.UserName}</option>`;
    }

    document.querySelector("#AssignedToUserID").innerHTML = html;
}

document.getElementById("AddTask").addEventListener("click", AddTask);

function AddTask() {
    const projectID = document.querySelector("#ProjectID").value;
    const userID = document.querySelector("#AssignedToUserID").value;
    const task = document.getElementById("Task").value;
    const taskDetail = document.getElementById("TaskDetail").innerText;
    if (projectID == "" || userID == "" || task == "") {
        alert("Project,AssignTo,TaskTitle不能为空!");
        return;
    }
    // 3.构造js对象
    const param = {
        Task: task,
        UserID: userID,
        ProjectID: projectID,
        TaskDetail: taskDetail
    }

    // 4.发送请求
    fetch("/API?APICommand=AddTask", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(param),
        })
        .then(Response => Response.json())
        .then(data => {
            // alert(data);
            if (data > 0) {
                alert("添加Task成功!");
                window.location = "/Home?DashboardID=1";
            } else {
                alert("添加Task失败!");
            }
        })
        .catch(error => {
            alert(error);
        })
}