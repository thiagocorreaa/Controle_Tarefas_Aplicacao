var tables = {};
var options = {};
var schemas_tables;

$(document).ready(function () {

  

    $("form").submit(function () {
        $(this).children("input[name=acao]").val(this.submited);
    });


    function createSchemas() {
        schemas_tables = {
            'tarefa_tabela': {
                'fields': [
                    { 'name': 'Titulo', 'label': 'Título', 'form': { 'title': 'Título', 'required': 'required' } },
                    { 'name': 'Status', 'label': 'Status', 'form': { 'title': 'Status', 'required': 'required' } },
                    { 'name': 'Descricao', 'label': 'Descrição', 'form': { 'title': 'Descrição' } },
                    { 'name': 'Data_Criacao', 'label': 'Data Inclusão' },
                    { 'name': 'Data_Edicao', 'label': 'Data Edição' },
                    { 'name': 'Data_Remocao', 'label': 'Data Remoção' },
                    { 'name': 'Data_Conclusao', 'label': 'Data Conclusão' },
                ],
                'buttons': [
                    create_button,
                    edit_button,
                    remove_button
                ],
            }
        }
    }

    createSchemas();
    createTables();   
    enableButtons(tables['tarefa_tabela']);
    myPlugin(tables['tarefa_tabela']);
             
    function createTables() {
        tables['tarefa_tabela'] = createDatatable('tarefa_tabela');
    }

    function createDatatable(type_table) {
        createHeadersHtml(type_table)

        fields = schemas_tables[type_table]['fields']

        columns = fields.filter(function (elem) { return !('type' in elem) || (('type' in elem) && (elem['type'] != 'hidden')) })
        columns = columns.map(function (elem) { return { 'data': elem['name'], 'visible': typeof (elem['name']) != 'undefined' && elem['name'].toLowerCase() == 'ativo' ? false : elem['visible'] } })
       
        var buttons = schemas_tables[type_table]['buttons']

        var datatable = $('#' + type_table + '-table').DataTable({
            dom: 'Bfrtip',
            autoWidth: false,            
            responsive:true,
            pageLength: 10,
            ajax: {
                url: "/Home/GridTarefas",
                type: "GET"
            },
            columns: columns,
            processing: true,
            serverSide: true,
            select: true,
            scrollX: true,
            buttons: buttons,           
            language: {
                processing: "Processando ...",
                search: "Pesquisar",
                lengthMenu: "Mostrar _MENU_ itens",
                info: "Exibição dos itens  _START_ até _END_ em _TOTAL_ itens",
                infoEmpty: "Exibição do item 0 até 0 de 0 itens",
                infoFiltered: "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)",
                infoPostFix: "",
                loadingRecords: "Carregando ...",
                zeroRecords: "Nenhum item exibido",
                emptyTable: "Nenhum dado disponível na tabela",
                paginate: {
                    first: "Primeiro",
                    previous: "Anterior",
                    next: "Seguinte",
                    last: "Passado"
                },
                aria: {
                    sortAscending: ": ativar para classificar a coluna em ordem crescente",
                    sortDescending: ": ativar para classificar a coluna em ordem decrescente"
                }
            }
        });

       // myPlugin(datatable);

        return datatable;
    }    
});
