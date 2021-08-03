var dataTable;

$(document).ready(function () {

    loadDataTable();

});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAllCompany",
            "type": "GET",
            "datatype":"json"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "10%" },
            { "data":"city","width":"15%"},
            { "data":"state","width":"15%"},
            { "data": "phoneNumber", "width": "15%" }, 
            { "data": "isAuthorizedCompany", "width": "15%" }, 
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Company/Upsert/${data}" class='btn btn-success' style='width:70px'>
                              <i class="fas fa-edit"></i>
                            </a>     
                            &nbsp;  
                             <a onclick=Delete("/Admin/Company/DeleteCompany/${data}") class='btn btn-danger text-white' style='width:70px;cursor:pointer'>
                              <i class="fas fa-trash-alt"></i>
                            </a> 
                        </div>
                        `;
                }
                , "width": "20%"
            },
        ],
        "language": {
            "emptyTable":"No Records"
        },"width":"100%"
    });
}

function Delete(url) {
    swal({
        title: "Are You Sure You Want To delete ?! ",
        text: "You will not be able to retrieve it again ! ",
        type: "warning",
        confirmButtonText: "Yes,Delete !",
        confirmButtonColor: "#DD6b55    ",
        closeOnConfirm: true,
        showCancelButton: true,
    }, function () {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success == 'true') {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.success(data.message);
                    }
                }
            });
    });
}
