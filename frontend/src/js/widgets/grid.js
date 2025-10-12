/**
 * FrameworkQ.Easyforms - Grid Widget Plugin
 * jQuery plugin for 2D grid widgets with generated rows/columns
 */

(function($) {
    'use strict';

    /**
     * jQuery plugin for form grids
     * Usage: $('table[data-grid]').formGrid()
     */
    $.fn.formGrid = function(options) {
        return this.each(function() {
            var $grid = $(this);

            // Skip if already initialized
            if ($grid.data('formGrid')) {
                return;
            }

            var settings = $.extend({}, $.fn.formGrid.defaults, options);

            // Get grid configuration
            var rowGen = $grid.attr('data-row-gen') || $grid.attr('data-rows');
            var colGen = $grid.attr('data-col-gen') || $grid.attr('data-columns');

            // Generate rows
            if (rowGen) {
                _generateRows($grid, rowGen);
            }

            // Generate columns
            if (colGen) {
                _generateColumns($grid, colGen);
            }

            // Store settings
            $grid.data('formGrid', settings);

            /**
             * Generate rows based on specification
             * @private
             */
            function _generateRows($grid, spec) {
                // Parse spec: "times:HH:mm-HH:mm/step" or "range:start-end/step" or "values:a,b,c"
                var parts = spec.split(':');
                var type = parts[0];
                var config = parts[1];

                var rows = [];

                if (type === 'times' && config) {
                    // Example: "07:00-14:00/60" (hourly from 7am to 2pm)
                    var match = config.match(/^(\d{2}:\d{2})-(\d{2}:\d{2})\/(\d+)$/);
                    if (match) {
                        var start = match[1];
                        var end = match[2];
                        var step = parseInt(match[3]);
                        rows = _generateTimeRange(start, end, step);
                    }
                } else if (type === 'range' && config) {
                    // Example: "1-10/1" (numbers 1 to 10)
                    var match = config.match(/^(\d+)-(\d+)\/(\d+)$/);
                    if (match) {
                        var start = parseInt(match[1]);
                        var end = parseInt(match[2]);
                        var step = parseInt(match[3]);
                        for (var i = start; i <= end; i += step) {
                            rows.push(i.toString());
                        }
                    }
                } else if (type === 'values' && config) {
                    // Example: "a,b,c,d"
                    rows = config.split(',');
                }

                // Generate row elements
                // TODO: Implement actual row generation (requires template)
                console.log('[GridWidget] Generated rows:', rows);
            }

            /**
             * Generate columns based on specification
             * @private
             */
            function _generateColumns($grid, spec) {
                // Parse spec: "days-of-month" or "times:HH:mm-HH:mm/step" or "values:A,B,C"
                var parts = spec.split(':');
                var type = parts[0];
                var config = parts[1];

                var columns = [];

                if (type === 'days-of-month') {
                    // Get context month from data-context-month or form field
                    var contextMonth = $grid.attr('data-context-month');
                    // Generate days 1-31 (simplified - real implementation would get actual days in month)
                    for (var i = 1; i <= 31; i++) {
                        columns.push(i.toString());
                    }
                } else if (type === 'times' && config) {
                    var match = config.match(/^(\d{2}:\d{2})-(\d{2}:\d{2})\/(\d+)$/);
                    if (match) {
                        columns = _generateTimeRange(match[1], match[2], parseInt(match[3]));
                    }
                } else if (type === 'values' && config) {
                    columns = config.split(',');
                }

                // Generate column headers
                // TODO: Implement actual column generation
                console.log('[GridWidget] Generated columns:', columns);
            }

            /**
             * Generate time range
             * @private
             */
            function _generateTimeRange(start, end, stepMinutes) {
                var times = [];
                var startParts = start.split(':');
                var endParts = end.split(':');

                var startMinutes = parseInt(startParts[0]) * 60 + parseInt(startParts[1]);
                var endMinutes = parseInt(endParts[0]) * 60 + parseInt(endParts[1]);

                for (var m = startMinutes; m <= endMinutes; m += stepMinutes) {
                    var hours = Math.floor(m / 60);
                    var minutes = m % 60;
                    times.push(
                        ('0' + hours).slice(-2) + ':' + ('0' + minutes).slice(-2)
                    );
                }

                return times;
            }
        });
    };

    $.fn.formGrid.defaults = {
        autoGenerate: true
    };

})(jQuery);
