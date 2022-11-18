
//加载静态页面
window.onload = function (){
    fetch("/API?APICommand=GetTask",{
        method:"POST",
        headers:{
            'Content-Type': 'application/json;charset=utf-8'
        },
        // body:
    })
    .then(Response => Response.json())
    .then(data => {
        console.log(data);

        let html = "";
        for (let i = 0; i < data.length; i++) {
            const item = data[i];

            html +=`<tr>`;
            html +=`<td>${item.TaskID}</td>`;
            html +=`<td>${item.Task}</td>`;
            html +=`<td>${item.AssignTo}</td>`;
            html +=`</tr>`;
        }

        document.getElementById("TaskList").innerHTML = html;
    })
    .catch(error => {
        console.log(error);
    })
}