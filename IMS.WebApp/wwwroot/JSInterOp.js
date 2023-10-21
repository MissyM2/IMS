function PreventFormSubmission(formId)
{

    document.getElementById(`${ formId}`).addEventListener("keydown", function(event) {

        console.log("formId", formId);

        if (event.keyCode == 13) {
                event.preventDefault();
            return false;
        }

    });

}