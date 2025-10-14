/* Composite: smart-table
 * Properties (data-prop-*):
 *  - rows: number | JSON array of row objects or labels
 *    row object may include { label: string, rowspan: number }
 *  - columns: number | JSON array of column objects or labels
 *    column object may include { header: string, key: string, colspan: number }
 *  - addrowhtmltemplate: HTML string for a new row (optional)
 */
(function(window, $){
  'use strict';

  function parseProp($inst, name){
    var val = $inst.attr('data-prop-' + name);
    if (!val) return null;
    val = val.trim();
    if (/^\d+$/.test(val)) return parseInt(val, 10);
    try { return JSON.parse(val); } catch { return val; }
  }

  function buildTable($inst){
    if ($inst.data('smartTableEnhanced')) return; // idempotent
    $inst.data('smartTableEnhanced', true);

    var rowsProp = parseProp($inst, 'rows');
    var colsProp = parseProp($inst, 'columns');
    var addTpl = $inst.attr('data-prop-addrowhtmltemplate');

    var rows = normalizeRows(rowsProp);
    var cols = normalizeCols(colsProp);

    var $table = $('<table data-table class="smart-table"></table>');
    var $thead = $('<thead><tr></tr></thead>');
    var $tbody = $('<tbody></tbody>');
    $table.append($thead).append($tbody);

    // Headers
    cols.forEach(function(c){
      var $th = $('<th></th>').text(c.header);
      if (c.colspan && c.colspan > 1) $th.attr('colspan', c.colspan);
      $thead.find('tr').append($th);
    });

    // Body with basic rowspan support for first column; smart inputs with data-state-key
    var rowIndex = 0;
    var logicalRow = 0;
    while (rowIndex < rows.length){
      var r = rows[rowIndex];
      var span = r.rowspan && r.rowspan > 1 ? r.rowspan : 1;
      var $tr = $('<tr></tr>');
      // First cell with potential rowspan
      var firstLabel = r.label != null ? r.label : (typeof r === 'string' ? r : '');
      var $td0 = $('<td></td>').text(firstLabel);
      if (span > 1) $td0.attr('rowspan', span);
      $tr.append($td0);
      // Remaining cells according to columns (excluding first col header)
      for (var ci=1; ci<cols.length; ci++){
        var c = cols[ci];
        var $td = $('<td></td>');
        if (c.colspan && c.colspan > 1) $td.attr('colspan', c.colspan);
        // smart input
        var key = (c.key || ('c'+ci));
        var stateKey = 'r'+logicalRow+'.'+key;
        var $inp = $('<input type="text" class="smart-cell-input" />').attr('data-state-key', stateKey);
        $td.append($inp);
        $tr.append($td);
      }
      $tbody.append($tr);
      // Add span-1 additional rows (skip first cell)
      for (var k=1; k<span && (rowIndex + k) < rows.length; k++){
        var $tr2 = $('<tr></tr>');
        for (var ci2=1; ci2<cols.length; ci2++){
          var c2 = cols[ci2];
          var $td2 = $('<td></td>');
          if (c2.colspan && c2.colspan > 1) $td2.attr('colspan', c2.colspan);
          var key2 = (c2.key || ('c'+ci2));
          var stateKey2 = 'r'+(logicalRow + k)+'.'+key2;
          var $inp2 = $('<input type="text" class="smart-cell-input" />').attr('data-state-key', stateKey2);
          $td2.append($inp2);
          $tr2.append($td2);
        }
        $tbody.append($tr2);
      }
      logicalRow += span;
      rowIndex += span;
    }

    // Add row button if template provided
    if (addTpl && addTpl.length){
      var $btn = $('<button type="button" class="btn btn-secondary" style="margin-top:6px;">+ Add Row</button>');
      $btn.on('click', function(){
        var $row = $(addTpl);
        if ($row.is('tr')){
          // assign state keys to inputs in order of non-first columns
          var cellIdx = 1;
          $row.find('input,select,textarea').each(function(){
            var c = cols[cellIdx] || {};
            var key = c.key || ('c'+cellIdx);
            var stateKey = 'r'+logicalRow+'.'+key;
            $(this).attr('data-state-key', stateKey);
            cellIdx += 1;
          });
          $tbody.append($row);
          logicalRow += 1;
        }
        else {
          var $tr = $('<tr></tr>');
          var $td = $('<td></td>').attr('colspan', totalColspan(cols));
          // state keys inside generic content
          var cellIdx2 = 1;
          $($row).find('input,select,textarea').each(function(){
            var c2 = cols[cellIdx2] || {};
            var key2 = c2.key || ('c'+cellIdx2);
            var stateKey2 = 'r'+logicalRow+'.'+key2;
            $(this).attr('data-state-key', stateKey2);
            cellIdx2 += 1;
          });
          $td.append($row); $tr.append($td); $tbody.append($tr);
          logicalRow += 1;
        }
      });
      $inst.append($btn);
    }

    $inst.prepend($table);
  }

  function normalizeRows(rowsProp){
    if (rowsProp == null) return [ { label: '' } ];
    if (typeof rowsProp === 'number'){ var arr=[]; for (var i=0;i<rowsProp;i++){ arr.push({ label: '' }); } return arr; }
    if (Array.isArray(rowsProp)){
      return rowsProp.map(function(r){
        if (typeof r === 'string') return { label: r };
        if (typeof r === 'object' && r !== null){ return { label: r.label || '', rowspan: r.rowspan }; }
        return { label: '' };
      });
    }
    return [ { label: String(rowsProp) } ];
  }

  function normalizeCols(colsProp){
    if (colsProp == null) return [ { header: '' } ];
    if (typeof colsProp === 'number'){ var arr=[{header:''}]; for (var i=1;i<colsProp;i++){ arr.push({ header: 'Col '+(i+1) }); } return arr; }
    if (Array.isArray(colsProp)){
      return colsProp.map(function(c, idx){
        if (typeof c === 'string') return { header: c, key: 'c'+idx };
        if (typeof c === 'object' && c !== null){ return { header: c.header || ('Col '+(idx+1)), key: c.key || ('c'+idx), colspan: c.colspan }; }
        return { header: 'Col '+(idx+1) };
      });
    }
    return [ { header: String(colsProp) } ];
  }

  function totalColspan(cols){
    return cols.reduce(function(sum, c){ return sum + (c.colspan && c.colspan>1 ? c.colspan : 1); }, 0);
  }

  function enhanceSmartTables($root){
    $root.find('[data-composite="smart-table"]').each(function(){ buildTable($(this)); });
  }

  // Hook into runtime lifecycle
  $(function(){
    // Enhance in any existing forms after runtime mount
    $(document).on('formdsl:ready', function(e, rt){ if (rt && rt.$form) enhanceSmartTables(rt.$form); });
    // Also enhance any present standalone content
    enhanceSmartTables($(document.body));
  });

})(window, jQuery);
