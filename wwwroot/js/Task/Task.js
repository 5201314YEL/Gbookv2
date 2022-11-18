function createPagingHtml(data) {

    let maxPage = Math.floor(data.allCount / data.pageSize);
    maxPage = data.allCount % data.pageSize > 0 ? maxPage + 1 : maxPage;

    const pageHTML = [];
    pageHTML.push(`<div onclick="queryTask(0)">First</div>`);
    if (data.pageNum > 0) {
        pageHTML.push(`<div onclick="queryTask(${data.pageNum -1})">Prev</div>`);
    }

    pageHTML.push(`<div id="pageIndexArea" style="display:flex">`);
    if (maxPage > 5) {
        let i = data.pageNum - 2;
        i = i < 0 ? 0 : i;
        i = i > maxPage ? maxPage : i
        let max = data.pageNum + 3;
        max = max < 5 ? 5 : max;
        for (; i < max; i++) {
            if (data.pageNum === i) {
                pageHTML.push(`<span class="noEvent">${i+1}</span>`);
            } else {
                pageHTML.push(`<span onclick="queryTask(${i})">${i+1}</span>`);
            }
        }
    } else {

        //show all page
        for (let i = 0; i <= maxPage; i++) {
            if (data.pageNum === i) {
                pageHTML.push(`<span class="noEvent">${i+1}</span>`);
            } else {
                pageHTML.push(`<span onclick="queryTask(${i})">${i+1}</span>`);
            }
        }
    }
    pageHTML.push(`</div>`);
    if (data.pageNum < maxPage - 1) {
        pageHTML.push(`<span class="next"onclick="queryTask(${data.pageNum +1})">Next</span>`);

    }

    pageHTML.push(`<div onclick="queryTask(${maxPage})">End</div>`);
    pageHTML.push(`<div style="display:flex">`);
    pageHTML.push(`     <select name="pageSize" id="pageSize" onchange="queryTask(${data.pageNum})">`);

    for (let i = 0, pageSizeArr = [10, 20, 30, 40]; i < pageSizeArr.length; i++) {
        pageHTML.push(`<option value="${pageSizeArr[i]}" ${pageSizeArr[i] === data.pageSize ? "selected = 'selected'":""}>${pageSizeArr[i]}</option>`)
    }
    pageHTML.push(`</select>`);
    pageHTML.push(`<span class="noEvent">条</span>`);
    pageHTML.push(`</div>`);

    document.getElementById("pagingArea").innerHTML = pageHTML.join("");
}


window.onload = function() {

    queryTask();
}

function detail(elID) {
    const el = document.getElementById(elID);
    const ss = el.style.display;
    if (ss === 'none') {
        el.style.display = ''
    } else {
        el.style.display = 'none'
    }
}

function initData(data) {
    let html = "";
    for (let i = 0; i < data.dataList.length; i++) {
        let item = data.dataList[i];
        html += `<tr class="parent-row">
        <td class="link-icon-column">
        <a href="/Home?DashboardID=4&TaskID=${item.TaskID}" title="View">
        <img src="../../image/view.svg" />
        </a>
        </td>
        <td class="link-icon-column">
        <a href="/Home?DashboardID=5&TaskID=${item.TaskID}" title="Edit" class="ews-icon-edit">
        <img src="../../image/edit.svg" />
        </a>
        </td>
        <td class="link-icon-column" onclick="deleteTask(${item.TaskID})" >
        <img src="../../image/delete.svg" / >
        </td>
        <td>${item.TaskID}</td>
        <td>
        <a href="/Home?DashboardID=6">${item.Project}</a>
        </td>
        <td>
        ${item.Task}
        </td>
        <td>
        ${item.AssignTo}
        </td>
        <td>
        ${item.TimeUpdated}
        </td>
        <td>
        ${item.UpdatedBy}
        </td>
        </tr>`;
    }
    document.querySelector(".task-body").innerHTML = html;
    createPagingHtml(data);
}


function deleteTask(TaskID) {
    if (confirm("Are you sure you want to delete this task")) {
        fetch("/API?APICommand=deleteTask", {
                method: "POST",
                headers: { 'Content-Type': 'application/json;charset=utf-8' },
                body: JSON.stringify({ "TaskID": TaskID })
            })
            .then(Response => Response.json())
            .then(data => {
                if (data.Code === "OK") {
                    queryTask();
                } else {
                    alert(data);
                }
            })
            .catch(error => {
                console.log(error);
            })
    }
}

function isNull(obj) {
    if (obj === "undefined" || obj === "" || obj === "null") {
        return true;
    }
    return false;
}
const queryTask = function(pageNum) {
    pageNum = isNull(pageNum) ? 0 : pageNum;
    let pageSize = document.getElementById("pageSize") ? document.getElementById("pageSize").value : 10;
    let taskID = document.getElementById("TaskID").value;
    let task = document.getElementById("Task").value;
    let userID = document.getElementById("AssignedToUserID").value;
    let param = {
        TaskID: taskID,
        Task: task,
        AssignedToUserID: userID,


        //分页
        "pageNum": pageNum,
        "pageSize": pageSize
    }
    fetch("/API?APICommand=QueryTask", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(param),
        })
        .then(Response => Response.json())
        .then(data => {

            initData(data);

        })
        .catch(error => {
            console.log(error);
        })
}

document.getElementById("SearchBtn").addEventListener("click", function() { queryTask() });