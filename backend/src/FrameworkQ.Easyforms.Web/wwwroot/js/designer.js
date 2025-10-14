/* Simple HTML form/composite designer (drag & drop + properties + code/preview) */
(function(window, $){
  'use strict';

  var $canvas, $codeView, $preview;
  var $propsBox, $propsNone, $propsGrid;
  var selected = null;

  $(function(){
    $canvas = $('#canvas');
    $codeView = $('#codeView');
    $preview = $('#preview');
    $propsBox = $('#props');
    $propsNone = $propsBox.find('.props__none');
    $propsGrid = $propsBox.find('.props__grid');

    initTabs();
    initPalette();
    initCanvas();
    initProps();
    initActions();

    // New default form
    newForm();
  });

  function initTabs(){
    $('.tab-btn').on('click', function(){
      var tab = $(this).data('tab');
      $('.tab-btn').removeClass('active'); $(this).addClass('active');
      $('.tab').removeClass('active'); $('#tab-' + tab).addClass('active');
      if (tab === 'code'){ refreshCode(); }
      if (tab === 'preview'){ refreshPreview(); }
    });
  }

  function initPalette(){
    $('#palette .palette-item').on('dragstart', function(e){
      e.originalEvent.dataTransfer.setData('text/plain', JSON.stringify({ template: $(this).attr('data-template')||'', composite: $(this).attr('data-composite')==='true' }));
    });
  }

  function initCanvas(){
    $canvas.on('dragover', function(e){ e.preventDefault(); e.originalEvent.dataTransfer.dropEffect = 'copy'; });
    $canvas.on('drop', function(e){ e.preventDefault(); var data = e.originalEvent.dataTransfer.getData('text/plain');
      try{
        var info = JSON.parse(data||'{}');
        if (info.composite){
          var cname = promptCompositeName(); if (!cname) return;
          var id = uniqueId('comp');
          var html = '<div data-composite="'+cname+'" id="'+id+'"></div>';
          insertAtCursor(html);
        } else if (info.template){ insertAtCursor(info.template); }
        attachSelectionHandlers();
      }catch(err){ console.error(err); }
    });

    $canvas.on('click', function(e){ var $t=$(e.target); if ($t.is('#canvas')){ selectElement(null); } });
  }

  function insertAtCursor(html){
    // Insert inside the currently selected container if any, else at canvas end
    if (selected && canContain(selected)) {
      $(selected).append(html);
    } else {
      $canvas.append(html);
    }
  }

  function canContain(el){
    var tag = el.tagName.toLowerCase();
    return ['div','section','tbody','thead','tfoot','table','form'].indexOf(tag) !== -1;
  }

  function attachSelectionHandlers(){
    $canvas.find('*').off('click.designer').on('click.designer', function(e){ e.stopPropagation(); selectElement(this); });
  }

  function selectElement(el){
    if (selected){ $(selected).removeClass('selected'); }
    selected = el;
    if (!el){ $propsNone.show(); $propsGrid.hide(); return; }
    $(selected).addClass('selected');
    $propsNone.hide(); $propsGrid.show();
    populateProps();
  }

  function initProps(){
    $('#btnApplyProps').on('click', function(){ applyProps(); });
    $('#btnDelete').on('click', function(){ if (!selected) return; var toRemove = selected; selectElement(null); $(toRemove).remove(); });
  }

  function populateProps(){
    var $el = $(selected); var tag = selected.tagName.toLowerCase();
    $('#propTag').val(tag);
    $('#propId').val($el.attr('id')||'');
    $('#propClass').val($el.attr('class')||'');
    $('#propName').val($el.attr('name')||'');
    $('#propStyle').val($el.attr('style')||'');
    $('#propType').val($el.attr('type')||'');
    $('#propRequired').prop('checked', $el.is('[required]'));
    // text content only for label/button/th/td/span/div without child elements
    var textEditable = ['label','button','th','td','span','div'].indexOf(tag) !== -1 && $el.children().length===0;
    $('#propText').prop('disabled', !textEditable).val(textEditable ? ($el.text()||'') : '');
    // data attributes
    var lines = [];
    $.each(selected.attributes, function(){ var n=this.name; if (n.startsWith('data-')){ lines.push(n+'='+this.value); } });
    $('#propData').val(lines.join('\n'));
  }

  function applyProps(){
    if (!selected) return; var $el=$(selected);
    var id=$('#propId').val(); var cls=$('#propClass').val(); var name=$('#propName').val(); var style=$('#propStyle').val(); var type=$('#propType').val(); var req=$('#propRequired').is(':checked'); var text=$('#propText').val();
    setOrRemove($el, 'id', id); setOrRemove($el,'class',cls); setOrRemove($el,'name',name); setOrRemove($el,'style', style);
    if (type) $el.attr('type', type); else $el.removeAttr('type');
    if (req) $el.attr('required', 'required'); else $el.removeAttr('required');
    if (!$('#propText').prop('disabled')){ $el.text(text||''); }
    // data attrs
    var lines=$('#propData').val().split(/\n/); lines.forEach(function(line){ line=line.trim(); if (!line) return; var idx=line.indexOf('='); if (idx<0) return; var k=line.substring(0,idx).trim(); var v=line.substring(idx+1).trim(); $el.attr(k, v); });
  }

  function setOrRemove($el, name, val){ if (val) $el.attr(name, val); else $el.removeAttr(name); }

  function initActions(){
    $('#btnNew').on('click', function(){ if (confirm('Clear the canvas?')) newForm(); });
    $('#btnDownload').on('click', function(){ var html = getFormHtml(); download('form.html', html); });
  }

  function newForm(){
    $canvas.empty();
    var id = uniqueId('form');
    var html = ''+
      '<form data-form="new-form" data-title="New Form" data-version="1.0" id="'+id+'">' +
      '  <section data-page id="page-1" data-title="Page 1">' +
      '    <section data-section id="main" data-title="Main"></section>' +
      '  </section>' +
      '</form>';
    $canvas.html(html);
    attachSelectionHandlers();
    selectElement($canvas.find('form')[0]);
  }

  function refreshCode(){ $codeView.text(getFormHtml()); }

  function refreshPreview(){
    var html = getFormHtml();
    $preview.empty().html(html);
    try{ if (window.FormRuntimeHTMLDSL){ window.FormRuntimeHTMLDSL.mount($preview.find('form[data-form]')); } }catch(e){ console.error(e); }
  }

  function getFormHtml(){ var $form = $canvas.find('form').first(); return $form.length ? $form[0].outerHTML : ''; }

  function promptCompositeName(){
    // Collect from page catalog
    var names = []; $('[data-composite-def]').each(function(){ names.push($(this).attr('data-composite-def')); });
    if (names.length === 0){ alert('No composite definitions found.'); return null; }
    var name = prompt('Enter composite name or pick one: \n'+names.join(', '), names[0]);
    if (!name) return null;
    if (names.indexOf(name) === -1) alert('Note: definition not found on page; it may not render until added.');
    return name;
  }

  function uniqueId(prefix){ return prefix + '-' + Math.random().toString(36).slice(2,8); }

  function download(filename, text){
    var a = document.createElement('a'); a.setAttribute('href', 'data:text/html;charset=utf-8,' + encodeURIComponent(text)); a.setAttribute('download', filename); a.style.display='none'; document.body.appendChild(a); a.click(); document.body.removeChild(a);
  }

})(window, jQuery);

