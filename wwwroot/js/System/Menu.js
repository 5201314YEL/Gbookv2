document.getElementById("UserName").innerText = getCookie("UserName");

function getCookie(cname){
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for(var i=0; i<ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name)==0) { 
            
            return c.substring(name.length,c.length); 
        }
    }
    return "";
}

function formatTime(Dates){
    const Y = Dates.getFullYear();
    const M = Dates.getMonth() + 1;
    const D = Dates.getDate();
    const H = Dates.getHours();   //hour
    const m = Dates.getMinutes(); //minute
    return M+"/"+D+"/"+Y+" "+H+": "+m;
}