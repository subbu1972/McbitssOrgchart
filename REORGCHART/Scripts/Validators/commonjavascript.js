/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


// Checks date difference in days and returns boolean value
function validatDate(fromDate, toDate, noOfDays) {
    var timeDiff = Math.abs(toDate.getTime() - fromDate.getTime());
    var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
    return diffDays <= noOfDays;
}

//On error Remove image Element and Add message No Image
function imageError(thisImage) {
    $(thisImage).hide();
    $(thisImage).parent().html("No Image");
    //  alert($(thisImage));
}

function imageZoom() {
    $('#zoomImage').click(function (event) {
        var scale = 150 / 100;
        var pos = $(this).offset();
        var clickX = event.pageX - pos.left;
        var clickY = event.pageY - pos.top;
        var container = $(this).parent().get(0);
        $(this).css({
            width: this.width * scale,
            height: this.height * scale
        });
        container.scrollLeft = ($(container).width() / -2) + clickX * scale;
        container.scrollTop = ($(container).height() / -2) + clickY * scale;
    });
}

function get_age(born, now) {
    var birthday = new Date(now.getFullYear(), born.getMonth(), born.getDate());
    if (now >= birthday)
        return now.getFullYear() - born.getFullYear();
    else
        return now.getFullYear() - born.getFullYear() - 1;
}

function calculateAge1(dateOfBirth, dateToCalculate) {
    var calculateYear = dateToCalculate.getFullYear();
    var calculateMonth = dateToCalculate.getMonth();
    var calculateDay = dateToCalculate.getDate();

    var birthYear = dateOfBirth.getFullYear();
    var birthMonth = dateOfBirth.getMonth();
    var birthDay = dateOfBirth.getDate();

    var age = calculateYear - birthYear;
    var ageMonth = calculateMonth - birthMonth;
    var ageDay = calculateDay - birthDay;

    if (ageMonth < 0 || (ageMonth === 0 && ageDay < 0)) {
        age = parseInt(age) - 1;
    }
    return age;
}

function multiZoom_img() {
    $('.multizoom').addimagezoom({// si
        imagevertcenter: true, // zoomable image centers vertically in its container (optional) - new
        magvertcenter: true, // magnified area centers vertically in relation to the zoomable image (optional) - new
        zoomrange: [5, 10],
        magnifiersize: [450, 450],
        magnifierpos: 'right',
        cursorshade: true
    });
}

function fillsubService() {
    $("#serviceId").change(function () {
        $('#subServiceId , #subSubServiceId').empty();

        $('#subServiceId , #subSubServiceId').attr('disabled', true);


        var serviceId = $(this).val();
        $('#subServiceId , #subSubServiceId').empty().append($('<option>', {
            value: '',
            text: 'All'
        }));
 resetRevalidateForm($(this).closest("form").attr('id'));
        //Show sub service only on ecommere and egovernance
        if (serviceId === '2' || serviceId === '6') {
            $('#subServiceId').attr('disabled', false);
            //$('#subSubServiceId').attr('disabled',false);


            $.ajax({
                url: 'StandardEquipmentMasterServlet',
                data: {
                    fillsubService: 1, serviceId: serviceId
                },
                success: function (items) {

                    $.each(items, function (i, item) {
                        $('#subServiceId').append($('<option>', {
                            value: item.subServiceId,
                            text: item.vkDescription
                        }));
                    });
                }
            });
        }
    });
}


function fillsubSubService() {
    $("#subServiceId").change(function () {
        $('#subSubServiceId').attr('disabled', true);
        $('#subSubServiceId').empty();
        $('#subSubServiceId').append($('<option>', {
            value: '',
            text: 'All'
        }));
        var subServiceId = $(this).val();
        resetRevalidateForm($(this).closest("form").attr('id'));
        $.ajax({
            url: 'StandardEquipmentMasterServlet',
            data: {
                fillSubSubService: 1, subServiceId: subServiceId
            },
            success: function (items) {
                if (items.length > 0) {
                    $('#subSubServiceId').attr('disabled', false);
                }
                $.each(items, function (i, item) {
                    $('#subSubServiceId').append($('<option>', {
                        value: item.subSubServiceId,
                        text: item.subSubServiceName
                    }));
                });
            }
        });
    });
}
function resetRevalidateForm(formName) {
    try{
    $('#' + formName).bootstrapValidator('destroy');
     $('#' + formName).data('bootstrapValidator', null);
     $('#' + formName).bootstrapValidator();
 }catch(er){
     console.log(er);
     
 }
}