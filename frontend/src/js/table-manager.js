/**
 * FrameworkQ.Easyforms - Table Manager
 * Dynamic row addition/removal for data-table widgets
 */

(function(window, $) {
    'use strict';

    /**
     * TableManager - Manages dynamic table rows
     */
    window.TableManager = {
        /**
         * Initialize all tables in the form
         * @param {jQuery} $form - Form element
         */
        initializeTables: function($form) {
            $form.find('table[data-table]').each(function() {
                var $table = $(this);
                TableManager._initializeTable($table);
            });
        },

        /**
         * Initialize a single table
         * @private
         */
        _initializeTable: function($table) {
            var tableId = $table.attr('id');
            var rowMode = $table.attr('data-row-mode') || 'infinite';
            var allowAdd = $table.attr('data-allow-add-rows') !== 'false';
            var allowDelete = $table.attr('data-allow-delete-rows') !== 'false';

            // Find row template
            var $template = $table.find('[data-row-template]').first();
            if ($template.length === 0) {
                console.warn('[TableManager] No row template found for table:', tableId);
                return;
            }

            // Store template and remove from DOM
            $table.data('rowTemplate', $template.clone());
            $template.remove();

            // Add initial rows if needed
            var minRows = parseInt($table.attr('data-min-rows')) || 0;
            for (var i = 0; i < minRows; i++) {
                this.addRow($table);
            }

            // Bind add row button (if exists or auto-create)
            if (allowAdd && rowMode === 'infinite') {
                this._bindAddRowButton($table);
            }

            // Bind delete row buttons
            if (allowDelete) {
                this._bindDeleteRowButtons($table);
            }

            console.log('[TableManager] Initialized table:', tableId);
        },

        /**
         * Add a row to the table
         * @param {jQuery} $table - Table element
         * @param {Object} rowData - Initial row data (optional)
         * @param {number} index - Insert position (optional, default: append)
         */
        addRow: function($table, rowData, index) {
            var $template = $table.data('rowTemplate');
            if (!$template) {
                console.error('[TableManager] No row template for table');
                return null;
            }

            // Clone template
            var $newRow = $template.clone();
            $newRow.removeAttr('data-row-template');

            // Get current row count
            var $tbody = $table.find('tbody');
            var rowIndex = $tbody.find('tr').length;

            // Update input names with row index
            $newRow.find('input, select, textarea').each(function() {
                var $input = $(this);
                var name = $input.attr('name');
                if (name) {
                    // Add row index to name (e.g., "voltage" -> "voltage_0")
                    $input.attr('name', name + '_' + rowIndex);
                    $input.attr('data-row-index', rowIndex);
                    $input.attr('data-col-name', name);
                }
            });

            // Populate row data if provided
            if (rowData) {
                $newRow.find('input, select, textarea').each(function() {
                    var $input = $(this);
                    var colName = $input.attr('data-col-name');
                    if (colName && rowData[colName] !== undefined) {
                        $input.val(rowData[colName]);
                    }
                });
            }

            // Add delete button to first cell if allowed
            var allowDelete = $table.attr('data-allow-delete-rows') !== 'false';
            if (allowDelete) {
                var $firstCell = $newRow.find('td').first();
                $firstCell.prepend('<button type="button" class="btn-delete-row" style="margin-right: 5px;">×</button>');
            }

            // Insert row
            if (index !== undefined && index >= 0) {
                var $rows = $tbody.find('tr');
                if (index < $rows.length) {
                    $rows.eq(index).before($newRow);
                } else {
                    $tbody.append($newRow);
                }
            } else {
                $tbody.append($newRow);
            }

            // Trigger event
            $table.trigger('table:row:add', { index: rowIndex, row: $newRow });

            return $newRow;
        },

        /**
         * Remove a row from the table
         * @param {jQuery} $table - Table element
         * @param {number} index - Row index
         */
        removeRow: function($table, index) {
            var $tbody = $table.find('tbody');
            var $row = $tbody.find('tr').eq(index);

            if ($row.length === 0) {
                return;
            }

            $row.remove();

            // Trigger event
            $table.trigger('table:row:remove', { index: index });

            // Recalculate aggregates
            this._recalculateAggregates($table);
        },

        /**
         * Bind add row button
         * @private
         */
        _bindAddRowButton: function($table) {
            var self = this;

            // Look for existing add button
            var $addBtn = $table.find('.btn-add-row, [data-action="add-row"]').first();

            // Create add button if not exists
            if ($addBtn.length === 0) {
                var $tfoot = $table.find('tfoot');
                if ($tfoot.length === 0) {
                    $tfoot = $('<tfoot></tfoot>');
                    $table.append($tfoot);
                }

                var colCount = $table.find('thead th').length;
                $addBtn = $('<button type="button" class="btn-add-row">+ Add Row</button>');
                $tfoot.append($('<tr><td colspan="' + colCount + '"></td></tr>').find('td').append($addBtn).end());
            }

            // Bind click
            $addBtn.off('click.tablemanager').on('click.tablemanager', function(e) {
                e.preventDefault();
                self.addRow($table);
            });
        },

        /**
         * Bind delete row buttons
         * @private
         */
        _bindDeleteRowButtons: function($table) {
            var self = this;

            $table.on('click', '.btn-delete-row', function(e) {
                e.preventDefault();
                var $row = $(this).closest('tr');
                var index = $row.index();
                self.removeRow($table, index);
            });
        },

        /**
         * Recalculate aggregate values
         * @private
         */
        _recalculateAggregates: function($table) {
            // This will be implemented when expression evaluator is integrated (T073-T075)
            $table.trigger('table:aggregates:recalculate');
        }
    };

})(window, jQuery);
