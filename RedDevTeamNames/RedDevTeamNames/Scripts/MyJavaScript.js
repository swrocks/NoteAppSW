var uri = 'api/notes';

//$(document).ready(function () {
//    // Send an AJAX request
//    refreshNoteList();
//});

//function formatItem(item) {
//    return item.Priority + ">" + item.Subject + ":  " + item.Details;
//}

function refreshNoteList() {
    $.getJSON(uri)
        .done(function (data) {
            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {
                // Add a list item for the product.
                // Change the way to format the string(Sunny)
                $('#notes').append('<li><a data-transition="pop" data-parm=' + item.Id + ' href="#details-page"><div hidden>' + item.Priority + '</div>' + item.Subject + '</a></li>');
                // Listview refresh after each inner loop(Sunny)
                $("#notes").listview("refresh");
            });
        });
};

function clearResponse() {
    $('#deleteResponse').text("");
    $('#saveResponse').text("");
};

function find() {
    clearResponse()
    var id = $('#noteId').val();
    $.getJSON(uri + '/' + id)
        .done(function (data) {
            $('#note').text(formatItem(data));
        })
        .fail(function (jqXHR, textStatus, err) {
            $('#note').text('Error: ' + err);
        });
};

function deleteNote() {
    clearResponse()
    var id = $('#deleteNote').val();
    $.ajax({
        url: uri + "/" + id,
        type: "DELETE",
        contentType: "application/json",
        success: function () {
            $("#notes").empty();
            refreshNoteList();
            $('#deleteResponse').text("Success: Note Deleted");
            $("#deleteNote").val('');    
        },
        error: function () {
            $('#deleteResponse').text("Error: Delete Failed");
        }
    });
};

function saveNote() {
    clearResponse()
    var note = {
        subject: $('#Subject').val(),
        details: $('#Details').val(),
        priority: $('#Priority').val()
    };

    $.ajax({
        url: uri + "/notes",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(note),
        success: function (data) {
            //self.notes.push(data);
            $("#notes").empty();
            refreshNoteList();
            $('#saveResponse').text("Success: Saved Note");
            $("#Subject").val('');
            $("#Details").val('');    
            $("#Priority").val('');    
        },
        error: function () {
            $('#saveResponse').text("Error: Save Failed");
        }
    });
};

//Added a page init for the adding the notes to the page
$(document).on('pageinit', '#pageone', function () {
    refreshNoteList();
});



$(document).on('pagebeforeshow', '#pageone', function () {
    //changed the onclick event. It used to look like $('a').on("click", function).......
    $(document).on("click", 'a', function (event) {     
        var parm = $(this).attr("data-parm");  //Get the para from the attribute in the <a> tag
        $("#detailParmHere").html(parm); //set the hidden <p> to the parm
    });
});

$(document).on('pagebeforeshow', '#delete-page', function () {

});

$(document).on('pagebeforeshow', '#details-page', function () {
    var Priority;
    var Subject;
    var Details;
    var id = $('#detailParmHere').text();
    $.getJSON(uri)  //get the notes again, 
        .done(function (data) {
            $.each(data, function (index, record) {
                if (id == record.Id) {//then search threw them to find the matching id
                    Priority = "Priority: " + record.Priority;
                    Subject = " Subject: " + record.Subject;
                    Details = " Details: " + record.Details;
                    $('#showdata').text(Priority).append('<br />');; //display to details page
                    $('#showdata').append(Subject).append('<br />');;
                    $('#showdata').append(Details);
                }
            });
        });
});