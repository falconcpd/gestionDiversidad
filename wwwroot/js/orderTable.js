document.addEventListener("DOMContentLoaded", function () {
    $('#listaAuditorias').DataTable({
        "language": {
            "search": "Buscar:",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "sInfoFiltered": "(filtrado de un total de _MAX_ entradas)",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "�ltimo",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "No hay datos disponibles en la tabla",
            "infoEmpty": "Mostrando 0 a 0 de 0 entradas"
        }
    });
    $('#listaInformes').DataTable({
        "language": {
            "search": "Buscar:",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "sInfoFiltered": "(filtrado de un total de _MAX_ entradas)",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "�ltimo",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "No hay informes disponibles en esta tabla",
            "infoEmpty": "Mostrando 0 a 0 de 0 entradas"
        }
    });
    $('#listaAsignaturas').DataTable({
        "language": {
            "search": "Buscar:",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "sInfoFiltered": "(filtrado de un total de _MAX_ entradas)",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "�ltimo",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "No hay asignaturas disponibles en esta tabla",
            "infoEmpty": "Mostrando 0 a 0 de 0 entradas"
        }
    });
});