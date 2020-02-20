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
                    { 'name': 'titulo', 'label': 'Título', 'form': { 'title': 'Título', 'required': 'required' } },
                    { 'name': 'status', 'label': 'Status', 'form': { 'title': 'Status', 'required': 'required' } },
                    { 'name': 'descricao', 'label': 'Descrição', 'form': { 'title': 'Descrição' } },
                    { 'name': 'data_criacao', 'label': 'Data Inclusão' },
                    { 'name': 'data_edicao', 'label': 'Data Edição' },
                    { 'name': 'data_remocao', 'label': 'Data Remoção' },
                    { 'name': 'data_conclusao', 'label': 'Data Conclusão' },
                ],
                'buttons': [
                    create_button,
                    edit_button,
                    remove_button
                ],
            }
        }
    }

    function getSetTableData(type_table, data) {

        $.ajax({
            "async": true,
            "crossDomain": true,
            "url": "/home/editingTables?table=" + type_table,
            "method": "POST",
            "data": data,
            success: function (response) {
                if (type_table in tables) {
                    tables[type_table].ajax.reload();
                }
                $("#" + type_table + "-form_seguinte").unbind('click')
                $("#" + type_table + "-form").modal('hide');
                getOpcoes(type_table);
            },
        });
    }

    function salvaDados() {
        $("#animationload").show();
        $.ajax({
            "async": true,
            "crossDomain": true,
            "url": "/home/salvaDados",
            "method": "GET",
            "data": { val: $('#optionScript').val(), texto: $('#optionScript').children("option:selected")[0].text },
            "headers": {
                "content-type": "application/json"
            },
            success: function (response) {
                if (response.result == 'OK') {
                    optionSelected = { val: response.val, texto: response.texto, active: true }
                }
                $('#optionScript').val(optionSelected.val)
            },
            complete: function () {
                $("#animationload").hide();
            },
            error: function () {
                $('#optionScript').val(optionSelected.val)
            }
        });
    }

    function getOpcoes(type_table) {
        $.ajax({
            "async": true,
            "crossDomain": true,
            "url": "/home/getInfoParams",
            "method": "POST",
            "headers": {
                //"authorization": "Bearer " + $("#CNPJ").attr("dataaccesstoken"),
                "content-type": "application/json"
            },
            success: function (result) {
                if (Object.keys(options).length == 0) {
                    createSchemas();
                    createTables();
                }

                if (type_table) {
                    enableButtons(tables[type_table]);
                }
            }
        });
    }

    function createTables() {
        tables['tarefa_tabela'] = createDatatable('tarefa_tabela');
    }
    function createDatatable(type_table) {
        createHeadersHtml(type_table)

        fields = schemas_tables[type_table]['fields']

        columns = fields.filter(function (elem) { return !('type' in elem) || (('type' in elem) && (elem['type'] != 'hidden')) })
        columns = columns.map(function (elem) { return { 'data': elem['name'], 'visible': typeof (elem['name']) != 'undefined' && elem['name'].toLowerCase() == 'ativo' ? false : elem['visible'] } })

        buttons = schemas_tables[type_table]['buttons']
        if (type_table == 'base_pre_aprovada') {
            var datatable = $('#' + type_table + '-table').DataTable({
                dom: 'frtip',
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
                rowGroup: {
                    // Group by office
                    dataSrc: 'alcada'
                },
                language: {
                    processing: "Processando ...",
                    search: "Pesquisar",
                    lengthMenu: "Mostrar _MENU_ itens",
                    info: "Exibição dos itens  _START_ até _END_ em _TOTAL_ itens",
                    infoEmpty: "Exibição do item 0 até 0 sur 0 itens",
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
        }


        myPlugin(datatable);

        return datatable;
    }
});
