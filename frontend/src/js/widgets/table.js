/**
 * FrameworkQ.Easyforms - Table Widget Plugin
 * jQuery plugin for table widget functionality
 */

(function($) {
    'use strict';

    /**
     * jQuery plugin for form tables
     * Usage: $('table[data-table]').formTable()
     */
    $.fn.formTable = function(options) {
        return this.each(function() {
            var $table = $(this);

            // Skip if already initialized
            if ($table.data('formTable')) {
                return;
            }

            var settings = $.extend({}, $.fn.formTable.defaults, options);

            // Initialize using TableManager
            if (window.TableManager) {
                window.TableManager._initializeTable($table);
            }

            // Store settings
            $table.data('formTable', settings);

            // Bind row change events for computed columns
            $table.on('change', 'input, select, textarea', function() {
                var $input = $(this);
                var $row = $input.closest('tr');

                // Recalculate computed columns in this row
                _recalculateRow($table, $row);

                // Recalculate aggregates
                _recalculateAggregates($table);
            });

            /**
             * Recalculate computed columns in a row
             * @private
             */
            function _recalculateRow($table, $row) {
                $row.find('input[data-compute], input[data-formula]').each(function() {
                    var $computed = $(this);
                    var formula = $computed.attr('data-compute') || $computed.attr('data-formula');

                    if (!formula) {
                        return;
                    }

                    // Build context from row values
                    var context = {};
                    $row.find('input, select, textarea').each(function() {
                        var $field = $(this);
                        var colName = $field.attr('data-col-name') || $field.attr('name');
                        if (colName) {
                            // Remove row index suffix from name
                            colName = colName.replace(/_\d+$/, '');
                            var val = $field.val();
                            context[colName] = val === '' ? null : parseFloat(val) || val;
                        }
                    });

                    // Evaluate formula (if expression evaluator available)
                    if (window.ExpressionEvaluator) {
                        try {
                            var result = window.ExpressionEvaluator.evaluate(formula, context);
                            $computed.val(result !== null && result !== undefined ? result : '');
                        } catch (e) {
                            console.error('[TableWidget] Formula evaluation failed:', formula, e);
                        }
                    }
                });
            }

            /**
             * Recalculate aggregate values in footer
             * @private
             */
            function _recalculateAggregates($table) {
                $table.find('tfoot [data-agg]').each(function() {
                    var $cell = $(this);
                    var aggSpec = $cell.attr('data-agg');
                    var targetId = $cell.attr('data-target');

                    if (!aggSpec) {
                        return;
                    }

                    // Parse "sum(column)" or "avg(column)" or "count()"
                    var match = aggSpec.match(/^(\w+)\((\w*)\)$/);
                    if (!match) {
                        return;
                    }

                    var func = match[1].toLowerCase();
                    var colName = match[2];

                    // Collect column values from all rows
                    var values = [];
                    $table.find('tbody tr').each(function() {
                        var $row = $(this);
                        var $input = $row.find('[data-col-name="' + colName + '"]');
                        if ($input.length > 0) {
                            var val = parseFloat($input.val());
                            if (!isNaN(val)) {
                                values.push(val);
                            }
                        }
                    });

                    // Calculate aggregate
                    var result = _calculateAggregate(func, values);

                    // Update target field or cell
                    if (targetId) {
                        var $target = $(targetId);
                        if ($target.is('input, select, textarea')) {
                            $target.val(result !== null ? result : '');
                        } else {
                            $target.text(result !== null ? result : '');
                        }
                    } else {
                        $cell.text(result !== null ? result : '');
                    }
                });
            }

            /**
             * Calculate aggregate function
             * @private
             */
            function _calculateAggregate(func, values) {
                if (values.length === 0 && func !== 'count') {
                    return null;
                }

                switch (func) {
                    case 'sum':
                        return values.reduce((a, b) => a + b, 0);
                    case 'avg':
                        return values.length > 0 ? values.reduce((a, b) => a + b, 0) / values.length : null;
                    case 'min':
                        return values.length > 0 ? Math.min(...values) : null;
                    case 'max':
                        return values.length > 0 ? Math.max(...values) : null;
                    case 'count':
                        return values.length;
                    default:
                        console.warn('[TableWidget] Unknown aggregate function:', func);
                        return null;
                }
            }
        });
    };

    $.fn.formTable.defaults = {
        autoCalculate: true
    };

})(jQuery);
