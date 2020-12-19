MyDOMGetBoundingClientRect = (element, parm) => {
    return element.getBoundingClientRect();
};

MyIdGetBoundingClientRect = (id, parm) => {
    var element = document.getElementById(id);
    return element.getBoundingClientRect();
};

ConvertToSvg = (element, rect) => {
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

HideModal = (name) => {
    $(name).modal('hide');
}
