var dataTable;

$(document).ready(function () {
    loadDataTable("Admin", "User");
})

function loadDataTable(area, controller) {
    dataTable = $('#tableData').DataTable({
        ajax: {
            url: `/${area}/${controller}/GetAll`
        },
        columns: [
            { data: 'name' },
            { data: 'email' },
            { data: 'phoneNumber' },
            { data: 'company.name' },
            { data: 'role' },
            {
                data: {
                    id: 'id',
                    lockoutEnd: 'lockoutEnd'
                },
                render: function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    let isLocked = lockout > today;

                    let lock = `<div class="text-center">
                                    <a onclick=lockAlert('${data.id}',${isLocked}) class="btn btn-danger" style="cursor:pointer;">
                                        <i class="fas fa-lock"> Lock</i>
                                    </a>
                                </div>`;
                    let unLock = `<div class="text-center">
                                    <a onclick=lockAlert('${data.id}',${isLocked}) class="btn btn-success" style="cursor:pointer;">
                                        <i class="fas fa-lock-open"> Unlock</i>
                                    </a>
                                  </div>`;

                    return isLocked ? unLock : lock;
                }
            }
        ],
    });
}

function lockAlert(id, isLocked) {
    let context = isLocked ? "unlock" : "lock";

    swal({
        title: `Are you sure you want to ${context} this user?`,
        text: `This user will ${isLocked ? "" : "not"} be able to access to website`,
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then(willDelete => {
            if (willDelete) {
                $.ajax({
                    type: "POST",
                    url: '/Admin/User/LockUnlock',
                    data: JSON.stringify(id),
                    contentType: "application/json",
                    success: function (data) {
                        if (data.success) {
                            toastr.success(data.message);
                            dataTable.ajax.reload();
                        }
                        else {
                            toastr.error(data.message);
                        }
                    }
                })
                
            }
        })
}