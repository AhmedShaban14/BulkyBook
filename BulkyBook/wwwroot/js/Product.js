var dataTable;

$(document).ready(function () {

    loadDataTable();

});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAllProduct",
            "type": "GET",
            "datatype":"json"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data":"price","width":"15%"},
            { "data":"author","width":"15%"},
            { "data":"category.name","width":"15%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Product/Upsert/${data}" class='btn btn-success' style='width:70px'>
                              <i class="fas fa-edit"></i>
                            </a>     
                            &nbsp;  
                             <a onclick=Delete("/Admin/Product/DeleteProduct/${data}") class='btn btn-danger text-white' style='width:70px;cursor:pointer'>
                              <i class="fas fa-trash-alt"></i>
                            </a> 
                        </div>
                        `;
                }
                , "width": "25%"
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
