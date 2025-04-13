namespace molmil {
    
    export function setOnContextMenu(obj, func, lefttoo) {
        obj.oncontextmenu = func;
        if (lefttoo) obj.onclick = func;

        obj.addEventListener("touchstart", molmil.handle_contextMenu_touchStart, false);
        obj.addEventListener("touchend", molmil.handle_contextMenu_touchEnd, false);
    }
}