/**
 * Copied from frontend/src/js/table-manager.js for UI hosting
 */
(function(window, $) {
    'use strict';
    window.TableManager = {
        initializeTables: function($form){ $form.find('table[data-table]').each(function(){ TableManager._initializeTable($(this)); }); },
        _initializeTable: function($table){ var tableId=$table.attr('id'); var rowMode=$table.attr('data-row-mode')||'infinite'; var allowAdd=$table.attr('data-allow-add-rows')!=='false'; var allowDelete=$table.attr('data-allow-delete-rows')!=='false'; var $template=$table.find('[data-row-template]').first(); if ($template.length===0){ console.warn('[TableManager] No row template for', tableId); return; }
            $table.data('rowTemplate', $template.clone()); $template.remove(); var minRows=parseInt($table.attr('data-min-rows'))||0; for (var i=0;i<minRows;i++){ this.addRow($table);} if (allowAdd && rowMode==='infinite'){ this._bindAddRowButton($table);} if (allowDelete){ this._bindDeleteRowButtons($table);} },
        addRow: function($table, rowData, index){ var $template=$table.data('rowTemplate'); if (!$template){ console.error('[TableManager] No row template'); return null;} var $newRow=$template.clone(); $newRow.removeAttr('data-row-template');
            var $tbody=$table.find('tbody'); var rowIndex=$tbody.find('tr').length; $newRow.find('input, select, textarea').each(function(){ var $input=$(this); var name=$input.attr('name'); if (name){ $input.attr('name', name + '_' + rowIndex); $input.attr('data-row-index', rowIndex); $input.attr('data-col-name', name);} }); if (rowData){ $newRow.find('input, select, textarea').each(function(){ var $input=$(this); var col=$input.attr('data-col-name'); if (col && rowData[col]!==undefined){ $input.val(rowData[col]); } }); }
            var allowDelete=$table.attr('data-allow-delete-rows')!=='false'; if (allowDelete){ var $firstCell=$newRow.find('td').first(); $firstCell.prepend('<button type="button" class="btn-delete-row" style="margin-right:5px;">×</button>'); }
            if (index!==undefined && index>=0){ var $rows=$tbody.find('tr'); if (index < $rows.length) { $rows.eq(index).before($newRow);} else { $tbody.append($newRow);} } else { $tbody.append($newRow);} $table.trigger('table:row:add', { index: rowIndex, row: $newRow }); return $newRow; },
        removeRow: function($table, index){ var $tbody=$table.find('tbody'); var $row=$tbody.find('tr').eq(index); if ($row.length===0) return; $row.remove(); $table.trigger('table:row:remove', { index: index }); this._recalculateAggregates($table); },
        _bindAddRowButton: function($table){ var self=this; var $btn=$table.find('.btn-add-row, [data-action="add-row"]').first(); if ($btn.length===0){ var $tfoot=$table.find('tfoot'); if ($tfoot.length===0){ $tfoot=$('<tfoot></tfoot>'); $table.append($tfoot);} var colCount=$table.find('thead th').length; $btn=$('<button type="button" class="btn-add-row">+ Add Row</button>'); $tfoot.append($('<tr><td colspan="'+colCount+'"></td></tr>').find('td').append($btn).end()); }
            $btn.off('click.tablemanager').on('click.tablemanager', function(e){ e.preventDefault(); self.addRow($table); }); },
        _bindDeleteRowButtons: function($table){ var self=this; $table.on('click', '.btn-delete-row', function(e){ e.preventDefault(); var $row=$(this).closest('tr'); var index=$row.index(); self.removeRow($table, index); }); },
        _recalculateAggregates: function($table){ $table.trigger('table:aggregates:recalculate'); }
    };
})(window, jQuery);

