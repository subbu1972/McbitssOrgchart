function initPartition(w, h, DragDrop) {
    var shape_X_Dimention = calculateShapeXDimention(Settings.SelectShape);
    var shape_Y_Dimention = calculateShapeYDimention(Settings.SelectShape);

    if (window.goSamples) goSamples();  // init for these samples -- you don't need to call this
    var $ = go.GraphObject.make;  // for conciseness in defining templates

    myPartitionDiagram =
        $(go.Diagram, "myPartitionDiagramDiv", // must be the ID or reference to div
            {
                initialDocumentSpot: go.Spot.TopCenter,
                initialContentAlignment: go.Spot.TopCenter,
                maxSelectionCount: 1, // users can select only one part at a time
                validCycle: go.Diagram.CycleDestinationTree, // make sure users can only create trees
                "clickCreatingTool.archetypeNodeData": {}, // allow double-click in background to create a new node
                "clickCreatingTool.insertPart": function (loc) {  // customize the data for the new node
                    this.archetypeNodeData = {
                        key: getNextKey(), // assign the key based on the number of nodes
                        name: "(new person)",
                        title: ""
                    };
                    return go.ClickCreatingTool.prototype.insertPart.call(this, loc);
                },
                layout:
                    $(go.TreeLayout,
                        {
                            treeStyle: go.TreeLayout.StyleLastParents,
                            arrangement: go.TreeLayout.ArrangementVertical,
                            // properties for most of the tree:
                            angle: 90,
                            layerSpacing: 20,
                            nodeSpacing: 20,
                            breadthLimit: w,
                            // properties for the "last parents":
                            alternateAngle: 90,
                            alternateLayerSpacing: 35,
                            alternateAlignment: go.TreeLayout.AlignmentBus,
                            alternateNodeSpacing: 20
                        }),
                "undoManager.isEnabled": true,  // enable undo & redo
                allowDragOut: DragDrop
            });

    myPartitionDiagram.doFocus = function () {
        var x = window.scrollX || window.pageXOffset;
        var y = window.scrollY || window.pageYOffset;
        go.Diagram.prototype.doFocus.call(this);
        window.scrollTo(x, y);
    }

    // when the document is modified, add a "*" to the title and enable the "Save" button
    myPartitionDiagram.addDiagramListener("Modified", function (e) {
        var button = document.getElementById("SaveButton");
        if (button) button.disabled = !myPartitionDiagram.isModified;
        var idx = document.title.indexOf("*");
        if (myPartitionDiagram.isModified) {
            if (idx < 0) document.title += "*";
        } else {
            if (idx >= 0) document.title = document.title.substr(0, idx);
        }
    });

    myPartitionDiagram.addDiagramListener("ViewportBoundsChanged", function (e) {
        let allowScroll = !e.diagram.viewportBounds.containsRect(e.diagram.documentBounds);
        myPartitionDiagram.allowHorizontalScroll = false;
        myPartitionDiagram.allowVerticalScroll = false;
    });

    // manage boss info manually when a node or link is deleted from the diagram
    myPartitionDiagram.addDiagramListener("SelectionDeleting", function (e) {
        var part = e.subject.first(); // e.subject is the myPartitionDiagram.selection collection,
        // so we'll get the first since we know we only have one selection
        myPartitionDiagram.startTransaction("clear boss");
        if (part instanceof go.Node) {
            var it = part.findTreeChildrenNodes(); // find all child nodes
            while (it.next()) { // now iterate through them and clear out the boss information
                var child = it.value;
                var bossText = child.findObject("boss"); // since the boss TextBlock is named, we can access it by name
                if (bossText === null) return;
                bossText.text = "";
            }
        } else if (part instanceof go.Link) {
            var child = part.toNode;
            var bossText = child.findObject("boss"); // since the boss TextBlock is named, we can access it by name
            if (bossText === null) return;
            bossText.text = "";
        }
        myPartitionDiagram.commitTransaction("clear boss");
    });

    myPartitionDiagram.addDiagramListener('SelectionMoved', function (e) {
        var dia = e.diagram;

        // add height for horizontal scrollbar
        dia.div.style.height = (dia.documentBounds.height + 24) + "px";
    });

    myPartitionDiagram.addDiagramListener("InitialLayoutCompleted", function (e) {
        var dia = e.diagram;

        // add height for horizontal scrollbar
        dia.div.style.height = (dia.documentBounds.height + 24) + "px";
    });

    var levelColors = ["#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#C0C0C0/#C0C0C0", "#D3D3D3/#D3D3D3", "#FFF44F/#FFF44F"];
    Settings.TextColor = "black";
    Settings.UpArrow = HOST_ENV + "/Content/Images/uparrow.jpg";
    Settings.DownArrow = HOST_ENV + "/Content/Images/downarrow.jpg";
    //Settings.BorderColor = "cyan";
    Settings.BorderWidth = 3;
    if (Settings.Skin.toUpperCase() == "BROWN") {
        levelColors = ["#634329/#634329", "#923222/#923222", "#e44c16/#e44c16", "#ec8026/#ec8026", "#fcaf17/#fcaf17", "#fed300/#fed300", "#f5eb07/#f5eb07", "#e44c16/#e44c16"];
        Settings.TextColor = "white";
        Settings.UpArrow = HOST_ENV + "/Content/Images/uparroww.png";
        Settings.DownArrow = HOST_ENV + "/Content/Images/downarrow.ico";
        //Settings.BorderColor = "cyan";
        Settings.BorderWidth = 0;
    }

    // override TreeLayout.commitNodes to also modify the background brush based on the tree depth level
    myPartitionDiagram.layout.commitNodes = function () {
        go.TreeLayout.prototype.commitNodes.call(myPartitionDiagram.layout);  // do the standard behavior
        // then go through all of the vertexes and set their corresponding node's Shape.fill
        // to a brush dependent on the TreeVertex.level value
        myPartitionDiagram.layout.network.vertexes.each(function (v) {
            if (v.node) {
                var level = v.level % (levelColors.length);
                var colors = levelColors[level].split("/");
                var shape = v.node.findObject("SHAPE");

                //if (shape) shape.fill = $(go.Brush, "Linear", { 0: color, 1: go.Brush.lightenBy(color, 0.05), start: go.Spot.Left, end: go.Spot.Right });
                if (shape) {
                    if (v.node.data.LEVEL_ID) {
                        if (v.node.data.LEVEL_ID == document.getElementById("hdnQueryStringSearchValue").value) {
                            colors = levelColors[7].split("/");
                        }
                    }
                    if (v.node.data.FULL_NAME) {
                        if (v.node.data.FULL_NAME.toUpperCase().indexOf("VACANT") != -1) {
                            colors = levelColors[6].split("/");
                        }
                    }
                    shape.fill = $(go.Brush, "Linear", { 0: colors[0], 1: colors[1], start: go.Spot.Left, end: go.Spot.Right });
                }
            }
        });
    };

    // This function is used to find a suitable ID when modifying/creating nodes.
    // We used the counter combined with findNodeDataForKey to ensure uniqueness.
    function getNextKey() {
        var key = nodeIdCounter;
        while (myPartitionDiagram.model.findNodeDataForKey(key) !== null) {
            key = nodeIdCounter--;
        }
        return key;
    }

    // Id first element but NOT CEO
    function isFirstElement(key) {

        if (key == document.getElementById("hdnOrgPartitionShowLevel").value && myPartitionDiagram.model.findNodeDataForKey(key).parent != "999999")
            return true;
        else
            return false;
    };

    // if not first element and has children then return true.
    function hasChildren(key) {

        // if first element 
        if (key == document.getElementById("hdnOrgPartitionShowLevel").value) return false;

        //if has children
        //alert(key + ":" + myPartitionDiagram.model.findNodeDataForKey(key).SOC_COUNT.toString() + ":" + (myPartitionDiagram.model.findNodeDataForKey(key).SOC_COUNT > 0).toString());
        return myPartitionDiagram.model.findNodeDataForKey(key).SOC_COUNT > 0 ? true : false;
    };

    var nodeIdCounter = -1; // use a sequence to guarantee key uniqueness as we add/remove/modify nodes

    // when a node is double-clicked, add a child to it
    function nodePartitionDoubleClick(e, obj) {

        if (obj.data.parent != "999999") {
            var element = document.querySelector('.overlay');
            element.style.display = 'block';

            var URL = HOST_ENV+"/Version/PartitionChangeShowLevel";
            var JsonData = {
                ShowLevel: obj.data.key,
                ParentLevel: obj.data.parent,
                Version: document.getElementById("hdnOrgPartitionVersion").value
            };

            jQuery.ajax({
                type: "POST",
                url: URL,
                data: JsonData,
                async: true,
                dateType: "json",
            }).done(function (JsonString) {
                if (JsonString.Message != "No Changes") {
                    document.getElementById("hdnOrgChartHRCoreData").value = JsonString.ChartData;
                    document.getElementById("hdnOrgPartitionShowLevel").value = JsonString.UsedShowLevel;
                    document.getElementById("hdnOrgPartitionVersion").value = JsonString.UsedVersion;
                    loadPartitionJSON("{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + JsonString.ChartData + " }");

                    myPartitionDiagram.startTransaction("properties");

                    var dia = e.diagram;

                    // add height for horizontal scrollbar
                    dia.div.style.height = (dia.documentBounds.height + 24) + "px";

                    myPartitionDiagram.commitTransaction("properties");

                    PartitionBreadGram();
                }
                element.style.display = 'none';
            });
        }
    }


    // this is used to determine feedback during drags
    function mayWorkFor(node1, node2) {
        if (!(node1 instanceof go.Node)) return false;  // must be a Node
        if (node1 === node2) return false;  // cannot work for yourself
        if (node2.isInTreeOf(node1)) return false;  // cannot work for someone who works for you
        return true;
    }

    // This function provides a common style for most of the TextBlocks.
    // Some of these values may be overridden in a particular TextBlock.
    function textStyle() {
        return { font: "9pt  Segoe UI,sans-serif", stroke: Settings.TextColor };
    }

    //// This converter is used by the Picture.
    //function findHeadShot(key) {
    //    if (key < 0 || key > 100000006) return "images/HSnopic.png"; // There are only 16 images on the server
    //    return HOST_ENV + "/content/images/photos/" + key + ".jpg"
    //}

    // This converter is used by the Picture.
    function findHeadShot(key) {
        if (key < 0 || key > 100000006) return "images/HSnopic.png"; // There are only 16 images on the server
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/" + (currentdate.getMonth() + 1) + "/" + currentdate.getFullYear() +
            " @ " + currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds();

        return HOST_ENV + "/Content/Images/PHOTOS/" + key + ".jpg?" + datetime;
    }

    // This converts dotted line to a different shape
    function findLinkColorForLevel(key) {
        return Settings.LineColor;
    };

    function addCommas(nStr) {
        nStr += '';
        var x = nStr.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }

    // This converts dotted line to a panel border width
    function findStrokeWidthForDottedLine(key) {
        return Settings.BorderWidth;
    }

    function findDottedLineReport(key) {
        if (key == 'N')
            return null;
        else
            return [2, 6];
    }

    // define the Node template
    if (document.getElementById("hdnOrgView").value == "Normal") {
        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                        }
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),

                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            source: Settings.UpArrow,
                            desiredSize: new go.Size(25, 15),
                            visible: false
                        },
                        new go.Binding("visible", "key", isFirstElement)),

                    $(go.Panel, "Horizontal",
                        $(go.Picture,
                            {
                                name: "Picture",
                                desiredSize: new go.Size(50, 50),
                                margin: new go.Margin(4, 4, 4, 4),
                            },
                            new go.Binding("source", "key", findHeadShot)),

                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 3, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 6 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 5,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 16), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 5,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "POSITION_TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                { row: 2, column: 0, stroke: Settings.TextColor },
                                new go.Binding("text", "key", function (v) { return v; })),
                            $(go.TextBlock, textStyle(),
                                { name: "boss", row: 2, column: 3, stroke: Settings.TextColor }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "parent", function (v) { return v; })),
                            $(go.TextBlock, textStyle(),
                                { row: 3, column: 0, stroke: Settings.TextColor },
                                new go.Binding("text", "SOC_COUNT", function (v) { return "SOC: "+v; })),
                            $(go.TextBlock, textStyle(),
                                { name: "boss", row: 3, column: 3, stroke: Settings.TextColor }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR: "+v; })),
                            $(go.TextBlock, textStyle(),  // the comments
                                {
                                    row: 4, column: 0, columnSpan: 5,
                                    font: "italic 9pt sans-serif",
                                    wrap: go.TextBlock.WrapFit,
                                    editable: true,  // by default newlines are allowed
                                    minSize: new go.Size(10, 14), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "comments").makeTwoWay())
                        )  // end Table Panel
                    ), // end Horizontal Panel

                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            source: Settings.DownArrow,
                            desiredSize: new go.Size(25, 15),
                            alignment: go.Spot.Bottom,
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))

            ),
            {
                selectionAdornmentTemplate:
                    $(go.Adornment, "Auto",
                        $(go.Shape, "RoundedRectangle",
                            { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                        $(go.Placeholder)
                    )  // end Adornment
            }  // end Vertical Panel
            );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "Mcbitss") {
        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
            { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                            SetParentChildRelationship(e, selnode.data);
                        }
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),

                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.UpArrow,
                            visible: false
                        },
                        new go.Binding("visible", "key", isFirstElement)),

                    $(go.Panel, "Horizontal",
                        $(go.Picture,
                            {
                                name: "Picture",
                                desiredSize: new go.Size(50, 50),
                                margin: new go.Margin(4, 4, 4, 4),
                            },
                            new go.Binding("source", "key", findHeadShot)),

                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 5,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 16), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 5,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "POSITION_TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 5,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "AD_DEPARTMENT").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 0, columnSpan: 5,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "DIVISION").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                { row: 4, column: 0, margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor },
                                new go.Binding("text", "key", function (v) { return "Corporate No: " + v; })),
                            $(go.TextBlock, textStyle(),
                                { name: "boss", row: 4, column: 3, margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "SOC_COUNT", function (v) { return "SOC :" + v; }))

                        )  // end Table Panel
                    ), // end Horizontal Panel

                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.DownArrow,
                            alignment: go.Spot.Bottom,
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))

            ),
            {
                selectionAdornmentTemplate:
                    $(go.Adornment, "Auto",
                        $(go.Shape, "RoundedRectangle",
                            { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                        $(go.Placeholder)
                    )  // end Adornment
            }  // end Vertical Panel
         );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "Cost") {
        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                        }

                        SetParentChildRelationship(e, selnode.data);
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),
                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.UpArrow,
                            visible: false
                        },
                        new go.Binding("visible", "key", isFirstElement)),

                    $(go.Panel, "Horizontal",
                        $(go.Picture,
                            {
                                name: "Picture",
                                desiredSize: new go.Size(50, 50),
                                margin: new go.Margin(4, 4, 4, 4),
                            },
                            new go.Binding("source", "key", findHeadShot)),
                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 3, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 6 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 5,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 16), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, "Title: ", textStyle(),
                                { row: 1, column: 0, stroke: Settings.TextColor }),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 1, columnSpan: 4,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3), stroke: Settings.TextColor
                                },
                                new go.Binding("text", "POSITION_TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                { row: 2, column: 0, stroke: Settings.TextColor },
                                new go.Binding("text", "POSITION_COST", function (v) { return "Cost: " + addCommas(parseFloat(v).toFixed(2).toLocaleString()); })),
                            $(go.TextBlock, textStyle(),
                                { name: "boss", row: 2, column: 4, stroke: Settings.TextColor, }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "POSITION_CALCULATED_COST", function (v) { return "Total Cost: " + addCommas(parseFloat(v).toFixed(2).toLocaleString()); })),
                            $(go.TextBlock, textStyle(),
                                { row: 3, column: 0, stroke: Settings.TextColor },
                                new go.Binding("text", "SOC_COUNT", function (v) { return "SOC: " + v; })),
                            $(go.TextBlock, textStyle(),
                                { name: "boss", row: 3, column: 4, stroke: Settings.TextColor }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR: " + v; }))
                        )  // end Table Panel
                    ), // end Horizontal Panel

                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.DownArrow,
                            alignment: go.Spot.Bottom,
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))
                ),
                {
                    selectionAdornmentTemplate:
                        $(go.Adornment, "Auto",
                            $(go.Shape, "RoundedRectangle",
                                { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                            $(go.Placeholder)
                        )  // end Adornment
                }  // end Vertical Panel
            );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "ALCON") {
        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                            SetParentChildRelationship(e, selnode.data);
                        }
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),

                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.UpArrow,
                            visible: false
                        },
                        new go.Binding("visible", "key", isFirstElement)),
                    $(go.Panel, "Horizontal",
                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 8,
                                    font: "7pt Segoe UI,sans-serif",
                                    editable: false, isMultiline: false,
                                    minSize: new go.Size(10, 10),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "key", function (v) {
                                    var SupervisoryName = myPartitionDiagram.model.findNodeDataForKey(v).SUPERVISORYORG_TEXT;
                                    SupervisoryName += "( " + myPartitionDiagram.model.findNodeDataForKey(v).SUPERVISORY_ORG + " )";

                                    return SupervisoryName;
                                })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 8,
                                    editable: false, isMultiline: false,
                                    minSize: new go.Size(10, 10),
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "key", function (v) {
                                    var EmployeeName = myPartitionDiagram.model.findNodeDataForKey(v).EMP_NAME;
                                    EmployeeName += "( " + myPartitionDiagram.model.findNodeDataForKey(v).PERS_NO + " )";

                                    return EmployeeName;
                                })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 8,
                                    editable: false, isMultiline: false,
                                    minSize: new go.Size(10, 10),
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "EMP_POSITION_TEXT").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 0, columnSpan: 4,
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Left
                                },
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR : " + v; })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 3, columnSpan: 4,
                                    margin: new go.Margin(0, 10, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Right
                                },
                                new go.Binding("text", "SOC_COUNT", function (v) { return "SOC :" + v; }))
                        )  // end Table Panel
                    ), // end Horizontal Panel

                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.DownArrow,
                            alignment: go.Spot.Bottom,
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))

                ), // end Vertical Panel
                {
                    selectionAdornmentTemplate:
                        $(go.Adornment, "Auto",
                            $(go.Shape, "RoundedRectangle",
                                { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                            $(go.Placeholder)
                        )  // end Adornment
                }
            );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "UNHCR") {

        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                            SetParentChildRelationship(e, selnode.data);
                        }
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),

                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.UpArrow,
                            visible: false
                        },
                        new go.Binding("visible", "key", isFirstElement)),
                    $(go.Panel, "Horizontal",
                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 6,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 16),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 6,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "POSITION_NUMBER").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 6,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 0, columnSpan: 6,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "GRADE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 0, columnSpan: 4,
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Left
                                },
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR : " + v; })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 3, columnSpan: 4,
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Right
                                },
                                new go.Binding("text", "SOC_COUNT", function (v) { return "SOC :" + v; }))
                        )  // end Table Panel
                    ), // end Horizontal Panel

                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.DownArrow,
                            alignment: go.Spot.Bottom,
                            margin: new go.Margin(-20, 4, 4, 4),
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))

                ), // end Vertical Panel
                {
                    selectionAdornmentTemplate:
                        $(go.Adornment, "Auto",
                            $(go.Shape, "RoundedRectangle",
                                { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                            $(go.Placeholder)
                        )  // end Adornment
                }
            );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "UNHCR_NO_NORSOC") {

        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
            { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                            SetParentChildRelationship(e, selnode.data);
                        }
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                    new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                    new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),

                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.UpArrow,
                            visible: false
                        },
                        new go.Binding("visible", "key", function (key) {
                            if (key == document.getElementById("hdnOrgShowLevel").value && myDiagram.model.findNodeDataForKey(key).parent != "999999")
                                return true;
                            else
                                return false;
                        })),
                    $(go.Panel, "Horizontal",
                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 6,
                                    font: "9pt Segoe UI,sans-serif",
                                    editable: false, isMultiline: true,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 2,
                                    textAlign: (Settings.SelectShape == "RoundedRectangle") ? "center" : "left",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: (Settings.SelectShape == "RoundedRectangle") ? "center" : "left",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "MSRP_POSITION_NBR").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: true,
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 2,
                                    textAlign: (Settings.SelectShape == "RoundedRectangle") ? "center" : "left",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "MSRP_FUNC_GP_L1_DESCR").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    margin: new go.Margin(0, 0, 0, 3),
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: (Settings.SelectShape == "RoundedRectangle") ? "center" : "left",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "MSRP_PERSONAL_GRADE").makeTwoWay())
                        )  // end Table Panel
                    ), // end Horizontal Panel
                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.DownArrow,
                            alignment: go.Spot.Left,
                            margin: new go.Margin(-20, 4, 4, 4),
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))

                ), // end Vertical Panel
                {
                    selectionAdornmentTemplate:
                        $(go.Adornment, "Auto",
                            $(go.Shape, "RoundedRectangle",
                                { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                            $(go.Placeholder)
                        )  // end Adornment
                }
            );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "UNHCR_COST") {
        myPartitionDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodePartitionDoubleClick },
                { // handle dragging a Node onto a Node to (maybe) change the reporting relationship
                    mouseDragEnter: function (e, node, prev) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();
                        if (!mayWorkFor(selnode, node)) return;
                        var shape = node.findObject("SHAPE");
                        if (shape) {
                            shape._prevFill = shape.fill;  // remember the original brush
                            shape.fill = "darkred";
                        }
                    },
                    mouseDragLeave: function (e, node, next) {
                        var shape = node.findObject("SHAPE");
                        if (shape && shape._prevFill) {
                            shape.fill = shape._prevFill;  // restore the original brush
                        }
                    },
                    mouseDrop: function (e, node) {
                        var diagram = node.diagram;
                        var selnode = diagram.selection.first();  // assume just one Node in selection
                        if (mayWorkFor(selnode, node)) {
                            // find any existing link into the selected node
                            var link = selnode.findTreeParentLink();
                            if (link !== null) {  // reconnect any existing link
                                link.fromNode = node;
                            } else {  // else create a new link
                                diagram.toolManager.linkingTool.insertLink(node, node.port, selnode, selnode.port);
                            }
                        }

                        SetParentChildRelationship(e, selnode.data);
                    }
                },
                // for sorting, have the Node.text be the data.name
                new go.Binding("text", "name"),
                // bind the Part.layerName to control the Node's layer depending on whether it isSelected
                new go.Binding("layerName", "isSelected", function (sel) { return sel ? "Foreground" : ""; }).ofObject(),
                // define the node's outer shape
                $(go.Shape, Settings.SelectShape,
                new go.Binding("strokeWidth", "DOTTED_LINE_FLAG", findStrokeWidthForDottedLine),
                new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG", findDottedLineReport),
                    {
                        name: "SHAPE", fill: "white", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    }),
                $(go.Panel, "Vertical",
                    $(go.Picture,
                        {
                            name: 'UpArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.UpArrow,
                            visible: false
                        },
                        new go.Binding("visible", "key", isFirstElement)),

                    $(go.Panel, "Horizontal",
                        // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.TopCenter
                            },
                            $(go.RowColumnDefinition,
                                { row: 3, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 6 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 6,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 16),
                                    stroke: Settings.TextColor,
                                    textAlign: "center"
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 6,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 6,
                                    editable: true, isMultiline: false,
                                    minSize: new go.Size(10, 14),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "GRADE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 0, columnSpan: 4,
                                    alignment: go.Spot.Left,
                                    stroke: Settings.TextColor
                                },
                                new go.Binding("text", "BUDGET_SALARY", function (v) { return "" + addCommas(parseFloat(v).toFixed(2).toLocaleString()); })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 4, columnSpan: 3,
                                    alignment: go.Spot.Right,
                                    stroke: Settings.TextColor,
                                }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "POSITION_CALCULATED_COST", function (v) { return "" + addCommas(parseFloat(v).toFixed(2).toLocaleString()); })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 0, columnSpan: 4,
                                    alignment: go.Spot.Left,
                                    stroke: Settings.TextColor
                                },
                                new go.Binding("text", "SOC_COUNT", function (v) { return "SOC: " + v; })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 4, columnSpan: 3,
                                    alignment: go.Spot.Right,
                                    stroke: Settings.TextColor
                                },
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR: " + v; }))
                        )  // end Table Panel
                    ), // end Horizontal Panel

                    $(go.Picture,
                        {
                            name: 'DownArrow',
                            desiredSize: new go.Size(25, 15),
                            source: Settings.DownArrow,
                            alignment: go.Spot.Bottom,
                            margin: new go.Margin(-20, 4, 4, 4),
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren))
                ),
                {
                    selectionAdornmentTemplate:
                        $(go.Adornment, "Auto",
                            $(go.Shape, "RoundedRectangle",
                                { fill: null, stroke: "dodgerblue", strokeWidth: 0 }),
                            $(go.Placeholder)
                        )  // end Adornment
                }  // end Vertical Panel
            );  // end Node
    }


    // define the Link template
    var dotted = [3, 3];
    var dashed = [5, 5];
    //myPartitionDiagram.linkTemplate =
    //    $(go.Link, go.Link.AvoidsNodes,  // may be either Orthogonal or AvoidsNodes
    //        { corner: 5, relinkableFrom: true, relinkableTo: true },
    //        $(go.Shape,
    //            {
    //                strokeWidth: 2,
    //                stroke: Settings.LineColor,
    //                strokeDashArray: []
    //            },
    //            new go.Binding("strokeDashArray", "DOTTED_LINE_FLAG",
    //                function (d) {
    //                    return d == "Y" ? dotted :
    //                        (d == "N" ? dashed : null);
    //                }).makeTwoWay()
    //        ));  // the link shape

    myPartitionDiagram.linkTemplate =
        $(go.Link, go.Link.AvoidsNodes,  // may be either Orthogonal or AvoidsNodes
            { corner: 5, relinkableFrom: true, relinkableTo: true },
            $(go.Shape,
                {
                    strokeWidth: 2,
                    stroke: Settings.LineColor,
                    strokeDashArray: []
                }
            ));  // the link shape

    // read in the JSON-format data from the "myPartitionSavedModel" element
    LoadPartition();

    // support editing the properties of the selected person in HTML
    if (window.Inspector) myInspector = new Inspector("myInspector", myPartitionDiagram,
        {
            properties: {
                "key": { readOnly: true },
                "comments": {}
            }
        });
}

// Show the diagram's model in JSON format
function SavePartition() {
    document.getElementById("myPartitionSavedModel").value = myPartitionDiagram.model.toJson();
    myPartitionDiagram.isModified = false;
}
function LoadPartition() {
    myPartitionDiagram.model = go.Model.fromJson(document.getElementById("myPartitionSavedModel").value);
}

// Show the diagram's model in JSON format
function loadPartitionJSON(strJson) {
    if (strJson) {
        document.getElementById("myPartitionSavedModel").value = strJson;
    }
    else document.getElementById("myPartitionSavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":\"\" }";

    myPartitionDiagram.model = go.Model.fromJson(document.getElementById("myPartitionSavedModel").value);
}
