
var ObjChange = [];
var ObjChangeType = [];

var SmallBoxWidth = 0;
function calculateShapeXDimention(shape, viewType) {

    switch (shape) {
        case "RoundedRectangle":
            return 280 - Settings.BoxWidth;
            break;

        case "Circle":
            return 220 - Settings.BoxWidth;
            break;

        case "Ellipse":
            return 240 - Settings.BoxWidth;
            break;
    }
    return 160;
}

function calculateShapeYDimention(shape) {
    switch (shape) {
        case "RoundedRectangle":
            return 90;
            break;

        case "Circle":
            return 120;
            break;

        case "Ellipse":
            return 120;
            break;
    }
    return 80;
}

//https://gojs.net/latest/intro/connectionPoints.html
//https://gojs.net/latest/extensions/ParallelRoute.html
//https://gojs.net/latest/intro/trees.html
//https://gojs.net/latest/intro/links.html
function init(w, h, DragDrop) {

    var shape_X_Dimention = calculateShapeXDimention(Settings.SelectShape);
    var shape_Y_Dimention = calculateShapeYDimention(Settings.SelectShape);

    if (window.goSamples) goSamples();  // init for these samples -- you don't need to call this
    var $ = go.GraphObject.make;        // for conciseness in defining templates
    go.Diagram.inherit(SideTreeLayout, go.TreeLayout);

    myDiagram =
        $(go.Diagram, "myDiagramDiv", // must be the ID or reference to div
        {
                initialDocumentSpot: go.Spot.TopCenter,
                initialContentAlignment: go.Spot.TopCenter,
                maxSelectionCount: 1, // users can select only one part at a time
                layout:
                    $(SideTreeLayout,
                        {
                            treeStyle: go.TreeLayout.StyleLastParents,
                            arrangement: go.TreeLayout.ArrangementVertical,
                            // properties for most of the tree:
                            angle: 90,
                            layerSpacing: 35,
                            breadthLimit: w,
                            // properties for the "last parents":
                            alternateAngle: 90,
                            alternateLayerSpacing: 35,
                            alternateAlignment: go.TreeLayout.AlignmentBus,
                            alternateNodeSpacing: 20

                            //treeStyle: go.TreeLayout.StyleLastParents,
                            //arrangement: go.TreeLayout.ArrangementVertical,
                            //// properties for most of the tree:
                            //angle: 90,
                            //layerSpacing: 20,
                            //nodeSpacing: 20,
                            //breadthLimit: w,
                            //// properties for the "last parents":
                            //alternateAngle: 90,
                            //alternateLayerSpacing: 35,
                            //alternateAlignment: go.TreeLayout.ArrangementVertical,
                            //alternateNodeSpacing: 20
                    }),
            "undoManager.isEnabled": true,  // enable undo & redo
            allowDragOut: DragDrop,
            allowDrop: DragDrop 
            });

    myDiagram.addModelChangedListener(function (evt) {

        // ignore unimportant Transaction events
        if (!evt.isTransactionFinished) return;
        var txn = evt.object;  // a Transaction
        if (txn === null) return;
        //console.log(txn);
        // iterate over all of the actual ChangedEvents of the Transaction
        txn.changes.each(function (e) {

            // record node insertions and removals
            if (e.change === go.ChangedEvent.Property) {
                if (e.modelChange === "linkFromKey") {
                    console.log(evt.propertyName + " changed From key of link: " +
                        e.object + " from: " + e.oldValue + " to: " + e.newValue);
                } else if (e.modelChange === "linkToKey") {
                    console.log(evt.propertyName + " changed To key of link: " +
                        e.object + " from: " + e.oldValue + " to: " + e.newValue);
                }

            } else if (e.change === go.ChangedEvent.Insert && e.modelChange === "linkDataArray") {
                console.log(evt.propertyName + " added link: " + e.newValue);
            } else if (e.change === go.ChangedEvent.Remove && e.modelChange === "linkDataArray") {
                console.log(evt.propertyName + " removed link: " + e.oldValue);
            }
        });
    });

    myDiagram.doFocus = function () {
        var x = window.scrollX || window.pageXOffset;
        var y = window.scrollY || window.pageYOffset;
        go.Diagram.prototype.doFocus.call(this);
        window.scrollTo(x, y);
    }

    // when the document is modified, add a "*" to the title and enable the "Save" button
    myDiagram.addDiagramListener("Modified", function (e) {

        var button = document.getElementById("SaveButton");
        if (button) button.disabled = !myDiagram.isModified;
        var idx = document.title.indexOf("*");
        if (myDiagram.isModified) {
            if (idx < 0) document.title += "*";
        } else {
            if (idx >= 0) document.title = document.title.substr(0, idx);
        }
    });

    myDiagram.addDiagramListener("ViewportBoundsChanged", function (e) {
        let allowScroll = !e.diagram.viewportBounds.containsRect(e.diagram.documentBounds);
        myDiagram.allowHorizontalScroll = false;
        myDiagram.allowVerticalScroll = false;
    });

    // manage boss info manually when a node or link is deleted from the diagram
    myDiagram.addDiagramListener("SelectionDeleting", function (e) {
        var part = e.subject.first(); // e.subject is the myDiagram.selection collection,
        // so we'll get the first since we know we only have one selection
        myDiagram.startTransaction("clear boss");
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
        myDiagram.commitTransaction("clear boss");
    });

    myDiagram.addDiagramListener("InitialLayoutCompleted", function (e) {
        var dia = e.diagram;

        // add height for horizontal scrollbar
        dia.div.style.height = (dia.documentBounds.height + 24) + "px";
    });

    var levelColors = ["#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#FFFFFF/#FFFFFF", "#A0A0A0/#A0A0A0", "#D3D3D3/#D3D3D3", "#FFF44F/#FFF44F"];
    Settings.TextColor = "black";
    Settings.UpArrow = HOST_ENV + "/Content/Images/uparrow.jpg";
    Settings.DownArrow = HOST_ENV + "/Content/Images/downarrow.jpg";
    Settings.BorderWidth = 3;
    if (Settings.Skin) {
        if (Settings.Skin.toUpperCase() == "BROWN") {
            levelColors = ["#634329/#634329", "#923222/#923222", "#e44c16/#e44c16", "#ec8026/#ec8026", "#fcaf17/#fcaf17", "#fed300/#fed300", "#D3D3D3/#D3D3D3", "#FFF44F/#FFF44F"];
            Settings.TextColor = "white";
            Settings.UpArrow = HOST_ENV + "/Content/Images/uparroww.png";
            Settings.DownArrow = HOST_ENV + "/Content/Images/downarrow.ico";
            //Settings.BorderColor = "cyan";
            Settings.BorderWidth = 0;
        }
    }

    // override TreeLayout.commitNodes to also modify the background brush based on the tree depth level
    myDiagram.layout.commitNodes = function () {
        go.TreeLayout.prototype.commitNodes.call(myDiagram.layout);  // do the standard behavior
        // then go through all of the vertexes and set their corresponding node's Shape.fill
        // to a brush dependent on the TreeVertex.level value
        myDiagram.layout.network.vertexes.each(function (v) {
            if (v.node) {
                var level = v.level % (levelColors.length);
                var colors = levelColors[level].split("/");
                var shape = v.node.findObject("SHAPE");

                if (shape) {
                    if (v.node.data.LEVEL_ID) {
                        if (v.node.data.LEVEL_ID == document.getElementById("hdnQueryStringSearchValue").value) {
                            colors = levelColors[7].split("/");
                        }
                    }
                    if (v.node.data.FULL_NAME) {
                        if (v.node.data.FULL_NAME.toUpperCase().indexOf("VACANT")!=-1) {
                            colors = levelColors[6].split("/");
                        }
                    }
                    if (v.node.data.GRAY_COLORED_FLAG) {
                        if (v.node.data.GRAY_COLORED_FLAG == "Y") {
                            colors = levelColors[5].split("/");
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
        while (myDiagram.model.findNodeDataForKey(key) !== null) {
            key = nodeIdCounter--;
        }
        return key;
    }

    function isBothRole(key) {

        // if Player Role 
        if (document.getElementById("hdnOrgRole").value == "Player" ||
            document.getElementById("hdnOrgRole").value == "Finalyzer") return true;

        return false;
    }

    function isPlayerRole(key) {
        // if Player Role 
        if (document.getElementById("hdnOrgRole").value == "Player") return true;

        return false;
    }

    function isFinalyzerRole(key) {
        // if Finalyzer Role
        if (document.getElementById("hdnOrgRole").value == "Finalyzer") return true;

        return false;
    }

    // Id first element but NOT CEO
    function isFirstElement(key) {
        if (key == $("#hdnQueryStringSearchValue").val()) {
            var nodeData = myDiagram.model.findNodeDataForKey(key);
            myDiagram.model.setDataProperty(nodeData, "color", "#e44c16");
        }
        if (key == document.getElementById("hdnOrgShowLevel").value && myDiagram.model.findNodeDataForKey(key).parent != "999999")
            return true;
        else
            return false;
    };

    // if not first element and has children then return true.
    function hasChildren(key) {
        
        // if first element 
        if (key == document.getElementById("hdnOrgShowLevel").value) return false;

        //if has children
        //alert(key + ":" + myDiagram.model.findNodeDataForKey(key).SOC_COUNT.toString() + ":" + (myDiagram.model.findNodeDataForKey(key).SOC_COUNT > 0).toString());
        return myDiagram.model.findNodeDataForKey(key).SOC_COUNT > 0 ? true : false;
    };

    // Show Multiple value.
    function ShowSupervisoryName(key) {

        var SupervisoryName = myDiagram.model.findNodeDataForKey(key).SUPERVISORYORG_TEXT;
        SupervisoryName += "( " + myDiagram.model.findNodeDataForKey(key).SUPERVISORY_ORG + " )";
        alert(SupervisoryName);

        return SupervisoryName;
    };

    function ShowEmployeeName(key) {

        var EmployeeName = myDiagram.model.findNodeDataForKey(key).EMP_NAME;
        EmployeeName += "( " + myDiagram.model.findNodeDataForKey(key).PERS_NO + " )";

        return EmployeeName;
    };

    var nodeIdCounter = -1; // use a sequence to guarantee key uniqueness as we add/remove/modify nodes

    // when a node is double-clicked
    function nodeDoubleClick(e, obj) {

        if (obj.data.parent != "999999") {
            var element = document.querySelector('.overlay');
            element.style.display = 'block';

            var URL = HOST_ENV + "/Version/ChangeShowLevel";
            var JsonData = {
                ShowLevel: obj.data.LEVEL_ID,
                ParentLevel: obj.data.PARENT_LEVEL_ID
            };

            jQuery.ajax({
                type: "POST",
                url: URL,
                data: JsonData,
                async: true,
                dateType: "json",
            }).done(function (JsonString) {
                if (JsonString.Message != "No Changes") {
                    document.getElementById("hdnOrgChartData").value = JsonString.ChartData;
                    document.getElementById("hdnOrgTreeData").value = JsonString.TreeData;
                    document.getElementById("hdnOrgShowLevel").value = JsonString.UsedShowLevel;
                    loadJSON("{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + JsonString.ChartData + " }", JsonString.ChartData);

                    myDiagram.startTransaction("properties");

                    var dia = e.diagram;

                    // add height for horizontal scrollbar
                    dia.div.style.height = (dia.documentBounds.height + 24) + "px";

                    myDiagram.commitTransaction("properties");

                    BreadGram();
                    ShowHierarchy();
                }
                element.style.display = 'none';
            });
        }
    }

    // when a node is clicked
    function nodeClick(e, obj) {
        var element = document.querySelector('.overlay');
        element.style.display = 'block';

        ShowEmployeeInformation(obj.data.key, isBothRole(obj.data.key) ? "Yes" : "No", obj.data.LEVEL_ID);

        element.style.display = 'none';
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

    // This converter is used by the Picture.
    function findHeadShot(key) {
        if (key < 0 || key > 100000006) return "images/HSnopic.png"; // There are only 16 images on the server
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/" + (currentdate.getMonth() + 1) + "/" + currentdate.getFullYear() +
            " @ " + currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds();

        return HOST_ENV + "/Content/Images/PHOTOS/" + key + ".jpg?" + datetime;
    }

    function SetParentChildRelationship(e, SelectNodeObj, NodeObj) {

        //alert(NodeObj.LEVEL_ID + ":" + NodeObj.PARENT_LEVEL_ID);
        //alert(SelectNodeObj.LEVEL_ID + ":" + SelectNodeObj.PARENT_LEVEL_ID);
        if (document.getElementById("hdnOrgRole").value.toUpperCase() == "PLAYER" ||
            document.getElementById("hdnOrgRole").value.toUpperCase() == "FINALYZER") {
            var element = document.querySelector('.overlay');
            element.style.display = 'block';

            ObjChangeType = [];
            ObjChange = [];

            var Obj = myDiagram.findNodeForKey(SelectNodeObj.parent);
            SelectNodeObj.SUP_DISPLAY_NAME = Obj.data.FULL_NAME;
            ObjChange.push(SelectNodeObj);
            ObjChangeType.push("C");

            var JsonData = {
                VersionData: JSON.stringify(ObjChange),
                MoveToParent: NodeObj.LEVEL_ID,
                ChangeType: ObjChangeType,
                OperType: document.getElementById("hdnOrgType").value
            };

            jQuery.ajax({
                type: "POST",
                url: HOST_ENV + "/Version/SaveVersion",
                data: JsonData,
                async: true,
                dateType: "json",
            }).done(function (Json) {
                if (Json.Success == "Success" || Json.Success == "Failure") {
                    document.getElementById("hdnOrgChartData").value = Json.ChartData;
                    document.getElementById("hdnOrgTreeData").value = Json.TreeData;
                    document.getElementById("hdnOrgShowLevel").value = Json.UsedShowLevel;
                    document.getElementById("hdnOrgVersion").value = Json.UsedVersion;

                    loadJSON("{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + Json.ChartData + " }", Json.ChartData);

                    myDiagram.startTransaction("properties");

                    var dia = e.diagram;

                    // add height for horizontal scrollbar
                    dia.div.style.height = (dia.documentBounds.height + 24) + "px";

                    myDiagram.commitTransaction("properties");

                    ShowMessage(Json.Success, Json.ShowMessage);
                    ShowHierarchy();
                }
                element.style.display = 'none';
            });
        }
        else {
            alert("Only FINALYER/PLAYER can change the relationship");
            loadJSON("{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + document.getElementById("hdnOrgChartData").value + " }",
                     document.getElementById("hdnOrgChartData").value);

            myDiagram.startTransaction("properties");

            var dia = e.diagram;

            // add height for horizontal scrollbar
            dia.div.style.height = (dia.documentBounds.height + 24) + "px";

            myDiagram.commitTransaction("properties");
        }
    }

    // This converts dotted line to a different shape
    function findLinkColorForLevelPortrait(key) {
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
    
    if (document.getElementById("hdnOrgView").value == "Normal") {
      
        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodeDoubleClick },
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
                            SetParentChildRelationship(e, selnode.data, node.data);
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
                            new go.Binding("text", "SOC_COUNT", function (v) { return "SOC: " + v; })),
                        $(go.TextBlock, textStyle(),
                            { name: "boss", row: 3, column: 3, stroke: Settings.TextColor }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                            new go.Binding("text", "NOR_COUNT", function (v) { return "NOR: " + v; })),
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
    else if (document.getElementById("hdnOrgView").value == "Mcbitss") {

        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodeDoubleClick },
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
                            SetParentChildRelationship(e, selnode.data, node.data);
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
                                new go.Binding("text", "key", function (v) { return "Corporate No: "+v; })),
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
    else if (document.getElementById("hdnOrgView").value == "Cost") {
        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodeDoubleClick },
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

                        SetParentChildRelationship(e, selnode.data, node.data);
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

        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodeDoubleClick },
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
                            SetParentChildRelationship(e, selnode.data, node.data);
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
                        name: "SHAPE", stroke: Settings.BorderColor,
                        // set the port properties:
                        portId: "", fromLinkable: false, toLinkable: false, cursor: "pointer"
                    },
                    new go.Binding("fill", "key", function (key) {
                        if (key == document.getElementById("hdnQueryStringSearchValue").value) {
                            return "#F1F1F1";
                        }
                        return "#F1F1F1";
                    })),

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
                                margin: new go.Margin(4, 4, 4, 4),
                                defaultAlignment: go.Spot.Left
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 8,
                                    font: "6pt Segoe UI,sans-serif",
                                    editable: false, isMultiline: false,
                                    minSize: new go.Size(10, 10),
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.TopCenter
                                },
                                new go.Binding("text", "key", function (v) {
                                    var SupervisoryName = myDiagram.model.findNodeDataForKey(v).SUPERVISORYORG_TEXT;
                                    SupervisoryName += "( " + myDiagram.model.findNodeDataForKey(v).SUPERVISORY_ORG + " )";

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
                                    var EmployeeName = myDiagram.model.findNodeDataForKey(v).EMP_NAME;
                                    EmployeeName += "( " + myDiagram.model.findNodeDataForKey(v).PERS_NO + " )";

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
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Left
                                },
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR : " + v; })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 4, columnSpan: 4,
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
                            margin: new go.Margin(-25, 4, 4, 4),
                            visible: false
                        },
                        new go.Binding("visible", "key", hasChildren)) // end Horizontal Panel
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

        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodeDoubleClick },
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
                            SetParentChildRelationship(e, selnode.data, node.data);
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
                    $(go.Panel, "Vertical",
                       // define the panel where the text will appear
                        $(go.Panel, "Table",
                            {
                                desiredSize: new go.Size(shape_X_Dimention, shape_Y_Dimention),
                                defaultAlignment: go.Spot.TopCenter
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 6,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "POSITION_NUMBER").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "GRADE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 0, columnSpan: 6,
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Left
                                },
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR : " + v; })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 3, columnSpan: 6,
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

                )
            );  // end Node
    }
    else if (document.getElementById("hdnOrgView").value == "UNHCR_NO_NORSOC") {
        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
            new go.Binding("location", "loc", go.Point.parse),
                { doubleClick: nodeDoubleClick },
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
                            SetParentChildRelationship(e, selnode.data, node.data);
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
                        portId: "", fromLinkable: true, toLinkable: true, cursor: "pointer"
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
                                    textAlign: (Settings.SelectShape =="RoundedRectangle")?"center":"left",
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
        myDiagram.nodeTemplate =
            $(go.Node, "Auto",
                { doubleClick: nodeDoubleClick },
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

                        SetParentChildRelationship(e, selnode.data, node.data);
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
                                defaultAlignment: go.Spot.TopCenter
                            },
                            $(go.RowColumnDefinition,
                                { row: 1, separatorStrokeWidth: 1, separatorStroke: "orange", separatorPadding: 2 }),
                            $(go.TextBlock, textStyle(),  // the name
                                {
                                    row: 0, column: 0, columnSpan: 6,
                                    font: "12pt Segoe UI,sans-serif",
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "FULL_NAME").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 1, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "POSITION_NUMBER").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 2, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "TITLE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 3, column: 0, columnSpan: 6,
                                    editable: false, isMultiline: false,
                                    stroke: Settings.TextColor,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 1,
                                    textAlign: "center",
                                    width: 280 - Settings.BoxWidth
                                },
                                new go.Binding("text", "GRADE").makeTwoWay()),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 0, columnSpan: 6,
                                    alignment: go.Spot.Left,
                                    stroke: Settings.TextColor
                                },
                                new go.Binding("text", "BUDGET_SALARY", function (v) { return "" + addCommas(parseFloat(v).toFixed(2).toLocaleString()); })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 4, column: 3, columnSpan: 6,
                                    alignment: go.Spot.Right,
                                    stroke: Settings.TextColor,
                                }, // we include a name so we can access this TextBlock when deleting Nodes/Links
                                new go.Binding("text", "POSITION_CALCULATED_COST", function (v) { return "" + addCommas(parseFloat(v).toFixed(2).toLocaleString()); })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 5, column: 0, columnSpan: 6,
                                    stroke: Settings.TextColor,
                                    alignment: go.Spot.Left
                                },
                                new go.Binding("text", "NOR_COUNT", function (v) { return "NOR : " + v; })),
                            $(go.TextBlock, textStyle(),
                                {
                                    row: 5, column: 3, columnSpan: 6,
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

    // Assume that the SideTreeLayout determines whether a node is an "assistant" if a particular data property is true.
    // You can adapt this code to decide according to your app's needs.
    function isAssistant(n) {
        if (n === null) return false;
        return n.data.isAssistant;
    }


    // This is a custom TreeLayout that knows about "assistants".
    // A Node for which isAssistant(n) is true will be placed at the side below the parent node
    // but above all of the other child nodes.
    // An assistant node may be the root of its own subtree.
    // An assistant node may have its own assistant nodes.
    function SideTreeLayout() {
        go.TreeLayout.call(this);
    }
    
    SideTreeLayout.prototype.makeNetwork = function (coll) {
        var net = go.TreeLayout.prototype.makeNetwork.call(this, coll);
        // copy the collection of TreeVertexes, because we will modify the network
        var vertexcoll = new go.Set(/*go.TreeVertex*/);
        vertexcoll.addAll(net.vertexes);
        for (var it = vertexcoll.iterator; it.next();) {
            var parent = it.value;
            // count the number of assistants
            var acount = 0;
            var ait = parent.destinationVertexes;
            while (ait.next()) {
                if (isAssistant(ait.value.node)) acount++;
            }
            // if a vertex has some number of children that should be assistants
            if (acount > 0) {
                // remember the assistant edges and the regular child edges
                var asstedges = new go.Set(/*go.TreeEdge*/);
                var childedges = new go.Set(/*go.TreeEdge*/);
                var eit = parent.destinationEdges;
                while (eit.next()) {
                    var e = eit.value;
                    if (isAssistant(e.toVertex.node)) {
                        asstedges.add(e);
                    } else {
                        childedges.add(e);
                    }
                }
                // first remove all edges from PARENT
                eit = asstedges.iterator;
                while (eit.next()) { parent.deleteDestinationEdge(eit.value); }
                eit = childedges.iterator;
                while (eit.next()) { parent.deleteDestinationEdge(eit.value); }
                // if the number of assistants is odd, add a dummy assistant, to make the count even
                if (acount % 2 == 1) {
                    var dummy = net.createVertex();
                    net.addVertex(dummy);
                    net.linkVertexes(parent, dummy, asstedges.first().link);
                }
                // now PARENT should get all of the assistant children
                eit = asstedges.iterator;
                while (eit.next()) {
                    parent.addDestinationEdge(eit.value);
                }
                // create substitute vertex to be new parent of all regular children
                var subst = net.createVertex();
                net.addVertex(subst);
                // reparent regular children to the new substitute vertex
                eit = childedges.iterator;
                while (eit.next()) {
                    var ce = eit.value;
                    ce.fromVertex = subst;
                    subst.addDestinationEdge(ce);
                }
                // finally can add substitute vertex as the final odd child,
                // to be positioned at the end of the PARENT's immediate subtree.
                var newedge = net.linkVertexes(parent, subst, null);
            }
        }
        return net;
    };

    SideTreeLayout.prototype.assignTreeVertexValues = function (v) {
        // if a vertex has any assistants, use Bus alignment
        var any = false;
        var children = v.children;
        for (var i = 0; i < children.length; i++) {
            var c = children[i];
            if (isAssistant(c.node)) {
                any = true;
                break;
            }
        }
        if (any) {
            // this is the parent for the assistant(s)
            v.alignment = go.TreeLayout.AlignmentBus;  // this is required
            v.nodeSpacing = 50; // control the distance of the assistants from the parent's main links
        } else if (v.node == null && v.childrenCount > 0) {
            // found the substitute parent for non-assistant children
            v.alignment = go.TreeLayout.AlignmentCenterChildren;
            v.breadthLimit = 9000;
            v.layerSpacing = 0;
        }
    };

    SideTreeLayout.prototype.commitLinks = function () {
        go.TreeLayout.prototype.commitLinks.call(this);
        // make sure the middle segment of an orthogonal link does not cross over the assistant subtree
        var eit = this.network.edges.iterator;
        while (eit.next()) {
            var e = eit.value;
            if (e.link == null) continue;
            var r = e.link;
            // does this edge come from a substitute parent vertex?
            var subst = e.fromVertex;
            if (subst.node == null && r.routing == go.Link.Orthogonal) {
                r.updateRoute();
                r.startRoute();
                // middle segment goes from point 2 to point 3
                var p1 = subst.center;  // assume artificial vertex has zero size
                var p2 = r.getPoint(2).copy();
                var p3 = r.getPoint(3).copy();
                var p5 = r.getPoint(r.pointsCount - 1);
                var dist = 10;
                if (subst.angle == 270 || subst.angle == 180) dist = -20;
                if (subst.angle == 90 || subst.angle == 270) {
                    p2.y = p5.y -dist; // (p1.y+p5.y)/2;
                    p3.y = p5.y - dist; // (p1.y+p5.y)/2;
                } else {
                    p2.x = p5.x - dist; // (p1.x+p5.x)/2;
                    p3.x = p5.x - dist; // (p1.x+p5.x)/2;
                }
                r.setPoint(2, p2);
                r.setPoint(3, p3);
                r.commitRoute();
            }
        }
    };  // end of SideTreeLayout

    // the context menu allows users to make a position vacant,
    // remove a role and reassign the subtree, or remove a department
    myDiagram.nodeTemplate.contextMenu =
        $(go.Adornment, "Vertical",
            $("ContextMenuButton",
                $(go.TextBlock, "Show Employee Information"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            var element = document.querySelector('.overlay');
                            element.style.display = 'block';
                            ShowEmployeeInformation(thisemp.key, isBothRole(thisemp.key)?"Yes":"No", thisemp.LEVEL_ID);
                            element.style.display = 'none';
                        }
                    },
                    visible: true
                }
            ),
            $("ContextMenuButton",
                $(go.TextBlock, "Functional Manager"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            var element = document.querySelector('.overlay');
                            element.style.display = 'block';
                            AddFunctionalManager(thisemp.key, thisemp.PARENT_LEVEL_ID, myDiagram.model.findNodeDataForKey(thisemp.PARENT_LEVEL_ID).FULL_NAME);
                            element.style.display = 'none';
                        }
                    },
                    visible: false
                },
                new go.Binding("visible", "key", isBothRole)
            ),
            $("ContextMenuButton",
                $(go.TextBlock, "Outside Legal Entity"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            var element = document.querySelector('.overlay');
                            element.style.display = 'block';
                            ShowOutsideLegalEntity(thisemp.key, thisemp.PARENT_LEVEL_ID);
                            element.style.display = 'none';
                        }
                    },
                    visible: false
                },
                new go.Binding("visible", "key", isBothRole)
            ),
            $("ContextMenuButton",
                $(go.TextBlock, "New Position"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            AddNewPositionModal(thisemp.key);
                        }
                    },
                    visible: false
                },
                new go.Binding("visible", "key", isBothRole)
            ),
            $("ContextMenuButton",
                $(go.TextBlock, "Add Picture"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            AddNewImage(thisemp.key, thisemp.LEVEL_ID);

                        }
                    },
                    visible: false
                },
                new go.Binding("visible", "key", isFinalyzerRole)
            ),
            $("ContextMenuButton",
                $(go.TextBlock, "Check/UnCheck Assistance"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            CheckUnCheckAssistance(thisemp.LEVEL_ID, thisemp.PARENT_LEVEL_ID);

                        }
                    },
                    visible: false
                },
                new go.Binding("visible", "key", isFinalyzerRole)
            ),
            $("ContextMenuButton",
                $(go.TextBlock, "Sorting"),
                {
                    click: function (e, obj) {
                        var node = obj.part.adornedPart;
                        if (node !== null) {
                            var thisemp = node.data;
                            ChangeSortOrder(thisemp.LEVEL_ID);
                        }
                    },
                    visible: false
                },
                new go.Binding("visible", "key", isBothRole)
            ));

    // define the Link template
    myDiagram.linkTemplate =
        $(go.Link, go.Link.AvoidsNodes,  // may be either Orthogonal or AvoidsNodes
            { corner: 5, relinkableFrom: true, relinkableTo: true },
            $(go.Shape,
                {
                    strokeWidth: 2,
                    stroke: Settings.LineColor,
                    strokeDashArray: null
                },
                new go.Binding("strokeDashArray", "dashes")
            ));  // the link shape

    //myDiagram.linkTemplate =
    //    $(go.Link,
    //        $(go.Shape),
    //        $(go.Shape,   // the "from" end arrowhead
    //            { fromArrow: "Chevron" }),
    //        $(go.Shape,   // the "to" end arrowhead
    //            { toArrow: "StretchedDiamond", fill: "red" })
    //    );

    //diagram.linkTemplate =
    //    $(go.Link,
    //        new go.Binding("routing", "routing"),
    //        $(go.Shape),
    //        $(go.Shape, { toArrow: "Standard" })
    //    );    

    // read in the JSON-format data from the "mySavedModel" element
    loadPortrait();

    // support editing the properties of the selected person in HTML
    if (window.Inspector) myInspector = new Inspector("myInspector", myDiagram,
        {
            properties: {
                "key": { readOnly: true },
                "comments": {}
            }
        });
}

// Show the diagram's model in JSON format
function save() {
    document.getElementById("mySavedModel").value = myDiagram.model.toJson();
    myDiagram.isModified = false;
}

function GetLevelNo(nodeDataArray, LevelId) {
    for (var idx = 0; idx <= nodeDataArray.length - 1; idx++) {
        if (nodeDataArray[idx].LEVEL_ID == LevelId) return nodeDataArray[idx].LEVEL_NO;
    }

    return 99;
}

function ShowNodesInCanvas(InitialValues, nodeDataArray, OrgChartType) {
    var linkDataArray = [];
    if (nodeDataArray.length >= 1) {
        for (var idx = 0; idx <= nodeDataArray.length - 1; idx++) {
            if (OrgChartType == "OS") {
                if (nodeDataArray[idx].DOTTED_LINE_FLAG == "Y") {
                    if (InitialValues.FuntionalManagerDottedLines == "Yes" &&
                        document.getElementById("hdnSelectedFunctionalManagerType").value == "ShowFM") {
                        var item = {};
                        if (GetLevelNo(nodeDataArray, nodeDataArray[idx].LEVEL_ID) <= GetLevelNo(nodeDataArray, nodeDataArray[idx].PARENT_LEVEL_ID)) {
                            item["from"] = nodeDataArray[idx].LEVEL_ID;
                            item["to"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                        }
                        else {
                            item["from"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                            item["to"] = nodeDataArray[idx].LEVEL_ID;
                        }
                        item["dashes"] = [12, 12];
                        item["fromSpot"] = "Left";
                        item["toSpot"] = "Right";
                        linkDataArray.push(item);
                    }
                    else {
                        if (InitialValues.FuntionalManagerDottedLines == "No" &&
                            document.getElementById("hdnSelectedFunctionalManagerType").value == "ShowFM") {
                            var item = {};
                            item["from"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                            item["to"] = nodeDataArray[idx].LEVEL_ID;
                            linkDataArray.push(item);
                        }
                    }
                }
                else {
                    var item = {};
                    item["from"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                    item["to"] = nodeDataArray[idx].LEVEL_ID;
                    item["fromSpot"] = "Bottom";
                    item["toSpot"] = "Top";
                    linkDataArray.push(item);
                }
            }
            else if (OrgChartType == "OD") {
                if (nodeDataArray[idx].DOTTED_LINE_FLAG == "Y") {
                    if (InitialValues.FuntionalManagerDottedLines == "Yes" &&
                        document.getElementById("hdnSelectedFunctionalManagerType").value == "ShowFM") {
                        if ((document.getElementById("hdnOrgShowLevel").value == nodeDataArray[idx].LEVEL_ID ||
                            document.getElementById("hdnOrgShowLevel").value == nodeDataArray[idx].PARENT_LEVEL_ID) &&
                            document.getElementById("hdnSelectedPortraitModeMultipleLevel").value == "No") {
                            var item = {};
                            item["from"] = nodeDataArray[idx].LEVEL_ID;
                            item["to"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                            item["dashes"] = [12, 12];
                            linkDataArray.push(item);
                        }
                        else if (document.getElementById("hdnSelectedPortraitModeMultipleLevel").value == "Yes") {
                            var item = {};
                            item["from"] = nodeDataArray[idx].LEVEL_ID;
                            item["to"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                            item["dashes"] = [12, 12];
                            linkDataArray.push(item);
                        }
                    }
                    else {
                        if (InitialValues.FuntionalManagerDottedLines == "No" &&
                            document.getElementById("hdnSelectedFunctionalManagerType").value == "ShowFM") {
                            var item = {};
                            item["from"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                            item["to"] = nodeDataArray[idx].LEVEL_ID;
                            linkDataArray.push(item);
                        }
                    }
                }
                else {
                    if ((document.getElementById("hdnOrgShowLevel").value == nodeDataArray[idx].LEVEL_ID ||
                        document.getElementById("hdnOrgShowLevel").value == nodeDataArray[idx].PARENT_LEVEL_ID) &&
                        document.getElementById("hdnSelectedPortraitModeMultipleLevel").value == "No") {
                        var item = {};
                        item["from"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                        item["to"] = nodeDataArray[idx].LEVEL_ID;
                        item["fromSpot"] = "Bottom";
                        item["toSpot"] = "Top";
                        linkDataArray.push(item);
                    }
                    else if (document.getElementById("hdnSelectedPortraitModeMultipleLevel").value == "Yes") {
                        var item = {};
                        item["from"] = nodeDataArray[idx].PARENT_LEVEL_ID;
                        item["to"] = nodeDataArray[idx].LEVEL_ID;
                        item["fromSpot"] = "Bottom";
                        item["toSpot"] = "Top";
                        linkDataArray.push(item);
                    }
                }
            }
        }

        if (OrgChartType == "OS") {
            for (var idr = nodeDataArray.length - 1; idr >= 0; idr--) {
                if (nodeDataArray[idr].DOTTED_LINE_FLAG == "Y" &&
                    InitialValues.FuntionalManagerDottedLines == "Yes")
                    nodeDataArray.splice(idr, 1);
            }
        }
        else if (OrgChartType == "OD") {
            for (var idr = nodeDataArray.length - 1; idr >= 0; idr--) {
                if (!(document.getElementById("hdnOrgShowLevel").value == nodeDataArray[idr].LEVEL_ID ||
                    document.getElementById("hdnOrgShowLevel").value == nodeDataArray[idr].PARENT_LEVEL_ID) &&
                    document.getElementById("hdnSelectedPortraitModeMultipleLevel").value == "No") {
                    nodeDataArray.splice(idr, 1);
                }
                else if (nodeDataArray[idr].DOTTED_LINE_FLAG == "Y" &&
                    InitialValues.FuntionalManagerDottedLines == "Yes") {
                    nodeDataArray.splice(idr, 1);
                }
            }
        }

        myDiagram.model = new go.GraphLinksModel(nodeDataArray, linkDataArray);
    }
}

function loadPortrait() {

    var InitialValues = JSON.parse(document.getElementById("hdnInitialValues").value);
    if (InitialValues.FuntionalManagerDottedLines == "Yes") {
        var nodeDataArray = JSON.parse(document.getElementById("hdnOrgChartData").value);
        var OrgChartType = document.getElementById("hdnOrgChartType").value;

        ShowNodesInCanvas(InitialValues, nodeDataArray, OrgChartType);
    }
    else {
        myDiagram.model = go.Model.fromJson(document.getElementById("mySavedModel").value);
    }
}

// Show the diagram's model in JSON format
function loadJSON(SaveJson, JsonString) {

    if (SaveJson) {
        document.getElementById("mySavedModel").value = SaveJson;
    }
    else document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":\"\" }";

    var InitialValues = JSON.parse(document.getElementById("hdnInitialValues").value);
    if (InitialValues.FuntionalManagerDottedLines == "Yes") {
        var nodeDataArray = JSON.parse(JsonString);
        var OrgChartType = document.getElementById("hdnOrgChartType").value;

        ShowNodesInCanvas(InitialValues, nodeDataArray, OrgChartType);
    }
    else {
        myDiagram.model = go.Model.fromJson(document.getElementById("mySavedModel").value);
    }

}
