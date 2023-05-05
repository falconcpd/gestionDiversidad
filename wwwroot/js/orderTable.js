document.addEventListener("DOMContentLoaded", function () {
    $('#example').DataTable({
        "language": {
            "search": "Buscar:",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "sInfoFiltered": "(filtrado de un total de _MAX_ entradas)",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "No hay datos disponibles en la tabla",
            "infoEmpty": "Mostrando 0 a 0 de 0 entradas"
        }
    });
});