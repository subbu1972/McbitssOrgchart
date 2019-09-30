var DataPPTX = 0;
var SplitScreen = "Y";
var SetSplitScreen = "N";
var header = document.getElementById("viewToggle");
var btns = header.getElementsByClassName("btn");

for (var Idx = 0; Idx < btns.length; Idx++) {
    btns[Idx].addEventListener("click", function () {
        var current = document.getElementsByClassName("active-class");
        current[0].className = current[0].className.replace(" active-class", "");
        this.className += " active-class";
    });
}

var HOST_ENV = ReadConfigurationSettings();
var Settings = {
    SelectShape: "RoundedRectangle",
    Skin: "white",
    ShowPicture: "Yes",
    SelectView: $("#selView").val(),
    SplitScreen: "Yes",
    SplitScreenDirection: "Vertical",
    TextColor: "black",
    UpArrow: HOST_ENV + "/Content/Images/uparrow.jpg",
    DownArrow: HOST_ENV + "/Content/Images/downarrow.jpg",
    BorderColor: "cyan",
    BorderWidth: 3,
    LineColor: "#634329",
    OrgChartType: "OD",
    BoxWidth: 0,
    PortraitModeMultipleLevel: "Yes",
    FunctionalManagerType: "ShowFM"
};

var zTreeSettings = {
    view: {
        dblClickExpand: true,
        showLine: true,
        selectedMulti: false
    },
    check: {
        enable: false
    },
    callback: {
        onCheck: OnCheck,
        onClick: OnClick
    },
    data: {
        simpleData: {
            enable: true,
            idkey: "LEVEL_ID",
            pIdkey: "PARENT_LEVEL_ID"
        }
    }
};

var code;

function setCheck(trName) {
    var zTree = $.fn.zTree.getZTreeObj(trName),
        py = $("#py").attr("checked") ? "p" : "",
        sy = $("#sy").attr("checked") ? "s" : "",
        pn = $("#pn").attr("checked") ? "p" : "",
        sn = $("#sn").attr("checked") ? "s" : "",
        type = { "Y": py + sy, "N": pn + sn };
    zTree.setting.check.chkboxType = type;
    showCode('setting.check.chkboxType = { "Y" : "' + type.Y + '", "N" : "' + type.N + '" };');
}

function showCode(str) {
    if (!code) code = $("#code");
    code.empty();
    code.append("<li>" + str + "</li>");
}

function OnCheck(e, treeId, treeNode) {
    alert(treeNode.name.toUpperCase() + ":" + treeNode.id.toUpperCase());
}

function OnClick(e, treeId, treeNode) {
    //alert(treeNode.name.toUpperCase() + ":" + treeNode.id.toUpperCase() + ":" + treeNode.pId.toUpperCase());

    $(".overlay").show();

    var JsonData = {
        ShowLevel: treeNode.id.toUpperCase(),
        ParentLevel: treeNode.pId.toUpperCase()
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/ChangeShowLevel",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            try {
                $("#div_OA_ShowChart").show();
                $("#div_OA_ShowUploadData").hide();
            }
            catch (ex) {
                Console.log(ex);
            }
            $("#hdnOrgTreeData").val(Json.TreeData);
            $("#hdnOrgChartData").val(Json.ChartData);
            $("#hdnOrgChartHRCoreData").val(Json.ChartData);
            $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgVersion").val(Json.UsedVersion);
            $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            CancelOperation();
            $(".overlay").hide();
        }
    });

    return false;
}

function ShowMessage(Type, Message) {
    if (Type == "Success") {
        $("#divMessage").html("<center><div>Success: " + Message + "</div></center>");
        $("#divMessage").fadeIn("slow");
        if ($("#divMessage").hasClass("centererror-div")) {
            $("#divMessage").removeClass("centererror-div");
            $("#divMessage").addClass("centermessage-div");
        }
    }
    else if (Type == "Failure") {
        $("#divMessage").html("<center><div>Error: " + Message + "</div></center>");
        $("#divMessage").fadeIn("slow");
        if ($("#divMessage").hasClass("centermessage-div")) {
            $("#divMessage").removeClass("centermessage-div");
            $("#divMessage").addClass("centererror-div");
        }
    }
    setTimeout(function () {
        $("#divMessage").fadeOut("slow");
    }, 5000);
}

function GetQueryStringValue(key) {
    return decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}

function ShowHideDropDownBar(Obj) {
    $(Obj).blur();
    if ($('#divReOrgUsers').is(':visible'))
        $('#divReOrgUsers').fadeOut();
    else
        $('#divReOrgUsers').fadeIn();
}

/* Set the width of the side navigation to 0 */
function closeNav() {
    $("#mySidenav").css("width", "40px");
    $("#mySidenav").removeClass("open");
    $("#mySidenav").addClass("closed");
    $("#divHelpData").css("marginLeft", "0%");
    $("#treeDemo").hide();
}

function DownloadInitiative() {
    window.location.href = HOST_ENV + "/Version/DownLoadInitiative";
}

function DownloadVersion() {
    window.location.href = HOST_ENV + "/Version/DownLoadVersion";
}

function SelectTempate(Obj, FileName) {
    if ($(Obj).prop("checked"))
        $("#hdnTemplatePPT").val(FileName);
    else
        $("#hdnTemplatePPT").val("");
}

function ShowTemplateFields(Value) {
    TemplateData = JSON.parse($("#hdnPPTXinfo").val());
    if (TemplateData.length >= 1) {
        for (var Idx = 0; Idx <= TemplateData.length - 1; Idx++) {
            if (TemplateData[Idx].TemplateName == Value) {
                var JsonOrgSelectFields = JSON.parse(TemplateData[Idx].FieldsInfo);
                if (JsonOrgSelectFields.length >= 1) {
                    var html = ""
                    for (var Idx = 0; Idx <= JsonOrgSelectFields.length - 1; Idx++) {
                        html += "<div class=\"col-md-4\">";
                        html += "    <label class=\"label\">" + JsonOrgSelectFields[Idx].SelectedField + " ( " + JsonOrgSelectFields[Idx].MappedFields + " )</label>";
                        html += "</div>";
                    }
                    $("#divFields").html(html);
                }
            }
        }
    }
}

function RemoveVersion(Obj, Value) {
    var RemoveEntity = "";

    if ($(Obj).attr("data-id") == "deleteinitiative") {
        if (confirm("Do you want to delete Initiative(" + Value + ")!")) {
            RemoveEntity = "Initiative";
        }
    }
    else if ($(Obj).attr("data-id") == "deletepopulation") {
        if (confirm("Do you want to delete Population(" + Value + ")!")) {
            RemoveEntity = "Population";
        }
    }
    else if ($(Obj).attr("data-id") == "deleteversion") {
        if (confirm("Do you want to delete Version(" + Value + ")!")) {
            RemoveEntity = "Version";
        }
    }

    if (RemoveEntity != "") {
        $(".overlay").show();
        var JsonData = {
            RE: RemoveEntity,
            EV: Value
        };
        $.ajax({
            type: "POST",
            url: HOST_ENV + "/Version/RemoveEntities",
            data: JsonData,
            async: true,
            dateType: "json",
            success: function (Json) {
                if (Json.Success == "Success") {
                    $("#hdnOrgDDL").val(Json.DDL);
                    $("#hdnSelectedInitiative").val(Json.Initiative);

                    SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#hdnSelectedInitiative").val(), "N");
                    if ($("#hdnOrgRole").val() == "Player" || $("#hdnOrgRole").val() == "Admin")
                        SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#hdnSelectedInitiative").val(), "Y");

                    $("#selInitiative").select2("val", Json.Initiative);
                    $("#selPopulation").select2("val", Json.Population);
                    $("#selUser").select2("val", Json.UserName);
                    $("#selVersion").select2("val", Json.Version);

                    $("#hdnOrgChartData").val(Json.ChartData);
                    $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                    $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
                    $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
                    $("#hdnOrgVersion").val(Json.UsedVersion);
                    $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
                    document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                    CancelOperation();
                    ShowMessage(Json.Success, Json.ShowMessage);
                }
                else if (Json.Success == "Failure") {
                    ShowMessage(Json.Success, Json.ShowMessage);
                }

                $(".overlay").hide();
            }
        });
    }
}

function DownAllLevelPDF() {
    window.location.href = HOST_ENV + '/Version/DownloadAllPDF'
}

var DownLoadId = "";
function ShowSelectFields(Obj) {
    $(".overlay").show();

    $("#divFields").empty();
    DownLoadId = $(Obj).attr("id");
    if ($(Obj).attr("id") == "divDownloadPDF" || $(Obj).attr("id") == "divDownloadMLPDF") {
        $("#divFieldSelection").show();
        $("#divFields").html("");

        var JsonData = JSON.parse($("#hdnOrgSelectFields").val());
        if (JsonData.length >= 1) {
            var html = "";
            for (var Idx = 0; Idx <= JsonData.length - 1; Idx++) {
                var CheckedFlag = "";
                if (JsonData[Idx].ACTIVE_IND == "Y") CheckedFlag = "checked";
                html += "<div class=\"col-md-4\">" +
                    "<input type =\"checkbox\" id=\"FieldSet" + Idx.toString() + "\" class=\"radio\" name=\"FieldSet\" style=\"vertical-align: text-top\" " + CheckedFlag + " value=\"" + JsonData[Idx].FIELD_NAME + "\" />" +
                    "<label class=\"label\" for=\"FieldSet" + Idx.toString() + "\">" + JsonData[Idx].FIELD_CAPTION + "</label>" +
                    "</div>";
            }
            $("#divFields").html(html);
        }
        $("#divTemplateSelection").hide();
        $("#btnDownLoadSave").html("Save");
        $(".overlay").hide();
    }
    else if ($(Obj).attr("id") == "divDownloadAllPDF") {
        $("#divFieldSelection").show();
        $("#divFields").html("");

        var PDFTemplateView =
            "<div class=\"col-md-12\" style=\"margin:10px;\">" +
            "  <div class=\"row\">" +
            "    <div class=\"col-md-2\">" +
            "       <input type=\"radio\" id=\"rdoPDFHorizontal\" name=\"PDFView\" value=\"Horizontal\" />" +
            "       <label for=\"rdoPDFHorizontal\">Horizantal<label/>" +
            "    </div>" +
            "    <div class=\"col-md-2\">" +
            "       <input type=\"radio\" id=\"rdoPDFVertical\" name=\"PDFView\" value=\"Vertical\" />" +
            "       <label for=\"rdoPDFVertical\">Vertical<label/>" +
            "    </div>" +
            "    <div class=\"col-md-3\">" +
            "       <input type=\"radio\" id=\"rdoPDFVerticalSL\" name=\"PDFView\" value=\"Multiple Pages\" />" +
            "       <label for=\"rdoPDFVerticalSL\">Multiple Pages<label/>" +
            "    </div>" +
            "    <div class=\"col-md-2\">" +
            "       <input type=\"checkbox\" id=\"chkPDFOneLevelUp\" name=\"PDFViewOLU\" value=\"One Level Up\" />" +
            "       <label for=\"chkPDFOneLevelUp\">One Level Up<label/>" +
            "    </div>" +
            "    <div class=\"col-md-2\">" +
            "       <input type=\"checkbox\" id=\"chkPDFCurrentLevel\" name=\"PDFViewCL\" value=\"Current Level\" />" +
            "       <label for=\"chkPDFCurrentLevel\">Current Level<label/>" +
            "    </div>" +
            "  </div>" +
            "</div>";

        $("#divFields").html(PDFTemplateView);
        $("#divTemplateSelection").hide();
        $("#btnDownLoadSave").html("Ok");
        $(".overlay").hide();
    }
    else if ($(Obj).attr("id") == "divDownloadPPT") {
        $("#divFieldSelection").hide();
        $("#divFields").html("");

        var templatedata = "";
        $.ajax({
            type: "post",
            url: HOST_ENV + "/Version/ShowTemplates",
            async: true,
            datetype: "json",
            success: function (json) {
                if (json.Success == "Yes") {
                    $("#hdnPPTXinfo").val(json.SF);
                    templatedata = JSON.parse(json.SF);
                    $("#divTemplateData").empty();

                    if (templatedata.length >= 1) {

                        var PPTXTemplatehtml = "";
                        PPTXTemplatehtml += "<div class=\"col-md-12\">" +
                            "<div class=\"row\">" +
                            "<div class=\"col-md-3\">" +
                            "<strong>TemplateName</strong>" +
                            "</div>" +
                            "<div class=\"col-md-3\">" +
                            "<strong> Description </strong>" +
                            "</div>" +
                            "<div class=\"col-md-3\">" +
                            "<strong>FileName </strong>" +
                            "</div>" +
                            "</div>" +
                            "</div>";

                        var TemplateName = ""
                        for (var Idx = 0; Idx <= templatedata.length - 1; Idx++) {
                            var checkedflag = "";
                            if (templatedata[Idx].ACTIVE_IND == "Y") CheckedFlag = "checked";
                            PPTXTemplatehtml += "<div class=\"col-md-12\">" +
                                "<div class=\"row\">" +
                                "<div class=\"col-md-3\">" +
                                "<input type =\"radio\" name=\"TemplatePPTX\" " + CheckedFlag + " value=\"" + templatedata[Idx].TemplateName + "\" onclick=\"ShowTemplateFields('" + templatedata[Idx].TemplateName + "');\" />" +
                                "<label class=\"label\">" + templatedata[Idx].TemplateName + "</label>" +
                                "</div>" +
                                "<div class=\"col-md-3\">" +
                                "<label class=\"label\">" + templatedata[Idx].Description + "</label>" +
                                "</div>" +
                                "<div class=\"col-md-3\">" +
                                "<label class=\"label\">" + templatedata[Idx].FileName + "</label>" +
                                "</div>" +
                                "</div>" +
                                "</div>";
                            TemplateName = templatedata[Idx].TemplateName
                        }
                        ShowTemplateFields(TemplateName);
                        $("#divFieldSelection").show();
                        $("#divTemplateData").html(PPTXTemplatehtml);
                    }
                    $(".overlay").hide();
                }
            }
        });
        $("#divTemplateSelection").show();
        $("#btnDownLoadSave").html("Save");
    }
    else {
        $("#divTemplateSelection").hide();
        $(".overlay").hide();
    }

    $("#divFieldSelection").show();
    $("#btnDownLoadSave").show();
}

function SaveSelectedFields() {
    if (DownLoadId != "divDownloadAllPDF") {
        if (DownLoadId != "") {
            $(".overlay").show();
            $("#hdnTemplatePPT").val($('input[name=TemplatePPTX]:checked').val());

            var SelectedFields = "";
            $("input[name='FieldSet']").each(function () {
                if ($(this).prop("checked") == true) {
                    SelectedFields += ",\'" + $(this).val() + "\'";
                }
            });

            var JsonData = {
                SetSelectedFields: ((SelectedFields == "") ? "" : SelectedFields.substring(1)),
                TemplatePPTX: $("#hdnTemplatePPT").val()
            };
            $.ajax({
                type: "POST",
                url: HOST_ENV + "/Version/SaveSelectedFields",
                data: JsonData,
                async: true,
                dateType: "json",
                success: function (Json) {
                    if (Json.Success == "Yes") {
                        $("#hdnOrgSelectFields").val(Json.SelectFields);

                        if (DownLoadId == "divDownloadMLPDF") {
                            window.location.href = HOST_ENV + '/Version/DownloadMLPDF'
                        }
                        else if (DownLoadId == "divDownloadPDF") {
                            window.location.href = HOST_ENV + '/Version/DownloadPDF'
                        }
                        else if (DownLoadId == "divDownloadPPT") {
                            window.location.href = HOST_ENV + '/Version/DownloadPPT'
                        }
                    }
                    $(".overlay").hide();
                }
            });
        }
        else alert("Please select download PPT/PDF");
    }
    else if (DownLoadId == "divDownloadAllPDF") {
        var SelectedFields = "";
        $("input[name='PDFView']").each(function () {
            if ($(this).prop("checked") == true) {
                SelectedFields = $(this).val();
            }
        });
        if ($("#rdoPDFHorizontal").prop("checked")) {
            window.location.href = HOST_ENV + '/Version/DownloadAllPDF?ViewFlag=' + SelectedFields +
                '&LevelUp=' + ($("#chkPDFOneLevelUp").prop("checked") ? "Yes" : "No") +
                '&CurrentLevel=' + ($("#chkPDFCurrentLevel").prop("checked") ? "Yes" : "No");
        }
        else if ($("#rdoPDFVertical").prop("checked")) {
            window.location.href = HOST_ENV + '/Version/DownloadAllPDF?ViewFlag=' + SelectedFields +
                '&LevelUp=' + ($("#chkPDFOneLevelUp").prop("checked") ? "Yes" : "No") +
                '&CurrentLevel=' + ($("#chkPDFCurrentLevel").prop("checked") ? "Yes" : "No");
        }
        else if ($("#rdoPDFVerticalSL").prop("checked")) {
            window.location.href = HOST_ENV + '/Version/DownloadPDF?ViewFlag=' + SelectedFields +
                '&LevelUp=' + ($("#chkPDFOneLevelUp").prop("checked") ? "Yes" : "No") +
                '&CurrentLevel=' + ($("#chkPDFCurrentLevel").prop("checked") ? "Yes" : "No");
        }
    }
}

function AddNewImage(parent, levelID) {
    $("#divImageError").html("");
    $("#uploadImageContent").val("");
    $("#hdnImageFileName").val(levelID);
    $("#AddNewImage").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function ShowSettingsValue(ObjName, Value) {
    $("input[name='" + ObjName + "']").each(function () {
        $(this).prop("checked", false);
        if ($(this).attr("value") == Value) $(this).prop("checked", true);
    });
}

var DragDrop = true;
var QueryStringValue = "";
var MenuSelected = "";
$(document).ready(function () {

    $("#spnUserName").html($("#hdnUserId").val());
    if ($("#hdnValidateSecurity").val() == "Yes") {

        QueryStringValue = GetQueryStringValue("Search");
        $("#hdnQueryStringSearchValue").val(QueryStringValue);

        if ($("#hdnOrgRole").val().toUpperCase() == "USER") DragDrop = false;

        if ($("#hdnCompanyName").val().toUpperCase() != "UNHCR") {
            $("#divDownloadMLPDF").hide();
            $("#divDownloadAllPDF").hide();
            $("#divDownloadPDF").show();
        }
        else if ($("#hdnCompanyName").val().toUpperCase() == "UNHCR") {
            $("#divDownloadMLPDF").hide();
            $("#divDownloadAllPDF").show();
            $("#divDownloadPDF").hide();
        }

        // Drag and Drop objects.
        var DragFields = "";
        var ShowDragObjects = JSON.parse($("#hdnSearchMenu").val())
        $.each(ShowDragObjects, function (key, item) {
            if (item.SearchFlag == "Y") DisplayField = ""; else DisplayField = "display:none;";
            DragFields += "<div data-id=\"" + item.SearchField + "\" id=\"div" + item.SearchField + "\" class=\"draggable\" draggable=\"true\">" + item.FieldCaption + "</div>";
        });
        $("#paletteZone").html(DragFields);

        if ($("#hdnOrgRole").val().toUpperCase() == "ENDUSER") {
            $("#divReOrgUsers").hide();
            $("#liUpdateSettings").show();
            $("#liDownloadDialogBox").show();
            $("#liSearch").show();
            $("#liEndUserDivider").show();
            $("#divDownloadInitiate").hide();
            $("#divDownloadVersion").show();
            $("#divDownloadAllPDF").show();
            $("#spnVersionDetailsBG").show();
        }
        else {
            $("#divReOrgUsers").show();
            $("#liUpdateSettings").hide();
            $("#liDownloadDialogBox").hide();
            $("#liSearch").hide();
            $("#liEndUserDivider").hide();
            $("#divDownloadInitiate").show();
            $("#divDownloadVersion").show();
            $("#spnVersionDetailsBG").show();
        }

        //Hide PPTX Template Upload, Revoke/Delete Template and Add Users (to Users and players)
        if ($("#hdnOrgRole").val() == "Finalyzer") {
            $("#liAddUsr").show();
            $("#liDivider").show();
            $("#tabUploadTemplate").show();
            $("#tabRevokeTemplate").show();
            $(".saveversion").show();
        }
        else {
            $("#liAddUsr").hide();
            $("#liDivider").hide();
            $("#tabUploadTemplate").hide();
            $("#tabRevokeTemplate").hide();
            if ($("#hdnOrgRole").val().toUpperCase() == "PLAYER" ||
                $("#hdnOrgRole").val().toUpperCase() == "USER" ||
                $("#hdnOrgRole").val().toUpperCase() == "ENDUSER") $(".saveversion").hide();
        }

        if ($("#hdnSelectedShape").val()) {
            Settings.SelectShape = $("#hdnSelectedShape").val();
            ShowSettingsValue('shape', $("#hdnSelectedShape").val());
        }
        if ($("#hdnSelectedSkin").val()) {
            Settings.Skin = $("#hdnSelectedSkin").val();
            ShowSettingsValue('skin', $("#hdnSelectedSkin").val());
        }
        if ($("#hdnSelectedShowPicture").val()) {
            Settings.ShowPicture = $("#hdnSelectedShowPicture").val();
            ShowSettingsValue('picture', $("#hdnSelectedShowPicture").val());
        }
        if ($("#hdnOrgView").val()) {
            Settings.SelectView = $("#hdnOrgView").val();
            $("#selView").val($("#hdnOrgView").val());
        }
        if ($("#hdnSelectedSplitScreenDirection").val()) {
            Settings.SplitScreenDirection = $("#hdnSelectedSplitScreenDirection").val();
            ShowSettingsValue('splitscreendirection', $("#hdnSelectedSplitScreenDirection").val());
        }
        if ($("#hdnSelectedSplitScreen").val()) Settings.SplitScreen = $("#hdnSelectedSplitScreen").val();
        if ($("#hdnSelectedFunctionalManagerType").val()) {
            Settings.FunctionalManagerType = $("#hdnSelectedFunctionalManagerType").val();
            ShowSettingsValue('functionalmanagertype', $("#hdnSelectedFunctionalManagerType").val());
        }
        if ($("#hdnSelectedPortraitModeMultipleLevel").val()) {
            Settings.PortraitModeMultipleLevel = $("#hdnSelectedPortraitModeMultipleLevel").val();
            $("#selPortraitModeMultipleLevel").val(Settings.PortraitModeMultipleLevel);
        }
        if ($("#hdnOrgChartType").val()) {
            Settings.OrgChartType = $("#hdnOrgChartType").val();
            $("#selOrgChartType").val(Settings.OrgChartType);
        }
        if ($("#hdnSelectedBorderColor").val()) {
            // Color Picker(Node Box)
            $('#cpInline2').colorpicker({ color: $("#hdnSelectedBorderColor").val(), defaultPalette: 'web' });
        }
        else {
            // Color Picker(Node Box)
            $('#cpInline2').colorpicker({ color: '#92cddc', defaultPalette: 'web' });
            $("#hdnSelectedBorderColor").val('#92cddc');
        }
        Settings.BorderColor = $('#cpInline2').colorpicker("val");
        // Methods demo 2 (inline colorpicker Node Box)
        $('#cpInline2').on('click', function () {
            alert('Selected color = "' + $('#cpInline2').colorpicker("val") + '"');
            Settings.BorderColor = $('#cpInline2').colorpicker("val");
        });

        if ($("#hdnSelectedLineColor").val()) {
            // Color Picker(Node Line color)
            $('#cpInline3').colorpicker({ color: $("#hdnSelectedLineColor").val(), defaultPalette: 'web' });
        }
        else {
            // Color Picker(Node Line color)
            $('#cpInline3').colorpicker({ color: '#92cddc', defaultPalette: 'web' });
        }
        Settings.LineColor = $('#cpInline3').colorpicker("val");
        // Methods demo 3 (inline colorpicker Node Line color)
        $('#cpInline3').on('click', function () {
            alert('Selected color = "' + $('#cpInline3').colorpicker("val") + '"');
            Settings.LineColor = $('#cpInline3').colorpicker("val");
        });

        $("form#ImageContent").submit(function (e) {
            e.preventDefault();
            if ($("#uploadImageContent").val() != null || $("#uploadImageContent").val() != "") {

                $("#divImageError").html("");
                $(".overlay").show();
                var formData = new FormData(this);
                $.ajax({
                    url: HOST_ENV + "/Version/UploadImage",
                    type: 'POST',
                    data: formData,
                    async: false,
                    success: function (data) {
                        if (data.Success == "Yes") {
                            $("#divImageError").html(data.Message);
                            $("#divImageError").css('color', 'blue');
                            $("#hdnOrgChartData").val(data.ChartData);
                            CancelOperation();

                        } else {

                            $("#divImageError").html(data.Message);
                            $(".overlay").hide();
                        }

                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
            }
            else {
                $("#divImageError").html("Please Select a Picture");
                $("#divImageError").css('color', 'red');
            }
            $(".overlay").hide();
        });

        // PPTX template Starts
        $("#img_PPTX1").attr("src", HOST_ENV + "/Content/Images/SelectedOne.svg");
        $("form#dataPPTX").submit(function (e) {

            $("#divPPTXError").html(""); $("#divPPTX").html("");
            if ($("#hdnPPTXFileName").val() == "") {
                e.preventDefault();
                var formData = new FormData(this);

                $.ajax({
                    url: HOST_ENV + "/Version/UploadPPTXFile",
                    type: 'POST',
                    data: formData,
                    async: false,
                    success: function (data) {
                        if (data.Success == "Yes") {
                            DataPPTX++;
                            $("#hdnPPTXUseFields").val(data.SF);
                            $("#hdnPPTXFields").val(data.MF);
                            $("#hdnPPTXFileName").val(data.FN);
                            var ShowFields = data.SF.split(",");
                            var MappingFields = $("#hdnPPTXFields").val().split(",");
                            var MF = "<option value=\"\">Select Mapping Field</option>";
                            for (Idx = 0; Idx < MappingFields.length; Idx++) {
                                MF += "<option value=\"" + MappingFields[Idx] + "\">" + MappingFields[Idx] + "</option>"
                            }
                            var SelDataType = "";
                            var Index = 0;
                            for (Idx = 0; Idx < ShowFields.length; Idx++) {
                                SelDataType += "<div class=\"col-md-4\" style=\"margin-bottom:10px;float:left;\">";
                                SelDataType += "         <input type=\"checkbox\" value=\"" + ShowFields[Idx] + "\" id=\"chkFieldsPPTX" + Index.toString() + "\" name=\"chkFieldsPPTX\"  data-id=\"" + Index.toString() + "\" />";
                                SelDataType += "         <lable for=\"lblFieldsPPTX" + Index.toString() + "\" value=" + ShowFields[Idx] + " style=\"width:100%;text-align:left;\">" + ShowFields[Idx] + "</lable>";
                                SelDataType += "    <div style=\"width:100%\">";
                                SelDataType += "       <select class=\"form-control\" id=\"selMapFieldsPPTX" + Index.toString() + "\" data-id=\"" + Index.toString() + "\" name=\"selMapFieldsPPTX" + Index.toString() + "\"> ";
                                SelDataType += MF;
                                SelDataType += "       </select> ";
                                SelDataType += "    </div> ";
                                SelDataType += "    <div style=\"width:100%\">";
                                SelDataType += "       <select class=\"form-control\" onclick=\"SetDataTypePPTX(this)\" id=\"selDataTypePPTX" + Index.toString() + "\" data-id=\"" + Index.toString() + "\" name=\"selDataTypePPTX" + Index.toString() + "\"> ";
                                SelDataType += "            <option value=\"String\"" + (this.FieldType == "String" ? " selected=\'selected\'" : "") + ">String</option>";
                                SelDataType += "            <option value=\"Float\"" + (this.FieldType == "Number" ? " selected=\'selected\'" : "") + ">Float</option>";
                                SelDataType += "            <option value=\"Int\"" + (this.FieldType == "Int" ? " selected=\'selected\'" : "") + ">Int</option>";
                                SelDataType += "            <option value=\"DateTime\"" + (this.FieldType == "DateTime" ? " selected=\'selected\'" : "") + ">DateTime</option>";
                                SelDataType += "       </select> ";
                                SelDataType += "    </div> ";
                                SelDataType += "    <div id=\"divFormatPPTX\"" + Index.toString() + " style=\"width:100%\">";
                                SelDataType += "       <select class=\"form-control\" id=\"selFormatPPTX" + Index.toString() + "\" data-id=\"" + Index.toString() + "\" name=\"selFormatPPTX" + Index.toString() + "\"> ";
                                SelDataType += "           <option value=\"\" selected=\"selected\">Format not Required</option>";
                                SelDataType += "       </select> ";
                                SelDataType += "    </div> ";
                                SelDataType += "</div>";
                                Index++;
                            }

                            SelDataType += "<div class=\"col-md-12\" style=\"clear: both;\">&nbsp;</div>";
                            $("#divPPTX2").html(SelDataType);
                        }
                        else {
                            if (data.Success == "No") {
                                $("#divPPTXError").html(data.Message);
                                $("#divRespPPTX").show();
                            }
                        }
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
            }
            else DataPPTX++;
        });
        // PPTX template Ends

        $("#ancOpenSideMenu").click(function () {
            $("#mySidenav").css("width", "350px");
            $("#mySidenav").show();
            $("#divSideMenu").show();
            $("#myMenuContainer").show();
        });

        $("#ancOpenTreeView").click(function () {

            $(".overlay").show();
            var JsonData = {};
            $.ajax({
                url: HOST_ENV + "/Version/ShowSearchMenu",
                type: 'GET',
                data: JsonData,
                dateType: "json",
                async: true,
                success: function (partialViewResult) {
                    $("#divSearchTable").html(partialViewResult);

                    var SearchFields = "", DisplayField = "";
                    var ShowSearch = JSON.parse($("#hdnSearchMenu").val());

                    SearchFields += "<div class=\"row\">";
                    SearchFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
                    SearchFields += "        <span style=\"float:left;font-weight:bold;padding-right:10px;\">Selected Search Fields</span>";
                    SearchFields += "        <img style=\"float:left;display:none;\" alt=\"show and hide Search Fields\" src=\"/Content/Images/downarrow.jpg\" onclick=\"ShowHide('divSelectedSearchFields', this)\" />";
                    SearchFields += "   </div>";
                    SearchFields += "   <div id=\"divSelectedSearchFields\" class=\"col-md-12\" style=\"margin: 0px 0px;\">";
                    $.each(ShowSearch, function (key, item) {
                        if (item.SearchFlag == "Y") DisplayField = ""; else DisplayField = "display:none;";
                        SearchFields += "<div class=\"col-md-3\" style=\"margin: 0px 0px;padding-bottom:5px;" + DisplayField + "\">";
                        SearchFields += "   <div style=\"width:100%;text-align:left;\">" + item.FieldCaption + "</div>";
                        SearchFields += "   <div style=\"width:100%;padding:2px;\"><input data-id=\"" + item.SearchField + "\" id=\"div" + item.SearchField + "\" name=\"searchtext\" value=\"\"  class=\"form-control searchtext\" style=\"max-width:100%!important;\"/></div>";
                        SearchFields += "</div>";
                    });
                    SearchFields += "   </div>";
                    SearchFields += "</div>";

                    $("#mySearchMenu").html(SearchFields);
                    GridViewPartial.PerformCallback();

                    $(".overlay").hide();
                },
                cache: false
            });

            $("#SearchModal").modal({
                backdrop: 'static',
                keyboard: false
            });
        });

        $("#imgPin").click(function () {
            if ($("#imgPin").attr("src").indexOf("unpin.svg") != -1) {
                $("#imgPin").attr("src", HOST_ENV + "/Content/images/pin.svg?cid=11");
            }
            else {
                $("#imgPin").attr("src", HOST_ENV + "/Content/images/unpin.svg?cid=11");
            }
        });

        var slider = document.getElementById("myRange");
        var output = document.getElementById("myLevel");
        $("#myLevel").val(slider.value == "6" ? "All" : slider.value); // Display the default slider value

        // Update the current slider value (each time you drag the slider handle)
        $("input[type='range']").change(function () {
            $("#myLevel").val(this.value == "6" ? "All" : slider.value);
            //alert(slider.value);
            //ChangeLevels(this.value);
        });

        // Use Select2
        $('.select').select2({ width: "190px" });

        if (NewLayout_ViewBagTitle == "AdminVersion" || NewLayout_ViewBagTitle == "UploadVersion" || NewLayout_ViewBagTitle == "UploadData") {

            var h = 0, w = 0;
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + "}";
            document.getElementById("myPartitionSavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartHRCoreData").val() + " }";

            // Org chart in Canvas
            h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
            w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));

            if ($("#hdnOrgChartType").val() == "OS") {
                initOrgChartStatic($("#hdnOrgChartData").val(), w, h, DragDrop);
                $("#myOverviewDiv").show();
                $("#divSearchDiagram").show();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbsbig");
            }
            else if ($("#hdnOrgChartType").val() == "OD") {
                init(w, h, DragDrop);
                $("#myOverviewDiv").hide();
                $("#divSearchDiagram").hide();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbs");
            }
            else if ($("#hdnOrgChartType").val() == "OC") {
                initCustom(w, h, DragDrop);
                $("#myOverviewDiv").hide();
                $("#divSearchDiagram").hide();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbs");
            }

            h = parseInt($("#divLeftOrgChart").css("height").substr(0, $("#divLeftOrgChart").css("height").indexOf("px")));
            w = parseInt($("#divLeftOrgChart").css("width").substr(0, $("#divLeftOrgChart").css("width").indexOf("px")));
            initPartition(w, h, DragDrop);

            //initDragAndDrop();

            $('#multiselect').multiselect();  // Full Name multi select
        }
        else if (NewLayout_ViewBagTitle == "Home Page") {
            var h = 0, w = 0;
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            // Org chart in Canvas
            h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
            w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
            if ($("#hdnOrgChartType").val() == "OS") {
                initOrgChartStatic($("#hdnOrgChartData").val(), w, h, DragDrop);
                $("#myOverviewDiv").show();
                $("#divSearchDiagram").show();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbsbig");
            }
            else if ($("#hdnOrgChartType").val() == "OD") {
                init(w, h, DragDrop);
                $("#myOverviewDiv").hide();
                $("#divSearchDiagram").hide();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbs");
            }
            else if ($("#hdnOrgChartType").val() == "OC") {
                initCustom(w, h, DragDrop);
                $("#myOverviewDiv").hide();
                $("#divSearchDiagram").hide();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbs");
            }

            //initDragAndDrop();
        }
        else if (NewLayout_ViewBagTitle == "EndUser") {
            var h = 0, w = 0;
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            // Org chart in Canvas
            h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
            w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));

            if ($("#hdnOrgChartType").val() == "OS") {
                initOrgChartStatic($("#hdnOrgChartData").val(), w, h, DragDrop);
                $("#myOverviewDiv").show();
                $("#divSearchDiagram").show();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbsbig");
            }
            else if ($("#hdnOrgChartType").val() == "OD") {
                init(w, h, DragDrop);
                $("#myOverviewDiv").hide();
                $("#divSearchDiagram").hide();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbs");
            }
            else if ($("#hdnOrgChartType").val() == "OC") {
                initCustom(w, h, DragDrop);
                $("#myOverviewDiv").hide();
                $("#divSearchDiagram").hide();
                $("#divBreadGram").removeClass("breadcrumbs");
                $("#divBreadGram").removeClass("breadcrumbsbig");
                $("#divBreadGram").addClass("breadcrumbs");
            }

            //initDragAndDrop();
        }

        var DivRow = "", GroupName = "";
        var TABLES = JSON.parse($("#hdnOrgMenu").val());

        DivRow += "<div>";
        DivRow += "<ul id=\"nav\">";
        if ($("#hdnOrgRole").val() != "EndUser") {
            DivRow += "<li><a data-html=\"Home\" href=\"" + (HOST_ENV == "" ? "/?Search=Refresh" : HOST_ENV + "/?Search=Refresh") + "\"><i class=\"fas fa-home menu-icon\"></i> <span class=\"sidemenu-label\">Home</span></a></li>";
            DivRow += "<li>";
            DivRow += "<a href=\"#\"><i class=\"fas fa-users-cog menu-icon\"></i><span class=\"sidemenu-label\">Roles</span> <i class=\"fas fa-angle-down control-btns\"></i><i class=\"fas fa-angle-up control-btns\"></i></a>";
            DivRow += "<ul>";
            var MENU_TABLE = TABLES.MT;
            if (MENU_TABLE.length >= 1) {

                for (var Idx = 0; Idx <= MENU_TABLE.length - 1; Idx++) {
                    if (MENU_TABLE[Idx].GroupName != "Views" && $("#hdnOrgRole").val() != "EndUser") {
                        if (MENU_TABLE[Idx].GroupName == "Role" && $("#hdnOrgAssignedRole").val().indexOf(MENU_TABLE[Idx].Role) != -1) {
                            if (GroupName != MENU_TABLE[Idx].GroupName) {
                                if (GroupName != "") DivRow += "</li>";
                                GroupName = MENU_TABLE[Idx].GroupName;
                            }
                            if (MENU_TABLE[Idx].Link == "Y")
                                DivRow += "<li><a href = '" + MENU_TABLE[Idx].URL + "'>";
                            else if (MENU_TABLE[Idx].JSMethod != "" && MENU_TABLE[Idx].ModelDailog == "")
                                DivRow += "<li><a href = 'javascript: void (0);' data-url=\"" + MENU_TABLE[Idx].URL + "\" onclick=\"return " + MENU_TABLE[Idx].JSMethod + "(this," + GetParameter(MENU_TABLE[Idx].Parameter) + " );\">";
                            else if (MENU_TABLE[Idx].ModelDailog != "")

                                DivRow += "<a href = 'javascript: void (0);' data-url=\" " + MENU_TABLE[Idx].URL + "\" data-toggle='modal' data-target='#" + MENU_TABLE[Idx].ModelDailog + "' onclick=\"return " + MENU_TABLE[Idx].JSMethod + " (this, " + GetParameter(MENU_TABLE[Idx].Parameter) + " );\">";
                            if (MENU_TABLE[Idx].DisplayName == "User") { DivRow += "<i class=\"fas fa-user-tie\"></i>"; }
                            else if (MENU_TABLE[Idx].DisplayName == "OrgPlanner") { DivRow += "<i class=\"fas fa-chart-line\"></i>"; }
                            else if (MENU_TABLE[Idx].DisplayName == "OrgAdmin") { DivRow += "<i class=\"fas fa-user-cog\"></i>"; }
                            DivRow += "<span  class=\"sidemenu-label\">" + MENU_TABLE[Idx].DisplayName + "</span>";
                            DivRow += "</a></li>";
                        }
                    }
                }
                DivRow += "</ul>";
                DivRow += "</li>";
            }
        }

        var MENU_TABLE = TABLES.VT;
        var Options = "";
        DivRow += "<li>";
        DivRow += "  <a data-html=\"Views\" href=\"#\"><img src=\"" + HOST_ENV + "/Content/NewTemplateUI/Images/views.svg\" width=\"25px\" alt=\"logo\"/><span class=\"sidemenu-label\">Views</span><i class=\"fas fa-angle-up control-btns\"></i><i class=\"fas fa-angle-down control-btns\"></i></a>";
        DivRow += "<ul>"
        for (var Idx = 0; Idx <= MENU_TABLE.length - 1; Idx++) {
            DivRow += "<li><a href='javascript: void (0);' data-url=\"" + MENU_TABLE[Idx].URL + "\" onclick=\"return " + MENU_TABLE[Idx].JSMethod + "(this, " + GetParameter(MENU_TABLE[Idx].Parameter) + ");\"><img src=\"" + HOST_ENV + "/Content/NewTemplateUI/Images/" + MENU_TABLE[Idx].ImageURL + "\" width=\"25px\" alt=\"logo\" /><span class=\"sidemenu-label\">" + MENU_TABLE[Idx].DisplayName + "</span></a></li>";
            DivRow += "</a></li>";
            Options += "<option value=\"" + MENU_TABLE[Idx].ViewName + "\">" + MENU_TABLE[Idx].DisplayName + "</option>"
        }
        $("#selView").html(Options);
        DivRow += "</ul>";
        DivRow += "</li>";

        DivRow += "<li>";
        DivRow += "  <a data-html=\"Hierarchy\" href=\"#\"><img src=\"" + HOST_ENV + "/Content/NewTemplateUI/Images/collaboration.svg\" width=\"25px\" alt=\"logo\"/><span class=\"sidemenu-label\">Managers</span><i class=\"fas fa-angle-up control-btns\"></i><i class=\"fas fa-angle-down control-btns\"></i></a>";
        DivRow += "<ul>";
        DivRow += "<li>";
        DivRow += "<div id=\"divHierarchy\" class=\"zTreeDemoBackground left\" style=\"max-height:500px;overflow:auto;\">";
        DivRow += "<ul id=\"treeDemo\" class=\"ztree clsHier1\"></ul>";
        DivRow += "</div>";
        DivRow += "</li>";
        DivRow += "</ul>";
        DivRow += "</li>";
        DivRow += "</ul>";
        DivRow += "</div>";

        $("#DivRolesViews").html(DivRow);
        ShowHierarchy();

        $('#nav li').first().addClass("active").find('ul').show();
        $('#nav > li > a').click(function () {
            if ($(this).attr('class') != 'active') {
                $('#nav li ul').slideUp();
                $(this).next().slideToggle();
                $('#nav li a').removeClass('active');
                $(this).addClass('active');

                try {
                    MenuSelected = $(this).attr('data-html');
                    if (MenuSelected == "Hierarchy") {
                        if (document.getElementById("mySidenav").style.width == "250px")
                            $("#treeDemo").show();
                    }
                }
                catch (ex) {

                }
            }
        });

        // Setting current position of Org Chart
        var DisplayUserRole = GetDisplayUserRole($("#hdnOrgRole").val());
        $("#spnOrgRole").html(DisplayUserRole);

        var sValue = $("#hdnOrgType").val();
        if (sValue == "OV") {
            $("#spnOrgType").html("Operational View");
            $("#divCountry").hide();
        }
        else if (sValue == "LV") {
            $("#spnOrgType").html("Legal View");
            $("#divCountry").show();
        }

        sValue = $("#hdnOrgLevel").val();
        if (sValue == "One") $("#spnOrgLevel").html("Level One");
        else if (sValue == "Two") $("#spnOrgLevel").html("Level Two");
        else if (sValue == "Three") $("#spnOrgLevel").html("Level Three");
        else if (sValue == "Four") $("#spnOrgLevel").html("Level Four");
        else if (sValue == "Five") $("#spnOrgLevel").html("Level Five");
        else if (sValue == "All") $("#spnOrgLevel").html("All Levels");

        sValue = $("#hdnOrgView").val();
        if (sValue == "Sample") $("#spnOrgView").html("Sample View");
        else if (sValue == "Normal") $("#spnOrgView").html("Normal View");
        else if (sValue == "Cost") $("#spnOrgView").html("Position Cost View");

        if ($("#hdnOrgRole").val() == "Finalyzer" ||
            $("#hdnOrgRole").val() == "Player" ||
            $("#hdnOrgRole").val() == "Admin" ||
            $("#hdnOrgRole").val() == "User") {

            SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#hdnSelectedInitiative").val(), "N");
            if ($("#hdnOrgRole").val() == "Player" || $("#hdnOrgRole").val() == "Finalyzer")
                SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#hdnSelectedInitiative").val(), "Y");

            $("#selInitiative").select2("val", $("#hdnSelectedInitiative").val());
            $("#selPopulation").select2("val", $("#hdnSelectedPopulation").val());
            $("#selUser").select2("val", $("#hdnSelectedUser").val());
            $("#selVersion").select2("val", $("#hdnSelectedVersion").val());
        }

        switch ($("#hdnOrgLevel").val().toUpperCase()) {
            case "ONE":
                Levels = 1;
                break;
            case "TWO":
                Levels = 2;
                break;
            case "THREE":
                Levels = 3;
                break;
            case "FOUR":
                Levels = 4;
                break;
            case "FIVE":
                Levels = 5;
                break;
            case "ALL":
                Levels = 6;
                break;
        }
        $("#myRange").val(Levels);
        $("#myLevel").val(Levels);

        if ($("#hdnOrgChartType").val() == "OS") {
            $("#myOverviewDiv").show();
            $("#divSearchDiagram").show();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbsbig");
        }
        else if ($("#hdnOrgChartType").val() == "OD") {
            $("#myOverviewDiv").hide();
            $("#divSearchDiagram").hide();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbs");
        }
        else if ($("#hdnOrgChartType").val() == "OC") {
            $("#myOverviewDiv").hide();
            $("#divSearchDiagram").hide();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbs");
        }
    }
    else if ($("#hdnValidateSecurity").val() == "No") {
        $("#divAddPopulation").hide();
        $("#divAuthorizedPage").show();

        $(".addinitiative").hide();
        $(".deleteinitiative").hide();
        $(".saveversion").hide();
        $(".splitscreen").hide();
        $(".showhidemenu").hide();
        $(".showdropdown-menu").hide();
        $(".showMenu").hide();
        $(".ddlselect").hide();

        var DisplayUserRole = GetDisplayUserRole($("#hdnOrgRole").val());
        $("#spnOrgRole").html(DisplayUserRole);
    }
    if ($("#hdnOrgRole").val() == "Finalyzer") {
        if ($("#hdnFinalyzerVersionFlag").val() == "Yes") {
            $(".splitscreen").show();
            $(".saveversion").hide();
        }
        else {
            $(".splitscreen").hide();
            $(".saveversion").show();
        }
    }
});

function ShowHierarchy() {
    try {
        //console.log($("#hdnOrgTreeData").val());
        var zNodesJson = JSON.parse($("#hdnOrgTreeData").val());
        var zNodes = [];
        if (zNodesJson.length >= 1) {
            for (var Idx = 0; Idx <= zNodesJson.length - 1; Idx++) {
                try {
                    if (parseInt(zNodesJson[Idx].NOR_COUNT, 10) >= 1) {
                        var item = {};
                        item["id"] = zNodesJson[Idx].LEVEL_ID;
                        item["pId"] = (zNodesJson[Idx].PARENT_LEVEL_ID == "999999") ? "0" : zNodesJson[Idx].PARENT_LEVEL_ID;
                        item["name"] = zNodesJson[Idx].FULL_NAME + "( " + zNodesJson[Idx].LEVEL_ID + " )";
                        item["obj"] = "";
                        item["checked"] = "false";
                        item["title"] = zNodesJson[Idx].FULL_NAME + "( " + zNodesJson[Idx].LEVEL_ID + " )";
                        item["dataid"] = (Idx + 1).toString();
                        zNodes.push(item);
                    }
                }
                catch (ex) {
                    alert(ex);
                }
            }
        }
    
        $.fn.zTree.destroy("treeDemo");
        $.fn.zTree.init($("#treeDemo"), zTreeSettings, zNodes);
        setCheck("treeDemo");
    }
    catch (ex) {
        console.log(ex);
    }
}

function SearchDialogBox() {

    $(".overlay").show();
    var JsonData = {};
    $.ajax({
        url: HOST_ENV + "/Version/ShowSearchMenu",
        type: 'GET',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (partialViewResult) {
            $("#divSearchTable").html(partialViewResult);

            var SearchFields = "", DisplayField = "";
            var ShowSearch = JSON.parse($("#hdnSearchMenu").val());

            SearchFields += "<div class=\"row\">";
            SearchFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
            SearchFields += "        <span style=\"float:left;font-weight:bold;padding-right:10px;\">Selected Search Fields</span>";
            SearchFields += "        <img style=\"float:left;\" alt=\"show and hide Selected Search Fields\" src=\"/Content/Images/downarrow.jpg\" onclick=\"ShowHide('divSelectedSearchFields', this)\" />";
            SearchFields += "   </div>";
            SearchFields += "   <div class=\"col-md-12\" style=\"margin: 0px 0px;\">";
            $.each(ShowSearch, function (key, item) {
                if (item.SearchFlag == "Y") DisplayField = ""; else DisplayField = "display:none;";
                SearchFields += "<div class=\"col-md-3\" style=\"margin: 0px 0px;padding-bottom:5px;" + DisplayField + "\">";
                SearchFields += "   <div style=\"width:100%;text-align:left;\">" + item.FieldCaption + "</div>";
                SearchFields += "   <div style=\"width:100%;padding:2px;\"><input data-id=\"" + item.SearchField + "\" id=\"div" + item.SearchField + "\" name=\"searchtext\" value=\"\"  class=\"form-control searchtext\" style=\"max-width:100%!important;\"/></div>";
                SearchFields += "</div>";
            });
            SearchFields += "   </div>";
            SearchFields += "</div>";

            $("#mySearchMenu").html(SearchFields);
            GridViewPartial.PerformCallback();

            $(".overlay").hide();
        },
        cache: false
    });

    $("#SearchModal").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function GetDuplicateField(FieldName) {
    if (JsonFieldInfo.length >= 1) {
        for (Idx = 0; Idx <= JsonFieldInfo.length - 1; Idx++) {
            if (JsonFieldInfo[Idx].SelectedField.toUpperCase() == FieldName.toUpperCase()) {
                return false;
            }
        }
    }

    return true;
}

var JsonFieldInfo = [];
var ErrorJsonData = "";
function GetFieldsInfoPPTX() {
    JsonFieldInfo = [];
    ErrorJsonData = "";

    $('input[name="chkFieldsPPTX"]:checked').each(function () {
        if ($("#selMapFieldsPPTX" + $(this).attr("data-id")).val() == "")
            ErrorJsonData += ", " + $("#chkFieldsPPTX" + $(this).attr("data-id")).val() + " not mapped"
        if (GetDuplicateField($("#chkFieldsPPTX" + $(this).attr("data-id")).val())) {
            JsonFieldInfo.push({
                SelectedField: $("#chkFieldsPPTX" + $(this).attr("data-id")).val(),
                MappedFields: $("#selMapFieldsPPTX" + $(this).attr("data-id")).val(),
                FieldDataType: $("#selDataTypePPTX" + $(this).attr("data-id")).val(),
                FieldDataFormat: $("#selFormatPPTX" + $(this).attr("data-id")).val()
            })
        }
    });
}

function SetDataTypePPTX(Obj, FieldFormat) {
    var DataTypePPTX = ""
    if ($(Obj).val().toUpperCase() == "FLOAT") {
        DataTypePPTX += "            <option value=\"0\"" + (FieldFormat == "0" ? " selected=\'selected\'" : "") + ">0</option>";
        DataTypePPTX += "            <option value=\"1\"" + (FieldFormat == "1" ? " selected=\'selected\'" : "") + ">1</option>";
        DataTypePPTX += "            <option value=\"2\"" + (FieldFormat == "2" ? " selected=\'selected\'" : "") + ">2</option>";
        DataTypePPTX += "            <option value=\"3\"" + (FieldFormat == "3" ? " selected=\'selected\'" : "") + ">3</option>";
        DataTypePPTX += "            <option value=\"4\"" + (FieldFormat == "4" ? " selected=\'selected\'" : "") + ">4</option>";
    }
    else if ($(Obj).val().toUpperCase() == "DATETIME") {
        DataTypePPTX += "            <option value=\"yyyy/MM/dd\"" + (FieldFormat == "yyyy/MM/dd" ? " selected=\'selected\'" : "") + ">yyyy/MM/dd</option>";
        DataTypePPTX += "            <option value=\"dd/MM/yyyy\"" + (FieldFormat == "dd/MM/yyyy" ? " selected=\'selected\'" : "") + "dd/MM/yyyy</option>";
        DataTypePPTX += "            <option value=\"MM/dd/yyyy\"" + (FieldFormat == "MM/dd/yyyy" ? " selected=\'selected\'" : "") + ">MM/dd/yyyy</option>";
    }
    else DataTypePPTX += "           <option value=\"\" selected=\"selected\">Format not Required</option>";

    $("#selFormatPPTX" + $(Obj).attr("data-id")).html(DataTypePPTX);
}


// PPTX template starts
function SetPPTXDivIds() {
    $("#divErrorPPTXUploadFile").hide();
    $("#hdnPPTXFileName").val("");
}

function ErrorPPTXString() {
    var sError = "N";
    $("#divErrorPPTXUploadFile").hide();
    $("#divErrorTemplateNamePPTX").hide();
    $("#divErrorTemplateDescriptionPPTX").hide();
    if (DataPPTX == 0) {
        if ($("#uploadPPTX").val().trim() == "") {
            $("#divErrorPPTXUploadFile").show();
            sError = "Y";
        }
        else {
            var ext = $('#uploadPPTX').val().split('.').pop().toLowerCase();
            if ($.inArray(ext, ['pptx']) == -1) {
                $("#divErrorPPTXUploadFile").show();
                sError = "Y";
            }
        }

        if ($("#txtTemplateNamePPTX").val().trim() == "") {
            $("#divErrorTemplateNamePPTX").show();
            sError = "Y";
        }
        if ($("#txtTemplateDescriptionPPTX").val().trim() == "") {
            $("#divErrorTemplateDescriptionPPTX").show();
            sError = "Y";
        }
    }
    else if (DataPPTX == 2) {
    }

    return sError;
}

//Vijay
function PPTXUploadClose() {
    var val = $('li.active').attr('data-id');
    if (val == "UploadTemplate") {
        if ($("#btnPPTXFinish").is(":disabled") == true && $("#btnPPTXNext").is(":disabled") == false || $("#btnPPTXFinish").is(":disabled") == false && $("#btnPPTXNext").is(":disabled") == true) {
            if (confirm("Are you sure you want to navigate away from this page? Be aware, if you press \"OK\" now, ALL your changes will be lost!. Press OK to continue or Cancel to stay on the current page"))
                popuprefresh();
            else $("#DownloadModal").modal("show");
        }
        else if ($("#btnPPTXFinish").is(":disabled") == true && $("#btnPPTXNext").is(":disabled") == true) popuprefresh();
    }
    else $("#DownloadModal").modal("hide");
}

function popuprefresh() {
    $("#img_PPTX1").attr("src", HOST_ENV + "/Content/Images/SelectedOne.svg");
    $("#txtTemplateDescriptionPPTX").val("");
    $("#uploadPPTX").val("");
    $("#txtTemplateNamePPTX").val("");
    $("#divErrorSelectionFields").val("");
    $("#dvNav_B_PPTX2").removeClass('navIndexGreen');
    $("#dvNav_B_PPTX2").addClass('navIndexWhite');
    $("#dvNav_B_PPTX3").removeClass('navIndexGreen');
    $("#dvNav_B_PPTX3").addClass('navIndexWhite');
    $("#btnPPTXFinish").prop("disabled", true);
    $("#btnPPTXNext").prop("disabled", false);
    $("#btnPPTXBack").prop("disabled", true);
    $("#divPPTX1").show();
    $("#divPPTX2").hide();
    $("#divPPTX3").hide();
    $("#img_PPTX2").attr("src", HOST_ENV + "/Content/Images/number2.svg");
    $("#img_PPTX3").attr("src", HOST_ENV + "/Content/Images/number3.svg");
    $("#DownloadModal").modal("hide");
    DataPPTX = 0;
}

function FinishPPTX() {
    $("#img_PPTX3").attr("src", HOST_ENV + "/Content/Images/SelectedThree.svg");

    $(".overlay").show();
    var JsonData = {
        txtPFN: $("#hdnPPTXFileName").val(),
        PPTXFields: JSON.stringify(JsonFieldInfo),
        PPTXTemplateName: $("#txtTemplateNamePPTX").val(),
        PPTXTemplateDesc: $("#txtTemplateDescriptionPPTX").val(),
        NodeCount: $("#selNC").val()
    };

    $.ajax({
        url: HOST_ENV + "/Version/SavePPTXInfo",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            if (Json.Success == "Yes") {
                $("#divPPTXError").html(Json.ShowMessage);
                $("#btnPPTXFinish").prop('disabled', true);
            }
            else if (Json.Success == "No") $("#divPPTXError").html(Json.ShowMessage);
            $(".overlay").hide();
        },
        cache: false
    });
}

function NextPQ() {
    if (ErrorPPTXString() == "N") {
        if (DataPPTX == 0) $("#dataPPTX").submit(); else DataPPTX++;

        $("#img_PPTX1").attr("src", HOST_ENV + "/Content/Images/number1.svg");
        $("#img_PPTX2").attr("src", HOST_ENV + "/Content/Images/number2.svg");
        $("#img_PPTX3").attr("src", HOST_ENV + "/Content/Images/number3.svg");

        if (DataPPTX == 1) {
            $("#dvNav_B_PPTX2").removeClass('navIndexWhite');
            $("#dvNav_B_PPTX2").addClass('navIndexGreen');
            $("#divPPTX1").hide();
            $("#divPPTX2").show();
            $("#divPPTX3").hide();
            $('#btnPPTXBack').prop('disabled', false);
            $("#img_PPTX2").attr("src", HOST_ENV + "/Content/Images/SelectedTwo.svg");
        }
        else if (DataPPTX == 2) {
            GetFieldsInfoPPTX();
            if (JsonFieldInfo.length <= 0 || ErrorJsonData != "") {
                DataPPTX--;
                $("#divPPTX2").show();
                $("#btnPPTXNext").prop('disabled', false);
                $("#btnPPTXFinish").prop('disabled', true);
                $("#divPPTX3").hide();
                if (JsonFieldInfo.length <= 0)
                    $("#divErrorSelectionFields").html("Please select field(s) to show.");
                else if (ErrorJsonData != "")
                    $("#divErrorSelectionFields").html(ErrorJsonData.substring(1));
                $("#divErrorSelectionFields").show();

            } else {
                $("#divErrorSelectionFields").hide();
                var SelDataType = "", SelectedValue = "";
                for (Idx = 0; Idx < JsonFieldInfo.length; Idx++) {
                    SelDataType += "<div style=\"width:100%;text-align:left;color:black;\">";
                    if (JsonFieldInfo[Idx].FieldDataFormat == "") {
                        SelectedValue = JsonFieldInfo[Idx].SelectedField + "( " + JsonFieldInfo[Idx].MappedFields + " : " + JsonFieldInfo[Idx].FieldDataType + " )";
                    }
                    else {
                        SelectedValue = JsonFieldInfo[Idx].SelectedField + "( " + JsonFieldInfo[Idx].MappedFields + " : " + JsonFieldInfo[Idx].FieldDataType + " : " + JsonFieldInfo[Idx].FieldDataFormat + " )";
                    }
                    SelDataType += "    <label style='padding-left: 5px'>" + SelectedValue + "</label>";
                    SelDataType += "</div>";
                }
                SelDataType += "<div class=\"col-md-12\" style=\"clear: both;\">&nbsp;</div>";
                $("#divPPTXShowFields").html(SelDataType);
                $("#hdnPPTXfields").val(JSON.stringify(JsonFieldInfo));
                $("#dvNav_B_PPTX3").removeClass('navIndexWhite');
                $("#dvNav_B_PPTX3").addClass('navIndexGreen');
                $("#divPPTX1").hide();
                $("#divPPTX2").hide();
                $("#divPPTX3").show();
                $("#btnPPTXNext").prop('disabled', true);
                $("#btnPPTXFinish").prop('disabled', false);
                $("#img_PPTX3").attr("src", HOST_ENV + "/Content/Images/SelectedThree.svg");
            }
        }
        else $("#img_PPTX1").attr("src", HOST_ENV + "/Content/Images/SelectedOne.svg");
    }
}

function BackPQ() {
    $("#divRepPopulationError").empty();
    $("#divRepPopulationHeader").empty();

    if (ErrorPPTXString() == "N") {
        DataPPTX--;

        $("#img_PPTX1").attr("src", HOST_ENV + "/Content/Images/number1.svg");
        $("#img_PPTX2").attr("src", HOST_ENV + "/Content/Images/number2.svg");
        $("#img_PPTX3").attr("src", HOST_ENV + "/Content/Images/number3.svg");

        if (DataPPTX == 0) {
            $('#btnPPTXBack').prop('disabled', true);
            $('#btnPPTXFinish').prop('disabled', true);
            $("#dvNav_B_PPTX2").removeClass('navIndexWhite');
            $("#dvNav_B_PPTX2").addClass('navIndexGreen');
            $("#divPPTX1").show();
            $("#divPPTX2").hide();
            $("#divPPTX3").hide();
            $("#img_PPTX1").attr("src", HOST_ENV + "/Content/Images/SelectedOne.svg");
        }
        else if (DataPPTX == 1) {
            $('#btnPPTXNext').prop('disabled', false);
            $('#btnPPTXFinish').prop('disabled', true);
            $("#dvNav_B_PPTX3").removeClass('navIndexWhite');
            $("#dvNav_B_PPTX3").addClass('navIndexGreen');
            $("#divPPTX1").hide();
            $("#divPPTX2").show();
            $("#divPPTX3").hide();
            $("#img_PPTX2").attr("src", HOST_ENV + "/Content/Images/SelectedTwo.svg");
        }
    }
}
// PPTX template ends

function ShowGridViewToInputBox(LevelID) {
    $(".overlay").show();
    var JsonData = {
        ShowLevel: LevelID
    };

    $.ajax({
        url: HOST_ENV + "/Version/ShowLevelInfo",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            if (Json.Success == "Yes") {
                $.each(JSON.parse(Json.IB)[0], function (key, val) {
                    try {
                        if (key.trim() != "") {
                            $("#txt" + key).val(val);
                            if (key == Json.PF) {
                                $("#txt" + key).attr("disabled", "disabled");
                            }
                        }
                    }
                    catch (ex) {
                    }
                });
            }
            else if (Json.Success == "No") $("#divAddPositionError").html(Json.ShowMessage);
            $(".overlay").hide();
        },
        cache: false
    });
}


function SaveAssignEmployee() {
    ArrayAssignEmployee = [];
    var ShowSearch = JSON.parse($("#hdnSearchMenu").val());
    $.each(ShowSearch, function (key, item) {
        try {
            var JsonFieldName = item.SearchField;
            item = {}
            item[JsonFieldName] = $("#txt" + JsonFieldName).val();
            ArrayAssignEmployee.push(item);
        }
        catch (ex) {
        }
    });

    return ArrayAssignEmployee;
}

function ClearAllFields() {
    var ShowSearch = JSON.parse($("#hdnSearchMenu").val());
    $.each(ShowSearch, function (key, item) {
        try {
            $("#txt" + item.SearchField).val("");
            if (item.PositionFlag == "Y") {
                $("#txt" + item.SearchField).attr("disabled", "disabled");
                var ShowPosition = JSON.parse($("#hdnInitialValues").val());
                if ($("#hdnSerialNoFlag").val() == "Y")
                    $("#txt" + item.SearchField).val((parseInt(ShowPosition.OprLevelId) - 100000).toString());
                else
                    $("#txt" + item.SearchField).val(ShowPosition.OprLevelId);
            }
        }
        catch (ex) {
        }
    });
}

function CopyPasteEmployeeInfo(Obj) {
    $(".overlay").show();

    AssignEmployee = SaveAssignEmployee();
    var JsonData = {
        CopyPaste: ($(Obj).hasClass("fa-copy") == true) ? "Copy" : "Paste",
        CopyInfo: JSON.stringify(AssignEmployee)
    };

    $.ajax({
        url: HOST_ENV + "/Version/CopyPasteEmployeeInfo",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            if (Json.Success == "Yes") {

                if ($(Obj).hasClass("fa-paste") == true) {

                    $("#hdnInitialValues").val(Json.InitialValues);
                    var ShowPosition = JSON.parse($("#hdnInitialValues").val());
                    if ($("#hdnSerialNoFlag").val() == "Y")
                        $("#txt" + Json.PF).val((parseInt(ShowPosition.OprLevelId) - 100000).toString());
                    else
                        $("#txt" + Json.PF).val(ShowPosition.OprLevelId);

                    $.each(JSON.parse(Json.IB), function (key, value) {
                        $.each(value, function (key, val) {
                            try {
                                if (key == Json.PF) {
                                    $("#txt" + key).attr("disabled", "disabled");
                                }
                                else $("#txt" + key).val(val);
                            }
                            catch (ex) {
                            }
                        });
                    });
                    $(Obj).removeClass("fa-paste");
                    $(Obj).addClass("fa-copy");

                    $("#divAddPositionError").html("* Employee information pasted.");
                }
                else if ($(Obj).hasClass("fa-copy") == true) {
                    $("#divAddPositionError").html("* Employee information copied.");

                    $(Obj).removeClass("fa-copy");
                    $(Obj).addClass("fa-paste");
                }
            }
            else if (Json.Success == "No") $("#divAddPositionError").html(Json.ShowMessage);
            $(".overlay").hide();
        },
        cache: false
    });
}

function AddEmployeeAssign() {
    var SearchFields = "", PositionField = "";
    var ShowSearch = JSON.parse($("#hdnSearchMenu").val());

    SearchFields += "<div class=\"row\">";
    SearchFields += "   <div class=\"col-md-12\" style=\"margin: 10px 0px;\">";
    $.each(ShowSearch, function (key, item) {
        SearchFields += "<div class=\"col-md-3\" style=\"margin: 0px 0px;padding-bottom:5px;\">";
        SearchFields += "   <div style=\"width:100%;height:60px;padding:2px;text-align:left;\">";
        if (item.PositionFlag == "Y") {
            SearchFields += "       <div style=\"width:100%\"><label style=\"width:65%\">" + item.SearchField + "</label>";
            SearchFields += "       <i class=\"fa-plus fa icon-x\" id=\"ico" + item.SearchField + "\" onclick=\"ClearAllFields();\" style=\"float:right;margin-left:5px;cursor:pointer;\"></i>";
            SearchFields += "       <i class=\"fa-copy fa icon-x\" id=\"icoCopyPaste" + item.SearchField + "\" onclick=\"CopyPasteEmployeeInfo(this);\" style=\"float:right;cursor:pointer;\"></i></div>";
            SearchFields += "       <input style=\"width:100%\" type=\"text\" data-id=\"" + item.SearchField + "\" id=\"txt" + item.SearchField + "\" name=\"inputbox\" value=\"\"  class=\"form-control textfields\" disabled=\"disabled\" style=\"width:10%!important;\"/>";
            PositionField = item.SearchField;
        }
        else {
            SearchFields += "      <label style=\"width:100%\">" + item.SearchField + "</label>";
            SearchFields += "      <input style=\"width:100%\" type=\"text\" data-id=\"" + item.SearchField + "\" id=\"txt" + item.SearchField + "\" name=\"inputbox\" value=\"\"  class=\"form-control textfields\" style=\"width:10%!important;\"/>";
        }
        SearchFields += "   </div>";
        SearchFields += "</div>";
    });
    SearchFields += "   </div>";
    SearchFields += "</div>";

    $("#divAssignEmployeeFields").html(SearchFields);
    var ShowPosition = JSON.parse($("#hdnInitialValues").val());
    if ($("#hdnSerialNoFlag").val() == "Y")
        $("#txt" + PositionField).val((parseInt(ShowPosition.OprLevelId) - 100000).toString());
    else
        $("#txt" + PositionField).val(ShowPosition.OprLevelId);

    GridViewEmployee.PerformCallback();
}

function ShowInputFilter() {
    var SelectedBoxes = [];
    $('.checkmarksearch:checkbox:checked').each(function () {
        SelectedBoxes.push($(this).val());
    });

    var SearchFields = "", DisplayField = "";
    var ShowSearch = JSON.parse($("#hdnSearchMenu").val());
    if (ShowSearch.length >= 1) {
        for (var Idx = 0; Idx <= ShowSearch.length - 1; Idx++) {
            ShowSearch[Idx].SearchFlag = "N";
            if (SelectedBoxes.indexOf(ShowSearch[Idx].SearchField) != -1) {
                ShowSearch[Idx].SearchFlag = "Y";
            }
        }
    }
    $("#hdnSearchMenu").val(JSON.stringify(ShowSearch));

    SearchFields += "<div class=\"row\">";
    SearchFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
    SearchFields += "        <span style=\"float:left;font-weight:bold;padding-right:10px;\">Selected Search Fields</span>";
    SearchFields += "        <img style=\"float:left;display:none;\" alt=\"show and hide Search Fields\" src=\"/Content/Images/downarrow.jpg\" onclick=\"ShowHide('divSelectedSearchFields', this)\" />";
    SearchFields += "   </div>";
    SearchFields += "   <div id=\"divSelectedSearchFields\" class=\"col-md-12\" style=\"margin: 0px 0px;\">";
    $.each(ShowSearch, function (key, item) {
        if (item.SearchField != "") {
            if (item.SearchFlag == "Y") DisplayField = ""; else DisplayField = "display:none;";
            SearchFields += "<div class=\"col-md-4\" style=\"margin: 0px 0px;padding-bottom:5px;" + DisplayField + "\">";
            SearchFields += "   <div style=\"width:100%;text-align:left;\">" + item.FieldCaption + "</div>";
            SearchFields += "   <div style=\"width:100%;padding:2px;\"><input data-id=\"" + item.SearchField + "\" id=\"div" + item.SearchField + "\" name=\"searchtext\" value=\"\"  class=\"form-control searchtext\" style=\"max-width:100%!important;\"/></div>";
            SearchFields += "</div>";
        }
    });
    SearchFields += "   </div>";
    SearchFields += "</div>";

    $("#mySearchMenu").html(SearchFields);
}

function SelectFilters() {
    if ($("#myFilterFields").html() == "") {
        var SearchFields = "", DisplayField = "";
        var ShowSearch = JSON.parse($("#hdnSearchMenu").val());

        SearchFields += "<div class=\"row\">";
        SearchFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
        SearchFields += "        <span style=\"float:left;font-weight:bold;padding-right:10px;\">List of Search Fields</span>";
        SearchFields += "        <img style=\"float:left;display:none;\" alt=\"show and hide list of search fields\" src=\"/Content/Images/downarrow.jpg\" onclick=\"ShowHide('divListOfSearchFields')\" />";
        SearchFields += "   </div>";
        SearchFields += "   <div id=\"divListOfSearchFields\" class=\"col-md-12\" style=\"margin: 0px 0px;\">";
        $.each(ShowSearch, function (key, item) {
            if (item.SearchField != "") {
                if (item.SearchFlag == "Y") DisplayField = "checked=\"checked\""; else DisplayField = "";
                SearchFields += "<div class=\"col-md-4\" style=\"margin: 0px 0px;padding-bottom:5px;\">";
                SearchFields += "   <div style=\"width:100%;padding:2px;text-align:left;\">";
                SearchFields += "       <input " + DisplayField + " onclick=\"ShowInputFilter();\" type=\"checkbox\" data-id=\"" + item.SearchField + "\" id=\"div" + item.SearchField + "\" name=\"checkmarksearch\" value=\"" + item.SearchField + "\"  class=\"checkmarksearch\" style=\"width:10%!important;\"/>";
                SearchFields += "       <label for=\"div" + item.SearchField + "\">" + item.SearchField + "</label>";
                SearchFields += "   </div>";
                SearchFields += "</div>";
            }
        });
        SearchFields += "   </div>";
        SearchFields += "</div>";

        $("#myFilterFields").html(SearchFields);
    }
    else {

        var SelectedBoxes = [];
        $('.checkmarksearch:checkbox:checked').each(function () {
            SelectedBoxes.push($(this).val());
        });

        var SearchFields = "", DisplayField = "";
        var ShowSearch = JSON.parse($("#hdnSearchMenu").val());
        if (ShowSearch.length >= 1) {
            for (var Idx = 0; Idx <= ShowSearch.length - 1; Idx++) {
                ShowSearch[Idx].SearchFlag = "N";
                if (SelectedBoxes.indexOf(ShowSearch[Idx].SearchField) != -1) {
                    ShowSearch[Idx].SearchFlag = "Y";
                }
            }
        }
        $("#hdnSearchMenu").val(JSON.stringify(ShowSearch));

        SearchFields += "<div class=\"row\">";
        SearchFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
        SearchFields += "        <span style=\"float:left;font-weight:bold;padding-right:10px;\">Selected Search Fields</span>";
        SearchFields += "        <img style=\"float:left;display:none\" alt=\"show and hide Search Fields\" src=\"/Content/Images/downarrow.jpg\" onclick=\"ShowHide('divSelectedSearchFields', this)\" />";
        SearchFields += "   </div>";
        SearchFields += "   <div id=\"divSelectedSearchFields\" class=\"col-md-12\" style=\"margin: 0px 0px;\">";
        $.each(ShowSearch, function (key, item) {
            if (item.SearchField != "") {
                if (item.SearchFlag == "Y") DisplayField = ""; else DisplayField = "display:none;";
                SearchFields += "<div class=\"col-md-4\" style=\"margin: 0px 0px;padding-bottom:5px;" + DisplayField + "\">";
                SearchFields += "   <div style=\"width:100%;text-align:left;\">" + item.FieldCaption + "</div>";
                SearchFields += "   <div style=\"width:100%;padding:2px;\"><input data-id=\"" + item.SearchField + "\" id=\"div" + item.SearchField + "\" name=\"searchtext\" value=\"\"  class=\"form-control searchtext\" style=\"max-width:100%!important;\"/></div>";
                SearchFields += "</div>";
            }
        });
        SearchFields += "   </div>";
        SearchFields += "</div>";

        $("#mySearchMenu").html(SearchFields);
        $("#myFilterFields").empty();
    }
}

function GetDisplayUserRole(Role) {
    var TABLES = JSON.parse($("#hdnOrgMenu").val());
    var MENU_TABLE = TABLES.MT;
    if (MENU_TABLE.length >= 1) {
        for (var Idx = 0; Idx <= MENU_TABLE.length - 1; Idx++) {
            if (MENU_TABLE[Idx].Role == Role) {
                return (MENU_TABLE[Idx].DisplayName);
            }
        }
    }

    return Role;
}

function ShowHide(Id, Obj) {
    if ($("#" + Id).is(":visible")) {
        $("#" + Id).fadeOut();
    }
    else $("#" + Id).fadeIn();
}

// when a Levels changes
function ChangeLevels(SelectedLevel) {
    $('.overlay').show();

    var URL = HOST_ENV + "/Version/ChangeLevels";
    if ($("#hdnOrgRole").val().toUpperCase() == "USER") URL = HOST_ENV + "/Home/ChangeLevels";

    var JsonData = {
        UptoLevel: SelectedLevel
    };

    jQuery.ajax({
        type: "POST",
        url: URL,
        data: JsonData,
        async: true,
        dateType: "json",
    }).done(function (JsonString) {
        if (JsonString != "No Changes") {
            $("#hdnOrgChartData").val(JsonString);
            switch (SelectedLevel) {
                case "1":
                    $("#hdnOrgLevel").val("One");
                    break;
                case "2":
                    $("#hdnOrgLevel").val("Two");
                    break;
                case "3":
                    $("#hdnOrgLevel").val("Three");
                    break;
                case "4":
                    $("#hdnOrgLevel").val("Four");
                    break;
                case "5":
                    $("#hdnOrgLevel").val("Five");
                    break;
                case "6":
                    $("#hdnOrgLevel").val("All");
                    break;
            }

            CancelOperation();
        }
        $('.overlay').hide();
    });
}

var SearchFieldValue = [];
function ShowSearchInformation() {
    $(".overlay").show();

    SearchFieldValue = [];
    $("input.searchtext").each(function () {
        SearchFieldValue.push({ "FieldName": $(this).attr("data-id"), "FieldValue": $(this).val() });
    });

    var JsonData = {
        IV: JSON.stringify(SearchFieldValue)
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/ShowSearchInformation",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            GridViewPartial.PerformCallback();
            $("#divSearchTable").show();
            $(".overlay").hide();
        }
    });
}


function SaveScreenSettings() {
    $(".overlay").show();

    DragDropSave();

    Settings.SelectShape = $("input[name='shape']:checked").val();
    $("#hdnSelectedShape").val(Settings.SelectShape);
    Settings.Skin = $("input[name='skin']:checked").val();
    $("#hdnSelectedSkin").val(Settings.Skin);
    Settings.ShowPicture = $("input[name='picture']:checked").val();
    $("#SelectedShowPicture").val(Settings.ShowPicture);
    Settings.SelectView = $("#selView").val();
    $("#hdnOrgView").val(Settings.SelectView);
    Settings.OrgChartType = $("#selOrgChartType").val();
    $("#hdnOrgChartType").val(Settings.OrgChartType);
    Settings.SplitScreenDirection = $("input[name='splitscreendirection']:checked").val();
    $("#hdnSelectedSplitScreen").val(Settings.SplitScreen);
    $("#hdnSelectedSplitScreenDirection").val(Settings.SplitScreenDirection);
    $("#hdnSelectedTextColor").val(Settings.TextColor);
    Settings.BorderColor = $('#cpInline2').colorpicker("val");
    $("#hdnSelectedBorderColor").val(Settings.BorderColor);
    $("#hdnSelectedBorderWidth").val(Settings.BorderWidth);
    Settings.LineColor = $('#cpInline3').colorpicker("val");
    $("#hdnSelectedLineColor").val(Settings.LineColor);
    Settings.BoxWidth = $("#selNodeSize").val();
    $("#hdnSelectedBoxWidth").val(Settings.BoxWidth);
    Settings.PortraitModeMultipleLevel = $("#selPortraitModeMultipleLevel").val();
    $("#hdnSelectedPortraitModeMultipleLevel").val(Settings.PortraitModeMultipleLevel);
    Settings.FunctionalManagerType = $("input[name='functionalmanagertype']:checked").val();
    $("#hdnSelectedFunctionalManagerType").val(Settings.FunctionalManagerType);

    switch ($("#myLevel").val()) {
        case "1":
            $("#hdnOrgLevel").val("One");
            break;
        case "2":
            $("#hdnOrgLevel").val("Two");
            break;
        case "3":
            $("#hdnOrgLevel").val("Three");
            break;
        case "4":
            $("#hdnOrgLevel").val("Four");
            break;
        case "5":
            $("#hdnOrgLevel").val("Five");
            break;
        case "All":
            $("#hdnOrgLevel").val("All");
            break;
    }

    var JsonData = {
        KeyDate: $("#hdnOrgKeyDate").val(),
        UsedView: $("#hdnOrgView").val(),
        Country: $("#hdnOrgCountry").val(),
        ShowLevel: $("#hdnOrgShowLevel").val(),
        Levels: $("#hdnOrgLevel").val(),
        Oper: $("#hdnOrgType").val(),
        Version: $("#hdnOrgVersion").val(),
        Role: $("#hdnOrgRole").val(),
        SelectedShape: $("#hdnSelectedShape").val(),
        SelectedSkin: $("#hdnSelectedSkin").val(),
        SelectedShowPicture: $("#SelectedShowPicture").val(),
        SelectedSplitScreen: $("#hdnSelectedSplitScreen").val(),
        SelectedSplitScreenDirection: $("#hdnSelectedSplitScreenDirection").val(),
        SelectedTextColor: $("#hdnSelectedTextColor").val(),
        SelectedBorderColor: $("#hdnSelectedBorderColor").val(),
        SelectedBorderWidth: $("#hdnSelectedBorderWidth").val(),
        SelectedLineColor: $("#hdnSelectedLineColor").val(),
        SelectedBoxWidth: $("#hdnSelectedBoxWidth").val(),
        SelectedPortraitModeMultipleLevel: $("#hdnSelectedPortraitModeMultipleLevel").val(),
        SelectedFunctionalManagerType: $("#hdnSelectedFunctionalManagerType").val(),
        OrgChartType: $("#hdnOrgChartType").val(),
        Type: "Settings"
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Home/SetSelectedValues",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            if ($("#hdnOrgType").val() == "OV") {
                $("#divRightOrgChart").show();
                $("#divShowMap").hide();

                $("#hdnOrgChartData").val(Json.ChartData);
                $("#hdnOrgTreeData").val(Json.TreeData);
                ShowHierarchy();
                CancelOperation();
            }
            else if ($("#hdnOrgType").val() == "LV") {
                $("#divRightOrgChart").hide();
                $("#divShowMap").show();
            }
            $(".overlay").hide();
        }
    });
}

function CheckAlreadyExist(Value, AE) {
    if (Value) {
        if (AE.length >= 1) {
            for (var Idy = 0; Idy <= AE.length - 1; Idy++) {
                if (AE[Idy] == Value) return true;
            }
        }
    }
    else return true;

    return false;
}

// Populate Options in respective DDLs
function ShowDDL(AEI, AEP, AEU, AEV, FlagPartition) {

    var SelectDDL = "";

    // Initiative DDL
    var SelectI = "<option value=\"SelectInitiative\">SELECT INITIATIVE</option>" +
        "<option value=\"AddInitiative\">ADD INITIATIVE</option>";
    if (FlagPartition == "Y" || $("#hdnOrgRole").val().toUpperCase() == "USER") SelectI = "<option value=\"SelectInitiative\">SELECT INITIATIVE</option>";

    if (AEI.length >= 1) {
        for (var Idx = 0; Idx <= AEI.length - 1; Idx++) {
            if (AEI[Idx] != "") {
                SelectI += "<option value=\"" + AEI[Idx] + "\">INITIATIVE::" + AEI[Idx] + "</option>";
            }
        }
    }
    if (FlagPartition == "Y") SelectDDL = '#selPartitionInitiative'; else SelectDDL = '#selInitiative';
    $(SelectDDL)
        .find('option')
        .remove()
        .end()
        .append(SelectI)
        .val('SelectInitiative');

    // Population DDL
    var SelectP = "<option value=\"SelectPopulation\">SELECT POPULATION</option>" +
        "<option value=\"AddPopulation\">ADD POPULATION</option>";
    if (FlagPartition == "Y" || $("#hdnOrgRole").val().toUpperCase() == "USER") SelectP = "<option value=\"SelectPopulation\">SELECT POPULATION</option>";

    if (AEP.length >= 1) {
        for (var Idx = 0; Idx <= AEP.length - 1; Idx++) {
            if (AEP[Idx] != "") {
                SelectP += "<option value=\"" + AEP[Idx] + "\">POPULATION::" + AEP[Idx] + "</option>";
            }
        }
    }
    if (FlagPartition == "Y") SelectDDL = '#selPartitionPopulation'; else SelectDDL = '#selPopulation';
    $(SelectDDL)
        .find('option')
        .remove()
        .end()
        .append(SelectP)
        .val('SelectPopulation');

    // User DDL
    var UserName = $("#hdnUserId").val();
    var SelectU = "<option value=\"SelectUser\">SELECT USER</option>" +
        "<option value=\"" + UserName + "\">USER::" + UserName.toUpperCase() + "</option>";

    if (AEU.length >= 1) {
        for (var Idx = 0; Idx <= AEU.length - 1; Idx++) {
            if (AEU[Idx] != "" && AEU[Idx] != UserName) {
                SelectU += "<option value=\"" + AEU[Idx] + "\">USER::" + AEU[Idx].toUpperCase() + "</option>";
            }
        }
    }
    if (FlagPartition == "Y") SelectDDL = '#selPartitionUser'; else SelectDDL = '#selUser';
    $(SelectDDL)
        .find('option')
        .remove()
        .end()
        .append(SelectU)
        .val('SelectUser');

    // Version DDL
    var SelectV = "<option value=\"SelectVersion\">SELECT VERSION</option>";
    if ($("#hdnHRCoreVersion").val() != "NOT AN HR CORE VERSION")
        SelectV += "<option value=\"" + $("#hdnHRCoreVersion").val() + "\">VERSION::" + $("#hdnHRCoreVersion").val() + "</option>";

    if (FlagPartition == "Y") {
        SelectV = "<option value=\"SelectVersion\">SELECT VERSION</option>" +
            "<option value=\"" + $("#hdnHRCoreVersion").val() + "\">VERSION::" + $("#hdnHRCoreVersion").val() + "</option>";
    }

    if (AEV.length >= 1) {
        for (var Idx = 0; Idx <= AEV.length - 1; Idx++) {
            if (AEV[Idx] != "") {
                if ($("#hdnHRCoreVersion").val() != AEV[Idx])
                    SelectV += "<option value=\"" + AEV[Idx] + "\">VERSION::" + AEV[Idx] + "</option>";
            }
        }
    }
    if (FlagPartition == "Y") SelectDDL = '#selPartitionVersion'; else SelectDDL = '#selVersion';
    $(SelectDDL)
        .find('option')
        .remove()
        .end()
        .append(SelectV)
        .val('SelectVersion');
}

var AEP = [], AEU = [], AEV = [];
function SelectedCountryDDL(Result, CValue, FlagPartition) {
    $(".overlay").show();

    var JSONResult = JSON.parse(Result);
    var Value = "";

    AEI = [], AEP = [], AEU = [], AEV = [];
    if (JSONResult.length >= 1) {
        for (var Idx = 0; Idx <= JSONResult.length - 1; Idx++) {
            if (JSONResult[Idx].Country == CValue) {
                if (JSONResult[Idx].Initiative) {
                    if (!CheckAlreadyExist(JSONResult[Idx].Initiative, AEI)) {
                        AEI.push(JSONResult[Idx].Initiative);
                    }
                }
                if (JSONResult[Idx].Population) {
                    if (!CheckAlreadyExist(JSONResult[Idx].Population, AEP)) {
                        AEP.push(JSONResult[Idx].Population);
                    }
                }

                if (JSONResult[Idx].UserName) {
                    if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) {
                        if ($("#hdnUserId").val() != JSONResult[Idx].UserName) AEU.push(JSONResult[Idx].UserName);
                    }
                }

                if (JSONResult[Idx].Version) {
                    if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) {
                        AEV.push(JSONResult[Idx].Version);
                    }
                }
            }
        }
    }

    if (FlagPartition == "Y") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selPartitionCountry').select2("val", CValue);
        $('#selPartitionInitiative').select2("val", "SelectInitiative");
        $('#selPartitionPopulation').select2("val", "SelectPopulation");
        $('#selPartitionUser').select2("val", $("#hdnUserId").val());
        $('#selPartitionVersion').select2("val", "SelectVersion");
    }
    else if (FlagPartition == "N") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selCountry').select2("val", CValue);
        $('#selInitiative').select2("val", "SelectInitiative");
        $('#selPopulation').select2("val", "SelectPopulation");
        $('#selUser').select2("val", $("#hdnUserId").val());
        $('#selVersion').select2("val", "SelectVersion");
    }

    $(".overlay").hide();
}

function SelectedInitiativeDDL(Result, CValue, FlagPartition) {
    $(".overlay").show();

    var JSONResult = JSON.parse(Result);
    var Value = "";

    AEI = [], AEP = [], AEU = [], AEV = [];
    if (JSONResult.length >= 1) {
        for (var Idx = 0; Idx <= JSONResult.length - 1; Idx++) {
            if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) AEU.push(JSONResult[Idx].UserName);
            //if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) AEV.push(JSONResult[Idx].Version);
            if (($("#hdnOrgType").val() == "OV" && JSONResult[Idx].Country == "CH") ||
                ($("#hdnOrgType").val() == "LV" &&
                    ((JSONResult[Idx].Country == $("#selCountry").val()) ||
                        (JSONResult[Idx].Country == "CH" && $("#selCountry").val() == "SelectCountry" && ($("#hdnOrgRole").val() == "Finalyzer" || $("#hdnOrgRole").val() == "User"))))) {
                if (!CheckAlreadyExist(JSONResult[Idx].Initiative, AEI)) AEI.push(JSONResult[Idx].Initiative);
                if (JSONResult[Idx].Initiative == CValue) {
                    if (JSONResult[Idx].Population) {
                        if (!CheckAlreadyExist(JSONResult[Idx].Population, AEP)) {
                            AEP.push(JSONResult[Idx].Population);
                        }

                        if (JSONResult[Idx].UserName) {
                            if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) {
                                if ($("#hdnUserId").val() != JSONResult[Idx].UserName) AEU.push(JSONResult[Idx].UserName);
                            }

                            if (JSONResult[Idx].Version) {
                                if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) {
                                    AEV.push(JSONResult[Idx].Version);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //alert(FlagPartition);
    if (FlagPartition == "Y") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selPartitionInitiative').select2("val", CValue);
        $('#selPartitionPopulation').select2("val", "SelectPopulation");
        $('#selPartitionUser').select2("val", $("#hdnUserId").val());
        $('#selPartitionVersion').select2("val", "SelectVersion");
    }
    else if (FlagPartition == "N") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selInitiative').select2("val", CValue);
        $('#selPopulation').select2("val", "SelectPopulation");
        $('#selUser').select2("val", $("#hdnUserId").val());
        $('#selVersion').select2("val", "SelectVersion");
    }

    $(".overlay").hide();
}

function SelectedPopulationDDL(Result, CValue, IValue, FlagPartition) {
    $(".overlay").show();

    var JSONResult = JSON.parse(Result);
    var Value = "";

    AEI = [], AEP = [], AEU = [], AEV = [];
    if (JSONResult.length >= 1) {
        for (var Idx = 0; Idx <= JSONResult.length - 1; Idx++) {
            if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) AEU.push(JSONResult[Idx].UserName);
            //if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) AEV.push(JSONResult[Idx].Version);
            if (($("#hdnOrgType").val() == "OV" && JSONResult[Idx].Country == "CH") ||
                ($("#hdnOrgType").val() == "LV" &&
                    ((JSONResult[Idx].Country == $("#selCountry").val()) ||
                        (JSONResult[Idx].Country == "CH" && $("#selCountry").val() == "SelectCountry" && ($("#hdnOrgRole").val() == "Finalyzer" || $("#hdnOrgRole").val() == "User"))))) {
                if (!CheckAlreadyExist(JSONResult[Idx].Initiative, AEI)) AEI.push(JSONResult[Idx].Initiative);
                if (JSONResult[Idx].Initiative == IValue) {
                    if (JSONResult[Idx].Population) {
                        if (!CheckAlreadyExist(JSONResult[Idx].Population, AEP)) {
                            AEP.push(JSONResult[Idx].Population);
                        }

                        if (JSONResult[Idx].Population == CValue) {
                            if (JSONResult[Idx].UserName) {
                                if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) {
                                    if ($("#hdnUserId").val() != JSONResult[Idx].UserName) AEU.push(JSONResult[Idx].UserName);
                                }

                                if (JSONResult[Idx].Version) {
                                    if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) {
                                        AEV.push(JSONResult[Idx].Version);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    if (FlagPartition == "Y") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selPartitionInitiative').select2("val", IValue);
        $('#selPartitionPopulation').select2("val", CValue);
        $('#selPartitionUser').select2("val", $("#hdnUserId").val());
        $('#selPartitionVersion').select2("val", "SelectVersion");
    }
    else if (FlagPartition == "N") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selInitiative').select2("val", IValue);
        $('#selPopulation').select2("val", CValue);
        $('#selUser').select2("val", $("#hdnUserId").val());
        $('#selVersion').select2("val", "SelectVersion");
    }

    $(".overlay").hide();
}

function SelectedUserDDL(Result, CValue, PValue, IValue, FlagPartition) {
    $(".overlay").show();

    var JSONResult = JSON.parse(Result);
    var Value = "";

    AEI = [], AEP = [], AEU = [], AEV = [];
    if (JSONResult.length >= 1) {
        for (var Idx = 0; Idx <= JSONResult.length - 1; Idx++) {
            if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) AEU.push(JSONResult[Idx].UserName);
            //if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) AEV.push(JSONResult[Idx].Version);
            if (($("#hdnOrgType").val() == "OV" && JSONResult[Idx].Country == "CH") ||
                ($("#hdnOrgType").val() == "LV" &&
                    ((JSONResult[Idx].Country == $("#selCountry").val()) ||
                        (JSONResult[Idx].Country == "CH" && $("#selCountry").val() == "SelectCountry" && ($("#hdnOrgRole").val() == "Finalyzer" || $("#hdnOrgRole").val() == "User"))))) {
                if (!CheckAlreadyExist(JSONResult[Idx].Initiative, AEI)) {
                    AEI.push(JSONResult[Idx].Initiative);
                    if (JSONResult[Idx].Initiative == IValue) {
                        if (JSONResult[Idx].Population) {
                            if (!CheckAlreadyExist(JSONResult[Idx].Population, AEP)) {
                                AEP.push(JSONResult[Idx].Population);
                            }

                            if (JSONResult[Idx].Population == PValue) {
                                if (JSONResult[Idx].UserName) {
                                    if (!CheckAlreadyExist(JSONResult[Idx].UserName, AEU)) {
                                        if ($("#hdnUserId").val() != JSONResult[Idx].UserName) AEU.push(JSONResult[Idx].UserName);
                                    }

                                    if (JSONResult[Idx].UserName == CValue) {
                                        if (JSONResult[Idx].Version) {
                                            if (!CheckAlreadyExist(JSONResult[Idx].Version, AEV)) {
                                                AEV.push(JSONResult[Idx].Version);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    if (FlagPartition == "Y") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selPartitionInitiative').select2("val", IValue);
        $('#selPartitionPopulation').select2("val", PValue);
        $('#selPartitionUser').select2("val", CValue);
        $('#selPartitionVersion').select2("val", "SelectVersion");
    }
    else if (FlagPartition == "N") {
        ShowDDL(AEI, AEP, AEU, AEV, FlagPartition); // Populate Options
        $('#selInitiative').select2("val", IValue);
        $('#selPopulation').select2("val", PValue);
        $('#selUser').select2("val", CValue);
        $('#selVersion').select2("val", "SelectVersion");
    }

    $(".overlay").hide();

}

function SaveInitiative() {
    $(".overlay").show();

    var MessageError = "";
    $("#divRepInitiativeHeader").html("Message");
    if ($("#InitiativeTitle").val() == "") {
        MessageError += "<div style=\"width:100%\">* Initiative Title should not be empty</div>";
    }
    if ($("#InitiativeDescription").val() == "") {
        MessageError += "<div style=\"width:100%\">* Initiative Description should not be empty</div>";
    }
    if (MessageError == "") {
        var JsonData = {
            Name: $("#InitiativeTitle").val(),
            Description: $("#InitiativeDescription").val()
        };
        $.ajax({
            type: "POST",
            url: HOST_ENV + "/Version/SaveInitiative",
            data: JsonData,
            async: true,
            dateType: "json",
            success: function (Json) {
                $("#divRepInitiativeError").html(Json.ShowMessage);
                if (Json.Success == "Yes") {
                    $("#hdnOrgDDL").val(Json.DDL);
                    SelectedInitiativeDDL(Json.DDL, $("#InitiativeTitle").val(), "N");
                }
                $(".overlay").hide();
            }
        });
    }
    else {
        $("#divRepInitiativeError").html(MessageError);
        $(".overlay").hide();
    }

    return false;
}

function SaveVersionTitle() {

    $(".overlay").show();
    var MessageError = "";
    $("#divRepVersionHeader").html("Message");
    if ($("#VersionTitleNew").val() == "") {
        MessageError += "<div style=\"width:100%\">* Version Title should not be empty</div>";
    }
    if ($("#VersionDescriptionNew").val() == "") {
        MessageError += "<div style=\"width:100%\">* Version Description should not be empty</div>";
    }
    if (MessageError == "") {
        var JsonData = {
            Initiative: $("#selInitiative").select2("val"),
            Population: $("#selPopulation").select2("val"),
            UserName: $("#selUser").select2("val"),
            VersionSelected: $('#selVersion').select2("val"),
            Name: $("#VersionTitleNew").val(),
            Description: $("#VersionDescriptionNew").val()
        };
        $.ajax({
            type: "POST",
            url: HOST_ENV + "/Version/SaveVersionTitle",
            data: JsonData,
            async: true,
            dateType: "json",
            success: function (Json) {
                if (Json.Success == "Yes") {
                    $("#divRepVersionError").html(Json.ShowMessage);
                    $("#hdnOrgDDL").val(Json.DDL);

                    $("#hdnSelectedInitiative").val(Json.Initiative);
                    SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#hdnSelectedInitiative").val(), "N");
                    if ($("#hdnOrgRole").val() == "Player" || $("#hdnOrgRole").val() == "Admin")
                        SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#hdnSelectedInitiative").val(), "Y");

                    $("#selInitiative").select2("val", Json.Initiative);
                    $("#selPopulation").select2("val", Json.Population);
                    $("#selUser").select2("val", Json.UserName);
                    $("#selVersion").select2("val", Json.Version);

                    $("#hdnSelectedInitiative").val($("#selInitiative").select2("val"));
                    $("#hdnSelectedPopulation").val($("#selPopulation").select2("val"));
                    $("#hdnSelectedUser").val($("#selUser").select2("val"));
                    $("#hdnSelectedVersion").val($("#selVersion").select2("val"));

                    $("#hdnOrgChartData").val(Json.ChartData);
                    $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                    $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
                    $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
                    $("#hdnOrgVersion").val(Json.UsedVersion);
                    $("#hdnOrgPartitionVersion").val(Json.UsedVersion);

                    try {
                        $("#div_OA_ShowChart").show();
                        $("#div_OA_ShowUploadData").hide();
                    }
                    catch (ex) {
                        Console.log(ex);
                    }
                    CancelOperation();
                    $(".overlay").hide();
                }
                else if (Json.Success == "No") {
                    $(".overlay").hide();
                    $("#divRepVersionError").html(Json.ShowMessage);
                }
            }
        });
    }
    else {
        $("#divRepVersionError").html(MessageError);
        $(".overlay").hide();
    }

    return false;
}

function ShowOrgChartPosition(SL, PL) {
    $(".overlay").show();

    var JsonData = {
        ShowLevel: SL,
        ParentLevel: PL
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/ChangeShowLevel",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            try {
                $("#div_OA_ShowChart").show();
                $("#div_OA_ShowUploadData").hide();
            }
            catch (ex) {
                Console.log(ex);
            }
            $("#hdnOrgTreeData").val(Json.TreeData);
            $("#hdnOrgChartData").val(Json.ChartData);
            $("#hdnOrgChartHRCoreData").val(Json.ChartData);
            $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgVersion").val(Json.UsedVersion);
            $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            CancelOperation();
            $(".overlay").hide();
        }
    });

    return false;
}

function ShowRightOrgChartPosition(SL, PL) {
    $(".overlay").show();

    var JsonData = {
        ShowLevel: SL,
        ParentLevel: PL
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/ChangeShowLevel",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            try {
                $("#div_OA_ShowChart").show();
                $("#div_OA_ShowUploadData").hide();
            }
            catch (ex) {
                Console.log(ex);
            }
            $("#hdnOrgTreeData").val(Json.TreeData);
            $("#hdnOrgChartData").val(Json.ChartData);
            $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgVersion").val(Json.UsedVersion);
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            $("#myDiagramDiv").detach();
            var iDiv = document.createElement('div');
            iDiv.id = 'myDiagramDiv';
            document.getElementById('myDiagramContainer').appendChild(iDiv);

            // Org chart in Canvas
            if (Settings.SplitScreenDirection == "Vertical")
                $("#divRightOrgChart").addClass("col-md-6");
            else {
                $("#divRightOrgChart").addClass("col-md-12");
            }
            h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
            w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";
            if (Settings.SplitScreenDirection == "Vertical") init(w, h, DragDrop); else init(w, 250, DragDrop);

            // Bread Gram
            var OrgData = JSON.parse($("#hdnOrgChartData").val());
            $.each(OrgData, function (key, item) {
                if (item.LEVEL_ID == $("#hdnOrgShowLevel").val()) {
                    var BREAD_GRAM = item.BREAD_GRAM.split("-->");
                    var BREAD_GRAM_NAME = item.BREAD_GRAM_NAME.split("-->");
                    var BREAD_GRAM_INFO = "";
                    for (var Idx = 0; Idx <= BREAD_GRAM_NAME.length - 1; Idx++) {
                        BREAD_GRAM_INFO += " >> <a href=\"javascript:void(0);\" onclick=\"ShowRightOrgChartPosition('" + BREAD_GRAM[Idx] + "','" + BREAD_GRAM[Idx] + "')\">" + BREAD_GRAM_NAME[Idx] + "</a>";
                    }
                    BREAD_GRAM_INFO = BREAD_GRAM_INFO.substring(4);
                    $("#divBreadCrumbs").html("Location: <span style='font-style:italic;'>" + BREAD_GRAM_INFO + "</span>");
                }
            });

            $(".overlay").hide();
        }
    });

    return false;
}

function LoadEndUserVersion() {
    $(".overlay").show();

    var JsonData = {
        VersionName: $("#selEndUserVersion").val()
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/LoadEndUserVersion",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            try {
                $("#div_OA_ShowChart").show();
                $("#div_OA_ShowUploadData").hide();
            }
            catch (ex) {
            }

            $("#hdnOrgChartData").val(Json.ChartData);
            $("#hdnOrgChartHRCoreData").val(Json.ChartData);
            $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgVersion").val(Json.UsedVersion);
            $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            CancelOperation();
            if (Json.UsedRole == "Finalyzer") {
                if (Json.FV == "Yes") {
                    $(".splitscreen").show();
                    $(".saveversion").hide();
                }
                else {
                    $(".splitscreen").hide();
                    $(".saveversion").show();
                }
            }
            $(".overlay").hide();
        }
    });

    return false;
}

function LoadSelectedVersion() {
    $(".overlay").show();

    var JsonData = {
        Country: $("#selCountry").val(),
        Initiative: $("#selInitiative").val(),
        Population: $("#selPopulation").val(),
        UserName: $("#selUser").val(),
        VersionName: $("#selVersion").val()
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/LoadSelectedVersion",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            try {
                $("#div_OA_ShowChart").show();
                $("#div_OA_ShowUploadData").hide();
            }
            catch (ex) {
            }

            $("#hdnOrgChartData").val(Json.ChartData);
            $("#hdnOrgTreeData").val(Json.TreeData);
            $("#hdnOrgChartHRCoreData").val(Json.ChartData);
            $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgVersion").val(Json.UsedVersion);
            $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            CancelOperation();
            if (Json.UsedRole == "Finalyzer") {
                if (Json.FV == "Yes") {
                    $(".splitscreen").show();
                    $(".saveversion").hide();
                }
                else {
                    $(".splitscreen").hide();
                    $(".saveversion").show();
                }
            }
            ShowHierarchy();
            $(".overlay").hide();
        }
    });

    return false;
}

function LoadSelectedUserVersion() {
    $(".overlay").show();

    var JsonData = {
        Country: $("#selCountry").val(),
        Initiative: $("#selInitiative").val(),
        Population: $("#selPopulation").val(),
        UserName: $("#selUser").val(),
        VersionName: $("#selVersion").val()
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/LoadSelectedVersion",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            $("#hdnOrgChartData").val(Json.ChartData);
            $("#hdnOrgChartHRCoreData").val(Json.ChartData);
            $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
            $("#hdnOrgVersion").val(Json.UsedVersion);
            $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
            document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

            CancelOperation();
            $(".overlay").hide();
        }
    });

    return false;
}

function CancelOperation() {
    if ($("#hdnOrgType").val() == "LV") {
        $("#divShowMap").hide();
        $("#divAddInitiative").hide();
        $("#divAddPopulation").show();
        $("#divAddVersion").hide();
    }
    $("#divRightOrgChart").removeClass("col-md-12");
    $("#divRightOrgChart").removeClass("col-md-8");
    $("#divRightOrgChart").removeClass("col-md-6");
    $("#divRightOrgChart").addClass("col-md-12");

    $("#divAddInitiative").hide();
    $("#divAddVersion").hide();
    $("#divRightOrgChart").show();
    $("#divLeftOrgChart").hide();
    $("#divUploadVersion").hide();

    $("#myOverviewDiv").detach();
    var iDiv = document.createElement('div');
    iDiv.id = 'myOverviewDiv';
    document.getElementById('myDiagramContainer').appendChild(iDiv);
    $("#myOverviewDiv").addClass("showoverviewdiv");


    $("#myDiagramDiv").detach();
    iDiv = document.createElement('div');
    iDiv.id = 'myDiagramDiv';
    document.getElementById('myDiagramContainer').appendChild(iDiv);

    // Org chart in Canvas
    h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
    w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
    document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";
    if ($("#hdnOrgChartType").val() == "OD") {
        init(w, h, DragDrop);
        $("#myOverviewDiv").hide();
        $("#divSearchDiagram").hide();
        $("#divBreadGram").removeClass("breadcrumbs");
        $("#divBreadGram").removeClass("breadcrumbsbig");
        $("#divBreadGram").addClass("breadcrumbs");
    }
    else if ($("#hdnOrgChartType").val() == "OS") {
        initOrgChartStatic($("#hdnOrgChartData").val(), w, h, DragDrop);
        $("#myOverviewDiv").show();
        $("#divSearchDiagram").show();
        $("#divBreadGram").removeClass("breadcrumbs");
        $("#divBreadGram").removeClass("breadcrumbsbig");
        $("#divBreadGram").addClass("breadcrumbsbig");
    }
    else if ($("#hdnOrgChartType").val() == "OC") {
        initCustom(w, h, DragDrop);
        $("#myOverviewDiv").hide();
        $("#divSearchDiagram").hide();
        $("#divBreadGram").removeClass("breadcrumbs");
        $("#divBreadGram").removeClass("breadcrumbsbig");
        $("#divBreadGram").addClass("breadcrumbs");
    }

    SplitScreen = "Y";

    // Bread Gram
    var OrgData = JSON.parse($("#hdnOrgChartData").val());
    $.each(OrgData, function (key, item) {
        if (item.LEVEL_ID == $("#hdnOrgShowLevel").val()) {
            var BREAD_GRAM = item.BREAD_GRAM.split("-->");
            var BREAD_GRAM_NAME = item.BREAD_GRAM_NAME.split("-->");
            var BREAD_GRAM_INFO = "";
            for (var Idx = 0; Idx <= BREAD_GRAM_NAME.length - 1; Idx++) {
                BREAD_GRAM_INFO += " >> <a href=\"javascript:void(0);\" onclick=\"ShowOrgChartPosition('" + BREAD_GRAM[Idx] + "','" + BREAD_GRAM[Idx] + "')\">" + BREAD_GRAM_NAME[Idx] + "</a>";
            }
            BREAD_GRAM_INFO = BREAD_GRAM_INFO.substring(4);
            $("#divBreadCrumbs").html("Location: <span style='font-style:italic;'>" + BREAD_GRAM_INFO + "</span>");
        }
    });
}

function ToggleSplitScreen(Obj) {
    $(Obj).blur();
    $(".overlay").show();

    if (SplitScreen == "Y") {
        $("#divRightOrgChart").removeClass("col-md-12");
        $("#divRightOrgChart").removeClass("col-md-8");
        $("#divRightOrgChart").removeClass("col-md-6");

        $("#divAddInitiative").hide();
        $("#divAddVersion").hide();
        $("#divRightOrgChart").show();
        $("#divLeftOrgChart").show();
        $("#divUploadVersion").hide();

        $("#myOverviewDiv").detach();
        var iDiv = document.createElement('div');
        iDiv.id = 'myOverviewDiv';
        document.getElementById('myDiagramContainer').appendChild(iDiv);
        $("#myOverviewDiv").addClass("showoverviewdiv");

        $("#myDiagramDiv").detach();
        var iDiv = document.createElement('div');
        iDiv.id = 'myDiagramDiv';
        document.getElementById('myDiagramContainer').appendChild(iDiv);

        // Org chart in Canvas
        if (Settings.SplitScreenDirection == "Vertical")
            $("#divRightOrgChart").addClass("col-md-6");
        else {
            $("#divRightOrgChart").addClass("col-md-12");
        }
        h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
        w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
        var hdnOrgChartData = JSON.parse($("#hdnOrgChartData").val());
        if (hdnOrgChartData.length >= 1) {
            for (var idr = hdnOrgChartData.length - 1; idr >= 0; idr--) {
                if (!(document.getElementById("hdnOrgShowLevel").value == hdnOrgChartData[idr].LEVEL_ID ||
                    document.getElementById("hdnOrgShowLevel").value == hdnOrgChartData[idr].PARENT_LEVEL_ID)) {
                    hdnOrgChartData.splice(idr, 1);
                }
                else if (hdnOrgChartData[idr].DOTTED_LINE_FLAG == "Y") {
                    hdnOrgChartData.splice(idr, 1);
                }
            }
        }
        document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + JSON.stringify(hdnOrgChartData) + " }";
        $("#myOverviewDiv").hide();
        $("#divSearchDiagram").hide();
        if (Settings.SplitScreenDirection == "Vertical") {
            init(w, h, DragDrop);
        }
        else {
            init(w, 250, DragDrop);
        }

        // Bread Gram
        var OrgData = JSON.parse($("#hdnOrgChartData").val());
        $.each(OrgData, function (key, item) {
            if (item.LEVEL_ID == $("#hdnOrgShowLevel").val()) {
                var BREAD_GRAM = item.BREAD_GRAM.split("-->");
                var BREAD_GRAM_NAME = item.BREAD_GRAM_NAME.split("-->");
                var BREAD_GRAM_INFO = "";
                for (var Idx = 0; Idx <= BREAD_GRAM_NAME.length - 1; Idx++) {
                    BREAD_GRAM_INFO += " >> <a href=\"javascript:void(0);\" onclick=\"ShowRightOrgChartPosition('" + BREAD_GRAM[Idx] + "','" + BREAD_GRAM[Idx] + "')\">" + BREAD_GRAM_NAME[Idx] + "</a>";
                }
                BREAD_GRAM_INFO = BREAD_GRAM_INFO.substring(4);
                $("#divBreadCrumbs").html("Location: <span style='font-style:italic;'>" + BREAD_GRAM_INFO + "</span>");
            }
        });

        $("#myPartitionDiagramDiv").detach();
        var iDiv = document.createElement('div');
        iDiv.id = 'myPartitionDiagramDiv';
        document.getElementById('myPartitionDiagramContainer').appendChild(iDiv);

        // Org chart in Canvas
        $("#divLeftOrgChart").removeClass("col-md-12");
        $("#divLeftOrgChart").removeClass("col-md-8");
        $("#divLeftOrgChart").removeClass("col-md-6");
        if (Settings.SplitScreenDirection == "Vertical")
            $("#divLeftOrgChart").addClass("col-md-6");
        else {
            $("#divLeftOrgChart").addClass("col-md-12");
        }
        h = parseInt($("#divLeftOrgChart").css("height").substr(0, $("#divLeftOrgChart").css("height").indexOf("px")));
        w = parseInt($("#divLeftOrgChart").css("width").substr(0, $("#divLeftOrgChart").css("width").indexOf("px")));
        var hdnOrgChartHRCoreData = JSON.parse($("#hdnOrgChartHRCoreData").val());
        if (hdnOrgChartData.length >= 1) {
            for (idr = hdnOrgChartHRCoreData.length - 1; idr >= 0; idr--) {
                if (!(document.getElementById("hdnOrgPartitionShowLevel").value == hdnOrgChartHRCoreData[idr].LEVEL_ID ||
                    document.getElementById("hdnOrgPartitionShowLevel").value == hdnOrgChartHRCoreData[idr].PARENT_LEVEL_ID)) {
                    hdnOrgChartHRCoreData.splice(idr, 1);
                }
                else if (hdnOrgChartHRCoreData[idr].DOTTED_LINE_FLAG == "Y") {
                    hdnOrgChartHRCoreData.splice(idr, 1);
                }
            }
        }
        document.getElementById("myPartitionSavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + JSON.stringify(hdnOrgChartHRCoreData) + " }";
        if (Settings.SplitScreenDirection == "Vertical") initPartition(w, h, DragDrop); else initPartition(w, 250, DragDrop);

        // Bread Gram
        var OrgData = JSON.parse($("#hdnOrgChartHRCoreData").val());
        $.each(OrgData, function (key, item) {
            if (item.LEVEL_ID == $("#hdnOrgPartitionShowLevel").val()) {
                var BREAD_GRAM = item.BREAD_GRAM.split("-->");
                var BREAD_GRAM_NAME = item.BREAD_GRAM_NAME.split("-->");
                var BREAD_GRAM_INFO = "";
                for (var Idx = 0; Idx <= BREAD_GRAM_NAME.length - 1; Idx++) {
                    BREAD_GRAM_INFO += " >> <a href=\"javascript:void(0);\" onclick=\"ShowOrgChartPartitionPosition('" + BREAD_GRAM[Idx] + "','" + BREAD_GRAM[Idx] + "')\">" + BREAD_GRAM_NAME[Idx] + "</a>";
                }
                BREAD_GRAM_INFO = BREAD_GRAM_INFO.substring(4);
                $("#divPartitionBreadCrumbs").html("Location: <span style='font-style:italic;'>" + BREAD_GRAM_INFO + "</span>");
            }
        });

        SplitScreen = "N";
        SetSplitScreen = "Y";
    }
    else if (SplitScreen == "N") {
        $("#divRightOrgChart").removeClass("col-md-12");
        $("#divRightOrgChart").removeClass("col-md-8");
        $("#divRightOrgChart").removeClass("col-md-6");
        $("#divRightOrgChart").addClass("col-md-12");

        $("#divAddInitiative").hide();
        $("#divAddVersion").hide();
        $("#divRightOrgChart").show();
        $("#divLeftOrgChart").hide();
        $("#divUploadVersion").hide();

        $("#myOverviewDiv").detach();
        var iDiv = document.createElement('div');
        iDiv.id = 'myOverviewDiv';
        document.getElementById('myDiagramContainer').appendChild(iDiv);
        $("#myOverviewDiv").addClass("showoverviewdiv");

        $("#myDiagramDiv").detach();
        var iDiv = document.createElement('div');
        iDiv.id = 'myDiagramDiv';
        document.getElementById('myDiagramContainer').appendChild(iDiv);

        // Org chart in Canvas
        h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
        w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
        if ($("#hdnOrgChartType").val() == "OD") {
            init(w, h, DragDrop);
            $("#myOverviewDiv").hide();
            $("#divSearchDiagram").hide();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbs");
        }
        else if ($("#hdnOrgChartType").val() == "OS") {
            initOrgChartStatic($("#hdnOrgChartData").val(), w, h, DragDrop);
            $("#myOverviewDiv").show();
            $("#divSearchDiagram").show();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbsbig");
        }
        else if ($("#hdnOrgChartType").val() == "OC") {
            init(w, h, DragDrop);
            $("#myOverviewDiv").hide();
            $("#divSearchDiagram").hide();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbs");
        }

        SplitScreen = "Y";
        SetSplitScreen = "N";

        // Bread Gram
        var OrgData = JSON.parse($("#hdnOrgChartData").val());
        $.each(OrgData, function (key, item) {
            if (item.LEVEL_ID == $("#hdnOrgShowLevel").val()) {
                var BREAD_GRAM = item.BREAD_GRAM.split("-->");
                var BREAD_GRAM_NAME = item.BREAD_GRAM_NAME.split("-->");
                var BREAD_GRAM_INFO = "";
                for (var Idx = 0; Idx <= BREAD_GRAM_NAME.length - 1; Idx++) {
                    BREAD_GRAM_INFO += " >> <a href=\"javascript:void(0);\" onclick=\"ShowOrgChartPosition('" + BREAD_GRAM[Idx] + "','" + BREAD_GRAM[Idx] + "')\">" + BREAD_GRAM_NAME[Idx] + "</a>";
                }
                BREAD_GRAM_INFO = BREAD_GRAM_INFO.substring(4);
                $("#divBreadCrumbs").html("Location: <span style='font-style:italic;'>" + BREAD_GRAM_INFO + "</span>");
            }
        });

        if ($("#hdnOrgChartType").val() == "OS") {
            $("#myOverviewDiv").show();
            $("#divSearchDiagram").show();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbsbig");
        }
        else if ($("#hdnOrgChartType").val() == "OD") {
            $("#myOverviewDiv").hide();
            $("#divSearchDiagram").hide();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbs");
        }
        else if ($("#hdnOrgChartType").val() == "OC") {
            $("#myOverviewDiv").hide();
            $("#divSearchDiagram").hide();
            $("#divBreadGram").removeClass("breadcrumbs");
            $("#divBreadGram").removeClass("breadcrumbsbig");
            $("#divBreadGram").addClass("breadcrumbs");
        }

    }

    $(".overlay").hide();
}

function UploadPopulation() {
    window.location.href = HOST_ENV + '/Version/UploadVersion';
}

function UploadTemplatePPTX() {
    $("#UploadTemplatePPTModal").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function UpdateSettings() {
    $("#SettingsModal").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function LoadPopupBox(popBoxID) {    // To Load the Popupbox
    $('#' + popBoxID).fadeIn("slow");
}

function UnloadPopupBox(popBoxID) {    // TO Unload the Popupbox
    $('#' + popBoxID).fadeOut("slow");
}

function SaveNewPosition(Obj) {
    $(".overlay").show();
    $("#divAddPositionError").hide();

    AssignEmployee = SaveAssignEmployee();
    var JsonData = {
        VersionName: $("#selVersion").select2("val"),
        PositionId: $("#txtPositionId").val(),
        NextPositionId: $("#txtNextPositionId").val(),
        FirstName: $("#txtFirstName").val(),
        LastName: $("#txtLastName").val(),
        PositionCost: $("#txtPositionCost").val(),
        AssignEmployee: JSON.stringify(AssignEmployee),
        SelectedDiv: $(".positionpopup.active").attr("data-id")
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/SaveNewPosition",
        data: JsonData,
        async: true,
        dateType: "json",
        success: function (Json) {
            if (Json.Success == "Success") {
                $(Obj).css("disabled", "disabled");
                $("#hdnOrgChartData").val(Json.Message);
                document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                CancelOperation();

                $("#hdnInitialValues").val(Json.InitialValues);
                var ShowPosition = JSON.parse($("#hdnInitialValues").val());
                $("#txtPositionId").val(ShowPosition.OprLevelId);
                $("#txtPositionId").attr("disabled", "disabled");

                $('#AddNewPositionModal').modal('toggle');
                ShowMessage(Json.Success, Json.ShowMessage);
            }
            else if (Json.Success == "Failure") {
                $('#AddNewPositionModal').modal('toggle');
                ShowMessage(Json.Success, Json.ShowMessage);
            }
            else if (Json.Success == "No") {
                $("#divAddPositionError").show();
                $("#divAddPositionError").html(Json.ShowMessage);
            }
            $(".overlay").hide();
        }
    });

    return false;
}

function ShowMultiSelectModel() {
    $("#MultiSelectModal").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function AddNewPositionModal(parent) {
    $("#divAddPositionError").hide();
    $("#txtPositionId").val("");
    $("#txtNextPositionId").val(parent);
    $("#txtFirstName").val("(Vacant)");
    $("#txtLastName").val("");
    $("#txtPositionCost").val("0.00");

    $(".overlay").show();
    var JsonData = {};
    $.ajax({
        url: HOST_ENV + "/Version/ShowEmployeeView",
        type: 'GET',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (partialViewResult) {
            $("#divAssignEmployeeInfo").html(partialViewResult)
            GridViewEmployee.PerformCallback();
            $(".overlay").hide();
        },
        cache: false
    });


    $("#AddNewPositionModal").modal({
        backdrop: 'static',
        keyboard: false
    });

    var ShowPosition = JSON.parse($("#hdnInitialValues").val());
    $("#txtPositionId").val(ShowPosition.OprLevelId);
    $("#txtPositionId").attr("disabled", "disabled");
}

function CheckUnCheckAssistance(LevelID, ParentLevelID) {

    var JsonData = {
        ShowLevel: LevelID,
        ParentLevel: ParentLevelID
    };

    $(".overlay").show();
    $.ajax({
        type: "post",
        url: HOST_ENV + "/Version/CheckUnCheckAssistance",
        data: JsonData,
        async: true,
        datetype: "json",
        success: function (Json) {
            if (Json.Success == "Success") {
                $("#hdnOrgChartData").val(Json.ChartData);
                $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
                $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
                $("#hdnOrgVersion").val(Json.UsedVersion);
                $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
                document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                CancelOperation();
                ShowMessage(Json.Success, Json.ShowMessage);
            }
            else if (Json.Success == "Failure") {
                ShowMessage(Json.Success, Json.ShowMessage);
            }

            $(".overlay").hide();
        }
    });
}

function SaveSorting() {

    var SortData = "", ParentLevel = "";
    $(".ui-state-default").each(function () {
        SortData += "," + $(this).attr("data-id");
        ParentLevel = $(this).attr("data-parent");
    });

    var JsonData = {
        SortedData: (SortData == "" ? "" : SortData.substring(1)),
        SortLevel: ParentLevel
    };

    $(".overlay").show();
    $.ajax({
        type: "post",
        url: HOST_ENV + "/Version/SaveSorting",
        data: JsonData,
        async: true,
        datetype: "json",
        success: function (Json) {
            if (Json.Success == "Success") {
                $("#hdnOrgChartData").val(Json.ChartData);
                $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                $("#hdnOrgShowLevel").val(Json.UsedShowLevel);
                $("#hdnOrgPartitionShowLevel").val(Json.UsedShowLevel);
                $("#hdnOrgVersion").val(Json.UsedVersion);
                $("#hdnOrgPartitionVersion").val(Json.UsedVersion);
                document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                CancelOperation();

                $('#AddSortingModal').modal('toggle');
                ShowMessage(Json.Success, Json.ShowMessage);
            }
            else if (Json.Success == "Failure") {
                $('#AddSortingModal').modal('toggle');
                ShowMessage(Json.Success, Json.ShowMessage);
            }

            $(".overlay").hide();
        }
    });
}

function ViewEditEmployeeInformation() {
    if ($("#hdnEmployeeInfEditFlag").val() == "Yes") {
        $("#imgEditEmployee").show();
        if ($("#imgEditEmployee").attr("src").indexOf("edit.svg") != -1) {
            $("#imgEditEmployee").attr("src", HOST_ENV + "/Content/Images/view.svg");

            var ShowFields = "";
            ShowFields += "<div class=\"row\">";
            $.each(ObjEI[0], function (key, val) {
                try {
                    ShowFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
                    ShowFields += "        <div class=\"col-md-4\">" + key + "</div>";
                    ShowFields += "        <div class=\"col-md-8\">";
                    ShowFields += "             <input type=\"text\" name=\"" + key + "\" id=\"" + key + "\" placeholder=\"" + key + "\" class=\"form-control\" value=\"" + ((val) ? val : "") + "\" onchange=\"ChangeEmployeeInformation(this)\" />";
                    ShowFields += "        </div>";
                    ShowFields += "   </div>";
                }
                catch (ex) {
                }
            });
            ShowFields += "</div>";
            $("#divEmployeeInformation").html(ShowFields);
            $("#btnSaveEmployeeInformation").show();
        }
        else {
            $("#imgEditEmployee").attr("src", HOST_ENV + "/Content/Images/edit.svg");

            var ShowFields = "";
            ShowFields += "<div class=\"row\">";
            $.each(JSON.parse($("#hdnEmployeeInf").val())[0], function (key, val) {
                try {
                    ShowFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
                    ShowFields += "        <div class=\"col-md-4\">" + key + "</div>";
                    ShowFields += "        <div class=\"col-md-8\">" + ((val) ? val : "") + "</div>";
                    ShowFields += "   </div>";
                }
                catch (ex) {
                }
            });
            ShowFields += "</div>";
            $("#divEmployeeInformation").html(ShowFields);
            $("#btnSaveEmployeeInformation").hide();
        }
    }
    else if ($("#hdnEmployeeInfEditFlag").val() == "No") {
        $("#imgEditEmployee").attr("src", HOST_ENV + "/Content/Images/edit.svg");
        $("#imgEditEmployee").hide();

        var ShowFields = "";
        ShowFields += "<div class=\"row\">";
        $.each(JSON.parse($("#hdnEmployeeInf").val())[0], function (key, val) {
            try {
                ShowFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
                ShowFields += "        <div class=\"col-md-4\">" + key + "</div>";
                ShowFields += "        <div class=\"col-md-8\">" + ((val) ? val : "") + "</div>";
                ShowFields += "   </div>";
            }
            catch (ex) {
            }
        });
        ShowFields += "</div>";
        $("#divEmployeeInformation").html(ShowFields);
        $("#btnSaveEmployeeInformation").hide();
    }
}

function ChangeEmployeeInformation(Obj) {
    try {
        for (var key in ObjEI[0]) {
            if (key == $(Obj).attr("id")) {
                ObjEI[0][key] = $(Obj).val();
            }
        }
    }
    catch (ex) {
        alert(ex);
    }
}

function SaveEmployeeInformation() {
    $(".overlay").show();
    var JsonData = {
        EmployeeInformation: "{ \"data\":" + JSON.stringify(ObjEI) + "}",
        ShowLevel: $("#hdnEmployeeLevelID").val()
    };

    $.ajax({
        url: HOST_ENV + "/Version/SaveEmployeeInformation",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            if (Json.Success == "Success") {
                $("#hdnOrgChartData").val(Json.ChartData);
                $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                CancelOperation();
                $('#ShowEmployeeInformationModal').modal('toggle');
                ShowMessage(Json.Success, Json.ShowMessage);
            }
            else if (Json.Success == "Failure") {
                $('#ShowEmployeeInformationModal').modal('toggle');
                ShowMessage(Json.Success, Json.ShowMessage);
            }
            else if (Json.Success == "No") {
                $("#divEmployeeInformationError").html(Json.ShowMessage);
            }
            $(".overlay").hide();
        },
        cache: false
    });
}

function SetFunctionalManager(LId) {
    $("#txtFunctionalManager").val(LId);
}

function AddFunctionalManager(LId, PId, PName) {
    $("#hdnLevelIdFM").val(LId);
    $("#hdnParentLevelIdFM").val(PId);
    $("#hdnParentNameFM").val(PName);

    $("#txtFunctionalManager").val("");
    $("#divFunctionalManagerError").html("");

    $(".overlay").show();
    var JsonData = {};
    $.ajax({
        url: HOST_ENV + "/Version/ShowFunctionalManagerPartial",
        type: 'GET',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (partialViewResult) {
            $("#divFuctionalManagerTable").html(partialViewResult)
            GridViewPartialFM.PerformCallback();
            $(".overlay").hide();
        },
        cache: false
    });

    $("#AddFunctionalManager").modal({
        backdrop: 'static',
        keyboard: false
    });
}

function ShowFunctionalManager(LId, PId, PName) {
    $(".overlay").show();
    $("#divFunctionalManagerError").html("");

    var JsonData = {
        LevelId: $("#hdnLevelIdFM").val(),
        ParentLevelId: $("#hdnParentLevelIdFM").val(),
        NewParentLevelId: $("#txtFunctionalManager").val(),
        OldParentName: $("#hdnParentNameFM").val()
    };

    $.ajax({
        url: HOST_ENV + "/Version/ShowFunctionalManager",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            if (Json.Message == "Success") {
                $("#hdnOrgChartData").val(Json.ChartData);
                $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                CancelOperation();
                $('#AddFunctionalManager').modal('toggle');
                ShowMessage(Json.Message, Json.ShowMessage);
            }
            else if (Json.Message == "Failure") {
                $('#AddFunctionalManager').modal('toggle');
                ShowMessage(Json.Message, Json.ShowMessage);
            }
            else if (Json.Message == "No") {
                $("#divFunctionalManagerError").html(Json.ShowMessage);
            }

            $(".overlay").hide();
        },
        cache: false
    });
}

function ShowOutsideLegalEntity(LId, PId) {
    $(".overlay").show();
    var JsonData = {
        LevelId: LId,
        ParentLevelId: PId
    };

    $.ajax({
        url: HOST_ENV + "/Version/ShowOutsideLegalEntity",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            if (Json.Message == "Success") {
                $("#hdnOrgChartData").val(Json.ChartData);
                $("#hdnOrgChartHRCoreData").val(Json.ChartData);
                document.getElementById("mySavedModel").value = "{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + $("#hdnOrgChartData").val() + " }";

                CancelOperation();
            }
            ShowMessage(Json.Message, Json.ShowMessage);
            $(".overlay").hide();
        },
        cache: false
    });
}

var ObjEI = [];
function ShowEmployeeInformation(KeyValue, EditFlag, ShowLevel) {

    ObjEI = [];
    $(".overlay").show();
    var JsonData = {
        ShowLevel: KeyValue
    };

    $.ajax({
        url: HOST_ENV + "/Version/ShowEmployeeInformationFields",
        type: 'POST',
        data: JsonData,
        dateType: "json",
        async: true,
        success: function (Json) {
            $("#hdnEmployeeInf").val(Json.IB);
            $("#hdnEmployeeInfEditFlag").val(EditFlag);
            $("#hdnEmployeeLevelID").val(ShowLevel);

            var ShowFields = "";
            $("#ShowEmployeeInformationModal").modal({
                backdrop: 'static',
                keyboard: false
            });

            if (Json.Success == "Yes") {
                $("#imgEditEmployee").attr("src", HOST_ENV + "/Content/Images/edit.svg");
                if ($("#hdnEmployeeInfEditFlag").val() == "Yes") $("#imgEditEmployee").show(); else $("#imgEditEmployee").hide();

                ShowFields += "<div class=\"row\">";
                var item = {}
                $.each(JSON.parse(Json.IB)[0], function (key, val) {
                    try {
                        ShowFields += "   <div class=\"col-md-12\" style=\"border-bottom: 1px solid #e0e0e0; padding: 5px;\">";
                        ShowFields += "        <div class=\"col-md-4\">" + key + "</div>";
                        ShowFields += "        <div class=\"col-md-8\">" + ((val) ? val : "") + "</div>";
                        ShowFields += "   </div>";
                        item[key] = val;
                    }
                    catch (ex) {
                    }
                });
                ObjEI.push(item);
                ShowFields += "</div>";
                $("#btnSaveEmployeeInformation").hide();
            }
            else ShowFields = "<div style=\"color:red;\">Employee Information not available</div>";
            $("#divEmployeeInformation").html(ShowFields);
            $(".overlay").hide();
        },
        cache: false
    });
}

function ChangeSortOrder(LevelID) {
    $(".overlay").show();

    var JsonData = {
        ShowLevel: LevelID
    };

    $.ajax({
        type: "post",
        url: HOST_ENV + "/Version/GetSortLevel",
        data: JsonData,
        async: true,
        datetype: "json",
        success: function (json) {

            // Sorting LI element
            var LI = "";
            var OrgData = JSON.parse(json.IB);
            $.each(OrgData, function (key, item) {
                var Caption = item.LEVEL_ID + "[ " + item.FULL_NAME + " ]";
                LI += "<li class=\"ui-state-default\" data-id=\"" + item.LEVEL_ID + "\" data-parent=\"" + item.PARENT_LEVEL_ID + "\"><span class=\"ui-icon ui-icon-arrowthick-2-n-s\"></span>" + Caption + "</li>";
            });
            $("#sortable").html(LI);
            $(".overlay").hide();

            $("#AddSortingModal").modal({
                backdrop: 'static',
                keyboard: false
            });
        }
    });
}

function DownloadDialogBox() {

    $("#divFieldSelection").hide();
    $("#btnDownLoadSave").hide();
    $("#divTemplateSelection").hide();

    var BG = "<b style=\"float:left;\">Select Version:&nbsp;</b>" +
        "<div style=\"color:black;float:left;\">" + ($("#selInitiative").val() == "" ? "SELECT INITIATIVE" : $("#selInitiative").val()) + "&nbsp;</div>" +
        "<div style=\"float:left;\">&nbsp;>>&nbsp;</div>" +
        "<div style=\"color:black;float:left;\">" + ($("#selPopulation").val() == "" ? "SELECT POPULATION" : $("#selPopulation").val()) + "&nbsp;</div>" +
        "<div style=\"float:left;\">&nbsp;>>&nbsp;</div>" +
        "<div style=\"color:black;float:left;\">" + ($("#selUser").val() == "" ? "SELECT USER" : $("#selUser").val()) + "&nbsp;>>&nbsp;</div>" +
        "<div style=\"color:black;float:left;\">" + ($("#selVersion").val() == "" ? "SELECT VERSION" : $("#selVersion").val()) + "&nbsp;</div>";

    if ($("#hdnOrgRole").val() == "EndUser") {
        var data = $('#selEndUserVersion').select2('data');
        $("#spnVersionDetailsBG").html("<b style=\"float:left;\">Select Version:&nbsp;</b>" + data.text);
    }
    else $("#spnVersionDetailsBG").html(BG);
    $("#DownloadModal").modal({
        backdrop: 'static',
        keyboard: false
    });
    DelRevTemplate();
}

function DelRevTemplate() {
    var templatedata = "";
    $.ajax({
        type: "post",
        url: HOST_ENV + "/Version/ShowTemplatesList",
        async: true,
        datetype: "json",
        success: function (json) {
            if (json.Success == "Yes") {
                templatedata = JSON.parse(json.SF);
                $("#divTemplateData").empty();

                if (templatedata.length >= 1) {

                    $("#divDRTemplateData").empty();
                    var DRPPTXTemplatehtml = "";
                    DRPPTXTemplatehtml += "<div class=\"col-md-12\">" +
                        "<div class=\"row\">" +
                        "<div class=\"col-md-3\">" +
                        "<strong>TemplateName</strong>" +
                        "</div>" +
                        "<div class=\"col-md-3\">" +
                        "<strong> Description </strong>" +
                        "</div>" +
                        "<div class=\"col-md-3\">" +
                        "<strong>FileName </strong>" +
                        "</div>" +
                        "<div class=\"col-md-3\">" +
                        "<strong>Action </strong>" +
                        "</div>" +
                        "</div>" +
                        "</div>";

                    for (var Idx = 0; Idx <= templatedata.length - 1; Idx++) {
                        var DeleteRevokeFlag = "";

                        if (templatedata[Idx].ACTIVE_IND == "N") DeleteRevokeFlag = "<i  onclick=DeleteRevokeTemplate(" + templatedata[Idx].ID + ",'R') class=\"fa fa-plus contextmenu\" style=\"padding-left:15px;\"></i>";
                        if (templatedata[Idx].ACTIVE_IND == "Y") DeleteRevokeFlag = "<i  onclick=DeleteRevokeTemplate(" + templatedata[Idx].ID + ",'D') class=\"fa fa-remove contextmenu\" style=\"padding-left:15px;\"></i>";

                        DRPPTXTemplatehtml += "<div class=\"col-md-12\">" +
                            "<div class=\"row\">" +
                            "<div class=\"col-md-3\">" +
                            "<label class=\"label\">" + templatedata[Idx].TemplateName + "</label>" +
                            "</div>" +
                            "<div class=\"col-md-3\">" +
                            "<label class=\"label\">" + templatedata[Idx].Description + "</label>" +
                            "</div>" +
                            "<div class=\"col-md-3\">" +
                            "<label class=\"label\">" + templatedata[Idx].FileName + "</label>" +
                            "</div>" +
                            "<div class=\"col-md-3\">" +
                            "<div class=\"btn-group btn-group-toggle\" data-toggle=\"buttons\">" +
                            "<label style=\"cursor: pointer;\" >" +
                            DeleteRevokeFlag +
                            "</label>" +
                            "</div>" +
                            "</div>" +
                            "</div>" +
                            "</div>";
                    }
                    $("#divDRTemplateData").html(DRPPTXTemplatehtml);
                }

            }
        }
    });
}
function DeleteRevokeTemplate(TemplateId, DRType) {
    if (DRType == "R") {
        if (confirm("Do you want to Revoke Template?")) {
            $(".overlay").show();
            var JsonData = {
                TemplateId: TemplateId,
                DRType: DRType
            };
            $.ajax({
                type: "POST",
                url: HOST_ENV + "/Version/DeleteRevokeTemplate",
                data: JsonData,
                async: true,
                dateType: "json",
                success: function () {
                    DelRevTemplate();
                    $(".overlay").hide();
                }
            });
        }
    }
    else if (DRType == "D") {
        if (confirm("Do you want to Delete Template?")) {
            $(".overlay").show();
            var JsonData = {
                TemplateId: TemplateId,
                DRType: DRType
            };
            $.ajax({
                type: "POST",
                url: HOST_ENV + "/Version/DeleteRevokeTemplate",
                data: JsonData,
                async: true,
                dateType: "json",
                success: function () {
                    DelRevTemplate();
                    $(".overlay").hide();
                }
            });
        }
    }

}

function VersionDetailsDialogBox() {

    var IColor = ($("#selInitiative").val() == "" ? "#5B64FD" : "#48FD45");
    var PColor = ($("#selPopulation").val() == "" ? "#FE4A38" : "#48FD45");
    if (IColor == "#48FD45" && PColor == "#FE4A38") {
        PColor = "#5B64FD";
    }
    var VColor = ($("#selVersion").val() == "" ? "#FE4A38" : "#FFFF00");
    if (PColor == "#48FD45" && VColor == "#FE4A38") {
        VColor = "#5B64FD";
    }
    var UColor = "#48FD45";

    var BG = "<b style=\"float:left;\">Select Version:&nbsp;</b>" +
        "<div style=\"color:" + IColor + ";float:left;\">" + ($("#selInitiative").val() == "" ? "SELECT INITIATIVE" : $("#selInitiative").val()) + "&nbsp;</div>" +
        "<div style=\"float:left;cursor:pointer;\"><i class=\"fa-plus fa icon-x\" onclick=\"AddInitiative('" + $("#txtName").val() + "','" + $("#txtDescription").val() + "')\"></i>&nbsp;</div>" +
        "<div style=\"float:left;cursor:pointer;\"><i class=\"fa-remove fa icon-x\" onclick=\"RemoveVersion('" + $("#selInitiative").val() + "')\"></i>&nbsp;</div>" +
        "<div style=\"float:left;\">&nbsp;>>&nbsp;</div>" +
        "<div style=\"color:" + PColor + ";float:left;\">" + ($("#selPopulation").val() == "" ? "SELECT POPULATION" : $("#selPopulation").val()) + "&nbsp;</div>" +
        "<div style=\"float:left;cursor:pointer;\"><i class=\"fa-plus fa icon-x\" onclick=\"AddPopulation('" + $("#selInitiative").val() + "','" + $("#txtName").val() + "','" + $("#txtDescription").val() + "')\"></i>&nbsp;</div>" +
        "<div style=\"float:left;cursor:pointer;\"><i class=\"fa-remove fa icon-x\" onclick=\"RemoveVersion('" + $("#selPopulation").val() + "')\"></i>&nbsp;</div>" +
        "<div style=\"float:left;\">&nbsp;>>&nbsp;</div>" +
        "<div style=\"color:" + UColor + ";float:left;\">" + ($("#selUser").val() == "" ? "SELECT USER" : $("#selUser").val()) + "&nbsp;>>&nbsp;</div>" +
        "<div style=\"color:" + VColor + ";float:left;\">" + ($("#selVersion").val() == "" ? "SELECT VERSION" : $("#selVersion").val()) + "&nbsp;</div>" +
        "<div style=\"float:left;cursor:pointer;\"><i class=\"fa-plus fa icon-x\" onclick=\"AddVersion('" + $("#selVersion").val() + "')\"></i>&nbsp;</div>" +
        "<div style=\"float:left;cursor:pointer;\"><i class=\"fa-remove fa icon-x\" onclick=\"RemoveVersion('" + $("#selVersion").val() + "')\"></i>&nbsp;</div>";

    //$("#spnVersionDetailsBG").html(BG);
    if ($("#hdnOrgRole").val() == "EndUser") {
        var data = $('#selEndUserVersion').select2('data');
        $("#spnVersionDetailsBG").html("<b style=\"float:left;\">Select Version:&nbsp;</b>" + data.text);
    }
    else $("#spnVersionDetailsBG").html(BG);

    $("#btnVersionDetailsPopup").click();
}

function GetSelectedCountryLevel(SelectedCountry) {
    var OrgC = JSON.parse($("#hdnOrgCountries").val());
    if (OrgC.length >= 1) {
        for (Idx = 0; Idx <= OrgC.length - 1; Idx++) {
            if (OrgC[Idx].CountryCode == SelectedCountry) return OrgC[Idx].OrgUnit;
        }
    }
    return "CH";
}

function ChangeFinalyzerVersion(SelectedCountry) {
    if ($("#hdnOrgRole").val() == "Finalyzer" || $("#hdnOrgRole").val() == "User") {
        var OrgDDL = JSON.parse($("#hdnOrgDDL").val());

        if (OrgDDL.length >= 1) {
            for (Idx = 0; Idx <= OrgDDL.length - 1; Idx++) {
                if (OrgDDL[Idx].UserRole == "Finalyzer" && OrgDDL[Idx].UserName == $("#hdnUserId").val() && OrgDDL[Idx].OperType == "LV") {
                    OrgDDL[Idx].Country = SelectedCountry;
                    OrgDDL[Idx].ShowLevel = GetSelectedCountryLevel(SelectedCountry);
                }
                else if (OrgDDL[Idx].UserRole == "User" && OrgDDL[Idx].OperType == "LV") {
                    OrgDDL[Idx].Country = SelectedCountry;
                    OrgDDL[Idx].ShowLevel = GetSelectedCountryLevel(SelectedCountry);
                }
            }
            $("#hdnOrgDDL").val(JSON.stringify(OrgDDL));
        }
    }
}

function AddNewVersion() {
    if ($("#selInitiative").val() != "SelectInitiative" &&
        $("#selInitiative").val() != "AddInitiative" &&
        $("#selPopulation").val() != "SelectPopulation" &&
        $("#selPopulation").val() != "AddPopulation" &&
        $("#selUser").val() != "AddUser") {
        $("#divRepVersionError").empty();
        $("#divRepVersionHeader").empty();

        $("#divShowMap").hide();
        $("#divAddPopulation").show();
        $("#divAddInitiative").hide();
        $("#divAddVersion").show();
        $("#divRightOrgChart").hide();
        $("#divLeftOrgChart").hide();
        $("#divUploadVersion").hide();
    }
    else alert("All this entities(Initiatives, Population and User) must have be selected");
}

function ShowPopupDialog(Obj) {
    switch ($(Obj).val()) {
        case "AddInitiative":
            $("#divRepInitiativeError").empty();
            $("#divRepInitiativeHeader").empty();
            if (($("#selCountry").val() != "SelectCountry" && $("#hdnOrgType").val() == "LV") ||
                ($("#hdnOrgType").val() == "OV") ||
                ($("#hdnOrgRole").val() == "Finalyzer")) {
                $("#divShowMap").hide();
                $("#divAddPopulation").hide();
                $("#divAddInitiative").show();
                $("#divAddVersion").hide();
                $("#divRightOrgChart").hide();
                $("#divLeftOrgChart").hide();
                $("#divUploadVersion").hide();

                $("#selPopulation").select2("val", "SelectPopulation");
                $("#selUser").select2("val", $("#hdnUserId").val());
                $("#selVersion").select2("val", "SelectVersion");
            }
            else alert("Please select Country");

            break;
        case "AddPopulation":
            $("#divRepPopulationError").empty();
            $("#divRepPopulationHeader").empty();

            if ($("#selInitiative").val() != "SelectInitiative" &&
                $("#selInitiative").val() != "AddInitiative") {

                $(".overlay").show();
                $("#divRightOrgChart").removeClass("col-md-12");
                $("#divRightOrgChart").removeClass("col-md-8");
                $("#divRightOrgChart").removeClass("col-md-6");
                $("#divRightOrgChart").addClass("col-md-8");

                $("#divShowMap").hide();
                $("#divAddPopulation").show();
                $("#divAddInitiative").hide();
                $("#divAddVersion").hide();
                $("#divRightOrgChart").show();
                $("#divLeftOrgChart").hide();
                $("#divUploadVersion").show();

                $("#myDiagramDiv").detach();
                var iDiv = document.createElement('div');
                iDiv.id = 'myDiagramDiv';
                document.getElementById('myDiagramContainer').appendChild(iDiv);

                // Org chart in Canvas
                h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
                w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
                init(w, h, DragDrop);

                $("#selUser").select2("val", $("#hdnUserId").val());
                $("#selVersion").select2("val", "SelectVersion");
                $(".overlay").hide();
            }
            else alert("Please select Initiative");

            break;
        case "AddVersion":
            if ($("#selInitiative").val() != "SelectInitiative" &&
                $("#selInitiative").val() != "AddInitiative" &&
                $("#selPopulation").val() != "SelectPopulation" &&
                $("#selPopulation").val() != "AddPopulation" &&
                $("#selUser").val() != "AddUser") {
                $("#divRepVersionError").empty();
                $("#divRepVersionHeader").empty();

                $("#divShowMap").hide();
                $("#divAddPopulation").show();
                $("#divAddInitiative").hide();
                $("#divAddVersion").show();
                $("#divRightOrgChart").hide();
                $("#divLeftOrgChart").hide();
                $("#divUploadVersion").hide();
            }
            else alert("All this entities(Initiatives, Population and User) must have be selected");

            break;
        case "Download":
            $("#btnDownloadPopup").click();
            break;
        default:
            if ($(Obj).attr("id") == "selCountry") {
                if ($("#selCountry").select2("val") == "SelectCountry") {
                    $(".overlay").show();
                    $("#selInitiative").select2("val", "SelectInitiative");
                    $("#selPopulation").select2("val", "SelectPopulation");
                    $("#selUser").select2("val", "SelectUser");
                    $("#selVersion").select2("val", "SelectVersion");
                    ShowDDL(AEI, [], [], [], "N");
                    $(".overlay").hide();
                }
                else {
                    ChangeFinalyzerVersion($("#selCountry").select2("val"));
                    SelectedCountryDDL($("#hdnOrgDDL").val(), $("#selCountry").select2("val"), "N");
                    $("#selInitiative").select2("val", "SelectInitiative");
                    $("#selPopulation").select2("val", "SelectPopulation");
                    $("#selUser").select2("val", $("#hdnUserId").val());
                    $("#selVersion").select2("val", "SelectVersion");
                }
            }
            else if ($(Obj).attr("id") == "selInitiative") {
                if ($("#selInitiative").select2("val") == "SelectInitiative") {
                    $(".overlay").show();
                    $("#selPopulation").select2("val", "SelectPopulation");
                    $("#selUser").select2("val", "SelectUser");
                    $("#selVersion").select2("val", "SelectVersion");
                    ShowDDL(AEI, [], [], [], "N");
                    $(".overlay").hide();
                }
                else {
                    SelectedInitiativeDDL($("#hdnOrgDDL").val(), $("#selInitiative").select2("val"), "N");
                    $("#selPopulation").select2("val", "SelectPopulation");
                    $("#selUser").select2("val", $("#hdnUserId").val());
                    $("#selVersion").select2("val", "SelectVersion");
                }
            }
            else if ($(Obj).attr("id") == "selPopulation") {
                if ($("#selPopulation").select2("val") == "SelectPopulation") {
                    $(".overlay").show();
                    $("#selUser").select2("val", $("#hdnUserId").val());
                    $("#selVersion").select2("val", "SelectVersion");
                    ShowDDL(AEI, AEP, [], [], "N");
                    $(".overlay").hide();
                }
                else {
                    SelectedPopulationDDL($("#hdnOrgDDL").val(), $("#selPopulation").select2("val"), $("#selInitiative").select2("val"), "N");
                    $("#selUser").select2("val", $("#hdnUserId").val());
                    $("#selVersion").select2("val", "SelectVersion");
                }
            }
            else if ($(Obj).attr("id") == "selUser") {
                if ($("#selUser").select2("val") == "SelectUser") {
                    $(".overlay").show();
                    $("#selVersion").select2("val", "SelectVersion");
                    ShowDDL(AEI, AEP, AEU, [], "N");
                    $(".overlay").hide();
                }
                else {
                    SelectedUserDDL($("#hdnOrgDDL").val(), $("#selUser").select2("val"), $("#selPopulation").select2("val"), $("#selInitiative").select2("val"), "N");
                    $("#selVersion").select2("val", "SelectVersion");
                }
            }
            else if ($(Obj).attr("id") == "selVersion") {
                if ($("#selVersion").select2("val") != "SelectVersion" &&
                    $("#selVersion").select2("val") != "AddVersion") {
                    if ($("#hdnOrgRole").val() == "User")
                        LoadSelectedUserVersion();
                    else
                        LoadSelectedVersion();
                }
            }

            break;
    }
}

function GetParameter(Parameter) {
    var StringConcat = "";

    if (Parameter.trim() != "") {
        var ArrayParam = Parameter.split(';');
        if (ArrayParam.length >= 1) {
            for (var Idx = 0; Idx <= ArrayParam.length - 1; Idx++) {
                var MenuItem = ArrayParam[Idx].split(':');
                if (MenuItem[0] == "S") StringConcat += "," + "\'" + MenuItem[1] + "\'"
            }

            return StringConcat.substring(1);
        }
    }

    return StringConcat;
}

function ShowEmptyTable() {
    return "<b style='color:red'>There is no Version</b>";
}

function FinalyzeVersion() {
    $(".overlay").show();

    var JsonData = {
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/FinalyseVersion",
        data: JsonData,
        async: false,
        dateType: "json",
        success: function (jsonStr) {
            alert("Version Updated");
            $(".overlay").hide();
        }
    });
}

function ShowVersionNo(VNo, VRole) {
    $(".overlay").show();
    var JsonData = {
        VersionNo: VNo,
        Role: VRole
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/ShowVersionInfo",
        data: JsonData,
        async: false,
        dateType: "json",
        success: function (jsonStr) {
            $(".overlay").hide();
            window.location.href = HOST_ENV + "/Version/UpdateVersion";
        }
    });
}

function GetVersionList() {
    $(".overlay").show();

    var JsonData = {
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/GetVersionList",
        data: JsonData,
        async: false,
        dateType: "json",
        success: function (data) {

            if (!($(".navbar-toggle").is(":visible"))) {
                $("#tblVersionList").show();
                $("#divVersionList").hide();

                $("#tblVersionList").empty();

                if ($("#tblVersionList").hasClass("dataTable") == true) {
                    var oTable = $('#tblVersionList').dataTable();
                    oTable.fnDestroy();
                }
                var tblHead = "";

                tblHead = "<thead>" +
                    "  <tr style=\"background-color:lightgray;\">" +
                    "     <th style=\"text-align:left;border-top:1px solid gray;border-left:1px solid gray;border-right:1px solid gray\">Version No</th>" +
                    "     <th style=\"text-align:left;border-top:1px solid gray;border-right:1px solid gray\">Version Name</th>" +
                    "     <th style=\"text-align:left;border-top:1px solid gray;border-right:1px solid gray\">Version Desc</th>" +
                    "     <th style=\"text-align:left;border-top:1px solid gray;border-right:1px solid gray\">Version Date</th>" +
                    "  </tr>" +
                    "</thead>";
                $("#tblVersionList").html(tblHead);
                $('#tblVersionList').dataTable({
                    "bPaginate": true,
                    "bLengthChange": false,
                    "bAutoWidth": false,
                    "iDisplayLength": 5,
                    "bDestroy": true,
                    "aaSorting": [[0, "desc"]],
                    "aaData": JSON.parse(data.VL),
                    "sDom": 'T<"clear">lfrtip',
                    "buttons": false,
                    "language": {
                        "emptyTable": ShowEmptyTable("1")
                    },
                    "aoColumnDefs": [{ "sWidth": "27%", "aTargets": [2] }],
                    "columns": [
                        {
                            "data": "VersionNo", "render": function (data, type, full, meta) {
                                return '<a href="javascript:void(0);"  onclick="return ShowVersionNo(\'' + data + '\',\'' + full.Role + '\');">' + data + '</a>';
                            }
                        },
                        {
                            "data": "VersionName", "render": function (data, type, full, meta) {
                                return full.VersionName.trim();
                            }
                        },
                        {
                            "data": "VersionDesc", "render": function (data, type, full, meta) {
                                return full.VersionDesc.trim();
                            }
                        },
                        {
                            "data": "KeyDate", "render": function (data, type, full, meta) {
                                return full.KeyDate.trim();
                            }
                        }
                    ]
                });
            }
            else {
                $("#tblVersionList").hide();
                $("#divVersionList").show();

                var VERSION_TABLE = JSON.parse(data.VL);
                if (VERSION_TABLE.length >= 1) {
                    var DivRow = "";
                    for (var Idx = 0; Idx <= VERSION_TABLE.length - 1; Idx++) {
                        DivRow += "<div style='width:100%;border-bottom:1px solid lightgray'>";
                        DivRow += "<div style='width:100%'><a href='javascript: void (0);'  onclick='return ShowVersionNo(\'" + VERSION_TABLE[Idx].VersionNo + "\',\'" + full.Role + "\');'>" + VERSION_TABLE[Idx].VersionNo + "</a></div>";
                        DivRow += "<div style='width:100%'>" + VERSION_TABLE[Idx].VersionName + "</div>";
                        DivRow += "<div style='width:100%'>" + VERSION_TABLE[Idx].VersionDesc + "</div>";
                        DivRow += "<div style='width:100%'>" + VERSION_TABLE[Idx].KeyDate + "</div>";
                        DivRow += "</div>";
                    }
                    $("#divVersionList").html(DivRow);
                }
                else $("#divVersionList").html("<div style='color:red;'>No Version available for the user</div>");
            }
            $(".overlay").hide();
        }
    });
}

function ClearTextBox(ObjName) {
    $("#" + ObjName).val("");
}

function SelectFullName() {
    var Option = "";
    $("#multiselect_to option").each(function () {
        Option += "," + $(this).val()
    });
    $("#txtFullName").val(Option.substring(1));
    var isEmpty = $("#txtFullName").val() == '';
    $('#tooltipPopulationDataFormStage3').bootstrapValidator('revalidateField', 'txtFullName');
}

function SaveVersion() {


    if ($("#hdnOrgRole").val() == "Finalyzer") {
        $(".overlay").show();
        var JsonData = {
        };
        $.ajax({
            type: "POST",
            url: HOST_ENV + "/Version/FinalyzePlayerVersion",
            data: JsonData,
            async: false,
            dateType: "json",
            success: function (Json) {
                if (Json.Success == "Yes") {
                    $("#hdnOrgChartData").val(Json.Data);
                    $("#hdnOrgChartHRCoreData").val(Json.IVData);

                    $("#divRightOrgChart").removeClass("col-md-12");
                    $("#divRightOrgChart").removeClass("col-md-8");
                    $("#divRightOrgChart").removeClass("col-md-6");
                    $("#divRightOrgChart").addClass("col-md-12");

                    $("#divAddInitiative").hide();
                    $("#divAddVersion").hide();
                    $("#divRightOrgChart").show();
                    $("#divLeftOrgChart").hide();
                    $("#divUploadVersion").hide();

                    $("#myDiagramDiv").detach();
                    var iDiv = document.createElement('div');
                    iDiv.id = 'myDiagramDiv';
                    document.getElementById('myDiagramContainer').appendChild(iDiv);

                    // Org chart in Canvas
                    h = parseInt($("#divRightOrgChart").css("height").substr(0, $("#divRightOrgChart").css("height").indexOf("px")));
                    w = parseInt($("#divRightOrgChart").css("width").substr(0, $("#divRightOrgChart").css("width").indexOf("px")));
                    init(w, h, DragDrop);
                }
                else alert(Json.Message);

                $(".overlay").hide();
            }
        });
    }
}

function LoadDataFromServer() {
    $(".overlay").show();

    var JsonData = {
        UserType: $("#hdnOrgRole").val(),
        Country: "",
        ShowLevel: $("#hdnOrgShowLevel").val(),
        Levels: $("#hdnOrgLevel").val(),
        Oper: $("#hdnOrgType").val(),
        Version: $("#hdnOrgVersion").val()
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/GetOrgChartData",
        data: JsonData,
        async: false,
        dateType: "json",
        success: function (jsonStr) {
            loadJSON("{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + jsonStr + " }", jsonStr);
            $(".overlay").hide();
        }
    });
}

function SetSelectedValues(Obj, sType) {
    $(".overlay").show();
    if (sType == "Role") {
        if ($("#hdnOrgRole").val().toUpperCase() == "USER")
            window.location.href = HOST_ENV + "/";
        else if ($("#hdnOrgRole").val().toUpperCase() == "PLAYER")
            window.location.href = HOST_ENV + "/Version/UploadVersion";
        else if ($("#hdnOrgRole").val().toUpperCase() == "ADMIN")
            window.location.href = HOST_ENV + "/Version/AdminVersion";
        else if ($("#hdnOrgRole").val().toUpperCase() == "FINALYZER")
            window.location.href = HOST_ENV + "/Version/UploadData";
    }
    else {

        var JsonData = {
            KeyDate: $("#hdnOrgKeyDate").val(),
            UsedView: $("#hdnOrgView").val(),
            Country: $("#hdnOrgCountry").val(),
            ShowLevel: $("#hdnOrgShowLevel").val(),
            Levels: $("#hdnOrgLevel").val(),
            Oper: $("#hdnOrgType").val(),
            Version: $("#hdnOrgVersion").val(),
            Role: $("#hdnOrgRole").val(),
            SelectedShape: $("#hdnSelectedShape").val(),
            SelectedSkin: $("#hdnSelectedSkin").val(),
            SelectedShowPicture: $("#SelectedShowPicture").val(),
            SelectedSplitScreen: $("#hdnSelectedSplitScreen").val(),
            SelectedTextColor: $("#hdnSelectedTextColor").val(),
            SelectedBorderColor: $("#hdnSelectedBorderColor").val(),
            SelectedBorderWidth: $("#hdnSelectedBorderWidth").val(),
            SelectedLineColor: $("#hdnSelectedLineColor").val(),
            SelectedPortraitModeMultipleLevel: $("#hdnSelectedPortraitModeMultipleLevel").val(),
            SelectedFunctionalManagerType: $("#hdnSelectedFunctionalManagerType").val(),
            Type: sType
        };
        $.ajax({
            type: "POST",
            url: HOST_ENV + "/Home/SetSelectedValues",
            data: JsonData,
            async: true,
            dateType: "json",
            success: function (jsonStr) {
                if ($("#hdnOrgType").val() == "OV") {
                    $("#divRightOrgChart").show();
                    $("#divShowMap").hide();

                    $("#hdnOrgChartData").val(jsonStr);
                    CancelOperation();
                }
                else if ($("#hdnOrgType").val() == "LV") {
                    $("#divRightOrgChart").hide();
                    $("#divShowMap").show();
                }
                $(".overlay").hide();
            }
        });
    }
}

function CreateOrgChart(Obj, sType, sValue) {
    switch (sType) {
        case 'Role':
            $("#hdnOrgRole").val(sValue);
            var DisplayUserRole = GetDisplayUserRole($("#hdnOrgRole").val());
            $("#spnOrgRole").html(DisplayUserRole);

            SetSelectedValues(Obj, sType);

            break;
        case 'Type':
            $("#hdnOrgType").val(sValue);
            if (sValue == "OV") {
                $("#spnOrgType").html("Operational View");
                $("#divCountry").hide();
            }
            else if (sValue == "LV") {
                $("#spnOrgType").html("Legal View");
                $("#divCountry").show();
            }

            SetSelectedValues(Obj, sType);

            break;
        case 'Level':
            $("#hdnOrgLevel").val(sValue);
            if (sValue == "One") $("#spnOrgLevel").html("Level One");
            else if (sValue == "Two") $("#spnOrgLevel").html("Level Two");
            else if (sValue == "Three") $("#spnOrgLevel").html("Level Three");
            else if (sValue == "Four") $("#spnOrgLevel").html("Level Four");
            else if (sValue == "Five") $("#spnOrgLevel").html("Level Five");
            else if (sValue == "All") $("#spnOrgLevel").html("All Levels");

            SetSelectedValues(Obj, sType);

            break;
        case 'View':
            $("#hdnOrgView").val(sValue);
            if (sValue == "Sample") $("#spnOrgView").html("Sample View");
            else if (sValue == "Normal") $("#spnOrgView").html("Normal View");
            else if (sValue == "Cost") $("#spnOrgView").html("Position Cost View");

            SetSelectedValues(Obj, sType);

            break;
    }

    return true;
}

function SelectVersion() {
    var JsonData = {
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/SelectVersion",
        data: JsonData,
        async: false,
        dateType: "json",
        success: function (jsonStr) {
            if ($(".navbar-toggle").is(":visible"))
                alert("Visible");
            else
                alert("Not Visible");
        }
    });
}

function VersionControl(vc) {
    var JsonData = {
        VersionControl: vc,
        Oper: $("#hdnOrgType").val(),
        Version: $("#hdnOrgVersion").val(),
        ChartData: $("#mySavedModel").val()
    };
    $.ajax({
        type: "POST",
        url: HOST_ENV + "/Version/VersionControl",
        data: JsonData,
        async: false,
        dateType: "json",
        success: function (jsonStr) {
            loadJSON("{ \"class\": \"go.TreeModel\",  \"nodeDataArray\":" + jsonStr + " }", jsonStr);
        }
    });

    return false;
}
