﻿export function MyDOMGetBoundingClientRect(element, parm) {
    return element.getBoundingClientRect();
};

export function MyIdGetBoundingClientRect(id, parm) {
    var element = document.getElementById(id);
    return element.getBoundingClientRect();
};

export function ConvertToSvg(element, rect) {
    const svg = element;
    const pt = svg.createSVGPoint();

    // pass event coordinates
    pt.x = rect.x;
    pt.y = rect.y;

    // transform to SVG coordinates
    const svgP = pt.matrixTransform(svg.getScreenCTM().inverse());

    var res = { x: svgP.x, y: svgP.y };
    return res;
}

export function HideModal(name) {
    $(name).modal('hide');
}

export function ShowToast(name) {
    $(name).toast('show');
}

export function Collapse(name) {
    $(name).collapse('hide');
}

export function CollapseShow(name) {
    $(name).collapse('show');
}

export function FocusElement(id) {
    //const element = document.getElementById(id);
    //element.focus();
    //element.select();
    $('#' + id).focus().select();
}

export function OpenFileClick(id) {
    $('#' + id).trigger('click');
}

export function GetFileName(id) {
    var res = document.getElementById(id).files[0];
    return res;
}

// not working
export function PreventLeaveApp() {
    window.onbeforeunload = function () {
        return "";
    }
}

