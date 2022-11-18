const debuggerServerInfo = "Y";
window.isShowServerInfoFlag = true;

function isNull (obj) {
    if (typeof obj === "undefined" || obj === null || obj === "") {
        return true;
    }
    return false;
}

function nullToEmptyStr (obj) {
    if(isNull(obj)){
        return "";
    }else{
        return obj;
    }
}

// 此处默认都先用post去提交，后面再扩展get, put, delete
class EC_Request {
    constructor(command) {
        this.Command = command;
        this.ContentType = "application/json;charset=utf-8";
        this.Variables = [];
    }
    AddRequestVariable(Name, value) {
        this.Variables.push({
            Name,
            Value: nullToEmptyStr(value)
        });
    }
    AddFile(fileEl){
        if (fileEl) {
            const files = fileEl.files;
            if(files){
                for (let j = 0; j < files.length; j++) {
                    const file = files[j];
                    if (!isNull(file)) {
                        this.Variables.push({
                            Name: file.name,
                            Value: file
                        });
                        this.ContentType = "multipart/form-data";
                    }
                }
            }
        }
    }
    Submit() {
        const Me = this;
        return new Promise((resolve, reject) => {
            let requestData = {};

            if (Me.ContentType === "multipart/form-data") {
                const paramsConstructData = new FormData();
                Me.Variables.forEach((param) => {
                    paramsConstructData.append(param["Name"], param["Value"]);
                });
                requestData = paramsConstructData;
            } else {
                Me.Variables.forEach((param) => {
                    requestData[param["Name"]] = param["Value"]
                });
                requestData.debuggerServerInfo = debuggerServerInfo;
                requestData = JSON.stringify(requestData);
            }

            fetch(`/API?APICommand=`+Me.Command,{
                method:"POST",
                headers: Me.ContentType === "multipart/form-data" ? {} : { 'Content-Type': Me.ContentType },
                body: requestData,
            })
            .then(Response => Response.json())
            .then(data => {
                if(debuggerServerInfo === "Y"){
                    if (!document.getElementById("systemRunInfoArea")) {
                        document.body.insertAdjacentHTML("beforebegin", `<div style="display:${debuggerServerInfo === "Y" ? "" : "none"}" onclick='showOrHideServerInfo()'>ServerDebuggerInfo</div><div id='systemRunInfoArea' style='display:none'></div>`);
                    }

                    document.getElementById("systemRunInfoArea").innerHTML = "";
                    const html = [];
                    html.push("<p>SQL Info</p>");
                    html.push(data.sqlInfo);
                    html.push("<p>Parameter Info</p>");
                    html.push(data.paramInfo);
                    html.push("<p>System error Info</p>");
                    html.push(data.retMsg);
                    document.getElementById("systemRunInfoArea").innerHTML = html.join("");
                }
                if (data.retCode == 1) {
                    resolve(data.data);
                } else {
                    alert(data.retMsg);
                    reject(data.retMsg);
                }
            }).catch(error => {
                alert(error);
                reject(error);
            });
        });
    }
};

function showOrHideServerInfo () {
    window.isShowServerInfoFlag=!window.isShowServerInfoFlag; 
    document.getElementById("systemRunInfoArea").style.display= window.isShowServerInfoFlag ? "" : "none";
}

// TODO: 后续添加loading, toast, dialog, alert, confirm等自定义的弹出框和遮罩层