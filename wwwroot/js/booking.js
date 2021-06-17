var LastUpdated = {
    "Adult": $("input#NoAdult").val(),
    "Youth": $("input#NoYouth").val(),
    "Children": $("input#NoChildren").val(),
    "Baby": $("input#NoBaby").val()
}

var max_capacity = parseInt($("td#Vacancies").text());


function onCountChange(ageGroup, value) {
    if (value == LastUpdated[ageGroup]) {
        return;
    }


    if (ageGroup == "Adult" && value < "1") {
        alert(`Invalid number of ${ageGroup}`);
        reset(ageGroup);
        return;
    }


    if (!value || value < "0") {
        value = "0";
        document.getElementById(`No${ageGroup}`).value = value;
    }

    if (!updateTotal()) {
        reset(ageGroup);
        return;
    }

    LastUpdated[ageGroup] = value;
    updateCustomerInfos();
}


function updateTotal() {
    let totalInp = $("input#NoTotal");

    let newTotal = parseInt($("input#NoAdult").val())
        + parseInt($("input#NoYouth").val())
        + parseInt($("input#NoChildren").val())
        + parseInt($("input#NoBaby").val());

    if (newTotal > max_capacity) {
        alert("The number of visitor exceeds the available slots");
        return false;
    }

    totalInp.attr('value', newTotal);
    return true;
}

function reset(ageGroup) {
    document.getElementById(`No${ageGroup}`).value = LastUpdated[ageGroup];
}

function onDobUpdated(ageGroup, input) {
    let dobRange = document.getElementById(`Range_${ageGroup}`).innerHTML.split(" - ");
    let from = moment(dobRange[0], "DD/MM/YYYY").toDate();
    let to = moment(dobRange[1], "DD/MM/YYYY").toDate();

    let inputDate = moment(input.value).toDate();

    if (inputDate < from || inputDate > to) {
        alert(`Invalid date of birth for age group ${ageGroup}`);
        input.value = null;
    }
}

function updateCustomerInfos() {
    let tripID = $('#tripID').val();
    let customerinfosContainer = $('#customerinfos-container');
    let clone = customerinfosContainer.clone();
    customerinfosContainer.html('<div class="w-100 text-center"><div style="height: 50px; width: 50px" class="spinner-grow text-primary" role="status"><span class="sr-only">Loading...</span></div></div>');

    $.ajax({
        type: "GET",
        url: 'CustomerInfos',
        data: `tripID=${tripID}&adult=${LastUpdated['Adult']}&youth=${LastUpdated['Youth']}&children=${LastUpdated['Children']}&baby=${LastUpdated['Baby']}`,
        dataType: "html",
        success: function (data) {
            customerinfosContainer.html(data);
        },
        error: function () {
            Snackbar.show({
                text: 'Error processing customer age groups ❌',
                textColor: "#dc3545",
                actionTextColor: "#ffffff",
                duration: 2000,
                pos: "top-right"
            });
            customerinfosContainer.replaceWith(clone);
        }
    });
}