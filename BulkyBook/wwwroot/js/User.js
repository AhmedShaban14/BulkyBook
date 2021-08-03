var dataTable;

$(document).ready(function () {

    loadDataTable();

});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/User/GetAllUsers",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "email", "width": "30%" },
            { "data": "phoneNumber", "width": "30%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    debugger

                    if (data.lockoutEnd == null || new Date(data.lockoutEnd).getTime() < new Date().getTime()) {
                        return `
                        <div class="text-center">
                            <a href="/Admin/User/Lock/${data.id}" class='btn btn-success' style='width:70px'>
                              <i class="fas fa-lock"></i>
                            </a>
                            &nbsp;
                             <a onclick=Delete("/Admin/User/DeleteUser/${data.id}") class='btn btn-danger text-white' style='width:70px;cursor:pointer'>
                              <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <a href="/Admin/User/UnLock/${data.id}" class='btn btn-danger' style='width:70px'>
                              <i class="fas fa-lock-open"></i>
                            </a>     
                            &nbsp;  
                             <a onclick=Delete("/Admin/User/DeleteUser/${data.id}") class='btn btn-danger text-white' style='width:70px;cursor:pointer'>
                              <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                        `;
                    }
                }
                , "width": "40%"
            },
        ],
        "language": {
            "emptyTable": "No Records"
        }, "width": "100%"
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
