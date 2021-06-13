var LastUpdated = {
    "Adult": $("input#NoAdult").val(),
    "Youth": $("input#NoYouth").val(),
    "Children": $("input#NoChildren").val(),
    "Baby": $("input#NoBaby").val()
}
var max_capacity = parseInt($("td#Vacancies").text());


function onCountChange(ageGroup, value) {

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

    let numValue = parseInt(value);
    let numLastValue = parseInt(LastUpdated[ageGroup]);

    if (numValue > numLastValue) {
        for (var i = 0; i < numValue - numLastValue; i++) {
            addCustomerInfoEntry(ageGroup);
        }
    } else if (numValue < numLastValue) {
        $(`div.${ageGroup}`).each(function (index) {
            if (index >= numValue) {
                $(this).remove();
            }
        });
    }
    updatePrice();
    LastUpdated[ageGroup] = value;
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

function updatePrice() {
    let totalTag = document.getElementById("Price_Total");
    let total = 0;
    $("span.Price_Item").each(function () {
        total += parseFloat($(this).text().substr(1));
    });
    totalTag.innerHTML = `$${total.toFixed(2)}`;
}

function reset(ageGroup) {
    document.getElementById(`No${ageGroup}`).value = LastUpdated[ageGroup];
}

function onDobUpdated(ageGroup, input) {
    console.log("date changed");
    let dobRange = document.getElementById(`Range_${ageGroup}`).innerHTML.split(" - ");
    let from = moment(dobRange[0], "DD/MM/YYYY").toDate();
    let to = moment(dobRange[1], "DD/MM/YYYY").toDate();
    console.log(from);
    console.log(to);

    let inputDate = moment(input.value).toDate();
    console.log(inputDate);

    if (inputDate < from || inputDate > to) {
        alert(`Invalid date of birth for age group ${ageGroup}`);
        input.value = null;
    }

    input.classList.add("is-valid");
}

function addCustomerInfoEntry(ageGroup) {
    let custonerInfoListContainer = document.getElementById("customerInfosContainer");
    let no = custonerInfoListContainer.children.length + 1;
    let container = document.createElement("div");
    container.classList.add(ageGroup);
    var content = '<div>' +
        `                    <h6 class="mb-3">Customer No.${no}</h6>` +
        '                    <div class="row">' +
        '                        <div class="form-group col-md-3 mb-3">' +
        `                            <label for="Name_${no}">Name</label>` +
        `                            <input name="CustomerNames" id="Name_${no}" class="form-control" required>` +
        '                        </div>' +
        '                        <div class="form-group col-md-3 mb-3">' +
        `                            <label for="Sex_${no}">Sex</label>` +
        `                            <select id="Sex_${no}" name="CustomerSexes" class="form-control" required>` +
        '                                <option value="0">Male</option>' +
        '                                <option value="1">Female</option>' +
        '                            </select>' +
        '                        </div>' +
        '                        <div class="form-group col-md-3 mb-3">' +
        `                            <label for="DOB_${no}">Date of birth</label>` +
        `                            <input onfocusout="onDobUpdated('${ageGroup}', this)"  id="DOB_${no}" name="CustomerDobs" class="form-control" type="date" required />` +
        '                        </div>' +
        '                        <div class="form-group col-md-3 mb-3">' +
        `                            <label for="AG_${no}">Age Group</label>` +
        `                            <select disabled id="AG_${no}" name="CustomerAgeGroups" class="form-control">` +
        `                                <option ${ageGroup == "Adult" ? "selected" : ""} value="0">Adult</option>` +
        `                                <option ${ageGroup == "Youth" ? "selected" : ""} value="1">Youth</option>` +
        `                                <option ${ageGroup == "Children" ? "selected" : ""} value="2">Children</option>` +
        `                                <option ${ageGroup == "Baby" ? "selected" : ""} value="3">Baby</option>` +
        '                            </select>' +
        '                        </div>' +
        '                    </div>' +
        `                    <p class="text-right">Price: <span class="Price_Item text-danger font-weight-bold">${$("td#Price_" + ageGroup).text()}</span></p>` +
        '                    <hr />' +
        '                </div>';
    container.innerHTML = content;
    custonerInfoListContainer.appendChild(container);
}