function initDragAndDrop() {
    if (window.goSamples) goSamples();  // init for these samples -- you don't need to call this

    // *********************************************************
    // First, set up the infrastructure to do HTML drag-and-drop
    // *********************************************************

    var dragged = null; // A reference to the element currently being dragged

    // highlight stationary nodes during an external drag-and-drop into a Diagram
    function highlight(node) {  // may be null
        var oldskips = myDiagramDragAndDrop.skipsUndoManager;
        myDiagramDragAndDrop.skipsUndoManager = true;
        myDiagramDragAndDrop.startTransaction("highlight");
        if (node !== null) {
            myDiagramDragAndDrop.highlight(node);
        } else {
            myDiagramDragAndDrop.clearHighlighteds();
        }
        myDiagramDragAndDrop.commitTransaction("highlight");
        myDiagramDragAndDrop.skipsUndoManager = oldskips;
    }

    // This event should only fire on the drag targets.
    // Instead of finding every drag target,
    // we can add the event to the document and disregard
    // all elements that are not of class "draggable"
    document.addEventListener("dragstart", function (event) {
        if (event.target.className !== "draggable") return;

        // Some data must be set to allow drag
        event.dataTransfer.setData("text", event.target.textContent);

        // store a reference to the dragged element and the offset of the mouse from the center of the element
        dragged = event.target;
        dragged.offsetX = event.offsetX - dragged.clientWidth / 2;
        dragged.offsetY = event.offsetY - dragged.clientHeight / 2;

        // Objects during drag will have a red border
        event.target.style.border = "2px solid red";
    }, false);

    // This event resets styles after a drag has completed (successfully or not)
    document.addEventListener("dragend", function (event) {
        // reset the border of the dragged element
        dragged.style.border = "";
        highlight(null);
    }, false);

    // Next, events intended for the drop target - the Diagram div

    var div = document.getElementById("myDiagramDragAndDrop");
    div.addEventListener("dragenter", function (event) {
        // Here you could also set effects on the Diagram,
        // such as changing the background color to indicate an acceptable drop zone

        // Requirement in some browsers, such as Internet Explorer
        event.preventDefault();
    }, false);

    div.addEventListener("dragover", function (event) {
        // We call preventDefault to allow a drop
        // But on divs that already contain an element,
        // we want to disallow dropping

        if (this === myDiagramDragAndDrop.div) {
            var can = event.target;
            var pixelratio = window.PIXELRATIO;

            // if the target is not the canvas, we may have trouble, so just quit:
            if (!(can instanceof HTMLCanvasElement)) return;

            var bbox = can.getBoundingClientRect();
            var bbw = bbox.width;
            if (bbw === 0) bbw = 0.001;
            var bbh = bbox.height;
            if (bbh === 0) bbh = 0.001;
            var mx = event.clientX - bbox.left * ((can.width / pixelratio) / bbw);
            var my = event.clientY - bbox.top * ((can.height / pixelratio) / bbh);
            var point = myDiagramDragAndDrop.transformViewToDoc(new go.Point(mx, my));
            var curnode = myDiagramDragAndDrop.findPartAt(point, true);
            if (curnode instanceof go.Node) {
                highlight(curnode);
            } else {
                highlight(null);
            }
        }

        if (event.target.className === "dropzone") {
            // Disallow a drop by returning before a call to preventDefault:
            return;
        }

        // Allow a drop on everything else
        event.preventDefault();
    }, false);

    div.addEventListener("dragleave", function (event) {
        // reset background of potential drop target
        if (event.target.className == "dropzone") {
            event.target.style.background = "";
        }
        highlight(null);
    }, false);

    // handle the user option for removing dragged items from the Palette
    var remove = document.getElementById('remove');

    div.addEventListener("drop", function (event) {
        // prevent default action
        // (open as link for some elements in some browsers)
        event.preventDefault();

        // Dragging onto a Diagram
        if (this === myDiagramDragAndDrop.div) {
            var can = event.target;
            var pixelratio = window.PIXELRATIO;

            // if the target is not the canvas, we may have trouble, so just quit:
            if (!(can instanceof HTMLCanvasElement)) return;

            var bbox = can.getBoundingClientRect();
            var bbw = bbox.width;
            if (bbw === 0) bbw = 0.001;
            var bbh = bbox.height;
            if (bbh === 0) bbh = 0.001;
            var mx = event.clientX - bbox.left * ((can.width / pixelratio) / bbw) - dragged.offsetX;
            var my = event.clientY - bbox.top * ((can.height / pixelratio) / bbh) - dragged.offsetY;
            var point = myDiagramDragAndDrop.transformViewToDoc(new go.Point(mx, my));
            myDiagramDragAndDrop.startTransaction('new node');
            myDiagramDragAndDrop.model.addNodeData({
                location: point,
                text: event.dataTransfer.getData('text'),
                color: "lightyellow"
            });
            myDiagramDragAndDrop.commitTransaction('new node');
            //alert("X:"+mx.toString()+" Y:"+my.toString());

            // remove dragged element from its old location
            if (remove.checked) dragged.parentNode.removeChild(dragged);
        }

        // If we were using drag data, we could get it here, ie:
        // var data = event.dataTransfer.getData('text');
    }, false);

    // *********************************************************
    // Second, set up a GoJS Diagram
    // *********************************************************

    var $ = go.GraphObject.make;  // for conciseness in defining templates

    myDiagramDragAndDrop = $(go.Diagram, "myDiagramDragAndDrop",  // create a Diagram for the DIV HTML element
    {
        "undoManager.isEnabled": true,
        "PartResized": function (e) {
            var obj = e.subject;
            console.log(obj.data.text + ":" + obj.desiredSize.toString());
        },
    });
    window.PIXELRATIO = myDiagramDragAndDrop.computePixelRatio(); // constant needed to determine mouse coordinates on the canvas
    myDiagramDragAndDrop.grid.visible = true;
    myDiagramDragAndDrop.grid.gridCellSize = new go.Size(3, 2);
    myDiagramDragAndDrop.toolManager.draggingTool.isGridSnapEnabled = true;
    myDiagramDragAndDrop.toolManager.resizingTool.isGridSnapEnabled = true;

    // define a simple Node template
    myDiagramDragAndDrop.nodeTemplate =
        $(go.Node, "Auto",
            { locationSpot: go.Spot.Center },
            { resizable: true },
            new go.Binding('location'),
            $(go.Shape, "Rectangle",
                { fill: 'white' },
                // Shape.fill is bound to Node.data.color
                new go.Binding("fill", "color"),
                // this binding changes the Shape.fill when Node.isHighlighted changes value
                new go.Binding("fill", "isHighlighted", function (h, shape) {
                    if (h) return "red";
                    var c = shape.part.data.color;
                    return c ? c : "white";
                }).ofObject()),  // binding source is Node.isHighlighted
            $(go.TextBlock,
                { margin: 3, font: "normal 10px sans-serif", width: 140, textAlign: 'center' },
                // TextBlock.text is bound to Node.data.key
                new go.Binding("text"))
        );

    // but use the default Link template, by not setting Diagram.linkTemplate

    // create the model data that will be represented by Nodes and Links
    myDiagramDragAndDrop.model = new go.GraphLinksModel(
        [],
        []
    );
}

// Show the diagram's model in JSON format
function DragDropSave() {
    //alert(myDiagramDragAndDrop.model.toJson());
}

function DragDropLoad() {
    myDiagram.model = go.Model.fromJson(document.getElementById("mySavedModel").value);
}
