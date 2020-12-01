MyDOMGetBoundingClientRect = (element, parm) => {
    return element.getBoundingClientRect();
};

MyIdGetBoundingClientRect = (id, parm) => {
    var element = document.getElementById(id);
    return element.getBoundingClientRect();
};

ConvertToSvg = (element, rect) => {
    const svg = element;
    //svg = document.getElementById('mysvg'),
    //NS = svg.getAttribute('xmlns');

    // click event
    //svg.addEventListener('click', (e) => {

    const pt = svg.createSVGPoint();

    // pass event coordinates
    pt.x = rect.x;
    pt.y = rect.y;

    // transform to SVG coordinates
    const svgP = pt.matrixTransform(svg.getScreenCTM().inverse());

    var res = { x: svgP.x, y: svgP.y };
    //res.X = svgP.x;
    //res.Y = svgP.y;
    return res;
}