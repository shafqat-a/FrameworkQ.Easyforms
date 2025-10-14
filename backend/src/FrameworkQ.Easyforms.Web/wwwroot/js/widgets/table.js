/**
 * Copied from frontend/src/js/widgets/table.js for UI hosting
 */
(function($) {
    'use strict';
    $.fn.formTable = function(options) {
        return this.each(function() {
            var $table = $(this);
            if ($table.data('formTable')) return;
            var settings = $.extend({}, $.fn.formTable.defaults, options);
            if (window.TableManager) window.TableManager._initializeTable($table);
            $table.data('formTable', settings);
            $table.on('change', 'input, select, textarea', function() {
                var $row = $(this).closest('tr');
                _recalcRow($table, $row); _recalcAgg($table);
            });
            function _recalcRow($table, $row){ $row.find('input[data-compute], input[data-formula]').each(function(){ var $c=$(this), f=$c.attr('data-compute')||$c.attr('data-formula'); if (!f) return; var ctx={}; $row.find('input, select, textarea').each(function(){ var $f=$(this); var col=$f.attr('data-col-name')||$f.attr('name'); if (col){ col=col.replace(/_\d+$/,''); var v=$f.val(); ctx[col]= v===''? null : parseFloat(v) || v; } }); if (window.ExpressionEvaluator){ try{ var r=window.ExpressionEvaluator.evaluate(f, ctx); $c.val(r ?? ''); } catch(e){ console.error('Formula failed', f, e);} } }); }
            function _recalcAgg($table){ $table.find('tfoot [data-agg]').each(function(){ var $cell=$(this), spec=$cell.attr('data-agg'), targetId=$cell.attr('data-target'); if (!spec) return; var m=spec.match(/^(\w+)\((\w*)\)$/); if(!m) return; var func=m[1].toLowerCase(), col=m[2]; var values=[]; $table.find('tbody tr').each(function(){ var $row=$(this); var $in=$row.find('[data-col-name="'+col+'"]'); if ($in.length>0){ var v=parseFloat($in.val()); if(!isNaN(v)) values.push(v); } }); var r=_calc(func, values); if (targetId){ var $t=$(targetId); if ($t.is('input,select,textarea')) $t.val(r!==null?r:''); else $t.text(r!==null?r:''); } else { $cell.text(r!==null?r:''); } }); }
            function _calc(func, values){ if(values.length===0 && func!=='count') return null; switch(func){ case 'sum': return values.reduce((a,b)=>a+b,0); case 'avg': return values.length>0? values.reduce((a,b)=>a+b,0)/values.length : null; case 'min': return values.length>0 ? Math.min(...values):null; case 'max': return values.length>0 ? Math.max(...values):null; case 'count': return values.length; default: console.warn('Unknown agg', func); return null;} }
        });
    };
    $.fn.formTable.defaults = { autoCalculate:true };
})(jQuery);

