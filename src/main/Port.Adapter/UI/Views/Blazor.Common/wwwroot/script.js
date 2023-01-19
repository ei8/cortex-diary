// Blazor.Common
// AvatarUrlEditorBox
var expanded = false;
function showCheckboxes(event) {
    if (!expanded) {
        event.style.display = "block";
        expanded = true;
    } else {
        event.style.display = "none";
        expanded = false;
    }
}
function updateCheckboxText(checked, event, dataset) {
    if (checked) {
        if (event.text == "Select an option") {
            event.text = dataset.value;
        } else {
            event.text += "," + dataset.value;
        }
    } else {
        if (event.text.indexOf(dataset.value) == 0) {
            event.text = event.text.replace(dataset.value, "");
            event.text = event.text.replace(",", "");
            if (event.text == "") {
                event.text = "Select an option";
            }
        }
        else {
            event.text = event.text.replace("," + dataset.value, "");
        }
    }
}

function deleteForm(child, parent) {
    if (child.value.length != 0) {

        child.value = "";
        var e = new Event('change');
        child.dispatchEvent(e);
    }
    parent.remove();
}

function deleteMultiSelectMenu(child, parent) {
    var e = new Event('change');
    event.target.parentNode.children[1].children[0].children[0].dispatchEvent(e);
    event.target.parentNode.remove();
}

//!!! tree, blazor.common.treeview
const yellow = "#ffa500";
const gray = "#e6e6e6";
const blue = "#1E90FF";

//!!! tree, blazor.common.treeview
function hover(tagName, tagId, over, highlight) {
    var element = document.getElementsByTagName(tagName);
    for (var i = 0; i < element.length; i++) {
        var currElement = element[i];
        if (currElement.id == tagId) {
            highlight(currElement, over);
        }
    }
}

// blazor.common.treeview
function highlightNode(currElement, over) {
    if (over)
        currElement.children[0].classList.add("blue");
    else
        currElement.children[0].classList.remove("blue");
}
