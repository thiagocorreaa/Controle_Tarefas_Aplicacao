$(document).ready(function () {
    $("form").submit(function () {
         $(this).children("input[name=acao]").val(this.submited);
    });
    //getOpcoes();
    //fileEvents();
    //showModalInfoUploads();

    //$('.countErrores').click(function () {
    //    var type = this.id;
    //    type = type.replace('list_', '')
    //    if ($('#listaErrores' + type).css('display') == "none") $('#listaErrores' + type ).show();
    //    else $('#listaErrores' + type ).hide();
    //});

});

/////////////////////////////////MYPLUGIN//////////////////////////////////

function getModal(dt, action){
    var type_table = $(dt).context[0].nTable.id.replace('-table', '');
    var modal = $("<div class='modal fade' id='"+type_table+"-form' tabindex='-1' role='dialog' aria-labelledby='"+type_table+"-form' aria-hidden=true'></div>")

    var modal_dialog = $("<div class='modal-dialog'></div>")
    var modal_content = $("<div class='modal-content'></div>")

    var modal_header = $("<div class='modal-header'></div>")
    var modal_body = $("<div class='modal-body'></div>")
    var modal_footer = $("<div class='modal-footer'></div>")


    modal_content.append(modal_header)
    modal_content.append(modal_body)
    modal_content.append(modal_footer)

    modal_dialog.append(modal_content)
    modal.append(modal_dialog)

    $('#'+$(dt).context[0].nTable.id).parent().parent().append(modal)
}

function camelize(word){
    return word.substring(0,1).toUpperCase() + word.substring(1, word.length).toLowerCase()
}

function getHtml(dt, action){
    var type_table = $(dt).context[0].nTable.id.replace('-table', '');
    if ($('#'+type_table+'-form').length ==0){
        getModal(dt,action)
    }
    
    var form = $("<form class='form-horizontal'></form>")

        
    var superDiv;
    var data = {};
    if (action == 'edit' || action == 'remove')
        data = dt.rows( { selected: true } ).data()[0];
    schemas_tables[type_table]['fields'].forEach(function(elem, index){
        var disabled = '';
        if ('form' in elem && 'disabled' in elem['form'] && elem['form']['disabled'].indexOf(action)>=0){
            if (elem['form']['type'] == 'select'){
                disabled = 'disabled';
            }else{
                disabled = 'readonly';
            }
        }

        var div;
        if (elem.visible != null && !elem.visible) {
            div = $("<div class=' col-sm-offset-2 col-md-offset-2 col-sm-6 col-md-6 hidden'> </div>")
    }
        else {
            div = $("<div class=' col-sm-offset-2 col-md-offset-2 col-sm-6 col-md-6'> </div>")
        }
        

        var input;
        var events = ('form' in elem && 'events' in elem.form) ? $.map(elem.form.events, function (val, key) { return key + "='" + val + "'" }).join(' ') : '';
        if ('form' in elem && elem['form']['type'] == 'select' && (action != 'edit' || !disabled)) {
            input = $("<select name='" + elem['name'] + "' " + events +"></select>")
            var selected = "";
            var default_opt = "";
            var options = elem['form']['options']
            var any_selected = false;
            if (typeof(options) == 'function'){
                options = options(action, elem, data)
            }
            options.forEach(function (elem1, index1) {
                if ((Object.keys(data).length > 0 && data[elem['name']] == elem1) && (selected == "" && default_opt == "")) {
                    selected = "selected"
                    any_selected = true;
                }

                if (elem1 == 'Sim' && (selected == "" && default_opt == "") ){
                    default_opt = 'selected="selected"'
                    any_selected = true;
                }
                var option = $("<option value='" + elem1 + "' " + selected + " "+disabled+" "+ default_opt +">" + elem1 + "</option>")
                input.append(option)
                selected = "";
                default_opt = "";
            })
            if (!any_selected && data[elem['name']]!= null) {
                var option = $("<option value='" + data[elem['name']] + "' selected " + disabled + " " + default_opt + ">" + data[elem['name']] + "</option>")
                input.append(option)
            }
        }
        else{

            var title = ''
            if ('form' in elem && 'title' in elem['form'])
                title = "title='" + elem['form']['title'] + "'"

            var placeholder = ''
            if ('form' in elem && 'placeholder' in elem['form'])
                placeholder = "placeholder='" + elem['form']['placeholder'] + "'"

            var pattern = ''
            if ('form' in elem && 'pattern' in elem['form'])
                pattern = "pattern='" + elem['form']['pattern'] + "'"

            var type_input = 'text'
            if ('form' in elem && 'type' in elem['form'])
                if ('form' in elem && elem['form']['type'] == 'select' && action == 'edit')
                    type_input = 'text'
                else
                    type_input = elem['form']['type']

            var min_max = ''
            if ('form' in elem && 'min' in elem['form'])
                min_max = "min=" + elem['form']['min']
            if ('form' in elem && 'max' in elem['form'])
                min_max += " max=" + elem['form']['max']

            var value_input = ''
            if (Object.keys(data).length > 0){
                value_input = data[elem['name']];
                if (value_input != null) {
                    if (type_input == 'date') {
                        value_input = value_input.replace('/', '-').replace('/', '-')
                    }
                }
                else {
                    value_input = ''
                }
            }
            var required = ''
            
            if ('form' in elem && 'required' in elem['form'])
            {
                required = elem['form']['required']
            }
            input = $("<input " + placeholder + " " + title + " " + min_max + " " + pattern + "' name='" + elem['name'] + "' type='" + type_input + "' class='form-control " + elem['class']+"' id='" + elem['name'] + "' value='" + value_input + "' " + disabled + " " + required + " " + events + ">")
            
        }

        div.append(input)

        var label = "";

        if (elem.visible != null && !elem.visible) {
            label = $("<label class='control-label col-sm-offset-2 col-md-offset-2 col-sm-2 col-md-2' for='" + elem['name'] + "' hidden>" + elem['label'] + ":</label>")
        }
        else {
            label = $("<label class='control-label col-sm-offset-2 col-md-offset-2 col-sm-2 col-md-2' for='" + elem['name'] + "'>" + elem['label'] + ":</label>")
        }
        
        superDiv = $("<div class='form-group'></div>")

        superDiv.append(label)
        superDiv.append(div)

        form.append(superDiv)
        
    })
    var button = $("<button id='"+type_table+"-form_seguinte' type='submit' class='btn btn-default'>Validar</button>")
    form.append(button)
    $("#"+type_table+"-form").find('.modal-body').html(form)
}

function createForm(dt, action, title){
    var title_camelized = camelize(title);
    var type_table = $(dt).context[0].nTable.id.replace('-table', '');
    var form_exist =  $('#'+type_table+'-form').length != 0
    getHtml(dt, action)
    window._action = action;
    $("#"+type_table+"-form").find('.modal-header').html('<h3>'+title_camelized+'</h3>');
    $("#"+type_table+"-form").modal('show');
    if (!form_exist) {
        $("#" + type_table + "-form").submit(function (e) {
            action = window._action;
            e.preventDefault();
            $("#" + type_table + "-form form option").prop('disabled', false);
            $("#" + type_table + "-form form input").prop('disabled', false);
            var data = $("#" + type_table + "-form form").serializeArray();
            $("#" + type_table + "-form form option").prop('disabled', true);
            $("#" + type_table + "-form form input").prop('disabled', true);
            data.push({ name: 'action', value: action })
            if (dt.rows( { selected: true } ).nodes()[0]){
                data.push({ name: 'id', value: dt.rows( { selected: true } ).nodes()[0].id })
            }
            if (type_table === 'cdi' || type_table === 'tecnologiasValores' || type_table === 'segmentosValores') {
                data = cleanParams(data, type_table);
            }
            getSetTableData(type_table, data)

        });
    }
}

function creating_element(e, dt, node, config){
    createForm(dt, config['name'], config['text'])
   
}

function edit_element(e, dt, node, config){
    createForm(dt, config['name'], config['text'])
  
}

function remove_element(e, dt, node, config){
    createForm(dt, config['name'], config['text'])
   
}

var create_button = {
            'name':'create',
            'text': 'Criar',
            'action': creating_element,
        }

var edit_button = {
            'name':'edit',
            'text': 'Editar',
            'action': edit_element,
}

var remove_button = {
            'name':'remove',
            'text': 'Remover',
            'action': remove_element,
        }

function getOptions(action, elem, data) {
    var options_return = []
    if (elem['name'] == 'Cod_Regional')
        if (action == 'create') {
            options_return = $.unique($.map(options['redes_regionais'], function (item) { return item.cod_regional }))
            options_return.unshift('')
        }
        else {
            val = data['Cod_Rede']
            item_rede = $.grep(options['redes_regionais'], function (item) { return item['cod_rede'] == val });
            options_return = $.unique($.map(item_rede, function (item) { return item.cod_regional }))
            options_return.unshift('')
        }
    return options_return 
}

function setSelected(name, data) {
    $("[name='" + name + "']").val(data[0])
}

function setNome(id, data) {
    if (data.length > 0 )
        $('#' + id).val(data[0])
}

function cleanSetOptions(nome, data) {
    var select = $("[name='" + nome + "']");
    select.empty();
    $.each(data, function (pos, value) {
        select.append($("<option></option>")
            .attr("value", value).text(value));
    });
}

////////////////////////////////////////////////////////////////////////////////////


function printDatas(info){
    info.forEach(function(elem){
        $('#'+elem['tabela']).html(elem['data'])
    });
}

function createHeadersHtml(type_table){
    var headers = $('#'+type_table+'-table tr')

    schemas_tables[type_table]['fields'].forEach(function(elem, index){
        headers.append('<th>'+elem['label']+'</th>')
    })
}

/////////////////////////////////MYPLUGIN//////////////////////////////////
function enableButtons(dt){
    dt.button( 'edit:name' ).enable(
        dt.rows( { selected: true } ).indexes().length === 0 ?
            false :
            true
    );
    dt.button( 'remove:name' ).enable(
        dt.rows( { selected: true } ).indexes().length === 0 ?
            false :
            true
    );
}

function myPlugin(dt){
    enableButtons(dt)

    dt.on( 'select', function ( e, dt, type, indexes ) {
        if ( type === 'row' ) {
            var data = dt.rows( indexes ).data().pluck( 'id' );
            enableButtons(dt)
        }
    } );

    dt.on( 'deselect', function ( e, dt, type, indexes ) {
        if ( type === 'row' ) {
            var data = dt.rows( indexes ).data().pluck( 'id' );
            enableButtons(dt)
            // do something with the ID of the deselected items
        }
    } );            
}
////////////////////////////////////////////////////////////////////////////////////

function fileEvents(){
    $(document).on('change', ':file', function() {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [numFiles, label]);
    });

    $(':file').on('fileselect', function(event, numFiles, label) {

        var input = $(this).parents('.input-group').find(':text'),
        log = numFiles > 1 ? numFiles + ' files selected' : label;

        if( input.length ) {
            input.val(log);
        } else {
            if( log ) alert(log);
        }

    });

    $('.panel.panel-default').on('shown.bs.collapse',function(){
        $(this).find('table').each(function () {
            if (this.id != '') {
                tables[this.id.replace('-table', '')].columns.adjust().draw();
            }
        })
    });
}